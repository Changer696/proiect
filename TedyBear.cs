using System;
public class TedyBear : Product
{
    public string Marime;
    public TedyBear(string nume, decimal productionCost, decimal sellingPrice, int cantitate, string marime)
        : base(nume, ProductCategory.PretendToys, productionCost, sellingPrice, cantitate)
    {
        Marime = marime;
    }
    // Returns a description string for the teddy bear including size.
    public override string GetDescription()
    {
        
        return Messages.ProductDescription(Messages.ProductTypeTeddyBear, Nume, Category, Marime);
    }
}


