using System;
public abstract class Product
{
    public decimal productionCost;
    public decimal sellingPrice;
    public int quantity;
    public decimal SellingPrice
    {
        get { return sellingPrice; }
        set
        {
            if (value < 0) throw new ArgumentException("Prețul de vânzare nu poate fi negativ.");
            sellingPrice = value;
        }
    }
    public decimal ProductionCost
    {
        get { return productionCost; }
        set
        {
            if (value < 0) throw new ArgumentException("Costul de producție nu poate fi negativ.");
            productionCost = value;
        }
    }
    public int Cantitate
    {
        get { return quantity; }
        set
        {
            if (value < 0) throw new ArgumentException("Cantitatea nu poate fi negativă.");
            quantity = value;
        }
    }
    public string Nume { get; set; }
    public ProductCategory Category { get; set; }
    
    protected Product(string nume, ProductCategory category, decimal productionCost, decimal sellingPrice, int cantitate)
    {
        Nume = nume;
        Category = category;
        SellingPrice = sellingPrice;
        Cantitate = cantitate;
        ProductionCost = productionCost;
        
    }
    public void AdaugaStoc(int cantitate)
    {
        if (cantitate < 0)
        {
            Console.WriteLine("Cantitatea nu poate fi negativa!");
            return;
        }
        Cantitate = Cantitate + cantitate;
    }
    public void VindeStoc(int cantitate)
    {
        if (cantitate > Cantitate)
        {
            Console.WriteLine("Stoc insuficient!");
            return;
        }
        Cantitate = Cantitate - cantitate;
    }
    public abstract string GetDescription();
    public virtual void Afiseaza()
    {
        Console.WriteLine(Nume + "-Production Cost" + ProductionCost + " - sellingPrice: " + SellingPrice + " RON - Stoc: " + Cantitate);
        Console.WriteLine("  " + GetDescription());
    }
}
