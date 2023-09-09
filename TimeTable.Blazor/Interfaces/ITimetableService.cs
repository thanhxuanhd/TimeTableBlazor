using TimeTable.Domain.Dtos;

namespace TimeTable.Blazor.Interfaces;

public interface ITimetableService
{
    public List<Appointment> GetAppointments(DateTime startDate);
}