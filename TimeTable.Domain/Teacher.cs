using System.ComponentModel.DataAnnotations;

namespace TimeTable.Domain;

public class Teacher : BaseEntity<Guid>
{
    [Required]
    [StringLength(20)]
    public string Code { get; set; }

    [Required]
    [StringLength(100)]
    public string FirstName { get; set; }

    public string LastName { get; set; }

    [EmailAddress]
    public string Email { get; set; }

    public string GetFullName()
    {
        return $"{FirstName} {LastName}";
    }

    public virtual ICollection<Subject> Subjects { get; set; }

    public virtual ICollection<Student> Students { get; set; }
}