/*
Título      :   OBR 2021 Funções de regate de vítimas
Autor       :   Mauro Moledo
Versão      :   1.2
Data scrum  :   04/08
Alterações  :   ALINHAR ANTES DE ENTRAR NA PISTAaaaaaaaaaaaaaaaaaAAAAAAAAAAAA
*/

// Funções Deletaveis

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

int AproximarAngulo(float angulo)
{
    if (angulo >= 315 || angulo < 45) return 0;
    if (angulo >= 45 && angulo < 135) return 90;
    if (angulo >= 135 && angulo < 225) return 180;
    if (angulo >= 225 && angulo < 315) return 270;
    else return 0;
}

float MatematicaCirculo(float angulo)
{
    if (angulo > 360)
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

// === === === FUNÇÕES RESGATE DE VÍTIMAS === === === ///

// ==================================
//  Funções do Auxiliares do Resgate
// ==================================

//o alvo é o angulo exato em q vc quer q pare, e n o tanto q vc quer q mude
void MoverBalde(double alvoBalde)
{

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

//o alvo é o angulo exato em q vc quer q pare, e n o tanto q vc quer q mude
void MoverEscavadora(double alvoEscavadora)
{
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

void PegarBolinha()
{
    bc.ActuatorSpeed(150);
    bc.PrintConsole(1, "Início da Captura");

    // abaixar a escavadora e o balde pra pegar a vitima
    PosicionarGarraBaixo();

    bc.PrintConsole(1, "Andando");
    bc.MoveFrontal(250, 250);
    bc.Wait(500);
    bc.MoveFrontal(0, 0);
    Tick();

    if (bc.HasVictim())
    {
        bc.ActuatorSpeed(130);

        MoverEscavadora(310);
        MoverBalde(319);

        bc.MoveFrontal(-100, -100);
        bc.Wait(300);

        bc.ActuatorSpeed(100);
        MoverEscavadora(291);

        bc.PrintConsole(1, "Capturado");
        bc.Wait(100);

    }
    else
    {
        bc.PrintConsole(1, "Não há vítima");
        Tick();

    }
}

void DevolverBolinha()
{
    bc.PrintConsole(1, "Devolvendo");
    MoverBalde(320);
    MoverEscavadora(340);
    bc.Wait(1000);
}

void PosicionarGarraAlto()
{
    MoverEscavadora(295);
    MoverBalde(330);
}

void PosicionarGarraBaixo()
{
    MoverBalde(11.5d);
    MoverEscavadora(11.5d);
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

// ====================
//  Funções do Resgate
// ====================

void BolinhaNaGuela()
{
    bc.MoveFrontal(-velocidade, -velocidade);
    bc.Wait(500);
    bc.MoveFrontal(0, 0);
    Tick();

    MoverEscavadora(10);
    MoverBalde(320);

    bc.MoveFrontal(velocidade, velocidade);
    bc.Wait(1200);
    bc.MoveFrontal(0, 0);
    Tick();

    PosicionarGarraAlto();

    if(bc.Distance(1 - 1) > 240){
        while(bc.Distance(1 - 1) < 240){
            bc.MoveFrontal(velocidade, velocidade);
            Tick();
        }
    }
    else{
        while(bc.Distance(1 - 1) > 240){
            bc.MoveFrontal(-velocidade, -velocidade);
            Tick();
        }        
    }
    bc.MoveFrontal(0, 0);
    Tick();
}

void IdentificarArea()
{
    bc.PrintConsole(1, anguloInicialResgate.ToString());

    // Caso 1: Parede com certeza (Parede  === Entre 232 e 240) 
    if (bc.Distance(3 - 1) > 230)
    {
        bc.PrintConsole(2, "Parede na esquerda. Entre 230 e 240. Caso 1");
    }

    //Caso 2: Identificou bolinha ou área (Bolinha === Menos de 210)
    else if (bc.Distance(3 - 1) < 210)
    {
        RetornarCirculo(-MatematicaCirculo(bc.Compass() - MatematicaCirculo(anguloInicialResgate - 10)), 600);

        bc.MoveFrontal(0, 0);
        bc.Wait(100);

        // Area com certeza (Area  === Entre 140 e 160 com 10° de desvio para esquerda)
        if (bc.Distance(3 - 1) > 140 && bc.Distance(3 - 1) < 160)
        {
            bc.PrintConsole(3, "Area na Esquerda. Entre 140 e 160 com 10°. Caso 2");
            area = "Esquerda";

        }
        // Parede com certeza (Parede  === Maior que 194 com 10°)
        else if (bc.Distance(3 - 1) > 194)
        {
            bc.PrintConsole(2, "Parede na Esquerda. Maior que 194 com 10°. Caso 2");
        }
    }

    while (bc.Compass() < anguloInicialResgate || bc.Compass() > MatematicaCirculo(anguloInicialResgate + 3))
    {
        //Giro no sentido horário
        bc.MoveFrontal(-velocidadeGiro, velocidadeGiro);
        Tick();

        if (bc.Distance(2 - 1) > 900)
        { //aproveita para verificar a direita
            saida = "Direita";
            bc.PrintConsole(4, "Saida Direita");
        }
    }
}

void IdentificarSaida()
{
    //== Identifica saida ==//

    while (true)
    {
        if (bc.Distance(1 - 1) > 900 && bc.Compass() < MatematicaCirculo(anguloInicialResgate + 50))
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
        else if ((bc.Distance(1 - 1) > 900 && bc.Compass() > MatematicaCirculo(anguloInicialResgate + 75)) || saida == "Direita")
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
        // Sempre da certo porque o ângulo é garantido > 0 (nunca == 350+) pela função IdentificarArena
        else if (bc.Compass() > MatematicaCirculo(anguloInicialResgate + 83))
        {
            saida = "Esquerda";
            bc.PrintConsole(4, "Saida Esquerda");            
            bc.MoveFrontal(0, 0);
            Tick();
            indetificarSaidaCasosEspeciais();
            break;
        }

        bc.MoveFrontal(-velocidadeGiro, velocidadeGiro);
        Tick();
    }

    bc.MoveFrontal(0, 0);
    Tick();
}

void indetificarSaidaCasosEspeciais()
{  // Gira para ir para o centro e verifica o caso especial   
    if(bc.Distance(3 - 1) < 28){
        bc.MoveFrontal(250, 250);
        bc.Wait(300);
        bc.MoveFrontal(-250, -250);
        bc.Wait(300);
    }
    PosicionarGarraBaixo();

    //Girar para 43º
    RetornarCirculo(-MatematicaCirculo(bc.Compass() - MatematicaCirculo(anguloInicialResgate + 43)), velocidadeGiro);

    //Andar até bater no resgate
    while (bc.Distance(1 - 1) > 100)
    {
        bc.MoveFrontal(velocidade, velocidade);
        Tick();
    }

    bc.MoveFrontal(0, 0);
    Tick();

    // Verificação extra de ter pego uma bolinha aleatoriamente
    if (bc.HasVictim())
    {
        MoverEscavadora(310);
        MoverBalde(319);

        bc.ActuatorSpeed(100);
        MoverEscavadora(291);
    }
    else
    {
        PosicionarGarraAlto();
    }

    if (bc.Distance(3 - 1) < 50)
    {
        bc.PrintConsole(3, "Area Frontal-DireitaC");
        area = "Frontal-DireitaC";
        if (bc.HasVictim())
        {
            // Entrega Bolinha
            while (bc.Distance(1 - 1) > 83 && bc.Lightness(6 - 1) < 10)
            {
                bc.MoveFrontal(velocidade, velocidade);
                Tick();
            }
            bc.MoveFrontal(200, 200);
            bc.Wait(400);

            bc.MoveFrontal(0, 0);
            Tick();

            // === Movimentação da Garra ===
            if (bc.HasVictim()) { DevolverBolinha(); }

            PosicionarGarraAlto();
        }
    }
    else
    {
        bc.PrintConsole(3, "Area DireitaC");
        area = "DireitaC";
        if (bc.HasVictim())
        {
            PosicionarMeio(100);
            // Entrega Bolinha
            // RetornarCirculo(MatematicaCirculo(MatematicaCirculo(anguloInicialResgate + 135) - bc.compass()), velocidadeGiro);
            RetornarCirculo(50, velocidadeGiro);
            while(bc.Distance(1 - 1) < 170 || bc.Distance(1 - 1) > 172){
                bc.MoveFrontal(-800, 800);
                Tick();
            }
            
            while (bc.Distance(1 - 1) > 82)
            {
                bc.MoveFrontal(velocidade, velocidade);
                Tick();
            }
            bc.MoveFrontal(0, 0);
            Tick();

            // === Movimentação da Garra ===
            if (bc.HasVictim()) { DevolverBolinha(); }
            PosicionarGarraAlto();
            // === === ===
        }

    }

}

void PosicionarMeio(int velocidadeFrontal)
{
    if (area == "DireitaC" || area == "Frontal-DireitaC")
    {
        while (bc.Distance(1 - 1) < 170)
        {
            bc.MoveFrontal(-velocidadeFrontal, -velocidadeFrontal);
            Tick();
        }
        bc.MoveFrontal(0, 0);
        Tick();
    }
    else{
        RetornarCirculo(-MatematicaCirculo(bc.Compass() - MatematicaCirculo(anguloInicialResgate + 43)), velocidadeGiro);

        while (bc.Distance(1 - 1) > 170)
        {
            bc.MoveFrontal(velocidadeFrontal, velocidadeFrontal);
            Tick();
        }
    }

}

void Radar()
{
    bool stop = false;
    float radarAnguloInicial = bc.Compass();
    while (true)
    {
        bc.PrintConsole(0, "Radar");

        float diferenca1 = bc.Distance(1 - 1) - bc.Distance(3 - 1);
        bc.Wait(20);
        float diferenca2 = bc.Distance(1 - 1) - bc.Distance(3 - 1);

        bc.PrintConsole(1, (diferenca1).ToString());
        bc.PrintConsole(2, (diferenca2).ToString());

        if (diferenca2 < 7.5)
        {
            bc.MoveFrontal(-900, 900);
            Tick();
        }

        else if (diferenca2 > 900 && diferenca2 < 9850)
        {
            bc.MoveFrontal(-900, 900);
            Tick();
        }

        else if (diferenca1 != 0 && diferenca2 > 900)
        {
            bc.MoveFrontal(-900, 900);
            Tick();
        }

        else if (diferenca2 - diferenca1 > 6)
        {
            bc.Wait(120);
            bc.MoveFrontal(0, 0);
            Tick();
            bc.PrintConsole(0, "===Bolinha==="); //achou bolinha
            EntregaBolinha(); // Toda a rotina de entregar a bolinha
            stop = false; // seta a varivel
        }

        else
        {
            bc.MoveFrontal(-900, 900);
            Tick();
        }

        if ((bc.Compass() > radarAnguloInicial && bc.Compass() < radarAnguloInicial + 2) && stop == true) //blind spot
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

    // Ir pra tras pra pegar a bolinha (não esmagar)
    if (bc.Distance(3 - 1) < 31)
    {

        while (bc.Distance(3 - 1) < 30)
        {
            bc.MoveFrontal(-velocidade, -velocidade);
            Tick();
        }
        bc.MoveFrontal(0, 0);
        Tick();

    }
    // Ir pra frente para pegar a bolinha
    else
    {
        while (bc.Distance(3 - 1) > 31)
        {
            bc.MoveFrontal(velocidade, velocidade);
            Tick();
        }
        bc.MoveFrontal(0, 0);
        Tick();
    }

    // === Movimentação da garra ===
    PegarBolinha();
    // === === ===

    if (bc.HasVictim() == true)
    {
        PosicionarMeioRadar(ultra1Inicial, 150);

        if ((area == "Direita" && bc.Compass() > anguloAreaSoma + 45 + 90) || (area == "DireitaC" && bc.Compass() > anguloAreaSoma + 45 + 90))
        {
            while (bc.Compass() < anguloAreaSoma + 90 + 45)
            {
                bc.MoveFrontal(velocidadeGiro, -velocidadeGiro);
                Tick();
            }
        }
        else if ((area == "Direita" && bc.Compass() < anguloAreaSoma + 45 + 90) || (area == "DireitaC" && bc.Compass() < anguloAreaSoma + 45 + 90))
        {
            while (bc.Compass() < anguloAreaSoma + 90 + 45)
            {
                bc.MoveFrontal(-velocidadeGiro, velocidadeGiro);
                Tick();
            }
        }
        else if ((area == "Frontal-Direita" && bc.Compass() > anguloAreaSoma + 45) || (area == "Frontal-DireitaC" && bc.Compass() > anguloAreaSoma + 45))
        {
            while (bc.Compass() > anguloAreaSoma + 45)
            {
                bc.MoveFrontal(velocidadeGiro, -velocidadeGiro);
                Tick();
            }
        }
        else if ((area == "Frontal-Direita" && bc.Compass() < anguloAreaSoma + 45) || (area == "Frontal-DireitaC" && bc.Compass() < anguloAreaSoma + 45))
        {
            while (bc.Compass() < anguloAreaSoma + 45)
            {
                bc.MoveFrontal(-velocidadeGiro, velocidadeGiro);
                Tick();
            }
        }

        else if (area == "Esquerda" && bc.Compass() < anguloAreaMenos - 45)
        {
            while (bc.Compass() < anguloAreaMenos - 45)
            {
                bc.MoveFrontal(-velocidadeGiro, velocidadeGiro);
                Tick();
            }
        }
        else if (area == "Esquerda" && bc.Compass() > anguloAreaMenos - 45)
        {
            while (bc.Compass() > anguloAreaMenos - 45)
            {
                bc.MoveFrontal(velocidadeGiro, -velocidadeGiro);
                Tick();
            }
        }

        ultra1Inicial = bc.Distance(1 - 1);

        while ((bc.Distance(3 - 1) > 4) || (bc.Distance(1 - 1) > 85) || (bc.ReturnColor(6 - 1) != "BLACK"))
        {
            bc.MoveFrontal(velocidade, velocidade);
            Tick();
        }
        bc.MoveFrontal(0, 0);

        // === Movimentação da Garra ===
        if (bc.HasVictim()) { DevolverBolinha(); }
        PosicionarGarraAlto();
        PosicionarMeioRadar(ultra1Inicial, velocidade);
    }
    else
    {
        PosicionarGarraAlto();
        PosicionarMeioRadar(ultra1Inicial, velocidade);
    }
}

void IrEmbora()
{
    if (saida == "Direita" && bc.Compass() > anguloAreaSoma + 35 + 90)
    {
        while (bc.Compass() < anguloAreaSoma + 90 + 35)
        {
            bc.MoveFrontal(velocidadeGiro, -velocidadeGiro);
            Tick();
        }
    }
    else if (saida == "Direita" && bc.Compass() < anguloAreaSoma + 35 + 90)
    {
        while (bc.Compass() < anguloAreaSoma + 90 + 35)
        {
            bc.MoveFrontal(-velocidadeGiro, velocidadeGiro);
            Tick();
        }
    }
    else if (saida == "Frontal-Direita" && bc.Compass() > anguloAreaSoma + 35)
    {
        while (bc.Compass() > anguloAreaSoma + 35)
        {
            bc.MoveFrontal(velocidadeGiro, -velocidadeGiro);
            Tick();
        }
    }
    else if (saida == "Frontal-Direita" && bc.Compass() < anguloAreaSoma + 35)
    {
        while (bc.Compass() < anguloAreaSoma + 35)
        {
            bc.MoveFrontal(-velocidadeGiro, velocidadeGiro);
            Tick();
        }
    }
    else if (saida == "Esquerda" && bc.Compass() < anguloAreaMenos - 35)
    {
        while (bc.Compass() < anguloAreaMenos - 35)
        {
            bc.MoveFrontal(-velocidadeGiro, velocidadeGiro);
            Tick();
        }
    }
    else if (saida == "Esquerda" && bc.Compass() > anguloAreaMenos - 35)
    {
        while (bc.Compass() > anguloAreaMenos - 35)
        {
            bc.MoveFrontal(velocidadeGiro, -velocidadeGiro);
            Tick();
        }
    }
    while (bc.ReturnColor(1) != "GREEN" || bc.ReturnColor(1) != "CYAN" || bc.ReturnColor(2) != "GREEN" || bc.ReturnColor(2) != "CYAN" || bc.ReturnColor(3) != "GREEN" || bc.ReturnColor(3) != "CYAN")
    {
        bc.MoveFrontal(velocidade, velocidade);
        Tick();
    }
    bc.MoveFrontal(0, 0);
    Tick();

}

void PosicionarMeioRadar(float ultra1Inicial, int velocidadeFrontal)
{
    if (bc.Distance(1 - 1) > 900)
    {
        RetornarCirculo(180, 900);

        while (bc.Distance(1 - 1) > 152)
        {
            bc.MoveFrontal(velocidadeFrontal, velocidadeFrontal);
        }
    }
    else
    {
        while (bc.Distance(1 - 1) < ultra1Inicial)
        {
            bc.MoveFrontal(-velocidadeFrontal, -velocidadeFrontal);
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

float anguloInicialResgate = 0;

// ================================== MAIN ================================== //
void Main()
{
    bc.ActuatorSpeed(150);

    // === FUNÇÕES === //
    BolinhaNaGuela();

    anguloInicialResgate = AproximarAngulo(bc.Compass());
    // AnguloArena();

    bc.PrintConsole(0, "Vou identificar a arena");
    bc.MoveFrontal(0, 0);
    bc.Wait(1000);

    IdentificarArea();

    bc.PrintConsole(0, "Vou identificar a arena");
    bc.MoveFrontal(0, 0);
    bc.Wait(1000);

    IdentificarSaida();

    bc.PrintConsole(0, "Vou identificar CASOS ESPECIAIS");
    bc.MoveFrontal(0, 0);
    bc.Wait(1000);

    PosicionarMeio(velocidade);

    bc.PrintConsole(0, "Vou começar o radar");
    bc.MoveFrontal(0, 0);
    bc.Wait(1000000);

    // Radar();

    // IrEmbora();

}