/*
Título      :   Pista OBR
Autores     :   Kim, Breno, Mauro & Tuco
Nome do Robo:   Batatinha quente
*/

// ====== Variáveis PID ====== //
float error = 0, lastError = 0, integral = 0, derivate = 0;
float movimento;
bool pararIntegro;

// ====== Variáveis Gerais ====== //
// - Recuperação de linha
int controleTempoRecuperarLinha = 0, tempoInicialLinha = 0, anguloInicialLinha = 0;

// - Gangorra
int tempoInicialGangorra = 0, controleEstagioGangorra = 0;
float controleAnguloGangorra = -1;

// === variáveis RESGATE === //
string saida;
string area;

float anguloInicialResgate;

// ===============
// Funções de suporte
// ===============

// ===============
// ====== Função de Passar Tempo ====== //
// ===============
void Tick(){ bc.Wait(30); }
 
// ===============
// ====== Funções de Matemática com Ângulos ====== //
// ===============
// Retornar aproximação de ângulo para um dos pontos cardeais
int AproximarAngulo(float angulo){
    if(angulo >= 315 || angulo < 45) return 0;
    if(angulo >= 45 && angulo < 135) return 90;
    if(angulo >= 135 && angulo < 225) return 180;
    if(angulo >= 225 && angulo < 315) return 270;
    else return 0;
}

// Girar por graus, independente da orientação
// positivo para sentido horário
// negativo para sentido anti-horário
// Margem de erro > 5º
// Máximo de movimento em uma direção = 355
void RetornarCirculo(float anguloMovimento, float velocidade = 950){
    float anguloInicial = bc.Compass();
    // Movimento positivo - sentido horário
    if(anguloMovimento > 0){
        // Alterando os valores para evitar o loop infinito em 360º/0º
        if(anguloInicial + anguloMovimento == 359) anguloInicial += -1;
        if(anguloInicial + anguloMovimento == 360) anguloInicial += 2; 
        if(anguloInicial + anguloMovimento == 361) anguloInicial += 1;   

        // Movimento passa pelo limite de 0/360º
        if(anguloInicial + anguloMovimento > 360){
            while(bc.Compass() > anguloInicial + anguloMovimento - 355 || bc.Compass() < anguloInicial + anguloMovimento - 360){
                bc.MoveFrontal(-velocidade, velocidade);
                Tick();
            }
        }
        // Movimento regular
        else{
            while(bc.Compass() < anguloInicial + anguloMovimento){
                bc.MoveFrontal(-velocidade, velocidade);
                Tick();
            }
        }
    }
    else{
        // Invertendo o sinal do ângulo pra facilitar a visualização da matemática
        anguloMovimento = anguloMovimento * -1;

        // Alterando os valores para evitar o loop infinito em 360º/0º
        if(anguloInicial - anguloMovimento == -1) anguloInicial += -1;
        if(anguloInicial - anguloMovimento == 0) anguloInicial += -2; 
        if(anguloInicial - anguloMovimento == 1) anguloInicial += 1;

        // Movimento passa pelo limite de 0/360º
        if(anguloInicial < anguloMovimento){
            while(bc.Compass() < anguloInicial + 355 - anguloMovimento || bc.Compass() > anguloInicial + 360 - anguloMovimento){
                bc.MoveFrontal(velocidade, -velocidade);
                Tick();
            }
        }
        //Movimento regular
        else{
            while(bc.Compass() > anguloInicial - anguloMovimento){
                bc.MoveFrontal(velocidade, -velocidade);
                Tick();
            }
        }
    }
}

// Se locomove da forma mais eficiente até o angulo desejado. Apenas valores positivos
void ChegarNoAngulo(float angulo){
    if(angulo < 1){
        angulo += 2;
    }
    if(MatematicaCirculo(angulo - bc.Compass()) < 180){
        //girar no sentido horário
        RetornarCirculo(MatematicaCirculo(angulo - bc.Compass()), velocidadeGiroGlobal);
    }
                
    else{
        //girar no sentido anti-horário
        RetornarCirculo(MatematicaCirculo(angulo - bc.Compass()) - 360, velocidadeGiroGlobal);
    }
}

// Faz matemática em ciclo
float MatematicaCirculo(float angulo){
    if(angulo >= 360){
        return angulo - 360;
    }
    else if(angulo < 0){
        
        return (float) (-360 * Math.Floor( (double) (angulo / 360) ) + angulo);
    }
    else{
        return angulo;
    }
}

// ===============
// ====== Funções luz ===== //
// ===============
float MedirLuz(int sensor){
    return bc.Lightness(sensor) ;
}

// ===============
// ===== Funções Garra ===== //
// ===============

//balde = 318, escavadora = 290
void AjustarAnguloBalde(){
    MoverBalde(318);
}

void AjustarAlturaBalde(){
    MoverEscavadora(290);
}

// ===============
// ==== Funções Movimento ==== //
// ===============
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

// ===============
// Funções de suporte resgate
// ===============

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

void MoverUltra(float distance, int velocidade){
    if(bc.Distance(1 - 1) > distance){
        while(bc.Distance(1 - 1) > distance){
            bc.MoveFrontal(velocidade, velocidade);
            Tick();
        }
    }
    else{
        while(bc.Distance(1 - 1) < distance){
            bc.MoveFrontal(-velocidade, -velocidade);
            Tick();
        }
    }
}

// ====================
//  Funções do Resgate
// ====================

void BolinhaNaGuela()
{   
    bc.TurnLedOn(0, 255, 0);
    bc.MoveFrontal(-295, -295);
    bc.Wait(500);
    bc.MoveFrontal(0, 0);
    Tick();

    MoverEscavadora(10);
    MoverBalde(320);

    bc.MoveFrontal(295, 295);
    bc.Wait(1200);
    bc.MoveFrontal(0, 0);
    Tick();

    PosicionarGarraAlto();

    bc.MoveFrontal(0, 0);
    Tick();

    bc.MoveFrontal(295, 295);
    bc.Wait(400);

    if(bc.Distance(1 - 1) > 240){
        while(bc.Distance(1 - 1) > 240){
            bc.MoveFrontal(100, 100);
            Tick();
        }
    }
    else{
        while(bc.Distance(1 - 1) < 240){
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
        bc.MoveFrontal(-velocidadeGiroGlobal, velocidadeGiroGlobal);
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
        bc.MoveFrontal(-velocidadeGiroGlobal, velocidadeGiroGlobal);
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
        bc.MoveFrontal(velocidadeGlobal, velocidadeGlobal);
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
                bc.MoveFrontal(velocidadeGlobal, velocidadeGlobal);
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
                bc.MoveFrontal(velocidadeGlobal, velocidadeGlobal);
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
    //Sempre apontando para diagonais do quadrado
    //44 + 190 = 234
    //44 - 190 = 214

    if(area == "Direita" || area == "DireitaC"){
        ChegarNoAngulo(MatematicaCirculo(anguloInicialResgate + 135));
    }
    else if(area == "Frontal-Direita" || area == "Frontal-DiretaC"){
        ChegarNoAngulo(MatematicaCirculo(anguloInicialResgate + 45));
    }
    else if(area == "Esquerda"){
        ChegarNoAngulo(MatematicaCirculo(anguloInicialResgate + 315));
    }
    
    
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
        else if (diferenca1 == 0 && diferenca2 < 9853) { //10.0000 - 150 = 9.850
            bc.MoveFrontal(-950, 950);                   // 140
        }                                                // 149

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
            bc.Wait(170);
            bc.MoveFrontal(0, 0);
            Tick();
            bc.PrintConsole(0, "===Bolinha==="); //achou bolinha
            EntregaBolinha("HORARIO"); // Toda a rotina de entregar a bolinha
            bc.Wait(1000);
        }

        else
        {
            bc.MoveFrontal(-950, 950);
        }

        // Lógica para parar o radar
        if (bc.Compass() > MatematicaCirculo(radarAnguloInicial + 190) && bc.Compass() < MatematicaCirculo(radarAnguloInicial + 195)) //blind spot
        {   
            RetornarCirculo(-150, 950);
            bc.MoveFrontal(0, 0);
            Tick();
            break;
        }
    }
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
            bc.MoveFrontal(950, -950);
        }

        //impede de pegar parede | vendo absimo -> viu parede e abmismo |
        else if (diferenca1 == 0 && diferenca2 < 9850) {
            bc.MoveFrontal(950, -950);
        }

        //impede de pegar parede | vendo parede -> viu parede e abismo |
        else if (diferenca1 != 0 && diferenca2 > 900){
            bc.MoveFrontal(950, -950);
        }

        //impede de pegar parede | vendo parede e abismo -> viu parede e abismo |
        else if (diferenca1 > 900 && diferenca2 > 900){
            bc.MoveFrontal(950, -950);
        }

        else if (diferenca2 - diferenca1 > 6.5d)
        {
            bc.Wait(170);
            bc.MoveFrontal(0, 0);
            Tick();
            bc.PrintConsole(0, "===Bolinha==="); //achou bolinha
            EntregaBolinha("ANTI-HORARIO"); // Toda a rotina de entregar a bolinha
            bc.Wait(1000);
        }

        else
        {
            bc.MoveFrontal(950, -950);
        }

        // Lógica para parar o radar
        if (bc.Compass() < MatematicaCirculo(radarAnguloInicial - 190) && bc.Compass() > MatematicaCirculo(radarAnguloInicial - 195)) //blind spot
        {
            break;
        }
    }
}

void EntregaBolinha(string direcao)
{
    float compassRadar = bc.Compass();
    float ultraRadar = UltraInicial();
    // Ir pra tras pra pegar a bolinha (não esmagar)
    if (bc.Distance(3 - 1) < 35)
    {   
        if(direcao == "HORARIO"){
            RetornarCirculo(5, 900);
        }
        else{
            RetornarCirculo(-5, 900);
        }

        while (bc.Distance(3 - 1) < 35)
        {
            bc.MoveFrontal(-velocidadeGlobal, -velocidadeGlobal);
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
            bc.MoveFrontal(velocidadeGlobal, velocidadeGlobal);
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
        ChegarNoAngulo(compassRadar);
        PosicionarMeioRadar(ultraRadar, 150);

    }
    else
    {
        ChegarNoAngulo(compassRadar);
        PosicionarMeioRadar(ultraRadar, velocidadeGlobal);
        if(direcao == "HORARIO"){
            RetornarCirculo(-5, 900);
        }
        else{
            RetornarCirculo(5, 900);
        }
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
        compassRadar = bc.Compass();
        ultraRadar = UltraInicial();
        bc.PrintConsole(0, ultraRadar.ToString());

        while (bc.Distance(1 - 1) > 75)
        {
            bc.MoveFrontal(velocidadeGlobal, velocidadeGlobal);
            Tick();
        }
        bc.MoveFrontal(0, 0);

        // === Movimentação da Garra ===
        if (bc.HasVictim()) { DevolverBolinha(); }
        PosicionarGarraAlto();
        ChegarNoAngulo(compassRadar);
        PosicionarMeioRadar(ultraRadar, velocidadeGlobal);
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
        ChegarNoAngulo(MatematicaCirculo(anguloInicialResgate + 180));
        
        if(bc.Distance(1 - 1) > 9000){
            RetornarCirculo(180);
            MoverUltra(219, velocidadeGlobal);
        }
        else{
            MoverUltra(28, velocidadeGlobal);
        }

        ChegarNoAngulo(MatematicaCirculo(anguloInicialResgate + 90));
    }
    
    else if (saida == "Frontal-Direita" )
    {
        ChegarNoAngulo(MatematicaCirculo(anguloInicialResgate + 90));
        
        if(bc.Distance(1 - 1) > 9000){
            RetornarCirculo(180);
            MoverUltra(219, velocidadeGlobal);
        }
        else{
            MoverUltra(28, velocidadeGlobal);
        }

        ChegarNoAngulo(anguloInicialResgate);
    }

    else if (saida == "Esquerda" )
    {
        ChegarNoAngulo(MatematicaCirculo(anguloInicialResgate + 270));
        
        if(bc.Distance(1 - 1) > 9000){
            RetornarCirculo(180);
            MoverUltra(219, velocidadeGlobal);
        }
        else{
            MoverUltra(28, velocidadeGlobal);
        }

        ChegarNoAngulo(anguloInicialResgate);
        
        if(bc.Distance(1 - 1) > 9000){
            RetornarCirculo(180);
            MoverUltra(219, velocidadeGlobal);
        }
        else{
            MoverUltra(28, velocidadeGlobal);
        }

        ChegarNoAngulo(MatematicaCirculo(anguloInicialResgate + 270));
    }

    while(bc.ReturnColor(1) == "WHITE" && bc.ReturnColor(3) == "WHITE"){
        bc.MoveFrontal(250, 250);
        Tick();
    }

    bc.MoveFrontal(150, 150);
    bc.Wait(600);

    bc.PrintConsole(0, "Procurando linha");
    
    while(bc.Lightness(3) > escuro && bc.Compass() > AproximarAngulo(bc.Compass()) - 35){
        bc.MoveFrontal(900, -900);
        Tick();
    }
    if(bc.Lightness(3) > escuro){
        while(bc.Lightness(2) > escuro && bc.Compass() < AproximarAngulo(bc.Compass()) + 35){
            bc.MoveFrontal(-900, 900);
            Tick();
        }
    }

    estagio = "Pista";
}

// =================
// Funções da pista
// =================

void RecuperarLinha(int velocidadeGiro){

    if(controleTempoRecuperarLinha == 0){
        controleTempoRecuperarLinha = 1;
        tempoInicialLinha = bc.Timer();
        anguloInicialLinha = AproximarAngulo(bc.Compass());
    }
    else if(controleTempoRecuperarLinha == 1){

        if(tempoInicialLinha + 2000 < bc.Timer()){
            bc.MoveFrontal(0, 0);
            Tick();

            bc.PrintConsole(2, "Voltando na Linha " + anguloInicialLinha.ToString());

            int controleLuz = 0;

            bc.MoveFrontal(-170, -170);
            bc.Wait(800);
            RetornarCirculo(anguloInicialLinha - bc.Compass(), velocidadeGiro);

            if(MedirLuz(0) < claro || MedirLuz(1) < claro || MedirLuz(2) < claro || MedirLuz(3) < claro || MedirLuz(4) < claro) {
                bc.PrintConsole(2, "Voltei na linha" );
                bc.MoveFrontal(0, 0);
                Tick();
            }
            else{
                for (int i = 0; i<3; i++){       
                    bc.MoveFrontal(170, 170);
                    bc.Wait(1350 + 100 * i);
                
                    if(MedirLuz(0) < claro || MedirLuz(1) < claro || MedirLuz(2) < claro || MedirLuz(3) < claro || MedirLuz(4) < claro) {break;}

                    int anguloRecuperandoLinha;
                    for(anguloRecuperandoLinha = 0; anguloRecuperandoLinha < 38; anguloRecuperandoLinha++){
                        bc.MoveFrontal(velocidadeGiro, -velocidadeGiro);
                        Tick();
                        if(MedirLuz(0) < claro || MedirLuz(1) < claro || MedirLuz(2) < claro || MedirLuz(3) < claro || MedirLuz(4) < claro) {controleLuz = 1; break;}    
                    }

                    if(controleLuz == 1){ break; }

                    for(anguloRecuperandoLinha = 0; anguloRecuperandoLinha > -76; anguloRecuperandoLinha--){
                        bc.MoveFrontal(-velocidadeGiro, velocidadeGiro);
                        Tick();
                        if(MedirLuz(0) < claro || MedirLuz(1) < claro || MedirLuz(2) < claro || MedirLuz(3) < claro || MedirLuz(4) < claro) {controleLuz = 1; break;}    
                    }

                    if(controleLuz == 1){ break; }

                    RetornarCirculo(anguloInicialLinha - bc.Compass(), velocidadeGiro);
                }

                bc.PrintConsole(2, "Voltei na linha" );
                bc.MoveFrontal(0, 0);
                Tick();

                if(MedirLuz(0) < claro){
                    bc.PrintConsole(2, "Ajeitando na linha" );

                    while(MedirLuz(2) < claro){
                        bc.MoveFrontal(velocidadeGiro, -velocidadeGiro);
                        Tick();
                    }

                    
                    RetornarCirculo(10, velocidadeGiro);
                }
                else if(MedirLuz(4) < claro){
                    bc.PrintConsole(2, "Ajeitando na linha" );

                    while(MedirLuz(2) < claro){
                        bc.MoveFrontal(velocidadeGiro, -velocidadeGiro);
                        Tick();
                    }

                    RetornarCirculo(-10, velocidadeGiro);
                }

                bc.PrintConsole(2, "");
            }

            controleTempoRecuperarLinha = 0;
            integral = 0;
        }
    }
}

void seguirLinhaPID (float velocidade, float kp, float ki,float kd){
    // matemática PID
    error = MedirLuz(1) - MedirLuz(3);

    integral += error;
    derivate = error - lastError;
    
    if(pararIntegro){
        movimento = error*kp + derivate*kd;
    }
    else{
        movimento = error*kp + integral*ki + derivate*kd;
    }
    
    // Controle de erros do integro
    pararIntegro = ( (error < 10 && integral > 100) || ( ( (error > 0 && movimento > 0) || (error < 0 && movimento < 0) ) && (movimento > 1000 - velocidade) ) );

    if (movimento > 1000 - velocidade) { movimento = 1000 - velocidade; }

    // Console
    bc.PrintConsole(0, "Error: "+ error.ToString("F") + " M: " + movimento.ToString());
    bc.PrintConsole(1, "Integral: " + integral.ToString() + " Integro Parado: " + pararIntegro.ToString());
    // bc.PrintConsole(2, "Derivate: " + derivate.ToString("F"));

    // Movimento
    bc.MoveFrontal(velocidade + movimento, velocidade - movimento);
    Tick();

    // Atualização de variável
    lastError = error;
}

void Curva90(string curva, float claro = 25){

    int velocidadeFrontal = 150, velocidadeGiro = 950;
    bool retornar = true;
    
    bc.MoveFrontal(velocidadeFrontal, velocidadeFrontal);
    bc.Wait(470);

    bc.MoveFrontal(0, 0);
    bc.Wait(100);

    // desvio para esquerda
    if( curva == "Esquerda" ){

        // detectar se é curva ou interseção e virar caso necessário
        if( MedirLuz(1) > claro && MedirLuz(2) > claro && MedirLuz(3) > claro ){
            
            bc.PrintConsole(2, "Virando Esquerda");

            float tempoAnterior = bc.Timer();

            while(MedirLuz(2) > claro){
                
                bc.MoveFrontal(velocidadeGiro, -velocidadeGiro);
                Tick();

                if(bc.Timer() > tempoAnterior+5000){

                    tempoAnterior = bc.Timer();
        
                    while(bc.Timer() < tempoAnterior+2000){
                        bc.MoveFrontal(-velocidadeGiro, velocidadeGiro);
                        Tick();
                    }
                    controleTempoRecuperarLinha = 0;
                    retornar = false;
                    break;
                }
            }
        }
    }

    //devio para direita
    if( curva == "Direita" ){

        // detectar se é curva ou interseção e virar caso necessário
        if( MedirLuz(1) > claro && MedirLuz(2) > claro && MedirLuz(3) > claro ){
            
            bc.PrintConsole(2, "Virando Direita");

            float tempoAnterior = bc.Timer();

            while(MedirLuz(2) > claro){
                
                bc.MoveFrontal(-velocidadeGiro, velocidadeGiro);
                Tick();

                
                if(bc.Timer() > tempoAnterior+5000){
                    
                    tempoAnterior = bc.Timer();
                    
                    while(bc.Timer() < tempoAnterior+2000){
                        bc.MoveFrontal(+velocidadeGiro, -velocidadeGiro);
                        Tick();
                    }
                    controleTempoRecuperarLinha = 0;
                    retornar = false;
                    break;
                }
            }
        }
    }

    if(retornar){
        bc.MoveFrontal(-velocidadeFrontal, -velocidadeFrontal);
        bc.Wait(200);

        bc.MoveFrontal(0, 0);
        bc.Wait(100);
    }
}

void Verde(string curva){

    int velocidadeFrontal = 150, velocidadeGiro = 950; 
    
    bc.MoveFrontal(velocidadeFrontal, velocidadeFrontal);
    bc.Wait(1000);

    bc.MoveFrontal(0, 0);
    bc.Wait(100);

    if(curva == "Esquerda"){
        
        bc.PrintConsole(2, "Verde Esquerda");

        RetornarCirculo(-20, velocidadeGiro);

        while(MedirLuz(2) > claro){

            bc.MoveFrontal(velocidadeGiro, -velocidadeGiro);
            Tick();
        }

        bc.PrintConsole(2, "");
    }
    if(curva == "Direita"){
        
        bc.PrintConsole(2, "Verde Direita");

        RetornarCirculo(20, velocidadeGiro);

        while(MedirLuz(2) > claro){
                
            bc.MoveFrontal(-velocidadeGiro, velocidadeGiro);
            Tick();
        }

        bc.PrintConsole(2, "");
    }
    if(curva == "Ambos"){
        bc.PrintConsole(2, "Ambos");
        RetornarCirculo(180, velocidadeGiro);
    }

    bc.MoveFrontal(-velocidadeFrontal, -velocidadeFrontal);
    bc.Wait(300);

    bc.MoveFrontal(0, 0);
    bc.Wait(100);
}

void DesvioUltrassom(){
    int velocidadeFrontal = 150, velocidadeGiro = 950;
    int anguloInicialDesvio = AproximarAngulo(bc.Compass());                      

    ChegarNoAngulo(MatematicaCirculo(anguloInicialDesvio + 270));

    bc.MoveFrontal(velocidadeFrontal,velocidadeFrontal);
    bc.Wait(1300);

    ChegarNoAngulo(MatematicaCirculo(anguloInicialDesvio));

    while(MedirLuz(1) > claro && MedirLuz(2) > claro && MedirLuz(3) > claro){
        if(bc.Distance(1) < 25){
            bc.PrintConsole(2, "Bloco detectado a direita");
            
            bc.MoveFrontal(velocidadeFrontal, velocidadeFrontal);
            bc.Wait(1800);

            RetornarCirculo(90, velocidadeGiro);
        }
        else{
            bc.MoveFrontal(velocidadeFrontal,velocidadeFrontal);
            Tick();
        }
    }
    
    bc.PrintConsole(2, "Linha detectada");

    bc.MoveFrontal(velocidadeFrontal, velocidadeFrontal);
    bc.Wait(950);

    
    RetornarCirculo(-90, velocidadeGiro);

    bc.MoveFrontal(0, 0);
    Tick();
}

void Gangorra(){

    if(controleEstagioGangorra == 0){
        bc.PrintConsole(2, "Plano Inclinado");
        tempoInicialGangorra = bc.Timer();
        controleEstagioGangorra = 1;
    }

    else{

        if(tempoInicialGangorra + 3350 < bc.Timer()){
            while(bc.Inclination() > 300 || bc.Inclination() < 15){
                
                bc.MoveFrontal(0, 0);
                bc.PrintConsole(2, "Estou parado");
                
                if(controleAnguloGangorra == bc.Inclination()){
                    break;
                }

                controleAnguloGangorra = bc.Inclination();
                
                bc.Wait(200);
            }
            
            controleEstagioGangorra = 0;
        }

    }
}

// ====== Variáveis Específicas (A serem calibradas) ====== //

string estagio = "Pista";
bool final = false;

// Claro = Desvio 90º, perder/recuperar linha
// escuro = Desvio 90º
float claro = 55, escuro = 37;
int velocidadeFrontal = 150;
int velocidadeGiroGlobal = 990;
int velocidadeGlobal = 295;

void Main(){
    bc.ResetTimer();

    bc.PrintConsole(1, "== BEM VINDO KIM ===");

    bc.ActuatorSpeed(150); 
    Thread threadAlturaBalde = new Thread(AjustarAlturaBalde); 
    // Thread threadAnguloGarra = new Thread(AjustarAnguloBalde);
    
    threadAlturaBalde.Start();
    AjustarAnguloBalde();

    while(true){
        while(estagio == "Pista"){

            // --- Desvio do Verde ---
            if((bc.ReturnColor(0) == "GREEN" || bc.ReturnColor(1) == "GREEN") && (bc.ReturnColor(3) == "GREEN" || bc.ReturnColor(4) == "GREEN")){Verde("Ambos");}  // verde dos dois lados

            else if(bc.ReturnColor(3) == "GREEN" || bc.ReturnColor(4) == "GREEN"){  
            Verde("Esquerda");}  // verde esquerda
            
            else if(bc.ReturnColor(0) == "GREEN" || bc.ReturnColor(1) == "GREEN"){  
            Verde("Direita");}  // verde direita
            
            // --- Saída Final ---
            if( final && bc.ReturnColor(1) == "RED" && bc.ReturnColor(3) == "RED" ){
                bc.PrintConsole(2, "AEEEEE TERMINOOOOOOO");
                bc.MoveFrontal(0, 0);
                bc.Wait(10000);
            }

            // --- Curva 90º ---
            if((MedirLuz(0) < escuro && MedirLuz(1) < escuro)){ 
            Curva90("Direita", claro);}
            
            if((MedirLuz(3) < escuro && MedirLuz(4) < escuro)){
            Curva90("Esquerda", claro);}

            // --- Desvio Objeto ---
            if(bc.Distance(2)<=15f){                        //Função para detectar o obstáculo utilizando o sensor de ultrassom
                bc.MoveFrontal(0,0);
                bc.Wait(100);
                DesvioUltrassom();                      //Aplicando a função definida
            }

            // --- Gangorra --- 
            if( bc.Inclination() > 335 && bc.Inclination() < 350 && bc.Distance(1) > 40 ){
                Gangorra();
            }

            // --- Rampa Final ---
            if( bc.Inclination() > 335 && bc.Inclination() < 345 && bc.Distance(1) < 40){
                bc.PrintConsole(2, "Vou pra rampa");
                bc.MoveFrontal(290, 290);
                bc.Wait(300);
                ChegarNoAngulo(AproximarAngulo(bc.Compass()));
                MoverEscavadora(312);
                estagio = "Rampa";
            }


            // --- Recuperar Linha ---
            if(MedirLuz(1) < claro || MedirLuz(2) < claro || MedirLuz(3) < claro) {
                controleTempoRecuperarLinha = 0;}

            else if(MedirLuz(1) > claro && MedirLuz(2) > claro && MedirLuz(3) > claro){
                RecuperarLinha(950);} 

            // --- Seguidor de Linha --- 
            
            // com clamping = 1:20
            // sem clamping = 1:22

            //150, 20, 1, 5 = 1:16
            //200, 22, 1, 6 = 1:15
            // 200, 24, 0.1f, 10 = 1:20
            seguirLinhaPID(velocidadeFrontal, 30, 0.3f, 6);
        }
        while(estagio == "Rampa"){

            bc.PrintConsole(2, "Rampa");

            if(bc.Inclination() > 345){
                bc.MoveFrontal(0, 0);
                Tick();
                ChegarNoAngulo(AproximarAngulo(bc.Compass()));
                estagio = "Resgate";
                break;
            }

            if(bc.Inclination() == 0){
                bc.PrintConsole(2, "Voltando pra pista");
                bc.MoveFrontal(0, 0);
                Tick();
                estagio = "Pista";
            }

            // --- Seguir Linha --- //
            bc.MoveFrontal(200, 200);
            Tick();
        }

        if(estagio == "Resgate"){
            bc.PrintConsole(2, "Resgate");
            bc.ActuatorSpeed(150);

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
        
            PosicionarMeio(velocidadeGlobal);

            bc.PrintConsole(0, "Vou começar o radar");
            bc.MoveFrontal(0, 0);

            Radar();

            bc.PrintConsole(0, "Vou embora");
            bc.MoveFrontal(0, 0);

            IrEmbora();
            final = true;
            estagio = "Pista";
        }
    }
}