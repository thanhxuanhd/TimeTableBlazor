using Microsoft.EntityFrameworkCore;

namespace TimeTable.Domain;

public class TimeTableDbContext : DbContext
{
    public TimeTableDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Teacher>().HasKey(x => x.Id);
        modelBuilder.Entity<Teacher>().HasMany(x => x.Students).WithMany(x => x.Teachers).UsingEntity<StudentTeacher>();
        modelBuilder.Entity<Teacher>().HasMany(x => x.Subjects).WithOne(x => x.Teacher).HasForeignKey(x => x.TeacherId);

        modelBuilder.Entity<Student>().HasKey(x => x.Id);
        modelBuilder.Entity<Student>().HasMany(x => x.Subjects).WithMany(x => x.Students);

        modelBuilder.Entity<Room>().HasKey(x => x.Id);
        modelBuilder.Entity<Room>().HasMany(x => x.Subjects);

        modelBuilder.Entity<Subject>().HasKey(x => x.Id);
        modelBuilder.Entity<Subject>().HasMany(x => x.Students);
        modelBuilder.Entity<Subject>().HasMany(x => x.Sessions);

        modelBuilder.Entity<Session>().HasKey(x => x.Id);
        modelBuilder.Entity<Session>().HasOne(x => x.Timeslot).WithOne(x => x.Session).HasForeignKey<Session>(x => x.TimeSlotId);
        modelBuilder.Entity<Session>().HasOne(x => x.Subject).WithMany(x => x.Sessions).HasForeignKey(x => x.SubjectId);

        modelBuilder.Entity<Timeslot>().HasKey(x => x.Id);
        modelBuilder.Entity<Timeslot>().HasOne(x => x.Session).WithOne(x => x.Timeslot);
    }

    public DbSet<Student> Students { get; set; }

    public DbSet<Teacher> Teachers { get; set; }

    public DbSet<Room> Rooms { get; set; }

    public DbSet<Subject> Subjects { get; set; }

    public DbSet<Timeslot> Timeslots { get; set; }
    public DbSet<Session> Sessions { get; set; }
}