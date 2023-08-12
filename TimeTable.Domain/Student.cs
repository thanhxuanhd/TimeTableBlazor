using System.ComponentModel.DataAnnotations;

namespace TimeTable.Domain;

public class Student : BaseEntity<Guid>
{
    [Required]
    [StringLength(20)]
    public string Code { get; set; }

    [Required]
    public string FirstName { get; set; }

    public string LastName { get; set; }

    [EmailAddress]
    public string Email { get; set; }

    public virtual ICollection<Teacher> Teachers { get; set; }
    public virtual ICollection<Subject> Subjects { get; set; }
}