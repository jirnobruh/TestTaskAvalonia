using System.Collections.ObjectModel;
using System.Windows.Input;
using TestTaskAvalonia.Data;
using TestTaskAvalonia.Domain;
using TestTaskAvalonia.Services;

namespace TestTaskAvalonia.ViewModels;

public class OrganizationsViewModel : ViewModelBase
{
    private readonly IDataStore _store;
    private readonly LoggingService _log;
    
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
    
    public OrganizationsViewModel(IDataStore store, LoggingService log)
    {
        _store = store;
        _log = log;
        
        Organizations = new ObservableCollection<Organization>(_store.Organizations);

        AddOrganizationCommand = new RelayCommand(AddOrganization);
        EditOrganizationCommand = new RelayCommand(EditOrganization);
        DeleteOrganizationCommand = new RelayCommand(DeleteOrganization);
    }

    private void AddOrganization()
    {
        var org = new Organization
        {
            Id = _store.NextOrganizationId,
            Name = "New Organization",
            Address = "New Address",
        };
        
        _store.Organizations.Add(org);
        Organizations.Add(org);
        
        _store.Save();
        _log.Log($"Добавлена организация {org.Name}");
    }
    
    private void EditOrganization()
    {
        if (SelectedOrganization == null)
            return;

        _log.Log($"Редактирование организации: {SelectedOrganization.Name}");

        _store.Save();
    }

    private void DeleteOrganization()
    {
        if (SelectedOrganization == null)
            return;

        _store.Organizations.Remove(SelectedOrganization);
        Organizations.Remove(SelectedOrganization);

        _store.Save();
        _log.Log($"Удалена организация: {SelectedOrganization.Name}");
    }
    
    public void Reload()
    {
        Organizations.Clear();
        foreach (var org in _store.Organizations)
            Organizations.Add(org);
    }
}