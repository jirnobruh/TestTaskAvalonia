using System;
using System.Windows.Input;
using TestTaskAvalonia.Data;
using TestTaskAvalonia.Services;

namespace TestTaskAvalonia.ViewModels;

public class MainWindowViewModel
{
    private readonly IDataStore _dataStore;
    private readonly LoggingService _logging;
    private readonly TestDataGenerator _testDataGenerator;

    public ICommand GenerateTestDataCommand { get; }
    
    public OrganizationsViewModel OrganizationsVM { get; }
    public EmployeesViewModel EmployeesVM { get; }

    public MainWindowViewModel()
    {
        _dataStore = new JsonDataStore();
        _logging = new LoggingService();
        _testDataGenerator = new TestDataGenerator(_dataStore, _logging);

        _dataStore.Load();

        GenerateTestDataCommand = new RelayCommand(GenerateTestData);
        
        OrganizationsVM = new OrganizationsViewModel(_dataStore, _logging);
        EmployeesVM = new EmployeesViewModel(_dataStore, _logging);
    }

    private void GenerateTestData()
    {
        try
        {
            _testDataGenerator.Generate();
            OrganizationsVM.Reload();
            EmployeesVM.Reload();
            _logging.Log("Пользователь создал тестовые данные");
        }
        catch (Exception ex)
        {
            _logging.LogError(ex);
        }
    }
}