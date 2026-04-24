using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

namespace Quick_FreeRDP.Views;

public partial class MessageBox : Window
{
    public MessageBox()
    {
        InitializeComponent();
    }
    
    private void InputElement_OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            e.Handled = true; // stop it bubbling further
            Close();
        }
    }
}