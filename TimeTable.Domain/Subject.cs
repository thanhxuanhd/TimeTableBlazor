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

    public virtual ICollection<Student> Students { get; set; }

    public virtual ICollection<Session> Sessions { get; set; }
}