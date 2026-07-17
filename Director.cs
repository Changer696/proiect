using System;

public class Director : Employee
{
    public Director(string id, string nume, decimal salariu, DateTime dataAngajarii)
        : base(id, nume, salariu, dataAngajarii)
    {
        Rol = EmployeeRole.Director;
    }

    public override void PerformDuty()
    {
        Console.WriteLine(Messages.DirectorDuty(Nume));
    }

    // Prints director-specific duty message.
    public override void Afiseaza()
    {
        Console.WriteLine(Messages.DirectorDisplay(Id, Nume, Salariu, GetVechimeZile()));
    }
}
