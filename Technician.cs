using System;

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
            Console.WriteLine(Nume + " nu poate repara o masina pornita!");
            return false;
        }

        for (int i = 0; i < masina.NrPiese; i++)
        {
            if (!masina.Piese[i].EFunctionala)
            {
                masina.Piese[i].Inlocuieste();
                Console.WriteLine("Piesa " + masina.Piese[i].Nume + " a fost inlocuita.");
            }
        }

        masina.RestoreazaConditia();
        Console.WriteLine(Nume + " a reparat masina " + masina.Nume);
        return true;
    }

    public override void PerformDuty()
    {
        Console.WriteLine(Nume + " (Technician) repara masinile.");
    }
}
