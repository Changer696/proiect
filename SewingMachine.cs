using System;

public class SewingMachine : Machine
{
    public SewingMachine(string serial, string nume, DateTime dataFabricatiei)
        : base(serial, nume, dataFabricatiei)
    {
    }

    // Produces items for sewing machines, degrades condition and checks parts.
    public override void Produce()
    {
        if (Status != MachineStatus.Running)
        {
            Console.WriteLine(Messages.SewingMachineCannotProduce(Nume));
            return;
        }
        Console.WriteLine(Messages.SewingMachineProducing(Nume));
        DegradeazaConditia();
        StareVerificarePiesa();
        RegisterProductionCycle();
    }

    // Produces items for sewing machines, degrades condition and checks parts.
    public override string RunDiagnostics()
    {
        if (Conditie == MachineCondition.Critical)
            return Messages.SewingMachineCriticalDiagnostic;
        else
            return Messages.SewingMachineNormalDiagnostic;
    }
}
