using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Layout;
using Avalonia.Media;

namespace Quick_FreeRDP.Helpers;

public class MessageBox
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    public async Task Show()
    {
        var lifetime = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;

        var dialog = new Window
        {
            Width = 320,
            Height = 180,
           // Title = Title,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false,
            WindowDecorations = WindowDecorations.Full,
            

            BorderThickness = new Thickness(3),
            BorderBrush = new SolidColorBrush(Colors.White),
            
        };
        

        // Layout
        var stack = new StackPanel
        {
            Margin = new Thickness(15),
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Spacing = 30
        };

        var text = new TextBlock
        {
            Text = Message,
            TextWrapping = Avalonia.Media.TextWrapping.Wrap,
            HorizontalAlignment = HorizontalAlignment.Center
        };

        var closeButton = new Button
        {
            Content = "Close",
            Width = 80,
            HorizontalAlignment = HorizontalAlignment.Center
        };

        closeButton.Click += (_, _) => dialog.Close();

        stack.Children.Add(text);
        stack.Children.Add(closeButton);

        dialog.Content = stack;

        await dialog.ShowDialog(lifetime?.MainWindow!);
    }
}