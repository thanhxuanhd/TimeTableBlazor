namespace TimeTable.Blazor.Models;

public class TeacherDto
{
    public Guid? Id { get; set; }

    public string Code { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public string DisplayInfo => $"{Code} - {FirstName} {LastName}";
}

public class TeacherImportDto
{
    public string Code { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }
}