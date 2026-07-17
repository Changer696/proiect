using System;

public abstract class Employee : IIdentifiable
{
    public string Id { get; set; }
    public string Nume;
    public EmployeeRole Rol;
    public decimal Salariu;
    public DateTime DataAngajarii;

    protected Employee(string id, string nume, decimal salariu, DateTime dataAngajarii)
    {
        Id = id;
        Nume = nume;
        Salariu = salariu;
        DataAngajarii = dataAngajarii;
    }

    // Returns the employee tenure in days calculated from hiring date.
    public int GetVechimeZile()
    {
        TimeSpan diferenta = DateTime.Now - DataAngajarii;
        return diferenta.Days;
    }

    // Performs the role-specific duty for the employee (implemented by subclasses).
    public abstract void PerformDuty();

    // Displays basic employee information to the console.
    public virtual void Afiseaza()
    {
        Console.WriteLine(Messages.EmployeeDisplay(Id, Nume, Rol, Salariu, GetVechimeZile()));
    }
}
