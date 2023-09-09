using Radzen;
using TimeTable.Domain.Dtos;

namespace TimeTable.Blazor.Interfaces;

public interface ITeacherService
{
    public List<TeacherDto> GetTeachers(LoadDataArgs args, out int count);

    public List<TeacherDto> GetTeachers(LoadDataArgs args);

    Tuple<bool, List<string>> CreateTeacher(TeacherDto teacherDto);

    Tuple<bool, List<string>> UpdateTeacher(TeacherDto teacherDto);

    Tuple<bool, List<string>> DeleteTeacher(Guid id);

    TeacherDto GetTeacherById(Guid id);

}