/*
Título      :   Pista OBR (sem rampa e obstáculo)

Autor       :   Kim

Versão      :   
                0.3 Função de recuperar linha
                0.2.1 Calibração das funções
                0.2 Função de retornar círculo
                0.1 (funcionalidades inciais)

Issues      :   
                conflito desvio verde com recuperação de linha
                adicionar ultrassom
                lidar rampas
                lidar com luzes 


                Recuperar linhas usando função seno ok
                Testar para curvas de 90º ok
                Verificar fator integral ok

    Observar:

        Retornar a linha
*/

// ====== Variáveis PID ====== //
float error = 0, lastError = 0, integral = 0, derivate = 0;
float movimento;
bool pararIntegro;

// ====== Variáveis Gerais ====== //
// - Recuperação de linha
int controleTempo = 0, tempoInicial = 0, anguloInicial = 0;

// ===============
// Funções de suporte
// ===============

// ====== Função de Passar Tempo ====== //
void Tick(){ bc.Wait(30); } 

// Retornar aproximação de ângulo para um dos pontos cardeais
int AproximarAngulo(){
    if(bc.Compass() >= 315 || bc.Compass() < 45) return 0;
    if(bc.Compass() >= 45 && bc.Compass() < 135) return 90;
    if(bc.Compass() >= 135 && bc.Compass() < 225) return 180;
    if(bc.Compass() >= 225 && bc.Compass() < 315) return 270;
    else return 0;
}

// Girar por graus, independente da orientação
// positivo para sentido horário
// negativo para sentido anti-horário
// Margem de erro > 5º
// Máximo de movimento em uma direção = 355
void RetornarCirculo(float angulo, float velocidade){
    float anguloInicial = bc.Compass();
    
    // Movimento positivo - sentido horário
    if(angulo > 0){
        // Alterando os valores para evitar o loop infinito em 360º/0º
        if(anguloInicial + angulo > 358 && anguloInicial + angulo < 362){ anguloInicial += 5; }   

        // Movimento passa pelo limite de 0/360º
        if(anguloInicial + angulo > 360){
            while(bc.Compass() > anguloInicial + angulo - 355 || bc.Compass() < anguloInicial + angulo - 360){
                bc.MoveFrontal(-velocidade, velocidade);
                Tick();
            }
        }
        // Movimento regular
        else{
            while(bc.Compass() < anguloInicial + angulo){
                bc.MoveFrontal(-velocidade, velocidade);
                Tick();
            }
        }
    }
    else{
        // Invertendo o sinal do ângulo pra facilitar a visualização da matemática
        angulo = angulo * -1;
        // Alterando os valores para evitar o loop infinito em 360º/0º
        if(anguloInicial - angulo > -2 && anguloInicial - angulo < 2){ anguloInicial = anguloInicial - 5; }

        // Movimento passa pelo limite de 0/360º
        if(anguloInicial < angulo){
            while(bc.Compass() < anguloInicial + 355 - angulo || bc.Compass() > anguloInicial + 360 - angulo){
                bc.MoveFrontal(velocidade, -velocidade);
                Tick();
            }
        }
        //Movimento regular
        else{
            while(bc.Compass() > anguloInicial - angulo){
                bc.MoveFrontal(velocidade, -velocidade);
                Tick();
            }
        }
    }
}
// ===============
// Funções de pista
// ===============

void RecuperarLinha(int velocidadeFrontal, int velocidadeGiro){

    if(controleTempo == 0){
        controleTempo = 1;
        tempoInicial = bc.Timer();
        anguloInicial = AproximarAngulo();
    }

    else if(tempoInicial + 2000 < bc.Timer()){
        bc.PrintConsole(1, "Voltando na Linha");

        bc.MoveFrontal(-velocidadeFrontal, -velocidadeFrontal);
        bc.Wait(1500);
        RetornarCirculo(anguloInicial - bc.Compass(), velocidadeGiro);
        controleTempo = 0;
    }
}

void seguirLinhaPID (float velocidade, float kp, float ki,float kd){
    // matemática PID
    error = bc.Lightness(1) - bc.Lightness(3);
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
    bc.PrintConsole(1, "Derivate: " + derivate.ToString("F"));
    bc.PrintConsole(2, "Integral: " + integral.ToString() + " Integro Parado: " + pararIntegro.ToString());

    // Movimento
    bc.MoveFrontal(velocidade + movimento, velocidade - movimento);
    Tick();

    // Atualização de variável
    lastError = error;
}

void Curva90(int velocidadeFrontal, int velocidadeGiro, int curva, float claro = 25){
    
    bc.MoveFrontal(velocidadeFrontal, velocidadeFrontal);
    bc.Wait(450);

    bc.MoveFrontal(0, 0);
    bc.Wait(100);

    // desvio para esquerda
    if( curva == 1 ){

        // detectar se é curva ou interseção e virar caso necessário
        if( bc.Lightness(1) > claro && bc.Lightness(2) > claro && bc.Lightness(3) > claro ){
            
            bc.PrintConsole(1, "Virando Esquerda");

            while(bc.Lightness(2) > claro){

                bc.MoveFrontal(velocidadeGiro, -velocidadeGiro);
                Tick();
            }
        }
    }

    //devio para direita
    if( curva == 2 ){
        bc.PrintConsole(1, "Curva Direita!");
        
        // detectar se é curva ou interseção e virar caso necessário
        if( bc.Lightness(1) > claro && bc.Lightness(2) > claro && bc.Lightness(3) > claro ){
            
            bc.PrintConsole(1, "Virando Direita");

            while(bc.Lightness(2) > claro){
                
                bc.MoveFrontal(-velocidadeGiro, velocidadeGiro);
                Tick();
            }
        }
    }

    bc.MoveFrontal(-velocidadeFrontal, -velocidadeFrontal);
    bc.Wait(200);

    bc.MoveFrontal(0, 0);
    bc.Wait(100);
}

void Verde(int velocidadeFrontal, int velocidadeGiro, int curva){
    
    bc.MoveFrontal(velocidadeFrontal, velocidadeFrontal);
    bc.Wait(1350);

    bc.MoveFrontal(0, 0);
    bc.Wait(100);

    if(curva == 1){
        
        bc.PrintConsole(1, "Verde Esquerda");

        RetornarCirculo(-10, velocidadeGiro);

        while(bc.Lightness(2) > claro){

            bc.MoveFrontal(velocidadeGiro, -velocidadeGiro);
            Tick();
        }

        bc.PrintConsole(1, "");
    }
    if(curva == 2){
        
        bc.PrintConsole(1, "Verde Direita");

        RetornarCirculo(10, velocidadeGiro);

        while(bc.Lightness(2) > claro){
                
            bc.MoveFrontal(-velocidadeGiro, velocidadeGiro);
            Tick();
        }

        bc.PrintConsole(1, "");
    }
    if(curva == 3){
        // Girar 180º
    }

    bc.MoveFrontal(-velocidadeFrontal, -velocidadeFrontal);
    bc.Wait(300);

    bc.MoveFrontal(0, 0);
    bc.Wait(100);
}

// ====== Variáveis Específicas (A serem calibradas) ====== //
float claro = 35, escuro = 25; 
int velocidadeFrontal = 140, velocidadeGiro = 900;

void Main(){
    bc.PrintConsole(1, "== BEM VINDO KIM ===");

    while(true){

        // --- Desvio do Verde ---
        if(bc.ReturnColor(3) == "GREEN" || bc.ReturnColor(4) == "GREEN")  Verde(velocidadeFrontal = velocidadeFrontal, velocidadeGiro = velocidadeGiro, 1);  // verde esquerda
        if(bc.ReturnColor(0) == "GREEN" || bc.ReturnColor(1) == "GREEN")  Verde(velocidadeFrontal = velocidadeFrontal, velocidadeGiro = velocidadeGiro, 2);  // verde direita
        if((bc.ReturnColor(0) == "GREEN" || bc.ReturnColor(1) == "GREEN") && (bc.ReturnColor(3) == "GREEN" || bc.ReturnColor(4) == "GREEN"))  Verde(velocidadeFrontal = velocidadeFrontal, velocidadeGiro = velocidadeGiro, 3);  // verde dos dois lados

        // --- Curva 90º ---
        if((bc.Lightness(0) < escuro && bc.Lightness(1) < escuro) || (bc.Lightness(1) < escuro && bc.Lightness(2) < escuro)) Curva90(velocidadeFrontal = velocidadeFrontal, velocidadeGiro = velocidadeGiro, 2, claro);  // curva esquerda
        if((bc.Lightness(2) < escuro && bc.Lightness(3) < escuro) || (bc.Lightness(3) < escuro && bc.Lightness(4) < escuro)) Curva90(velocidadeFrontal = velocidadeFrontal, velocidadeGiro = velocidadeGiro, 1, claro);  // curva direita

        // --- Recuperar Linha ---
        if(bc.Lightness(1) < claro || bc.Lightness(2) < claro || bc.Lightness(3) < claro) controleTempo = 0;
        if(bc.Lightness(1) > claro && bc.Lightness(2) > claro && bc.Lightness(3) > claro) RecuperarLinha(velocidadeFrontal = velocidadeFrontal, velocidadeGiro = velocidadeGiro);

        // --- Seguidor de Linha --- 
        
        // com clamping = 1:20
        // sem clamping = 1:22

        //150, 20, 1, 5 = 1:16
        //200, 22, 1, 6 = 1:15
        // 200, 24, 0.1f, 10 = 1:20
        seguirLinhaPID(velocidadeFrontal, 10, 0.5f, 5);
        
    }
}