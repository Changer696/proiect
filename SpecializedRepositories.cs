using SmartFactorySimple;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


public class EmployeeRepository : RepositoryWithId<Employee>
{
    public override bool Add(Employee employee)
    {
        if (employee == null)
            return false;

        if (ExistsById(employee.Id))
        {
            Console.WriteLine(Messages.ExistingEmployeeConflict(employee.Id));
            return false;
        }

        return base.Add(employee);
    }

    public void DisplayAll()
    {
        if (!_items.Any())
        {
            Console.WriteLine(Messages.NoEmployeesMessage);
            return;
        }

        Console.WriteLine(Messages.EmployeesHeader);
        _items.ForEach(employee => employee.Afiseaza());
    }
}


public class MachineRepository : Repository<Machine>
{
    private string _machinesFilePath;
    // Generic find/exists/remove operations are provided by the base Repository<T>

    public override bool Add(Machine machine)
    {
        if (machine == null)
            return false;

        if (Exists(m => m.SerialNumber == machine.SerialNumber))
        {
            Console.WriteLine(Messages.ExistingMachineConflict(machine.SerialNumber));
            return false;
        }

        return base.Add(machine);
    }

    public void DisplayAll()
    {
        if (!_items.Any())
        {
            Console.WriteLine(Messages.NoMachinesMessage);
            return;
        }

        Console.WriteLine(Messages.MachinesHeader);
        _items.ForEach(machine => machine.Afiseaza());
    }

    // ---------- SALVARE ----------

    public bool SaveAllMachines(string machinesFileName)
    {
        _machinesFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, machinesFileName);

            try
            {
                var lines = _items.Select(SerializeMachine);
                File.WriteAllLines(_machinesFilePath, lines);
                Console.WriteLine(Messages.SavedMachinesCount(_items.Count));
                return true;
            }
        catch (Exception ex)
        {
            Console.WriteLine(Messages.SaveFailed("machines", ex));
            return false;
        }
    }

    private string SerializeMachine(Machine m)
    {
        string tip = m.GetType().Name;
        string lastMaint = m.LastMaintenanceDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? "";

        string pieseSerializate = string.Join("~", m.Piese.Take(m.NrPiese).Select(p =>
            $"{p.Nume},{p.Tip},{p.EFunctionala},{p.DataInstalarii:yyyy-MM-dd HH:mm:ss}"));

        return string.Join(";",
            tip,
            m.SerialNumber,
            m.Nume,
            m.Status,
            m.Conditie,
            m.DataFabricatiei.ToString("yyyy-MM-dd HH:mm:ss"),
            m.ProductionCycles,
            lastMaint,
            pieseSerializate);
    }

    // ---------- INCARCARE ----------

    public void LoadMachines(string machinesFileName)
    {
        _machinesFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, machinesFileName);
        Clear(); // golim ce era deja in _items, ca sa nu duplicam la reincarcare

        if (!File.Exists(_machinesFilePath))
        {
            Console.WriteLine(Messages.FileNotFound(_machinesFilePath));
            return;
        }

        var rawLines = File.ReadAllLines(_machinesFilePath)
            .Select(l => l.Trim())
            .Where(l => !string.IsNullOrWhiteSpace(l) && !l.StartsWith("#"))
            .ToList();

        var parsed = rawLines.Select(l => new { Line = l, Parts = l.Split(';') }).ToList();

        foreach (var entry in parsed)
        {
            if (entry.Parts.Length != 9)
            {
                Console.WriteLine(Messages.WarningInvalidLine(entry.Line));
                continue;
            }

            try
            {
                string tip = entry.Parts[0].Trim();
                string serial = entry.Parts[1].Trim();
                string nume = entry.Parts[2].Trim();
                var status = Enum.Parse<MachineStatus>(entry.Parts[3].Trim());
                var conditie = Enum.Parse<MachineCondition>(entry.Parts[4].Trim());
                var dataFab = DateTime.Parse(entry.Parts[5].Trim());
                int cicluri = int.Parse(entry.Parts[6].Trim());
                DateTime? lastMaint = string.IsNullOrWhiteSpace(entry.Parts[7])
                    ? null
                    : DateTime.Parse(entry.Parts[7].Trim());
                string pieseRaw = entry.Parts[8].Trim();

                Machine masina = CreeazaMasina(tip, serial, nume, dataFab);
                if (masina == null)
                {
                    Console.WriteLine(Messages.WarningUnknownMachineType(tip));
                    continue;
                }

                masina.Status = status;
                masina.Conditie = conditie;
                masina.RestoreState(cicluri, lastMaint);

                if (!string.IsNullOrWhiteSpace(pieseRaw))
                {
                    foreach (string piesaStr in pieseRaw.Split('~'))
                    {
                        string[] pp = piesaStr.Split(',');
                        if (pp.Length != 4) continue;

                        var piesa = new MachinePart(pp[0].Trim(), pp[1].Trim())
                        {
                            EFunctionala = bool.Parse(pp[2].Trim()),
                            DataInstalarii = DateTime.Parse(pp[3].Trim())
                        };
                        masina.AdaugaPiesa(piesa);
                    }
                }

                Add(masina);
            }
            catch (Exception ex)
            {
                Console.WriteLine(Messages.WarningParseMachineLine(entry.Line, ex));
            }
        }
        Console.WriteLine(Messages.LoadedMachinesCount(_items.Count));
    }

    // ---------- FACTORY ----------

    private Machine CreeazaMasina(string tip, string serial, string nume, DateTime dataFab)
    {
        return tip switch
        {
            "SewingMachine" => new SewingMachine(serial, nume, dataFab),
            "CuttingMachine" => new CuttingMachine(serial, nume, dataFab),
            _ => null
        };
    }
}


public class ProductRepository : Repository<Product>
{
    private string _productsFilePath;
    // Use base Repository<T> generic methods for find/exists/remove operations.
    public void DisplayAll()
    {
        if (!_items.Any())
        {
            Console.WriteLine(Messages.NoProductsMessage);
            return;
        }

        Console.WriteLine(Messages.ProductsHeader);
        _items.ForEach(product => product.Afiseaza());
    }

    // ---------- SAVE / LOAD ----------

    public bool SaveAllProducts(string productsFileName)
    {
        _productsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, productsFileName);

        try
        {
            var lines = _items.Select(SerializeProduct);
            File.WriteAllLines(_productsFilePath, lines);
            Console.WriteLine(Messages.SavedProductsCount(_items.Count));
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(Messages.SaveFailed("products", ex));
            return false;
        }
    }

    private string SerializeProduct(Product p)
    {
        string tip = p.GetType().Name;
        // marime is a common extra property for derived products; reflect if present
        string marime = "";
        var prop = p.GetType().GetField("Marime");
        if (prop != null)
        {
            var val = prop.GetValue(p);
            marime = val?.ToString() ?? "";
        }

        return string.Join(";",
            tip,
            p.Nume,
            p.Category,
            p.ProductionCost.ToString(System.Globalization.CultureInfo.InvariantCulture),
            p.SellingPrice.ToString(System.Globalization.CultureInfo.InvariantCulture),
            p.Cantitate,
            marime);
    }

    public void LoadProducts(string productsFileName)
    {
        _productsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, productsFileName);
        Clear();

        if (!File.Exists(_productsFilePath))
        {
            Console.WriteLine(Messages.FileInfoNotFound(_productsFilePath));
            return;
        }

        var rawLines = File.ReadAllLines(_productsFilePath)
            .Select(l => l.Trim())
            .Where(l => !string.IsNullOrWhiteSpace(l) && !l.StartsWith("#"))
            .ToList();

        var parsed = rawLines.Select(l => new { Line = l, Parts = l.Split(';') }).ToList();

        foreach (var entry in parsed)
        {
            if (entry.Parts.Length < 6)
            {
                Console.WriteLine(Messages.WarningInvalidProductLine(entry.Line));
                continue;
            }

            try
            {
                string tip = entry.Parts[0].Trim();
                string nume = entry.Parts[1].Trim();
                var category = Enum.Parse<ProductCategory>(entry.Parts[2].Trim());
                decimal productionCost = decimal.Parse(entry.Parts[3].Trim(), System.Globalization.CultureInfo.InvariantCulture);
                decimal sellingPrice = decimal.Parse(entry.Parts[4].Trim(), System.Globalization.CultureInfo.InvariantCulture);
                int cantitate = int.Parse(entry.Parts[5].Trim());
                string marime = entry.Parts.Length >= 7 ? entry.Parts[6].Trim() : "";

                Product produs = CreeazaProdus(tip, nume, productionCost, sellingPrice, cantitate, marime);
                if (produs == null)
                {
                    Console.WriteLine(Messages.WarningUnknownProductType(tip));
                    continue;
                }

                Add(produs);
            }
            catch (Exception ex)
            {
                Console.WriteLine(Messages.WarningParseProductLine(entry.Line, ex));
            }
        }
        Console.WriteLine(Messages.LoadedProductsCount(_items.Count));
    }

    private Product CreeazaProdus(string tip, string nume, decimal productionCost, decimal sellingPrice, int cantitate, string marime)
    {
        return tip switch
        {
            "WoodenCubes" => new WoodenCubes(nume, productionCost, sellingPrice, cantitate, marime),
            "TedyBear" => new TedyBear(nume, productionCost, sellingPrice, cantitate, marime),
            "Ball" => new Ball(nume, productionCost, sellingPrice, cantitate, marime),
            "Doll" => new Doll(nume, productionCost, sellingPrice, cantitate, marime),
            "Frisbee" => new Frisbee(nume, productionCost, sellingPrice, cantitate, marime),
            _ => null
        };


    }

    public List<Product> FindByCategory(ProductCategory category)
    {
        return GetWhere(p => p.Category == category);
    }
}


public class ProductionOrderRepository : RepositoryWithId<ProductionOrder>
{
    public override bool Add(ProductionOrder order)
    {
        if (order == null)
            return false;

        return base.Add(order);
    }

    public void DisplayAll()
    {
        if (!_items.Any())
        {
            Console.WriteLine(Messages.NoOrdersAvailable);
            return;
        }

        Console.WriteLine(Messages.OrdersHeader);
        _items.ForEach(order => order.Afiseaza());
    }

    public List<ProductionOrder> GetAllActive()
    {
        return GetWhere(o => o.Status != ProductionOrderStatus.Completed);
    }

    public List<ProductionOrder> GetAllCompleted()
    {
        return GetWhere(o => o.Status == ProductionOrderStatus.Completed);
    }

    public List<ProductionOrder> GetByStatus(ProductionOrderStatus status)
    {
        return GetWhere(o => o.Status == status);
    }

    public List<ProductionOrder> GetByPriority(Priority priority)
    {
        return GetWhere(o => o.Prioritate == priority);
    }

    public List<ProductionOrder> GetSortedByPriority()
    {
        return GetWhere(o => o.Status != ProductionOrderStatus.Completed)
            .OrderByDescending(o => GetPriorityValue(o.Prioritate))
            .ThenBy(o => GetStatusValue(o.Status))
            .ToList();
    }

    public ProductionOrder GetNextByPriority()
    {
        return GetWhere(o => o.Status != ProductionOrderStatus.Completed)
            .OrderByDescending(o => GetPriorityValue(o.Prioritate))
            .ThenBy(o => GetStatusValue(o.Status))
            .FirstOrDefault();
    }

    private int GetPriorityValue(Priority priority)
    {
        return priority switch
        {
            Priority.High => 3,
            Priority.Medium => 2,
            Priority.Low => 1,
            _ => 0
        };
    }

    private int GetStatusValue(ProductionOrderStatus status)
    {
        return status switch
        {
            ProductionOrderStatus.Created => 2,
            ProductionOrderStatus.InProgress => 1,
            ProductionOrderStatus.Completed => 0,
            _ => 0
        };
    }
}