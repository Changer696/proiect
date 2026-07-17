using System;

// Represents a raw/prime material and its available quantity in stock.
public class PrimeMaterial
{
    public string Name { get; set; }
    public int Quantity { get; set; }

    public PrimeMaterial(string name, int quantity)
    {
        Name = name;
        Quantity = quantity;
    }

    // Attempts to consume the specified amount. Returns true if successful.
    public bool Consume(int amount)
    {
        if (amount <= 0) return true;
        if (Quantity < amount) return false;
        Quantity -= amount;
        return true;
    }

    // Adds stock to this material.
    public void AddStock(int qty)
    {
        if (qty <= 0) return;
        Quantity += qty;
    }

    // Displays the material status.
    public void Afiseaza()
    {
        Console.WriteLine($"{Name}: {Quantity}");
    }
}
