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
                bc.Move(velocidade, - velocidade);
                Tick();
            }
        }
        // Movimento regular
        else{
            while(bc.Compass() < anguloInicial + anguloMovimento){
                bc.Move(velocidade, - velocidade);
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
                bc.Move(-velocidade, velocidade);
                Tick();
            }
        }
        //Movimento regular
        else{
            while(bc.Compass() > anguloInicial - anguloMovimento){
                bc.Move(-velocidade, velocidade);
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


void DesvioUltrassom(){
    int velocidadeFrontal = 100, velocidadeGiro = 950;
    int anguloInicialDesvio = AproximarAngulo(bc.Compass());

    ChegarNoAngulo(MatematicaCirculo(anguloInicialDesvio + 270));

    bc.MoveFrontal(velocidadeFrontal,velocidadeFrontal);
    bc.Wait(980);

    ChegarNoAngulo(MatematicaCirculo(anguloInicialDesvio) - 3);

    while(MedirLuz(0) > claro && MedirLuz(1) > claro){
        if(bc.Distance(1) < 25){
            bc.PrintConsole(2, "Bloco detectado a direita");
            
            bc.MoveFrontal(velocidadeFrontal, velocidadeFrontal);
            bc.Wait(1650);

            RetornarCirculo(88, velocidadeGiro);
        }
        else{
            bc.MoveFrontal(velocidadeFrontal,velocidadeFrontal);
            Tick();
        }
    }
   
    bc.PrintConsole(2, "Linha detectada");

    bc.MoveFrontal(velocidadeFrontal, velocidadeFrontal);
    bc.Wait(900);

    
    RetornarCirculo(-90, velocidadeGiro);

    bc.MoveFrontal(0, 0);
    Tick();
}

float MedirLuz(int sensor){
    return bc.Lightness(sensor) ;
}

float claro = 55, escuro = 37;
int velocidadeFrontal = 100;
int velocidadeGiroGlobal = 990;
int velocidadeGlobal = 295;

void Main(){
	while(true){
		if(bc.Distance(0) <= 15 || bc.ReturnColor(2) != "BRANCO" || bc.ReturnColor(2) != "CIANO"){
			bc.MoveFrontal(0, 0);
			bc.Wait(100);

			DesvioUltrassom();
		}
		else{
			bc.MoveFrontalRotations(200, 5);
			bc.Wait(100);
		}
	}
}
