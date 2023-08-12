namespace TimeTable.Domain;

public class StudentTeacher : BaseEntity<Guid>
{
    public Guid TeacherId { get; set; }
    public Guid StudentId { get; set; }

}
