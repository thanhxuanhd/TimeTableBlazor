using Microsoft.EntityFrameworkCore;
using TimeTable.Blazor.Interfaces;
using TimeTable.Domain;
using TimeTable.Domain.Dtos;

namespace TimeTable.Blazor.Services;

public class TimetableService : ITimetableService
{
    private readonly TimeTableDbContext _context;

    public TimetableService(TimeTableDbContext context)
    {
        _context = context;
    }

    public async Task<List<Appointment>> GetAppointments(DateTime startDate, DateTime endDate)
    {
        var appointments = _context.Timeslots.Include(t => t.Session)
            .Include(t => t.Session.Room)
            .Include(t => t.Session.Subject)
            .Include(t => t.Session.Subject.Teacher)
            .Where(t => t.StartTime <= endDate && t.EndTime >= startDate)
            .OrderBy(t => t.StartTime)
            .Select(t => new Appointment()
            {
                StartDate = t.StartTime,
                EndDate = t.EndTime,
                Location = t.Session.Room.Location,
                RoomCode = t.Session.Room.Code,
                SujectId = t.Session.SubjectId,
                RoomId = t.Session.RoomId,
                SessionName = t.Session.Name,
                SubjectName = t.Session.Subject.Name,
                TeacherName = t.Session.Subject.Teacher.GetFullName(),
                Description = t.Session.Description,
                Id = t.Session.Id,
            });

        return await appointments.ToListAsync();
    }
}