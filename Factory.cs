using System;
using System.Collections.Generic;
using System.Linq;
using SmartFactorySimple;

public class Factory
{
    public string Nume;

    
    private EmployeeRepository _employeeRepository = new EmployeeRepository();
    private MachineRepository _machineRepository = new MachineRepository();
    private ProductRepository _productRepository = new ProductRepository();
    private ProductionOrderRepository _orderRepository = new ProductionOrderRepository();

    private int _idComandaCounter = 1;
    private decimal _totalRevenue = 0;
    private int _totalSalesQuantity = 0;

    public Factory(string nume)
    {
        Nume = nume;
    }

    

    public bool AdaugaAngajat(Employee angajat)
    {
        bool added = _employeeRepository.Add(angajat);
        if (added)
        {
            Logging.Log(angajat.Id, Messages.EmployeeAddedLog(angajat.Nume));
        }
        return added;
    }

    public bool EmployeeIdExists(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return false;

        return _employeeRepository.ExistsById(id);
    }

    public void AfiseazaAngajati()
    {
        _employeeRepository.DisplayAll();
    }

    public Employee GasesteAngajat(string id)
    {
        return _employeeRepository.FindById(id);
    }

    public bool StergeAngajat(string id)
    {
        if (_employeeRepository.RemoveById(id))
        {
            Console.WriteLine(Messages.EmployeeDeleted());
            Logging.Log(id, Messages.EmployeeRemovedLog(id));
            return true;
        }
        else
        {
            Console.WriteLine(Messages.EmployeeDoesNotExist);
            return false;
        }
    }


    public bool AdaugaMasina(Machine masina)
    {
        return _machineRepository.Add(masina);
    }

    public void AfiseazaMasini()
    {
        _machineRepository.DisplayAll();
    }

    public Machine GasesteMasina(string serial)
    {
        return _machineRepository.FindBySerialNumber(serial);
    }



    public bool AdaugaProdus(Product produs)
    {
        _productRepository.Add(produs);
        return true;
    }

    public void AfiseazaProduse()
    {
        _productRepository.DisplayAll();
    }

    public Product GasesteProdus(string nume)
    {
        return _productRepository.FindByName(nume);
    }

    

    public void CreazaComanda(string idManager, string serialMasina,
                               string produs, int cantitate, Priority prioritate)
    {
        Employee angajat = GasesteAngajat(idManager);
        if (angajat == null)
        {
            Console.WriteLine(Messages.EmployeeDoesNotExist);
            return;
        }

        if (!(angajat is ProductionManager))
        {
            Console.WriteLine(Messages.NotProductionManager(angajat.Nume));
            return;
        }

        ProductionManager manager = (ProductionManager)angajat;

        Machine masina = GasesteMasina(serialMasina);
        if (masina == null)
        {
            Console.WriteLine(Messages.MachineDoesNotExist);
            return;
        }

        string idComanda = Messages.OrderIdPrefix + _idComandaCounter;
        _idComandaCounter++;

        ProductionOrder comanda = manager.CreazaComanda(idComanda, masina, produs, cantitate, prioritate);
        _orderRepository.Add(comanda);
    }

    public void ExecutaComanda(string idOperator, string idComanda, int unitati)
    {
        Employee angajat = GasesteAngajat(idOperator);
        if (angajat == null)
        {
            Console.WriteLine(Messages.EmployeeDoesNotExist);
            return;
        }

        if (!(angajat is MachineOperator))
        {
            Console.WriteLine(Messages.NotMachineOperator(angajat.Nume));
            return;
        }

        MachineOperator op = (MachineOperator)angajat;

        ProductionOrder comanda = _orderRepository.FindById(idComanda);

        if (comanda == null)
        {
            Console.WriteLine(Messages.OrderDoesNotExist);
            return;
        }

        op.Opereaza(comanda.Masina);

        if (comanda.Masina.Status == MachineStatus.Running)
        {
            comanda.InregistreazaProductie(unitati);
            Logging.Log(idOperator, Messages.ProductionLogged(unitati, idComanda, comanda.NumeProdus));
        }
    }

    public void ReparaMasina(string idTehnician, string idEngineer, string serial)
    {
        Employee a1 = GasesteAngajat(idTehnician);
        Employee a2 = GasesteAngajat(idEngineer);

        if (a1 == null || a2 == null)
        {
            Console.WriteLine(Messages.EmployeesDoNotExist);
            return;
        }

        if (a1 is not Technician)
        {
            Console.WriteLine(Messages.NotTechnician(a1.Nume));
            return;
        }

        if (a2 is not Engineer)
        {
            Console.WriteLine(Messages.NotEngineer(a2.Nume));
            return;
        }

        Technician teh = (Technician)a1;
        Engineer eng = (Engineer)a2;

        Machine masina = GasesteMasina(serial);
        if (masina == null)
        {
            Console.WriteLine(Messages.MachineDoesNotExist);
            return;
        }

        if (masina.Status == MachineStatus.Running)
        {
            Console.WriteLine(Messages.StopMachineBeforeRepair);
            return;
        }

        eng.Inspecteaza(masina);
        teh.Repara(masina);
    }

    public void AdaugaStocProduse(string numeProdus, int cantitate)
    {
        Product produs = GasesteProdus(numeProdus);
        if (produs == null)
        {
            Console.WriteLine(Messages.ProductDoesNotExistForStock);
            return;
        }
        produs.AdaugaStoc(cantitate);
        Console.WriteLine(Messages.StockAdded(numeProdus, cantitate));
    }

    public void VindeProdus(string idAgent, string numeProdus, int cantitate)
    {
        Employee angajat = GasesteAngajat(idAgent);
        if (angajat == null)
        {
            Console.WriteLine(Messages.EmployeeDoesNotExist);
            return;
        }

        if (!(angajat is SalesAgent))
        {
            Console.WriteLine(Messages.NotSalesAgent(angajat.Nume));
            return;
        }

        SalesAgent agent = (SalesAgent)angajat;

        Product produs = GasesteProdus(numeProdus);
        if (produs == null)
        {
            Console.WriteLine(Messages.ProductDoesNotExist);
            return;
        }

        agent.VindeProdus(produs, cantitate, this);
    }

    

    public void AfiseazaRaportGeneral()
    {
        Console.WriteLine(Messages.FactoryReport(Nume, _employeeRepository.Count, _machineRepository.Count,
            _productRepository.Count, _orderRepository.Count, _totalRevenue, _totalSalesQuantity,
            GetMachinesRequiringMaintenance(7).Count, GetLowStockProducts().Count));
    }

    

    public void RecordSale(string productName, int quantity, decimal unitPrice)
    {
        decimal saleAmount = quantity * unitPrice;
        _totalRevenue += saleAmount;
        _totalSalesQuantity += quantity;

        Product p = GasesteProdus(productName);
        if (p != null)
        {
            p.VindeStoc(quantity);
            Console.WriteLine(Messages.SaleRecorded(quantity, productName, saleAmount));
            DisplayInventoryAlert(p);
        }
    }

    public decimal GetTotalRevenue()
    {
        return _totalRevenue;
    }

    public int GetTotalSalesQuantity()
    {
        return _totalSalesQuantity;
    }

    public decimal CalculateProfit()
    {
        decimal totalCost = _productRepository
            .GetAll()
            .Sum(product => product.ProductionCost * (1000 - product.Cantitate));

        return _totalRevenue - totalCost;
    }

    public List<Machine> GetMachinesRequiringMaintenance(int daysAhead = 7)
    {
        return _machineRepository
            .GetAll()
            .Where(machine => machine.EstimateDaysUntilMaintenance() <= daysAhead)
            .ToList();
    }

    public void AfiseazaMentenantaPredictiva(int daysAhead = 7)
    {
        List<Machine> machines = GetMachinesRequiringMaintenance(daysAhead);
        Console.WriteLine(Messages.PredictiveMaintenanceTitle);

        if (machines.Count == 0)
        {
            Console.WriteLine(Messages.NoMaintenanceRequired(daysAhead));
            return;
        }

        machines.ForEach(machine => Console.WriteLine(
            Messages.MaintenanceDue(machine.SerialNumber, machine.Nume, machine.EstimateDaysUntilMaintenance())));
    }

    public void AfiseazaDashboardEficienta() 
    {
        List<Machine> machines = _machineRepository.GetAll();
        Console.WriteLine(Messages.EfficiencyTitle);

        if (machines.Count == 0)
        {
            Console.WriteLine(Messages.NoMachines);
            return;
        }

        machines.ForEach(machine => Console.WriteLine(
            Messages.MachineEfficiency(machine.SerialNumber, machine.Nume, machine.CalculateEfficiencyPercentage(), machine.ProductionCycles)));
        Console.WriteLine(Messages.AverageEfficiency(machines.Average(machine => machine.CalculateEfficiencyPercentage())));
    }

    public void AfiseazaStareMasini()
    {
        List<Machine> machines = _machineRepository.GetAll();
        Console.WriteLine(Messages.HealthMonitoringTitle);

        if (machines.Count == 0)
        {
            Console.WriteLine(Messages.NoMachines);
            return;
        }

        machines.ForEach(machine => Console.WriteLine(
            Messages.MachineHealth(machine.SerialNumber, machine.Nume, machine.Conditie, machine.GetHealthAlert())));
    }

    public List<Product> GetLowStockProducts(int threshold = 5)
    {
        return _productRepository
            .GetAll()
            .Where(product => product.Cantitate <= threshold)
            .ToList();
    }

    public void AfiseazaAlerteInventar(int threshold = 5)
    {
        Console.WriteLine(Messages.InventoryAlertsTitle);
        List<Product> products = GetLowStockProducts(threshold);

        if (products.Count == 0)
        {
            Console.WriteLine(Messages.AllProductsAboveThreshold(threshold));
            return;
        }

        products.ForEach(product => DisplayInventoryAlert(product, threshold));
    }

    private static void DisplayInventoryAlert(Product product, int threshold = 5)
    {
        if (product.Cantitate <= threshold)
            Console.WriteLine(Messages.LowStock(product.Nume, product.Cantitate, threshold));
    }

    public void AfiseazaRaportVanzari()
    {
        Console.WriteLine(Messages.SalesReport(Nume, _totalRevenue, _totalSalesQuantity, CalculateProfit()));
    }

    public void AfiseazaComenzi()
    {
        _orderRepository.DisplayAll();
    }

    public void AfiseazaComenziSortedByPriority()
    {
        List<ProductionOrder> comenziSortate = _orderRepository.GetSortedByPriority();

        if (comenziSortate.Count == 0)
        {
            Console.WriteLine(Messages.NoOrders);
            return;
        }

        Console.WriteLine(Messages.OrdersSortedByPriority);
        foreach (var comanda in comenziSortate)
        {
            comanda.Afiseaza();
        }
    }

    public ProductionOrder GetNextPriorityOrder(string idOperator)
    {
        Employee angajat = GasesteAngajat(idOperator);
        if (angajat == null)
            return null;

        if (!(angajat is MachineOperator))
            return null;

        return _orderRepository.GetNextByPriority();
    }
}
