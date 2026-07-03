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
            Console.WriteLine("Nu mai e loc pentru angajati!");
            return false;
        }

        for (int i = 0; i < _nrAngajati; i++)
        {
            if (_angajati[i].Id == angajat.Id)
            {
                Console.WriteLine("Exista deja un angajat cu ID-ul " + angajat.Id);
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
            Console.WriteLine("Nu exista angajati!");
            return;
        }

        Console.WriteLine("=== ANGAJATI ===");
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
                Console.WriteLine("Angajat sters cu succes!");
                return true;
            }
        }
        Console.WriteLine("Angajatul nu exista!");
        return false;
    }

    //  MASINI 

    public bool AdaugaMasina(Machine masina)
    {
        if (_nrMasini >= _masini.Length)
        {
            Console.WriteLine("Nu mai e loc pentru masini!");
            return false;
        }

        for (int i = 0; i < _nrMasini; i++)
        {
            if (_masini[i].SerialNumber == masina.SerialNumber)
            {
                Console.WriteLine("Exista deja o masina cu serialul " + masina.SerialNumber);
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
            Console.WriteLine("Nu exista masini!");
            return;
        }

        Console.WriteLine("=== MASINI ===");
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
            Console.WriteLine("Nu mai e loc pentru produse!");
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
            Console.WriteLine("Nu exista produse!");
            return;
        }

        Console.WriteLine("=== PRODUSE ===");
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
            Console.WriteLine("Angajatul nu exista!");
            return;
        }

        if (!(angajat is ProductionManager))
        {
            Console.WriteLine(angajat.Nume + " nu este ProductionManager!");
            return;
        }

        ProductionManager manager = (ProductionManager)angajat;

        Machine masina = GasesteMasina(serialMasina);
        if (masina == null)
        {
            Console.WriteLine("Masina nu exista!");
            return;
        }

        if (_nrComenzi >= _comenzi.Length)
        {
            Console.WriteLine("Nu mai e loc pentru comenzi!");
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
            Console.WriteLine("Angajatul nu exista!");
            return;
        }

        if (!(angajat is MachineOperator))
        {
            Console.WriteLine(angajat.Nume + " nu este MachineOperator!");
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
            Console.WriteLine("Comanda nu exista!");
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
            Console.WriteLine("Unul dintre angajati nu exista!");
            return;
        }

        if (a1 is not Technician)
        {
            Console.WriteLine(a1.Nume + " nu este Technician!");
            return;
        }

        if (a2 is not Engineer)
        {
            Console.WriteLine(a2.Nume + " nu este Engineer!");
            return;
        }

        Technician teh = (Technician)a1;
        Engineer eng = (Engineer)a2;

        Machine masina = GasesteMasina(serial);
        if (masina == null)
        {
            Console.WriteLine("Masina nu exista!");
            return;
        }

        if (masina.Status == MachineStatus.Running)
        {
            Console.WriteLine("Opreste masina inainte de reparatie!");
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
            Console.WriteLine("Nu exista un asemenea produs");
            return;
        }
        produs.AdaugaStoc(cantitate);
        Console.WriteLine($"Stoc nou adaugat:{numeProdus} + {cantitate} bucati");
    }

    public void VandeProdus(string idAgent, string numeProdus, int cantitate)
    {
        Employee angajat = GasesteAngajat(idAgent);
        if (angajat == null)
        {
            Console.WriteLine("Angajatul nu exista!");
            return;
        }

        if (!(angajat is SalesAgent))
        {
            Console.WriteLine(angajat.Nume + " nu este SalesAgent!");
            return;
        }

        

        SalesAgent agent = (SalesAgent)angajat;

        Product produs = GasesteProdus(numeProdus);
        if (produs == null)
        {
            Console.WriteLine("Produsul nu exista!");
            return;
        }

        agent.VindeProdus(produs, cantitate);
    }

    // ===== RAPOARTE =====

    public void AfiseazaRaportGeneral()
    {
        Console.WriteLine("\n=== RAPORT: " + Nume + " ===");
        Console.WriteLine("Angajati: " + _nrAngajati);
        Console.WriteLine("Masini:   " + _nrMasini);
        Console.WriteLine("Produse:  " + _nrProduse);
        Console.WriteLine("Comenzi:  " + _nrComenzi);
    }

    public void AfiseazaComenzi()
    {
        if (_nrComenzi == 0)
        {
            Console.WriteLine("Nu exista comenzi!");
            return;
        }

        Console.WriteLine("=== COMENZI ===");
        for (int i = 0; i < _nrComenzi; i++)
        {
            _comenzi[i].Afiseaza();
        }
    }
}
