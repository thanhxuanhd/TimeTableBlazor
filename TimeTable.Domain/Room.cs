using System.ComponentModel.DataAnnotations;

namespace TimeTable.Domain;

public class Room : BaseEntity<Guid>
{
    [Required]
    [MaxLength(20)]
    public string Code { get; set; }

    public string Location { get; set; }

    public virtual List<Subject> Subjects { get; set; }
}