using System.Collections.Generic;
using TestTaskAvalonia.Domain;

namespace TestTaskAvalonia.Data;

public interface IDataStore
{
    List<Organization> Organizations { get; }
    List<Employee> Employees { get; }
    
    void Save();
    void Load();
    
    int NextOrganizationId { get; }
    int NextEmployeeId { get; }
}