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

    private const string ErrorMessage = "Error: {message}";

    public StudentService(TimeTableDbContext context, ILogger<StudentService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public Tuple<bool, List<string>> CreateStudent(StudentDto studentDto)
    {
        var success = true;
        var errors = new List<string>();
        try
        {
            if (!ValidationStudent(studentDto, errors))
            {
                success = false;
                return Tuple.Create(success, errors);
            }

            if (IsDuplicate(studentDto.Code, errors))
            {
                success = false;
                return Tuple.Create(success, errors);
            }

            if (!success)
            {
                return Tuple.Create(success, errors);
            }

            var student = new Student()
            {
                Id = Guid.NewGuid(),
                Code = studentDto.Code?.Trim(),
                Email = studentDto.Email?.Trim(),
                FirstName = studentDto.FirstName?.Trim(),
                LastName = studentDto.LastName?.Trim()
            };

            _context.Add(student);
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            success = false;
            _logger.LogError(ErrorMessage, ex.Message);
        }
        return Tuple.Create(success, errors);
    }

    public Tuple<bool, List<string>> DeleteStudent(Guid id)
    {
        var success = true;
        string message;
        var errors = new List<string>();
        try
        {
            var student = GetStudent(id);

            if (student is null)
            {
                message = "Student doesn't exist.";
                errors.Add(message);
                _logger.LogError(ErrorMessage, message);
                success = false;
            }

            _context.Remove(student);
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            success = false;
            message = ex.Message;
            _logger.LogError(ErrorMessage, message);
        }

        return Tuple.Create(success, errors);
    }

    public StudentDto GetStudentById(Guid id)
    {
        var student = _context.Students.FirstOrDefault(s => s.Id == id);
        return student is not null ? new StudentDto()
        {
            Id = student.Id,
            Code = student.Code?.Trim(),
            Email = student.Email?.Trim(),
            FirstName = student.FirstName?.Trim(),
            LastName = student.LastName?.Trim()
        } : new StudentDto();
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

    public Tuple<bool, List<string>> UpdateStudent(StudentDto studentDto)
    {
        var success = true;
        var errors = new List<string>();

        try
        {
            if (!ValidationStudent(studentDto, errors))
            {
                success = false;
            }

            if (IsDuplicate(studentDto.Code, errors, studentDto.Id))
            {
                success = false;
            }

            if (!success)
            {
                return Tuple.Create(success, errors);
            }
            var student = GetStudent(studentDto.Id.Value);

            if (student is null)
            {
                var message = $"The student doesn't exist with Id: [{studentDto.Id.Value}]";
                errors.Add(message);
                _logger.LogError(ErrorMessage, message);
                success = false;
                return Tuple.Create(success, errors);
            }

            student.FirstName = studentDto.FirstName?.Trim();
            student.LastName = studentDto.LastName?.Trim();
            student.Email = studentDto.Email?.Trim();
            student.Code = studentDto.Code?.Trim();

            _context.Update(student);
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            success = false;
            _logger.LogError("Errors: {message}", ex.Message);
        }

        return Tuple.Create(success, errors);
    }

    private bool ValidationStudent(StudentDto student, List<string> errors)
    {
        bool valid = true;
        string message;
        if (string.IsNullOrWhiteSpace(student.FirstName))
        {
            message = "First Name is required.";
            errors.Add(message);
            _logger.LogError(ErrorMessage, message);
            valid = false;
        }

        if (string.IsNullOrWhiteSpace(student.Code))
        {
            message = "Student Code is required.";
            errors.Add(message);
            _logger.LogError(ErrorMessage, message);
            valid &= false;
        }

        if (valid && student.Code?.Length > 20)
        {
            message = "Student Code greater than  greater than 20 characters.";
            errors.Add(message);
            _logger.LogError(ErrorMessage, message);
            valid &= false;
        }

        return valid;
    }

    private Student GetStudent(Guid id)
    {
        return _context.Students.FirstOrDefault(s => s.Id == id);
    }

    private bool IsDuplicate(string studentCode, List<string> errors, Guid? id = null)
    {
        var isDuplicate = false;
        Student studentExist;

        if (!id.HasValue)
        {
            studentExist = _context.Students.AsEnumerable().FirstOrDefault(s => s.Code.Equals(studentCode?.Trim(), StringComparison.OrdinalIgnoreCase));
        }
        else
        {
            studentExist = _context.Students.AsEnumerable().FirstOrDefault(s => s.Code.Equals(studentCode?.Trim(), StringComparison.OrdinalIgnoreCase) && s.Id != id);
        }

        isDuplicate = studentExist is not null;

        if (isDuplicate)
        {
            var message = $"Student is duplicate with Code [{studentCode}]";
            errors.Add(message);
            _logger.LogError(ErrorMessage, message);
        }
        return isDuplicate;
    }
}