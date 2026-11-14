
using Pillstacular.Services;

namespace Pillstacular;

public partial class App : Application
{
    private readonly WindowsNotificationService
        _notificationService;

    public App(
        WindowsNotificationService notificationService)
    {
        InitializeComponent();

        _notificationService = notificationService;
    }

    protected override Window CreateWindow(
        IActivationState? activationState)
    {
        var window = new Window(new AppShell());

#if WINDOWS
        window.Created += async (_, _) =>
        {
            try
            {
                await _notificationService.StartAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"Notification startup failed: {ex}");
            }
        };

        window.Destroying += (_, _) =>
        {
            _notificationService.Stop();
        };
#endif

        return window;
    }
}
