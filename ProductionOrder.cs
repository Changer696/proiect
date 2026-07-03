using System;

public class ProductionOrder
{
    public string Id;
    public Machine Masina;
    public ProductionManager CreatDe;
    public string NumeProdus;
    public int CantitateTarget;
    public int CantitateProdusa;
    public ProductionOrderStatus Status;
    public Priority Prioritate;
    public DateTime DataCrearii;

    public ProductionOrder(string id, Machine masina, ProductionManager creatDe,
                           string numeProdus, int cantitateTarget)
    {
        Id = id;
        Masina = masina;
        CreatDe = creatDe;
        NumeProdus = numeProdus;
        CantitateTarget = cantitateTarget;
        CantitateProdusa = 0;
        Status = ProductionOrderStatus.Created;
        DataCrearii = DateTime.Now;
    }

    public void InregistreazaProductie(int unitati)
    {
        if (Status == ProductionOrderStatus.Completed)
        {
            Console.WriteLine("Comanda e deja finalizata!");
            return;
        }
        CantitateProdusa = CantitateProdusa + unitati;

        if (CantitateProdusa >= CantitateTarget)
        {
            CantitateProdusa = CantitateTarget;
            Status = ProductionOrderStatus.Completed;
            Console.WriteLine("Comanda " + Id + " FINALIZATA!");
        }
        else
        {
            Status = ProductionOrderStatus.InProgress;
            Console.WriteLine("Progres " + Id + ": " + CantitateProdusa + "/" + CantitateTarget);
        }
    }
    public void SetPriority()
    {
        if (CantitateTarget > 1 && CantitateTarget <= 5)
        {
            Prioritate = Priority.High;
        }
        else if (CantitateTarget > 5 && CantitateTarget < 10)
        {
            Prioritate = Priority.Medium;
        }
        else
        {
            Prioritate = Priority.Low;
        }
    }

    public void Afiseaza()
    {
        Console.WriteLine("[" + Id + "] " + NumeProdus +
                          " x" + CantitateTarget +
                          " | Produs: " + CantitateProdusa +
                          " | Status: " + Status +
                          " | Prioritate: " + Prioritate +
                          " | Manager: " + CreatDe.Nume +
                          " | Masina: " + Masina.SerialNumber +
                          " | Data: " + DataCrearii.ToString("yyyy-MM-dd"));
    }
}
