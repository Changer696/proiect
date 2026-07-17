using System;

namespace SmartFactorySimple
{
    internal class Ball : Product
    {
        public string Marime;
        public Ball(string nume, decimal productionCost, decimal sellingPrice, int cantitate, string marime)
        : base(nume, ProductCategory.OutdoorToys, productionCost, sellingPrice, cantitate)
        {
            Marime = marime;
        }

        // Returns a descriptive string for the ball product including size.
        public override string GetDescription()
        {
           
            return Messages.ProductDescription(Messages.ProductTypeBall, Nume, Category, Marime);
        }
    }
}
