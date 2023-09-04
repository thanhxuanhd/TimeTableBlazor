namespace TimeTable.Blazor.Interfaces;

public interface INotificationService
{
    public void Error(string message);

    public void Success(string message);

    public void Warning(string message);
}
