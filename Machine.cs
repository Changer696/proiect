using System;
using System.Linq;
using SmartFactorySimple;

public abstract class Machine
{
    public string SerialNumber;
    public string Nume;
    public MachineStatus Status;
    public MachineCondition Conditie;
    public DateTime DataFabricatiei;
    public static Random _random = new Random();
    public int ProductionCycles { get; private set; }
    public DateTime? LastMaintenanceDate { get; private set; }

    public MachinePart[] Piese;
    public int NrPiese;

    protected Machine(string serialNumber, string nume, DateTime dataFabricatiei)
    {
        SerialNumber = serialNumber;
        Nume = nume;
        DataFabricatiei = dataFabricatiei;
        Status = MachineStatus.Stopped;
        Conditie = MachineCondition.Good;
        Piese = new MachinePart[10];
        NrPiese = 0;
    }

    // Adds a part to the machine's part array.
    public void AdaugaPiesa(MachinePart piesa)
    {
        Piese[NrPiese] = piesa;
        NrPiese++;
    }

    // Returns whether all existing parts are functional.
    public bool ArePieseComplete()
    {
        return NrPiese > 0 && Piese
            .Take(NrPiese)
            .All(piesa => piesa.EFunctionala);
    }

    // Randomly simulates part failures and reports broken parts.
    public void StareVerificarePiesa()
    {
        if (NrPiese == 0)
        {
            Console.WriteLine("There are no existing parts");
        }
         int sansa = _random.Next(1,101);
         if(sansa <= 20)
        {
            int index = _random.Next(NrPiese);
            if (Piese[index].EFunctionala)
            {
                Piese[index].Strica();
                Console.WriteLine($"Pieces:{Piese[index].Nume} from {Nume} are broken");
            }
        }
    }

    // Returns machine age in days since manufacture.
    public int GetVarstaZile()
    {
        TimeSpan varsta = DateTime.Now - DataFabricatiei;
        return varsta.Days;
    }

    // Estimates days until next maintenance based on condition.
    public int EstimateDaysUntilMaintenance()
    {
        int conditionAllowance = Conditie switch
        {
            MachineCondition.Excellent => 60,
            MachineCondition.Good => 30,
            MachineCondition.Worn => 7,
            _ => 0
        };

        return Math.Max(0, conditionAllowance - GetVarstaZile() / 365);
    }

    // Calculates an efficiency percentage based on condition and functional parts ratio.
    public decimal CalculateEfficiencyPercentage()
    {
        decimal conditionScore = Conditie switch
        {
            MachineCondition.Excellent => 100m,
            MachineCondition.Good => 85m,
            MachineCondition.Worn => 60m,
            _ => 30m
        };

        decimal functionalPartsRatio = NrPiese == 0
            ? 0m
            : Piese.Take(NrPiese).Count(piesa => piesa.EFunctionala) / (decimal)NrPiese;

        return Math.Round(conditionScore * functionalPartsRatio, 2);
    }

    // Returns a health alert string describing maintenance needs or warnings.
    public string GetHealthAlert()
    {
        int brokenParts = Piese.Take(NrPiese).Count(piesa => !piesa.EFunctionala);

        if (Status == MachineStatus.Maintenance || Conditie == MachineCondition.Critical)
            return "CRITICAL: Maintenance is required immediately.";
        if (brokenParts > 0)
            return $"WARNING: {brokenParts} broken part(s) require attention.";
        if (Conditie == MachineCondition.Worn || EstimateDaysUntilMaintenance() <= 7)
            return $"WARNING: Preventive maintenance is due within {EstimateDaysUntilMaintenance()} day(s).";

        return "HEALTHY: No maintenance alert.";
    }

    // Increments the production cycle counter.
    protected void RegisterProductionCycle()
    {
        ProductionCycles++;
    }


            // Restores machine runtime state (production cycles and last maintenance date).
            public void RestoreState(int productionCycles, DateTime? lastMaintenanceDate)
    {
        ProductionCycles = productionCycles;
        LastMaintenanceDate = lastMaintenanceDate;
    }

    // Starts the machine if not under maintenance and parts are complete.
    public virtual void Start()
    {
        if (Status == MachineStatus.Maintenance)
        {
            Console.WriteLine(Nume + " It's under maintenance, it can't be started!");
            return;
        }
        if (!ArePieseComplete())
        {
            Console.WriteLine(Nume + " has broken or missing parts!");
            return;
        }
        Status = MachineStatus.Running;
        Console.WriteLine(Nume + " was turned on.");
        Logging.Log($"Started machine {SerialNumber}");
    }

    // Stops the machine and logs the action.
    public virtual void Stop()
    {
        Status = MachineStatus.Stopped;
        Console.WriteLine(Nume + " was stopped.");
        Logging.Log($"Stopped machine {SerialNumber}");
    }

    // Sets the machine into maintenance mode (cannot be started while in maintenance).
    public void SetMaintenance()
    {
        if (Status == MachineStatus.Running)
        {
            Console.WriteLine("Stop the machine before maintenance!");
            return;
        }
        Status = MachineStatus.Maintenance;
    }

    // Downgrades the machine condition to the next worse level.
    protected void DegradeazaConditia()
    {
        if (Conditie == MachineCondition.Excellent)
            Conditie = MachineCondition.Good;
        else if (Conditie == MachineCondition.Good)
            Conditie = MachineCondition.Worn;
        else if (Conditie == MachineCondition.Worn)
        { Conditie = MachineCondition.Critical; Status = MachineStatus.Maintenance; }
    
    }

    // Restores machine condition to excellent and records maintenance date.
    public void RestoreazaConditia()
    {
        Conditie = MachineCondition.Excellent;
        Status = MachineStatus.Stopped;
        LastMaintenanceDate = DateTime.Now;
    }

    // Produces items (implementation varies by machine type).
    public abstract void Produce();
    // Runs diagnostics and returns a textual result.
    public abstract string RunDiagnostics();

    // Displays the machine's summary information to the console.
    public virtual void Afiseaza()
    {
        Console.WriteLine("[" + SerialNumber + "] " + Nume +
                          " - Status: " + Status +
                          " - Condition: " + Conditie +
                          " - Age: " + GetVarstaZile() + " zile" +
                          " - Pieces: " + NrPiese);
    }

    // Serializes the machine to a semicolon-separated data line for persistence.
    public string ToDataLine()
    {
        string tip = GetType().Name;
        string lastMaint = LastMaintenanceDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? "";

        string pieseSerializate = string.Join("~", Piese.Take(NrPiese).Select(p =>
            $"{p.Nume},{p.Tip},{p.EFunctionala},{p.DataInstalarii:yyyy-MM-dd HH:mm:ss}"));

        return string.Join(";",
            tip,
            SerialNumber,
            Nume,
            Status,
            Conditie,
            DataFabricatiei.ToString("yyyy-MM-dd HH:mm:ss"),
            ProductionCycles,
            lastMaint,
            pieseSerializate);
    }

    // Deserializes a machine from a data line and returns the appropriate derived instance.
    public static Machine FromDataLine(string line)
    {
        var parts = line.Split(';');
        if (parts.Length != 9) throw new FormatException("Invalid machine line format");

        string tip = parts[0].Trim();
        string serial = parts[1].Trim();
        string nume = parts[2].Trim();
        var status = Enum.Parse<MachineStatus>(parts[3].Trim());
        var conditie = Enum.Parse<MachineCondition>(parts[4].Trim());
        var dataFab = DateTime.Parse(parts[5].Trim());
        int cicluri = int.Parse(parts[6].Trim());
        DateTime? lastMaint = string.IsNullOrWhiteSpace(parts[7]) ? null : DateTime.Parse(parts[7].Trim());
        string pieseRaw = parts[8].Trim();

        Machine masina = tip switch
        {
            "SewingMachine" => new SewingMachine(serial, nume, dataFab),
            "CuttingMachine" => new CuttingMachine(serial, nume, dataFab),
            _ => null
        };

        if (masina == null) return null;

        masina.Status = status;
        masina.Conditie = conditie;
        masina.RestoreState(cicluri, lastMaint);

        if (!string.IsNullOrWhiteSpace(pieseRaw))
        {
            foreach (string piesaStr in pieseRaw.Split('~'))
            {
                string[] pp = piesaStr.Split(',');
                if (pp.Length != 4) continue;

                var piesa = new MachinePart(pp[0].Trim(), pp[1].Trim())
                {
                    EFunctionala = bool.Parse(pp[2].Trim()),
                    DataInstalarii = DateTime.Parse(pp[3].Trim())
                };
                masina.AdaugaPiesa(piesa);
            }
        }

        return masina;
    }
}
