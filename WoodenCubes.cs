using System;
public class WoodenCubes : Product
{
    public string Marime;
    public WoodenCubes(string nume, decimal productionCost, decimal sellingPrice, int cantitate, string marime)
        : base(nume, ProductCategory.EducationalToys, productionCost, sellingPrice, cantitate)
    {
        Marime = marime;
    }
    public override string GetDescription()
    {
        return Messages.ProductDescription(Messages.ProductTypeWoodenCubes, Nume, Category, Marime);
    }
}
