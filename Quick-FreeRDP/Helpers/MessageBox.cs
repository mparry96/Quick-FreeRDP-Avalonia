using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Layout;
using Avalonia.Media;
using Quick_FreeRDP.Views;
using Avalonia.Threading;

namespace Quick_FreeRDP.Helpers;

public class MessageBox
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    
    public void Show()
    {
        Dispatcher.UIThread.Post(() =>
        {
            var lifetime =
                Application.Current?.ApplicationLifetime
                    as IClassicDesktopStyleApplicationLifetime;

            if (lifetime?.MainWindow == null)
                return;

            var msgBox = new Views.MessageBox
            {
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
            };

            msgBox.MessageContent.Text = Message;

            msgBox.Opened += (_, _) =>
            {
                msgBox.Activate();
                msgBox.Topmost = true;
                msgBox.Focus();
            };

            msgBox.Show(lifetime.MainWindow);
        });
    }

    public async Task ShowAsync()
    {
        await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            var lifetime =
                Application.Current?.ApplicationLifetime
                    as IClassicDesktopStyleApplicationLifetime;

            if (lifetime?.MainWindow == null)
                return;

            var msgBox = new Views.MessageBox
            {
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
            };

            msgBox.MessageContent.Text = Message;

            msgBox.Opened += (_, _) =>
            {
                msgBox.Activate();
                msgBox.Topmost = true;
                msgBox.Focus();
            };

            await msgBox.ShowDialog(lifetime.MainWindow);
        });
    }
    
    
    // public async Task Show()
    // {
    //     var lifetime = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
    //     var msgBox = new Views.MessageBox
    //     {
    //         WindowStartupLocation = WindowStartupLocation.CenterOwner,
    //         MessageContent =
    //         {
    //             Text = Message,
    //         }
    //     };
    //
    //     // Ensure focus when opened
    //     msgBox.Opened += (_, _) =>
    //     {
    //         msgBox.Activate();
    //         msgBox.CloseButton?.Focus(); // assuming you name it
    //     };
    //
    //
    //     await msgBox.ShowDialog(lifetime?.MainWindow!);
    // }
}