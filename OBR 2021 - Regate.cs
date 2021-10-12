/*
    GPS
        Movimentação no triangulo       [concluido]
        Mapeamento da posição inicial   [concluido]
        Localizar arena preta           [concluido] 
        Função de Localização           
        Traçar Rota    
        Entregar bloco azul             [concluido]
              
    Radar                               ===
        Identificar bolinha sem erro    [concluído]
        alinhamento com a bolinha       [concluido]
        Igorar saidas e abismos         [concluido]
        Bolinhas pretas ou arena        {pela metade} [deixar por ultimo]
        Movimentação para pegar bolinha [concluido]
        Entregar brancas primeiro       [deixar por ultimo]
        Não esmagar bolinhas perto      [deixar por ultimo]
    Movimentação                        ===
        Movimentação até a arena        
        Movimentação de entrega         [tuco] [deixar por ultimo]
        Localizar saida (sem confundir) 
        Movimentação até a saida        
*/

//by Mauro Moledo



void Main()
{
    resgate.Resgate();
}
class resgate
{
    bool bolinhaPreta = false;
    string arenaPreta;

    static public void Resgate(float velocidadeReto = 290, float velocidadeGiro = 990)
    {
        bc.ActuatorSpeed(150);
        bc.CloseActuator();
        while (bc.AngleActuator() < 86) { bc.ActuatorUp(10); }
        mov.MoverProAngulo(matAng.AproximarAngulo(bc.Compass()), velocidadeGiro);
        bc.MoveFrontal(0, 0);

        while (bc.distance(3 - 1) + bc.distance(2 - 1) > 15000)
        {
            bc.MoveFrontal(velocidadeReto, velocidadeReto);
            aux.Tick();
        }
        bc.Wait(230);

        bc.MoveFrontal(0, 0);
        bc.PrintConsole(0, "Correçãao finalizada");

        float[] mapa;
        mapa = new float[7];
        mapa = gps.Mapeamento();
        resgate.Radar();

    }
    static public void Radar(float velocidadeReto = 200)
    {
        //Variavel para controle da direção 
        int direcao = 1;
        float paredeDireita = 0;
        float paredeEsquerda = 0;
        while (true)
        {
            //Verifica a variação para indentificar bolinhas e ignorar o terreno
            bc.PrintConsole(0, " ");
            bc.Wait(5);
            float val1 = bc.distance(2 - 1); // Ultrassom da direita
            float val2 = bc.distance(3 - 1); // Ultrassom da esquerda
            bc.Wait(5);
            float variacaoDireita = (val1 - bc.distance(2 - 1));
            float variacaoEsquerda = (val2 - bc.distance(3 - 1));

            bc.PrintConsole(1, "Variação direita: " + variacaoDireita.ToString());
            bc.PrintConsole(2, "Variação esquerda: " + variacaoEsquerda.ToString());


            //Ignorar abismos
            if (bc.distance(3 - 1) < paredeEsquerda + 10 && bc.distance(3 - 1) > paredeEsquerda - 10 && paredeEsquerda != 0)
            {
                bc.PrintConsole(4, "Ignorar abismo");
                paredeDireita = 0;
                paredeEsquerda = 0;
            }

            else if (bc.distance(2 - 1) < paredeDireita + 10 && bc.distance(2 - 1) > paredeDireita - 10 && paredeDireita != 0)
            {
                bc.PrintConsole(4, "Ignorar abismo");
                paredeDireita = 0;
                paredeEsquerda = 0;
            }

            else if (bc.distance(3 - 1) > 9000 && paredeEsquerda == 0)
            {

                while (bc.distance(3 - 1) > 9000)
                {
                    bc.MoveFrontal(-200 * direcao, -200 * direcao);
                }

                bc.MoveFrontal(0, 0);

                paredeEsquerda = bc.distance(3 - 1);

                while (bc.distance(3 - 1) < 9000)
                {
                    bc.MoveFrontal(200 * direcao, 200 * direcao);
                }

                bc.MoveFrontal(0, 0);

            }
            else if (bc.distance(2 - 1) > 9000 && paredeDireita == 0)
            {  //Lara esteve aqui

                while (bc.distance(2 - 1) > 9000)
                {
                    bc.MoveFrontal(-200 * direcao, -200 * direcao);
                }

                bc.MoveFrontal(0, 0);

                paredeDireita = bc.distance(2 - 1);

                while (bc.distance(2 - 1) < 9000)
                {
                    bc.MoveFrontal(200 * direcao, 200 * direcao);
                }
                bc.MoveFrontal(0, 0);

            }

            //Se a variação for maior que 7, busca a bolinha no lado.
            else if (variacaoDireita > 7 || variacaoDireita < -7)
            {
                if (variacaoDireita < -7) { resgate.buscaBolinha("Direita", 1 * direcao); }
                if (variacaoDireita > 7) { resgate.buscaBolinha("Direita", -1 * direcao); }
            }
            else if (variacaoEsquerda > 7 || variacaoEsquerda < -7)
            {
                if (variacaoEsquerda < -7) { resgate.buscaBolinha("Esquerda", 1 * direcao); }
                if (variacaoEsquerda > 7) { resgate.buscaBolinha("Esquerda", -1 * direcao); }
            }


            //Se achar coisas no meio do caminho
            /*if (bc.ReturnColor(3 - 1) == "BLACK") //Ou sensor de temperatura
            {
                
                //Se for a area preta
                if (bc.distance(1 - 1) < 73 && (bc.distance(3 - 1) < 21 || bc.distance(2 - 1) < 21))
                {
                    bc.PrintConsole(0, "Arena");
                }

                //Se for a bolinha preta
                else if (bolinhaPreta == true)
                {
                    bc.PrintConsole(0, "Fudeu");
                }

                else
                {
                    
                bc.PrintConsole(0, "Fudeu");
                
            }
            bc.MoveFrontal(0, 0);
            bc.Wait(10000);
            
            }*/


            //Se chegar na parede inverte a variavel de direção.
            if (bc.distance(1 - 1) < 20 || bc.Touch(1 - 1) == true)
            {
                bc.PrintConsole(0, "Parede, uêpa");

                direcao = direcao * -1;

                while (bc.distance(1 - 1) < 30 || bc.Touch(1 - 1) == true)
                {
                    bc.MoveFrontal(velocidadeReto * direcao, velocidadeReto * direcao);
                }
            }
            else
            {
                bc.MoveFrontal(velocidadeReto * direcao, velocidadeReto * direcao);
            }

        }
    }
    static public void buscaBolinha(string lado, int passou, float velocidadeGiro = 950, float velocidadeReto = 200)
    {
        bc.MoveFrontal(0, 0);
        bc.PrintConsole(0, "Situação: " + lado + " " + passou.ToString());
        bc.PrintConsole(3, " 1 = passou");
        bc.PrintConsole(4, "-1 = precipitou");

        //Corrige precipitação ou atraso
        //Interessante utilizar a largura das bolinhas para medir a precisão {não implementado}
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
        float val = 0;
        bc.PrintConsole(0, "Angulo " + bc.Compass().ToString());
        if (lado == "Direita")
        {
            val = bc.distance(2 - 1);
            mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(bc.Compass() + 90)), velocidadeGiro);
        }
        if (lado == "Esquerda")
        {
            val = bc.distance(3 - 1);
            mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(bc.Compass() - 90)), velocidadeGiro);
        }
        bc.MoveFrontal(0, 0);
        float atual = bc.distance(1 - 1);

        //Movimentação da garra
        while (bc.AngleActuator() > 1) { bc.ActuatorDown(10); }
        bc.OpenActuator();
        if (bc.distance(1 - 1) < 9000)
        {
            while (bc.distance(1 - 1) > atual - val + 14 && bc.distance(1 - 1) > 30) { bc.MoveFrontal(velocidadeReto, velocidadeReto); }
        }
        else
        {
            while (bc.HasVictim() == false) { bc.MoveFrontal(velocidadeReto, velocidadeReto); }
            bc.Wait(200);
        }
        bc.MoveFrontal(0, 0);
        bc.CloseActuator();
        while (bc.AngleActuator() < 86) { bc.ActuatorUp(10); }

        //EntregarBolinha();
    }
}

class gps

{

    public static float[] Mapeamento(float velocidadeGiro = 990, float velocidadeReto = 290)
    {
        /* === Array de MAPA ===

            index 0 = Largura da resolução de arena
            index 1 = Altura da resolução de arena
            index 2 = Angulo inicial
            index 3 = X0 Posição da entrada
            index 4 = XF Posição da entrada
            index 5 = Arena preta na linha inicial 1 sim / 0 não
            index 6 = posição da Arena preta 

            === === === === === ===
        */

        float ultra1 = bc.Distance(1 - 1);
        float ultra2 = bc.Distance(2 - 1); //Direita
        float ultra3 = bc.Distance(3 - 1); //Esquerda
        float compas = bc.Compass();
        float[] mapa;
        mapa = new float[7];

        //Mapeamento da resolução da arena === Largura(0) Altura(1) == Mapeamento da arena preta
        if (250 < ultra1 && ultra1 < 300)
        {
            mapa[0] = 400; mapa[1] = 300; bc.PrintConsole(0, "Mapa 0 " + mapa[0].ToString());
            bc.PrintConsole(1, "Mapa 1 " + mapa[1].ToString());
        }
        if (350 < ultra1 && ultra1 < 400)
        {
            mapa[0] = 300; mapa[1] = 400;
            bc.PrintConsole(0, "Mapa 0 " + mapa[0].ToString()); bc.PrintConsole(1, "Mapa 1 " + mapa[1].ToString());
        }


        //soma para 400 puro =  366
        //soma para 400 arena = 278
        //soma para 300 puro =  266
        //soma para 300 arena = 184

        if (ultra2 + ultra3 >= 350 && ultra2 + ultra3 < 900) { mapa[0] = 400; mapa[1] = 300; mapa[5] = 0; bc.PrintConsole(5, "Mapa 5 " + mapa[5].ToString()); } //soma para 400 puro =  366

        if (ultra2 + ultra3 >= 270 && ultra2 + ultra3 < 290) { mapa[0] = 400; mapa[1] = 300; mapa[5] = 1; bc.PrintConsole(5, "Mapa 5 " + mapa[5].ToString()); } //soma para 400 arena = 278

        if (ultra2 + ultra3 > 250 && ultra2 + ultra3 < 270) { mapa[0] = 300; mapa[1] = 400; mapa[5] = 0; bc.PrintConsole(5, "Mapa 5 " + mapa[5].ToString()); }  //soma para 300 puro =  266

        if (ultra2 + ultra3 > 100 && ultra2 + ultra3 < 200) { mapa[0] = 300; mapa[1] = 400; mapa[5] = 1; bc.PrintConsole(5, "Mapa 5 " + mapa[5].ToString()); }  //soma para 300 arena = 184


        //Mapeamento do giroscópio inicial
        mapa[2] = matAng.AproximarAngulo(compas);
        bc.PrintConsole(2, "Mapa 2 " + mapa[2].ToString());


        //Mapeamento da localização da entrada === y = 0 || parte 1 
        //isso se não tiver abismo na linha inicial
        if (mapa[5] == 0)
        {
            mapa[3] = ultra3 - 33;
            mapa[4] = mapa[3] + 100;

            bc.PrintConsole(3, "Mapa 3 " + mapa[3].ToString());
            bc.PrintConsole(4, "Mapa 4 " + mapa[4].ToString());
        }

        //Mapeamento da area preta e Mapeamento da localização da entrada || parte 2
        if (ultra2 < 900)
        {
            //Se não tiver abismo vira pra direita
            bc.MoveFrontal(200, 200);
            bc.Wait(290);
            bc.MoveFrontal(0, 0);
            mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(compas + 90)), velocidadeGiro);

            //certeza de arena na linha
            if (mapa[5] == 1)
            {
                if (ultra2 < bc.distance(1 - 1) - 10)
                {
                    mapa[6] = 3;
                    mapa[3] = ultra3 - 33;
                    mapa[4] = mapa[3] + 100;
                    bc.PrintConsole(3, "Mapa 3 " + mapa[3].ToString());
                    bc.PrintConsole(4, "Mapa 4 " + mapa[4].ToString());
                    bc.PrintConsole(6, "Mapa 6 " + mapa[6].ToString());
                    while (bc.distance(1 - 1) > 82) { bc.MoveFrontal(velocidadeReto, velocidadeReto); }
                    EntregarAzul();
                }
                else
                {
                    mapa[6] = 0;
                    mapa[3] = ultra3 - 33 + 85;
                    mapa[4] = mapa[3] + 100;
                    bc.PrintConsole(3, "Mapa 3 " + mapa[3].ToString());
                    bc.PrintConsole(4, "Mapa 4 " + mapa[4].ToString());
                    bc.PrintConsole(6, "Mapa 6 " + mapa[6].ToString());
                    mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(bc.Compass() - 180)), velocidadeGiro);
                    while (bc.distance(1 - 1) > 82) { bc.MoveFrontal(velocidadeReto, velocidadeReto); }
                    EntregarAzul();
                }

            }
            //certeza de falta de arena na linha
            if (mapa[5] == 0)
            {
                while (bc.distance(1 - 1) > 15) { bc.MoveFrontal(velocidadeReto, velocidadeReto); }
                mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(bc.Compass() - 90)), velocidadeGiro);
                Abismo();

                if (bc.ReturnColor(3 - 1) == "BLACK")
                {
                    mapa[6] = 2;
                    bc.PrintConsole(6, "Mapa 6 " + mapa[6].ToString());
                    EntregarAzul();
                }
                else
                {
                    mapa[6] = 1;
                    bc.PrintConsole(6, "Mapa 6 " + mapa[6].ToString());
                    while (bc.distance(1 - 1) > 15) { bc.MoveFrontal(velocidadeReto, velocidadeReto); }
                    mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(bc.Compass() - 90)), velocidadeGiro);

                    Abismo();
                    EntregarAzul();
                }
            }

        }
        //Se tiver abismo na direita
        else
        {
            //Salvar saida
            bc.MoveFrontal(200, 200);
            bc.Wait(290);
            bc.MoveFrontal(0, 0);
            mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(compas - 90)), velocidadeGiro);
            //Se tiver arena no 0
            if (ultra3 < bc.distance(1 - 1) - 10)
            {
                mapa[6] = 0;
                mapa[3] = ultra3 - 33 + 85;
                mapa[4] = mapa[3] + 100;
                bc.PrintConsole(3, "Mapa 3 " + mapa[3].ToString());
                bc.PrintConsole(4, "Mapa 4 " + mapa[4].ToString());
                bc.PrintConsole(6, "Mapa 6 " + mapa[6].ToString());

                while (bc.distance(1 - 1) > 82) { bc.MoveFrontal(velocidadeReto, velocidadeReto); }
                EntregarAzul();
            }
            //Se não tiver, pecorre o mapa
            else
            {
                while (bc.distance(1 - 1) > 15) { bc.MoveFrontal(velocidadeReto, velocidadeReto); }

                mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(bc.Compass() + 90)), velocidadeGiro);

                while (bc.distance(1 - 1) > 82) { bc.MoveFrontal(velocidadeReto, velocidadeReto); }

                //Se tiver arena no 1
                if (bc.ReturnColor(3 - 1) == "BLACK")
                {
                    mapa[6] = 1;
                    bc.PrintConsole(6, "Mapa 6 " + mapa[6].ToString());
                    EntregarAzul();
                }
                //Se não, tem arena no 2
                else
                {
                    mapa[6] = 2;
                    bc.PrintConsole(6, "Mapa 6 " + mapa[6].ToString());
                    while (bc.distance(1 - 1) > 15) { bc.MoveFrontal(velocidadeReto, velocidadeReto); }
                    mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(bc.Compass() + 90)), velocidadeGiro);

                    while (bc.distance(1 - 1) > 82) { bc.MoveFrontal(velocidadeReto, velocidadeReto); }
                    EntregarAzul();
                }
            }
        }

        return mapa;
    }
    public static void Abismo(float velocidadeGiro = 950, float velocidadeReto = 290)
    {
        if (bc.distance(1 - 1) > 9000)
        {
            while (bc.distance(3 - 1) < 900 && bc.distance(2 - 1) < 900)
            {
                bc.MoveFrontal(velocidadeReto, velocidadeReto);
            }
            bc.MoveFrontal(-velocidadeReto, -velocidadeReto);
            bc.Wait(250);
            bc.MoveFrontal(0, 0);
        }
        else
        {
            while (bc.distance(1 - 1) > 82) { bc.MoveFrontal(velocidadeReto, velocidadeReto); }
        }

    }
    public static void EntregarAzul()
    {
        bc.MoveFrontal(0, 0);
        bc.OpenActuator();
        while (bc.AngleActuator() > 20) { bc.ActuatorDown(10); }
        while (bc.AngleActuator() < 86) { bc.ActuatorUp(10); }
        bc.CloseActuator();
    }
    /*
    public static float[] Localização()
    {
        //fazer

    }
    */

    public static void MoverNoTriangulo(float y, float x, float ultra1, float velocidadeGiro = 950, float velocidadeReto = 290)
    {
        bc.PrintConsole(1, "Andando " + x.ToString() + " no eixo x === esquerda(-) || direita(+)");
        bc.PrintConsole(0, "Andando " + y.ToString() + " no eixo y === trás    (-) || frente (+)");

        //Teorema de pitagoras
        float h = (float)Math.Sqrt((x * x) + (y * y));

        //Lei dos senos
        float teta = (float)(Math.Asin((Math.Sin(Math.PI / 2) / (double)h * (double)x)) * (180 / Math.PI));

        //Aplica o angulo teta
        if (y >= 0) { mov.MoverProAngulo(matAng.MatematicaCirculo(bc.Compass() + teta), velocidadeGiro); }
        if (y < 0) { mov.MoverProAngulo(matAng.MatematicaCirculo(bc.Compass() - teta), velocidadeGiro); }
        bc.MoveFrontal(0, 0);

        ultra1 = bc.Distance(1 - 1);

        //Se ver abismo gira 180º
        if (ultra1 > 9000)
        {
            mov.MoverProAngulo(matAng.MatematicaCirculo(bc.Compass() + 180), velocidadeGiro);
            y = (y + 1) * -1; // inverte a direção da movimentação
        }
        bc.MoveFrontal(0, 0);

        ultra1 = bc.Distance(1 - 1);

        //Pecorre a hipotenusa
        if (ultra1 < 9000)
        {
            if (y >= 0)
            {
                while (bc.distance(1 - 1) > ultra1 - h && bc.distance(1 - 1) > 30)
                {
                    bc.MoveFrontal(velocidadeReto, velocidadeReto);
                }
            }

            if (y < 0)
            {
                while (bc.distance(1 - 1) < ultra1 + h && bc.Touch(1 - 1) == false)
                {
                    bc.MoveFrontal(-velocidadeReto, -velocidadeReto);
                }
            }

            bc.MoveFrontal(0, 0);
        }
        else
        {
            bc.PrintConsole(1, "Usar Função Metros por segundo");
        }

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
    public pista(float psensibilidade90 = 45)
    {
        sensibilidade90 = psensibilidade90;
    }

    public void SeguirLinhaPID(float velocidade, float kp, float ki, float kd)
    {
        // matemática PID
        error = aux.MedirLuz(1) - aux.MedirLuz(2);
        integral += error;
        derivate = error - lastError;

        if (integral * kp > 1000 - velocidade) integral = 1000 - velocidade;

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
        if (error > sensibilidade90 && Math.Abs(error - lastError) < 1)
        {
            giro = aux.tipoGiro90(2);
            bc.PrintConsole(3, "Tipo do giro: " + giro.ToString());
            pista.Girar90("esquerda", giro);
        }
        else if (error < -sensibilidade90 && Math.Abs(error - lastError) < 1)
        {
            giro = aux.tipoGiro90(1);
            bc.PrintConsole(3, "Tipo do giro: " + giro.ToString());
            pista.Girar90("direita", giro);
        }

        // Atualização de variável
        lastError = error;
    }

    static public void Girar90(string lado, string giro)
    {
        /*
            Giro em situações onde a ângulação do robô com a linha preta está muito alta.

            A partir da identificação de uma angulação alta (circulo) ou muito alta (quadrado) giro90 pode assumir duas formas:

            quadrado - anda pra frente e gira. O movimento pra frente ajuda no posicionamento e na detecção de linha preta para parar.
                       Caso o giro passe de um limite ele retorna, mantendo uma posição de 90º com a linha.


            circulo - não anda pra frente. Gira apenas o necessário para se alinhar. 
        */
        bc.Move(0, 0);
        aux.Tick();

        if (giro == "quadrado")
        {
            mov.MoverPorUnidadeRotacao(8);

            float anguloInicial = bc.Compass();
            float anguloGiro; // usado para verificar se é necessário retornar
            bool retornando = false; // Controle para verificar se o giro passou do limite adequado

            if (lado == "esquerda")
            {
                bc.PrintConsole(2, "Giro para esquerda");

                // mov.MoverNoCirculo(-5);

                while (aux.MedirLuz(2) > escuroLinha)
                {
                    anguloGiro = matAng.MatematicaCirculo(anguloInicial - bc.Compass());
                    if (anguloGiro > 160 && anguloGiro < 165)
                    {
                        bc.Move(0, 0);
                        aux.Tick();
                        bc.PrintConsole(2, "Retornando giro para esquerda " + anguloGiro.ToString());
                        mov.MoverNoCirculo(75);
                        retornando = true;
                        break;
                    }
                    bc.Move(-970, 970);
                }

                if (retornando == false)
                {
                    mov.MoverNoCirculo(-16);
                }
            }

            else if (lado == "direita")
            {
                bc.PrintConsole(2, "Giro para direita");

                // mov.MoverNoCirculo(5);

                while (aux.MedirLuz(1) > escuroLinha)
                {
                    anguloGiro = matAng.MatematicaCirculo(bc.Compass() - anguloInicial);
                    if (anguloGiro > 160 && anguloGiro < 165)
                    {
                        bc.PrintConsole(2, "Retornando giro para direita " + anguloGiro.ToString());
                        mov.MoverNoCirculo(-75);
                        retornando = true;
                        break;
                    }
                    bc.Move(970, -970);
                }

                if (retornando == false)
                {
                    mov.MoverNoCirculo(16);
                }
            }
        }
        else
        {
            float anguloInicial;
            float anguloGiro;

            anguloInicial = bc.Compass();

            if (lado == "esquerda")
            {
                bc.PrintConsole(2, "Giro para esquerda");
                do
                {
                    bc.Move(-900, 900);
                    if (bc.ReturnColor(0) == "Green") pista.GirarVerde("direita");
                    if (aux.MedirLuz(0) < pista.escuroLinha) break;

                    anguloGiro = matAng.MatematicaCirculo(anguloInicial - bc.Compass());
                }
                while (anguloGiro < 5 || anguloGiro > 7);
            }
            else
            {
                bc.PrintConsole(2, "Giro para direita");
                do
                {
                    bc.Move(900, -900);
                    if (bc.ReturnColor(1) == "Green") pista.GirarVerde("esquerda");
                    if (aux.MedirLuz(1) < pista.escuroLinha) break;

                    anguloGiro = matAng.MatematicaCirculo(bc.Compass() - anguloInicial);
                }
                while (anguloGiro < 5 || anguloGiro > 7);
            }
        }
    }

    static public void GirarVerde(string lado)
    {
        /*
            Segue os comandos devidos quando o robô encontra um quadrado verde.
        */
        bc.Move(0, 0);
        aux.Tick();

        bc.Move(200, 200);
        bc.wait(500);

        if (lado == "esquerda")
        {
            bc.PrintConsole(2, "Verde para esquerda");
            mov.MoverNoCirculo(-30);
            while (aux.MedirLuz(2) > escuroLinha)
            {
                bc.Move(-970, 970);
            }
            mov.MoverNoCirculo(-14);
        }

        else if (lado == "direita")
        {
            bc.PrintConsole(2, "Verde para direita");
            mov.MoverNoCirculo(30);
            while (aux.MedirLuz(1) > escuroLinha)
            {
                bc.Move(970, -970);
            }
            mov.MoverNoCirculo(14);
        }
    }

    static public void Gangorra()
    {
        // bc.PrintConsole(2, counter.ToString() + " " + inclinacaoAtual.ToString("F2") + " " + inclinacaoAntiga.ToString("F2" ));

        if (counter == 20)
        {
            inclinacaoAntiga = inclinacaoAtual;

            inclinacaoAtual = (int)bc.Inclination();
            if (inclinacaoAtual > -1 && inclinacaoAtual < 23) inclinacaoAtual += 360;

            counter = 0;
        }
        else
        {
            counter++;
        }

        if (inclinacaoAtual > inclinacaoAntiga + 5)
        {
            bc.PrintConsole(1, "Gangorra esperando");

            bc.Move(0, 0);
            aux.Tick();
        }
    }

    static public void desvio()
    {

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
        if (bc.Lightness(sensor - 1) > 65)
        {
            return 65;
        }
        else
        {
            return bc.Lightness(sensor - 1);
        }
    }

    static public void Preto2()
    {
        /*
            Realiza uma rotina de procurar o quadrado verde caso encontre preto nos dois sensores.

            Resolve o problema do robô estar muito para um lado da linha e passar direto pelo quadrado verde.
            Os valores de VerificarVerde() podem ser auterados para aumentar a detecção. É necessário tomar cuidado para que o robô não gire demais e encontre o quadrado verde com o sensor errado.
        */

        bc.PrintConsole(2, "Preto2");
        bc.Move(0, 0);
        aux.Tick();

        if (aux.VerificarVerde(10) == false)
        {
            bc.Move(0, 0);
            aux.Tick();
            mov.MoverPorUnidadeRotacao(-3.5f);

            if (aux.VerificarVerde(10) == false)
            {
                bc.Move(0, 0);
                aux.Tick();
                if (contador == true)
                {
                    contador = false;
                    mov.MoverPorUnidadeRotacao(10);
                }
                else
                {
                    contador = true;
                }
            }
        }
    }

    static public bool VerificarVerde(int area)
    {
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

        if (lado == "direita" || lado == "esquerda")
        {
            mov.MoverProAngulo(anguloInicial);
            pista.GirarVerde(lado);
            return true;
        }

        lado = VerificarVerdeMov(area * 2, "antihorario");

        if (lado == "direita" || lado == "esquerda")
        {
            mov.MoverProAngulo(anguloInicial);
            pista.GirarVerde(lado);
            return true;
        }

        lado = VerificarVerdeMov(area, "horario");

        if (lado == "direita" || lado == "esquerda")
        {
            mov.MoverProAngulo(anguloInicial);
            pista.GirarVerde(lado);
            return true;
        }

        return false;
    }

    static public string VerificarVerdeMov(int area, string sentido)
    {
        /*
            Rotina de movimentação da verificação de verde. Gira para os dois lados procurando o quadrdo verde.

            Auxilia a encontrar quadrados verdes com mais precisão
        */
        bc.Move(0, 0);
        aux.Tick();
        float anguloInicial = bc.Compass();
        float anguloGiro;

        if (sentido == "horario")
        {
            // Sentido horário areaº
            anguloGiro = matAng.MatematicaCirculo(bc.Compass() - anguloInicial);
            while ((anguloGiro < area || anguloGiro > area + 2) && (MedirLuz(2) > pista.escuroLinha))
            {
                if (bc.ReturnColor(0) == "GREEN")
                {
                    return "direita";
                }
                else if (bc.ReturnColor(1) == "GREEN")
                {
                    return "esquerda";
                }
                bc.Move(850, -850);
                anguloGiro = matAng.MatematicaCirculo(bc.Compass() - anguloInicial);
            }
        }
        if (sentido == "antihorario")
        {
            // Sentido anti horário areaº
            anguloGiro = matAng.MatematicaCirculo(anguloInicial - bc.Compass());
            while ((anguloGiro < area || anguloGiro > area + 2) && (MedirLuz(1) > pista.escuroLinha))
            {
                if (bc.ReturnColor(0) == "GREEN")
                {
                    return "direita";
                }
                else if (bc.ReturnColor(1) == "GREEN")
                {
                    return "esquerda";
                }
                bc.Move(-850, 850);
                anguloGiro = matAng.MatematicaCirculo(anguloInicial - bc.Compass());
            }
        }

        return "";
    }

    static public string tipoGiro90(int sensor)
    {
        /* 
        Função auxiliar para identificar se o giro é a partir de uma peça quadrada ou circular. Utiliza o fato de que uma deteção de uma peça circular tem mais linha preta na frente caso o robÔ continue andando.

        É utilizada para o robô não andar pra frente quando estiver percorrendo círculos, facilitando assim a leitura dos quadrados verdes.
        */

        bc.Move(0, 0);
        aux.Tick();
        bc.MoveFrontalRotations(100, 0.5f);
        if (sensor == 1)
        {
            mov.MoverNoCirculo(10);
            if (aux.MedirLuz(sensor) < 15)
            {
                mov.MoverNoCirculo(-10);
                return "quadrado";
            }
            else
            {
                return "circulo";
            }
        }
        else if (sensor == 2)
        {
            mov.MoverNoCirculo(-10);
            if (aux.MedirLuz(sensor) < 15)
            {
                mov.MoverNoCirculo(10);
                return "quadrado";
            }
            else
            {
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

        if (bc.Distance(1 - 1) > distance)
        {
            while (bc.Distance(1 - 1) > distance)
            {
                bc.MoveFrontal(velocidade, velocidade);
            }
        }
        else
        {
            while (bc.Distance(1 - 1) < distance)
            {
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
            bc.MoveFrontalRotations(200, (float)(distancia / 2.066));
        }
        else
        {
            bc.MoveFrontalRotations(-200, (float)(Math.Abs(distancia) / 2.066));

        }
        bc.Move(0, 0);

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

        if (anguloMovimento > 0)
        {
            do
            {
                bc.Move(velocidade, -velocidade);

                anguloGiro = matAng.MatematicaCirculo(bc.Compass() - anguloInicial);
            }
            while (anguloGiro < anguloMovimento || anguloGiro > anguloMovimento + 2);
        }
        else if (anguloMovimento < 0)
        {
            anguloMovimento = Math.Abs(anguloMovimento);
            do
            {
                bc.Move(-velocidade, velocidade);

                anguloGiro = matAng.MatematicaCirculo(anguloInicial - bc.Compass());
            }
            while (anguloGiro < anguloMovimento || anguloGiro > anguloMovimento + 2);
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

        if (angulo == 360) angulo = 0;

        if (angulo >= 0 && angulo < 360)
        {
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

