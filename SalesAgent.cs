using System;
using SmartFactorySimple;

public class SalesAgent : Employee
{
    public SalesAgent(string id, string nume, decimal salariu, DateTime dataAngajarii)
        : base(id, nume, salariu, dataAngajarii)
    {
        Rol = EmployeeRole.SalesAgent;
    }

    public bool VindeProdus(Product produs, int cantitate, Factory fabrica)
    {
        if (produs.Cantitate < cantitate)
        {
            Console.WriteLine(Messages.InsufficientStockAvailable(produs.Cantitate));
            return false;
        }
        fabrica.RecordSale(produs.Nume, cantitate, produs.SellingPrice);
        Console.WriteLine(Messages.SalesAgentSale(Nume, cantitate, produs.Nume));
        Logging.Log(Id, Messages.ProductSoldLog(produs.Nume, cantitate));
        return true;
    }

    public override void PerformDuty()
    {
        Console.WriteLine(Messages.SalesAgentDuty(Nume));
    }
}
