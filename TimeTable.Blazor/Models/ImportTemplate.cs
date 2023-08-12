namespace TimeTable.Blazor.Models;

public class ImportTemplate
{
    public string Type { get; set; }

    public string Description { get; set; }
}

public enum TemplateType
{
    Student,
    Teacher,
    Subject,
    TimeTable
}