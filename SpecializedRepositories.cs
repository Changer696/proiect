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
        // Displays all employees or an empty-message when none exist.
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
        // Displays all machines or an empty-message when none exist.
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
        // Saves all machines to the given file path.
        try
        {
            var lines = _items.Select(m => m.ToDataLine());
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

    // ---------- INCARCARE ----------

    public void LoadMachines(string machinesFileName)
    {
        // Loads machines from file into repository (skips comments and invalid lines).
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

        var parsed = rawLines.Select(l => new { Line = l }).ToList();

        foreach (var entry in parsed)
        {
            var tip = entry.Line.Split(';')[0].Trim();
            try
            {
                Machine masina = Machine.FromDataLine(entry.Line);
                if (masina == null)
                {
                    Console.WriteLine(Messages.WarningUnknownMachineType(tip));
                    continue;
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

    
}


public class ProductRepository : Repository<Product>
{
    private string _productsFilePath;
    // Use base Repository<T> generic methods for find/exists/remove operations.
    public void DisplayAll()
    {
        // Displays all products or a message when none are present.
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
        // Saves all products to the specified file.
        _productsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, productsFileName);

        try
        {
            var lines = _items.Select(p => p.ToDataLine());
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

    public void LoadProducts(string productsFileName)
    {
        // Loads products from file into repository, skipping comments and invalid lines.
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

        var parsed = rawLines.Select(l => new { Line = l }).ToList();

        foreach (var entry in parsed)
        {
            var tip = entry.Line.Split(';')[0].Trim();
            try
            {
                Product produs = Product.FromDataLine(entry.Line);
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

    

    public List<Product> FindByCategory(ProductCategory category)
    {
        // Returns all products that match the given category.
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
        // Displays all orders or a message when none exist.
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