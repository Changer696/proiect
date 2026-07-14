using System;
using SmartFactorySimple;

public abstract class Machine
{
    public string SerialNumber;
    public string Nume;
    public MachineStatus Status;
    public MachineCondition Conditie;
    public DateTime DataFabricatiei;
    public static Random _random = new Random();

    public MachinePart[] Piese;
    public int NrPiese;

    protected Machine(string serialNumber, string nume, DateTime dataFabricatiei)
    {
        SerialNumber = serialNumber;
        Nume = nume;
        DataFabricatiei = dataFabricatiei;
        Status = MachineStatus.Stopped;
        Conditie = MachineCondition.Good;
        Piese = new MachinePart[10];
        NrPiese = 0;
    }

    public void AdaugaPiesa(MachinePart piesa)
    {
        Piese[NrPiese] = piesa;
        NrPiese++;
    }

    public bool ArePieseComplete()
    {
        if (NrPiese == 0)
            return false;

        for (int i = 0; i < NrPiese; i++)
        {
            if (!Piese[i].EFunctionala)
                return false;
        }
        return true;
    }

    public void StareVerificarePiesa()
    {
        if (NrPiese == 0)
        {
            Console.WriteLine("There are no existing parts");
        }
         int sansa = _random.Next(1,101);
         if(sansa <= 20)
        {
            int index = _random.Next(NrPiese);
            if (Piese[index].EFunctionala)
            {
                Piese[index].Strica();
                Console.WriteLine($"Pieces:{Piese[index].Nume} from {Nume} are broken");
            }
        }
    }

    public int GetVarstaZile()
    {
        TimeSpan varsta = DateTime.Now - DataFabricatiei;
        return varsta.Days;
    }

    public virtual void Start()
    {
        if (Status == MachineStatus.Maintenance)
        {
            Console.WriteLine(Nume + " It's under maintenance, it can't be started!");
            return;
        }
        if (!ArePieseComplete())
        {
            Console.WriteLine(Nume + " has broken or missing parts!");
            return;
        }
        Status = MachineStatus.Running;
        Console.WriteLine(Nume + " was turned on.");
        Logging.Log($"Started machine {SerialNumber}");
    }

    public virtual void Stop()
    {
        Status = MachineStatus.Stopped;
        Console.WriteLine(Nume + " was stopped.");
        Logging.Log($"Stopped machine {SerialNumber}");
    }

    public void SetMaintenance()
    {
        if (Status == MachineStatus.Running)
        {
            Console.WriteLine("Stop the machine before maintenance!");
            return;
        }
        Status = MachineStatus.Maintenance;
    }

    protected void DegradeazaConditia()
    {
        if (Conditie == MachineCondition.Excellent)
            Conditie = MachineCondition.Good;
        else if (Conditie == MachineCondition.Good)
            Conditie = MachineCondition.Worn;
        else if (Conditie == MachineCondition.Worn)
        { Conditie = MachineCondition.Critical; Status = MachineStatus.Maintenance; }
    
    }

    public void RestoreazaConditia()
    {
        Conditie = MachineCondition.Excellent;
        Status = MachineStatus.Stopped;
    }

    public abstract void Produce();
    public abstract string RunDiagnostics();

    public virtual void Afiseaza()
    {
        Console.WriteLine("[" + SerialNumber + "] " + Nume +
                          " - Status: " + Status +
                          " - Condition: " + Conditie +
                          " - Age: " + GetVarstaZile() + " zile" +
                          " - Pieces: " + NrPiese);
    }
}
