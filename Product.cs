using System;
using SmartFactorySimple;
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
        if (value < 0) throw new ArgumentException(Messages.SellingPriceCannotBeNegative);
            sellingPrice = value;
        }
    }
    public decimal ProductionCost
    {
        get { return productionCost; }
        set
        {
        if (value < 0) throw new ArgumentException(Messages.ProductionCostCannotBeNegative);
            productionCost = value;
        }
    }
    public int Cantitate
    {
        get { return quantity; }
        set
        {
        if (value < 0) throw new ArgumentException(Messages.QuantityCannotBeNegative);
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
            Console.WriteLine(Messages.QuantityCannotBeNegative);
            return;
        }
        Cantitate = Cantitate + cantitate;
    }
    public void VindeStoc(int cantitate)
    {
        if (cantitate > Cantitate)
        {
            Console.WriteLine(Messages.InsufficientStock);
            return;
        }
        Cantitate = Cantitate - cantitate;
    }
    public abstract string GetDescription();
    public virtual void Afiseaza()
    {
        Console.WriteLine(Messages.ProductDisplay(GetDescription(), ProductionCost, SellingPrice, Cantitate));
    }

    public string ToDataLine()
    {
        string tip = GetType().Name;
        // marime may be present on derived types as a field
        string marime = "";
        var field = GetType().GetField("Marime");
        if (field != null)
        {
            var val = field.GetValue(this);
            marime = val?.ToString() ?? "";
        }

        return string.Join(";",
            tip,
            Nume,
            Category,
            ProductionCost.ToString(System.Globalization.CultureInfo.InvariantCulture),
            SellingPrice.ToString(System.Globalization.CultureInfo.InvariantCulture),
            Cantitate,
            marime);
    }

    public static Product FromDataLine(string line)
    {
        var parts = line.Split(';');
        if (parts.Length < 6) throw new FormatException("Invalid product line format");

        string tip = parts[0].Trim();
        string nume = parts[1].Trim();
        var category = Enum.Parse<ProductCategory>(parts[2].Trim());
        decimal productionCost = decimal.Parse(parts[3].Trim(), System.Globalization.CultureInfo.InvariantCulture);
        decimal sellingPrice = decimal.Parse(parts[4].Trim(), System.Globalization.CultureInfo.InvariantCulture);
        int cantitate = int.Parse(parts[5].Trim());
        string marime = parts.Length >= 7 ? parts[6].Trim() : "";

        Product produs = tip switch
        {
            "WoodenCubes" => new WoodenCubes(nume, productionCost, sellingPrice, cantitate, marime),
            "TedyBear" => new TedyBear(nume, productionCost, sellingPrice, cantitate, marime),
            "Ball" => new Ball(nume, productionCost, sellingPrice, cantitate, marime),
            "Doll" => new Doll(nume, productionCost, sellingPrice, cantitate, marime),
            "Frisbee" => new Frisbee(nume, productionCost, sellingPrice, cantitate, marime),
            _ => null
        };

        return produs;
    }
}
