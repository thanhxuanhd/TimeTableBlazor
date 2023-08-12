using System.ComponentModel.DataAnnotations;

namespace TimeTable.Blazor.Models;

public class StudentDto
{
    public Guid Id { get; set; }

    public string Code { get; set; }
    public string FirstName { get; set; }

    public string LastName { get; set; }

    [EmailAddress]
    public string Email { get; set; }
}

public class StudentImportDto
{
    public string Code { get; set; }
    public string FirstName { get; set; }

    public string LastName { get; set; }

    [EmailAddress]
    public string Email { get; set; }
}

public class Appointment
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string Text { get; set; }
}
