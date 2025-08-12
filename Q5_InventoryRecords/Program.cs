using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

// Marker interface for logging
public interface IInventoryEntity { int Id { get; } }

// Immutable record
public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

// Generic logger
public class InventoryLogger<T> where T : IInventoryEntity
{
    private readonly List<T> _log = new();
    private readonly string _filePath;

    public InventoryLogger(string filePath) { _filePath = filePath; }

    public void Add(T item) => _log.Add(item);
    public List<T> GetAll() => new List<T>(_log);

    public void SaveToFile()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(_log, options);
        File.WriteAllText(_filePath, json);
    }

    public void LoadFromFile()
    {
        if (!File.Exists(_filePath)) { _log.Clear(); return; }
        var json = File.ReadAllText(_filePath);
        var items = JsonSerializer.Deserialize<List<T>>(json);
        _log.Clear();
        if (items != null) _log.AddRange(items);
    }
}

// Integration app
public class InventoryApp
{
    private readonly InventoryLogger<InventoryItem> _logger;

    public InventoryApp(string path) { _logger = new InventoryLogger<InventoryItem>(path); }

    public void SeedSampleData()
    {
        _logger.Add(new InventoryItem(1, "USB Cable", 30, DateTime.Now));
        _logger.Add(new InventoryItem(2, "Notebook", 100, DateTime.Now.AddDays(-2)));
        _logger.Add(new InventoryItem(3, "Pen", 200, DateTime.Now.AddDays(-1)));
        _logger.Add(new InventoryItem(3, "Kenkey", 500, DateTime.Now.AddDays(-3)));

    }

    public void SaveData() { _logger.SaveToFile(); Console.WriteLine("Saved data."); }

    public void LoadData() { _logger.LoadFromFile(); Console.WriteLine("Loaded data."); }

    public void PrintAllItems()
    {
        foreach (var item in _logger.GetAll())
            Console.WriteLine($"{item.Id}: {item.Name} (Qty: {item.Quantity}) Added: {item.DateAdded}");
    }

    // Expose logger for tests if needed
}

class Program
{
    static void Main()
    {
        var path = "inventory.json";
        var app = new InventoryApp(path);

        app.SeedSampleData();
        app.SaveData();

        // Simulate new session by creating a fresh app and loading
        var newApp = new InventoryApp(path);
        newApp.LoadData();
        newApp.PrintAllItems();
    }
}
