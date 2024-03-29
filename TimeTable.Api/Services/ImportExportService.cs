﻿using CsvHelper;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TimeTable.Api.Interfaces;
using TimeTable.Domain;
using TimeTable.Domain.Dtos;
using TimeTable.Domain.Enums;

namespace TimeTable.Api.Controllers;

public class ImportExportService : IImportExportService
{
    private readonly TimeTableDbContext _context;
    private readonly ILogger _logger;

    public ImportExportService(TimeTableDbContext context, ILogger<ImportExportService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public int ImportData(string type, string fileData)
    {
        var importCount = 0;
        var typeImportEnum = Enum.Parse<TemplateType>(type, true);

        switch (typeImportEnum)
        {
            case TemplateType.Student:
                importCount = ImportStudent(fileData);
                break;

            case TemplateType.Teacher:
                importCount = ImportTeacher(fileData);
                break;

            case TemplateType.Subject:
                importCount = ImportSubject(fileData);
                break;

            case TemplateType.Room:
                importCount = ImportRoom(fileData);
                break;

            case TemplateType.TimeTable:
                importCount = ImportTimetable(fileData);
                break;

            default:
                break;
        }

        return importCount;
    }

    private int ImportStudent(string fileData)
    {
        var studentDtos = new List<StudentImportDto>();

        studentDtos = GetDataFromCsv<StudentImportDto>(fileData);

        var studentCodes = studentDtos.Select(s => s.Code).ToList();
        var existingStudent = _context.Students.Where(x => studentCodes.Contains(x.Code)).ToList();

        if (existingStudent.Any())
        {
            foreach (var student in existingStudent)
            {
                var studentDto = studentDtos.FirstOrDefault(s => s.Code.Equals(student.Code, StringComparison.OrdinalIgnoreCase));
                if (studentDto is not null)
                {
                    _logger.LogWarning("Duplicate Student: [{Code}]", student.Code);
                    studentDtos.Remove(studentDto);
                }
            }
        }

        var students = studentDtos.Select(s => new Student()
        {
            Id = Guid.NewGuid(),
            Code = s.Code?.Trim(),
            Email = s.Email?.Trim(),
            FirstName = s.FirstName?.Trim(),
            LastName = s.LastName?.Trim(),
        });

        if (students.Any())
        {
            _context.AddRange(students);
            _context.SaveChanges();
        }

        return students.Count();
    }

    private int ImportTeacher(string fileData)
    {
        var teacherDtos = new List<TeacherImportDto>();

        teacherDtos = GetDataFromCsv<TeacherImportDto>(fileData);

        var teacherCodes = teacherDtos.Select(s => s.Code).ToList();
        var existingTeacher = _context.Teachers.Where(x => teacherCodes.Contains(x.Code)).ToList();

        if (existingTeacher.Any())
        {
            foreach (var teacher in existingTeacher)
            {
                var teacherDto = teacherDtos.FirstOrDefault(s => s.Code.Equals(teacher.Code, StringComparison.OrdinalIgnoreCase));
                if (teacherDto is not null)
                {
                    _logger.LogWarning("Duplicate Teacher: [{Code}]", teacherDto.Code);
                    teacherDtos.Remove(teacherDto);
                }
            }
        }

        var teachers = teacherDtos.Select(s => new Teacher()
        {
            Id = Guid.NewGuid(),
            Code = s.Code?.Trim(),
            Email = s.Email?.Trim(),
            FirstName = s.FirstName?.Trim(),
        });

        if (teachers.Any())
        {
            _context.AddRange(teachers);
            _context.SaveChanges();
        }

        return teachers.Count();
    }

    private int ImportSubject(string fileData)
    {
        var subjectDtos = new List<SubjectImportDto>();

        subjectDtos = GetDataFromCsv<SubjectImportDto>(fileData);

        // TODO Validation Teacher

        var teacherCodes = subjectDtos.Select(s => s.TeacherCode).ToList();
        var subjectCodes = subjectDtos.Select(s => s.Code).ToList();
        var existingTeacher = _context.Teachers.Where(x => teacherCodes.Contains(x.Code)).ToList();
        var existingSubject = _context.Subjects.Where(x => subjectCodes.Contains(x.Code)).ToList();

        if (existingSubject.Any())
        {
            foreach (var subject in existingSubject)
            {
                var subjectDto = subjectDtos.FirstOrDefault(s => s.Code.Equals(subject.Code, StringComparison.OrdinalIgnoreCase));
                if (subjectDto is not null)
                {
                    _logger.LogWarning("Duplicate Subject: [{Code}]", subject.Code);
                    subjectDtos.Remove(subjectDto);
                }
            }
        }

        var subjects = subjectDtos.Select(s => new Subject()
        {
            Id = Guid.NewGuid(),
            Code = s.Code?.Trim(),
            Description = s.Description?.Trim(),
            Name = s.Name?.Trim(),
            TeacherId = existingTeacher.FirstOrDefault(x => x.Code.Equals(s.TeacherCode, StringComparison.OrdinalIgnoreCase))?.Id ?? Guid.Empty
        });

        var subjectsImport = subjects.Where(s => s.TeacherId != Guid.Empty);

        if (subjectsImport.Any())
        {
            _context.AddRange(subjectsImport);
            _context.SaveChanges();
        }

        return subjectsImport.Count();
    }

    private int ImportRoom(string fileData)
    {
        var roomDtos = new List<RoomImportDto>();

        roomDtos = GetDataFromCsv<RoomImportDto>(fileData);

        var roomCodes = roomDtos.Select(r => r.Code).ToList();
        var exsitingRooms = _context.Rooms.Where(r => roomCodes.Contains(r.Code)).ToList();

        if (exsitingRooms.Any())
        {
            foreach (Room room in exsitingRooms)
            {
                RoomImportDto roomDto = roomDtos.FirstOrDefault(s => s.Code.Equals(room.Code, StringComparison.OrdinalIgnoreCase));
                if (roomDto is not null)
                {
                    _logger.LogWarning("Duplicate Room: {Code}", room.Code);
                    roomDtos.Remove(roomDto);
                }
            }
        }

        var rooms = roomDtos.Select(s => new Room()
        {
            Id = Guid.NewGuid(),
            Code = s.Code?.Trim(),
            Location = s.Location?.Trim(),
        });

        if (rooms.Any())
        {
            _context.AddRange(rooms);
            _context.SaveChanges();
        }

        return rooms.Count();
    }

    private int ImportTimetable(string fileData)
    {
        var timetableDtos = new List<TimetableImportDto>();

        timetableDtos = GetDataFromCsv<TimetableImportDto>(fileData);

        var roomCodes = timetableDtos.Select(t => t.RoomCode).ToList();
        var subjectCodes = timetableDtos.Select(t => t.SubjectCode).Distinct().ToList();

        var rooms = _context.Rooms.Where(r => roomCodes.Contains(r.Code)).ToList();
        var subjects = _context.Subjects.Where(s => subjectCodes.Contains(s.Code)).ToList();

        var timetables = timetableDtos.GroupBy(t => t.SubjectCode);

        // TODO Validation

        var sessionDtos = new List<SessionImportDto>();
        foreach (var timetable in timetables)
        {
            var sessionData = timetable.Select(t => new SessionImportDto()
            {
                Id = Guid.NewGuid(),
                Name = t.SessionName?.Trim(),
                Description = t.SessionDescription?.Trim(),
                RoomId = rooms.FirstOrDefault(r => r.Code == t.RoomCode)?.Id ?? Guid.Empty,
                SubjectId = subjects.FirstOrDefault(s => s.Code == t.SubjectCode)?.Id ?? Guid.Empty,
                StartTime = t.StartTime,
                EndTime = t.EndTime,
                TimeSlotId = Guid.NewGuid()
            });

            sessionDtos.AddRange(sessionData);
        }

        var sessions = sessionDtos.Where(s => s.RoomId != Guid.Empty && s.SubjectId != Guid.Empty).Select(s => new Session()
        {
            Id = s.Id,
            Name = s.Name?.Trim(),
            Description = s.Description?.Trim(),
            RoomId = s.RoomId,
            SubjectId = s.SubjectId,
            TimeSlotId = s.TimeSlotId,
            Timeslot = new Timeslot()
            {
                SessionId = s.Id,
                Id = s.TimeSlotId,
                StartTime = s.StartTime,
                EndTime = s.EndTime
            }
        }).ToList();

        ValidationTimeTable(sessions);

        if (sessions.Any())
        {
            _context.AddRange(sessions);
            _context.SaveChanges();
        }

        return sessions.Count;
    }

    private void ValidationTimeTable(List<Session> sessions)
    {
        var subjectIds = sessions.Select(s => s.SubjectId).ToList();
        var existingTimeTables = _context.Sessions
            .Include(s => s.Timeslot)
            .Where(s => subjectIds.Contains(s.SubjectId)).ToList();

        if (!existingTimeTables.Any())
        {
            return;
        }

        foreach (var session in sessions)
        {
            var exisitingSessions = existingTimeTables
                .Where(c => c.RoomId == session.RoomId && c.SubjectId == session.SubjectId);

            if (!exisitingSessions.Any())
            {
                continue;
            }

            foreach (var exisitingSession in exisitingSessions)
            {
                string message = string.Empty;
                if (session.Timeslot is null && exisitingSession.Timeslot is null)
                {
                    message = $"";
                    _logger.LogWarning("Timeslot is null: [{Name}]", session.Name);
                    continue;
                }

                if ((session.Timeslot.StartTime == exisitingSession.Timeslot.StartTime && session.Timeslot.EndTime == exisitingSession.Timeslot.EndTime)
                    || (session.Timeslot.StartTime <= exisitingSession.Timeslot.EndTime && session.Timeslot.EndTime >= exisitingSession.Timeslot.StartTime))
                {
                    _logger.LogWarning("Duplicate Session Or Overlaps: [{Name}]", session.Name);
                    sessions.Remove(session);
                }
            }
        }
    }

    private static List<T> GetDataFromCsv<T>(string fileData)
    {
        var datas = new List<T>();

        using var stringReader = new StringReader(fileData);
        using var csv = new CsvReader(stringReader, CultureInfo.InvariantCulture);
        csv.Read();
        csv.ReadHeader();
        while (csv.Read())
        {
            var row = csv.GetRecord<T>();

            datas.Add(row);
        }

        return datas;
    }

    public byte[] ExportData(DateTime? startDate, DateTime? endDate)
    {
        if (!startDate.HasValue && !endDate.HasValue)
        {
            return Array.Empty<byte>();
        }

        var file = Array.Empty<byte>();

        var appointments = _context.Timeslots
            .Include(t => t.Session)
            .Include(t => t.Session.Room)
            .Include(t => t.Session.Subject)
            .Include(t => t.Session.Subject.Teacher)
            .Where(t => t.StartTime <= endDate && t.EndTime >= startDate)
            .OrderBy(t => t.StartTime)
            .Select(t => new AppointmentMap()
            {
                StartDate = t.StartTime,
                EndDate = t.EndTime,
                Location = t.Session.Room.Location,
                RoomCode = t.Session.Room.Code,
                SessionName = t.Session.Name,
                SubjectName = t.Session.Subject.Name,
                TeacherName = t.Session.Subject.Teacher.GetFullName(),
            }).ToList();

        using (var memoryStream = new MemoryStream())
        {
            using (var writer = new StreamWriter(memoryStream))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteHeader<AppointmentMap>();
                csv.NextRecord();
                foreach (var appointment in appointments)
                {
                    csv.WriteRecord(appointment);
                    csv.NextRecord();
                }
            }

            file = memoryStream.ToArray();
        }

        return file;
    }
}

public class AppointmentMap
{
    public string SubjectName { get; set; }
    public string SessionName { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string TeacherName { get; set; }
    public string RoomCode { get; set; }
    public string Location { get; set; }
}