namespace TimeTable.Domain;

public class Timeslot : BaseEntity<Guid>
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public Guid? SessionId { get; set; }

    public virtual Session Session { get; set; }
}