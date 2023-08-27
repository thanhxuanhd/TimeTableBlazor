using Radzen;
using TimeTable.Blazor.Models;

namespace TimeTable.Blazor.Interfaces;

public interface ITeacherService
{
    public List<TeacherDto> GetTeachers(LoadDataArgs args, out int count);

    public List<TeacherDto> GetTeachers(LoadDataArgs args);
}