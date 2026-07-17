using System;
using System.Linq;
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
    // Maximum distinct prime material types that producing this product may consume during a single execution.
    // Default is 5 as per requirement.
    public int MaxPrimeMaterialTypes { get; set; } = 5;
    // Recipe of prime materials required per unit: material name -> amount per unit.
    public System.Collections.Generic.Dictionary<string,int> PrimeMaterialRecipe { get; private set; } = new System.Collections.Generic.Dictionary<string,int>();
    
    protected Product(string nume, ProductCategory category, decimal productionCost, decimal sellingPrice, int cantitate)
    {
        Nume = nume;
        Category = category;
        SellingPrice = sellingPrice;
        Cantitate = cantitate;
        ProductionCost = productionCost;
        
    }
    // Adds quantity to product stock, validates non-negative input.
    public void AdaugaStoc(int cantitate)
    {
        if (cantitate < 0)
        {
            Console.WriteLine(Messages.QuantityCannotBeNegative);
            return;
        }
        Cantitate = Cantitate + cantitate;
    }
    // Reduces stock by the sold quantity if available.
    public void VindeStoc(int cantitate)
    {
        if (cantitate > Cantitate)
        {
            Console.WriteLine(Messages.InsufficientStock);
            return;
        }
        Cantitate = Cantitate - cantitate;
    }
    // Returns a textual description of the product (implemented in derived types).
    public abstract string GetDescription();
    // Displays product details to the console.
    public virtual void Afiseaza()
    {
        Console.WriteLine(Messages.ProductDisplay(GetDescription(), ProductionCost, SellingPrice, Cantitate));
        if (PrimeMaterialRecipe != null && PrimeMaterialRecipe.Count > 0)
        {
            Console.WriteLine("  Prime materials:");
            foreach (var kv in PrimeMaterialRecipe)
            {
                Console.WriteLine($"   - {kv.Key}: {kv.Value} per unit");
            }
        }
    }

    // Sets the recipe for this product. Validates the max number of distinct material types.
    public bool SetPrimeMaterialRecipe(System.Collections.Generic.Dictionary<string,int> recipe)
    {
        if (recipe == null)
        {
            PrimeMaterialRecipe.Clear();
            return true;
        }
        if (recipe.Count > MaxPrimeMaterialTypes) return false;
        PrimeMaterialRecipe = new System.Collections.Generic.Dictionary<string,int>(recipe);
        return true;
    }

    // Serializes the product to a semicolon-separated data line for persistence.
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

        // serialize recipe as "mat=qty|mat2=qty2"
        string recipeSerialized = "";
        if (PrimeMaterialRecipe != null && PrimeMaterialRecipe.Count > 0)
        {
            recipeSerialized = string.Join("|", PrimeMaterialRecipe.Select(kv => kv.Key + "=" + kv.Value));
        }

        return string.Join(";",
            tip,
            Nume,
            Category,
            ProductionCost.ToString(System.Globalization.CultureInfo.InvariantCulture),
            SellingPrice.ToString(System.Globalization.CultureInfo.InvariantCulture),
            Cantitate,
            marime,
            MaxPrimeMaterialTypes,
            recipeSerialized);
    }

    // Deserializes a product from a data line and returns the appropriate derived product.
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
        int maxTypes = parts.Length >= 8 && int.TryParse(parts[7].Trim(), out var mt) ? mt : 5;
        string recipeSerialized = parts.Length >= 9 ? parts[8].Trim() : "";

        Product produs = tip switch
        {
            "WoodenCubes" => new WoodenCubes(nume, productionCost, sellingPrice, cantitate, marime),
            "TedyBear" => new TedyBear(nume, productionCost, sellingPrice, cantitate, marime),
            "Ball" => new Ball(nume, productionCost, sellingPrice, cantitate, marime),
            "Doll" => new Doll(nume, productionCost, sellingPrice, cantitate, marime),
            "Frisbee" => new Frisbee(nume, productionCost, sellingPrice, cantitate, marime),
            _ => null
        };

        if (produs != null)
        {
            produs.MaxPrimeMaterialTypes = maxTypes;
            if (!string.IsNullOrWhiteSpace(recipeSerialized))
            {
                var dict = new System.Collections.Generic.Dictionary<string,int>();
                foreach (var entry in recipeSerialized.Split('|'))
                {
                    var kv = entry.Split('=');
                    if (kv.Length != 2) continue;
                    if (int.TryParse(kv[1], out int v)) dict[kv[0]] = v;
                }
                produs.SetPrimeMaterialRecipe(dict);
            }
        }

        return produs;
    }
}
