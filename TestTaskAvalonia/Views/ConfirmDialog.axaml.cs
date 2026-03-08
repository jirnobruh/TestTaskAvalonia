using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace TestTaskAvalonia.Views;

public partial class ConfirmDialog : Window {
    public ConfirmDialog(string message) {
        InitializeComponent();
        this.FindControl<TextBlock>("MessageText").Text = message;
    }
    private void OnYesClick(object sender, RoutedEventArgs e) => Close(true);
    private void OnNoClick(object sender, RoutedEventArgs e) => Close(false);
}