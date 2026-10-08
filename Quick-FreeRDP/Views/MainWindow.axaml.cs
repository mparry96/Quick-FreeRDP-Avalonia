using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using Quick_FreeRDP.Helpers;
using Quick_FreeRDP.ViewModels;

namespace Quick_FreeRDP.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        if (Design.IsDesignMode)
            return;
        
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

    private void ResolutionsListbox_OnKeyUp(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Delete &&
            sender is ListBox listBox &&
            listBox.SelectedItem is string resolution &&
            DataContext is MainWindowViewModel vm)
        {
            vm.ResolutionsListboxItems.Remove(resolution);

            ConfigManager.SaveConfig(
                vm.RdpItems,
                vm.ResolutionsListboxItems);
        }
        
    }

    private void ResolutionsListbox_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (sender is ListBox listBox &&
            listBox.SelectedItem is string resolution &&
            DataContext is MainWindowViewModel vm)
        {
            vm.ResolutionX = resolution.Split('x')[0];
            vm.ResolutionY = resolution.Split('x')[1];
        }
    }
}