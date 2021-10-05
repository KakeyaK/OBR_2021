//Sensibilidade ao erro no tempo.
//Mauro Moledo

int velocidade = 170;
string estagio = "Pista";

void Main()
{
    res.Radar("Não");
}
class res
{
    static public void Radar(string recursividade)
    {
        //Variavel para controle do vetor
        int direcao = 1;
        while (true)
        {
            //Verficia a variação para indentificar bolinhas e ignorar o terreno
            bc.PrintConsole(0, " ");
            bc.Wait(5);
            float val1 = bc.distance(2 - 1); // Ultrassom da direita
            float val2 = bc.distance(3 - 1); // Ultrassom da esquerda
            bc.Wait(5);
            float variacaoDireita = (val1 - bc.distance(2 - 1));
            float variacaoEsquerda = (val2 - bc.distance(3 - 1));

            bc.PrintConsole(1, "Variação direita: " + variacaoDireita.ToString());
            bc.PrintConsole(2, "Variação esquerda: " + variacaoEsquerda.ToString());

            //se ver a saida ou o inicio
            if (false)
            {
                //Estima onde a parede vai estar 
                if (bc.Distance(2 - 1) > 900) { float paredeEsperada = 267 - bc.Distance(2 - 1); }
                if (bc.Distance(3 - 1) > 900) { float paredeEsperada = 267 - bc.Distance(3 - 1); }
            }

            //Se a variação for maior que 7, busca a bolinha no lado.
            else if (variacaoDireita > 7 || variacaoDireita < -7)
            {
                if (variacaoDireita < -7) { res.buscaBolinha("Direita", 1 * direcao); }
                if (variacaoDireita > 7) { res.buscaBolinha("Direita", -1 * direcao); }
            }
            else if (variacaoEsquerda > 7 || variacaoEsquerda < -7)
            {
                if (variacaoEsquerda < -7) { res.buscaBolinha("Esquerda", 1 * direcao); }
                if (variacaoEsquerda > 7) { res.buscaBolinha("Esquerda", -1 * direcao); }
            }


            //Se chegar na parede inverte a variavel de direção.
            if (bc.distance(1 - 1) < 30 || bc.Touch(1 - 1) == true)
            {
                bc.PrintConsole(0, "Parede, uêpa");

                direcao = direcao * -1;

                while (bc.distance(1 - 1) < 30 || bc.Touch(1 - 1) == true)
                {
                    bc.MoveFrontal(200 * direcao, 200 * direcao);
                }
            }
            else
            {
                bc.MoveFrontal(200 * direcao, 200 * direcao);
            }

        }
    }
    static public void buscaBolinha(string lado, int passou)
    {
        bc.MoveFrontal(0, 0);
        bc.PrintConsole(0, "Situação: " + lado + " " + passou.ToString());

        //Corrige precipitação ou atraso
        if (passou == -1)
        {
            bc.MoveFrontal(200, 200);
            bc.Wait(150);
        }
        if (passou == 1)
        {
            bc.MoveFrontal(-200, -200);
            bc.Wait(250);
        }
        bc.MoveFrontal(0, 0);

        //Gira para o lado certo da bolinha
        if (lado == "Direita") { mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(bc.Compass() + 90)), 950); }
        if (lado == "Esquerda") { mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(bc.Compass() - 90)), 950); }
        bc.MoveFrontal(0, 0);
        bc.MoveFrontal(200, 200);
        bc.Wait(1000);
    }
}
class pista
{
    public static int escuro = 20;

    // Variáveis PID
    private float error = 0, lastError = 0, integral = 0, derivate = 0;
    private float movimento;
    private bool pararIntegro;

    public void SeguirLinhaPID(float velocidade, float kp, float ki, float kd)
    {
        float sensibilidade90 = 53;

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

        if (error > sensibilidade90)
        {
            pista.Girar90("esquerda");
        }
        else if (error < -sensibilidade90)
        {
            pista.Girar90("direita");
        }
    }

    static public void Girar90(string lado)
    {
        mov.MoverPorUnidade(16);

        float anguloInicial = bc.Compass();
        bool retornando = false;

        if (lado == "esquerda")
        {
            bc.PrintConsole(1, "Giro para esquerda");

            while (aux.MedirLuz(1) > escuro)
            {
                if (matAng.MatematicaCirculo(anguloInicial - bc.Compass()) > 160)
                {
                    mov.MoverNoCirculo(75);
                    retornando = true;
                    break;
                }
                bc.MoveFrontal(970, -970);
            }
            if (retornando == false)
            {
                bc.MoveFrontal(-900, 900);
                bc.Wait(350);
            }
        }

        else if (lado == "direita")
        {
            bc.PrintConsole(1, "Giro para direita");

            while (aux.MedirLuz(2) > escuro)
            {
                if (matAng.MatematicaCirculo(bc.Compass() - anguloInicial) > 160)
                {
                    mov.MoverNoCirculo(-75);
                    retornando = true;
                    break;
                }
                bc.MoveFrontal(-970, 970);
            }
            if (retornando == false)
            {
                bc.MoveFrontal(900, -900);
                bc.Wait(350);
            }
        }

        if (retornando == false)
        {
            mov.MoverPorUnidade(-10);
        }
        bc.MoveFrontal(0, 0);
    }

    static public void GirarVerde(string lado)
    {
        bc.MoveFrontal(200, 200);
        bc.wait(500);

        if (lado == "esquerda")
        {
            bc.PrintConsole(1, "Verde para esquerda");
            mov.MoverNoCirculo(-35);
            while (aux.MedirLuz(1) > escuro)
            {
                bc.MoveFrontal(970, -970);
            }
            bc.MoveFrontal(-900, 900);
            bc.Wait(350);
        }

        else if (lado == "direita")
        {
            bc.PrintConsole(1, "Verde para direita");
            mov.MoverNoCirculo(35);
            while (aux.MedirLuz(2) > escuro)
            {
                bc.MoveFrontal(-970, 970);
            }
            bc.MoveFrontal(900, -900);
            bc.Wait(350);
        }
    }

    static public void Gangorra()
    {
        //variaves para checar a inclinação
        float val1 = 30;
        float val2 = 30;

        mov.MoverProAngulo(matAng.AproximarAngulo(bc.Compass()), 500);

        while (val2 - val1 < 4)
        {
            bc.PrintConsole(1, "Gangorra");
            bc.MoveFrontal(150, 150);
            val1 = bc.Inclination();
            bc.Wait(500);
            val2 = bc.Inclination();
        }

        bc.PrintConsole(1, "Gangorra Caindo...");
        bc.MoveFrontal(100, 100);
        bc.Wait(800);

        bc.PrintConsole(1, "Sai");
        bc.MoveFrontal(0, 0);
        aux.Tick();


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
        if (aproximacao == 1)
        {
            // Retornar aproximação de ângulo para um dos pontos cardeais
            if (angulo >= 315 || angulo < 45) return 0;
            if (angulo >= 45 && angulo < 135) return 90;
            if (angulo >= 135 && angulo < 225) return 180;
            if (angulo >= 225 && angulo < 315) return 270;
        }
        else if (aproximacao == 2)
        {
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
        // Faz matemática em ciclo, retornando o valor de deslocamento no ciclo trigonométrico.
        // Em resumo retorna a distância angular entre um ângulo específico e o ângulo 0
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
    - AjustarAnguloBalde
    - AjustarAlturaBalde
    */
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
        if (bc.Lightness(sensor - 1) > 65)
        {
            return 65;
        }
        else
        {
            return bc.Lightness(sensor - 1);
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

        if (bc.Distance(1 - 1) > distance)
        {
            while (bc.Distance(1 - 1) > distance)
            {
                bc.MoveFrontal(velocidade, velocidade);
                aux.Tick();
            }
        }
        else
        {
            while (bc.Distance(1 - 1) < distance)
            {
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

        velocidade do robô 1 a 200 de força = 54zm/s
        */

        if (distancia > 0)
        {
            bc.MoveFrontal(200, 200);
            bc.Wait((int)(distancia / 54 * 1000));
        }
        else
        {
            bc.MoveFrontal(-200, -200);
            bc.Wait((int)(-distancia / 54 * 1000));
        }
    }

    static public void MoverBalde(float alvoBalde)
    {
        /*
        Move o balde do robô até o ângulo desejado
        */
        // Verifica se o ângulo desejado está no intervalo do que o robô consegue alcançar.
        if (alvoBalde > 0 && alvoBalde < 12)
        {
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
        if (alvoEscavadora >= 0 && alvoEscavadora < 90)
        {
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

    static public bool MoverNoCirculo(float anguloMovimento, float velocidade = 950)
    {
        /*
        Girar por graus, independente da orientação
        positivo para sentido horário
        negativo para sentido anti-horário
        Margem de erro > 5º
        Máximo de movimento em uma direção = 355
        */

        // Verificar se passou por cima de uma linha
        bool linha = false;

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
                    if (aux.MedirLuz(1) < pista.escuro) linha = true;
                }
            }
            // Movimento regular
            else
            {
                while (bc.Compass() < anguloInicial + anguloMovimento)
                {
                    bc.MoveFrontal(-velocidade, velocidade);
                    if (aux.MedirLuz(1) < pista.escuro) linha = true;
                }
            }
        }
        else if (anguloMovimento < 0)
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
                    if (aux.MedirLuz(2) < pista.escuro) linha = true;
                }
            }
            //Movimento regular
            else
            {
                while (bc.Compass() > anguloInicial - anguloMovimento)
                {
                    bc.MoveFrontal(velocidade, -velocidade);
                    if (aux.MedirLuz(2) < pista.escuro) linha = true;
                }
            }
        }

        return linha;
    }

    static public bool MoverProAngulo(float angulo, float velocidade = 950)
    {
        /*
        Se locomove até o ângulo desejado. 
        Apenas valores positivos
        */
        bool linha = false;

        if (angulo == 360) angulo = 0;

        if (angulo >= 0 && angulo < 360)
        {
            float anguloDiferenca = matAng.MatematicaCirculo(angulo - bc.Compass());

            if (anguloDiferenca < 180)
            {
                //girar no sentido horário
                linha = MoverNoCirculo(anguloDiferenca, velocidade);
            }

            else
            {
                //girar no sentido anti-horário
                linha = MoverNoCirculo(anguloDiferenca - 360, velocidade);
            }
        }

        return linha;
    }

}