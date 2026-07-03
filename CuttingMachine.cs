using System;

public class CuttingMachine : Machine
{
    public CuttingMachine(string serial, string nume, DateTime dataFabricatiei)
        : base(serial, nume, dataFabricatiei)
    {
    }

    public override void Produce()
    {
        if (Status != MachineStatus.Running)
        {
            Console.WriteLine(Nume + " nu e pornita, nu poate produce!");
            return;
        }
        Console.WriteLine(Nume + " taie materialul dupa tipare.");
        DegradeazaConditia();
        StareVerificarePiesa();

    }

    public override string RunDiagnostics()
    {
        if (Conditie == MachineCondition.Critical)
            return "ATENTIE: Lama e tocita, necesita inlocuire!";
        else
            return "Lama ascutita. Functionare normala.";
    }
}
