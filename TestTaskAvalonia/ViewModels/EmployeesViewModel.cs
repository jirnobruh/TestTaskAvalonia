using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TestTaskAvalonia.Data;
using TestTaskAvalonia.Domain;
using TestTaskAvalonia.Services;

namespace TestTaskAvalonia.ViewModels;

public class EmployeesViewModel
{
    private readonly IDataStore _store;
    private readonly LoggingService _log;

    public ObservableCollection<Employee> Employees { get; private set; }
    public ObservableCollection<Organization> Organizations { get; }

    public Employee? SelectedEmployee { get; set; }

    private Organization? _selectedOrganizationFilter;
    public Organization? SelectedOrganizationFilter
    {
        get => _selectedOrganizationFilter;
        set
        {
            _selectedOrganizationFilter = value;
            ApplyFilters();
        }
    }

    private string _searchLastName = "";
    public string SearchLastName
    {
        get => _searchLastName;
        set
        {
            _searchLastName = value;
            ApplyFilters();
        }
    }
    
    public ICommand AddCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }

    public EmployeesViewModel(IDataStore store, LoggingService log)
    {
        _store = store;
        _log = log;

        Organizations = new ObservableCollection<Organization>(_store.Organizations);
        Employees = new ObservableCollection<Employee>(_store.Employees);

        AddCommand = new RelayCommand(AddEmployee);
        EditCommand = new RelayCommand(EditEmployee, () => SelectedEmployee != null);
        DeleteCommand = new RelayCommand(DeleteEmployee, () => SelectedEmployee != null);
    }

    private void ApplyFilters()
    {
        var filtered = _store.Employees.AsEnumerable();

        if (SelectedOrganizationFilter != null)
            filtered = filtered.Where(e => e.OrganizationId == SelectedOrganizationFilter.Id);

        if (!string.IsNullOrWhiteSpace(SearchLastName))
            filtered = filtered.Where(e => e.LastName.Contains(SearchLastName, StringComparison.OrdinalIgnoreCase));

        Employees = new ObservableCollection<Employee>(filtered);
    }

    private void AddEmployee()
    {
        var emp = new Employee
        {
            Id = _store.NextEmployeeId,
            FirstName = "Имя",
            LastName = "Фамилия",
            Position = "Должность",
            OrganizationId = _store.Organizations.First().Id
        };

        _store.Employees.Add(emp);
        ApplyFilters();

        _store.Save();
        _log.Log($"Добавлен сотрудник: {emp.LastName}");
    }

    private void EditEmployee()
    {
        if (SelectedEmployee == null)
            return;

        _store.Save();
        _log.Log($"Редактирование сотрудника: {SelectedEmployee.LastName}");
    }

    private void DeleteEmployee()
    {
        if (SelectedEmployee == null)
            return;

        _store.Employees.Remove(SelectedEmployee);
        ApplyFilters();

        _store.Save();
        _log.Log($"Удалён сотрудник: {SelectedEmployee.LastName}");
    }

    public void Reload()
    {
        Organizations.Clear();
        foreach (var org in _store.Organizations)
            Organizations.Add(org);

        ApplyFilters();
    }
}