using Radzen;
using TimeTable.Blazor.Models;

namespace TimeTable.Blazor.Interfaces;

public interface IStudentService
{
    List<StudentDto> GetStudents(LoadDataArgs args, out int count);

    Tuple<bool, List<string>> CreateStudent(StudentDto studentDto);

    Tuple<bool, List<string>> UpdateStudent(StudentDto studentDto);

    Tuple<bool, List<string>> DeleteStudent(Guid id);

    StudentDto GetStudentById(Guid id);
}
