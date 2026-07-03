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
            Console.WriteLine("The order is already completed!");
            return;
        }
        CantitateProdusa = CantitateProdusa + unitati;

        if (CantitateProdusa >= CantitateTarget)
        {
            CantitateProdusa = CantitateTarget;
            Status = ProductionOrderStatus.Completed;
            Console.WriteLine("Order " + Id + " COMPLETED!");
        }
        else
        {
            Status = ProductionOrderStatus.InProgress;
            Console.WriteLine("Progres " + Id + ": " + CantitateProdusa + "/" + CantitateTarget);
        }
    }
    public void SetPriority()
    {

    }

    public void Afiseaza()
    {
        Console.WriteLine("[" + Id + "] " + NumeProdus +
                          " x" + CantitateTarget +
                          " | Product: " + CantitateProdusa +
                          " | Status: " + Status +
                          " | Manager: " + CreatDe.Nume +
                          " | Machine: " + Masina.SerialNumber +
                          " | Date: " + DataCrearii.ToString("yyyy-MM-dd"));
    }
}
