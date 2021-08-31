int velocidade = 170;

void Main()
{   
    pista pista = new pista();
    while (true)
    {   
        if(bc.ReturnColor(1 - 1) == "GREEN"){
            pista.GirarVerde("direita");
        }
        if(bc.ReturnColor(2 - 1) == "GREEN"){
            pista.GirarVerde("esquerda");
        }
        pista.SeguirLinhaPID(velocidade, 20, 1.5f, 10);
    }
    
}

class pista
{
    // Variáveis PID
    private float error = 0, lastError = 0, integral = 0, derivate = 0;
    private float movimento;
    private bool pararIntegro;
    static int escuro = 20;
    
    public void SeguirLinhaPID(float velocidade, float kp, float ki, float kd)
    {
        float sensibilidade90 = 37;

        // matemática PID
        error = aux.MedirLuz(1) - aux.MedirLuz(2);
        integral += error;
        derivate = error - lastError;


        if (pararIntegro)
        {
            movimento = error * kp + derivate * kd;
        }
        else
        {
            movimento = error * kp + integral * ki + derivate * kd;
        }

        // Controle de erros do integro
        pararIntegro = ((error < 10 && integral > 100) || (((error > 0 && movimento > 0) || (error < 0 && movimento < 0)) && (movimento > 1000 - velocidade)));

        if (movimento > 1000 - velocidade) { movimento = 1000 - velocidade; }

        // Console
        bc.PrintConsole(0, "Error: " + error.ToString("F") + " M: " + movimento.ToString());
        bc.PrintConsole(1, "Integral: " + integral.ToString() + " Integro Parado: " + pararIntegro.ToString());
        // bc.PrintConsole(2, "Derivate: " + derivate.ToString("F"));

        // Movimento
        bc.MoveFrontal(velocidade + movimento, velocidade - movimento);
        aux.Tick();

        // Atualização de variável
        lastError = error;
        
        if(error > sensibilidade90){
            pista.Girar90("esquerda");
        }
        else if(error < -sensibilidade90){
            pista.Girar90("direita");
        }
    }

    static public void Girar90(string lado){
        bc.MoveFrontal(200, 200);
        bc.wait(400);
        
        if(lado == "esquerda"){
            bc.PrintConsole(1, "Giro para esquerda");
            
            int anguloInicial = bc.Compass();
            while(aux.MedirLuz(1) > escuro){
                bc.MoveFrontal(900, -900);
                aux.Tick();
            }
            bc.MoveFrontal(-900, 900);
            bc.Wait(350);
        }
        
        else if(lado == "direita"){
            bc.PrintConsole(1, "Giro para direita");

            int anguloInicial = bc.Compass();
            while(aux.MedirLuz(2) > escuro){
                bc.MoveFrontal(-900, 900);
                aux.Tick();

                // if(bc.Compass()) girar usando aproximação de angulo + 90
            }
            bc.MoveFrontal(900, -900);
            bc.Wait(350);
        }

        // bc.MoveFrontal(-200, -200);
        // bc.wait(150);
    }

    static public void GirarVerde(string lado){
        bc.MoveFrontal(200, 200);
        bc.wait(400);
        
        if(lado == "esquerda"){
            bc.PrintConsole(1, "Verde para esquerda");
            mov.MoverNoCirculo(-35);
            while(aux.MedirLuz(1) > escuro){
                bc.MoveFrontal(900, -900);
                aux.Tick();
            }
            bc.MoveFrontal(-900, 900);
            bc.Wait(350);
        }
        
        else if(lado == "direita"){
            bc.PrintConsole(1, "Verde para direita");
            mov.MoverNoCirculo(35);
            while(aux.MedirLuz(2) > escuro){
                bc.MoveFrontal(-900, 900);
                aux.Tick();
            }
            bc.MoveFrontal(900, -900);
            bc.Wait(350);
        }
    }
}

class MatAng
{
    /*
    ====== Funções de Matemática com Ângulos ======
    - AproximarAngulo
    - MatematicaCirculo
    */ 
    
    static public int AproximarAngulo(float angulo)
    {
    // Retornar aproximação de ângulo para um dos pontos cardeais
        if (angulo >= 315 || angulo < 45) return 0;
        if (angulo >= 45 && angulo < 135) return 90;
        if (angulo >= 135 && angulo < 225) return 180;
        if (angulo >= 225 && angulo < 315) return 270;
        else return 0;
    }
    
    static public float MatematicaCirculo(float angulo)
    {
    // Faz matemática em ciclo, retornando o valor de deslocamento no ciclo trigonométrico.
        if (angulo >= 360)
        {
            return angulo - 360;
        }
        else if (angulo < 0)
        {

            return (float)(-360 * Math.Floor((double)(angulo / 360)) + angulo);
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
    - AjustarAnguloBalde
    - AjustarAlturaBalde
    */ 
    static public void Tick()
    {
        /*
        Tempo de espera entre ações na programação
        */
        bc.Wait(30);
    }

    static public float MedirLuz(int sensor)
    {
        /* 
        Filtro da função de medir luz do robô
        */
        return bc.Lightness(sensor - 1);
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

        if(bc.Distance(1 - 1) > distance){
            while(bc.Distance(1 - 1) > distance){
                bc.MoveFrontal(velocidade, velocidade);
                aux.Tick();
            }
        }
        else{
            while(bc.Distance(1 - 1) < distance){
                bc.MoveFrontal(-velocidade, -velocidade);
                aux.Tick();
            }
        }
    }

    static public void MoverPorUnidade(float distancia)
    {
        /*
        A partir do cáculo de velocidade por segundo do robô se move uma quantidade desejada
        Exige calibração prévia
        */

        if (distancia > 0)
        {
            bc.MoveFrontal(200, 200);
            bc.Wait((int)(distancia / 39.64 * 1000));
        }
        else
        {
            bc.MoveFrontal(-200, -200);
            bc.Wait((int)(-distancia / 39.64 * 1000));
        }
    }
    
    static public void MoverBalde(double alvoBalde)
    {   
        /*
        Move o balde do robô até o ângulo desejado
        */

        if (Math.Sin(bc.AngleScoop() * Math.PI / 180) > Math.Sin(alvoBalde * Math.PI / 180))
        {
            //enquanto o seno da posicao atual da escavadora for menor q o seno da posicao alvo, a escavadora sobe
            while (Math.Sin(bc.AngleScoop() * Math.PI / 180) > Math.Sin(alvoBalde * Math.PI / 180))
            {
                bc.TurnActuatorDown(30);
            }
        }

        else
        {
            //enquanto o seno da posicao atual da escavadora for maior q o seno da posicao alvo, a escavadora desce
            while (Math.Sin(bc.AngleScoop() * Math.PI / 180) < Math.Sin(alvoBalde * Math.PI / 180))
            {

                bc.TurnActuatorUp(30);
            }
        }
    }

    static public void MoverEscavadora(double alvoEscavadora)
    {   
        /* 
        Move a escavadora do robô até o ângulo desejado
        */
        if (Math.Sin(bc.AngleActuator() * Math.PI / 180) > Math.Sin(alvoEscavadora * Math.PI / 180))
        {
            //enquanto o seno da posicao atual da escavadora for menor q o seno da posicao alvo, a escavadora sobe
            while (Math.Sin(bc.AngleActuator() * Math.PI / 180) > Math.Sin(alvoEscavadora * Math.PI / 180))
            {
                //A escavadora tem os angulos invertidos :P

                bc.ActuatorUp(30);
            }
        }
        else
        {
            //enquanto o seno da posicao atual da escavadora for maior q o seno da posicao alvo, a escavadora desce
            while (Math.Sin(bc.AngleActuator() * Math.PI / 180) < Math.Sin(alvoEscavadora * Math.PI / 180))
            {

                bc.ActuatorDown(30);
            }
        }
    }

    static public void MoverNoCirculo(float anguloMovimento, float velocidade = 950)
    {
        /*
        Girar por graus, independente da orientação
        positivo para sentido horário
        negativo para sentido anti-horário
        Margem de erro > 5º
        Máximo de movimento em uma direção = 355
        */
        
        float anguloInicial = bc.Compass();
        // Movimento positivo - sentido horário
        if (anguloMovimento > 0)
        {
            // Alterando os valores para evitar o loop infinito em 360º/0º
            if (anguloInicial + anguloMovimento == 359) anguloInicial += -1;
            if (anguloInicial + anguloMovimento == 360) anguloInicial += 2;
            if (anguloInicial + anguloMovimento == 361) anguloInicial += 1;

            // Movimento passa pelo limite de 0/360º
            if (anguloInicial + anguloMovimento > 360)
            {
                while (bc.Compass() > anguloInicial + anguloMovimento - 355 || bc.Compass() < anguloInicial + anguloMovimento - 360)
                {
                    bc.MoveFrontal(-velocidade, velocidade);
                    aux.Tick();
                }
            }
            // Movimento regular
            else
            {
                while (bc.Compass() < anguloInicial + anguloMovimento)
                {
                    bc.MoveFrontal(-velocidade, velocidade);
                    aux.Tick();
                }
            }
        }
        else
        {
            // Invertendo o sinal do ângulo pra facilitar a visualização da matemática
            anguloMovimento = anguloMovimento * -1;

            // Alterando os valores para evitar o loop infinito em 360º/0º
            if (anguloInicial - anguloMovimento == -1) anguloInicial += -1;
            if (anguloInicial - anguloMovimento == 0) anguloInicial += -2;
            if (anguloInicial - anguloMovimento == 1) anguloInicial += 1;

            // Movimento passa pelo limite de 0/360º
            if (anguloInicial < anguloMovimento)
            {
                while (bc.Compass() < anguloInicial + 355 - anguloMovimento || bc.Compass() > anguloInicial + 360 - anguloMovimento)
                {
                    bc.MoveFrontal(velocidade, -velocidade);
                    aux.Tick();
                }
            }
            //Movimento regular
            else
            {
                while (bc.Compass() > anguloInicial - anguloMovimento)
                {
                    bc.MoveFrontal(velocidade, -velocidade);
                    aux.Tick();
                }
            }
        }
    }

    static public void MoverProAngulo(float angulo, float velocidade)
    {   
        /*
        Se locomove até o ângulo desejado. 
        Apenas valores positivos
        
        TOMAR CUIDADO COM PRECISÃO
        */

        if (MatAng.MatematicaCirculo(angulo - bc.Compass()) < 180)
        {
            //girar no sentido horário
            MoverNoCirculo(MatAng.MatematicaCirculo(angulo - bc.Compass()), velocidade);
        }

        else
        {
            //girar no sentido anti-horário
            MoverNoCirculo(MatAng.MatematicaCirculo(angulo - bc.Compass()) - 360, velocidade);
        }
    }

}

