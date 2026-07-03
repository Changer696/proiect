using System;

public class SewingMachine : Machine
{
    public SewingMachine(string serial, string nume, DateTime dataFabricatiei)
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
        Console.WriteLine(Nume + " coase materialul si produce o camasa.");
        DegradeazaConditia();
        StareVerificarePiesa();
    }

    public override string RunDiagnostics()
    {
        if (Conditie == MachineCondition.Critical)
            return "ATENTIE: Tensiunea acului e neregulata!";
        else
            return "Ac si ata verificate. Functionare normala.";
    }
}
