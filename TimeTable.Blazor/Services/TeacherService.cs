using Microsoft.EntityFrameworkCore;
using Radzen;
using System.Linq.Dynamic.Core;
using TimeTable.Blazor.Interfaces;
using TimeTable.Blazor.Models;
using TimeTable.Domain;

namespace TimeTable.Blazor.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly TimeTableDbContext _context;

        public TeacherService(TimeTableDbContext context)
        {
            _context = context;
        }

        public List<TeacherDto> GetTeachers(LoadDataArgs args, out int count)
        {
            var query = _context.Teachers.AsQueryable();

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
            return query.Skip(args.Skip.Value).Take(args.Top.Value).Select(s => new TeacherDto()
            {
                Code = s.Code,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Id = s.Id
            }).AsNoTracking().ToList();
        }
    }
}