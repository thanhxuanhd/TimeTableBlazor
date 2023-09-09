using TimeTable.Domain.Dtos;

namespace TimeTable.Blazor.Interfaces;

public interface ITimetableService
{
    public Task<List<Appointment>> GetAppointments(DateTime startDate, DateTime endDate);
}