string estagio = "pista";

void Main() 
{   
    // ===== Calibrações =====
    pista pista = new pista(35, // Sensibilidade do giro de 90º. > "escuroPreto2" !!!  (quanto maior menos sensível)
                            5); // Sensibilidade de erro para a recuperacao de linha (quanto maior mais sensível) 
    pista.escuroLinha = 25; pista.escuroPreto2 = 35; pista.escuroIdentLinha = 45; 

    bc.SetPrecision(4); 
    // Importante calibrar cores para diferênciar kit de resgate do obstáculo
    bc.ColorSensibility(30); // 0 - 100, default = 33
    bc.ActuatorSpeed(150); // 0 - 150

    // !Lembrar de calibrar a deteccao do resgate

    // ===== Começo ======
    mov.MoverEscavadora(85);

    while(true){

        while (estagio == "pista")
        {    
            // bc.PrintConsole(0, "CE: " + bc.ReturnColor(0) + " CD: " + bc.ReturnColor(1));

            if(pista.VerResgate()){
                bc.PrintConsole(2, "Começando o resgate");
                estagio = "resgate";
                break;
            }

            if(aux.MedirLuz(1) < pista.escuroPreto2 && aux.MedirLuz(2) < pista.escuroPreto2){
                aux.Preto2();
            }

            // Verde ambos
            if((bc.ReturnColor(1 - 1) == "GREEN" || bc.ReturnColor(1 - 1) == "YELLOW" || bc.ReturnColor(1 - 1) == "CYAN") && 
               (bc.ReturnColor(2 - 1) == "GREEN" || bc.ReturnColor(2 - 1) == "YELLOW" || bc.ReturnColor(2 - 1) == "CYAN"))
            {
                pista.GirarVerde("ambos");
            }
            // Verde direita
            else if(bc.ReturnColor(1 - 1) == "GREEN" || bc.ReturnColor(1 - 1) == "YELLOW" || bc.ReturnColor(1 - 1) == "CYAN"){
                if(aux.ProcurarMaisVerde("direita")) pista.GirarVerde("ambos");
                else pista.GirarVerde("direita");
            }
            // Verde Esquerda
            else if(bc.ReturnColor(2 - 1) == "GREEN" || bc.ReturnColor(2 - 1) == "YELLOW" || bc.ReturnColor(2 - 1) == "CYAN"){
                if(aux.ProcurarMaisVerde("esquerda")) pista.GirarVerde("ambos");
                else pista.GirarVerde("esquerda");
            }

            if(bc.Distance(0) > 14 && bc.Distance(0) <= 20){
                pista.Desvio();
            }

            if(bc.ReturnColor(2) == "CYAN"){
                pista.PegarKit();
            }

            aux.ContarGangorra();
            pista.Gangorra();

            pista.SeguirLinhaPID(150, 22, 0.03f, 20);
        }    

        while (estagio == "resgate"){
            bc.Move(0, 0);
            bc.Wait(10);
        }

        while(estagio == "pistaFinal"){
            if(aux.MedirLuz(1) < pista.escuroPreto2 && aux.MedirLuz(2) < pista.escuroPreto2){
                aux.Preto2();
            }

            // Verde direita
            if(bc.ReturnColor(1 - 1) == "GREEN" || bc.ReturnColor(1 - 1) == "YELLOW" || bc.ReturnColor(1 - 1) == "CYAN"){
                pista.GirarVerde("direita");
            }
            // Verde Esquerda
            if(bc.ReturnColor(2 - 1) == "GREEN" || bc.ReturnColor(2 - 1) == "YELLOW" || bc.ReturnColor(2 - 1) == "CYAN"){
                pista.GirarVerde("esquerda");
            }

            if(bc.Distance(0) <= 20){
                pista.Desvio();
            }

            if(bc.ReturnColor(0) == "RED" && bc.ReturnColor(1) == "RED"){
                bc.Move(0, 0);
                bc.Wait(1000000);
            }

            pista.Gangorra();

            pista.SeguirLinhaPID(150, 22, 0.03f, 20);

        }

    }

}


class pista
{    

    public static int escuroLinha, escuroPreto2, escuroIdentLinha;
    private float sensibilidade90;

    // Variáveis PID
    private float error = 0, lastError = 0, integral = 0, derivate = 0;
    private float movimento;
    private bool pararIntegro;

    // Variáveis recuperar linha 
    public int counterRecuperacao = 0;
    private int sensibilidadeErroRecuperacao;

    // Variáveis Gangorra
    public static int counterGangorra = 0;
    public static int inclinacaoAtual = 360, inclinacaoAntiga = 360, inclinacaoAntigassa = 360;

    // Método construtor
    public pista(float psensibilidade90 = 45, int precuperacao = 5){
        sensibilidade90 = psensibilidade90;
        sensibilidadeErroRecuperacao = precuperacao;
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
        bc.PrintConsole(0, counterRecuperacao.ToString());
        bc.PrintConsole(1, "Err: " + error.ToString("F") + " lErr: " + lastError.ToString("F") + " M: " + movimento.ToString());
        // bc.PrintConsole(0, "Integral: " + integral.ToString() + " Derivate: " + derivate.ToString("F") + " Integro Parado: " + pararIntegro.ToString());

        // Movimento
        bc.Move(velocidade - movimento, velocidade + movimento);
        aux.Tick();
        
        // Atualização de variável
        lastError = error;

        // ===== GIROS ======
        string giro;
        // Lado esquerdo
        if(error > sensibilidade90 && Math.Abs(error - lastError) < 1){
            giro = aux.tipoGiro90("esquerda");
            bc.PrintConsole(2, "Tipo do giro: " + giro);
            if(giro == "preto2"){
                aux.Preto2();
                return;
            } 
            pista.Girar90("esquerda", giro);
        }
        // Lado direito
        else if(error < -sensibilidade90 && Math.Abs(error - lastError) < 1){
            giro = aux.tipoGiro90("direita");
            bc.PrintConsole(2, "Tipo do giro: " + giro);
            if(giro == "preto2"){
                aux.Preto2();
                return;
            } 
            pista.Girar90("direita", giro);
        }


        // ==== Recuperacao de Linha ====
        /*
            Conta o número de loopings sem erro. Depois de um determinado número procurar linha para ver se perdeu.
            Caso tenha se perdido volta o caminho e alinha aproximando o ângulo
        */
        float anguloGiro, anguloInicial;

        if(Math.Abs(error) < sensibilidadeErroRecuperacao) counterRecuperacao++;
        else counterRecuperacao = 0;
        
        if(counterRecuperacao == 100){
            counterRecuperacao = 0;

            anguloInicial = bc.Compass();
            do{
                bc.Move(900, -900);

                anguloGiro = matAng.MatematicaCirculo(bc.Compass() - anguloInicial);

                if(aux.MedirLuz(1) < pista.escuroLinha || aux.MedirLuz(2) < pista.escuroLinha){
                    bc.PrintConsole(1, "Ainda to na linha!");
                    mov.MoverNoCirculo(-anguloGiro);
                    return;
                }
            }
            while(anguloGiro < 22 || anguloGiro > 25);
            
            bc.PrintConsole(1, "Me perdi!");
            mov.MoverNoCirculo(-anguloGiro);

            bc.MoveFrontalRotations(-150, 25);

            mov.MoverProAngulo(matAng.AproximarAngulo(bc.Compass()));
        }
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

            if(lado == "esquerda"){
                bc.PrintConsole(2, "Giro para esquerda");
                
                do{
                    bc.Move(-900, 900);

                    if(aux.MedirLuz(2) < escuroLinha){
                        mov.MoverNoCirculo(-16);
                        return;
                    }
                    if(aux.MedirLuz(1) < escuroLinha){
                        mov.MoverNoCirculo(10);
                        return;
                    }

                    anguloGiro = matAng.MatematicaCirculo(anguloInicial - bc.Compass());
                }
                while(anguloGiro < 145 || anguloGiro > 147);

                bc.PrintConsole(2, "Retornando giro para esquerda " + anguloGiro.ToString());
                mov.MoverNoCirculo(75);
            }
            
            else if(lado == "direita"){
                bc.PrintConsole(2, "Giro para direita");

                do{
                    bc.Move(900, -900);

                    if(aux.MedirLuz(1) < escuroLinha){
                        mov.MoverNoCirculo(16);
                        return;
                    }
                    if(aux.MedirLuz(2) < escuroLinha){
                        mov.MoverNoCirculo(-10);
                        return;
                    }

                    anguloGiro = matAng.MatematicaCirculo(bc.Compass() - anguloInicial);
                }
                while(anguloGiro < 145 || anguloGiro > 147);

                bc.PrintConsole(2, "Retornando giro para direita " + anguloGiro.ToString());
                mov.MoverNoCirculo(-75);
            }
        }
        else if(giro == "circulo"){
            float anguloInicial;
            float anguloGiro = 0;

            anguloInicial = bc.Compass();

            if(lado == "esquerda"){
                bc.PrintConsole(2, "Giro para esquerda");

                do{
                    bc.Move(-900, 900);

                    if(aux.MedirLuz(1) < pista.escuroLinha) break;
                    if(bc.ReturnColor(0) == "GREEN"){
                        pista.GirarVerde("direita");
                        return;
                    }
                    if(bc.ReturnColor(1) == "GREEN"){
                        pista.GirarVerde("esquerda");
                        return;
                    }

                    anguloGiro = matAng.MatematicaCirculo(anguloInicial - bc.Compass()); 
                }
                while(anguloGiro < 8 || anguloGiro > 13);


            }
            else{
                bc.PrintConsole(2, "Giro para direita");

                do{
                    bc.Move(900, -900);

                    if(aux.MedirLuz(2) < pista.escuroLinha) break;
                    if(bc.ReturnColor(0) == "GREEN"){
                        pista.GirarVerde("direita");
                        return;
                    }
                    if(bc.ReturnColor(1) == "GREEN"){
                        pista.GirarVerde("esquerda");
                        return;
                    }

                    anguloGiro = matAng.MatematicaCirculo(bc.Compass() - anguloInicial); 
                }
                while(anguloGiro < 8 || anguloGiro > 13);
            }
        }

        // Utiliza os giros como uma medida de andamento da pista para resetar o contador de preto2
        aux.contadorPreto2 = false;
    }

    static public void GirarVerde(string lado){
        /*
            Segue os comandos devidos quando o robô encontra um quadrado verde.
        */
        bc.Move(0, 0);
        aux.Tick();

        bc.MoveFrontalRotations(200, 13);
        
        if(lado == "esquerda"){
            bc.PrintConsole(2, "Verde para esquerda");
            mov.MoverNoCirculo(-15);
            while(aux.MedirLuz(2) > escuroLinha){
                bc.Move(-970, 970);
            }
            mov.MoverNoCirculo(-10);
        }
        
        else if(lado == "direita"){
            bc.PrintConsole(2, "Verde para direita");
            mov.MoverNoCirculo(15);
            while(aux.MedirLuz(1) > escuroLinha){
                bc.Move(970, -970);
            }
            mov.MoverNoCirculo(10);
        }

        else if(lado == "ambos"){
            bc.PrintConsole(2, "Volta verde");
            mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(bc.Compass() + 180)));
        }
    }

    static public void Gangorra(){
        /*
            Detecta variação de inclinação e para o robô enquanto houver alteração.
            Utilizar um sistema de contadores de looping para medir tempo.

        */

        // bc.PrintConsole(2, counterGangorra.ToString() + " " + inclinacaoAtual.ToString("F2") + " " + inclinacaoAntiga.ToString("F2" ));

        while(inclinacaoAtual > inclinacaoAntiga + 2){
            bc.TurnLedOn(0, 100, 0);
            aux.ContarGangorra();

            bc.PrintConsole(1, "Gangorra esperando");
            bc.Move(-12, -12);
            aux.Tick();
        }
        bc.TurnLedOff();
    }

    static public void Desvio(){
            /*
                Desvio. Feito o mais próximo possível do obstáculo.
            */

            bc.PrintConsole(1, "Desvio");

            int anguloInicial = matAng.AproximarAngulo(bc.Compass());

            bc.Move(0, 0);
            aux.Tick();

            mov.MoverProAngulo(matAng.MatematicaCirculo(anguloInicial + 270));

            bc.MoveFrontalRotations(200, 13.5f);

            mov.MoverProAngulo(matAng.MatematicaCirculo(anguloInicial));

            while((aux.MedirLuz(1) > pista.escuroLinha) && (aux.MedirLuz(2) > pista.escuroLinha)){
                if(bc.Distance(1) < 25){
                    bc.PrintConsole(1, "Bloco detectado a direita");
                    
                    bc.MoveFrontalRotations(200, 24.5f);

                    mov.MoverNoCirculo(90);
                }
                else{
                    bc.Move(150, 150);
                    aux.Tick();
                }
            }

            bc.PrintConsole(2, "Linha detectada");

            bc.MoveFrontalRotations(200, 11);

            mov.MoverNoCirculo(-90);

            bc.MoveFrontal(0, 0);
            aux.Tick();

            while(bc.Touch(0) == false){
                bc.Move(-100, -100);
            }
    }

    static public void PegarKit(){
        /* 
            Pegar o kit de resgate. 
            MoverEscavadora está em 1 para impedir que de bug tentando chegar em valores muito baixos.
        */

        bc.Move(0, 0);

        mov.MoverProAngulo(matAng.AproximarAngulo(bc.Compass()));

        mov.MoverPorUnidadeRotacao(-12);
        mov.MoverEscavadora(1);
        mov.MoverPorUnidadeRotacao(47);
        mov.MoverEscavadora(85);
        mov.MoverPorUnidadeRotacao(-35);
    }

    static public bool VerResgate(){
        /* 
            Detecta a área de resgate.
            Utiliza um padrão de cor específico da fita cinza.
        */ 

        int countUltra = 0;
        
        if((bc.ReturnBlue(0) > 50 &&
          (bc.ReturnGreen(0) > 40 && bc.ReturnGreen(0) < 50) &&
          (bc.ReturnRed(0) > 40 && bc.ReturnRed(0) < 50)) &&
          // =====
          (bc.ReturnBlue(1) > 50 &&
          (bc.ReturnGreen(1) > 40 && bc.ReturnGreen(1) < 50) &&
          (bc.ReturnRed(1) > 40 && bc.ReturnRed(1) < 50)))
          { 
            bc.PrintConsole(2, "Vi a linha cinza");
            mov.MoverPorUnidadeRotacao(30);
            if(bc.Distance(0) < 380) countUltra++;
            if(bc.Distance(1) < 380) countUltra++;
            if(bc.Distance(2) < 380) countUltra++;
            
            if(countUltra >= 2){
                return true;
            }
            else{
                mov.MoverPorUnidadeRotacao(-30);
            }

          }
        
        return false;
    }
}

class aux
{   
    /*
    ====== Funções Auxiliares ======
    - Tick
    - MedirLuz
    */ 

    // Reseta o valor em cada giro
    static public bool contadorPreto2 = false;

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
                if(contadorPreto2 == true){
                    contadorPreto2 = false;
                    mov.MoverPorUnidadeRotacao(10);
                }
                else{
                    contadorPreto2 = true;
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

    static public string tipoGiro90(string lado){
        /* 
        Função auxiliar para identificar se o giro é a partir de uma peça quadrada ou circular. Utiliza o fato de que uma deteção de uma peça circular tem mais linha preta na frente caso o robÔ continue andando.

        É utilizada para o robô não andar pra frente quando estiver percorrendo círculos, facilitando assim a leitura dos quadrados verdes.
        */

        bc.Move(0, 0);
        aux.Tick();

        // bc.MoveFrontalRotations(100, 0.5f);
        
        float anguloGiro = 0, anguloInicial;
        int sensibilidadeCirculo = 8; // se o giro for menor que esse valor ele é considerado um círculo

        bc.MoveFrontalRotations(100, 0.7f);
        if(aux.MedirLuz(1) < pista.escuroPreto2 && aux.MedirLuz(2) < pista.escuroPreto2){
            bc.MoveFrontalRotations(-100, 0.7f);
            return "preto2";
        }

        aux.ChegarNoMeioDaLinha(lado);

        anguloInicial = bc.Compass();

        if(lado == "direita"){

            do{
                bc.Move(900, -900);
                if(aux.MedirLuz(1) > pista.escuroIdentLinha) break;
                if(bc.ReturnColor(0) == "GREEN"){
                    pista.GirarVerde("direita");
                    return "";
                }
                if(bc.ReturnColor(1) == "GREEN"){
                    pista.GirarVerde("esquerda");
                    return "";
                }

                anguloGiro = matAng.MatematicaCirculo(bc.Compass() - anguloInicial); 
            }
            while(anguloGiro < sensibilidadeCirculo + 5 || anguloGiro > sensibilidadeCirculo + 10);
        }
        else if(lado == "esquerda"){

            do{
                bc.Move(-900, 900);
                if(aux.MedirLuz(2) > pista.escuroIdentLinha) break;
                if(bc.ReturnColor(0) == "GREEN"){
                    pista.GirarVerde("direita");
                    return "";
                }
                if(bc.ReturnColor(1) == "GREEN"){
                    pista.GirarVerde("esquerda");
                    return "";
                }

                anguloGiro = matAng.MatematicaCirculo(anguloInicial - bc.Compass()); 
            }
            while(anguloGiro < sensibilidadeCirculo + 5 || anguloGiro > sensibilidadeCirculo + 10);
        }

        if(anguloGiro > sensibilidadeCirculo && anguloGiro < 80){
            bc.PrintConsole(3, "QUADRADO " + anguloGiro.ToString());
            if(lado == "esquerda") mov.MoverNoCirculo(anguloGiro);
            else mov.MoverNoCirculo(-anguloGiro);

            return "quadrado";
        }
        else{
            bc.PrintConsole(3, "CIRCULO " + anguloGiro.ToString());
            return "circulo";
        }
    }

    static public void ChegarNoMeioDaLinha(string lado){
        bc.PrintConsole(2, "Indo pro meio da linha");

        bc.Move(0, 0);
        bc.Wait(50);

        int sensor;
        float luzAntiga, luzAtual = 0;

        if(lado == "esquerda") sensor = 2;
        else sensor = 1;
        
        luzAtual = aux.MedirLuz(sensor);


        while(true){
            bc.Move(-30, -30);
            bc.Wait(7);
         
            luzAntiga = luzAtual;
            luzAtual = aux.MedirLuz(sensor);
            if(luzAtual < 4) break;
            if(luzAtual > luzAntiga) break;
        }
    }

    static public void ContarGangorra(){
        if(pista.counterGangorra == 20){
            pista.inclinacaoAntigassa = pista.inclinacaoAntiga;
            pista.inclinacaoAntiga = pista.inclinacaoAtual;

            pista.inclinacaoAtual = (int)bc.Inclination();
            if(pista.inclinacaoAtual > -10 && pista.inclinacaoAtual < 23) pista.inclinacaoAtual += 360;

            pista.counterGangorra = 0;
        }
        else{
            pista.counterGangorra++;
        }
    }

    static public bool ProcurarMaisVerde(string lado){
        bool res = false;
        float anguloInicial, anguloGiro;

        anguloInicial = bc.Compass();

        if(lado == "esquerda"){
            do{
                anguloGiro = matAng.MatematicaCirculo(anguloInicial - bc.Compass());
                
                if(bc.ReturnColor(1 - 1) == "GREEN"){
                    res = true;
                    break;
                }
                if(MedirLuz(1) < pista.escuroLinha) break;

                bc.Move(-900, 900);
            }
            while(anguloGiro < 10 || anguloGiro > 12);
        
            mov.MoverNoCirculo(anguloGiro);
        }
        else if(lado == "direita"){
            do{
                anguloGiro = matAng.MatematicaCirculo(bc.Compass() - anguloInicial);
                
                if(bc.ReturnColor(2 - 1) == "GREEN"){
                    res = true;
                    break;
                }
                if(MedirLuz(2) < pista.escuroLinha) break;

                bc.Move(900, -900);
            }
            while(anguloGiro < 10 || anguloGiro > 12);
        
            mov.MoverNoCirculo(-anguloGiro);
        }

        return res;
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

