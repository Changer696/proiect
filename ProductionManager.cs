using System;
public class ProductionManager : Employee
{
    public ProductionManager(string id, string nume, decimal salariu, DateTime dataAngajarii)
        : base(id, nume, salariu, dataAngajarii)
    {
        Rol = EmployeeRole.ProductionManager;
    }

    public ProductionOrder CreazaComanda(string idComanda, Machine masina,
                                          string produs, int cantitate, Priority prioritate)
    {
        Console.WriteLine(Messages.ProductionOrderCreated(Nume, prioritate, idComanda, cantitate, produs));
        return new ProductionOrder(idComanda, masina, this, produs, cantitate, prioritate);
    }

    public override void PerformDuty()
    {
        Console.WriteLine(Messages.ProductionManagerDuty(Nume));
    }
}
