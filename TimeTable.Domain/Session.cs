namespace TimeTable.Domain;

public class Session : BaseEntity<Guid>
{
    public string Name { get; set; }

    public string Description { get; set; }

    public Guid? TimeSlotId { get; set; }

    public Guid RoomId { get; set; }

    public virtual Room Room
    {
        get; set;
    }

    public virtual Timeslot Timeslot { get; set; }
}