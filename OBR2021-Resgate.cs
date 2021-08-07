/*
Título      :   OBR 2021 Funções de regate de vítimas
Autor       :   Mauro Moledo & Tuco
Versão      :   1.4
Data scrum  :   07/08
Alterações  :   
Nome do Robo:   Batatinha quente
*/

// Funções Deletaveis


void MoverPorUnidade(float distancia){
    if(distancia > 0){
        bc.MoveFrontal(200, 200);
        bc.Wait((int) (distancia/39.64*1000));
    }
    else{
        bc.MoveFrontal(-200, -200);
        bc.Wait((int) (-distancia/39.64*1000));
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

void ChegarNoAngulo(float angulo){
    if(MatematicaCirculo(angulo - bc.Compass()) < 180){
        //girar no sentido horário
        RetornarCirculo(MatematicaCirculo(angulo - bc.Compass()), velocidadeGiro);
    }
                
    else{
        //girar no sentido anti-horário
        RetornarCirculo(MatematicaCirculo(angulo - bc.Compass()) - 360, velocidadeGiro);
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

    // abaixar a escavadora e o balde pra pegar a vitima
    PosicionarGarraBaixo();

    bc.MoveFrontal(250, 250);
    bc.Wait(700);
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

        bc.Wait(100);

    }
    else
    {
        PosicionarGarraAlto();
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
    MoverBalde(11);
    MoverEscavadora(11);
    bc.PrintConsole(0, "Terminei de abaixar");
    Tick();
}


float UltraInicial () 
{   
    float ultraInicial;

    if (bc.Distance(1 - 1) > 9000)
    {
        RetornarCirculo(180, 990);
        ultraInicial = bc.Distance(1 - 1);
        Tick();
        RetornarCirculo(180, 990);
    }
    else
    {
        ultraInicial = bc.Distance(1 - 1);
    }

    return ultraInicial;
}

// ====================
//  Funções do Resgate
// ====================

void BolinhaNaGuela()
{   
    bc.TurnLedOn(0, 255, 0);
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

    bc.MoveFrontal(0, 0);
    Tick();

    if(bc.Distance(1 - 1) > 240){
        while(bc.Distance(1 - 1) > 240 || bc.Inclination() > 5){
            bc.MoveFrontal(100, 100);
            Tick();
        }
    }
    else{
        while(bc.Distance(1 - 1) < 240 || bc.Inclination() > 5){
            bc.MoveFrontal(-100, -100);
            Tick();
        }        
    }
    bc.MoveFrontal(0, 0);
    Tick();
    bc.TurnLedOff();
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
        if (bc.Distance(1 - 1) > 900 && bc.Compass() > MatematicaCirculo(anguloInicialResgate + 30) && bc.Compass() < MatematicaCirculo(anguloInicialResgate + 45))
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

        // 270 = 340 < X < 470/110
        else if ((bc.Distance(1 - 1) > 900 && bc.Compass() > MatematicaCirculo(anguloInicialResgate + 70) && bc.Compass() < MatematicaCirculo(anguloInicialResgate + 85)) || saida == "Direita")
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
        else if (bc.Compass() > MatematicaCirculo(anguloInicialResgate + 85))
        {
            saida = "Esquerda";
            bc.PrintConsole(4, "Saida Esquerda");            
            bc.MoveFrontal(0, 0);
            Tick();
            indetificarSaidaCasosEspeciais();
            break;
        }

        // Girando para identificar saídas
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
    ChegarNoAngulo(anguloInicialResgate + 43);

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
            ChegarNoAngulo(anguloInicialResgate + 135);

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
        ChegarNoAngulo(anguloInicialResgate + 43);

        //336.76 - 170 = 
        //No caso frontal-Direita, não se poder usar o ultrassom
        if(bc.Distance(1 - 1) > 9000){
            MoverPorUnidade(166.76f);
        }
        else{
            while (bc.Distance(1 - 1) > 170)
            {
                bc.MoveFrontal(velocidadeFrontal, velocidadeFrontal);
                Tick();
            }
        }
    }

}

void Radar()
{
    bool stop = false;
    //Sempre apontando para diagonais do quadrado
    float radarAnguloInicial = bc.Compass();
    while (true)
    {
        bc.PrintConsole(0, "Radar");

        float diferenca1 = bc.Distance(1 - 1) - bc.Distance(3 - 1);
        bc.Wait(10);
        float diferenca2 = bc.Distance(1 - 1) - bc.Distance(3 - 1);

        bc.PrintConsole(1, (diferenca1).ToString());
        bc.PrintConsole(2, (diferenca2).ToString());
        
        //Pega no sentido certo
        if (diferenca1 > diferenca2) {
            bc.MoveFrontal(-950, 950);
        }

        //impede de pegar parede | vendo absimo -> viu parede e abmismo |
        else if (diferenca1 == 0 && diferenca2 < 9850) {
            bc.MoveFrontal(-950, 950);
        }

        //impede de pegar parede | vendo parede -> viu parede e abismo |
        else if (diferenca1 != 0 && diferenca2 > 900){
            bc.MoveFrontal(-950, 950);
        }

        //impede de pegar parede | vendo parede e abismo -> viu parede e abismo |
        else if (diferenca1 > 900 && diferenca2 > 900){
            bc.MoveFrontal(-950, 950);
        }

        else if (diferenca2 - diferenca1 > 6.5d)
        {
            bc.Wait(150);
            bc.MoveFrontal(0, 0);
            Tick();
            bc.PrintConsole(0, "===Bolinha==="); //achou bolinha
            EntregaBolinha(); // Toda a rotina de entregar a bolinha
            bc.Wait(1000);
            stop = false; // seta a varivel
        }

        else
        {
            bc.MoveFrontal(-950, 950);
        }

        // Lógica para parar o radar
        if ((bc.Compass() > radarAnguloInicial && bc.Compass() < radarAnguloInicial + 3) && stop == true) //blind spot
        {
            break;
        }
        else if (bc.Compass() > radarAnguloInicial + 3 && bc.Compass() < radarAnguloInicial + 5) //blind spot para setar a variavel
        {
            stop = true;
        }
    }
}

void EntregaBolinha()
{
    float ultraRadar = UltraInicial();
    // Ir pra tras pra pegar a bolinha (não esmagar)
    if (bc.Distance(3 - 1) < 35)
    {
        RetornarCirculo(3, 900);

        while (bc.Distance(3 - 1) < 35)
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
        while (bc.Distance(3 - 1) > 35)
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

    //Vai devagar se tiver vitima
    if (bc.HasVictim())
    {
        PosicionarMeioRadar(ultraRadar, 150);

    }
    else
    {
        PosicionarMeioRadar(ultraRadar, velocidade);
        RetornarCirculo(-5, 900);
    }

    if (bc.HasVictim() == true)
    {
        // Girar na direção da área de resgate
        if (area == "Direita" || area == "DireitaC"){
            
            ChegarNoAngulo(anguloInicialResgate + 135);

        }

        if (area == "Frontal-Direita" || area == "Frontal-DireitaC"){

            ChegarNoAngulo(anguloInicialResgate + 45);

        }

        if (area == "Esquerda"){

            ChegarNoAngulo(anguloInicialResgate + 315);

        }

        ultraRadar = UltraInicial();
        bc.PrintConsole(0, ultraRadar.ToString());

        while (bc.Distance(1 - 1) > 72)
        {
            bc.MoveFrontal(velocidade, velocidade);
            Tick();
        }
        bc.MoveFrontal(0, 0);

        // === Movimentação da Garra ===
        if (bc.HasVictim()) { DevolverBolinha(); }
        PosicionarGarraAlto();
        PosicionarMeioRadar(ultraRadar, velocidade);
    }
}

void PosicionarMeioRadar(float ultraInicial, int velocidadeFrontal)
{
    if (bc.Distance(1 - 1) > 9000)
    {
        RetornarCirculo(180, 900);
        while (bc.Distance(1 - 1) > ultraInicial)
        {
            bc.MoveFrontal(velocidadeFrontal, velocidadeFrontal);
            Tick();
        }
    }
    else {

        while (bc.Distance(1 - 1) < ultraInicial)
        {
            bc.MoveFrontal(-velocidadeFrontal, -velocidadeFrontal);
            Tick();
        }
    }
    
    bc.MoveFrontal(0,0);
    Tick();
}

void IrEmbora()
{
    if (saida == "Direita")
    {
        ChegarNoAngulo(anguloInicialResgate + 125);
    }
    
    else if (saida == "Frontal-Direita" )
    {
        ChegarNoAngulo(anguloInicialResgate + 35);
    }

    else if (saida == "Esquerda" )
    {
        ChegarNoAngulo(anguloInicialResgate - 55);
    }
    while (bc.ReturnColor(1) != "GREEN" || bc.ReturnColor(1) != "CYAN" || bc.ReturnColor(2) != "GREEN" || bc.ReturnColor(2) != "CYAN" || bc.ReturnColor(3) != "GREEN" || bc.ReturnColor(3) != "CYAN")
    {
        bc.MoveFrontal(velocidade, velocidade);
        Tick();
    }
    bc.MoveFrontal(0, 0);
    Tick();
}

// === VARIÁVEIS === //
string saida;
string area;
int velocidadeGiro = 990;
int velocidadeBaixa = 200;
int velocidade = 295;

float anguloInicialResgate;

// ================================== MAIN ================================== //
void Main()
{
    bc.ActuatorSpeed(150);

    // === FUNÇÕES === //
    BolinhaNaGuela();

    anguloInicialResgate = AproximarAngulo(bc.Compass());

    bc.PrintConsole(0, "Vou identificar a arena");
    bc.MoveFrontal(0, 0);
   

    IdentificarArea();

    bc.PrintConsole(0, "Vou identificar a arena");
    bc.MoveFrontal(0, 0);
   

    IdentificarSaida();

    bc.PrintConsole(0, "Vou pro meio");
    bc.MoveFrontal(0, 0);
  

    PosicionarMeio(velocidade);

    bc.PrintConsole(0, "Vou começar o radar");
    bc.MoveFrontal(0, 0);
  

    Radar();

    bc.PrintConsole(0, "Vou embora");
    bc.MoveFrontal(0, 0);

    IrEmbora();

}