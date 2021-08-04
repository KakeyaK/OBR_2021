// Mover Escavadora
void MoverEscavadora(double alvoEscavadora)  //o alvo é o angulo exato em q vc quer q pare, e n o tanto q vc quer q mude
{
    if (Math.Sin(bc.AngleActuator()*Math.PI/180) > Math.Sin(alvoEscavadora*Math.PI/180))
    {
        //enquanto o seno da posicao atual da escavadora for menor q o seno da posicao alvo, a escavadora sobe
        while (Math.Sin(bc.AngleActuator()*Math.PI/180) > Math.Sin(alvoEscavadora*Math.PI/180))
        {
            //A escavadora tem os angulos invertidos :P
            bc.PrintConsole(1, "Escavadora Subindo");
            bc.ActuatorUp(30);
        }
    }

    else
    {
        //enquanto o seno da posicao atual da escavadora for maior q o seno da posicao alvo, a escavadora desce
        while (Math.Sin(bc.AngleActuator()*Math.PI/180) < Math.Sin(alvoEscavadora*Math.PI/180))
        {
            bc.PrintConsole(1, "Escavadora Descendo");
            bc.ActuatorDown(30);
        }
    }
}

// Mover Balde
void MoverBalde(double alvoBalde) //o alvo é o angulo exato em q vc quer q pare, e n o tanto q vc quer q mude
{
    if (Math.Sin(bc.AngleScoop()*Math.PI/180) > Math.Sin(alvoBalde*Math.PI/180))
    {
        //enquanto o seno da posicao atual da escavadora for menor q o seno da posicao alvo, a escavadora sobe
        while (Math.Sin(bc.AngleScoop()*Math.PI/180) > Math.Sin(alvoBalde*Math.PI/180))
        {
            bc.PrintConsole(2, "Balde Descendo");
            bc.TurnActuatorDown(30);
        }
    }

    else
    {
        //enquanto o seno da posicao atual da escavadora for maior q o seno da posicao alvo, a escavadora desce
        while (Math.Sin(bc.AngleScoop()*Math.PI/180) < Math.Sin(alvoBalde*Math.PI/180))
        {
            bc.PrintConsole(2, "Balde Subindo");
            bc.TurnActuatorUp(30);
        }
    }
}

// Resgatar
void Pegar()
{
    bool hasVictim = bc.HasVictim();
    if (hasVictim == true)
    {
        MoverEscavadora(350);
        bc.PrintConsole(1,"Capturado");
        bc.Wait(100);
    }
    else
    {
        bc.PrintConsole(1,"Não há vítima");
        bc.Wait(100);
    }
}

// Devolver
void Devolver()
{
    bc.PrintConsole(1,"Devolvendo");
    MoverEscavadora(10);
}

//Isso é o que tem q incluir na programação principal, mas vai ter q revisar. O q tem de importante e pronto são as funcoes q o main chama
void Main()
{   
    bc.ActuatorSpeed(150);
    bc.PrintConsole(1,"Início da Captura");
    
    // abaixar a escavadora e o balde pra pegar a vitima
    MoverEscavadora(11);
    MoverBalde(10);

    bc.MoveFrontal(250,250);
    bc.PrintConsole(1,"Andando");
    bc.Wait(1000);
    Pegar();
}