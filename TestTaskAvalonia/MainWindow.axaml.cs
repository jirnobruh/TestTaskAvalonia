using Avalonia.Controls;
using TestTaskAvalonia.ViewModels;

namespace TestTaskAvalonia;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }
}