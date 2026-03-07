using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using TestTaskAvalonia.Domain;

namespace TestTaskAvalonia.Data;

public class JsonDataStore : IDataStore
{
    private readonly string _jsonFilePath;

    public List<Organization> Organizations { get; private set; } = new();
    public List<Employee> Employees { get; private set; } = new();
    
    public JsonDataStore(string? jsonFilePath = null)
    {
        _jsonFilePath = jsonFilePath ?? "data.json";
    }

    public void Load()
    {
        if (!File.Exists(_jsonFilePath))
        {
            Save();
            return;
        }
        
        var json = File.ReadAllText(_jsonFilePath);
        var data = JsonSerializer.Deserialize<Root>(json);

        Organizations = data?.Organizations ?? new();
        Employees = data?.Employees ?? new();
    }

    public void Save()
    {
        var data = new Root
        {
            Organizations = Organizations,
            Employees = Employees
        };

        var json = JsonSerializer.Serialize(
            data,
            new JsonSerializerOptions { WriteIndented = true });

        File.WriteAllText(_jsonFilePath, json);
    }
    
    public int NextOrganizationId => Organizations.Count == 0 ? 1 : Organizations.Max(o => o.Id) + 1;
    public int NextEmployeeId => Employees.Count == 0 ? 1 : Employees.Max(e => e.Id) + 1;
    
    private class Root
    {
        public List<Organization> Organizations { get; set; } = new();
        public List<Employee> Employees { get; set; } = new();
    }
    
}