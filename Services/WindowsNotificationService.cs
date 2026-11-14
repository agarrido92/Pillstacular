
#if WINDOWS
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
#endif

using Pillstacular.Models;

namespace Pillstacular.Services;

public class WindowsNotificationService
{
    private readonly DatabaseService _databaseService;
    private CancellationTokenSource? _cancellationTokenSource;

#if WINDOWS
    private bool _registered;
#endif

    public WindowsNotificationService(
        DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public void Initialize()
    {
#if WINDOWS
        if (_registered)
            return;

        var manager = AppNotificationManager.Default;

        manager.NotificationInvoked += OnNotificationInvoked;
        manager.Register();

        _registered = true;
#endif
    }

#if WINDOWS
    private void OnNotificationInvoked(
        AppNotificationManager sender,
        AppNotificationActivatedEventArgs args)
    {
        // Intake navigation can be added later.
    }
#endif

    public void ShowNotification(
        string title,
        string message)
    {
#if WINDOWS
        Initialize();

        var notification = new AppNotificationBuilder()
            .AddText(title)
            .AddText(message)
            .BuildNotification();

        AppNotificationManager.Default.Show(notification);
#endif
    }

    public async Task StartAsync()
    {
        Stop();

        Initialize();

        _cancellationTokenSource =
            new CancellationTokenSource();

        var token = _cancellationTokenSource.Token;

        await Task.Yield();

        _ = RunReminderLoopAsync(token);
    }

    public void Stop()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;
    }

    public async Task RestartAsync()
    {
        await StartAsync();
    }

    private async Task RunReminderLoopAsync(
        CancellationToken token)
    {
        var lastCheckedMinute = DateTime.Now.AddMinutes(-1);

        while (!token.IsCancellationRequested)
        {
            try
            {
                var now = DateTime.Now;

                if (now.Date != lastCheckedMinute.Date ||
                    now.Hour != lastCheckedMinute.Hour ||
                    now.Minute != lastCheckedMinute.Minute)
                {
                    await CheckRemindersAsync(now);

                    lastCheckedMinute = now;
                }

                await Task.Delay(
                    TimeSpan.FromSeconds(5),
                    token);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"Reminder error: {ex}");

                try
                {
                    await Task.Delay(
                        TimeSpan.FromSeconds(10),
                        token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }
    }

    private async Task CheckRemindersAsync(DateTime now)
    {
        var medications = await _databaseService
            .GetMedicationsAsync();

        string today = now.DayOfWeek.ToString();

        foreach (var medication in medications)
        {
            if (!medication.IsActive ||
                medication.IsAsNeeded)
                continue;

            var schedules = await _databaseService
                .GetSchedulesAsync(medication.Id);

            foreach (var schedule in schedules)
            {
                var days = schedule.DaysOfWeek.Split(
                    ',',
                    StringSplitOptions.RemoveEmptyEntries |
                    StringSplitOptions.TrimEntries);

                if (!days.Contains(today))
                    continue;

                if (schedule.ScheduledTime.Hours != now.Hour ||
                    schedule.ScheduledTime.Minutes != now.Minute)
                    continue;

                string displayName =
                    string.IsNullOrWhiteSpace(medication.Nickname)
                        ? medication.Name
                        : medication.Nickname;

                ShowNotification(
                    "Pillstacular",
                    $"{displayName} is due.");
            }
        }
    }
}
