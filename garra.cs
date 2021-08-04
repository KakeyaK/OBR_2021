// Escavadora
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

// Balde
void MoverBalde()  //o alvo é o angulo exato em q vc quer q pare, e n o tanto q vc quer q mude
{
    double alvoBalde = 7.5;
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
    if (hasVictim = true)
    {
        MoverEscavadora(350);
    }
}

void Main()
{   
    bc.ActuatorSpeed(100);
    Thread res = new Thread(MoverBalde);
    res.Start();
    MoverEscavadora(11);
    bc.PrintConsole(1,"AAAAA");
    bc.MoveFrontal(300,300);
    bc.Wait(1000);
    Pegar();
}