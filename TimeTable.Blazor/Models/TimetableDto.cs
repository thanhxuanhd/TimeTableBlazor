namespace TimeTable.Blazor.Models;

public class TimetableDto
{
}

public class TimetableImportDto
{
    public string SubjectCode { get; set; }

    public string SessionName { get; set; }

    public string SessionDescription { get; set; }

    public string RoomCode { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }
}