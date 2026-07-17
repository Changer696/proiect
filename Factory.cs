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
    private decimal _lastShareChange = 0;

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

    

    // Adds an employee to the repository and logs the addition.
    public bool AdaugaAngajat(Employee angajat)
    {
        bool added = _employeeRepository.Add(angajat);
        if (added)
        {
            Logging.Log(angajat.Id, $"Employee added: {angajat.Nume}");
        }
        return added;
    }

    // Checks whether an employee with the given id exists.
    public bool EmployeeIdExists(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return false;

        return _employeeRepository.ExistsById(id);
    }

    // Displays all employees stored in the repository.
    public void AfiseazaAngajati()
    {
        _employeeRepository.DisplayAll();
    }

    // Finds and returns an employee by id, or null if not found.
    public Employee GasesteAngajat(string id)
    {
        return _employeeRepository.FindById(id);
    }

    // Removes an employee by id, applies price decrease and logs the removal.
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

    // Applies a market-wide random price fluctuation if the company is public.
    public void AplicaFluctuatiePreturiMeniu()
    {
        // Only apply and show market-wide fluctuations when the company is public
        if (!_companyPublic)
        {
            return;
        }

        decimal schimbare = (decimal)(_random.Next(-200, 201)) / 100m;
        ModificaPreturi(schimbare);
        Console.WriteLine(Messages.MarketUpdate(schimbare));
    }

    // Applies a price increase to products (and share price when public) after a sale.
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

    // Applies a fixed price decrease when an employee is removed.
    public void AplicaScaderePreturiAngajatEliminat()
    {
        ModificaPreturi(-5m);
        Console.WriteLine(Messages.EmployeeRemovalImpact);
    }

    // Modifies product prices by a percentage; optionally targets a specific product and updates share price when public.
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

        // if the company is public, adjust share price by the same percentage and record the change
        if (_companyPublic)
        {
            decimal newSharePrice = Math.Round(_sharePrice * (1 + procent / 100m), 2);
            _lastShareChange = procent;
            _sharePrice = newSharePrice;
        }
    }

    // Displays stock-related information; when company is public shows share price and last change.
    public void AfiseazaPreturiStoc()
    {
        var produse = _productRepository.GetAll();
        if (!produse.Any())
        {
            Console.WriteLine(Messages.NoProductsAvailable);
            return;
        }

        // Show share price and last change instead of individual product stock prices
        Console.WriteLine(Messages.SharePriceTitle);
        if (_companyPublic)
        {
            Console.WriteLine(Messages.SharePriceLine(_sharePrice, _lastShareChange));
        }
        else
        {
            Console.WriteLine(Messages.CompanyNotPublic);
        }
        Console.WriteLine(string.Empty);
    }

    // Adds a machine to the machine repository.
    public bool AdaugaMasina(Machine masina)
    {
        return _machineRepository.Add(masina);
    }

    // Displays all machines in the repository.
    public void AfiseazaMasini()
    {
        _machineRepository.DisplayAll();
    }

    // Finds and returns a machine by serial number, or null if not found.
    public Machine GasesteMasina(string serial)
    {
        return _machineRepository.Find(m => m.SerialNumber == serial);
    }



    // Adds a product to the product repository.
    public bool AdaugaProdus(Product produs)
    {
        _productRepository.Add(produs);
        return true;
    }

    // Displays all products in the repository.
    public void AfiseazaProduse()
    {
        _productRepository.DisplayAll();
    }

    // Finds and returns a product by name, or null if not found.
    public Product GasesteProdus(string nume)
    {
        return _productRepository.Find(p => p.Nume == nume);
    }

    

    // Creates a production order by a production manager and persists it.
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

    // Executes a production order by a machine operator and records produced units.
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

    // Coordinates a technician and an engineer to inspect and repair a machine.
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

    // Adds stock quantity to a named product and reports the update.
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

    // Processes a sale initiated by a sales agent for a product and quantity.
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

    

    // Makes the company public with the given percentage, shares and initial share price.
    public bool MakeCompanyPublic(string directorId, decimal percentagePublic, int shares, decimal sharePrice)
    {
        if (_companyPublic)
        {
            Console.WriteLine(Messages.CompanyAlreadyPublicMessage);
            return false;
        }
        // The caller (UI) should ensure the caller is the Director; avoid requiring the Employee object
        // to exist in the in-memory repository because credentials and employee records may be stored separately.

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

    // Displays a general report about the factory (counts, revenue, status, alerts).
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

    // Returns the total inventory value. By default uses selling price; set useSellingPrice=false to use production cost.
    public decimal GetTotalInventoryValue(bool useSellingPrice = true)
    {
        var produse = _productRepository.GetAll();
        if (!produse.Any()) return 0m;

        if (useSellingPrice)
            return produse.Sum(p => p.SellingPrice * p.Cantitate);
        else
            return produse.Sum(p => p.ProductionCost * p.Cantitate);
    }

    

    // Records a sale: updates revenue, reduces stock, applies price changes and shows alerts.
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

    // Returns the total revenue recorded by the factory.
    public decimal GetTotalRevenue()
    {
        return _totalRevenue;
    }

    // Returns the total number of units sold.
    public int GetTotalSalesQuantity()
    {
        return _totalSalesQuantity;
    }

    // Estimates profit by subtracting estimated production cost from revenue.
    public decimal CalculateProfit()
    {
        decimal totalCost = _productRepository
            .GetAll()
            .Sum(product => product.ProductionCost * (1000 - product.Cantitate));

        return _totalRevenue - totalCost;
    }

    // Returns a list of machines that require maintenance within the given number of days.
    public List<Machine> GetMachinesRequiringMaintenance(int daysAhead = 7)
    {
        return _machineRepository
            .GetAll()
            .Where(machine => machine.EstimateDaysUntilMaintenance() <= daysAhead)
            .ToList();
    }
    // Loads machines from the specified file into the repository.
    public void IncarcaMasini(string machinesFileName)
    {
        _machineRepository.LoadMachines(machinesFileName);
    }

    // Loads products from the specified file into the repository.
    public void IncarcaProduse(string productsFileName)
    {
        _productRepository.LoadProducts(productsFileName);
    }

    // Saves all machines to the specified file.
    public void SalveazaMasini(string machinesFileName)
    {
        _machineRepository.SaveAllMachines(machinesFileName);
    }

    // Saves all products to the specified file.
    public void SalveazaProduse(string productsFileName)
    {
        _productRepository.SaveAllProducts(productsFileName);
    }

    // Displays predictive maintenance information for machines within the given window.
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

    
    // Interactive console flow to add a new machine and optionally save it.
    public void InteractiveAddMachine(string machinesFileName)
    {
        Console.Write(Messages.SerialNumberPrompt);
        string serial = Console.ReadLine();
        Console.Write(Messages.NamePrompt);
        string nume = Console.ReadLine();

        Console.WriteLine(Messages.MachineTypesMenu);
        Console.Write(Messages.Choose);
        string tip = Console.ReadLine();

        Machine masina = null;

        if (tip == "1")
            masina = new SewingMachine(serial, nume, DateTime.Now);
        else if (tip == "2")
            masina = new CuttingMachine(serial, nume, DateTime.Now);
        else
        {
            Console.WriteLine(Messages.InvalidOption);
            return;
        }

        Console.Write(Messages.AddPartPrompt);
        string raspuns = Console.ReadLine();
        if (raspuns == "yes")
        {
            Console.Write(Messages.PartNamePrompt);
            string numePiesa = Console.ReadLine();
            Console.Write(Messages.PartTypePrompt);
            string tipPiesa = Console.ReadLine();
            masina.AdaugaPiesa(new MachinePart(numePiesa, tipPiesa));
        }

        if (AdaugaMasina(masina))
        {
            Console.WriteLine(Messages.MachineAdded);
            SalveazaMasini(machinesFileName);
        }
    }

    // Interactive console flow to select technician, engineer and machine to repair.
    public void InteractiveRepairMachine()
    {
        AfiseazaAngajati();
        Console.Write(Messages.TechnicianIdPrompt);
        string idTeh = Console.ReadLine();
        Console.Write(Messages.EngineerIdPrompt);
        string idEng = Console.ReadLine();

        AfiseazaMasini();
        Console.Write(Messages.MachineSerialPrompt);
        string serial = Console.ReadLine();

        ReparaMasina(idTeh, idEng, serial);
    }

    // Interactive console flow to create a new product with typed attributes and persist it.
    public void InteractiveAddProduct(string productsFileName)
    {
        Console.Write(Messages.NamePrompt);
        string nume = Console.ReadLine();
        Console.Write(Messages.ProductionCostPrompt);
        if (!decimal.TryParse(Console.ReadLine(), out decimal productionCost))
        {
            Console.WriteLine(Messages.InvalidOption);
            return;
        }
        Console.Write(Messages.SellingPricePrompt);
        if (!decimal.TryParse(Console.ReadLine(), out decimal sellingPrice))
        {
            Console.WriteLine(Messages.InvalidOption);
            return;
        }
        Console.Write(Messages.InitialQuantityPrompt);
        if (!int.TryParse(Console.ReadLine(), out int cantitate))
        {
            Console.WriteLine(Messages.InvalidOption);
            return;
        }

        Console.WriteLine(Messages.ProductTypesMenu);
        Console.Write(Messages.Choose);
        string tip = Console.ReadLine();

        Product produs = null;

        if (tip == "1")
        {
            Console.Write(Messages.SizePrompt);
            string marime = Console.ReadLine();
            produs = new WoodenCubes(nume, productionCost, sellingPrice, cantitate, marime);
        }
        else if (tip == "2")
        {
            Console.Write(Messages.SizePrompt);
            string marime = Console.ReadLine();
            produs = new Doll(nume, productionCost, sellingPrice, cantitate, marime);
        }
        else if (tip == "3")
        {
            Console.Write(Messages.SizePrompt);
            string marime = Console.ReadLine();
            produs = new TedyBear(nume, productionCost, sellingPrice, cantitate, marime);
        }
        else if (tip == "4")
        {
            Console.Write(Messages.SizePrompt);
            string marime = Console.ReadLine();
            produs = new Ball(nume, productionCost, sellingPrice, cantitate, marime);
        }
        else if (tip == "5")
        {
            Console.Write(Messages.SizePrompt);
            string marime = Console.ReadLine();
            produs = new Frisbee(nume, productionCost, sellingPrice, cantitate, marime);
        }

        if (produs == null)
        {
            Console.WriteLine(Messages.InvalidOption);
            return;
        }

        if (AdaugaProdus(produs))
        {
            Console.WriteLine(Messages.ProductAdded);
            SalveazaProduse(productsFileName);
        }
    }

    // Interactive console flow to add stock to an existing product.
    public void InteractiveAddStock()
    {
        AfiseazaProduse();
        Console.Write(Messages.ProductNamePrompt);
        string nume = Console.ReadLine();

        Console.Write(Messages.AmountToAddPrompt);
        if (!int.TryParse(Console.ReadLine(), out int cantitate))
        {
            Console.WriteLine(Messages.InvalidOption);
            return;
        }

        AdaugaStocProduse(nume, cantitate);
    }

    // Interactive console flow to process a sale by a sales agent.
    public void InteractiveSellProduct()
    {
        AfiseazaAngajati();
        Console.Write(Messages.SalesAgentIdPrompt);
        string idAgent = Console.ReadLine();

        AfiseazaProduse();
        Console.Write(Messages.ProductToSellPrompt);
        string numeProdus = Console.ReadLine();

        Console.Write(Messages.SellingQuantityPrompt);
        if (!int.TryParse(Console.ReadLine(), out int cantitate))
        {
            Console.WriteLine(Messages.InvalidOption);
            return;
        }

        VindeProdus(idAgent, numeProdus, cantitate);
    }

    // Interactive console flow to add an employee and create login credentials.
    public void InteractiveAddEmployee(string employeesFileName, Login loginManager)
    {
        Console.Write(Messages.IdPrompt);
        string id = Console.ReadLine();
        if (EmployeeIdExists(id))
        {
            Console.WriteLine(Messages.EmployeeIdAlreadyExists(id));
            return;
        }

        Console.Write(Messages.NamePrompt);
        string nume = Console.ReadLine();
        Console.Write(Messages.SalaryPrompt);
        if (!decimal.TryParse(Console.ReadLine(), out decimal salariu))
        {
            Console.WriteLine(Messages.InvalidOption);
            return;
        }

        Console.WriteLine(Messages.EmployeeTypesMenu);
        Console.Write(Messages.Choose);
        string tip = Console.ReadLine();

        Employee angajat = null;
        string role = null;

        if (tip == "1") { angajat = new Director(id, nume, salariu, DateTime.Now); role = "Director"; }
        else if (tip == "2") { angajat = new ProductionManager(id, nume, salariu, DateTime.Now); role = "ProductionManager"; }
        else if (tip == "3") { angajat = new Engineer(id, nume, salariu, DateTime.Now); role = "Engineer"; }
        else if (tip == "4") { angajat = new Technician(id, nume, salariu, DateTime.Now); role = "Technician"; }
        else if (tip == "5") { angajat = new MachineOperator(id, nume, salariu, DateTime.Now); role = "MachineOperator"; }
        else if (tip == "6") { angajat = new SalesAgent(id, nume, salariu, DateTime.Now); role = "SalesAgent"; }
        else { Console.WriteLine(Messages.InvalidUser); return; }

        // Ask for login credentials
        Console.Write(Messages.UsernameForLoginPrompt);
        string username = Console.ReadLine();
        Console.Write(Messages.PasswordForLoginPrompt);
        string password = Console.ReadLine();

        if (AdaugaAngajat(angajat))
        {
            if (loginManager.SaveEmployeeCredential(id, username, password, role))
            {
                Console.WriteLine(Messages.EmployeeAdded);
            }
            else
            {
                Console.WriteLine(Messages.EmployeeAddedCredentialsFailed);
            }
        }
    }

    // Interactive console flow to create a production order by prompting for inputs.
    public void InteractiveCreateOrder()
    {
        AfiseazaAngajati();
        Console.Write(Messages.ProductionManagerIdPrompt);
        string idManager = Console.ReadLine();

        AfiseazaMasini();
        Console.Write(Messages.MachineForOrderPrompt);
        string serial = Console.ReadLine();

        Console.Write(Messages.ProductToManufacturePrompt);
        string produs = Console.ReadLine();

        Console.Write(Messages.TargetAmountPrompt);
        if (!int.TryParse(Console.ReadLine(), out int cantitate))
        {
            Console.WriteLine(Messages.InvalidOption);
            return;
        }

        Console.WriteLine(Messages.PriorityMenu);
        Console.Write(Messages.Choose);
        string prio = Console.ReadLine();

        Priority prioritate;
        if (prio == "1") prioritate = Priority.Low;
        else if (prio == "3") prioritate = Priority.High;
        else prioritate = Priority.Medium;

        CreazaComanda(idManager, serial, produs, cantitate, prioritate);
    }

    // Interactive console flow to execute a chosen production order.
    public void InteractiveExecuteOrder()
    {
        AfiseazaAngajati();
        Console.Write(Messages.MachineOperatorIdPrompt);
        string idOp = Console.ReadLine();

        AfiseazaComenzi();
        Console.Write(Messages.OrderIdPrompt);
        string idComanda = Console.ReadLine();

        Console.Write(Messages.UnitsToProducePrompt);
        if (!int.TryParse(Console.ReadLine(), out int unitati))
        {
            Console.WriteLine(Messages.InvalidOption);
            return;
        }

        ExecutaComanda(idOp, idComanda, unitati);
    }

    // Interactive console flow to execute the next prioritized order for an operator.
    public void InteractiveExecuteNextPriority()
    {
        AfiseazaAngajati();
        Console.Write(Messages.MachineOperatorIdPrompt);
        string idOp = Console.ReadLine();

        ProductionOrder nextOrder = GetNextPriorityOrder(idOp);
        if (nextOrder == null)
        {
            Console.WriteLine(Messages.NoActiveOrder);
            return;
        }

        Console.WriteLine(Messages.NextPriorityOrder);
        nextOrder.Afiseaza();

        Console.Write(Messages.PriorityUnitsPrompt);
        if (!int.TryParse(Console.ReadLine(), out int unitati))
        {
            Console.WriteLine(Messages.InvalidOption);
            return;
        }

        ExecutaComanda(idOp, nextOrder.Id, unitati);
    }

    // Interactive console flow to prompt director for company-public parameters and apply them.
    public void InteractiveMakeCompanyPublic(string directorId)
    {
        if (string.IsNullOrWhiteSpace(directorId))
        {
            Console.WriteLine(Messages.NoUserLoggedIn);
            return;
        }

        Console.Write(Messages.PercentagePublicPrompt);
        if (!decimal.TryParse(Console.ReadLine(), out decimal percentagePublic))
        {
            Console.WriteLine(Messages.InvalidPercentage);
            return;
        }

        Console.Write(Messages.SharesPrompt);
        if (!int.TryParse(Console.ReadLine(), out int shares))
        {
            Console.WriteLine(Messages.InvalidShareCount);
            return;
        }

        Console.Write(Messages.SharePricePrompt);
        if (!decimal.TryParse(Console.ReadLine(), out decimal sharePrice))
        {
            Console.WriteLine(Messages.InvalidSharePrice);
            return;
        }

        MakeCompanyPublic(directorId, percentagePublic, shares, sharePrice);
    }

    // Displays a dashboard with production efficiency metrics for each machine.
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

    // Displays current health/status for all machines.
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

    // Returns products whose stock is below or equal to the threshold.
    public List<Product> GetLowStockProducts(int threshold = 5)
    {
        return _productRepository
            .GetAll()
            .Where(product => product.Cantitate <= threshold)
            .ToList();
    }

    // Displays inventory alerts for products below the specified threshold.
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

    // Prints a low-stock alert for a single product when below threshold.
    private static void DisplayInventoryAlert(Product product, int threshold = 5)
    {
        if (product.Cantitate <= threshold)
            Console.WriteLine(Messages.LowStockAlert(product.Nume, product.Cantitate, threshold));
    }

    // Displays the sales report including totals and estimated profit.
    public void AfiseazaRaportVanzari()
    {
        Console.WriteLine(Messages.SalesReportTitle(Nume));
        Console.WriteLine(Messages.FactoryReportLine("Total Revenue", _totalRevenue + " RON"));
        Console.WriteLine(Messages.FactoryReportLine("Total Units Sold", _totalSalesQuantity));
        Console.WriteLine(Messages.FactoryReportLine("Average Price Per Unit", (_totalSalesQuantity > 0 ? (_totalRevenue / _totalSalesQuantity).ToString("F2") : "N/A") + " RON"));
        Console.WriteLine(Messages.FactoryReportLine("Estimated Profit", CalculateProfit() + " RON"));
        Console.WriteLine(string.Empty);
    }

    // Displays all production orders from the repository.
    public void AfiseazaComenzi()
    {
        _orderRepository.DisplayAll();
    }

    // Displays all production orders sorted by priority.
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

    // Returns the next production order by priority for the given operator, or null.
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
