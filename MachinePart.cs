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

    public void Strica()
    {
        EFunctionala = false;
    }

    public void Inlocuieste()
    {
        EFunctionala = true;
        DataInstalarii = DateTime.Now;
    }

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
