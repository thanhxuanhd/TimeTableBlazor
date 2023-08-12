using Radzen;
using TimeTable.Blazor.Interfaces;

namespace TimeTable.Blazor.Services;

public class NotifyService : INotifyService
{
    private readonly NotificationService _notificationService;

    public NotifyService(NotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public void Error(string message)
    {
        var notificationMessage = new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error", Detail = message, Duration = 2000 };
        _notificationService.Notify(notificationMessage);
    }

    public void Success(string message)
    {
        var notificationMessage = new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "Success", Detail = message, Duration = 2000 };
        _notificationService.Notify(notificationMessage);
    }

    public void Warning(string message)
    {
        var notificationMessage = new NotificationMessage { Severity = NotificationSeverity.Warning, Summary = "Warning", Detail = message, Duration = 2000 };
        _notificationService.Notify(notificationMessage);
    }
}