namespace TimeTable.Blazor.Models;

public class Appointment
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public string Text
    {
        get
        {
            return $"{SubjectName} - {SessionName} [{TeacherName}] - {RoomCode} - {Location}";
        }
    }

    public string SubjectName { get; set; }
    public string SessionName { get; set; }

    public string RoomCode { get; set; }

    public string Location { get; set; }

    public string TeacherName { get; set; }
}