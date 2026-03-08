using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using TestTaskAvalonia.Data;
using TestTaskAvalonia.Domain;
using TestTaskAvalonia.Services;
using TestTaskAvalonia.Views;

namespace TestTaskAvalonia.ViewModels;

public class EmployeesViewModel : ViewModelBase
{
    private readonly IDataStore _store;
    private readonly LoggingService _log;

    public ObservableCollection<Employee> Employees { get; } = new();
    public ObservableCollection<Organization> Organizations { get; } = new();
    
    public ICommand ResetFiltersCommand { get; }

    private Employee? _selectedEmployee;
    public Employee? SelectedEmployee
    {
        get => _selectedEmployee;
        set 
        {
            if (SetProperty(ref _selectedEmployee, value))
            {
                (AddCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (EditCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (DeleteCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }
    }

    private Organization? _selectedOrganizationFilter;
    public Organization? SelectedOrganizationFilter
    {
        get => _selectedOrganizationFilter;
        set
        {
            if (SetProperty(ref _selectedOrganizationFilter, value))
                ApplyFilters();
        }
    }

    private string _searchLastName = "";
    public string SearchLastName
    {
        get => _searchLastName;
        set
        {
            if (SetProperty(ref _searchLastName, value))
                ApplyFilters();
        }
    }

    public RelayCommand AddCommand { get; }
    public RelayCommand EditCommand { get; }
    public RelayCommand DeleteCommand { get; }

    public EmployeesViewModel(IDataStore store, LoggingService log)
    {
        _store = store;
        _log = log;

        AddCommand = new RelayCommand(AddEmployee);
        EditCommand = new RelayCommand(EditEmployee, () => SelectedEmployee != null);
        DeleteCommand = new RelayCommand(DeleteEmployee, () => SelectedEmployee != null);

        ResetFiltersCommand = new RelayCommand(ResetFilters);
        
        Reload();
    }

    private void ApplyFilters()
    {
        var filtered = _store.Employees.AsEnumerable();

        if (SelectedOrganizationFilter != null)
            filtered = filtered.Where(e => e.OrganizationId == SelectedOrganizationFilter.Id);

        if (!string.IsNullOrWhiteSpace(SearchLastName))
            filtered = filtered.Where(e => e.LastName.Contains(SearchLastName, StringComparison.OrdinalIgnoreCase));

        Employees.Clear();
        foreach (var emp in filtered)
            Employees.Add(emp);
    }

    private async void AddEmployee()
    {
        if (!_store.Organizations.Any()) return;

        var newEmp = new Employee { Id = _store.NextEmployeeId, OrganizationId = _store.Organizations[0].Id };
        var dialog = new EditWindow();
        dialog.SetupForEmployee(newEmp, _store.Organizations, "Новый сотрудник");

        var desktop = Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        if (await dialog.ShowDialog<bool>(desktop.MainWindow))
        {
            _store.Employees.Add(newEmp);
            _store.Save();
            _log.Log($"Добавлен сотрудник: {newEmp.LastName}");
            ApplyFilters();
        }
    }

    private async void EditEmployee()
    {
        if (SelectedEmployee == null) return;

        var dialog = new EditWindow();
        dialog.SetupForEmployee(SelectedEmployee, _store.Organizations, "Редактирование сотрудника");

        var desktop = Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        if (await dialog.ShowDialog<bool>(desktop.MainWindow))
        {
            _store.Save();
            _log.Log($"Изменен сотрудник: {SelectedEmployee.LastName}");
            ApplyFilters();
        }
    }

    private async void DeleteEmployee()
    {
        if (SelectedEmployee == null) return;

        var dialog = new ConfirmDialog($"Удалить сотрудника {SelectedEmployee.LastName}?");
        var mainWindow = (Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
        var result = await dialog.ShowDialog<bool>(mainWindow);

        if (result)
        {
            string name = SelectedEmployee.LastName;
            _store.Employees.Remove(SelectedEmployee);
            _store.Save();
        
            _log.Log($"Удалён сотрудник: {name}"); 
            ApplyFilters(); 
        }
    }
    
    private void ResetFilters()
    {
        SelectedOrganizationFilter = null;
        SearchLastName = string.Empty;
    
        _log.Log("Фильтры списка сотрудников сброшены");
    }
    
    public void Reload()
    {
        Organizations.Clear();
        foreach (var org in _store.Organizations)
            Organizations.Add(org);
        ApplyFilters();
    }
}