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

        return templates;
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

            case TemplateType.TimeTable:
                break;

            default:
                break;
        }

        return importCount;
    }

    private int ImportStudent(string fileData)
    {
        List<StudentImportDto> studentDtos = new List<StudentImportDto>();

        using var stringReader = new StringReader(fileData);
        using var csv = new CsvReader(stringReader, CultureInfo.InvariantCulture);
        csv.Read();
        csv.ReadHeader();
        while (csv.Read())
        {
            var student = csv.GetRecord<StudentImportDto>();

            studentDtos.Add(student);
        }

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

        using var stringReader = new StringReader(fileData);
        using var csv = new CsvReader(stringReader, CultureInfo.InvariantCulture);
        csv.Read();
        csv.ReadHeader();
        while (csv.Read())
        {
            var teacher = csv.GetRecord<TeacherImportDto>();

            teacherDtos.Add(teacher);
        }

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
        List<SubjectImportDto> subjectDtos = new List<SubjectImportDto>();

        using var stringReader = new StringReader(fileData);
        using var csv = new CsvReader(stringReader, CultureInfo.InvariantCulture);
        csv.Read();
        csv.ReadHeader();
        while (csv.Read())
        {
            var subject = csv.GetRecord<SubjectImportDto>();

            subjectDtos.Add(subject);
        }

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
}