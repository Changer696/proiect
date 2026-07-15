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

    // Removes an employee without printing messages, logging, or registering
    // a new undo entry. Intended to be called only from inside an Undo action.
    public bool RemoveAngajatForUndo(Employee angajat)
    {
        return _employeeRepository.Remove(angajat);
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
        Employee angajat = _employeeRepository.FindById(id);

        if (angajat != null && _employeeRepository.RemoveById(id))
        {
            Console.WriteLine("Employee successfully deleted!");
            Logging.Log(id, $"Employee removed: {id}");
            UndoManager.Register($"Delete employee {angajat.Nume} ({id})", () =>
            {
                _employeeRepository.Add(angajat);
            });
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
        bool added = _machineRepository.Add(masina);
        if (added)
        {
            UndoManager.Register($"Add machine {masina.Nume}", () =>
            {
                _machineRepository.Remove(masina);
            });
        }
        return added;
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

        if (produs != null)
        {
            UndoManager.Register($"Add product {produs.Nume}", () =>
            {
                _productRepository.Remove(produs);
            });
        }

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

        ProductionOrder comanda = manager.CreazaComanda(idComanda, masina, produs, cantitate);
        _orderRepository.Add(comanda);

        UndoManager.Register($"Create order {idComanda} ({produs})", () =>
        {
            _orderRepository.Remove(comanda);
        });
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

        UndoManager.Register($"Add stock {cantitate}x {numeProdus}", () =>
        {
            produs.VindeStoc(cantitate);
        });
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

            UndoManager.Register($"Sale of {quantity}x {productName}", () =>
            {
                _totalRevenue -= saleAmount;
                _totalSalesQuantity -= quantity;
                p.AdaugaStoc(quantity);
            });
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
        decimal totalCost = 0;
        foreach (var product in _productRepository.GetAll())
        {
            totalCost += product.ProductionCost * (1000 - product.Cantitate);
        }
        return _totalRevenue - totalCost;
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

        List<ProductionOrder> comenziSortate = _orderRepository.GetSortedByPriority();
        return comenziSortate.FirstOrDefault();
    }
}