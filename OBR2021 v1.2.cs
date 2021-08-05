/*
Título      :   OBR 2021 Funções de regate de vítimas
Autor       :   Mauro Moledo
Versão      :   1.2
Data scrum  :   04/08
Alterações  :   

                FALTA(muito importante): Conseguir indetificar a faixa verde de saida @kim
                */

// === === === FUNÇÕES RESGATE DE VÍTIMAS === === === ///
void BolinhaNaGuela()
{
    //== Valores de inclinação atuador. Garra abaixada para empurrar bolinha no inicio da area de resgate==//
    /* 
    AngleActuator() = Atuador
    AngleScoop()    = Balde

    Atuador === 290° -> 0/360° -> 10°
    Balde   === 10° -> 0/360° -> 320° (Fundo vai pra trás) 
    */

    bc.MoveFrontal(-velocidade, -velocidade);
    bc.Wait(500);
    bc.MoveFrontal(0, 0);

    while (bc.AngleActuator() > 100 || bc.AngleActuator() < 10)
    {
        bc.ActuatorDown(10);

    }
    while (bc.AngleScoop() < 100 || bc.AngleScoop() > 320)
    {

        bc.TurnActuatorDown(10);
    }

    bc.MoveFrontal(velocidade, velocidade);
    bc.Wait(1200);
    bc.MoveFrontal(0, 0);

    PosicionarGarraAlto();

}

void PosicionarGarraAlto()
{
    //== Valores de inclinação atuador. Garra levantada para se movimentar ==//
    /* 
    AngleActuator() = Atuador
    AngleScoop()    = Balde

    Atuador   === 5° -> 0/360° -> 290° 
    Balde     === 355° -> 0/360° -> 10°  (Fundo vai pra frente)
    */
    while (bc.AngleActuator() > 291 || bc.AngleActuator() < 100)
    {
        bc.ActuatorUp(10);

    }
    while (bc.AngleScoop() < 10 || bc.AngleScoop() > 100)
    {
        bc.TurnActuatorUp(10);
    }
}

void PosicionarGarraBaixo()
{
    //== Valores de inclinação atuador. Garra levantada para se movimentar ==//
    /* 
    AngleActuator() = Atuador
    AngleScoop()    = Balde

    Atuador   === 290° --> 360/0° -> 11°
    */
    while (bc.AngleActuator() > 100 || bc.AngleActuator() < 11)
    {
        bc.ActuatorDown(10);

    }
}

void AnguloArena()
{
    float set = bc.Compass();
    if (set > 80 && set < 100)
    {
        anguloAreaSoma = 90;
        anguloAreaMenos = 90;
    }
    else if (set > 170 && set < 190)
    {
        anguloAreaSoma = 180;
        anguloAreaMenos = 180;
    }
    else if (set > 260 && set < 280)
    {
        anguloAreaSoma = 270;
        anguloAreaMenos = 270;
    }
    else if (set > 350 || set < 10)
    {
        anguloAreaSoma = 0;
        anguloAreaMenos = 360;
    }
    else
    {
        anguloAreaSoma = set;
        anguloAreaMenos = set;
    }

}
void Tick() { bc.Wait(30); }

void RetornarCirculo(float anguloMovimento, float velocidade)
{
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
                Tick();
            }
        }
        // Movimento regular
        else
        {
            while (bc.Compass() < anguloInicial + anguloMovimento)
            {
                bc.MoveFrontal(-velocidade, velocidade);
                Tick();
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
                Tick();
            }
        }
        //Movimento regular
        else
        {
            while (bc.Compass() > anguloInicial - anguloMovimento)
            {
                bc.MoveFrontal(velocidade, -velocidade);
                Tick();
            }
        }
    }
}


void IdentificarArea()
{
    //== Valores de distância do ultrassom area 3 ==//
    /* 
    Parede  === Maior que 232 
    Area   === Entre 160 e 210 - Pode ser bolinha tb!
    Bolinha === Menos de 160

    == Para angulo com alteração de 10°: ==
    Parede  === Maior que 194 
    Area   === Entre 140 e 160
    Bolinha === Caso não considerado
    */


    bc.PrintConsole(1, anguloAreaSoma.ToString());

    if (bc.Distance(3 - 1) > 232)
    {                              // Caso 1: Parede com certeza (Parede  === Entre 232 e 240)
        bc.PrintConsole(2, "Parede na Esquerda. Entre 232 e 240. Caso 1");
    }
    else if (bc.Distance(3 - 1) < 160)
    {                          //Caso 2: Identificou bolinha (Bolinha === Menos de 160)
        while (bc.Compass() > anguloAreaMenos - 9)
        {
            bc.MoveFrontal(velocidadeGiro, -velocidadeGiro);    // Gira 10°
        }
        if (bc.Distance(3 - 1) < 160 && bc.Distance(3 - 1) > 140)
        { // Area com certeza (Area  === Entre 140 e 160 com 10°)
            bc.PrintConsole(3, "Area na Esquerda. Entre 140 e 160 com 10°. Caso 2");
            area = "Esquerda";
            while (bc.Compass() < anguloAreaMenos)
            {
                bc.MoveFrontal(-velocidadeGiro, velocidadeGiro);

                if (bc.Distance(2 - 1) > 900)
                { //aproveita para verificar a direita
                    saida = "Direita";
                    bc.PrintConsole(4, "Saida Direita");
                }
            }
        }
        else if (bc.Distance(3 - 1) > 194)
        {                      // Parede com certeza (Parede  === Maior que 194 com 10°)
            bc.PrintConsole(2, "Parede na Esquerda. Maior que 194 com 10°. Caso 2");
            while (bc.Compass() < anguloAreaMenos)
            {
                bc.MoveFrontal(-velocidadeGiro, velocidadeGiro);

                if (bc.Distance(2 - 1) > 900)
                { //aproveita para verificar a direita
                    saida = "Direita";
                    bc.PrintConsole(4, "Saida Direita");
                }
            }
        }
    }
    else if (bc.Distance(3 - 1) < 210 && bc.Distance(3 - 1) > 160)
    { //Caso 3: Pode ser bolinha ou area (Area   === Entre 160 e 210)
        while (bc.Compass() > anguloAreaMenos - 9)
        {
            bc.MoveFrontal(velocidadeGiro, -velocidadeGiro);     // Gira 10°
        }
        if (bc.Distance(3 - 1) < 170 && bc.Distance(3 - 1) > 140)
        {  // Area com certeza (Area  === Entre 140 e 194 com 10°)

            bc.PrintConsole(3, "Area na Esquerda. Entre 140 e 160 com 10°. Caso 3");
            area = "Esquerda";

            while (bc.Compass() < anguloAreaMenos - 1 && anguloAreaSoma > 3)
            {
                bc.MoveFrontal(-velocidadeGiro, velocidadeGiro);

                if (bc.Distance(2 - 1) > 900)
                { //aproveita para verificar a direita
                    saida = "Direita";
                    bc.PrintConsole(4, "Saida Direita");
                }
            }


        }
        else if (bc.Distance(3 - 1) > 194)
        {  // Parede com certeza (Parede  === Maior que 194 com 10°)
            bc.PrintConsole(2, "Parede na Esquerda. Maior que 194 com 10°. Caso 3");

            while (bc.Compass() < anguloAreaMenos - 1 && anguloAreaSoma > 3)
            {
                bc.MoveFrontal(-velocidadeGiro, velocidadeGiro);
                if (bc.Distance(2 - 1) > 900)
                { //aproveita para verificar a direita
                    saida = "Direita";
                    bc.PrintConsole(4, "Saida Direita");
                }
            }
        }

    }


}

void IdentificarSaida()
{
    //== Identifica saida ==//

    while (true)
    {
        if (bc.Distance(1 - 1) > 900 && bc.Compass() < anguloAreaSoma + 50)
        {
            saida = "Frontal-Direita";
            bc.PrintConsole(4, "Saida Frontal-Direita");
            if (area != "Esquerda")
            {
                bc.PrintConsole(3, "Area Direita");
                area = "Direita";
            }
            break;
        }
        else if (bc.Distance(1 - 1) > 900 && bc.Compass() > anguloAreaSoma + 75 || saida == "Direita")
        {
            saida = "Direita";
            bc.PrintConsole(4, "Saida Direita");
            if (area != "Esquerda")
            {
                bc.PrintConsole(3, "Area Frontal-Direita");
                area = "Frontal-Direita";
            }
            break;
        }
        else if (bc.Compass() > anguloAreaSoma + 83)
        {
            saida = "Esquerda";
            bc.PrintConsole(4, "Saida Esquerda");
            area = "Caso Especial";
            break;
        }
        else
        {
            bc.MoveFrontal(-velocidadeGiro, velocidadeGiro);
        }
    }

    bc.MoveFrontal(0, 0);


    //== Gira para ir para o centro e verifica o caso especial
    if (area == "Caso Especial")
    {
        PosicionarGarraBaixo();

        if (bc.Compass() > anguloAreaSoma + 45)
        {
            while (bc.Compass() > anguloAreaSoma + 45)
            {
                bc.MoveFrontal(velocidadeGiro, -velocidadeGiro);
            }
        }
        else
        {
            while (bc.Compass() < anguloAreaSoma + 45)
            {
                bc.MoveFrontal(-velocidadeGiro, velocidadeGiro);
            }
        }

        while (bc.Distance(1 - 1) > 100)
        {
            bc.MoveFrontal(velocidade, velocidade);
        }

        bc.MoveFrontal(0, 0);

        if (bc.HasVictim())
        {
            bc.ActuatorSpeed(100);
            MoverEscavadora(290);
            MoverBalde(318);
            bc.PrintConsole(1, "Capturado");
            bc.Wait(100);
            bc.ActuatorSpeed(150);
        }
        else
        {
            PosicionarGarraAlto();
        }

        if (bc.Distance(3 - 1) < 62)
        {
            bc.PrintConsole(3, "Area Frontal-DireitaC");
            area = "Frontal-DireitaC";
            if (bc.HasVictim())
            {
                // Entrega Bolinha
                while ((bc.Distance(3 - 1) > 4) && (bc.Distance(1 - 1) < 85) && (bc.ReturnColor(6 - 1) != "BLACK" || bc.ReturnColor(6 - 1) != "PRETO"))
                {
                    bc.MoveFrontal(velocidade, velocidade);
                }
                bc.MoveFrontal(0, 0);

                // === Movimentação da Garra ===
                if (bc.HasVictim()) { Devolver(); }
                PosicionarGarraAlto();
                // === === ===
            }
        }
        else
        {
            bc.PrintConsole(3, "Area DireitaC");
            area = "DireitaC";
            if (bc.HasVictim())
            {
                PosicionarMeio();
                // Entrega Bolinha
                while (bc.Compass() < anguloAreaSoma + 90 + 45)
                {
                    bc.MoveFrontal(-velocidadeGiro, velocidadeGiro);
                }
                while ((bc.Distance(3 - 1) > 4) && (bc.Distance(1 - 1) < 85) && (bc.ReturnColor(6 - 1) != "BLACK" || bc.ReturnColor(6 - 1) != "PRETO"))
                {
                    bc.MoveFrontal(velocidade, velocidade);
                }
                bc.MoveFrontal(0, 0);

                // === Movimentação da Garra ===
                if (bc.HasVictim()) { Devolver(); }
                PosicionarGarraAlto();
                // === === ===
            }


        }

    }

}

void PosicionarMeio()
{
    if (area == "DireitaC" || area == "Frontal-DireitaC")
    {

        while (bc.Distance(1 - 1) < 155)
        {
            bc.MoveFrontal(-velocidade, -velocidade);
        }
        bc.MoveFrontal(0, 0);

    }

    else if (bc.Compass() > anguloAreaSoma + 45)
    {
        while (bc.Compass() > anguloAreaSoma + 45)
        {
            bc.MoveFrontal(velocidadeGiro, -velocidadeGiro);
        }

        while (bc.Distance(1 - 1) > 152)
        {
            bc.MoveFrontal(velocidade, velocidade);
        }
        bc.MoveFrontal(0, 0);
    }
    else
    {
        while (bc.Compass() < anguloAreaSoma + 45)
        {
            bc.MoveFrontal(-velocidadeGiro, velocidadeGiro);
        }

        while (bc.Distance(1 - 1) > 152)
        {
            bc.MoveFrontal(velocidade, velocidade);
        }
        bc.MoveFrontal(0, 0);
    }

}

void Radar()
{
    bool stop = false;
    float radarAnguloInicial = bc.Compass();
    while (true)
    {
        bc.PrintConsole(0, " ");

        float diferenca1 = bc.Distance(1 - 1) - bc.Distance(3 - 1);
        bc.Wait(20);
        float diferenca2 = bc.Distance(1 - 1) - bc.Distance(3 - 1);

        bc.PrintConsole(1, (diferenca1).ToString());
        bc.PrintConsole(2, (diferenca2).ToString());

        if (diferenca2 < 7.5)
        {
            bc.MoveFrontal(-900, 900);
        }

        else if (diferenca2 > 900 && diferenca2 < 9850)
        {
            bc.MoveFrontal(-900, 900);
        }

        else if (diferenca1 != 0 && diferenca2 > 900)
        {
            bc.MoveFrontal(-900, 900);
        }

        else if (diferenca2 - diferenca1 > 6)
        {
            bc.Wait(150);
            bc.MoveFrontal(0, 0);
            bc.PrintConsole(0, "===Bolinha==="); //achou bolinha
            EntregaBolinha();
            stop = false; // seta a varivel
        }

        else
        {
            bc.MoveFrontal(-900, 900);
        }

        if (bc.Compass() > radarAnguloInicial && bc.Compass() < radarAnguloInicial + 2 && stop == true) //blind spot
        {
            break;
        }
        else if (bc.Compass() > radarAnguloInicial + 2 && bc.Compass() < radarAnguloInicial + 4) //blind spot para setar a variavel
        {
            stop = true;
        }
    }
}

void EntregaBolinha()
{

    float ultra1Inicial = bc.Distance(1 - 1);
    float ultra3Inicial = bc.Distance(3 - 1);

    if (bc.Distance(3 - 1) < 31)
    {

        while (bc.Distance(3 - 1) < 30)
        {
            bc.MoveFrontal(-velocidade, -velocidade);
        }
        bc.MoveFrontal(0, 0);

    }
    else
    {
        while (bc.Distance(3 - 1) > 31)
        {
            bc.MoveFrontal(velocidade, velocidade);
        }
        bc.MoveFrontal(0, 0);
    }

    // === Movimentação da garra ===
    bool hasVictim = Pegar();
    // === === ===

    if (hasVictim)
    {
        while (bc.Distance(1 - 1) < ultra1Inicial)
        {
            bc.MoveFrontal(-velocidadeBaixa, -velocidadeBaixa);
        }

        if (area == "Direita" && bc.Compass() > anguloAreaSoma + 45 + 90 || area == "DireitaC" && bc.Compass() > anguloAreaSoma + 45 + 90)
        {
            while (bc.Compass() < anguloAreaSoma + 90 + 45)
            {
                bc.MoveFrontal(velocidadeGiro, -velocidadeGiro);
            }
        }
        else if (area == "Direita" && bc.Compass() < anguloAreaSoma + 45 + 90 || area == "DireitaC" && bc.Compass() < anguloAreaSoma + 45 + 90)
        {
            while (bc.Compass() < anguloAreaSoma + 90 + 45)
            {
                bc.MoveFrontal(-velocidadeGiro, velocidadeGiro);
            }
        }
        else if (area == "Frontal-Direita" && bc.Compass() > anguloAreaSoma + 45 || area == "Frontal-DireitaC" && bc.Compass() > anguloAreaSoma + 45)
        {
            while (bc.Compass() > anguloAreaSoma + 45)
            {
                bc.MoveFrontal(velocidadeGiro, -velocidadeGiro);
            }
        }
        else if (area == "Frontal-Direita" && bc.Compass() < anguloAreaSoma + 45 || area == "Frontal-DireitaC" && bc.Compass() < anguloAreaSoma + 45)
        {
            while (bc.Compass() < anguloAreaSoma + 45)
            {
                bc.MoveFrontal(-velocidadeGiro, velocidadeGiro);
            }
        }

        else if (area == "Esquerda" && bc.Compass() < anguloAreaMenos - 45)
        {
            while (bc.Compass() < anguloAreaMenos - 45)
            {
                bc.MoveFrontal(-velocidadeGiro, velocidadeGiro);
            }
        }
        else if (area == "Esquerda" && bc.Compass() > anguloAreaMenos - 45)
        {
            while (bc.Compass() > anguloAreaMenos - 45)
            {
                bc.MoveFrontal(velocidadeGiro, -velocidadeGiro);
            }
        }

        ultra1Inicial = bc.Distance(1 - 1);

        while ((bc.Distance(3 - 1) > 4) && (bc.Distance(1 - 1) < 85) && (bc.ReturnColor(6 - 1) != "BLACK" || bc.ReturnColor(6 - 1) != "PRETO"))
        {
            bc.MoveFrontal(velocidade, velocidade);
        }
        bc.MoveFrontal(0, 0);

        // === Movimentação da Garra ===
        if (bc.HasVictim()) { Devolver(); }
        PosicionarGarraAlto();
        // === === ===
        PosicionarMeioRadar(ultra1Inicial);
    }
    else
    {
        PosicionarGarraAlto();
        PosicionarMeioRadar(ultra1Inicial);
    }
}

void IrEmbora()
{
    if (saida == "Direita" && bc.Compass() > anguloAreaSoma + 35 + 90)
    {
        while (bc.Compass() < anguloAreaSoma + 90 + 35)
        {
            bc.MoveFrontal(velocidadeGiro, -velocidadeGiro);
        }
    }
    else if (saida == "Direita" && bc.Compass() < anguloAreaSoma + 35 + 90)
    {
        while (bc.Compass() < anguloAreaSoma + 90 + 35)
        {
            bc.MoveFrontal(-velocidadeGiro, velocidadeGiro);
        }
    }
    else if (saida == "Frontal-Direita" && bc.Compass() > anguloAreaSoma + 35)
    {
        while (bc.Compass() > anguloAreaSoma + 35)
        {
            bc.MoveFrontal(velocidadeGiro, -velocidadeGiro);
        }
    }
    else if (saida == "Frontal-Direita" && bc.Compass() < anguloAreaSoma + 35)
    {
        while (bc.Compass() < anguloAreaSoma + 35)
        {
            bc.MoveFrontal(-velocidadeGiro, velocidadeGiro);
        }
    }

    else if (saida == "Esquerda" && bc.Compass() < anguloAreaMenos - 35)
    {
        while (bc.Compass() < anguloAreaMenos - 35)
        {
            bc.MoveFrontal(-velocidadeGiro, velocidadeGiro);
        }
    }
    else if (saida == "Esquerda" && bc.Compass() > anguloAreaMenos - 35)
    {
        while (bc.Compass() > anguloAreaMenos - 35)
        {
            bc.MoveFrontal(velocidadeGiro, -velocidadeGiro);
        }
    }
    // === kim melhora essa verificação aqui da faixa verde na hora de sair da pista
    // ==============================================================================================================
    // ==============================================================================================================
    // ==============================================================================================================
    // ==============================================================================================================
    // ==============================================================================================================
    // ==============================================================================================================
    // ==============================================================================================================

    while ((bc.ReturnColor(0) != "GREEN" || bc.ReturnColor(0) != "VERDE" || bc.ReturnColor(0) != "CIANO" || bc.ReturnColor(0) != "CYAN") && (bc.ReturnColor(4) != "GREEN" || bc.ReturnColor(4) != "VERDE" || bc.ReturnColor(4) != "CIANO" || bc.ReturnColor(4) != "CYAN"))
    {
        bc.MoveFrontal(velocidade, velocidade);
    }
    // ==============================================================================================================
    // ==============================================================================================================
    // ==============================================================================================================
    // ==============================================================================================================
    // ==============================================================================================================
    bc.MoveFrontal(0, 0);


    // Espera para o console //
    bc.Wait(500);

}


// == Tuco stuff

// Mover Escavadora
void MoverEscavadora(double alvoEscavadora)  //o alvo é o angulo exato em q vc quer q pare, e n o tanto q vc quer q mude
{
    if (Math.Sin(bc.AngleActuator() * Math.PI / 180) > Math.Sin(alvoEscavadora * Math.PI / 180))
    {
        //enquanto o seno da posicao atual da escavadora for menor q o seno da posicao alvo, a escavadora sobe
        while (Math.Sin(bc.AngleActuator() * Math.PI / 180) > Math.Sin(alvoEscavadora * Math.PI / 180))
        {
            //A escavadora tem os angulos invertidos :P
            bc.PrintConsole(1, "Escavadora Subindo");
            bc.ActuatorUp(30);
        }
    }

    else
    {
        //enquanto o seno da posicao atual da escavadora for maior q o seno da posicao alvo, a escavadora desce
        while (Math.Sin(bc.AngleActuator() * Math.PI / 180) < Math.Sin(alvoEscavadora * Math.PI / 180))
        {
            bc.PrintConsole(1, "Escavadora Descendo");
            bc.ActuatorDown(30);
        }
    }
}

// Mover Balde
void MoverBalde(double alvoBalde) //o alvo é o angulo exato em q vc quer q pare, e n o tanto q vc quer q mude
{
    if (Math.Sin(bc.AngleScoop() * Math.PI / 180) > Math.Sin(alvoBalde * Math.PI / 180))
    {
        //enquanto o seno da posicao atual da escavadora for menor q o seno da posicao alvo, a escavadora sobe
        while (Math.Sin(bc.AngleScoop() * Math.PI / 180) > Math.Sin(alvoBalde * Math.PI / 180))
        {
            bc.PrintConsole(2, "Balde Descendo");
            bc.TurnActuatorDown(30);
        }
    }

    else
    {
        //enquanto o seno da posicao atual da escavadora for maior q o seno da posicao alvo, a escavadora desce
        while (Math.Sin(bc.AngleScoop() * Math.PI / 180) < Math.Sin(alvoBalde * Math.PI / 180))
        {
            bc.PrintConsole(2, "Balde Subindo");
            bc.TurnActuatorUp(30);
        }
    }
}

// Resgatar
bool Pegar()
{
    bc.ActuatorSpeed(150);
    bc.PrintConsole(1, "Início da Captura");

    // abaixar a escavadora e o balde pra pegar a vitima
    MoverEscavadora(11);
    MoverBalde(10);

    bc.MoveFrontal(250, 250);
    bc.PrintConsole(1, "Andando");
    bc.Wait(1000);
    bc.MoveFrontal(0, 0);

    bool hasVictim = bc.HasVictim();
    if (hasVictim == true)
    {
        bc.ActuatorSpeed(100);
        MoverEscavadora(290);
        MoverBalde(318);
        bc.PrintConsole(1, "Capturado");
        bc.Wait(100);
        bc.ActuatorSpeed(150);
        hasVictim = bc.HasVictim();
        return hasVictim;
    }
    else
    {
        bc.PrintConsole(1, "Não há vítima");
        bc.Wait(100);
        return hasVictim;
    }
}

// Devolver
void Devolver()
{
    bc.PrintConsole(1, "Devolvendo");
    MoverEscavadora(10);
}

void PosicionarMeioRadar(float ultra1Inicial)
{
    if (bc.Distance(1 - 1) > 900)
    {
        RetornarCirculo(180, 900);

        while (bc.Distance(1 - 1) > 152)
        {
            bc.MoveFrontal(velocidade, velocidade);
        }
    }
    else
    {
        while (bc.Distance(1 - 1) < ultra1Inicial)
        {
            bc.MoveFrontal(-velocidade, -velocidade);
        }
    }
}



// === VARIÁVEIS === //
string saida;
string area;
int velocidadeGiro = 900;
int velocidadeBaixa = 200;
int velocidade = 295;

float anguloAreaSoma;
float anguloAreaMenos;

// ================================== MAIN ================================== //
void Main()
{
    // === FUNÇÕES === //
    BolinhaNaGuela();
    AnguloArena();
    IdentificarArea();
    IdentificarSaida();
    PosicionarMeio();
    Radar();
    IrEmbora();
}