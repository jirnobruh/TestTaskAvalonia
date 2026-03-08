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

public class OrganizationsViewModel : ViewModelBase
{
    private readonly IDataStore _store;
    private readonly LoggingService _log;
    private readonly MainWindowViewModel _mainVM;
    
    public ObservableCollection<Organization> Organizations { get; } = new();

    private Organization? _selectedOrganization;
    public Organization? SelectedOrganization
    {
        get => _selectedOrganization;
        set => SetProperty(ref _selectedOrganization, value);
    }
    
    public ICommand AddOrganizationCommand { get; }
    public ICommand EditOrganizationCommand { get; }
    public ICommand DeleteOrganizationCommand { get; }
    
    public OrganizationsViewModel(IDataStore store, LoggingService log, MainWindowViewModel mainVM)
    {
        _store = store;
        _log = log;
        _mainVM = mainVM;
        
        Organizations = new ObservableCollection<Organization>(_store.Organizations);

        AddOrganizationCommand = new RelayCommand(AddOrganization);
        EditOrganizationCommand = new RelayCommand(EditOrganization);
        DeleteOrganizationCommand = new RelayCommand(DeleteOrganization);
    }

    private async void AddOrganization()
    {
        var newOrg = new Organization { Id = _store.NextOrganizationId };
        var dialog = new EditWindow();
        dialog.SetupForOrganization(newOrg, "Добавление организации");

        var desktop = Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        var result = await dialog.ShowDialog<bool>(desktop.MainWindow);

        if (result)
        {
            _store.Organizations.Add(newOrg);
            Organizations.Add(newOrg);
            _store.Save();
            _log.Log($"Добавлена организация: {newOrg.Name}");
        }
    }
    
    private async void EditOrganization()
    {
        if (SelectedOrganization == null) return;

        var dialog = new EditWindow();
        dialog.SetupForOrganization(SelectedOrganization, "Редактирование организации");

        var desktop = Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        if (await dialog.ShowDialog<bool>(desktop.MainWindow))
        {
            _store.Save();
            _log.Log($"Изменена организация: {SelectedOrganization.Name}");
            Reload();
        }
    }

    private async void DeleteOrganization()
    {
        if (SelectedOrganization == null) return;

        var dialog = new ConfirmDialog(
            $"Вы уверены, что хотите удалить организацию '{SelectedOrganization.Name}'? " +
            "Внимание: все сотрудники этой организации также будут удалены!");
    
        var mainWindow = (Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
        var result = await dialog.ShowDialog<bool>(mainWindow);

        if (result)
        {
            string orgName = SelectedOrganization.Name;
            int orgId = SelectedOrganization.Id;

            var toRemove = _store.Employees.Where(e => e.OrganizationId == orgId).ToList();
            foreach (var emp in toRemove) _store.Employees.Remove(emp);
        
            _store.Organizations.Remove(SelectedOrganization);
            _store.Save();
        
            _log.Log($"Удалена организация: {orgName} и {toRemove.Count} сотрудников");
        
            Reload();
            
            if (result)
            {
                _mainVM.RefreshAll(); 
            }
        }
    }
    
    public void Reload()
    {
        Organizations.Clear();
        foreach (var org in _store.Organizations)
            Organizations.Add(org);
    }
}