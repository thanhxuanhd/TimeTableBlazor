using System.ComponentModel.DataAnnotations;

namespace TimeTable.Domain;

public class Subject : BaseEntity<Guid>
{
    [Required]
    [StringLength(20)]
    public string Code { get; set; }

    [Required]
    public string Name { get; set; }

    public string Description { get; set; }

    public Guid TeacherId { get; set; }

    public virtual Teacher Teacher { get; set; }

    public virtual List<Student> Students { get; set; }

    public virtual List<Session> Sessions { get; set; }
}