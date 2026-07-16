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
            Logging.Log(angajat.Id, $"Employee added: {angajat.Nume}");
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
            Console.WriteLine("Employee successfully deleted!");
            Logging.Log(id, $"Employee removed: {id}");
            return true;
        }
        else
        {
            Console.WriteLine("The employee doesn't exist!");
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
            Console.WriteLine("The employee doesn't exist!");
            return;
        }

        if (!(angajat is ProductionManager))
        {
            Console.WriteLine(angajat.Nume + " is not ProductionManager!");
            return;
        }

        ProductionManager manager = (ProductionManager)angajat;

        Machine masina = GasesteMasina(serialMasina);
        if (masina == null)
        {
            Console.WriteLine("Machine doesn't exist!");
            return;
        }

        string idComanda = "ORD" + _idComandaCounter;
        _idComandaCounter++;

        ProductionOrder comanda = manager.CreazaComanda(idComanda, masina, produs, cantitate, prioritate);
        _orderRepository.Add(comanda);
    }

    public void ExecutaComanda(string idOperator, string idComanda, int unitati)
    {
        Employee angajat = GasesteAngajat(idOperator);
        if (angajat == null)
        {
            Console.WriteLine("Employee doesn't exist!");
            return;
        }

        if (!(angajat is MachineOperator))
        {
            Console.WriteLine(angajat.Nume + " is not MachineOperator!");
            return;
        }

        MachineOperator op = (MachineOperator)angajat;

        ProductionOrder comanda = _orderRepository.FindById(idComanda);

        if (comanda == null)
        {
            Console.WriteLine("Order doesn't exist!");
            return;
        }

        op.Opereaza(comanda.Masina);

        if (comanda.Masina.Status == MachineStatus.Running)
        {
            comanda.InregistreazaProductie(unitati);
            Logging.Log(idOperator, $"Produced {unitati} units for order {idComanda} ({comanda.NumeProdus})");
        }
    }

    public void ReparaMasina(string idTehnician, string idEngineer, string serial)
    {
        Employee a1 = GasesteAngajat(idTehnician);
        Employee a2 = GasesteAngajat(idEngineer);

        if (a1 == null || a2 == null)
        {
            Console.WriteLine("One of the employees doesn't exist!");
            return;
        }

        if (a1 is not Technician)
        {
            Console.WriteLine(a1.Nume + " is not a Technician!");
            return;
        }

        if (a2 is not Engineer)
        {
            Console.WriteLine(a2.Nume + " is not a Engineer!");
            return;
        }

        Technician teh = (Technician)a1;
        Engineer eng = (Engineer)a2;

        Machine masina = GasesteMasina(serial);
        if (masina == null)
        {
            Console.WriteLine("Machine doesn't exist!");
            return;
        }

        if (masina.Status == MachineStatus.Running)
        {
            Console.WriteLine("Stop the car before the repair!");
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
            Console.WriteLine("There is no such product");
            return;
        }
        produs.AdaugaStoc(cantitate);
        Console.WriteLine($"New stock added: {numeProdus} + {cantitate} pieces");
    }

    public void VindeProdus(string idAgent, string numeProdus, int cantitate)
    {
        Employee angajat = GasesteAngajat(idAgent);
        if (angajat == null)
        {
            Console.WriteLine("Employee doesn't exist!");
            return;
        }

        if (!(angajat is SalesAgent))
        {
            Console.WriteLine(angajat.Nume + " is not a SalesAgent!");
            return;
        }

        SalesAgent agent = (SalesAgent)angajat;

        Product produs = GasesteProdus(numeProdus);
        if (produs == null)
        {
            Console.WriteLine("Product doesn't exist!");
            return;
        }

        agent.VindeProdus(produs, cantitate, this);
    }

    

    public void AfiseazaRaportGeneral()
    {
        Console.WriteLine("\n=== REPORT: " + Nume + " ===");
        Console.WriteLine("Employees: " + _employeeRepository.Count);
        Console.WriteLine("Machines:   " + _machineRepository.Count);
        Console.WriteLine("Products:  " + _productRepository.Count);
        Console.WriteLine("Orders:  " + _orderRepository.Count);
        Console.WriteLine("Total Revenue: " + _totalRevenue + " RON");
        Console.WriteLine("Total Units Sold: " + _totalSalesQuantity);
        Console.WriteLine("Machines requiring maintenance: " + GetMachinesRequiringMaintenance(7).Count);
        Console.WriteLine("Products below stock threshold: " + GetLowStockProducts().Count);
        Console.WriteLine("");
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
            Console.WriteLine("Sale recorded: " + quantity + "x " + productName + " = " + saleAmount + " RON");
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
    public void IncarcaMasini()
    {
        _machineRepository.LoadMachines();
    }

    public void IncarcaProduse()
    {
        _productRepository.LoadProducts();
    }

    public void SalveazaMasini()
    {
        _machineRepository.SaveAllMachines();
    }

    public void SalveazaProduse()
    {
        _productRepository.SaveAllProducts();
    }

    public void AfiseazaMentenantaPredictiva(int daysAhead = 7)
    {
        List<Machine> machines = GetMachinesRequiringMaintenance(daysAhead);
        Console.WriteLine("\n=== PREDICTIVE MAINTENANCE ===");

        if (machines.Count == 0)
        {
            Console.WriteLine($"No machines require maintenance in the next {daysAhead} days.");
            return;
        }

        machines.ForEach(machine => Console.WriteLine(
            $"{machine.SerialNumber} - {machine.Nume}: maintenance due in {machine.EstimateDaysUntilMaintenance()} day(s)."));
    }

    public void AfiseazaDashboardEficienta() 
    {
        List<Machine> machines = _machineRepository.GetAll();
        Console.WriteLine("\n=== PRODUCTION EFFICIENCY DASHBOARD ===");

        if (machines.Count == 0)
        {
            Console.WriteLine("There are no machines!");
            return;
        }

        machines.ForEach(machine => Console.WriteLine(
            $"{machine.SerialNumber} - {machine.Nume}: {machine.CalculateEfficiencyPercentage():F2}% efficiency, {machine.ProductionCycles} production cycle(s)."));
        Console.WriteLine($"Average efficiency: {machines.Average(machine => machine.CalculateEfficiencyPercentage()):F2}%");
    }

    public void AfiseazaStareMasini()
    {
        List<Machine> machines = _machineRepository.GetAll();
        Console.WriteLine("\n=== MACHINE HEALTH MONITORING ===");

        if (machines.Count == 0)
        {
            Console.WriteLine("There are no machines!");
            return;
        }

        machines.ForEach(machine => Console.WriteLine(
            $"{machine.SerialNumber} - {machine.Nume} | {machine.Conditie} | {machine.GetHealthAlert()}"));
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
        Console.WriteLine("\n=== INVENTORY ALERTS ===");
        List<Product> products = GetLowStockProducts(threshold);

        if (products.Count == 0)
        {
            Console.WriteLine($"All products are above the stock threshold of {threshold}.");
            return;
        }

        products.ForEach(product => DisplayInventoryAlert(product, threshold));
    }

    private static void DisplayInventoryAlert(Product product, int threshold = 5)
    {
        if (product.Cantitate <= threshold)
            Console.WriteLine($"ALERT: {product.Nume} stock is low ({product.Cantitate} remaining; threshold: {threshold}).");
    }

    public void AfiseazaRaportVanzari()
    {
        Console.WriteLine("\n=== SALES REPORT: " + Nume + " ===");
        Console.WriteLine("Total Revenue: " + _totalRevenue + " RON");
        Console.WriteLine("Total Units Sold: " + _totalSalesQuantity);
        Console.WriteLine("Average Price Per Unit: " + (_totalSalesQuantity > 0 ? (_totalRevenue / _totalSalesQuantity).ToString("F2") : "N/A") + " RON");
        Console.WriteLine("Estimated Profit: " + CalculateProfit() + " RON");
        Console.WriteLine("");
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
            Console.WriteLine("There are no orders!");
            return;
        }

        Console.WriteLine("=== Orders sorted by priority ===");
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
