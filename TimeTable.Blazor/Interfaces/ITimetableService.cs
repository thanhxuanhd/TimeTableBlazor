using TimeTable.Blazor.Models;

namespace TimeTable.Blazor.Interfaces;

public interface ITimetableService
{
    public List<Appointment> GetAppointments(DateTime startDate);
}