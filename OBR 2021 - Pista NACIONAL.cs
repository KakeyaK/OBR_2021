string estagio = "Pista";



// pista.escuroLinha

// ===== Calibrações =====
// Sensibilidade no giro de 90º
// Sensibiliadde no Preto2
// Sensibilidade nos retornos para o preto

void Main() 
{   
    pista pista = new pista(30); // Sensibilidade do giro de 90º. > "escuroPreto2" !!!
    pista.escuroLinha = 25; pista.escuroPreto2 = 25; 

    bc.SetPrecision(4); 
    bc.ColorSensibility(25); // 0 - 100, default = 33
    bc.ActuatorSpeed(150); // 0 - 150

    mov.MoverEscavadora(85);
    int counter = 0;

    // Pista
    while (estagio == "Pista")
    {    

        // bc.PrintConsole(0, "CE: " + bc.ReturnColor(0) + " CD: " + bc.ReturnColor(1));
        if(aux.MedirLuz(1) < pista.escuroPreto2 && aux.MedirLuz(2) < pista.escuroPreto2){
            aux.Preto2();
        }

        // Verde direita
        if(bc.ReturnColor(1 - 1) == "GREEN"){
            pista.GirarVerde("direita");
        }
        // Verde Esquerda
        if(bc.ReturnColor(2 - 1) == "GREEN"){
            pista.GirarVerde("esquerda");
        }

        pista.Gangorra();

        pista.SeguirLinhaPID(150, 20, 0.03f, 20);
    }    
}

class pista
{    

    public static int escuroLinha, escuroPreto2;
    private float sensibilidade90;

    // Variáveis PID
    private float error = 0, lastError = 0, integral = 0, derivate = 0;
    private float movimento;
    private bool pararIntegro;
    
    // Variáveis Gangorra
    public static int counter = 0;
    public static int inclinacaoAtual = 360, inclinacaoAntiga = 360;

    // Método construtor
    public pista(float psensibilidade90 = 45){
        sensibilidade90 = psensibilidade90;
    }
    
    public void SeguirLinhaPID(float velocidade, float kp, float ki, float kd)
    {
        // matemática PID
        error = aux.MedirLuz(1) - aux.MedirLuz(2);
        integral += error;
        derivate = error - lastError;

        if(integral * kp > 1000 - velocidade) integral = 1000 - velocidade;

        if (pararIntegro)
        {
            movimento = error * kp + derivate * kd;
        }
        else
        {
            movimento = error * kp + integral * ki + derivate * kd;
        }

        // Controle de erros do integro
        pararIntegro = ((Math.Abs(error) < 10 && Math.Abs(integral) > 250) ||
                       ((error > 0 && movimento > 0) || (error < 0 && movimento < 0)) && 
                       (Math.Abs(movimento) > 1000 - velocidade));


        if (movimento > 1000 - velocidade) { movimento = 1000 - velocidade; }
        if (movimento < velocidade - 1000) { movimento = velocidade - 1000; }

        // Console
        bc.PrintConsole(1, "Err: " + error.ToString("F") + " lErr: " + lastError.ToString("F") + " M: " + movimento.ToString());
        // bc.PrintConsole(0, "Integral: " + integral.ToString() + " Derivate: " + derivate.ToString("F") + " Integro Parado: " + pararIntegro.ToString());

        // Movimento
        bc.Move(velocidade - movimento, velocidade + movimento);
        aux.Tick();
        
        string giro;
        if(error > sensibilidade90 && Math.Abs(error - lastError) < 1){
            giro = aux.tipoGiro90(2);
            bc.PrintConsole(3, "Tipo do giro: " + giro.ToString());
            pista.Girar90("esquerda", giro);
        }
        else if(error < -sensibilidade90 && Math.Abs(error - lastError) < 1){
            giro = aux.tipoGiro90(1);
            bc.PrintConsole(3, "Tipo do giro: " + giro.ToString());
            pista.Girar90("direita", giro);
        }

        // Atualização de variável
        lastError = error;
    }

    static public void Girar90(string lado, string giro){
        /*
            Giro em situações onde a ângulação do robô com a linha preta está muito alta.

            A partir da identificação de uma angulação alta (circulo) ou muito alta (quadrado) giro90 pode assumir duas formas:

            quadrado - anda pra frente e gira. O movimento pra frente ajuda no posicionamento e na detecção de linha preta para parar.
                       Caso o giro passe de um limite ele retorna, mantendo uma posição de 90º com a linha.


            circulo - não anda pra frente. Gira apenas o necessário para se alinhar. 
        */
        bc.Move(0, 0);
        aux.Tick();

        if(giro == "quadrado"){
            mov.MoverPorUnidadeRotacao(8);

            float anguloInicial = bc.Compass();
            float anguloGiro; // usado para verificar se é necessário retornar
            bool retornando = false; // Controle para verificar se o giro passou do limite adequado

            if(lado == "esquerda"){
                bc.PrintConsole(2, "Giro para esquerda");
                
                // mov.MoverNoCirculo(-5);

                while(aux.MedirLuz(2) > escuroLinha){
                    anguloGiro = matAng.MatematicaCirculo(anguloInicial - bc.Compass());
                    if(anguloGiro > 160 && anguloGiro < 165){
                        bc.Move(0, 0);
                        aux.Tick();
                        bc.PrintConsole(2, "Retornando giro para esquerda " + anguloGiro.ToString());
                        mov.MoverNoCirculo(75);
                        retornando = true;
                        break;
                    }
                    bc.Move(-970, 970);
                }

                if(retornando == false){
                    mov.MoverNoCirculo(-16);
                }
            }
            
            else if(lado == "direita"){
                bc.PrintConsole(2, "Giro para direita");

                // mov.MoverNoCirculo(5);
                
                while(aux.MedirLuz(1) > escuroLinha){
                    anguloGiro = matAng.MatematicaCirculo(bc.Compass() - anguloInicial);
                    if(anguloGiro > 160 && anguloGiro < 165){
                        bc.PrintConsole(2, "Retornando giro para direita " + anguloGiro.ToString());
                        mov.MoverNoCirculo(-75);
                        retornando = true;
                        break;
                    }
                    bc.Move(970, -970);
                }

                if(retornando == false){
                    mov.MoverNoCirculo(16);
                }
            }
        }
        else{
            float anguloInicial;
            float anguloGiro;

            anguloInicial = bc.Compass();

            if(lado == "esquerda"){
                bc.PrintConsole(2, "Giro para esquerda");
                do{
                    bc.Move(-900, 900);
                    if(bc.ReturnColor(0) == "Green") pista.GirarVerde("direita");
                    if(aux.MedirLuz(0) < pista.escuroLinha) break;

                    anguloGiro = matAng.MatematicaCirculo(anguloInicial - bc.Compass()); 
                }
                while(anguloGiro < 5 || anguloGiro > 7);
            }
            else{
                bc.PrintConsole(2, "Giro para direita");
                do{
                    bc.Move(900, -900);
                    if(bc.ReturnColor(1) == "Green") pista.GirarVerde("esquerda");
                    if(aux.MedirLuz(1) < pista.escuroLinha) break;

                    anguloGiro = matAng.MatematicaCirculo(bc.Compass() - anguloInicial); 
                }
                while(anguloGiro < 5 || anguloGiro > 7);
            }
        }
    }

    static public void GirarVerde(string lado){
        /*
            Segue os comandos devidos quando o robô encontra um quadrado verde.
        */
        bc.Move(0, 0);
        aux.Tick();

        bc.Move(200, 200);
        bc.wait(500);
        
        if(lado == "esquerda"){
            bc.PrintConsole(2, "Verde para esquerda");
            mov.MoverNoCirculo(-30);
            while(aux.MedirLuz(2) > escuroLinha){
                bc.Move(-970, 970);
            }
            mov.MoverNoCirculo(-14);
        }
        
        else if(lado == "direita"){
            bc.PrintConsole(2, "Verde para direita");
            mov.MoverNoCirculo(30);
            while(aux.MedirLuz(1) > escuroLinha){
                bc.Move(970, -970);
            }
            mov.MoverNoCirculo(14);
        }
    }

    static public void Gangorra(){
        // bc.PrintConsole(2, counter.ToString() + " " + inclinacaoAtual.ToString("F2") + " " + inclinacaoAntiga.ToString("F2" ));

        if(counter == 20){
            inclinacaoAntiga = inclinacaoAtual;

            inclinacaoAtual = (int)bc.Inclination();
            if(inclinacaoAtual > -1 && inclinacaoAtual < 23) inclinacaoAtual += 360;

            counter = 0;
        }
        else{
            counter++;
        }

        if(inclinacaoAtual > inclinacaoAntiga + 5){
            bc.PrintConsole(1, "Gangorra esperando");
            
            bc.Move(0, 0);
            aux.Tick();
        }
    }

    static public void desvio(){

    }
}

class matAng
{
    /*
    ====== Funções de Matemática com Ângulos ======
    - AproximarAngulo
    - MatematicaCirculo
    */ 
    
    static public int AproximarAngulo(float angulo, int aproximacao = 1)
    {
        if(aproximacao == 1){
            // Retornar aproximação de ângulo para um dos pontos cardeais
            if (angulo >= 315 || angulo < 45) return 0;
            if (angulo >= 45 && angulo < 135) return 90;
            if (angulo >= 135 && angulo < 225) return 180;
            if (angulo >= 225 && angulo < 315) return 270;
        }
        else if(aproximacao == 2){
            // Retornar aproximação de ângulo em intervalos de 45º
            if (angulo >= 337.5 && angulo < 22.5) return 0;
            if (angulo >= 22.5 && angulo < 67.5) return 45;
            if (angulo >= 67.5 && angulo < 112.5) return 90;
            if (angulo >= 112.5 && angulo < 157.5) return 135;
            if (angulo >= 157.5 && angulo < 202.5) return 180;
            if (angulo >= 202.5 && angulo < 247.5) return 225;
            if (angulo >= 247.5 && angulo < 292.5) return 270;
            if (angulo >= 292.5 && angulo < 337.5) return 315;
        }

        return 0;
    }
    
    static public float MatematicaCirculo(float angulo)
    {
        /*
            Faz matemática em ciclo, retornando o valor de deslocamento no ciclo trigonométrico.
            Em resumo retorna a distância angular entre um ângulo específico e o ângulo 0
        */
        
        if (angulo >= 360)
        {
            return angulo - 360;
        }
        else if (angulo < 0)
        {

            return (float)(-1 * 360 * Math.Floor((double)(angulo / 360)) + angulo);
        }
        else
        {
            return angulo;
        }
    }


}

class aux
{   
    /*
    ====== Funções Auxiliares ======
    - Tick
    - MedirLuz
    */ 
    static public bool contador = false;

    static public void Tick()
    {
        /*
            Tempo de espera entre ações na programação
        */
        bc.Wait(10);
    }

    static public float MedirLuz(int sensor)
    {
        /* 
            Filtro da função de medir luz do robô
        */
        if(bc.Lightness(sensor - 1) > 65){
            return 65;
        }
        else{
            return bc.Lightness(sensor - 1);
        }
    }

    static public void Preto2(){
        /*
            Realiza uma rotina de procurar o quadrado verde caso encontre preto nos dois sensores.

            Resolve o problema do robô estar muito para um lado da linha e passar direto pelo quadrado verde.
            Os valores de VerificarVerde() podem ser auterados para aumentar a detecção. É necessário tomar cuidado para que o robô não gire demais e encontre o quadrado verde com o sensor errado.
        */

        bc.PrintConsole(2, "Preto2");
        bc.Move(0, 0);
        aux.Tick();

        if(aux.VerificarVerde(10) == false){
            bc.Move(0, 0);
            aux.Tick();
            mov.MoverPorUnidadeRotacao(-3.5f);
        
            if(aux.VerificarVerde(10) == false){
                bc.Move(0, 0);
                aux.Tick();
                if(contador == true){
                    contador = false;
                    mov.MoverPorUnidadeRotacao(10);
                }
                else{
                    contador = true;
                }
            }
        }
    }

    static public bool VerificarVerde(int area){
        /*
            Rotina para verificar a linha verde. Salva a posição orignal para o robô poder voltar para onde começou e gira em sentido horário, anti horário em dobro e horário para voltar para o início.

            Auxilia a encontrar quadrados verdes com mais precisão
        */

        bc.PrintConsole(2, "Verificando a linha verde");
        bc.Move(0, 0);
        aux.Tick();
        string lado;

        float anguloInicial = bc.Compass();

        lado = VerificarVerdeMov(area, "horario");

        if(lado == "direita" || lado == "esquerda"){
            mov.MoverProAngulo(anguloInicial);
            pista.GirarVerde(lado);
            return true;
        }

        lado = VerificarVerdeMov(area*2, "antihorario");
        
        if(lado == "direita" || lado == "esquerda"){
            mov.MoverProAngulo(anguloInicial);
            pista.GirarVerde(lado);
            return true;
        }

        lado = VerificarVerdeMov(area, "horario");
        
        if(lado == "direita" || lado == "esquerda"){
            mov.MoverProAngulo(anguloInicial);
            pista.GirarVerde(lado);
            return true;
        }
        
        return false;
    }

    static public string VerificarVerdeMov(int area, string sentido){
        /*
            Rotina de movimentação da verificação de verde. Gira para os dois lados procurando o quadrdo verde.

            Auxilia a encontrar quadrados verdes com mais precisão
        */
        bc.Move(0, 0);
        aux.Tick();
        float anguloInicial = bc.Compass();
        float anguloGiro;
        
        if(sentido == "horario"){
            // Sentido horário areaº
            anguloGiro = matAng.MatematicaCirculo(bc.Compass() - anguloInicial); 
            while((anguloGiro < area || anguloGiro > area + 2) && (MedirLuz(2) > pista.escuroLinha)){
                if(bc.ReturnColor(0) == "GREEN"){
                    return "direita";
                }
                else if(bc.ReturnColor(1) == "GREEN"){
                    return "esquerda";
                }
                bc.Move(850, -850);
                anguloGiro = matAng.MatematicaCirculo(bc.Compass() - anguloInicial); 
            }
        }
        if(sentido == "antihorario"){
            // Sentido anti horário areaº
            anguloGiro = matAng.MatematicaCirculo(anguloInicial - bc.Compass()); 
            while((anguloGiro < area || anguloGiro > area + 2) && (MedirLuz(1) > pista.escuroLinha)){
                if(bc.ReturnColor(0) == "GREEN"){
                    return "direita";
                }
                else if(bc.ReturnColor(1) == "GREEN"){
                    return "esquerda";
                }
                bc.Move(-850, 850);
                anguloGiro = matAng.MatematicaCirculo(anguloInicial - bc.Compass()); 
            }
        }

        return "";
    }

    static public string tipoGiro90(int sensor){
        /* 
        Função auxiliar para identificar se o giro é a partir de uma peça quadrada ou circular. Utiliza o fato de que uma deteção de uma peça circular tem mais linha preta na frente caso o robÔ continue andando.

        É utilizada para o robô não andar pra frente quando estiver percorrendo círculos, facilitando assim a leitura dos quadrados verdes.
        */

        bc.Move(0, 0);
        aux.Tick();
        bc.MoveFrontalRotations(100, 0.5f);
        if(sensor == 1){
            mov.MoverNoCirculo(10);
            if(aux.MedirLuz(sensor) < 15){
                mov.MoverNoCirculo(-10);
                return "quadrado";
            }
            else{
                return "circulo";
            }
        }
        else if(sensor == 2){
            mov.MoverNoCirculo(-10);
            if(aux.MedirLuz(sensor) < 15){
                mov.MoverNoCirculo(10);
                return "quadrado";
            }
            else{
                return "circulo";
            }
        }
        return "";
    }
}

class mov
{
    /*
    ====== Funções de Movimento ======
    - MoverUltra
    - MoverPorUnidade
    - MoverBalde
    - MoverEscavadora
    - MoverNoCirculo
    - MoverProAngulo
    */ 

    static public void MoverUltra(float distance, int velocidade)
    {
        /*
        Se movimenta até a distância desejada usando sensor de ultrassom
        */
        bc.Move(0, 0);
        aux.Tick();

        if(bc.Distance(1 - 1) > distance){
            while(bc.Distance(1 - 1) > distance){
                bc.MoveFrontal(velocidade, velocidade);
            }
        }
        else{
            while(bc.Distance(1 - 1) < distance){
                bc.MoveFrontal(-velocidade, -velocidade);
            }
        }
        bc.MoveFrontal(0, 0);
    }

    static public void MoverPorUnidadeRotacao(float distancia)
    {
        /*
            Utiliza distância por rotações para mover o robô uma quantidade determinada. 1 rotação ~= 2,066 zm
        */
        bc.Move(0, 0);
        aux.Tick();

        if (distancia > 0)
        {
            bc.MoveFrontalRotations(200, (float) (distancia / 2.066));
        }
        else
        {
            bc.MoveFrontalRotations(-200, (float) (Math.Abs(distancia) / 2.066));
            
        }
        bc.Move(0, 0);

    }

    static public void MoverBalde(float alvoBalde)
    {   
        /*
        Move o balde do robô até o ângulo desejado
        */
        // Verifica se o ângulo desejado está no intervalo do que o robô consegue alcançar.
        if(alvoBalde > 0 && alvoBalde < 12){
            if (bc.AngleScoop() > alvoBalde)
            {
                while (bc.AngleScoop() > alvoBalde)
                {
                    bc.TurnActuatorUp(30);
                }
            }

            else
            {
                //enquanto o seno da posicao atual da escavadora for maior q o seno da posicao alvo, a escavadora desce
                while (bc.AngleScoop() < alvoBalde)
                {
                    bc.TurnActuatorDown(30);
                }
            }
        }
    }

    static public void MoverEscavadora(float alvoEscavadora)
    {   
        /*
        Move o braço da escavadora até o ângulo desejado
        */

        // Verifica se o ângulo desejado está no intervalo do que o robô consegue alcançar.
        if(alvoEscavadora >= 0 && alvoEscavadora < 90){
            if (bc.AngleActuator() > alvoEscavadora)
            {
                while (bc.AngleActuator() > alvoEscavadora)
                {
                    bc.ActuatorDown(10);
                }
            }

            else
            {
                //enquanto o seno da posicao atual da escavadora for maior q o seno da posicao alvo, a escavadora desce
                while (bc.AngleActuator() < alvoEscavadora)
                {
                    bc.ActuatorUp(10);
                }
            }
        }
    }

    static public void MoverNoCirculo(float anguloMovimento, float velocidade = 950)
    {
        /*
        Girar por graus, independente da orientação
        positivo para sentido horário
        negativo para sentido anti-horário
        Margem de erro ~= 2º
        Máximo de movimento em uma direção = 355
        */
        bc.Move(0, 0);
        bc.Wait(10);

        // Verificar se passou por cima de uma linha
        bool linha = false;    

        float anguloInicial = bc.Compass();
        float anguloGiro;

        if(anguloMovimento > 0){
            do{
                bc.Move(velocidade, -velocidade);

                anguloGiro = matAng.MatematicaCirculo(bc.Compass() - anguloInicial); 
            }
            while(anguloGiro < anguloMovimento || anguloGiro > anguloMovimento + 2);  
        }
        else if(anguloMovimento < 0){
            anguloMovimento = Math.Abs(anguloMovimento);
            do{
                bc.Move(-velocidade, velocidade);

                anguloGiro = matAng.MatematicaCirculo(anguloInicial - bc.Compass()); 
            }
            while(anguloGiro < anguloMovimento || anguloGiro > anguloMovimento + 2);  
        }

        bc.MoveFrontal(0, 0);
    }

    static public void MoverProAngulo(float angulo, float velocidade = 950)
    {   
        /*
        Se locomove até o ângulo desejado. 
        Apenas valores positivos
        */
        bc.Move(0, 0);
        aux.Tick();

        if(angulo == 360) angulo = 0;

        if(angulo >= 0 && angulo < 360){
            float anguloDiferenca = matAng.MatematicaCirculo(angulo - bc.Compass());
            
            if (anguloDiferenca < 180)
            {
                //girar no sentido horário
                MoverNoCirculo(anguloDiferenca, velocidade);
            }

            else
            {
                //girar no sentido anti-horário
                MoverNoCirculo(anguloDiferenca - 360, velocidade);
            }
        }

        bc.MoveFrontal(0, 0);
    }

}

