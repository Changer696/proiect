using System;

public class CuttingMachine : Machine
{
    public CuttingMachine(string serial, string nume, DateTime dataFabricatiei)
        : base(serial, nume, dataFabricatiei)
    {
    }

    // Produces items for cutting machines, degrades condition and checks parts.
    public override void Produce()
    {
        if (Status != MachineStatus.Running)
        {
            Console.WriteLine(Messages.MachineCannotProduce(Nume));
            return;
        }
        Console.WriteLine(Messages.CuttingMachineProducing(Nume));
        DegradeazaConditia();
        StareVerificarePiesa();
        RegisterProductionCycle();

    }

    // Produces items for cutting machines, degrades condition and checks parts.
    public override string RunDiagnostics()
    {
        if (Conditie == MachineCondition.Critical)
            return Messages.CuttingMachineCriticalDiagnostic;
        else
            return Messages.CuttingMachineNormalDiagnostic;
    }
}
