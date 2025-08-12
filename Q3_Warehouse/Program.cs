using System;
using System.Collections.Generic;

// Marker interface for inventory items
public interface IInventoryItem
{
    int Id { get; }
    string Name { get; }
    int Quantity { get; set; }
}

// ElectronicItem
public class ElectronicItem : IInventoryItem
{
    public int Id { get; init; }
    public string Name { get; init; }
    public int Quantity { get; set; }
    public string Brand { get; init; }
    public int WarrantyMonths { get; init; }

    public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
    {
        Id = id; Name = name; Quantity = quantity; Brand = brand; WarrantyMonths = warrantyMonths;
    }

    public override string ToString() => $"[E] {Id}: {Name} (Qty: {Quantity}) Brand:{Brand} Warranty:{WarrantyMonths}m";
}

// GroceryItem
public class GroceryItem : IInventoryItem
{
    public int Id { get; init; }
    public string Name { get; init; }
    public int Quantity { get; set; }
    public DateTime ExpiryDate { get; init; }

    public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
    {
        Id = id; Name = name; Quantity = quantity; ExpiryDate = expiryDate;
    }

    public override string ToString() => $"[G] {Id}: {Name} (Qty: {Quantity}) Expiry:{ExpiryDate:d}";
}

// Custom exceptions
public class DuplicateItemException : Exception { public DuplicateItemException(string msg):base(msg){} }
public class ItemNotFoundException : Exception { public ItemNotFoundException(string msg):base(msg){} }
public class InvalidQuantityException : Exception { public InvalidQuantityException(string msg):base(msg){} }

// InventoryRepository<T>
public class InventoryRepository<T> where T : IInventoryItem
{
    private readonly Dictionary<int, T> _items = new();

    public void AddItem(T item)
    {
        if (_items.ContainsKey(item.Id)) throw new DuplicateItemException($"Item with ID {item.Id} already exists.");
        _items[item.Id] = item;
    }

    public T GetItemById(int id)
    {
        if (!_items.TryGetValue(id, out var item)) throw new ItemNotFoundException($"Item with ID {id} not found.");
        return item;
    }

    public void RemoveItem(int id)
    {
        if (!_items.Remove(id)) throw new ItemNotFoundException($"Item with ID {id} not found.");
    }

    public List<T> GetAllItems() => new List<T>(_items.Values);

    public void UpdateQuantity(int id, int newQuantity)
    {
        if (newQuantity < 0) throw new InvalidQuantityException("Quantity cannot be negative.");
        var item = GetItemById(id);
        item.Quantity = newQuantity;
    }
}

// Warehouse manager
public class WareHouseManager
{
    private readonly InventoryRepository<ElectronicItem> _electronics = new();
    private readonly InventoryRepository<GroceryItem> _groceries = new();

    public void SeedData()
    {
        try
        {
            _electronics.AddItem(new ElectronicItem(1, "Laptop", 5, "Dell", 24));
            _electronics.AddItem(new ElectronicItem(2, "Headphones", 15, "Sony", 12));
            _groceries.AddItem(new GroceryItem(101, "Rice", 50, DateTime.Today.AddMonths(12)));
            _groceries.AddItem(new GroceryItem(102, "Milk", 20, DateTime.Today.AddDays(7)));
        }
        catch (Exception ex) { Console.WriteLine($"Seed error: {ex.Message}"); }
    }

    public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
    {
        var items = repo.GetAllItems();
        foreach (var i in items) Console.WriteLine(i);
    }

    public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
    {
        try
        {
            var item = repo.GetItemById(id);
            if (quantity < 0) throw new InvalidQuantityException("Quantity to increase cannot be negative.");
            repo.UpdateQuantity(id, item.Quantity + quantity);
            Console.WriteLine($"Increased item {id} by {quantity}. New qty: {repo.GetItemById(id).Quantity}");
        }
        catch (Exception ex) { Console.WriteLine($"IncreaseStock error: {ex.Message}"); }
    }

    public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
    {
        try
        {
            repo.RemoveItem(id);
            Console.WriteLine($"Removed item {id}");
        }
        catch (Exception ex) { Console.WriteLine($"RemoveItem error: {ex.Message}"); }
    }

    // Expose repos for demonstration
    public InventoryRepository<ElectronicItem> ElectronicsRepo => _electronics;
    public InventoryRepository<GroceryItem> GroceriesRepo => _groceries;
}

class Program
{
    static void Main()
    {
        var manager = new WareHouseManager();
        manager.SeedData();

        Console.WriteLine("Grocery items:");
        manager.PrintAllItems(manager.GroceriesRepo);

        Console.WriteLine("\nElectronic items:");
        manager.PrintAllItems(manager.ElectronicsRepo);

        Console.WriteLine("\n-- Testing exceptions --");
        // Add duplicate
        try { manager.ElectronicsRepo.AddItem(new ElectronicItem(1, "Tablet", 3, "Samsung", 12)); }
        catch (Exception ex) { Console.WriteLine($"Expected: {ex.Message}"); }

        // Remove non-existent
        manager.RemoveItemById(manager.GroceriesRepo, 999);

        // Update invalid quantity
        try { manager.GroceriesRepo.UpdateQuantity(101, -5); }
        catch (Exception ex) { Console.WriteLine($"Expected: {ex.Message}"); }
    }
}
