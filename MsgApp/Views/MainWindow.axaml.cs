using Avalonia.Controls;
using Avalonia.Interactivity;
using MsgApp.ViewModels;

namespace MsgApp;

public partial class MainWindow : Window
{
    public required MainWindowViewModel _mainviewModel;

    // Parameterlose Konstruktor nötig für den XAML-Loader
    public MainWindow()
    {
        InitializeComponent();
    }

    // zusätzlicher Konstruktor für DI
    public MainWindow(MainWindowViewModel mainWindowViewModel)
    :this() // call den Parameterlose Konstruktor zuerst
    {
        _mainviewModel = mainWindowViewModel;
        // ViewModel als DataContext
        DataContext = _mainviewModel;
    }

    private void SortBySender_Click(object? sender, RoutedEventArgs e)
    {
        _mainviewModel.SortBySender();
    }

    private void SortByDate_Click(object? sender, RoutedEventArgs e)
    {
        _mainviewModel.SortByDate();
    }
}