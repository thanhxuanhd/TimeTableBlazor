namespace TimeTable.Domain;

public class Room : BaseEntity<Guid>
{
    public string Name { get; set; }

    public string Location { get; set; }

    public virtual List<Subject> Subjects { get; set; }
}