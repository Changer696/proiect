using System;

/// <summary>
/// Central place for text shown to the user by the console application.
/// </summary>
public static class Messages
{
    public const string Choose = "Choose: ";
    public const string InvalidOption = "Invalid option!";
    public const string EmployeeDoesNotExist = "Employee doesn't exist!";
    public const string MachineDoesNotExist = "Machine doesn't exist!";
    public const string ProductDoesNotExist = "Product doesn't exist!";
    public const string NoMachines = "There are no machines!";
    public const string NoEmployees = "There are no employees!";
    public const string NoProducts = "There are no products!";
    public const string NoOrders = "There are no orders!";
    public const string InsufficientStock = "Insufficient stock!";
    public const string QuantityCannotBeNegative = "The quantity can't be negative!";
    public const string SellingPriceCannotBeNegative = "The selling price can't be negative.";
    public const string ProductionCostCannotBeNegative = "The production cost can't be negative.";
    public const string LoginTitle = "\n========== SMART FACTORY LOGIN ==========";
    public const string UsernamePrompt = "Username: ";
    public const string PasswordPrompt = "Password: ";
    public const string InvalidCredentials = "\nInvalid username or password!\n";
    public const string LoginFailed = "Login failed after maximum attempts. Exiting...";
    public const string UnknownUser = "unknown";
    public const string MainTitle = "\n========== SMART FACTORY ===========";
    public const string Goodbye = "Good Bye!";

    public static readonly string DirectorMenu = string.Join(Environment.NewLine,
        "1. Employees", "2. Machines", "3. Products", "4. Production", "5. Sales",
        "6. General Report", "7. Show Operation Logs", "8. Log out", "0. Exit");
    public static readonly string ProductionMenu = string.Join(Environment.NewLine,
        "\n--- PRODUCTIE ---", "1. Creeaza comanda", "2. Execute Order (manualy)",
        "3. Execute the next priority order (auto)", "4. Show orders", "5. Show orders sorted by priority");
    public static readonly string SalesMenu = string.Join(Environment.NewLine,
        "\n--- SALES ---", "1. Sell Product", "2. View Sales Report", "3. View General Report");
    public static readonly string ProductionManagerMenu = string.Join(Environment.NewLine,
        "1. Show all employees", "2. Machines", "3. Products", "4. Production", "5. General Report", "6. Log out", "0. Exit");
    public static readonly string ProductionManagerMachinesMenu = string.Join(Environment.NewLine,
        "\n--- MACHINES ---", "1. Add a machine", "2. Show all", "3. Stop a machine", "4. Start a machine");
    public static readonly string EngineerMenu = string.Join(Environment.NewLine,
        "1. Show all machines", "2. Repare a machine", "3.Show when a machine may require maintenance ", "4.Machine health monitoring", "5. Log out", "0. Exit");
    public static readonly string TechnicianMenu = string.Join(Environment.NewLine,
        "1. Show all machines", "2. Repare a machine", "3. Stop a machine", "4. Start a machine", "5.Show when a machine may require maintenance ", "6.Machine health monitoring", "7. Log out", "0. Exit");
    public static readonly string MachineOperatorMenu = string.Join(Environment.NewLine,
        "1. Production", "2. Show all machines", "3.Show when a machine may require maintenance ", "4.Machine health monitoring", "5. Log out", "0. Exit");
    public static readonly string SalesAgentMenu = string.Join(Environment.NewLine,
        "1. Sales", "2. Show all products", "3. Log out", "0. Exit");
    public static readonly string EmployeesMenu = string.Join(Environment.NewLine,
        "\n--- Employees ---", "1. Add employee", "2. Show all employees", "3. Delete employee", "4. The employee is doing their duty");
    public static readonly string MachinesMenu = string.Join(Environment.NewLine,
        "\n--- MACHINES ---", "1. Add a machine", "2. Show all", "3. Stop a machine", "4. Repare a machine", "5. Start a machine", "6. Predictive maintenance", "7. Production efficiency dashboard", "8. Machine health monitoring");
    public static readonly string ProductsMenu = string.Join(Environment.NewLine,
        "\n--- PRODUCTS ---", "1. Add a product", "2. Show all products", "3. Add Stock ", "4. Sell a product", "5.Show production efficiency dashboard", "6. Inventory alerts");
    public static readonly string EmployeeTypesMenu = string.Join(Environment.NewLine,
        "Employee Type:", "1. Director", "2. ProductionManager", "3. Engineer", "4. Technician", "5. MachineOperator", "6. SalesAgent");
    public static readonly string MachineTypesMenu = string.Join(Environment.NewLine,
        "Machine type:", "1. SewingMachine", "2. CuttingMachine");
    public static readonly string ProductTypesMenu = string.Join(Environment.NewLine,
        "Product Type:", "1. Wooden Cubes", "2. Teddy Bear", "3. FootBall", "4. Doll", "5. Frisbee");
    public static readonly string DirectorProductionMenu = string.Join(Environment.NewLine,
        "Production", "1.Show Orders", "2.Show Orders by priority", "3.Show production efficiency dashboard");
    public static readonly string DirectorSalesMenu = string.Join(Environment.NewLine,
        "Sales", "1.View Sales Report", "2.View General Report");

    public const string UnknownRoleInDatabase = "Rol necunoscut in baza de date!";
    public const string UnknownRole = "Rol necunoscut!";
    public const string ViewOnlyMachines = "He can only see the machines";
    public const string ViewOnlyProducts = "He can only see the products";
    public const string ChooseOneOption = "Choose one of this options";
    public const string StopMachinePrompt = "Serial number for the machine you want to stop: ";
    public const string StartMachinePrompt = "Serial number for the machine you want to start: ";
    public const string OperationHistoryTitle = "\n=== Operation History ===";
    public const string NoOperationLogs = "No operation logs available.";
    public const string OperationHistoryEnd = "=========================";
    public const string LoggingOut = "\nLogging out...";
    public const string AuthenticationFailed = "Authentication failed. Exiting application.";
    public const string EmployeeIdPrompt = "Employee ID to delete: ";
    public const string IdEmployeePrompt = "ID employee: ";
    public const string IdPrompt = "ID: ";
    public const string NamePrompt = "Name: ";
    public const string SalaryPrompt = "Salary: ";
    public const string UsernameForLoginPrompt = "Username for login: ";
    public const string PasswordForLoginPrompt = "Password for login: ";
    public const string InvalidUser = "Invalid user!";
    public const string EmployeeAdded = "Employee added successfully!";
    public const string EmployeeAddedCredentialsFailed = "Employee added but failed to save credentials!";
    public const string SerialNumberPrompt = "Serial number: ";
    public const string AddPartPrompt = "Add a part? (yes/no): ";
    public const string PartNamePrompt = "Part name: ";
    public const string PartTypePrompt = "Type of part (Engine/Needle/Blade): ";
    public const string MachineAdded = "Car added successfully!";
    public const string TechnicianIdPrompt = "ID Technician: ";
    public const string EngineerIdPrompt = "ID Engineer: ";
    public const string MachineSerialPrompt = "Serial masina: ";
    public const string ProductNamePrompt = "Product name: ";
    public const string AmountToAddPrompt = "Amount to add: ";
    public const string ProductionCostPrompt = "Production Cost: ";
    public const string SellingPricePrompt = "Selling Price: ";
    public const string InitialQuantityPrompt = "Initial Quantity: ";
    public const string SizePrompt = "Size: ";
    public const string ProductAdded = "Product added successfully!";
    public const string SalesAgentIdPrompt = "ID Sales Agent: ";
    public const string SellingQuantityPrompt = "Selling Quantity: ";
    public const string ProductionManagerIdPrompt = "ID Production Manager: ";
    public const string MachineForOrderPrompt = "Serial number for a machine: ";
    public const string ProductToManufacturePrompt = "Product name to manufacture: ";
    public const string TargetAmountPrompt = "Target amount: ";
    public const string PriorityMenu = "Priority: 1.Low  2.Medium  3.High";
    public const string MachineOperatorIdPrompt = "ID MachineOperator: ";
    public const string OrderIdPrompt = "ID Order (ex: ORD1): ";
    public const string UnitsToProducePrompt = "Units to prodce now: ";
    public const string NoActiveOrder = "Nu exista comenzi active sau operatorul nu este valid!";
    public const string NextPriorityOrder = "\nUrmatoarea comanda prioritara:";
    public const string PriorityUnitsPrompt = "Unitati de produs acum: ";
    public const string SalesAgentNotFound = "Sales Agent not found!";
    public const string ProductToSellPrompt = "Product name to sell: ";
    public const string QuantityToSellPrompt = "Quantity to sell: ";
    public const string DemoLoaded = "Demo data loaded! Press Enter to continue...";
    public const string NoMachineParts = "There are no existing parts";
    public const string StopMachineBeforeMaintenance = "Stop the machine before maintenance!";
    public const string OrderDoesNotExist = "Order doesn't exist!";
    public const string EmployeesDoNotExist = "One of the employees doesn't exist!";
    public const string StopMachineBeforeRepair = "Stop the car before the repair!";
    public const string ProductDoesNotExistForStock = "There is no such product";
    public const string PredictiveMaintenanceTitle = "\n=== PREDICTIVE MAINTENANCE ===";
    public const string EfficiencyTitle = "\n=== PRODUCTION EFFICIENCY DASHBOARD ===";
    public const string HealthMonitoringTitle = "\n=== MACHINE HEALTH MONITORING ===";
    public const string InventoryAlertsTitle = "\n=== INVENTORY ALERTS ===";
    public const string OrdersSortedByPriority = "=== Orders sorted by priority ===";
    public const string OrderAlreadyCompleted = "The order is already completed!";
    public const string MachineOperatorMaintenance = "The car is in maintenance, wait for it to be repaired";
    public const string MachineOperatorStopped = "The car is off, you can't execute the command";
    public const string MachineStartConfirmation = "Do you want to start the car? YES/NO";
    public const string EmployeesHeader = "=== EMPLOYEES ===";
    public const string MachinesHeader = "=== MACHINES ===";
    public const string ProductsHeader = "=== PRODUCTS ===";
    public const string OrdersHeader = "=== ORDERS ===";
    public const string CuttingMachineCriticalDiagnostic = "WARNING: The blade is dull, needs replacing!";
    public const string CuttingMachineNormalDiagnostic = "Sharp blade. Normal operation.";
    public const string SewingMachineCriticalDiagnostic = "WARNING: The needle tension is irregular!";
    public const string SewingMachineNormalDiagnostic = "Needle and thread checked. Working normally.";
    public const string FunctionalPartStatus = "OK";
    public const string BrokenPartStatus = "BROKEN";
    public const string Yes = "YES";
    public const string No = "NO";
    public const string PasswordMaskCharacter = "*";
    public const string PasswordMaskBackspace = "\b \b";
    public const string OrderIdPrefix = "ORD";
    public const string BallProductType = "Ball";
    public const string DollProductType = "Doll";
    public const string FrisbeeProductType = "Frisbee";
    public const string TeddyBearProductType = "Teddy Bear";
    public const string WoodenCubesProductType = "Wooden Cubes";

    public static string Welcome(string username, string role) => $"\nWelcome {username}! Role: {role}\n";
    public static string EmployeeIdAlreadyExists(string id) => $"Employee ID {id} already exists. Please choose a unique ID.";
    public static string SuccessfullyLoggedIn(string username, string role) => $"Successfully logged in as: {username} ({role})";
    public static string ProductDescription(string type, string name, ProductCategory category, string size) => $"{type}:{name} {category} Size {size}";
    public static string TechnicianCannotRepair(string name) => $"{name} he can't fix a running machine!";
    public static string PartReplaced(string part) => $"Part {part} was replaced.";
    public static string TechnicianRepaired(string name, string machine) => $"{name} fixed the machine {machine}";
    public static string TechnicianDuty(string name) => $"{name} (Technician) repares the machines.";
    public static string InsufficientStockAvailable(int quantity) => $"Insufficient stock! Available: {quantity}";
    public static string FactoryReport(string name, int employees, int machines, int products, int orders, decimal revenue, int units, int maintenance, int lowStock) =>
        $"\n=== REPORT: {name} ==={Environment.NewLine}Employees: {employees}{Environment.NewLine}Machines:   {machines}{Environment.NewLine}Products:  {products}{Environment.NewLine}Orders:  {orders}{Environment.NewLine}Total Revenue: {revenue} RON{Environment.NewLine}Total Units Sold: {units}{Environment.NewLine}Machines requiring maintenance: {maintenance}{Environment.NewLine}Products below stock threshold: {lowStock}";
    public static string NoMaintenanceRequired(int days) => $"No machines require maintenance in the next {days} days.";
    public static string AverageEfficiency(decimal efficiency) => $"Average efficiency: {efficiency:F2}%";
    public static string AllProductsAboveThreshold(int threshold) => $"All products are above the stock threshold of {threshold}.";
    public static string SaleRecorded(int quantity, string product, decimal amount) => $"Sale recorded: {quantity}x {product} = {amount} RON";
    public static string SalesReport(string name, decimal revenue, int units, decimal profit) =>
        $"\n=== SALES REPORT: {name} ==={Environment.NewLine}Total Revenue: {revenue} RON{Environment.NewLine}Total Units Sold: {units}{Environment.NewLine}Average Price Per Unit: {(units > 0 ? (revenue / units).ToString("F2") : "N/A")} RON{Environment.NewLine}Estimated Profit: {profit} RON";
    public static string MachinePartStatus(string name, string type, string status) => $"  The piece: {name} | Type: {type} | Status: {status}";
    public static string LoggingFailed(string error) => $"Logging failed: {error}";
    public static string EmployeeDisplay(string id, string name, EmployeeRole role, decimal salary, int days) =>
        $"[{id}] {name} - Role: {role} - Salary: {salary} - Period of activity: {days} days";
    public static string DirectorDisplay(string id, string name, decimal salary, int days) =>
        $"[{id}] {name} - Director - Salary {salary} - Perios of Activity: {days} days";
    public static string MachineDisplay(string serial, string name, MachineStatus status, MachineCondition condition, int age, int parts) =>
        $"[{serial}] {name} - Status: {status} - Condition: {condition} - Age: {age} zile - Pieces: {parts}";
    public static string ProductDisplay(string description, decimal productionCost, decimal sellingPrice, int stock) =>
        $"  {description}{Environment.NewLine} -Production Cost={productionCost} -sellingPrice= {sellingPrice} -RON - Stock: {stock}";
    public static string OrderDisplay(string id, string product, int target, int produced, ProductionOrderStatus status, Priority priority, string manager, string machine, DateTime date) =>
        $"[{id}] {product} x{target} | Product: {produced} | Status: {status} | Prioritate: {priority} | Manager: {manager} | Machine: {machine} | Date: {date:yyyy-MM-dd}";
    public static string LoginAttemptFailed(int attempt, int remaining) =>
        $"Attempt {attempt} failed. {remaining} attempts remaining.\n";
    public static string LoggedInAs(string username, string role) => $"Logged in as: {username} ({role})";
    public static string FileNotFound(string path) => $"Error: {path} not found. Please ensure the file exists.";
    public static string InvalidCredentialLine(string line) => $"Warning: Invalid line format: {line}";
    public static string CredentialsLoaded(int count) => $"Loaded {count} employee credentials.";
    public static string CredentialsLoadFailed(string error) => $"Error loading credentials: {error}";
    public static string CredentialFieldsRequired() => "Error: All credential fields must be provided!";
    public static string UsernameAlreadyExists(string username) => $"Error: Username '{username}' already exists!";
    public static string CredentialsSaved(string username, string role) => $"Credentials saved for employee {username} ({role})";
    public static string CredentialsSaveFailed(string error) => $"Error saving credentials: {error}";
    public static string MachineUnderMaintenance(string name) => $"{name} It's under maintenance, it can't be started!";
    public static string MachineMissingParts(string name) => $"{name} has broken or missing parts!";
    public static string MachineStarted(string name) => $"{name} was turned on.";
    public static string MachineStopped(string name) => $"{name} was stopped.";
    public static string MachinePartBroken(string part, string machine) => $"Pieces:{part} from {machine} are broken";
    public static string HealthCritical() => "CRITICAL: Maintenance is required immediately.";
    public static string HealthBrokenParts(int count) => $"WARNING: {count} broken part(s) require attention.";
    public static string HealthPreventive(int days) => $"WARNING: Preventive maintenance is due within {days} day(s).";
    public static string HealthHealthy() => "HEALTHY: No maintenance alert.";
    public static string OrderCompleted(string id) => $"Order {id} COMPLETED!";
    public static string OrderProgress(string id, int produced, int target) => $"Progres {id}: {produced}/{target}";
    public static string EmployeeAlreadyExists(string id) => $"There is already an employee with the ID {id}";
    public static string MachineAlreadyExists(string serial) => $"There is already a machine with the serial number {serial}";
    public static string EmployeeDeleted() => "Employee successfully deleted!";
    public static string StockAdded(string name, int quantity) => $"New stock added: {name} + {quantity} pieces";
    public static string LowStock(string name, int quantity, int threshold) =>
        $"ALERT: {name} stock is low ({quantity} remaining; threshold: {threshold}).";
    public static string MachineCannotProduce(string name) => $"{name} it's not on, it can't produce!";
    public static string CuttingMachineProducing(string name) => $"{name}Cuts the material according to the patterns.";
    public static string SewingMachineCannotProduce(string name) => $"{name} isn't started!";
    public static string SewingMachineProducing(string name) => $"{name} sews the material .";
    public static string DirectorDuty(string name) => $"{name} (The director) checks the factory reports.";
    public static string EngineerInspection(string name, string machine, string result) => $"{name} (Engineer) inspected the  {machine}: {result}";
    public static string EngineerDuty(string name) => $"{name} (Engineer) inspects the machines.";
    public static string MachineOperatorDuty(string name) => $"{name} (Machine Operator) operates the machines.";
    public static string ProductionManagerOrderCreated(string name, Priority priority, string id, int quantity, string product) => $"{name} created the {priority} priority order {id} for {quantity} x {product}";
    public static string ProductionManagerDuty(string name) => $"{name} (Production Manager) coordinates production.";
    public static string SalesAgentSale(string name, int quantity, string product) => $"{name} sold {quantity}x {product}";
    public static string SalesAgentDuty(string name) => $"{name} (Sales Agent) sell the factory's products.";
    public static string NotProductionManager(string name) => $"{name} is not ProductionManager!";
    public static string NotMachineOperator(string name) => $"{name} is not MachineOperator!";
    public static string NotTechnician(string name) => $"{name} is not a Technician!";
    public static string NotEngineer(string name) => $"{name} is not a Engineer!";
    public static string NotSalesAgent(string name) => $"{name} is not a SalesAgent!";
    public static string MaintenanceDue(string serial, string name, int days) => $"{serial} - {name}: maintenance due in {days} day(s).";
    public static string MachineEfficiency(string serial, string name, decimal efficiency, int cycles) => $"{serial} - {name}: {efficiency:F2}% efficiency, {cycles} production cycle(s).";
    public static string MachineHealth(string serial, string name, MachineCondition condition, string alert) => $"{serial} - {name} | {condition} | {alert}";
    public static string EmployeeAddedLog(string name) => $"Employee added: {name}";
    public static string EmployeeRemovedLog(string id) => $"Employee removed: {id}";
    public static string ProductionLogged(int quantity, string orderId, string product, bool completed = false) => $"Produced {quantity} units for order {orderId} ({product})" + (completed ? " - completed" : string.Empty);
    public static string MachineStartedLog(string serial) => $"Started machine {serial}";
    public static string MachineStoppedLog(string serial) => $"Stopped machine {serial}";
    public static string MachineRepairedLog(string serial) => $"Repaired machine {serial}";
    public static string ProductSoldLog(string product, int quantity) => $"Sold product {product} x{quantity}";
    public static string SuccessfulLoginLog() => "Successful login";
    public static string FailedLoginAttemptLog() => "Failed login attempt";
    public static string UserLoggedOutLog() => "User logged out";
}
