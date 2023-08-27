using Microsoft.EntityFrameworkCore;
using Radzen;
using System.Linq.Dynamic.Core;
using TimeTable.Blazor.Interfaces;
using TimeTable.Blazor.Models;
using TimeTable.Domain;

namespace TimeTable.Blazor.Services;

public class SubjectService : ISubjectService
{
    private readonly TimeTableDbContext _context;

    public SubjectService(TimeTableDbContext context)
    {
        _context = context;
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
}