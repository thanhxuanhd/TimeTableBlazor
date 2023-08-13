using CsvHelper;
using Microsoft.Extensions.Options;
using System.Globalization;
using TimeTable.Blazor.Interfaces;
using TimeTable.Blazor.Models;
using TimeTable.Domain;

namespace TimeTable.Blazor.Services;

public class ImportExportService : IImportExportService
{
    private readonly IOptions<List<ImportTemplate>> _options;
    private readonly TimeTableDbContext _context;

    public ImportExportService(IOptions<List<ImportTemplate>> options, TimeTableDbContext context)
    {
        _options = options;
        _context = context;
    }

    public List<ImportTemplate> GetImportTemplates()
    {
        var templates = _options.Value;

        return templates.OrderBy(x => x.Type).ToList();
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

        // TODO Validation Student

        var studentCodes = studentDtos.Select(s => s.Code).ToList();
        var existingStudent = _context.Students.Where(x => studentCodes.Contains(x.Code)).ToList();

        if (existingStudent.Any())
        {
            foreach (var student in existingStudent)
            {
                var studentDto = studentDtos.FirstOrDefault(s => s.Code.Equals(student.Code, StringComparison.OrdinalIgnoreCase));
                if (studentDto is not null)
                {
                    studentDtos.Remove(studentDto);
                }
            }
        }

        var students = studentDtos.Select(s => new Student()
        {
            Id = Guid.NewGuid(),
            Code = s.Code,
            Email = s.Email,
            FirstName = s.FirstName,
            LastName = s.LastName,
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

        // TODO Validation Teacher

        var teacherCodes = teacherDtos.Select(s => s.Code).ToList();
        var existingTeacher = _context.Teachers.Where(x => teacherCodes.Contains(x.Code)).ToList();

        if (existingTeacher.Any())
        {
            foreach (var teacher in existingTeacher)
            {
                var teacherDto = teacherDtos.FirstOrDefault(s => s.Code.Equals(teacher.Code, StringComparison.OrdinalIgnoreCase));
                if (teacherDto is not null)
                {
                    teacherDtos.Remove(teacherDto);
                }
            }
        }

        var teachers = teacherDtos.Select(s => new Teacher()
        {
            Id = Guid.NewGuid(),
            Code = s.Code,
            Email = s.Email,
            FirstName = s.FirstName,
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
                    subjectDtos.Remove(subjectDto);
                }
            }
        }

        var subjects = subjectDtos.Select(s => new Subject()
        {
            Id = Guid.NewGuid(),
            Code = s.Code,
            Description = s.Description,
            Name = s.Name,
            TeacherId = existingTeacher.FirstOrDefault(x => x.Code.Equals(s.TeacherCode, StringComparison.OrdinalIgnoreCase))?.Id ?? Guid.NewGuid()
        });

        if (subjects.Any())
        {
            _context.AddRange(subjects);
            _context.SaveChanges();
        }

        return subjects.Count();
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
                    roomDtos.Remove(roomDto);
                }
            }
        }

        var rooms = roomDtos.Select(s => new Room()
        {
            Id = Guid.NewGuid(),
            Code = s.Code,
            Location = s.Location
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
                Name = t.SessionName,
                Description = t.SessionDescription,
                RoomId = rooms.FirstOrDefault(r => r.Code == t.RoomCode)?.Id ?? Guid.Empty,
                SubjectId = subjects.FirstOrDefault(s => s.Code == t.SubjectCode)?.Id ?? Guid.Empty,
                StartTime = t.StartTime,
                EndTime = t.EndTime,
                TimeSlotId = Guid.NewGuid()
            });

            sessionDtos.AddRange(sessionData);
        }

        var sessions = sessionDtos.Select(s => new Session()
        {
            Id = s.Id,
            Name = s.Name,
            Description = s.Description,
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
        });

        if (sessions.Any())
        {
            _context.AddRange(sessions);
            _context.SaveChanges();
        }

        return sessions.Count();
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
}