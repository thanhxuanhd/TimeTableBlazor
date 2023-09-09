using Microsoft.EntityFrameworkCore;
using Radzen;
using System.Linq.Dynamic.Core;
using TimeTable.Blazor.Interfaces;
using TimeTable.Domain;
using TimeTable.Domain.Dtos;

namespace TimeTable.Blazor.Services;

public class SubjectService : ISubjectService
{
    private readonly TimeTableDbContext _context;
    private readonly ILogger<SubjectService> _logger;

    private const string ErrorMessage = "Error: {message}";

    public SubjectService(TimeTableDbContext context, ILogger<SubjectService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public Tuple<bool, List<string>> CreateSubject(SubjectDto subjectDto)
    {
        var success = true;
        var errors = new List<string>();
        try
        {
            if (!ValidateSubject(subjectDto, errors))
            {
                success = false;
                return Tuple.Create(success, errors);
            }

            if (IsDuplicate(subjectDto.Code, errors))
            {
                success = false;
                return Tuple.Create(success, errors);
            }

            var subject = new Subject()
            {
                Id = Guid.NewGuid(),
                Code = subjectDto.Code?.Trim(),
                Description = subjectDto.Description?.Trim(),
                Name = subjectDto.Name?.Trim(),
                TeacherId = subjectDto.TeacherId.Value
            };

            _context.Add(subject);
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            AddError(errors, ex.Message);
        }

        return Tuple.Create(success, errors);
    }

    public Tuple<bool, List<string>> DeleteSubject(Guid id)
    {
        var success = true;
        var errors = new List<string>();

        try
        {
            var subject = GetSubject(id);

            if (subject is null)
            {
                AddError(errors, "Subject doesn't exist.");
                success = false;

                return Tuple.Create(success, errors);
            }

            if (!CanRemoveSubject(subject.Id))
            {
                AddError(errors, $"Subject [{subject.Code}] is used in Session.");
                success = false;

                return Tuple.Create(success, errors);
            }

            _context.Remove(subject);
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            AddError(errors, ex.Message);
            success = false;
        }

        return Tuple.Create(success, errors);
    }

    public SubjectDto GetSubjectById(Guid id)
    {
        var subject = GetSubject(id);

        return subject is null ? new SubjectDto() : new SubjectDto()
        {
            Code = subject.Code?.Trim(),
            Description = subject.Description?.Trim(),
            Id = subject.Id,
            Name = subject.Name?.Trim(),
            TeacherId = subject.TeacherId
        };
    }

    public List<SubjectDto> GetSubjects(LoadDataArgs args, out int count)
    {
        var query = _context.Subjects.Include(x => x.Teacher).AsQueryable();

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
        return query.Skip(args.Skip.Value).Take(args.Top.Value).Select(s => new SubjectDto()
        {
            Description = s.Description,
            Name = s.Name,
            TeacherName = $"{s.Teacher.FirstName} {s.Teacher.LastName}",
            TeacherId = s.TeacherId,
            Id = s.Id,
            Code = s.Code
        }).AsNoTracking().ToList();
    }

    public List<SubjectDto> GetSubjects(LoadDataArgs args)
    {
        var query = _context.Subjects.Include(x => x.Teacher).AsQueryable();

        if (!string.IsNullOrEmpty(args.Filter))
        {
            query = query.Where(c => c.Code.ToLower().Contains(args.Filter.ToLower()) || c.Name.ToLower().Contains(args.Filter.ToLower()));
        }

        return query.Select(s => new SubjectDto()
        {
            Description = s.Description,
            Name = s.Name,
            TeacherName = $"{s.Teacher.FirstName} {s.Teacher.LastName}",
            TeacherId = s.TeacherId,
            Id = s.Id,
            Code = s.Code
        }).AsNoTracking().ToList();
    }

    public Tuple<bool, List<string>> UpdateSubject(SubjectDto subjectDto)
    {
        var success = true;
        var errors = new List<string>();
        try
        {
            if (!ValidateSubject(subjectDto, errors))
            {
                success = false;
                return Tuple.Create(success, errors);
            }

            if (IsDuplicate(subjectDto.Code, errors, subjectDto.Id.Value))
            {
                success = false;
                return Tuple.Create(success, errors);
            }

            var subject = GetSubject(subjectDto.Id.Value);

            if (subject is null)
            {
                success = false;
                AddError(errors, "Subject doesn't exist.");
                return Tuple.Create(success, errors);
            }

            subject.Code = subjectDto?.Code.Trim();
            subject.TeacherId = subject.TeacherId;
            subject.Name = subjectDto.Name?.Trim();
            subject.Description = subjectDto.Description.Trim();

            _context.Update(subject);
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            AddError(errors, ex.Message);
        }

        return Tuple.Create(success, errors);
    }

    private bool ValidateSubject(SubjectDto subject, List<string> errors)
    {
        var isValid = true;

        if (string.IsNullOrEmpty(subject.Code))
        {
            isValid = false;
            AddError(errors, "Subject Code is required.");
        }

        if (string.IsNullOrEmpty(subject.Name))
        {
            isValid &= false;
            AddError(errors, "Subject Name is required.");
        }

        if (isValid && subject.Code?.Length > 20)
        {
            isValid &= false;
            AddError(errors, "Subject Code greater than  greater than 20 characters.");
        }

        if (isValid && !subject.TeacherId.HasValue)
        {
            isValid &= false;
            AddError(errors, "Subject Teacher is required");

        }

        return isValid;
    }

    private bool IsDuplicate(string code, List<string> errors, Guid? id = null)
    {
        Subject subject = null;

        if (!id.HasValue)
        {
            subject = _context.Subjects.AsEnumerable().FirstOrDefault(s => s.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
        }
        else
        {
            subject = _context.Subjects.AsEnumerable().FirstOrDefault(s => s.Code.Equals(code, StringComparison.OrdinalIgnoreCase) && s.Id != id);
        }

        var isDuplicate = subject is not null;

        if (isDuplicate)
        {
            AddError(errors, $"Subject is duplicate with [{code}]");
        }

        return isDuplicate;
    }

    private Subject GetSubject(Guid id)
    {
        return _context.Subjects.Include(s => s.Teacher).FirstOrDefault(s => s.Id == id);
    }

    private bool CanRemoveSubject(Guid subjectId)
    {
        var session = _context.Sessions.FirstOrDefault(s => s.SubjectId == subjectId);

        return session is null;
    }

    private void AddError(List<string> errors, string message)
    {
        errors.Add(message);
        _logger.LogError(ErrorMessage, message);
    }
}