using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Layout;
using Avalonia.Media;
using Quick_FreeRDP.Views;

namespace Quick_FreeRDP.Helpers;

public class MessageBox
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    public async Task Show()
    {
        var lifetime = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        var msgBox = new Views.MessageBox
        {
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            MessageContent =
            {
                Text = Message,
            }
        };

        // Ensure focus when opened
        msgBox.Opened += (_, _) =>
        {
            msgBox.Activate();
            msgBox.CloseButton?.Focus(); // assuming you name it
        };


        await msgBox.ShowDialog(lifetime?.MainWindow!);
    }
}