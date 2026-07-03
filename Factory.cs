using System;
using System.Security.Cryptography.X509Certificates;

public class Factory
{
    public string Nume;

    private Employee[] _angajati;
    private Machine[] _masini;
    private Product[] _produse;
    private ProductionOrder[] _comenzi;

    private int _nrAngajati = 0;
    private int _nrMasini = 0;
    private int _nrProduse = 0;
    private int _nrComenzi = 0;
    private int _idComandaCounter = 1;

    public Factory(string nume)
    {
        Nume = nume;
        _angajati = new Employee[50];
        _masini = new Machine[20];
        _produse = new Product[50];
        _comenzi = new ProductionOrder[100];
    }

    // ANGAJATI 

    public bool AdaugaAngajat(Employee angajat)
    {
        if (_nrAngajati >= _angajati.Length)
        {
            Console.WriteLine("There's no more room for employees!");
            return false;
        }

        for (int i = 0; i < _nrAngajati; i++)
        {
            if (_angajati[i].Id == angajat.Id)
            {
                Console.WriteLine("There is already an employee with the ID " + angajat.Id);
                return false;
            }
        }

        _angajati[_nrAngajati] = angajat;
        _nrAngajati++;
        return true;
    }

    public void AfiseazaAngajati()
    {
        if (_nrAngajati == 0)
        {
            Console.WriteLine("There are no employees!");
            return;
        }

        Console.WriteLine("=== EMPLOYEES ===");
        for (int i = 0; i < _nrAngajati; i++)
        {
            _angajati[i].Afiseaza();
        }
    }

    public Employee GasesteAngajat(string id)
    {
        for (int i = 0; i < _nrAngajati; i++)
        {
            if (_angajati[i].Id == id)
                return _angajati[i];
        }
        return null;
    }

    public bool StergeAngajat(string id)
    {
        for (int i = 0; i < _nrAngajati; i++)
        {
            if (_angajati[i].Id == id)
            {
                for (int j = i; j < _nrAngajati - 1; j++)
                {
                    _angajati[j] = _angajati[j + 1];
                }
                _angajati[_nrAngajati - 1] = null;
                _nrAngajati--;
                Console.WriteLine("Employee successfully deleted!");
                return true;
            }
        }
        Console.WriteLine("The employee doesn't exist!");
        return false;
    }

    //  MASINI 

    public bool AdaugaMasina(Machine masina)
    {
        if (_nrMasini >= _masini.Length)
        {
            Console.WriteLine("There's no more room for machines!");
            return false;
        }

        for (int i = 0; i < _nrMasini; i++)
        {
            if (_masini[i].SerialNumber == masina.SerialNumber)
            {
                Console.WriteLine("There is already a machine with the serial number " + masina.SerialNumber);
                return false;
            }
        }

        _masini[_nrMasini] = masina;
        _nrMasini++;
        return true;
    }

    
    public void AfiseazaMasini()
    {
        if (_nrMasini == 0)
        {
            Console.WriteLine("There are no machines!");
            return;
        }

        Console.WriteLine("=== MACHINES ===");
        for (int i = 0; i < _nrMasini; i++)
        {
            _masini[i].Afiseaza();
        }
    }

    public Machine GasesteMasina(string serial)
    {
        for (int i = 0; i < _nrMasini; i++)
        {
            if (_masini[i].SerialNumber == serial)
                return _masini[i];
        }
        return null;
    }

    // PRODUSE

    public bool AdaugaProdus(Product produs)
    {
        if (_nrProduse >= _produse.Length)
        {
            Console.WriteLine("There's no more room for products!");
            return false;
        }
        _produse[_nrProduse] = produs;
        _nrProduse++;
        return true;
    }

    public void AfiseazaProduse()
    {
        if (_nrProduse == 0)
        {
            Console.WriteLine("There are no products!");
            return;
        }

        Console.WriteLine("=== PRODUCTS ===");
        for (int i = 0; i < _nrProduse; i++)
        {
            _produse[i].Afiseaza();
        }
    }

    public Product GasesteProdus(string nume)
    {
        for (int i = 0; i < _nrProduse; i++)
        {
            if (_produse[i].Nume == nume)
                return _produse[i];
        }
        return null;
    }

    // PRODUCTIE 

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

        if (_nrComenzi >= _comenzi.Length)
        {
            Console.WriteLine("There's no room for orders anymore!");
            return;
        }

        string idComanda = "ORD" + _idComandaCounter;
        _idComandaCounter++;

        ProductionOrder comanda = manager.CreazaComanda(idComanda, masina, produs, cantitate);
        _comenzi[_nrComenzi] = comanda;
        _nrComenzi++;
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

        ProductionOrder comanda = null;
        for (int i = 0; i < _nrComenzi; i++)
        {
            if (_comenzi[i].Id == idComanda)
            {
                comanda = _comenzi[i];
                break;
            }
        }

        if (comanda == null)
        {
            Console.WriteLine("Order doesn't exist!");
            return;
        }

        op.Opereaza(comanda.Masina);

        if (comanda.Masina.Status == MachineStatus.Running)
        {
            comanda.InregistreazaProductie(unitati);
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

    public void AdaugaStocProduse(string numeProdus , int cantitate)
    {
        Product produs = GasesteProdus(numeProdus);
        if(numeProdus == null)
        {
            Console.WriteLine("There is no such product");
            return;
        }
        produs.AdaugaStoc(cantitate);
        Console.WriteLine($"New stock added:{numeProdus} + {cantitate} pieces");
    }

    public void VandeProdus(string idAgent, string numeProdus, int cantitate)
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

        agent.VindeProdus(produs, cantitate);
    }

    // ===== RAPOARTE =====

    public void AfiseazaRaportGeneral()
    {
        Console.WriteLine("\n=== REPORT: " + Nume + " ===");
        Console.WriteLine("Employees: " + _nrAngajati);
        Console.WriteLine("Machines:   " + _nrMasini);
        Console.WriteLine("Products:  " + _nrProduse);
        Console.WriteLine("Orders:  " + _nrComenzi);
    }

    public void AfiseazaComenzi()
    {
        if (_nrComenzi == 0)
        {
            Console.WriteLine("There are no orders!");
            return;
        }

        Console.WriteLine("=== Orders ===");
        for (int i = 0; i < _nrComenzi; i++)
        {
            _comenzi[i].Afiseaza();
        }
    }

    public void AfiseazaComenziSortedByPriority()
    {
        if (_nrComenzi == 0)
        {
            Console.WriteLine("Nu exista comenzi!");
            return;
        }

        ProductionOrder[] comenziActive = new ProductionOrder[_nrComenzi];
        int nrActive = 0;

        for (int i = 0; i < _nrComenzi; i++)
        {
            if (_comenzi[i].Status != ProductionOrderStatus.Completed)
            {
                comenziActive[nrActive] = _comenzi[i];
                nrActive++;
            }
        }

        // Sort by priority: High > Medium > Low, then by Status (Created > InProgress)
        for (int i = 0; i < nrActive - 1; i++)
        {
            for (int j = 0; j < nrActive - i - 1; j++)
            {
                if (ComparePriority(comenziActive[j], comenziActive[j + 1]) < 0)
                {
                    ProductionOrder temp = comenziActive[j];
                    comenziActive[j] = comenziActive[j + 1];
                    comenziActive[j + 1] = temp;
                }
            }
        }

        Console.WriteLine("=== COMENZI SORTATE DUPA PRIORITATE ===");
        for (int i = 0; i < nrActive; i++)
        {
            comenziActive[i].Afiseaza();
        }
    }

    private int ComparePriority(ProductionOrder a, ProductionOrder b)
    {
        int priorityOrder_a = GetPriorityValue(a.Prioritate);
        int priorityOrder_b = GetPriorityValue(b.Prioritate);

        if (priorityOrder_a != priorityOrder_b)
            return priorityOrder_a.CompareTo(priorityOrder_b);

        int statusOrder_a = GetStatusValue(a.Status);
        int statusOrder_b = GetStatusValue(b.Status);

        return statusOrder_a.CompareTo(statusOrder_b);
    }

    private int GetPriorityValue(Priority priority)
    {
        if (priority == Priority.High)
            return 3;
        else if (priority == Priority.Medium)
            return 2;
        else
            return 1;
    }

    private int GetStatusValue(ProductionOrderStatus status)
    {
        if (status == ProductionOrderStatus.Created)
            return 2;
        else if (status == ProductionOrderStatus.InProgress)
            return 1;
        else
            return 0;
    }

    public ProductionOrder GetNextPriorityOrder(string idOperator)
    {
        Employee angajat = GasesteAngajat(idOperator);
        if (angajat == null)
            return null;

        if (!(angajat is MachineOperator))
            return null;

        ProductionOrder nextOrder = null;

        for (int i = 0; i < _nrComenzi; i++)
        {
            if (_comenzi[i].Status != ProductionOrderStatus.Completed)
            {
                if (nextOrder == null || ComparePriority(_comenzi[i], nextOrder) > 0)
                {
                    nextOrder = _comenzi[i];
                }
            }
        }

        return nextOrder;
    }
}
