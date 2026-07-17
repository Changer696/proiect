using SmartFactorySimple;
using System;
using System.Linq;

class Program
{
    static readonly string EmployeesFileName = "employees.txt";
    static readonly string MachinesFileName = "machines.txt";
    static readonly string ProductsFileName = "products.txt";
    static readonly string OrdersFileName = "orders.txt";
    static readonly string OperationsFileName = "operations.txt";

    static Factory fabrica = new Factory("TOYS R US", OrdersFileName);
    static Login.EmployeeCredential loggedInUser;
    static Login loginManager;

    static void Main()
    {
        // Authentication - Login required
        Logging.ConfigureFilePath(OperationsFileName);
        loginManager = new Login(EmployeesFileName);
        loggedInUser = loginManager.LoginWithAttempts(3);

        if (loggedInUser == null)
        {
            return; // Exit if authentication fails
        }

        DateDemo();
        // Load persisted orders from orders.txt if present
        fabrica.LoadOrdersFromFile(OrdersFileName);
        fabrica.IncarcaMasini(MachinesFileName);
        fabrica.IncarcaProduse(ProductsFileName);

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
        fabrica.SalveazaMasini(MachinesFileName);
        fabrica.SalveazaProduse(ProductsFileName);
    }



    static bool MeniuDirector()
    {
        fabrica.AplicaFluctuatiePreturiMeniu();
        fabrica.AfiseazaPreturiStoc();
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
            case "8": MakeCompanyPublic(); break;
            case "9": return Logout();
            case "0": return false;
            default: Console.WriteLine(Messages.InvalidOption); break;
        }
        return true;
    }

    static void MakeCompanyPublic()
    {
        if (loggedInUser == null)
        {
            Console.WriteLine(Messages.NoUserLoggedIn);
            return;
        }

        if (!Enum.TryParse(loggedInUser.Role, out EmployeeRole rolCurent) || rolCurent != EmployeeRole.Director)
        {
            Console.WriteLine(Messages.DirectorOnlyPublicCompany);
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

        fabrica.MakeCompanyPublic(loggedInUser.EmployeeId, percentagePublic, shares, sharePrice);
    }

    static bool MeniuProductionManager()
    {
        fabrica.AplicaFluctuatiePreturiMeniu();
        fabrica.AfiseazaPreturiStoc();
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
        fabrica.AplicaFluctuatiePreturiMeniu();
        fabrica.AfiseazaPreturiStoc();
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
        fabrica.AplicaFluctuatiePreturiMeniu();
        fabrica.AfiseazaPreturiStoc();
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
                    Console.Write(Messages.StopMachinePrompt);
                    string serial = Console.ReadLine();
                    Machine m = fabrica.GasesteMasina(serial);
                    if (m == null)
                        Console.WriteLine(Messages.MachineDoesNotExist);
                    else
                        m.Stop();
                    break;
                }
            case "4":
                {
                    fabrica.AfiseazaMasini();
                    Console.Write(Messages.StartMachinePrompt);
                    string serial = Console.ReadLine();
                    Machine m = fabrica.GasesteMasina(serial);
                    if (m == null)
                        Console.WriteLine(Messages.MachineDoesNotExist);
                    else
                        m.Start();
                    break;
                }
            case "5": fabrica.AfiseazaMentenantaPredictiva(); break;
            case "6": fabrica.AfiseazaStareMasini(); break;
            case "7": return Logout();
            case "0": return false;
            default: Console.WriteLine(Messages.InvalidOption); break;
        }
        return true;
    }

    static bool MeniuMachineOperator()
    {
        fabrica.AplicaFluctuatiePreturiMeniu();
        fabrica.AfiseazaPreturiStoc();
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
            default: Console.WriteLine(Messages.InvalidOption); break;
        }
        return true;
    }

    static bool MeniuSalesAgent()
    {
        fabrica.AplicaFluctuatiePreturiMeniu();
        fabrica.AfiseazaPreturiStoc();
        Console.WriteLine(Messages.SalesAgentMenu);
        Console.Write(Messages.Choose);
        string alegere = Console.ReadLine();

        switch (alegere)
        {
            case "1": MeniuVanzari(); break;
            case "2": fabrica.AfiseazaProduse(); break;
            case "3": return Logout();
            case "0": return false;
            default: Console.WriteLine(Messages.InvalidOption); break;
        }
        return true;
    }

    static void ShowOperationLogs()
    {
        Console.WriteLine(Messages.OperationHistoryTitle);
        string[] entries = Logging.GetAllEntries();
        if (entries.Length == 0)
        {
            Console.WriteLine(Messages.NoOperationLogs);
        }
        else
        {
            entries.ToList().ForEach(e => Console.WriteLine(e));
        }
        Console.WriteLine(Messages.OperationHistoryEnd);
    }

    // Logout and re-authenticate. Returns true to continue running, false to exit application.
    static bool Logout()
    {
        Console.WriteLine(Messages.LoggingOut);
        if (loggedInUser != null)
        {
            Logging.Log(loggedInUser.Username, "User logged out");
        }

        loggedInUser = loginManager.LoginWithAttempts(3);
        if (loggedInUser == null)
        {
            Console.WriteLine(Messages.AuthenticationFailed);
            return false;
        }

        Console.WriteLine(Messages.SuccessfullyLoggedIn(loggedInUser.Username, loggedInUser.Role));
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
            Console.Write(Messages.EmployeeIdPrompt);
            string id = Console.ReadLine();
            fabrica.StergeAngajat(id);
        }
        else if (alegere == "4")
        {
            fabrica.AfiseazaAngajati();
            Console.Write(Messages.IdEmployeePrompt);
            string id = Console.ReadLine();
            Employee ang = fabrica.GasesteAngajat(id);
            if (ang == null)
                Console.WriteLine(Messages.EmployeeDoesNotExist);
            else
                ang.PerformDuty();
        }
    }

    static void AdaugaAngajat()
    {
        Console.Write(Messages.IdPrompt);
        string id = Console.ReadLine();
        if (fabrica.EmployeeIdExists(id))
        {
            Console.WriteLine(Messages.EmployeeIdAlreadyExists(id));
            return;
        }

        Console.Write(Messages.NamePrompt);
        string nume = Console.ReadLine();
        Console.Write(Messages.SalaryPrompt);
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
            Console.WriteLine(Messages.InvalidUser);
            return;
        }

        // Ask for login credentials
        Console.Write(Messages.UsernameForLoginPrompt);
        string username = Console.ReadLine();
        Console.Write(Messages.PasswordForLoginPrompt);
        string password = Console.ReadLine();

        if (fabrica.AdaugaAngajat(angajat))
        {
            // Save credentials to file
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
            Console.WriteLine(Messages.InvalidUser);
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

        if (fabrica.AdaugaMasina(masina))
        {
            Console.WriteLine(Messages.MachineAdded);
            // Save immediately so added machines are persisted even if the program is closed unexpectedly
            fabrica.SalveazaMasini(MachinesFileName);
        }
    }

    static void ReparaMasina()
    {
        fabrica.AfiseazaAngajati();
        Console.Write(Messages.TechnicianIdPrompt);
        string idTeh = Console.ReadLine();
        Console.Write(Messages.EngineerIdPrompt);
        string idEng = Console.ReadLine();

        fabrica.AfiseazaMasini();
        Console.Write(Messages.MachineSerialPrompt);
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
        Console.Write(Messages.ProductNamePrompt);
        string nume = Console.ReadLine();

        Console.Write(Messages.AmountToAddPrompt);
        int cantitate = int.Parse(Console.ReadLine());

        fabrica.AdaugaStocProduse(nume, cantitate);
    }

    static void AdaugaProdus()
    {
        Console.Write(Messages.NamePrompt);
        string nume = Console.ReadLine();
        Console.Write(Messages.ProductionCostPrompt);
        decimal productionCost = decimal.Parse(Console.ReadLine());
        Console.Write(Messages.SellingPricePrompt);
        decimal sellingPrice = decimal.Parse(Console.ReadLine());
        Console.Write(Messages.InitialQuantityPrompt);
        int cantitate = int.Parse(Console.ReadLine());

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
        if (fabrica.AdaugaProdus(produs))
        {
            Console.WriteLine(Messages.ProductAdded);
            // Persist products immediately
            fabrica.SalveazaProduse(ProductsFileName);
        }
    }

    static void VandeProdus()
    {
        fabrica.AfiseazaAngajati();
        Console.Write(Messages.SalesAgentIdPrompt);
        string idAgent = Console.ReadLine();

        fabrica.AfiseazaProduse();
        Console.Write(Messages.ProductToSellPrompt);
        string numeProdus = Console.ReadLine();

        Console.Write(Messages.SellingQuantityPrompt);
        int cantitate = int.Parse(Console.ReadLine());

        fabrica.VindeProdus(idAgent, numeProdus, cantitate);
    }

        // ===== MENIU PRODUCTIE =====

        static void MeniuProductie()
        {
        Console.WriteLine(Messages.ProductionMenu);
        Console.Write(Messages.Choose);
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
            Console.Write(Messages.ProductionManagerIdPrompt);
            string idManager = Console.ReadLine();

            fabrica.AfiseazaMasini();
            Console.Write(Messages.MachineForOrderPrompt);
            string serial = Console.ReadLine();

            Console.Write(Messages.ProductToManufacturePrompt);
            string produs = Console.ReadLine();

            Console.Write(Messages.TargetAmountPrompt);
            int cantitate = int.Parse(Console.ReadLine());

            Console.WriteLine(Messages.PriorityMenu);
            Console.Write(Messages.Choose);
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
            Console.Write(Messages.MachineOperatorIdPrompt);
            string idOp = Console.ReadLine();

            fabrica.AfiseazaComenzi();
            Console.Write(Messages.OrderIdPrompt);
            string idComanda = Console.ReadLine();

            Console.Write(Messages.UnitsToProducePrompt);
            int unitati = int.Parse(Console.ReadLine());

            fabrica.ExecutaComanda(idOp, idComanda, unitati);
        }

        static void ExecutaComanaPrioritara()
        {
            fabrica.AfiseazaAngajati();
            Console.Write(Messages.MachineOperatorIdPrompt);
            string idOp = Console.ReadLine();

            ProductionOrder nextOrder = fabrica.GetNextPriorityOrder(idOp);
            if (nextOrder == null)
            {
                Console.WriteLine(Messages.NoActiveOrder);
                return;
            }

            Console.WriteLine(Messages.NextPriorityOrder);
            nextOrder.Afiseaza();

            Console.Write(Messages.PriorityUnitsPrompt);
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
            Console.Write(Messages.SalesAgentIdPrompt);
            string idAgent = Console.ReadLine();

            Employee ang = fabrica.GasesteAngajat(idAgent);
            if (ang == null || !(ang is SalesAgent))
            {
                Console.WriteLine(Messages.SalesAgentNotFound);
                return;
            }

            SalesAgent agent = (SalesAgent)ang;

            fabrica.AfiseazaProduse();
            Console.Write(Messages.ProductToSellPrompt);
            string produsNume = Console.ReadLine();

            Product produs = fabrica.GasesteProdus(produsNume);
            if (produs == null)
            {
                Console.WriteLine(Messages.ProductNotFound);
                return;
            }

            Console.Write(Messages.QuantityToSellPrompt);
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

            Console.WriteLine(Messages.DemoLoaded);
            Console.ReadLine();
        }
    }
