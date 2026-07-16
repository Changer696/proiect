using System;

public class Engineer : Employee
{
    public Engineer(string id, string nume, decimal salariu, DateTime dataAngajarii)
        : base(id, nume, salariu, dataAngajarii)
    {
        Rol = EmployeeRole.Engineer;
    }

    public void Inspecteaza(Machine masina)
    {
        string rezultat = masina.RunDiagnostics();
        Console.WriteLine(Messages.EngineerInspection(Nume, masina.Nume, rezultat));
    }

    public override void PerformDuty()
    {
        Console.WriteLine(Messages.EngineerDuty(Nume));
    }
}
