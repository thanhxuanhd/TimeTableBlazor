using Radzen;
using TimeTable.Domain.Dtos;

namespace TimeTable.Blazor.Interfaces;

public interface ISubjectService
{
    public List<SubjectDto> GetSubjects(LoadDataArgs args, out int count);

    public List<SubjectDto> GetSubjects(LoadDataArgs args);

    Tuple<bool, List<string>> CreateSubject(SubjectDto subjectDto);

    Tuple<bool, List<string>> UpdateSubject(SubjectDto subjectDto);

    Tuple<bool, List<string>> DeleteSubject(Guid id);

    SubjectDto GetSubjectById(Guid id);
}