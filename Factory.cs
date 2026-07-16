using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
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

    // File where orders are persisted. Search for orders.txt in app base dir and up to 4 parent folders.
    private string OrdersFilePath
    {
        get
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string candidate = Path.Combine(baseDir, "orders.txt");
            if (File.Exists(candidate)) return candidate;

            var dir = new DirectoryInfo(baseDir);
            for (int i = 0; i < 5 && dir != null; i++)
            {
                candidate = Path.Combine(dir.FullName, "orders.txt");
                if (File.Exists(candidate)) return candidate;
                dir = dir.Parent;
            }

            // fallback to baseDir path (file may be created there)
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "orders.txt");
        }
    }

    // Load orders from orders.txt. Expects lines in the format:
    // Id;MachineSerial;ProductName;Quantity;Priority;Status;CreatedBy;CreatedAt
    public void LoadOrdersFromFile()
    {
        try
        {
            if (!File.Exists(OrdersFilePath))
                return;

            string[] lines = File.ReadAllLines(OrdersFilePath);
            int maxIdSeen = 0;
            foreach (var raw in lines)
            {
                var line = raw.Trim();
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                    continue;

                var parts = line.Split(';');
                if (parts.Length < 8)
                {
                    Console.WriteLine($"Warning: invalid order line: {line}");
                    continue;
                }

                string id = parts[0].Trim();
                string machineSerial = parts[1].Trim();
                string productName = parts[2].Trim();
                if (!int.TryParse(parts[3].Trim(), out int qty))
                    qty = 0;

                if (!Enum.TryParse(parts[4].Trim(), true, out Priority prioritate))
                    prioritate = Priority.Medium;

                if (!Enum.TryParse(parts[5].Trim(), true, out ProductionOrderStatus status))
                    status = ProductionOrderStatus.Created;

                string createdBy = parts[6].Trim();
                DateTime createdAt = DateTime.Now;
                DateTime.TryParse(parts[7].Trim(), out createdAt);

                Machine masina = GasesteMasina(machineSerial);
                Employee emp = GasesteAngajat(createdBy);
                ProductionManager manager = emp as ProductionManager;

                if (masina == null || manager == null)
                {
                    // can't construct a valid order without machine and manager; skip
                    Console.WriteLine($"Skipping order {id}: missing machine or manager");
                    continue;
                }

                // if order already exists, update its properties, otherwise create new
                var existing = _orderRepository.FindById(id);
                if (existing != null)
                {
                    existing.Masina = masina;
                    existing.NumeProdus = productName;
                    existing.CantitateTarget = qty;
                    existing.Prioritate = prioritate;
                    existing.Status = status;
                    existing.DataCrearii = createdAt;
                }
                else
                {
                    var order = new ProductionOrder(id, masina, manager, productName, qty, prioritate);
                    order.Status = status;
                    order.CantitateProdusa = 0; // we don't persist produced amount in file currently
                    order.DataCrearii = createdAt;

                    _orderRepository.Add(order);
                }
                // track numeric suffix of ORD... ids so we can continue numbering
                if (id.StartsWith("ORD", StringComparison.OrdinalIgnoreCase))
                {
                    string numPart = id.Substring(3);
                    if (int.TryParse(numPart, out int parsed))
                    {
                        if (parsed > maxIdSeen) maxIdSeen = parsed;
                    }
                }
            }

            // ensure next generated id is higher than any existing one
            if (maxIdSeen > 0)
            {
                _idComandaCounter = maxIdSeen + 1;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load orders: {ex.Message}");
        }
    }

    // Persist all orders to orders.txt (overwrites file)
    public void SaveOrdersToFile()
    {
        try
        {
            var orders = _orderRepository.GetAll();
            List<string> lines = new List<string>();
            lines.Add("# Production Orders");
            lines.Add("# Format: Id;MachineSerial;ProductName;Quantity;Priority;Status;CreatedBy;CreatedAt");
            foreach (var o in orders)
            {
                string createdBy = o.CreatDe != null ? o.CreatDe.Id : "";
                string createdAt = o.DataCrearii.ToString("s");
                string line = string.Join(";", o.Id, o.Masina?.SerialNumber ?? "", o.NumeProdus, o.CantitateTarget.ToString(), o.Prioritate.ToString(), o.Status.ToString(), createdBy, createdAt);
                lines.Add(line);
            }

            File.WriteAllLines(OrdersFilePath, lines);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to save orders: {ex.Message}");
        }
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
        // Create order with priority, add to repository, log and persist
        ProductionOrder comanda = manager.CreazaComanda(idComanda, masina, produs, cantitate, prioritate);
        _orderRepository.Add(comanda);
        Logging.Log(idManager, $"Order created: {idComanda} ({produs}) qty={cantitate} priority={prioritate}");
        SaveOrdersToFile();
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
        // reload orders to make sure we consider persisted orders
        LoadOrdersFromFile();

        Employee angajat = GasesteAngajat(idOperator);
        if (angajat == null)
            return null;

        if (!(angajat is MachineOperator))
            return null;

        return _orderRepository.GetNextByPriority();
    }
}
