using Radzen;
using TimeTable.Blazor.Models;

namespace TimeTable.Blazor.Interfaces;

public interface ISubjectService
{
    public List<SubjectDto> GetSubjects(LoadDataArgs args, out int count);
    public List<SubjectDto> GetSubjects(LoadDataArgs args);
}