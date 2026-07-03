using System;

public class SalesAgent : Employee
{
    public SalesAgent(string id, string nume, decimal salariu, DateTime dataAngajarii)
        : base(id, nume, salariu, dataAngajarii)
    {
        Rol = EmployeeRole.SalesAgent;
    }

    public bool VindeProdus(Product produs, int cantitate)
    {
        if (produs.Cantitate < cantitate)
        {
            Console.WriteLine("Insufficient stock! Available: " + produs.Cantitate);
            return false;
        }
        produs.VindeStoc(cantitate);
        Console.WriteLine(Nume + " sold " + cantitate + "x " + produs.Nume);
        return true;
    }

    public override void PerformDuty()
    {
        Console.WriteLine(Nume + " (Sales Agent) sell the factory's products.");
    }
}
