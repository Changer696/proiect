using System;

public class MachinePart
{
    public string Nume;
    public string Tip;
    public bool EFunctionala;
    public DateTime DataInstalarii;

    public MachinePart(string nume, string tip)
    {
        Nume = nume;
        Tip = tip;
        EFunctionala = true;
        DataInstalarii = DateTime.Now;
    }

    // Marks the part as broken.
    public void Strica()
    {
        EFunctionala = false;
    }

    // Replaces the part (marks functional and updates installation date).
    public void Inlocuieste()
    {
        EFunctionala = true;
        DataInstalarii = DateTime.Now;
    }

    // Displays the part status to the console.
    public void Afiseaza()
    {
        string status;
        if (EFunctionala)
            status = Messages.FunctionalPartStatus;
        else
            status = Messages.BrokenPartStatus;

        Console.WriteLine(Messages.MachinePartStatus(Nume, Tip, status));
    }
}
