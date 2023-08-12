using Radzen;
using TimeTable.Blazor.Models;

namespace TimeTable.Blazor.Interfaces;

public interface IStudentService
{
    public List<StudentDto> GetStudents(LoadDataArgs args, out int count);
}
