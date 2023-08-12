namespace TimeTable.Blazor.Interfaces;

public interface INotifyService
{
    public void Error(string message);

    public void Success(string message);

    public void Warning(string message);
}
