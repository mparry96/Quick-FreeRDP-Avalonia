using Avalonia.Controls;
using Avalonia.Threading;
using Quick_FreeRDP.ViewModels;

namespace Quick_FreeRDP.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        DataContextChanged += (_, _) =>
        {
            if (DataContext is MainWindowViewModel vm)
            {
                vm.ScrollToEndRequested += () =>
                {
                    Dispatcher.UIThread.Post(() =>
                    {
                        ConsoleScrollViewer.ScrollToEnd();
                    });
                };
            }
        };
    }
}