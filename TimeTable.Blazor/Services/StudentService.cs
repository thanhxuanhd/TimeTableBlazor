using Microsoft.EntityFrameworkCore;
using Radzen;
using System.Linq.Dynamic.Core;
using TimeTable.Blazor.Interfaces;
using TimeTable.Blazor.Models;
using TimeTable.Domain;

namespace TimeTable.Blazor.Services;

public class StudentService : IStudentService
{
    private readonly TimeTableDbContext _context;

    private readonly ILogger<StudentService> _logger;

    public StudentService(TimeTableDbContext context, ILogger<StudentService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public bool CreateStudent(StudentDto studentDto)
    {
        var success = true;

        try
        {
            if (!ValidationStudent(studentDto))
            {
                success = false;
                return success;
            }

            var studentExist = _context.Students.AsEnumerable().Any(s => s.Code.Equals(studentDto.Code, StringComparison.OrdinalIgnoreCase));

            if (studentExist)
            {
                success = false;
                _logger.LogError($"Student [{studentDto.Code}] is duplicated.");
                return success;
            }

            var student = new Student()
            {
                Id = Guid.NewGuid(),
                Code = studentDto.Code,
                Email = studentDto.Email,
                FirstName = studentDto.FirstName,
                LastName = studentDto.LastName
            };

            _context.Add(student);
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            success = false;
            _logger.LogError($"Errors: {ex.Message}");
        }
        return success;
    }

    public List<StudentDto> GetStudents(LoadDataArgs args, out int count)
    {
        var query = _context.Students.AsQueryable();

        if (!string.IsNullOrEmpty(args.Filter))
        {
            // Filter via the Where method
            query = query.Where(args.Filter);
        }

        if (!string.IsNullOrEmpty(args.OrderBy))
        {
            // Sort via the OrderBy method
            query = query.OrderBy(args.OrderBy);
        }

        // Important!!! Make sure the Count property of RadzenDataGrid is set.
        count = query.Count();

        // Perform paging via Skip and Take.
        return query.Skip(args.Skip.Value).Take(args.Top.Value).Select(s => new StudentDto()
        {
            Code = s.Code,
            Email = s.Email,
            FirstName = s.FirstName,
            Id = s.Id,
            LastName = s.LastName
        }).AsNoTracking().ToList();
    }

    bool ValidationStudent(StudentDto student)
    {
        bool valid = true;

        if (string.IsNullOrWhiteSpace(student.FirstName))
        {
            _logger.LogError("First Name is required.");
            valid = false;
        }

        if (string.IsNullOrWhiteSpace(student.Code))
        {
            _logger.LogError("Student Code is required.");
            valid = false;
        }

        return valid;
    }
}