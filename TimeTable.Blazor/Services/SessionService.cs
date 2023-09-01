﻿using Microsoft.EntityFrameworkCore;
using TimeTable.Blazor.Interfaces;
using TimeTable.Blazor.Models;
using TimeTable.Domain;

namespace TimeTable.Blazor.Services;

public class SessionService : ISessionService
{
    private readonly ILogger<SessionService> _logger;
    private readonly TimeTableDbContext _context;

    public SessionService(ILogger<SessionService> logger, TimeTableDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public bool AddSession(AppointmentDto appointment)
    {
        var success = true;

        try
        {
            if (IsValidateSession(appointment))
            {
                var sessionId = Guid.NewGuid();
                var session = new Session()
                {
                    Id = sessionId,
                    Description = appointment.Description,
                    Name = appointment.SessionName,
                    RoomId = appointment.RoomId,
                    SubjectId = appointment.SubjectId,
                    Timeslot = new Timeslot()
                    {
                        Id = Guid.NewGuid(),
                        EndTime = appointment.EndDate,
                        StartTime = appointment.StartDate,
                        SessionId = sessionId
                    }
                };

                _context.Sessions.Add(session);
                _context.SaveChanges();
            }
            else
            {
                success = false;
            }
        }
        catch (Exception ex)
        {
            success = false;
            _logger.LogError(ex.Message);
        }

        return success;
    }

    public bool UpdateSession(AppointmentDto appointment)
    {
        var success = true;

        try
        {
            if (IsValidateSession(appointment))
            {
                var session = _context.Sessions.Include(s => s.Timeslot).FirstOrDefault(s => s.Id == appointment.Id);
                if (session is null)
                {
                    return false;
                }

                session.Name = appointment.SessionName;
                session.RoomId = appointment.RoomId;
                session.SubjectId = appointment.SubjectId;
                session.Timeslot.StartTime = appointment.StartDate;
                session.Timeslot.EndTime = appointment.EndDate;
                session.Description = appointment.Description;

                _context.Update(session);
                _context.SaveChanges();
            }
            else
            {
                success = false;
            }
        }
        catch (Exception ex)
        {
            success = false;
            _logger.LogError(ex.Message);
        }

        return success;
    }

    private bool IsValidateSession(AppointmentDto appointment)
    {
        var valid = true;
        if (string.IsNullOrEmpty(appointment.SessionName))
        {
            valid = false;
        }

        if (appointment.StartDate > appointment.EndDate)
        {
            valid = false;
            _logger.LogError($"StartDate: {appointment.StartDate} greater than End Date: {appointment.EndDate}");
            return valid;
        }

        var existingSubject = _context.Subjects.FirstOrDefault(s => s.Id == appointment.SubjectId);

        if (existingSubject is null)
        {
            _logger.LogError($"Subject [{appointment.SubjectId}] doesn't exist");
            valid = false;
        }

        var existingRoom = _context.Rooms.FirstOrDefault(r => r.Id == appointment.RoomId);

        if (existingRoom is null)
        {
            _logger.LogError($"Room [{appointment.RoomId}] doesn't exist");
            valid = false;
        }

        if (!appointment.Id.HasValue)
        {
            var exisitingSessions = _context.Sessions.Include(s => s.Timeslot).Where(s => s.SubjectId == appointment.SubjectId).ToList();

            foreach (var exisitingSession in exisitingSessions)
            {
                if ((appointment.StartDate == exisitingSession.Timeslot.StartTime && appointment.EndDate == exisitingSession.Timeslot.EndTime)
                       || (appointment.StartDate <= exisitingSession.Timeslot.EndTime && appointment.EndDate >= exisitingSession.Timeslot.StartTime))
                {
                    var message = $"Duplicate Session Or Overlaps: {appointment.SessionName}";
                    _logger.LogWarning(message);
                    valid = false;
                    break;
                }
            }
        }

        return valid;
    }
}