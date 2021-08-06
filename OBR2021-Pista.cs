/*
Título      :   Pista OBR
Autores     :   Kim & Breno
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

// ===============
// Funções de suporte
// ===============

// ====== Função de Passar Tempo ====== //
void Tick(){ bc.Wait(30); } 

// ====== Funções de Matemática com Ângulos ====== //

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
void RetornarCirculo(float anguloMovimento, float velocidade){
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

float MatematicaCirculo(float angulo){
    if(angulo > 360){
        return angulo - 360;
    }
    else if(angulo < 0){
        
        return (float) (-360 * Math.Floor( (double) (angulo / 360) ) + angulo);
    }
    else{
        return angulo;
    }
}

// ====== Funções luz ===== //
float MedirLuz(int sensor){
    return bc.Lightness(sensor) ;
}

// ===== Posicionamento inicial garra ===== //

//balde = 318, escavadora = 318

void AjustarAnguloBalde(){
    while( Math.Sin(bc.AngleScoop() * Math.PI / 180 ) > Math.Sin(318 * Math.PI / 180)){

		bc.TurnActuatorDown(40);

    }
}

void AjustarAlturaBalde(){
    while( Math.Sin(bc.AngleActuator() * Math.PI / 180 ) > Math.Sin(290 * Math.PI / 180)){

		bc.ActuatorUp(40);

    }
}

// ===============
// Funções da pista
// ===============

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
                    break;
                }
            }
        }
    }

    bc.MoveFrontal(-velocidadeFrontal, -velocidadeFrontal);
    bc.Wait(200);

    bc.MoveFrontal(0, 0);
    bc.Wait(100);
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

    RetornarCirculo(MatematicaCirculo(AproximarAngulo(bc.Compass()) - bc.Compass() - 90) - 360, velocidadeGiro);

    bc.MoveFrontal(velocidadeFrontal,velocidadeFrontal);
    bc.Wait(1500);

    RetornarCirculo(MatematicaCirculo(AproximarAngulo(bc.Compass()) - bc.Compass() + 90), velocidadeGiro);

    while(MedirLuz(1) > claro && MedirLuz(2) > claro && MedirLuz(3) > claro){
        if(bc.Distance(1) < 25){
            bc.PrintConsole(2, "Bloco detectado a direita");
            
            bc.MoveFrontal(velocidadeFrontal, velocidadeFrontal);
            bc.Wait(1800);

            RetornarCirculo(MatematicaCirculo(AproximarAngulo(bc.Compass()) - bc.Compass() + 90), velocidadeGiro);
  }
        else{
            bc.MoveFrontal(velocidadeFrontal,velocidadeFrontal);
            Tick();
        }
    }
    
    bc.PrintConsole(2, "Linha detectada");

    bc.MoveFrontal(velocidadeFrontal, velocidadeFrontal);
    bc.Wait(950);

    RetornarCirculo(MatematicaCirculo(AproximarAngulo(bc.Compass()) - bc.Compass() - 90) - 360, velocidadeGiro);
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
                bc.Wait(12000);
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
            seguirLinhaPID(200, 30, 0.3f, 6);
        }
        while(estagio == "Resgate"){
            bc.PrintConsole(2, "Resgate");

            bc.MoveFrontal(1000, -1000);
            Tick();
        }
    }
}