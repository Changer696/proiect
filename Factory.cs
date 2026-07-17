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
    private static readonly Random _random = new Random();
    private bool _companyPublic = false;
    private decimal _companyPublicPercentage = 0;
    private int _companyShares = 0;
    private decimal _sharePrice = 0;

    public bool IsCompanyPublic => _companyPublic;
    public decimal CompanyPublicPercentage => _companyPublicPercentage;
    public int CompanyShares => _companyShares;
    public decimal SharePrice => _sharePrice;

    private string _ordersFileName;

    public Factory(string nume, string ordersFileName)
    {
        Nume = nume;
        _ordersFileName = ordersFileName;
    }

    // File where orders are persisted. Search for the requested file name in app base dir and up to 4 parent folders.
    private string OrdersFilePath
    {
        get
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string candidate = Path.Combine(baseDir, _ordersFileName);
            if (File.Exists(candidate)) return candidate;

            var dir = new DirectoryInfo(baseDir);
            for (int i = 0; i < 5 && dir != null; i++)
            {
                candidate = Path.Combine(dir.FullName, _ordersFileName);
                if (File.Exists(candidate)) return candidate;
                dir = dir.Parent;
            }

            // fallback to baseDir path (file may be created there)
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _ordersFileName);
        }
    }

    // Load orders from orders.txt. Expects lines in the format:
    // Id;MachineSerial;ProductName;Quantity;Priority;Status;CreatedBy;CreatedAt
    public void LoadOrdersFromFile(string ordersFileName = null)
    {
        if (!string.IsNullOrWhiteSpace(ordersFileName))
        {
            _ordersFileName = ordersFileName;
        }

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
                    Console.WriteLine(Messages.WarningInvalidLine(line));
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
                    Console.WriteLine(Messages.OrderLoadSkipped(id));
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
            Console.WriteLine(Messages.OrderLoadFailed(ex));
        }
    }

    // Persist all orders to orders.txt (overwrites file)
    public void SaveOrdersToFile(string ordersFileName = null)
    {
        if (!string.IsNullOrWhiteSpace(ordersFileName))
        {
            _ordersFileName = ordersFileName;
        }

        try
        {
            var orders = _orderRepository.GetAll();
            var lines = new List<string>
            {
                "# Production Orders",
                "# Format: Id;MachineSerial;ProductName;Quantity;Priority;Status;CreatedBy;CreatedAt"
            };

            lines.AddRange(orders.Select(o => string.Join(";",
                o.Id,
                o.Masina?.SerialNumber ?? "",
                o.NumeProdus,
                o.CantitateTarget.ToString(),
                o.Prioritate.ToString(),
                o.Status.ToString(),
                (o.CreatDe != null ? o.CreatDe.Id : ""),
                o.DataCrearii.ToString("s")
            )));

            File.WriteAllLines(OrdersFilePath, lines);
        }
        catch (Exception ex)
        {
            Console.WriteLine(Messages.OrderSaveFailed(ex));
        }
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
            Console.WriteLine(Messages.EmployeeDeletedSuccessfully);
            AplicaScaderePreturiAngajatEliminat();
            Logging.Log(id, $"Employee removed: {id}");
            return true;
        }
        else
        {
            Console.WriteLine(Messages.EmployeeDoesNotExistGeneric);
            return false;
        }
    }

    public void AplicaFluctuatiePreturiMeniu()
    {
        decimal schimbare = (decimal)(_random.Next(-200, 201)) / 100m;
        ModificaPreturi(schimbare);
        Console.WriteLine(Messages.MarketUpdate(schimbare));
    }

    public void AplicaCresterePreturiVanzare(Product produs)
    {
        if (produs == null)
        {
            return;
        }

        decimal schimbare = (decimal)(_random.Next(500, 1001)) / 100m;
        ModificaPreturi(schimbare, produs);
        Console.WriteLine(Messages.SaleImpact(produs.Nume, schimbare));
    }

    public void AplicaScaderePreturiAngajatEliminat()
    {
        ModificaPreturi(-5m);
        Console.WriteLine(Messages.EmployeeRemovalImpact);
    }

    private void ModificaPreturi(decimal procent, Product produsSpecific = null)
    {
        var produse = produsSpecific == null
            ? _productRepository.GetAll()
            : new List<Product> { produsSpecific };

            produse.Where(produs => produs != null)
                   .ToList()
                   .ForEach(produs =>
                   {
                       decimal noulPret = produs.SellingPrice * (1 + procent / 100m);
                       produs.SellingPrice = Math.Round(noulPret, 2);
                   });
    }

    public void AfiseazaPreturiStoc()
    {
        var produse = _productRepository.GetAll();
            if (!produse.Any())
        {
            Console.WriteLine(Messages.NoProductsAvailable);
            return;
        }

        Console.WriteLine(Messages.StockPricesTitle);
        produse.ForEach(produs => Console.WriteLine(Messages.StockPriceLine(produs.Nume, produs.SellingPrice, produs.Cantitate)));
        Console.WriteLine(string.Empty);
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
        return _machineRepository.Find(m => m.SerialNumber == serial);
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
        return _productRepository.Find(p => p.Nume == nume);
    }

    

    public void CreazaComanda(string idManager, string serialMasina,
                               string produs, int cantitate, Priority prioritate)
    {
        Employee angajat = GasesteAngajat(idManager);
        if (angajat == null)
        {
            Console.WriteLine(Messages.EmployeeDoesNotExistGeneric);
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
            Console.WriteLine(Messages.MachineDoesNotExist);
            return;
        }

        string idComanda = "ORD" + _idComandaCounter;
        _idComandaCounter++;
        // Create order with priority, add to repository, log and persist
        ProductionOrder comanda = manager.CreazaComanda(idComanda, masina, produs, cantitate, prioritate);
        _orderRepository.Add(comanda);
        Logging.Log(idManager, $"Order created: {idComanda} ({produs}) qty={cantitate} priority={prioritate}");
        SaveOrdersToFile(_ordersFileName);
    }

    public void ExecutaComanda(string idOperator, string idComanda, int unitati)
    {
        Employee angajat = GasesteAngajat(idOperator);
        if (angajat == null)
        {
            Console.WriteLine(Messages.EmployeeDoesNotExistGeneric);
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
            Console.WriteLine(Messages.OrderDoesNotExist);
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
            Console.WriteLine(Messages.EmployeeDoesNotExistGeneric);
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
            Console.WriteLine(Messages.MachineDoesNotExist);
            return;
        }

        if (masina.Status == MachineStatus.Running)
        {
            Console.WriteLine(Messages.MachineStopBeforeRepair);
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
            Console.WriteLine(Messages.ProductNotFoundGeneric);
            return;
        }
        produs.AdaugaStoc(cantitate);
        Console.WriteLine(Messages.NewStockAdded(numeProdus, cantitate));
    }

    public void VindeProdus(string idAgent, string numeProdus, int cantitate)
    {
        Employee angajat = GasesteAngajat(idAgent);
        if (angajat == null)
        {
            Console.WriteLine(Messages.EmployeeDoesNotExistGeneric);
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
            Console.WriteLine(Messages.ProductNotFoundGeneric);
            return;
        }

        agent.VindeProdus(produs, cantitate, this);
    }

    

    public bool MakeCompanyPublic(string directorId, decimal percentagePublic, int shares, decimal sharePrice)
    {
        Employee angajat = GasesteAngajat(directorId);
        if (angajat == null)
        {
            Console.WriteLine(Messages.EmployeeDoesNotExistGeneric);
            return false;
        }

        if (angajat.Rol != EmployeeRole.Director)
        {
            Console.WriteLine(angajat.Nume + " is not the Director!");
            return false;
        }

        if (percentagePublic <= 0 || percentagePublic > 100)
        {
            Console.WriteLine(Messages.InvalidPublicPercentage);
            return false;
        }

        if (shares <= 0)
        {
            Console.WriteLine(Messages.InvalidShareCountMessage);
            return false;
        }

        if (sharePrice <= 0)
        {
            Console.WriteLine(Messages.InvalidSharePriceMessage);
            return false;
        }

        _companyPublic = true;
        _companyPublicPercentage = percentagePublic;
        _companyShares = shares;
        _sharePrice = sharePrice;

        Console.WriteLine(Messages.CompanyPublicAnnouncement(percentagePublic, shares, sharePrice));
        Logging.Log(directorId, $"Company made public: {percentagePublic}% / {shares} shares / {sharePrice} RON");
        return true;
    }

    public void AfiseazaRaportGeneral()
    {
        Console.WriteLine(Messages.ReportTitle(Nume));
        Console.WriteLine(Messages.FactoryReportLine("Employees", _employeeRepository.Count));
        Console.WriteLine(Messages.FactoryReportLine("Machines", _machineRepository.Count));
        Console.WriteLine(Messages.FactoryReportLine("Products", _productRepository.Count));
        Console.WriteLine(Messages.FactoryReportLine("Orders", _orderRepository.Count));
        Console.WriteLine(Messages.FactoryReportLine("Total Revenue", _totalRevenue + " RON"));
        Console.WriteLine(Messages.FactoryReportLine("Total Units Sold", _totalSalesQuantity));
        Console.WriteLine(Messages.FactoryReportLine("Company Status", _companyPublic ? "Public" : "Private"));
        if (_companyPublic)
        {
            Console.WriteLine(Messages.FactoryReportLine("Public percentage", _companyPublicPercentage + "%"));
            Console.WriteLine(Messages.FactoryReportLine("Shares issued", _companyShares));
            Console.WriteLine(Messages.FactoryReportLine("Share price", _sharePrice + " RON"));
        }
        Console.WriteLine(Messages.FactoryReportLine("Machines requiring maintenance", GetMachinesRequiringMaintenance(7).Count));
        Console.WriteLine(Messages.FactoryReportLine("Products below stock threshold", GetLowStockProducts().Count));
        Console.WriteLine(string.Empty);
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
            AplicaCresterePreturiVanzare(p);
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
    public void IncarcaMasini(string machinesFileName)
    {
        _machineRepository.LoadMachines(machinesFileName);
    }

    public void IncarcaProduse(string productsFileName)
    {
        _productRepository.LoadProducts(productsFileName);
    }

    public void SalveazaMasini(string machinesFileName)
    {
        _machineRepository.SaveAllMachines(machinesFileName);
    }

    public void SalveazaProduse(string productsFileName)
    {
        _productRepository.SaveAllProducts(productsFileName);
    }

    public void AfiseazaMentenantaPredictiva(int daysAhead = 7)
    {
        List<Machine> machines = GetMachinesRequiringMaintenance(daysAhead);
        Console.WriteLine(Messages.PredictiveMaintenanceTitle);

        if (!machines.Any())
        {
            Console.WriteLine(Messages.NoMaintenanceInNextDays(daysAhead));
            return;
        }

        machines.ForEach(machine => Console.WriteLine(
            $"{machine.SerialNumber} - {machine.Nume}: maintenance due in {machine.EstimateDaysUntilMaintenance()} day(s)."));
    }

    public void AfiseazaDashboardEficienta() 
    {
        List<Machine> machines = _machineRepository.GetAll();
        Console.WriteLine(Messages.ProductionEfficiencyDashboardTitle);

        if (!machines.Any())
        {
            Console.WriteLine(Messages.NoMachinesMessage);
            return;
        }

        machines.ForEach(machine => Console.WriteLine(
            $"{machine.SerialNumber} - {machine.Nume}: {machine.CalculateEfficiencyPercentage():F2}% efficiency, {machine.ProductionCycles} production cycle(s)."));
        Console.WriteLine(Messages.AverageEfficiency(machines.Average(machine => machine.CalculateEfficiencyPercentage())));
    }

    public void AfiseazaStareMasini()
    {
        List<Machine> machines = _machineRepository.GetAll();
        Console.WriteLine(Messages.MachineHealthMonitoringTitle);

        if (!machines.Any())
        {
            Console.WriteLine(Messages.NoMachinesMessage);
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
        Console.WriteLine(Messages.InventoryAlertsTitle);
        List<Product> products = GetLowStockProducts(threshold);

        if (!products.Any())
        {
            Console.WriteLine(Messages.InventoryThresholdAlert(threshold));
            return;
        }

        products.ForEach(product => DisplayInventoryAlert(product, threshold));
    }

    private static void DisplayInventoryAlert(Product product, int threshold = 5)
    {
        if (product.Cantitate <= threshold)
            Console.WriteLine(Messages.LowStockAlert(product.Nume, product.Cantitate, threshold));
    }

    public void AfiseazaRaportVanzari()
    {
        Console.WriteLine(Messages.SalesReportTitle(Nume));
        Console.WriteLine(Messages.FactoryReportLine("Total Revenue", _totalRevenue + " RON"));
        Console.WriteLine(Messages.FactoryReportLine("Total Units Sold", _totalSalesQuantity));
        Console.WriteLine(Messages.FactoryReportLine("Average Price Per Unit", (_totalSalesQuantity > 0 ? (_totalRevenue / _totalSalesQuantity).ToString("F2") : "N/A") + " RON"));
        Console.WriteLine(Messages.FactoryReportLine("Estimated Profit", CalculateProfit() + " RON"));
        Console.WriteLine(string.Empty);
    }

    public void AfiseazaComenzi()
    {
        _orderRepository.DisplayAll();
    }

    public void AfiseazaComenziSortedByPriority()
    {
        List<ProductionOrder> comenziSortate = _orderRepository.GetSortedByPriority();

        if (!comenziSortate.Any())
        {
            Console.WriteLine(Messages.NoOrdersAvailable);
            return;
        }

        Console.WriteLine(Messages.OrdersSortedByPriorityTitle);
        foreach (var comanda in comenziSortate)
        {
            comanda.Afiseaza();
        }
    }

    public ProductionOrder GetNextPriorityOrder(string idOperator)
    {
        // reload orders to make sure we consider persisted orders
        LoadOrdersFromFile(_ordersFileName);

        Employee angajat = GasesteAngajat(idOperator);
        if (angajat == null)
            return null;

        if (!(angajat is MachineOperator))
            return null;

        return _orderRepository.GetNextByPriority();
    }
}
