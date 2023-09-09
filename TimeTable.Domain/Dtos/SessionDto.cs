namespace TimeTable.Domain.Dtos;

public class SessionDto
{
}

public class SessionImportDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    public string Description { get; set; }

    public Guid TimeSlotId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public Guid RoomId { get; set; }

    public Guid SubjectId { get; set; }
}