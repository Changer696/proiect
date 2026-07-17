using System;

public abstract class Employee : IIdentifiable
{
    public string Id { get; set; }
    public string Nume;
    public EmployeeRole Rol;
    public decimal Salariu;
    public DateTime DataAngajarii;
        // Arbitrary monetary valuation assigned to this employee.
        public decimal Valuation { get; set; } = 0m;

    protected Employee(string id, string nume, decimal salariu, DateTime dataAngajarii)
    {
        Id = id;
        Nume = nume;
        Salariu = salariu;
        DataAngajarii = dataAngajarii;
    }

    // Returns the employee tenure in days calculated from hiring date.
    public int GetVechimeZile()
    {
        TimeSpan diferenta = DateTime.Now - DataAngajarii;
        return diferenta.Days;
    }

    // Performs the role-specific duty for the employee (implemented by subclasses).
    public abstract void PerformDuty();

    // Displays basic employee information to the console.
    public virtual void Afiseaza()
    {
        Console.WriteLine(Messages.EmployeeDisplay(Id, Nume, Rol, Salariu, GetVechimeZile()));
        Console.WriteLine($"  Valuation: {Valuation} RON");
    }

    // Serializes employee data for line-based persistence.
    public string ToDataLine()
    {
        return string.Join(";",
            GetType().Name,
            Id,
            Nume,
            Salariu.ToString(System.Globalization.CultureInfo.InvariantCulture),
            DataAngajarii.ToString("s"),
            Valuation.ToString(System.Globalization.CultureInfo.InvariantCulture));
    }

    // Deserializes an employee from a persisted line.
    public static Employee FromDataLine(string line)
    {
        var parts = line.Split(';');
        if (parts.Length < 5)
            throw new FormatException("Invalid employee line format");

        string tip = parts[0].Trim();
        string id = parts[1].Trim();
        string nume = parts[2].Trim();
        decimal salariu = decimal.Parse(parts[3].Trim(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
        DateTime dataAngajarii = DateTime.Parse(parts[4].Trim(), null, System.Globalization.DateTimeStyles.RoundtripKind);
        decimal valuation = 0m;
        if (parts.Length >= 6)
        {
            decimal.TryParse(parts[5].Trim(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out valuation);
        }

        Employee angajat = tip switch
        {
            "Director" => new Director(id, nume, salariu, dataAngajarii),
            "ProductionManager" => new ProductionManager(id, nume, salariu, dataAngajarii),
            "Engineer" => new Engineer(id, nume, salariu, dataAngajarii),
            "Technician" => new Technician(id, nume, salariu, dataAngajarii),
            "MachineOperator" => new MachineOperator(id, nume, salariu, dataAngajarii),
            "SalesAgent" => new SalesAgent(id, nume, salariu, dataAngajarii),
            _ => null
        };

        if (angajat != null)
        {
            angajat.Valuation = valuation;
        }

        return angajat;
    }
}
