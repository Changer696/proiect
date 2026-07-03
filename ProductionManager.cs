using System;
public class ProductionManager : Employee
{
    public ProductionManager(string id, string nume, decimal salariu, DateTime dataAngajarii)
        : base(id, nume, salariu, dataAngajarii)
    {
        Rol = EmployeeRole.ProductionManager;
    }

    public ProductionOrder CreazaComanda(string idComanda, Machine masina,
                                          string produs, int cantitate)
    {
        Console.WriteLine(Nume + " a creat comanda " + idComanda + " pentru " + cantitate + "x " + produs);
        ProductionOrder comanda = new ProductionOrder(idComanda, masina, this, produs, cantitate);
        comanda.SetPriority();
        return comanda;
    }

    public override void PerformDuty()
    {
        Console.WriteLine(Nume + " (Production Manager) coordoneaza productia.");
    }
}
