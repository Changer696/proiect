using SmartFactorySimple;
using System;

class Program
{
    static Factory fabrica = new Factory("Fabrica Sunrise");

    static void Main()
    {
        DateDemo();

        bool running = true;
        while (running)
        {
            Console.WriteLine("\n========== SMART FACTORY ==========");
            Console.WriteLine("1. Angajati");
            Console.WriteLine("2. Masini");
            Console.WriteLine("3. Produse");
            Console.WriteLine("4. Productie");
            Console.WriteLine("5. Raport general");
            Console.WriteLine("0. Iesire");
            Console.Write("Alege: ");

            string alegere = Console.ReadLine();

            if (alegere == "1")
                MeniuAngajati();
            else if (alegere == "2")
                MeniuMasini();
            else if (alegere == "3")
                MeniuProduse();
            else if (alegere == "4")
                MeniuProductie();
            else if (alegere == "5")
                fabrica.AfiseazaRaportGeneral();
            else if (alegere == "0")
                running = false;
            else
                Console.WriteLine("Optiune invalida!");
        }

        Console.WriteLine("La revedere!");
    }

    // ===== MENIU ANGAJATI =====

    static void MeniuAngajati()
    {
        Console.WriteLine("\n--- ANGAJATI ---");
        Console.WriteLine("1. Adauga angajat");
        Console.WriteLine("2. Afiseaza toti");
        Console.WriteLine("3. Sterge angajat");
        Console.WriteLine("4. Angajat isi face datoria");
        Console.Write("Alege: ");
        string alegere = Console.ReadLine();

        if (alegere == "1")
        {
            AdaugaAngajat();
        }
        else if (alegere == "2")
        {
            fabrica.AfiseazaAngajati();
        }
        else if (alegere == "3")
        {
            fabrica.AfiseazaAngajati();
            Console.Write("ID angajat de sters: ");
            string id = Console.ReadLine();
            fabrica.StergeAngajat(id);
        }
        else if (alegere == "4")
        {
            fabrica.AfiseazaAngajati();
            Console.Write("ID angajat: ");
            string id = Console.ReadLine();
            Employee ang = fabrica.GasesteAngajat(id);
            if (ang == null)
                Console.WriteLine("Angajatul nu exista!");
            else
                ang.PerformDuty();
        }
    }

    static void AdaugaAngajat()
    {
        Console.Write("ID: ");
        string id = Console.ReadLine();
        Console.Write("Nume: ");
        string nume = Console.ReadLine();
        Console.Write("Salariu: ");
        decimal salariu = decimal.Parse(Console.ReadLine());

        Console.WriteLine("Tip angajat:");
        Console.WriteLine("1. Director");
        Console.WriteLine("2. ProductionManager");
        Console.WriteLine("3. Engineer");
        Console.WriteLine("4. Technician");
        Console.WriteLine("5. MachineOperator");
        Console.WriteLine("6. SalesAgent");
        Console.Write("Alege: ");
        string tip = Console.ReadLine();

        Employee angajat = null;

        if (tip == "1")
            angajat = new Director(id, nume, salariu, DateTime.Now);
        else if (tip == "2")
            angajat = new ProductionManager(id, nume, salariu, DateTime.Now);
        else if (tip == "3")
            angajat = new Engineer(id, nume, salariu, DateTime.Now);
        else if (tip == "4")
            angajat = new Technician(id, nume, salariu, DateTime.Now);
        else if (tip == "5")
            angajat = new MachineOperator(id, nume, salariu, DateTime.Now);
        else if (tip == "6")
            angajat = new SalesAgent(id, nume, salariu, DateTime.Now);
        else
        {
            Console.WriteLine("Tip invalid!");
            return;
        }

        if (fabrica.AdaugaAngajat(angajat))
            Console.WriteLine("Angajat adaugat cu succes!");
    }

    // ===== MENIU MASINI =====

    static void MeniuMasini()
    {
        Console.WriteLine("\n--- MASINI ---");
        Console.WriteLine("1. Adauga masina");
        Console.WriteLine("2. Afiseaza toate");
        Console.WriteLine("3. Opreste masina");
        Console.WriteLine("4. Repara masina");
        Console.WriteLine("5.Porneste Masina");
        Console.Write("Alege: ");
        string alegere = Console.ReadLine();

        if (alegere == "1")
        {
            AdaugaMasina();
        }
        else if (alegere == "2")
        {
            fabrica.AfiseazaMasini();
        }
        else if (alegere == "3")
        {
            fabrica.AfiseazaMasini();
            Console.Write("Serial masina de oprit: ");
            string serial = Console.ReadLine();
            Machine m = fabrica.GasesteMasina(serial);
            if (m == null)
                Console.WriteLine("Masina nu exista!");
            else
                m.Stop();
        }
        else if (alegere == "4")
        {
            ReparaMasina();
        }
        else if (alegere == "5")
        {
            fabrica.AfiseazaMasini();
            Console.Write("Serial masina de pornit: ");
            string serial = Console.ReadLine();
            Machine m = fabrica.GasesteMasina(serial);
            if (m == null)
                Console.WriteLine("Masina nu exista!");
            else
                m.Start();

        }
    }

    static void AdaugaMasina()
    {
        Console.Write("Serial: ");
        string serial = Console.ReadLine();
        Console.Write("Nume: ");
        string nume = Console.ReadLine();

        Console.WriteLine("Tip masina:");
        Console.WriteLine("1. SewingMachine");
        Console.WriteLine("2. CuttingMachine");
        Console.Write("Alege: ");
        string tip = Console.ReadLine();

        Machine masina = null;

        if (tip == "1")
            masina = new SewingMachine(serial, nume, DateTime.Now);
        else if (tip == "2")
            masina = new CuttingMachine(serial, nume, DateTime.Now);
        else
        {
            Console.WriteLine("Tip invalid!");
            return;
        }

        Console.Write("Adauga piesa? (da/nu): ");
        string raspuns = Console.ReadLine();
        if (raspuns == "da")
        {
            Console.Write("Nume piesa: ");
            string numePiesa = Console.ReadLine();
            Console.Write("Tip piesa (Motor/Needle/Blade): ");
            string tipPiesa = Console.ReadLine();
            masina.AdaugaPiesa(new MachinePart(numePiesa, tipPiesa));
        }

        if (fabrica.AdaugaMasina(masina))
            Console.WriteLine("Masina adaugata cu succes!");
    }

    static void ReparaMasina()
    {
        fabrica.AfiseazaAngajati();
        Console.Write("ID Technician: ");
        string idTeh = Console.ReadLine();
        Console.Write("ID Engineer: ");
        string idEng = Console.ReadLine();

        fabrica.AfiseazaMasini();
        Console.Write("Serial masina: ");
        string serial = Console.ReadLine();

        fabrica.ReparaMasina(idTeh, idEng, serial);
    }

    // ===== MENIU PRODUSE =====

    static void MeniuProduse()
    {
        Console.WriteLine("\n--- PRODUSE ---");
        Console.WriteLine("1. Adauga produs");
        Console.WriteLine("2. Afiseaza toate");
        Console.WriteLine("3. Adauga Stoc");
        Console.WriteLine("4. Vinde produs");
        Console.Write("Alege: ");
        string alegere = Console.ReadLine();

        if (alegere == "1")
            AdaugaProdus();
        else if (alegere == "2")
            fabrica.AfiseazaProduse();
        else if (alegere == "3")
            AdaugaStocProdus();
        else if (alegere == "4")
            VandeProdus();


        static void AdaugaStocProdus()
        {
            fabrica.AfiseazaProduse();
            Console.Write("Nume produs: ");
            string nume = Console.ReadLine();

            Console.Write("Cantitate de adaugat: ");
            int cantitate = int.Parse(Console.ReadLine());

            fabrica.AdaugaStocProduse(nume, cantitate);
        }

    }

    static void AdaugaProdus()
    {
        Console.Write("Nume: ");
        string nume = Console.ReadLine();
        Console.Write("Pret de Productie: ");
        decimal productionCost = decimal.Parse(Console.ReadLine());
        Console.Write("Pret de Vanzare: ");
        decimal sellingPrice = decimal.Parse(Console.ReadLine());
        Console.Write("Cantitate initiala: ");
        int cantitate = int.Parse(Console.ReadLine());

        Console.WriteLine("Tip produs:");
        Console.WriteLine("1. Wooden Cubes");
        Console.WriteLine("2. Teddy Bear");
        Console.WriteLine("3. FootBall");
        Console.WriteLine("4. Doll");
        Console.WriteLine("5. Frisbee");

        Console.Write("Alege: ");
        string tip = Console.ReadLine();

        Product produs = null;

        if (tip == "1")
        {
            Console.Write("Marime: ");
            string marime = Console.ReadLine();
            produs = new WoodenCubes(nume, productionCost, sellingPrice, cantitate, marime);
        }
        else if (tip == "2")
        {
            Console.Write("Marime: ");
            string marime = Console.ReadLine();
            produs = new Doll(nume, productionCost, sellingPrice, cantitate, marime);
        }
        else if (tip == "3")
        {
            Console.Write("Marime: ");
            string marime = Console.ReadLine();
            produs = new TedyBear(nume, productionCost, sellingPrice, cantitate, marime);
        }
        else if (tip == "4")
        {
            Console.Write("Marime: ");
            string marime = Console.ReadLine();
            produs = new Ball(nume, productionCost, sellingPrice, cantitate, marime);
        }
        else if (tip == "5")
        {
            Console.Write("Marime: ");
            string marime = Console.ReadLine();
            produs = new Frisbee(nume, productionCost, sellingPrice, cantitate, marime);
        }
        if (fabrica.AdaugaProdus(produs))
            Console.WriteLine("Produs adaugat cu succes!");
    }

    static void VandeProdus()
    {
        fabrica.AfiseazaAngajati();
        Console.Write("ID Sales Agent: ");
        string idAgent = Console.ReadLine();

        fabrica.AfiseazaProduse();
        Console.Write("Nume produs: ");
        string numeProdus = Console.ReadLine();

        Console.Write("Cantitate de vandut: ");
        int cantitate = int.Parse(Console.ReadLine());

        fabrica.VandeProdus(idAgent, numeProdus, cantitate);
    }

    // ===== MENIU PRODUCTIE =====

    static void MeniuProductie()
    {
        Console.WriteLine("\n--- PRODUCTIE ---");
        Console.WriteLine("1. Creeaza comanda");
        Console.WriteLine("2. Execute Order (manualy)");
        Console.WriteLine("3. Execute the next priority order (auto)");
        Console.WriteLine("4. Show orders");
        Console.WriteLine("5. Show orders sorted by priority");
        Console.Write("Alege: ");
        string alegere = Console.ReadLine();

        if (alegere == "1")
            CreazaComanda();
        else if (alegere == "2")
            ExecutaComanda();
        else if (alegere == "3")
            ExecutaComanaPrioritara();
        else if (alegere == "4")
            fabrica.AfiseazaComenzi();
        else if (alegere == "5")
            fabrica.AfiseazaComenziSortedByPriority();
    }

    static void CreazaComanda()
    {
        fabrica.AfiseazaAngajati();
        Console.Write("ID Production Manager: ");
        string idManager = Console.ReadLine();

        fabrica.AfiseazaMasini();
        Console.Write("Serial masina: ");
        string serial = Console.ReadLine();

        Console.Write("Nume produs de fabricat: ");
        string produs = Console.ReadLine();

        Console.Write("Cantitate target: ");
        int cantitate = int.Parse(Console.ReadLine());

        Console.WriteLine("Prioritate: 1.Low  2.Medium  3.High");
        Console.Write("Alege: ");
        string prio = Console.ReadLine();

        Priority prioritate;
        if (prio == "1")
            prioritate = Priority.Low;
        else if (prio == "3")
            prioritate = Priority.High;
        else
            prioritate = Priority.Medium;

        fabrica.CreazaComanda(idManager, serial, produs, cantitate, prioritate);
    }

    static void ExecutaComanda()
    {
        fabrica.AfiseazaAngajati();
        Console.Write("ID MachineOperator: ");
        string idOp = Console.ReadLine();

        fabrica.AfiseazaComenzi();
        Console.Write("ID Comanda (ex: ORD1): ");
        string idComanda = Console.ReadLine();

        Console.Write("Unitati de produs acum: ");
        int unitati = int.Parse(Console.ReadLine());

        fabrica.ExecutaComanda(idOp, idComanda, unitati);
    }

    static void ExecutaComanaPrioritara()
    {
        fabrica.AfiseazaAngajati();
        Console.Write("ID MachineOperator: ");
        string idOp = Console.ReadLine();

        ProductionOrder nextOrder = fabrica.GetNextPriorityOrder(idOp);
        if (nextOrder == null)
        {
            Console.WriteLine("Nu exista comenzi active sau operatorul nu este valid!");
            return;
        }

        Console.WriteLine("\nUrmatoarea comanda prioritara:");
        nextOrder.Afiseaza();

        Console.Write("Unitati de produs acum: ");
        int unitati = int.Parse(Console.ReadLine());

        fabrica.ExecutaComanda(idOp, nextOrder.Id, unitati);
    }

    // ===== DATE DEMO =====

    static void DateDemo()
    {
        fabrica.AdaugaAngajat(new Director("DIR001", "Alex Popescu", 8000, DateTime.Now.AddYears(-5)));
        fabrica.AdaugaAngajat(new ProductionManager("PM001", "Maria Ionescu", 5500, DateTime.Now.AddYears(-3)));
        fabrica.AdaugaAngajat(new Engineer("ENG001", "Ion Vasile", 5000, DateTime.Now.AddYears(-2)));
        fabrica.AdaugaAngajat(new Technician("TH001", "Andrei Marin", 4000, DateTime.Now.AddYears(-1)));
        fabrica.AdaugaAngajat(new MachineOperator("OP001", "Elena Dumitru", 3500, DateTime.Now.AddMonths(-8)));
        fabrica.AdaugaAngajat(new SalesAgent("SA001", "Ioana Radu", 3300, DateTime.Now.AddMonths(-4)));

        SewingMachine s1 = new SewingMachine("M001", "Juki Sewing", DateTime.Now.AddYears(-3));
        s1.AdaugaPiesa(new MachinePart("Ac Industrial", "Needle"));
        s1.AdaugaPiesa(new MachinePart("Ata Poliester", "Thread"));
        fabrica.AdaugaMasina(s1);

        CuttingMachine c1 = new CuttingMachine("M002", "Auto Cutter", DateTime.Now.AddYears(-2));
        c1.AdaugaPiesa(new MachinePart("Lama Otel", "Blade"));
        fabrica.AdaugaMasina(c1);

        fabrica.AdaugaProdus(new WoodenCubes("MagicCubes", 15, 30, 3, "S"));
        fabrica.AdaugaProdus(new Doll("Barbie", 12, 50, 7, "S"));
        fabrica.AdaugaProdus(new TedyBear("Barnie", 20, 60, 15, "M"));
        fabrica.AdaugaProdus(new Ball("Minge de Fotball",13,50,5,"Normal"));
        fabrica.AdaugaProdus(new Frisbee("Frisbee",10, 25, 7, "S"));

        Console.WriteLine("Date demo incarcate! Apasa Enter pentru a continua...");
        Console.ReadLine();
    }
}
