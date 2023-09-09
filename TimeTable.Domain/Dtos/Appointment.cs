namespace TimeTable.Domain.Dtos;

public class Appointment
{
    public Guid Id { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string Text
    {
        get
        {
            return $"{SessionName} - {SubjectName} - [{TeacherName}] - {RoomCode} - {Location}";
        }
    }

    public string SubjectName { get; set; }

    public string SessionName { get; set; }

    public string RoomCode { get; set; }

    public Guid RoomId { get; set; }

    public Guid SujectId { get; set; }

    public string Location { get; set; }

    public string TeacherName { get; set; }

    public string Description { get; set; }
}

public class AppointmentDto
{
    public Guid? Id { get; set; }

    public string Description { get; set; }

    public string SessionName { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public Guid SubjectId { get; set; }

    public Guid RoomId { get; set; }
}