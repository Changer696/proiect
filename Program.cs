using SmartFactorySimple;
using System;

class Program
{
    static Factory fabrica = new Factory("TOYS R US");
    static Login.EmployeeCredential loggedInUser;
    static Login loginManager;

    static void Main()
    {
        // Authentication - Login required
        loginManager = new Login();
        loggedInUser = loginManager.LoginWithAttempts(3);

        if (loggedInUser == null)
        {
            return; // Exit if authentication fails
        }

        DateDemo();
        // Load persisted orders from orders.txt if present
        fabrica.LoadOrdersFromFile();
        fabrica.IncarcaMasini();
        fabrica.IncarcaProduse();

        bool running = true;
        while (running)
        {
            Console.WriteLine(Messages.MainTitle);
            Console.WriteLine(Messages.LoggedInAs(loggedInUser.Username, loggedInUser.Role));
            EmployeeRole rolCurent;
            if (!Enum.TryParse(loggedInUser.Role, out rolCurent))
            {
                Console.WriteLine(Messages.UnknownRoleInDatabase);
                return;
            }


            switch (rolCurent)
            {
                case EmployeeRole.Director:
                    running = MeniuDirector();
                    break;
                case EmployeeRole.ProductionManager:
                    running = MeniuProductionManager();
                    break;
                case EmployeeRole.Engineer:
                    running = MeniuEngineer();
                    break;
                case EmployeeRole.Technician:
                    running = MeniuTechnician();
                    break;
                case EmployeeRole.MachineOperator:
                    running = MeniuMachineOperator();
                    break;
                case EmployeeRole.SalesAgent:
                    running = MeniuSalesAgent();
                    break;
                default:
                    Console.WriteLine(Messages.UnknownRole);
                    running = false;
                    break;
            }
        }

        Console.WriteLine(Messages.Goodbye);
        Console.WriteLine(Messages.SavingData);
        fabrica.SalveazaMasini();
        fabrica.SalveazaProduse();
    }



    static bool MeniuDirector()
    {
        Console.WriteLine(Messages.DirectorMenu);
        Console.Write(Messages.Choose);
        string alegere = Console.ReadLine();

        switch (alegere)
        {
            case "1": MeniuAngajati(); break;
            case "2":
                fabrica.AfiseazaMasini();
                Console.WriteLine(Messages.ViewOnlyMachines);
                break;
            case "3":
                fabrica.AfiseazaProduse();
                Console.WriteLine(Messages.ViewOnlyProducts);
                break;
            case "4":
                Console.WriteLine(Messages.DirectorProductionMenu);
                string choose = Console.ReadLine();
                switch (choose)
                {
                    case "1":fabrica.AfiseazaComenzi(); break;
                    case "2":fabrica.AfiseazaComenziSortedByPriority();break;
                    case "3":fabrica.AfiseazaDashboardEficienta(); break;
                    default: Console.WriteLine(Messages.ChooseOneOption);break;
                }

                break;
            case "5":
                Console.WriteLine(Messages.DirectorSalesMenu);
                string opt = Console.ReadLine();
                switch (opt)
                {
                    case "1": fabrica.AfiseazaRaportVanzari(); break;
                    case "2": fabrica.AfiseazaRaportGeneral(); break;
                    default: Console.WriteLine(Messages.ChooseOneOption); break;
                }

                break;
            case "6": fabrica.AfiseazaRaportGeneral(); break;
            case "7": ShowOperationLogs(); break;
            case "8": return Logout();
            case "0": return false;
            default: Console.WriteLine(Messages.InvalidOption); break;
        }
        return true;
    }

    static bool MeniuProductionManager()
    {
        Console.WriteLine(Messages.ProductionManagerMenu);
        Console.Write(Messages.Choose);
        string alegere = Console.ReadLine();

        switch (alegere)
        {
            case "1": fabrica.AfiseazaAngajati(); break;
            case "2": MeniuMasiniProductionManager(); break;
            case "3": MeniuProduse(); break;
            case "4": MeniuProductie(); break;
            case "5": fabrica.AfiseazaRaportGeneral(); break;
            case "6": return Logout();
            case "0": return false;
            default: Console.WriteLine(Messages.InvalidOption); break;
        }
        return true;
    }
    static void MeniuMasiniProductionManager()
    {
        Console.WriteLine(Messages.ProductionManagerMachinesMenu);
        Console.Write(Messages.Choose);
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
            Console.Write(Messages.StopMachinePrompt);
            string serial = Console.ReadLine();
            Machine m = fabrica.GasesteMasina(serial);
            if (m == null)
                Console.WriteLine(Messages.MachineDoesNotExist);
            else
                m.Stop();
        }
        else if (alegere == "4")
        {
            fabrica.AfiseazaMasini();
            Console.Write(Messages.StartMachinePrompt);
            string serial = Console.ReadLine();
            Machine m = fabrica.GasesteMasina(serial);
            if (m == null)
                Console.WriteLine(Messages.MachineDoesNotExist);
            else
                m.Start();
        }
    }
    static bool MeniuEngineer()
    {
        Console.WriteLine(Messages.EngineerMenu);
        Console.Write(Messages.Choose);
        string alegere = Console.ReadLine();

        switch (alegere)
        {
            case "1": fabrica.AfiseazaMasini(); break;
            case "2": ReparaMasina(); break;
            case "3": fabrica.AfiseazaMentenantaPredictiva(); break;
            case "4": fabrica.AfiseazaStareMasini(); break;
            case "5": return Logout();
            case "0": return false;
            default: Console.WriteLine(Messages.InvalidOption); break;
        }
        return true;
    }

    static bool MeniuTechnician()
    {
        Console.WriteLine(Messages.TechnicianMenu);
        Console.Write(Messages.Choose);
        string alegere = Console.ReadLine();

        switch (alegere)
        {
            case "1":
                fabrica.AfiseazaMasini();
                break;
            case "2":
                ReparaMasina();
                break;
            case "3":
                {
                    fabrica.AfiseazaMasini();
                    Console.Write("Serial number for the machine you want to stop: ");
                    string serial = Console.ReadLine();
                    Machine m = fabrica.GasesteMasina(serial);
                    if (m == null)
                        Console.WriteLine("Machine doesn't exist!");
                    else
                        m.Stop();
                    break;
                }
            case "4":
                {
                    fabrica.AfiseazaMasini();
                    Console.Write("Serial number for the machine you want to start: ");
                    string serial = Console.ReadLine();
                    Machine m = fabrica.GasesteMasina(serial);
                    if (m == null)
                        Console.WriteLine("Machine doesn't exist!");
                    else
                        m.Start();
                    break;
                }
            case "5": fabrica.AfiseazaMentenantaPredictiva(); break;
            case "6": fabrica.AfiseazaStareMasini(); break;
            case "7": return Logout();
            case "0": return false;
            default: Console.WriteLine("Invalid option!"); break;
        }
        return true;
    }

    static bool MeniuMachineOperator()
    {
        Console.WriteLine(Messages.MachineOperatorMenu);
        Console.Write(Messages.Choose);
        string alegere = Console.ReadLine();

        switch (alegere)
        {
            case "1": MeniuProductie(); break;
            case "2": fabrica.AfiseazaMasini(); break;
            case "3": fabrica.AfiseazaMentenantaPredictiva(); break;
            case "4": fabrica.AfiseazaStareMasini(); break;
            case "5": return Logout();
            case "0": return false;
            default: Console.WriteLine("Invalid option!"); break;
        }
        return true;
    }

    static bool MeniuSalesAgent()
    {
        Console.WriteLine(Messages.SalesAgentMenu);
        Console.Write(Messages.Choose);
        string alegere = Console.ReadLine();

        switch (alegere)
        {
            case "1": MeniuVanzari(); break;
            case "2": fabrica.AfiseazaProduse(); break;
            case "3": return Logout();
            case "0": return false;
            default: Console.WriteLine("Invalid option!"); break;
        }
        return true;
    }

    static void ShowOperationLogs()
    {


        Console.WriteLine("\n=== Operation History ===");
        string[] entries = Logging.GetAllEntries();
        if (entries.Length == 0)
        {
            Console.WriteLine("No operation logs available.");
        }
        else
        {
            foreach (string entry in entries)
            {
                Console.WriteLine(entry);
            }
        }
        Console.WriteLine("=========================");
    }

    // Logout and re-authenticate. Returns true to continue running, false to exit application.
    static bool Logout()
    {
        Console.WriteLine("\nLogging out...");
        if (loggedInUser != null)
        {
            Logging.Log(loggedInUser.Username, "User logged out");
        }

        loggedInUser = loginManager.LoginWithAttempts(3);
        if (loggedInUser == null)
        {
            Console.WriteLine("Authentication failed. Exiting application.");
            return false;
        }

        Console.WriteLine($"Successfully logged in as: {loggedInUser.Username} ({loggedInUser.Role})");
        return true;
    }

    // ===== MENIU ANGAJATI =====

    static void MeniuAngajati()
    {
        Console.WriteLine(Messages.EmployeesMenu);
        Console.Write(Messages.Choose);
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
            Console.Write("Employee ID to delete: ");
            string id = Console.ReadLine();
            fabrica.StergeAngajat(id);
        }
        else if (alegere == "4")
        {
            fabrica.AfiseazaAngajati();
            Console.Write("ID employee: ");
            string id = Console.ReadLine();
            Employee ang = fabrica.GasesteAngajat(id);
            if (ang == null)
                Console.WriteLine("Employee doesn't exist!");
            else
                ang.PerformDuty();
        }
    }

    static void AdaugaAngajat()
    {
        Console.Write("ID: ");
        string id = Console.ReadLine();
        if (fabrica.EmployeeIdExists(id))
        {
            Console.WriteLine($"Employee ID {id} already exists. Please choose a unique ID.");
            return;
        }

        Console.Write("Name: ");
        string nume = Console.ReadLine();
        Console.Write("Salary: ");
        decimal salariu = decimal.Parse(Console.ReadLine());

        Console.WriteLine(Messages.EmployeeTypesMenu);
        Console.Write(Messages.Choose);
        string tip = Console.ReadLine();

        Employee angajat = null;
        string role = null;

        if (tip == "1")
        {
            angajat = new Director(id, nume, salariu, DateTime.Now);
            role = "Director";
        }
        else if (tip == "2")
        {
            angajat = new ProductionManager(id, nume, salariu, DateTime.Now);
            role = "ProductionManager";
        }
        else if (tip == "3")
        {
            angajat = new Engineer(id, nume, salariu, DateTime.Now);
            role = "Engineer";
        }
        else if (tip == "4")
        {
            angajat = new Technician(id, nume, salariu, DateTime.Now);
            role = "Technician";
        }
        else if (tip == "5")
        {
            angajat = new MachineOperator(id, nume, salariu, DateTime.Now);
            role = "MachineOperator";
        }
        else if (tip == "6")
        {
            angajat = new SalesAgent(id, nume, salariu, DateTime.Now);
            role = "SalesAgent";
        }
        else
        {
            Console.WriteLine("Invalid user!");
            return;
        }

        // Ask for login credentials
        Console.Write("Username for login: ");
        string username = Console.ReadLine();
        Console.Write("Password for login: ");
        string password = Console.ReadLine();

        if (fabrica.AdaugaAngajat(angajat))
        {
            // Save credentials to file
            if (loginManager.SaveEmployeeCredential(id, username, password, role))
            {
                Console.WriteLine("Employee added successfully!");
            }
            else
            {
                Console.WriteLine("Employee added but failed to save credentials!");
            }
        }
    }

    // ===== MENIU MASINI =====

    static void MeniuMasini()
    {
        Console.WriteLine(Messages.MachinesMenu);
        Console.Write(Messages.Choose);
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
            Console.Write(Messages.StopMachinePrompt);
            string serial = Console.ReadLine();
            Machine m = fabrica.GasesteMasina(serial);
            if (m == null)
                Console.WriteLine(Messages.MachineDoesNotExist);
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
            Console.Write(Messages.StartMachinePrompt);
            string serial = Console.ReadLine();
            Machine m = fabrica.GasesteMasina(serial);
            if (m == null)
                Console.WriteLine(Messages.MachineDoesNotExist);
            else
                m.Start();
        }
        else if (alegere == "6")
        {
            fabrica.AfiseazaMentenantaPredictiva();
        }
        else if (alegere == "7")
        {
            fabrica.AfiseazaDashboardEficienta();
        }
        else if (alegere == "8")
        {
            fabrica.AfiseazaStareMasini();
        }
    }

    static void AdaugaMasina()
    {
        Console.Write("Serial number: ");
        string serial = Console.ReadLine();
        Console.Write("Name: ");
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
            Console.WriteLine("Invalid user!");
            return;
        }

        Console.Write("Add a part? (yes/no): ");
        string raspuns = Console.ReadLine();
        if (raspuns == "yes")
        {
            Console.Write("Part name: ");
            string numePiesa = Console.ReadLine();
            Console.Write("Type of part (Engine/Needle/Blade): ");
            string tipPiesa = Console.ReadLine();
            masina.AdaugaPiesa(new MachinePart(numePiesa, tipPiesa));
        }

        if (fabrica.AdaugaMasina(masina))
        {
            Console.WriteLine("Car added successfully!");
            // Save immediately so added machines are persisted even if the program is closed unexpectedly
            fabrica.SalveazaMasini();
        }
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
        Console.WriteLine(Messages.ProductsMenu);
        Console.Write(Messages.Choose);
        string alegere = Console.ReadLine();

        if (alegere == "1")
            AdaugaProdus();
        else if (alegere == "2")
            fabrica.AfiseazaProduse();
        else if (alegere == "3")
            AdaugaStocProdus();
        else if (alegere == "4")
            VandeProdus();
        else if (alegere == "5")
            fabrica.AfiseazaDashboardEficienta();
        else if (alegere == "6")
            fabrica.AfiseazaAlerteInventar();
    }

    static void AdaugaStocProdus()
    {
        fabrica.AfiseazaProduse();
        Console.Write("Product name: ");
        string nume = Console.ReadLine();

            Console.Write("Amount to add: ");
            int cantitate = int.Parse(Console.ReadLine());

            fabrica.AdaugaStocProduse(nume, cantitate);
        }

        static void AdaugaProdus()
        {
            Console.Write("Name: ");
            string nume = Console.ReadLine();
            Console.Write("Production Cost: ");
            decimal productionCost = decimal.Parse(Console.ReadLine());
            Console.Write("Selling Price: ");
            decimal sellingPrice = decimal.Parse(Console.ReadLine());
            Console.Write("Initial Quantity: ");
            int cantitate = int.Parse(Console.ReadLine());

        Console.WriteLine(Messages.ProductTypesMenu);
        Console.Write(Messages.ChooseRomanian);
            string tip = Console.ReadLine();

            Product produs = null;

            if (tip == "1")
            {
                Console.Write("Size: ");
                string marime = Console.ReadLine();
                produs = new WoodenCubes(nume, productionCost, sellingPrice, cantitate, marime);
            }
            else if (tip == "2")
            {
                Console.Write("Size: ");
                string marime = Console.ReadLine();
                produs = new Doll(nume, productionCost, sellingPrice, cantitate, marime);
            }
            else if (tip == "3")
            {
                Console.Write("Size: ");
                string marime = Console.ReadLine();
                produs = new TedyBear(nume, productionCost, sellingPrice, cantitate, marime);
            }
            else if (tip == "4")
            {
                Console.Write("Size: ");
                string marime = Console.ReadLine();
                produs = new Ball(nume, productionCost, sellingPrice, cantitate, marime);
            }
            else if (tip == "5")
            {
                Console.Write("Size: ");
                string marime = Console.ReadLine();
                produs = new Frisbee(nume, productionCost, sellingPrice, cantitate, marime);
            }
            if (fabrica.AdaugaProdus(produs))
            {
                Console.WriteLine("Product added successfully!");
                // Persist products immediately
                fabrica.SalveazaProduse();
            }
        }

        static void VandeProdus()
        {
            fabrica.AfiseazaAngajati();
            Console.Write("ID Sales Agent: ");
            string idAgent = Console.ReadLine();

            fabrica.AfiseazaProduse();
            Console.Write("Product Name: ");
            string numeProdus = Console.ReadLine();

            Console.Write("Selling Quantity: ");
            int cantitate = int.Parse(Console.ReadLine());

            fabrica.VindeProdus(idAgent, numeProdus, cantitate);
        }

        // ===== MENIU PRODUCTIE =====

        static void MeniuProductie()
        {
        Console.WriteLine(Messages.ProductionMenu);
        Console.Write(Messages.ChooseRomanian);
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
            Console.Write("Serial number for a machine: ");
            string serial = Console.ReadLine();

            Console.Write("Product name to manufacture: ");
            string produs = Console.ReadLine();

            Console.Write("Target amount: ");
            int cantitate = int.Parse(Console.ReadLine());

            Console.WriteLine("Priority: 1.Low  2.Medium  3.High");
            Console.Write("Choose: ");
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
            Console.Write("ID Order (ex: ORD1): ");
            string idComanda = Console.ReadLine();

            Console.Write("Units to prodce now: ");
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

        static void MeniuVanzari()
        {
        Console.WriteLine(Messages.SalesMenu);
        Console.Write(Messages.Choose);
            string alegere = Console.ReadLine();

            if (alegere == "1")
                VindeProdus();
            else if (alegere == "2")
                fabrica.AfiseazaRaportVanzari();
            else if (alegere == "3")
                fabrica.AfiseazaRaportGeneral();
        }

        static void VindeProdus()
        {
            fabrica.AfiseazaAngajati();
            Console.Write("ID SalesAgent: ");
            string idAgent = Console.ReadLine();

            Employee ang = fabrica.GasesteAngajat(idAgent);
            if (ang == null || !(ang is SalesAgent))
            {
                Console.WriteLine("Sales Agent not found!");
                return;
            }

            SalesAgent agent = (SalesAgent)ang;

            fabrica.AfiseazaProduse();
            Console.Write("Product name to sell: ");
            string produsNume = Console.ReadLine();

            Product produs = fabrica.GasesteProdus(produsNume);
            if (produs == null)
            {
                Console.WriteLine("Product not found!");
                return;
            }

            Console.Write("Quantity to sell: ");
            int cantitate = int.Parse(Console.ReadLine());

            agent.VindeProdus(produs, cantitate, fabrica);
        }

        static void DateDemo()
        {
            fabrica.AdaugaAngajat(new Director("DIR001", "Alex Popescu", 8000, DateTime.Now.AddYears(-5)));
            fabrica.AdaugaAngajat(new ProductionManager("PM001", "Maria Ionescu", 5500, DateTime.Now.AddYears(-3)));
            fabrica.AdaugaAngajat(new Engineer("ENG001", "Ion Vasile", 5000, DateTime.Now.AddYears(-2)));
            fabrica.AdaugaAngajat(new Technician("TH001", "Andrei Marin", 4000, DateTime.Now.AddYears(-1)));
            fabrica.AdaugaAngajat(new MachineOperator("OP001", "Elena Dumitru", 3500, DateTime.Now.AddMonths(-8)));
            fabrica.AdaugaAngajat(new SalesAgent("SA001", "Ioana Radu", 3300, DateTime.Now.AddMonths(-4)));

            SewingMachine s1 = new SewingMachine("M001", "Juki Sewing", DateTime.Now.AddYears(-3));
            s1.AdaugaPiesa(new MachinePart("Industrial Needle", "Needle"));
            s1.AdaugaPiesa(new MachinePart("Polyester Thread", "Thread"));
            fabrica.AdaugaMasina(s1);

            CuttingMachine c1 = new CuttingMachine("M002", "Auto Cutter", DateTime.Now.AddYears(-2));
            c1.AdaugaPiesa(new MachinePart("Steel Blade", "Blade"));
            fabrica.AdaugaMasina(c1);

            fabrica.AdaugaProdus(new WoodenCubes("MagicBlocks", 15, 30, 3, "S"));
            fabrica.AdaugaProdus(new Doll("Barbie", 12, 50, 7, "S"));
            fabrica.AdaugaProdus(new TedyBear("Barnie", 20, 60, 15, "M"));
            fabrica.AdaugaProdus(new Ball("Football", 13, 50, 5, "Normal"));
            fabrica.AdaugaProdus(new Frisbee("OZN", 10, 25, 7, "S"));

            Console.WriteLine("Demo data loaded! Press Enter to continue...");
            Console.ReadLine();
        }
    }
