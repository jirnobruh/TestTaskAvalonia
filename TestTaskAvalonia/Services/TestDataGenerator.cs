using TestTaskAvalonia.Data;
using TestTaskAvalonia.Domain;

namespace TestTaskAvalonia.Services;

public class TestDataGenerator
{
    private readonly IDataStore _store;
    private readonly LoggingService _log;

    public TestDataGenerator(IDataStore store, LoggingService log)
    {
        _store = store;
        _log = log;
    }

    public void Generate()
    {
        _store.Organizations.Clear();
        _store.Employees.Clear();
        
        for (int i = 1; i <= 3; i++)
        {
            var org = new Organization
            {
                Id = _store.NextOrganizationId,
                Name = $"Организация {i}",
                Address = $"Адрес {i}"
            };

            _store.Organizations.Add(org);

            for (int j = 1; j <= 10; j++)
            {
                var emp = new Employee
                {
                    Id = _store.NextEmployeeId,
                    FirstName = $"Имя{j}",
                    LastName = $"Фамилия{j}",
                    Position = "Сотрудник",
                    OrganizationId = org.Id
                };

                _store.Employees.Add(emp);
            }
        }

        _store.Save();
        _log.Log("Сгенерированы тестовые данные");
    }
}