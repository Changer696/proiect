using System;
using SmartFactorySimple;

public class ProductionOrder : IIdentifiable
{
    public string Id { get; set; }
    public Machine Masina;
    public ProductionManager CreatDe;
    public string NumeProdus;
    public int CantitateTarget;
    public int CantitateProdusa;
    public ProductionOrderStatus Status;
    public Priority Prioritate;
    public DateTime DataCrearii;

    public ProductionOrder(string id, Machine masina, ProductionManager creatDe,
                           string numeProdus, int cantitateTarget, Priority prioritate)
    {
        Id = id;
        Masina = masina;
        CreatDe = creatDe;
        NumeProdus = numeProdus;
        CantitateTarget = cantitateTarget;
        CantitateProdusa = 0;
        Status = ProductionOrderStatus.Created;
        Prioritate = prioritate;
        DataCrearii = DateTime.Now;
    }

    public void InregistreazaProductie(int unitati)
    {
        if (Status == ProductionOrderStatus.Completed)
        {
            Console.WriteLine(Messages.OrderAlreadyCompleted);
            return;
        }
        CantitateProdusa = CantitateProdusa + unitati;

        if (CantitateProdusa >= CantitateTarget)
        {
            CantitateProdusa = CantitateTarget;
            Status = ProductionOrderStatus.Completed;
            Console.WriteLine(Messages.OrderCompleted(Id));
            if (CreatDe != null)
                Logging.Log(CreatDe.Id, Messages.ProductionLogged(unitati, Id, NumeProdus, true));
        }
        else
        {
            Status = ProductionOrderStatus.InProgress;
            Console.WriteLine(Messages.OrderProgress(Id, CantitateProdusa, CantitateTarget));
            if (CreatDe != null)
                Logging.Log(CreatDe.Id, Messages.ProductionLogged(unitati, Id, NumeProdus));
        }
    }
    public void Afiseaza()
    {
        Console.WriteLine(Messages.OrderDisplay(Id, NumeProdus, CantitateTarget, CantitateProdusa, Status,
            Prioritate, CreatDe.Nume, Masina.SerialNumber, DataCrearii));
    }
}
