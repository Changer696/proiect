using System;
using System.Collections.Generic;
using System.Linq;


public class EmployeeRepository : RepositoryWithId<Employee>
{
    public override bool Add(Employee employee)
    {
        if (employee == null)
            return false;

        if (ExistsById(employee.Id))
        {
            Console.WriteLine($"There is already an employee with the ID {employee.Id}");
            return false;
        }

        return base.Add(employee);
    }

    public void DisplayAll()
    {
        if (_items.Count == 0)
        {
            Console.WriteLine("There are no employees!");
            return;
        }

        Console.WriteLine("=== EMPLOYEES ===");
        foreach (var employee in _items)
        {
            employee.Afiseaza();
        }
    }
}


public class MachineRepository : Repository<Machine>
{
    public Machine FindBySerialNumber(string serialNumber)
    {
        return _items.FirstOrDefault(m => m.SerialNumber == serialNumber);
    }

    public bool ExistsBySerialNumber(string serialNumber)
    {
        return _items.Any(m => m.SerialNumber == serialNumber);
    }

    public bool RemoveBySerialNumber(string serialNumber)
    {
        Machine machine = FindBySerialNumber(serialNumber);
        if (machine != null)
        {
            return Remove(machine);
        }
        return false;
    }

    public override bool Add(Machine machine)
    {
        if (machine == null)
            return false;

        if (ExistsBySerialNumber(machine.SerialNumber))
        {
            Console.WriteLine($"There is already a machine with the serial number {machine.SerialNumber}");
            return false;
        }

        return base.Add(machine);
    }

    public void DisplayAll()
    {
        if (_items.Count == 0)
        {
            Console.WriteLine("There are no machines!");
            return;
        }

        Console.WriteLine("=== MACHINES ===");
        foreach (var machine in _items)
        {
            machine.Afiseaza();
        }
    }
}


public class ProductRepository : Repository<Product>
{
    public Product FindByName(string name)
    {
        return _items.FirstOrDefault(p => p.Nume == name);
    }

    public bool ExistsByName(string name)
    {
        return _items.Any(p => p.Nume == name);
    }

    public bool RemoveByName(string name)
    {
        Product product = FindByName(name);
        if (product != null)
        {
            return Remove(product);
        }
        return false;
    }

    public void DisplayAll()
    {
        if (_items.Count == 0)
        {
            Console.WriteLine("There are no products!");
            return;
        }

        Console.WriteLine("=== PRODUCTS ===");
        foreach (var product in _items)
        {
            product.Afiseaza();
        }
    }

    public List<Product> FindByCategory(ProductCategory category)
    {
        return _items.Where(p => p.Category == category).ToList();
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
        if (_items.Count == 0)
        {
            Console.WriteLine("There are no orders!");
            return;
        }

        Console.WriteLine("=== ORDERS ===");
        foreach (var order in _items)
        {
            order.Afiseaza();
        }
    }

    public List<ProductionOrder> GetAllActive()
    {
        return _items.Where(o => o.Status != ProductionOrderStatus.Completed).ToList();
    }

    public List<ProductionOrder> GetAllCompleted()
    {
        return _items.Where(o => o.Status == ProductionOrderStatus.Completed).ToList();
    }

    public List<ProductionOrder> GetByStatus(ProductionOrderStatus status)
    {
        return _items.Where(o => o.Status == status).ToList();
    }

    public List<ProductionOrder> GetByPriority(Priority priority)
    {
        return _items.Where(o => o.Prioritate == priority).ToList();
    }

    public List<ProductionOrder> GetSortedByPriority()
    {
        return _items
            .Where(o => o.Status != ProductionOrderStatus.Completed)
            .OrderByDescending(o => GetPriorityValue(o.Prioritate))
            .ThenBy(o => GetStatusValue(o.Status))
            .ToList();
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
