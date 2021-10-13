/*
    GPS
        Salvar localização saida

    Radar                        
        Bolinhas na frente durante radar            [concluido]
        Fazer triangulação considerando abismos     
        contagens de bolinhas                       [concluido]
        função pega bola preta                      
    
    Preocupações do kim
        Abismo na frente da linha do radar          [concluido]
        Bunda do robo no abismo (posicionarRadar)   [não faremos] [eu acho]
        bolinha perto da arena preta                usar bc.RobotSpeed() > 3) se for fazer
        
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
    static public void Resgate(float velocidadeReto = 290, float velocidadeGiro = 950)
    {
        bc.ActuatorSpeed(150);
        bc.CloseActuator();
        mov.MoverEscavadora(85);
        // while (bc.AngleActuator() < 86) { bc.ActuatorUp(10); }
        mov.MoverProAngulo(matAng.AproximarAngulo(bc.Compass()), velocidadeGiro);
        bc.MoveFrontal(0, 0);

        while (bc.distance(3 - 1) + bc.distance(2 - 1) > 15000)
        {
            bc.MoveFrontal(velocidadeReto, velocidadeReto);
            aux.Tick();
        }
        //bc.MoveFrontalRotations(200, 15);
        mov.MoverPorUnidadeRotacao(30);
        // bc.Wait(230);a

        bc.MoveFrontal(0, 0);
        bc.PrintConsole(0, "Correção finalizada");

        float[] mapa;
        mapa = new float[11];

        mapa = gps.Mapeamento();

        resgate.EntregarObjeto();
        gps.PosicionarRadar(mapa);

        resgate.Radar(mapa);
        bc.MoveFrontal(0, 0);
        bc.PrintConsole(0, "Saida!");
        bc.Wait(1000);

    }
    static public void Radar(float[] mapa, float velocidadeReto = 200, float velocidadeGiro = 950)
    {
        //Variavel para controle da direção 
        int direcao = -1;
        int parada = 0;
        bool KimPreocupado = false;

        //garra
        while (bc.AngleActuator() > 1) { bc.ActuatorDown(10); }
        bc.OpenActuator();

        //KimPreocupado
        if (bc.distance(1 - 1) > 300) { KimPreocupado = true; }

        while (true)
        {
            //Verifica a variação para indentificar bolinhas e ignorar o terreno
            bc.PrintConsole(0, " ");

            bc.Wait(5);

            float val1 = bc.distance(2 - 1); // Ultrassom da direita
            if (val1 > mapa[8]) { val1 = mapa[8]; }
            float val2 = bc.distance(3 - 1); // Ultrassom da esquerda
            if (val2 > mapa[9]) { val2 = mapa[9]; };

            bc.Wait(5);

            float val3 = bc.distance(2 - 1); // Ultrassom da direita
            if (val3 > mapa[8]) { val3 = mapa[8]; }
            float val4 = bc.distance(3 - 1); // Ultrassom da esquerda
            if (val4 > mapa[9]) { val4 = mapa[9]; }


            float variacaoDireita = (val1 - val3);
            float variacaoEsquerda = (val2 - val4);



            if (variacaoDireita > 7 || variacaoDireita < -7)
            {
                bc.MoveFrontal(0, 0);
                if (variacaoDireita < -7) { resgate.buscaBolinha("Direita", 1 * direcao, mapa); }
                if (variacaoDireita > 7) { resgate.buscaBolinha("Direita", -1 * direcao, mapa); }
                direcao = -1;
                parada = 0;


            }
            //Quando identificar variação, podendo ser bolinha ou kim preocupado
            if ((variacaoEsquerda > 7 || variacaoEsquerda < -7) && KimPreocupado == true && direcao > 0)
            {
                bc.PrintConsole(0, "vai");
                bc.MoveFrontalRotations(50, 10);
                bc.MoveFrontalRotations(0, 0);
                if (bc.distance(3 - 1) > mapa[9] + 50)
                {
                    bc.PrintConsole(0, "Bolinha nada");
                }
                else
                {
                    bc.PrintConsole(0, "volta");
                    bc.MoveFrontalRotations(-50, 10);
                    bc.MoveFrontalRotations(0, 0);
                    if (variacaoEsquerda < -7) { resgate.buscaBolinha("Esquerda", 1 * direcao, mapa); }
                    if (variacaoEsquerda > 7) { resgate.buscaBolinha("Esquerda", -1 * direcao, mapa); }
                    direcao = -1;
                    parada = 0;
                }

            }

            else if (variacaoEsquerda > 7 || variacaoEsquerda < -7)
            {
                if (variacaoEsquerda < -7) { resgate.buscaBolinha("Esquerda", 1 * direcao, mapa); }
                if (variacaoEsquerda > 7) { resgate.buscaBolinha("Esquerda", -1 * direcao, mapa); }
                direcao = -1;
                parada = 0;

            }

            if (bc.distance(3 - 1) > mapa[9] + 50 && bc.distance(2 - 1) > mapa[8] + 50 && KimPreocupado == true && direcao > 0)
            {
                bc.PrintConsole(0, "Abismo, uêpa");
                if (parada == 1) { parada = parada + 1; }
                direcao = -1;
                bc.MoveFrontalRotations(-200, 15);
            }
            //Se tiver bolinha na frente

            if (bc.HasVictim() == true)
            {
                bc.MoveFrontalRotations(200, 15);
                bc.CloseActuator();
                while (bc.AngleActuator() < 86) { bc.ActuatorUp(10); }
                mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(bc.Compass() - 90)), velocidadeGiro);
                gps.TracarRotaArena(mapa);
                direcao = -1;
                parada = 0;
            }

            //Se chegar na parede inverte a variavel de direção.
            if (bc.distance(1 - 1) < 30 || bc.Touch(1 - 1) == true)
            {
                bc.PrintConsole(0, "Parede, uêpa");
                parada = parada + 1;
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

            if (parada == 3 || mapa[10] == 3)
            {
                break;
            }

        }
    }
    static public void buscaBolinha(string lado, int passou, float[] mapa, float velocidadeGiro = 950, float velocidadeReto = 200)
    {
        bc.MoveFrontal(0, 0);
        bc.PrintConsole(0, "Situação: " + lado + " " + passou.ToString());
        bc.CloseActuator();
        while (bc.AngleActuator() < 86) { bc.ActuatorUp(10); }

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
            bc.Wait(300);
        }
        bc.MoveFrontal(0, 0);

        //Gira para o lado certo da bolinha
        float val = 0;

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

        if (val < 40)
        {
            bc.MoveFrontal(-200, -200);
            bc.Wait(400);
        }
        bc.MoveFrontal(0, 0);

        float atual = bc.distance(1 - 1);

        //Movimentação da garra

        while (bc.AngleActuator() > 1) { bc.ActuatorDown(10); }
        bc.OpenActuator();

        if (bc.distance(1 - 1) < 400)
        {
            while (bc.distance(1 - 1) > atual - val + 18 && bc.distance(1 - 1) > 30)
            {
                bc.MoveFrontal(velocidadeReto, velocidadeReto);
            }
            bc.MoveFrontal(200, 200);
            bc.Wait(500);

        }
        else
        {
            while (bc.HasVictim() == false) { bc.MoveFrontal(velocidadeReto, velocidadeReto); } // aqui o robo pega a bolinha pela boleana
            bc.MoveFrontalRotations(200, 15);
        }


        bc.MoveFrontal(0, 0);
        bc.CloseActuator();

        while (bc.AngleActuator() < 86) { bc.ActuatorUp(10); }
        bc.MoveFrontalRotations(-200, 10);

        gps.TracarRotaArena(mapa);

    }
    public static void EntregarObjeto()
    {
        bc.MoveFrontal(0, 0);
        bc.OpenActuator();
        while (bc.AngleActuator() > 20) { bc.ActuatorDown(10); }
        while (bc.AngleActuator() < 86) { bc.ActuatorUp(10); }
        bc.CloseActuator();

    }
    public static void EntregarBolinha()
    {
        bc.ActuatorSpeed(150);

        do
        {
            bc.Move(170, 170);
            bc.Wait(200);
        }
        while (bc.RobotSpeed() > 4);

        bc.OpenActuator();
        mov.MoverBalde(12);
        mov.MoverEscavadora(0);
        bc.Wait(300);
        mov.MoverEscavadora(30);
        mov.MoverEscavadora(0);
        bc.Wait(1000);

        bc.CloseActuator();

        mov.MoverEscavadora(85);
        mov.MoverBalde(0);

        if (bc.HasVictim() == true) { resgate.EntregarBolinha(); }
    }

    public static void GuardarBolinha(int lado)
    {
        int multiplo = 1;
        if (lado == 1) { multiplo = -1; };

        float distanciaIncial;

        mov.MoverNoCirculo(110 * multiplo);
        distanciaIncial = bc.Distance(0);

        if (distanciaIncial < 150) mov.MoverUltra(42, 100);
        else { mov.MoverPorUnidadeRotacao(65); };

        bc.Move(0, 0);
        bc.Wait(500);

        bc.ActuatorSpeed(20);
        mov.MoverEscavadora(0);

        bc.MoveFrontalRotations(50, 3);

        bc.Move(0, 0);
        bc.Wait(300);

        bc.MoveFrontalRotations(-200, 3);

        if (distanciaIncial < 150) mov.MoverUltra(distanciaIncial, 100);
        else { mov.MoverPorUnidadeRotacao(-65); };

        bc.ActuatorSpeed(150);
        mov.MoverEscavadora(85);
        mov.MoverNoCirculo(-110 * multiplo);

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
            index 7 = lado de verificação
            index 8 = Parede Esperada Direita
            index 9 = Parede Esperada Esquerda
            inex 10 = bolinhas entregadas

            === === === === === ===
        */

        float ultra1 = bc.Distance(1 - 1);
        float ultra2 = bc.Distance(2 - 1); //Direita
        float ultra3 = bc.Distance(3 - 1); //Esquerda
        float compas = bc.Compass();
        float[] mapa;
        mapa = new float[11];

        mapa[10] = 0;

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

        if (ultra2 + ultra3 >= 270 && ultra2 + ultra3 < 320) { mapa[0] = 400; mapa[1] = 300; mapa[5] = 1; bc.PrintConsole(5, "Mapa 5 " + mapa[5].ToString()); } //soma para 400 arena = 278

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
        if (ultra2 < 400)
        {
            //Se não tiver abismo vira pra direita
            mov.MoverPorUnidadeRotacao(14);
            bc.MoveFrontal(0, 0);
            aux.Tick();
            mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(compas + 90)), velocidadeGiro);
            bc.MoveFrontal(0, 0);
            aux.Tick();

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
                    Abismo();

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
                    Abismo();

                }

            }
            //certeza de falta de arena na linha
            else
            {
                while (bc.distance(1 - 1) > 16) { bc.Move(velocidadeReto, velocidadeReto); }
                mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(bc.Compass() - 90)), velocidadeGiro);
                Abismo();

                if (bc.ReturnColor(3 - 1) == "BLACK")
                {
                    mapa[5] = 0;
                    mapa[6] = 2;
                    bc.PrintConsole(5, "Mapa 5 " + mapa[5].ToString());
                    bc.PrintConsole(6, "Mapa 6 " + mapa[6].ToString());

                }
                else
                {
                    mapa[5] = 0;
                    mapa[6] = 1;
                    bc.PrintConsole(4, "Mapa 4 " + mapa[4].ToString());
                    bc.PrintConsole(5, "Mapa 5 " + mapa[5].ToString());
                    bc.PrintConsole(6, "Mapa 6 " + mapa[6].ToString());
                    if (bc.distance(1 - 1) > 900) { while (bc.distance(1 - 1) > 16) { bc.Move(velocidadeReto, velocidadeReto); } }
                    mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(bc.Compass() - 90)), velocidadeGiro);
                    Abismo();

                }
            }

        }
        //Se tiver abismo na direita
        else
        {
            //Salvar saida
            mov.MoverPorUnidadeRotacao(14);
            bc.MoveFrontal(0, 0);
            aux.Tick();
            mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(compas - 90)), velocidadeGiro);
            //Se tiver arena no 0
            if (ultra3 < bc.distance(1 - 1) - 10)
            {
                mapa[3] = ultra3 - 33 + 85;
                mapa[4] = mapa[3] + 100;
                mapa[5] = 1;
                mapa[6] = 0;
                bc.PrintConsole(3, "Mapa 3 " + mapa[3].ToString());
                bc.PrintConsole(4, "Mapa 4 " + mapa[4].ToString());
                bc.PrintConsole(5, "Mapa 5 " + mapa[5].ToString());
                bc.PrintConsole(6, "Mapa 6 " + mapa[6].ToString());

                Abismo();

            }
            //Se não tiver, pecorre o mapa
            else
            {
                mapa[5] = 0;
                bc.PrintConsole(5, "Mapa 5 " + mapa[5].ToString());
                while (bc.distance(1 - 1) > 16) { bc.Move(velocidadeReto, velocidadeReto); }

                mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(bc.Compass() + 90)), velocidadeGiro);

                Abismo();

                //Se tiver arena no 1
                if (bc.ReturnColor(3 - 1) == "BLACK")
                {
                    mapa[6] = 1;
                    bc.PrintConsole(6, "Mapa 6 " + mapa[6].ToString());

                }
                //Se não, tem arena no 2
                else
                {
                    mapa[6] = 2;
                    bc.PrintConsole(6, "Mapa 6 " + mapa[6].ToString());

                    if (bc.distance(1 - 1) > 900) { while (bc.distance(1 - 1) > 16) { bc.Move(velocidadeReto, velocidadeReto); } }
                    mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(bc.Compass() + 90)), velocidadeGiro);

                    Abismo();

                }
            }
        }
        //Mapeamento do lado de verificação
        if (mapa[6] == 0)
        {
            if (mapa[0] == 300) { mapa[7] = 0; }
            if (mapa[0] == 400) { mapa[7] = 1; }
        }
        if (mapa[6] == 1)
        {
            if (mapa[0] == 300) { mapa[7] = 1; }
            if (mapa[0] == 400) { mapa[7] = 0; }
        }
        if (mapa[6] == 2)
        {
            if (mapa[0] == 300) { mapa[7] = 0; }
            if (mapa[0] == 400) { mapa[7] = 1; }
        }
        if (mapa[6] == 3)
        {
            if (mapa[0] == 300) { mapa[7] = 1; }
            if (mapa[0] == 400) { mapa[7] = 0; }
        }
        //Parede Esperada
        if (mapa[7] == 0) { mapa[9] = 269; mapa[8] = 99; }
        if (mapa[7] == 1) { mapa[9] = 99; mapa[8] = 269; }


        return mapa;
    }
    public static void Abismo(float velocidadeGiro = 950, float velocidadeReto = 290)
    {
        if (bc.distance(1 - 1) > 9000)
        {
            while (bc.distance(3 - 1) < 900 && bc.distance(2 - 1) < 900)
            {
                bc.Move(velocidadeReto, velocidadeReto);
            }
            mov.MoverPorUnidadeRotacao(12.5f);
            // bc.Move(-velocidadeReto, -velocidadeReto);
            // bc.Wait(250);
            bc.Move(0, 0);
        }
        else
        {
            while (bc.ReturnColor(3 - 1) != "BLACK" && bc.distance(1 - 1) > 16) { bc.Move(velocidadeReto, velocidadeReto); }
            bc.Move(0, 0);
        }

    }
    public static void PosicionarRadar(float[] mapa, float velocidadeReto = 290, float velocidadeGiro = 950)
    {
        float compas = matAng.AproximarAngulo(bc.compass());

        if (mapa[5] == 1)
        {
            if (mapa[0] == 300)
            {
                mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] + 180)), velocidadeGiro);
                while (bc.distance(1 - 1) < 90)
                {
                    bc.Move(-velocidadeReto, -velocidadeReto);
                }

                bc.Move(0, 0);
                if (mapa[6] == 3)
                {
                    mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] - 90)), velocidadeGiro);
                }
                if (mapa[6] == 0)
                {
                    mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] + 90)), velocidadeGiro);
                }
                while (bc.Touch(1 - 1) == false)
                {
                    bc.Move(-velocidadeReto, -velocidadeReto);
                }
                //não fazer radar na frente de abismo
            }

            if (mapa[0] == 400)
            {
                if (mapa[6] == 3)
                {
                    mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] + 90)), velocidadeGiro);
                }
                if (mapa[6] == 0)
                {
                    mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] - 90)), velocidadeGiro);
                }
                while (bc.distance(1 - 1) < 90)
                {
                    bc.Move(-velocidadeReto, -velocidadeReto);
                }
                mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2])), velocidadeGiro);
                while (bc.Touch(1 - 1) == false)
                {
                    bc.Move(-velocidadeReto, -velocidadeReto);
                }

            }
        }
        if (mapa[5] == 0)
        {
            if (mapa[0] == 300)
            {
                //arena 1
                if (mapa[6] == 1)
                {
                    mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2])), velocidadeGiro);
                    while (bc.distance(1 - 1) < 90)
                    {
                        bc.Move(-velocidadeReto, -velocidadeReto);
                    }
                    mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] + 90)), velocidadeGiro);
                }

                //arena 2
                if (mapa[6] == 2)
                {
                    mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2])), velocidadeGiro);
                    while (bc.distance(1 - 1) < 90)
                    {
                        bc.Move(-velocidadeReto, -velocidadeReto);
                    }
                    mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] - 90)), velocidadeGiro);
                }
                while (bc.Touch(1 - 1) == false)
                {
                    bc.Move(-velocidadeReto, -velocidadeReto);
                }
            }

            if (mapa[0] == 400)
            {
                //arena 1
                if (mapa[6] == 1)
                {
                    mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] - 90)), velocidadeGiro);
                    while (bc.distance(1 - 1) < 90)
                    {
                        bc.Move(-velocidadeReto, -velocidadeReto);
                    }
                    mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] - 180)), velocidadeGiro);
                }
                //arena 2
                if (mapa[6] == 2)
                {
                    mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] + 90)), velocidadeGiro);
                    while (bc.distance(1 - 1) < 90)
                    {
                        bc.Move(-velocidadeReto, -velocidadeReto);
                    }
                    mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] - 180)), velocidadeGiro);
                }
                while (bc.Touch(1 - 1) == false)
                {
                    bc.Move(-velocidadeReto, -velocidadeReto);
                }
            }
        }
        bc.Move(0, 0);

    }

    public static void MoverNoTriangulo(float y, float x, float anguloFinal, float velocidadeGiro = 950, float velocidadeReto = 290)
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

        float ultra1 = bc.Distance(1 - 1);

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
                while (bc.distance(1 - 1) > ultra1 - h && bc.distance(1 - 1) > 30 && (bc.distance(1 - 1) > 67 || bc.ReturnColor(3 - 1) != "BLACK"))
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
            mov.MoverPorUnidadeRotacao(-h);
        }
        mov.MoverProAngulo(anguloFinal);

    }

    public static void TracarRotaArena(float[] mapa, float velocidadeReto = 290, float velocidadeGiro = 950)
    {
        float ultra1 = bc.distance(1 - 1);
        float ultra2 = bc.distance(2 - 1);
        float ultra3 = bc.distance(3 - 1);
        float y = 0;
        float x = 0;
        float anguloFinal = 0;
        float compas = matAng.AproximarAngulo(bc.compass());

        if (mapa[6] == 0)
        {
            //400
            if (compas == matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] - 90)))
            {
                y = ultra1 - 50;

                mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] - 180)), velocidadeGiro);
                x = (bc.distance(1 - 1) - 50) * -1;

                if (y > 350)
                {
                    mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] + 90)), velocidadeGiro);
                    y = 355 - bc.distance(1 - 1) - 50;
                }

                if (x > 350)
                {
                    mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2])), velocidadeGiro);
                    x = (255 - bc.distance(1 - 1) - 50) * -1;
                }
                mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] - 90)), velocidadeGiro);
                anguloFinal = matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] - 90 - 45), 2);
            }
            if (compas == matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] + 90)))
            {
                y = (ultra1 - 50) * -1;

                mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] + 180)), velocidadeGiro);
                x = bc.distance(1 - 1) - 50;

                if (y > 350)
                {
                    mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] + 180)), velocidadeGiro);
                    y = (355 - bc.distance(1 - 1) - 50) * -1;
                }

                if (x > 350)
                {
                    mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2])), velocidadeGiro);
                    x = 255 - bc.distance(1 - 1) - 50;
                }
                mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] + 90)), velocidadeGiro);

                anguloFinal = matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] - 90 - 45), 2);
            }

            //300 estamos aqui
            if (compas == matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2])))
            {
                y = (355 - ultra1 - 50) * -1;

                mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] - 90)), velocidadeGiro);
                x = (bc.distance(1 - 1) - 50) * -1;

                if (y > 350)
                {
                    mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] + 180)), velocidadeGiro);
                    y = -bc.distance(1 - 1);
                }

                if (x > 350)
                {
                    mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] + 90)), velocidadeGiro);
                    x = (255 - bc.distance(1 - 1) - 50) * -1;
                }
                mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2])), velocidadeGiro);

                anguloFinal = matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] - 90 - 45), 2);

            }
            if (compas == matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] - 180)))
            {
                y = ultra1 - 50;

                mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] + 90)), velocidadeGiro);
                x = bc.distance(1 - 1) - 50;

                if (y > 350)
                {
                    mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] + 180)), velocidadeGiro);
                    y = 355 - bc.distance(1 - 1) - 50;
                }

                if (x > 350)
                {
                    mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] + 90)), velocidadeGiro);
                    x = 255 - bc.distance(1 - 1) - 50;
                }
                mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] - 180)), velocidadeGiro);

                anguloFinal = matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] - 90 - 45), 2);
            }


        }
        if (mapa[6] == 1)
        {
            if (compas == matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] - 90)))
            {
                y = ultra1 - 50;

                mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] + 180)), velocidadeGiro);
                x = 255 - bc.distance(1 - 1) - 50;

                if (y > 350)
                {
                    mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] + 90)), velocidadeGiro);
                    y = 355 - bc.distance(1 - 1) - 50;
                }

                if (x > 350)
                {
                    mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2])), velocidadeGiro);
                    x = bc.distance(1 - 1) - 50;
                }
                mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] - 90)), velocidadeGiro);
                anguloFinal = matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] - 45), 2);
            }
            if (compas == matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] + 90)))
            {
                y = (355 - ultra1 - 50) * -1;

                mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] + 180)), velocidadeGiro);
                x = (255 - bc.distance(1 - 1) - 50) * -1;

                if (y > 350)
                {
                    mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] - 90)), velocidadeGiro);
                    y = (bc.distance(1 - 1) - 50) * -1;
                }

                if (x > 350)
                {
                    mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2])), velocidadeGiro);
                    x = (bc.distance(1 - 1) - 50) * -1;
                }
                mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] + 90)), velocidadeGiro);
                anguloFinal = matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] - 45), 2);
            }

            if (compas == matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2])))
            {
                y = ultra1 - 50;

                mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] - 90)), velocidadeGiro);
                x = (bc.distance(1 - 1) - 50) * -1;

                if (y > 350)
                {
                    mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] - 180)), velocidadeGiro);
                    y = 355 - bc.distance(1 - 1) - 50;
                }

                if (x > 350)
                {
                    mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] + 90)), velocidadeGiro);
                    x = (255 - bc.distance(1 - 1) - 50) * -1;
                }
                mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2])), velocidadeGiro);
                anguloFinal = matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] - 45), 2);

            }
            if (compas == matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] - 180)))
            {
                y = (355 - ultra1 - 50) * -1;

                mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] - 90)), velocidadeGiro);
                x = bc.distance(1 - 1) - 50;

                if (y > 350)
                {
                    mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2])), velocidadeGiro);
                    y = (355 - bc.distance(1 - 1) - 50) * -1;
                }

                if (x > 350)
                {
                    mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] + 90)), velocidadeGiro);
                    x = 255 - bc.distance(1 - 1) - 50;
                }
                mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] - 180)), velocidadeGiro);
                anguloFinal = matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] - 45), 2);
            }


        }
        if (mapa[6] == 2)
        {
            if (compas == matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] + 90)))
            {
                y = ultra1 - 50;

                mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2])), velocidadeGiro);
                x = (bc.distance(1 - 1) - 50) * -1;

                if (y > 350)
                {
                    mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] - 90)), velocidadeGiro);
                    y = 355 - bc.distance(1 - 1) - 50;
                }

                if (x > 350)
                {
                    mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] + 180)), velocidadeGiro);
                    x = (255 - bc.distance(1 - 1) - 50) * -1;
                }
                mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] + 90)), velocidadeGiro);
                anguloFinal = matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] + 45), 2);
            }
            if (compas == matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] - 90)))
            {
                y = (355 - ultra1 - 50) * -1;

                mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2])), velocidadeGiro);
                x = bc.distance(1 - 1) - 50;

                if (y > 350)
                {
                    mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] + 90)), velocidadeGiro);
                    y = (bc.distance(1 - 1) - 50) * -1;
                }

                if (x > 350)
                {
                    mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] - 180)), velocidadeGiro);
                    x = 255 - bc.distance(1 - 1) - 50;
                }
                mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] - 90)), velocidadeGiro);
                anguloFinal = matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] + 45), 2);
            }

            if (compas == matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2])))
            {
                y = ultra1 - 50;

                mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] + 90)), velocidadeGiro);
                x = bc.distance(1 - 1) - 50;

                if (y > 350)
                {
                    mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] + 180)), velocidadeGiro);
                    y = 355 - bc.distance(1 - 1) - 50;
                }

                if (x > 350)
                {
                    mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] - 90)), velocidadeGiro);
                    x = 255 - bc.distance(1 - 1) - 50;
                }
                mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2])), velocidadeGiro);
                anguloFinal = matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] + 45), 2);

            }
            if (compas == matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] - 180)))
            {
                y = (355 - ultra1 - 50) * -1;

                mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] - 90)), velocidadeGiro);
                x = (255 - bc.distance(1 - 1) - 50) * -1;

                if (y > 350)
                {
                    mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2])), velocidadeGiro);
                    y = (bc.distance(1 - 1) - 50) * -1;
                }

                if (x > 350)
                {
                    mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] + 90)), velocidadeGiro);
                    x = (bc.distance(1 - 1) - 50) * -1;
                }
                mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] - 180)), velocidadeGiro);
                anguloFinal = matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] + 45), 2);
            }
        }
        if (mapa[6] == 3)
        {

            if (compas == matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] - 90)))
            {
                y = (355 - ultra1 - 50) * -1;

                mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] - 180)), velocidadeGiro);
                x = (bc.distance(1 - 1) - 50) * -1;

                if (y > 350)
                {
                    mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] + 90)), velocidadeGiro);
                    y = (bc.distance(1 - 1) - 50) * -1;
                }

                if (x > 350)
                {
                    mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2])), velocidadeGiro);
                    x = (255 - bc.distance(1 - 1) - 50) * -1;
                }
                mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] - 90)), velocidadeGiro);
                anguloFinal = matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] + 90 + 45), 2);
            }
            if (compas == matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] + 90)))
            {
                y = ultra1 - 50;

                mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] + 180)), velocidadeGiro);
                x = bc.distance(1 - 1) - 50;

                if (y > 350)
                {
                    mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] - 90)), velocidadeGiro);
                    y = 355 - bc.distance(1 - 1) - 50;
                }

                if (x > 350)
                {
                    mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2])), velocidadeGiro);
                    x = 255 - bc.distance(1 - 1) - 50;
                }
                mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] + 90)), velocidadeGiro);
                anguloFinal = matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] + 90 + 45), 2);
            }

            if (compas == matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2])))
            {
                y = (355 - ultra1 - 50) * -1;

                mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] + 90)), velocidadeGiro);
                x = bc.distance(1 - 1) - 50;

                if (y > 350)
                {
                    mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] - 180)), velocidadeGiro);
                    y = (bc.distance(1 - 1) - 50) * -1;
                }

                if (x > 350)
                {
                    mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] - 90)), velocidadeGiro);
                    x = 255 - bc.distance(1 - 1) - 50;
                }
                mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2])), velocidadeGiro);
                anguloFinal = matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] + 90 + 45), 2);

            }
            if (compas == matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] - 180)))
            {
                y = ultra1 - 50;

                mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] + 90)), velocidadeGiro);
                x = (bc.distance(1 - 1) - 50) * -1;

                if (y > 350)
                {
                    mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2])), velocidadeGiro);
                    y = 355 - bc.distance(1 - 1) - 50;
                }

                if (x > 350)
                {
                    mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] - 90)), velocidadeGiro);
                    x = (255 - bc.distance(1 - 1) - 50) * -1;
                }
                mov.MoverProAngulo(matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] - 180)), velocidadeGiro);
                anguloFinal = matAng.AproximarAngulo(matAng.MatematicaCirculo(mapa[2] + 90 + 45), 2);
            }
        }
        gps.MoverNoTriangulo(y, x, anguloFinal);

        if (bc.Heat() > 38 && bc.HasVictim() == true)
        {
            resgate.EntregarBolinha();
            mapa[10] = mapa[10] + 1;
        }
        else if (bc.HasVictim())
        {
            if (mapa[10] >= 2) { resgate.EntregarBolinha(); }
            else { resgate.GuardarBolinha((int)mapa[7]); }
            mapa[10] = mapa[10] + 1;
        }
        gps.PosicionarRadar(mapa);


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
            if (bc.AngleActuator() > alvoEscavadora && bc.AngleScoop() < 120)
            {
                while (bc.AngleActuator() > alvoEscavadora)
                {
                    bc.ActuatorDown(10);
                }
            }

            else
            {
                //enquanto o seno da posicao atual da escavadora for maior q o seno da posicao alvo, a escavadora desce
                while (bc.AngleActuator() < alvoEscavadora && bc.AngleScoop() < 120)
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

