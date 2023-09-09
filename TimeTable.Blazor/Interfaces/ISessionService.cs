using TimeTable.Domain.Dtos;

namespace TimeTable.Blazor.Interfaces;

public interface ISessionService
{
    bool AddSession(AppointmentDto appointment);
    bool UpdateSession(AppointmentDto appointment);
}
