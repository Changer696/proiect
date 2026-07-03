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
            Console.WriteLine("Stoc insuficient! Disponibil: " + produs.Cantitate);
            return false;
        }
        produs.VindeStoc(cantitate);
        Console.WriteLine(Nume + " a vandut " + cantitate + "x " + produs.Nume);
        return true;
    }

    public override void PerformDuty()
    {
        Console.WriteLine(Nume + " (Sales Agent) vinde produsele fabricii.");
    }
}
