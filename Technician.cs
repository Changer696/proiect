using System;
using SmartFactorySimple;

public class Technician : Employee
{
    public Technician(string id, string nume, decimal salariu, DateTime dataAngajarii)
        : base(id, nume, salariu, dataAngajarii)
    {
        Rol = EmployeeRole.Technician;
    }

    public bool Repara(Machine masina)
    {
        if (masina.Status == MachineStatus.Running)
        {
            Console.WriteLine(Messages.TechnicianCannotRepair(Nume));
            return false;
        }

        for (int i = 0; i < masina.NrPiese; i++)
        {
            if (!masina.Piese[i].EFunctionala)
            {
                masina.Piese[i].Inlocuieste();
                Console.WriteLine(Messages.PartReplaced(masina.Piese[i].Nume));
            }
        }

        masina.RestoreazaConditia();
        Console.WriteLine(Messages.TechnicianRepaired(Nume, masina.Nume));
        Logging.Log(Id, $"Repaired machine {masina.SerialNumber}");
        return true;
    }
    // Attempts to repair a machine by replacing faulty parts and restoring condition.
    public override void PerformDuty()
    {
        Console.WriteLine(Messages.TechnicianDuty(Nume));
    }
}
