using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Collections.Generic;
using TestTaskAvalonia.Domain;

namespace TestTaskAvalonia.Views;

public partial class EditWindow : Window
{
    private readonly Dictionary<string, Control> _controls = new();
    private object _item;

    public EditWindow()
    {
        InitializeComponent();
    }

    public void SetupForOrganization(Organization org, string title)
    {
        _item = org;
        Header.Text = title;
        
        AddTextField("Name", "Название", org.Name);
        AddTextField("Address", "Адрес", org.Address);
    }

    public void SetupForEmployee(Employee emp, List<Organization> orgs, string title)
    {
        _item = emp;
        Header.Text = title;

        AddTextField("FirstName", "Имя", emp.FirstName);
        AddTextField("LastName", "Фамилия", emp.LastName);
        AddTextField("Position", "Должность", emp.Position);
        
        var cb = new ComboBox { Width = 250, ItemsSource = orgs, DisplayMemberBinding = new Avalonia.Data.Binding("Name") };
        foreach(var o in orgs) if(o.Id == emp.OrganizationId) cb.SelectedItem = o;
        
        AddCustomControl("Organization", "Организация", cb);
    }

    private void AddTextField(string key, string label, string value)
    {
        var tb = new TextBox { Text = value, Width = 250 };
        AddCustomControl(key, label, tb);
    }

    private void AddCustomControl(string key, string label, Control control)
    {
        var stack = new StackPanel { Spacing = 5 };
        stack.Children.Add(new TextBlock { Text = label });
        stack.Children.Add(control);
        FieldsContainer.Children.Add(stack);
        _controls[key] = control;
    }

    private void OnSaveClick(object sender, RoutedEventArgs e)
    {
        if (_item is Organization org)
        {
            org.Name = ((TextBox)_controls["Name"]).Text ?? "";
            org.Address = ((TextBox)_controls["Address"]).Text ?? "";
        }
        else if (_item is Employee emp)
        {
            emp.FirstName = ((TextBox)_controls["FirstName"]).Text ?? "";
            emp.LastName = ((TextBox)_controls["LastName"]).Text ?? "";
            emp.Position = ((TextBox)_controls["Position"]).Text ?? "";
            var cb = (ComboBox)_controls["Organization"];
            if (cb.SelectedItem is Organization selectedOrg) emp.OrganizationId = selectedOrg.Id;
        }
        
        Close(true); 
    }

    private void OnCancelClick(object sender, RoutedEventArgs e) => Close(false);
}