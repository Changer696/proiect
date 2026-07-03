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
        Console.WriteLine(Nume + " (Director) revizuieste rapoartele fabricii.");
    }

    public override void Afiseaza()
    {
        Console.WriteLine("[" + Id + "] " + Nume +
                          " - Director" +
                          " - Salariu: " + Salariu +
                          " - Vechime: " + GetVechimeZile() + " zile");
    }
}
