namespace TimeTable.Blazor.Models;

public class SubjectDto
{
    public Guid? Id { get; set; }

    public string Code { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public Guid? TeacherId { get; set; }

    public string TeacherName { get; set; }
}

public class SubjectImportDto
{
    public string Code { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string TeacherCode { get; set; }
}