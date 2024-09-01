using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SCMS_back_end.Models;

namespace SCMS_back_end.Data
{
    public class StudyCenterDbContext:IdentityDbContext<User>
    {
        public StudyCenterDbContext(DbContextOptions options): base (options)
        {
            
        }
        //public DbSet<Admin> Admins { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<StudentAssignment> StudentAssignments { get; set; }
        public DbSet<Lecture> Lectures { get; set; }
        public DbSet<LectureAttendance> LectureAttendances { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<WeekDay> WeekDays { get; set; }
        public DbSet<ScheduleDay> ScheduleDays { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Unique constraint on LectureAttendance (LectureId, StudentId)
            modelBuilder.Entity<LectureAttendance>()
                .HasIndex(la => new { la.LectureId, la.StudentId })
                .IsUnique();

            // Unique constraint on StudentAssignment (AssignmentId, StudentId)
            modelBuilder.Entity<StudentAssignment>()
                .HasIndex(sa => new { sa.AssignmentId, sa.StudentId })
                .IsUnique();

            // User - Admin/Teacher/Student relationship
            //modelBuilder.Entity<User>()
            //    .HasOne(u => u.Admin)
            //    .WithOne(a => a.User)
            //    .HasForeignKey<Admin>(a => a.UserId);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Teacher)
                .WithOne(t => t.User)
                .HasForeignKey<Teacher>(t => t.UserId);
                

            modelBuilder.Entity<User>()
                .HasOne(u => u.Student)
                .WithOne(s => s.User)
                .HasForeignKey<Student>(s => s.UserId);

            // Department - Teacher relationship
            modelBuilder.Entity<Teacher>()
                .HasOne(t => t.Department)
                .WithMany(d => d.Teachers)
                .HasForeignKey(t => t.DepartmentId);

            // Department - Subject relationship
            modelBuilder.Entity<Subject>()
                .HasOne(s => s.Department)
                .WithMany(d => d.Subjects)
                .HasForeignKey(s => s.DepartmentId);

            // Course - Teacher/Subject/Schedule relationship
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Teacher)
                .WithMany(t => t.Courses)
                .HasForeignKey(c => c.TeacherId);

            modelBuilder.Entity<Course>()
                .HasOne(c => c.Subject)
                .WithMany(s => s.Courses)
                .HasForeignKey(c => c.SubjectId);

            modelBuilder.Entity<Course>()
                .HasOne(c => c.Schedule)
                .WithOne(s => s.Course)
                .HasForeignKey<Course>(c => c.ScheduleId);

            // StudentCourse - Student/Course relationship
            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.Student)
                .WithMany(s => s.StudentCourses)
                .HasForeignKey(sc => sc.StudentId);

            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.Course)
                .WithMany(c => c.StudentCourses)
                .HasForeignKey(sc => sc.CourseId);

            // Assignment - Course relationship
            modelBuilder.Entity<Assignment>()
                .HasOne(a => a.Course)
                .WithMany(c => c.Assignments)
                .HasForeignKey(a => a.CourseId);

            // StudentAssignment - Assignment/Student relationship
            modelBuilder.Entity<StudentAssignment>()
                .HasOne(sa => sa.Assignment)
                .WithMany(a => a.StudentAssignments)
                .HasForeignKey(sa => sa.AssignmentId);

            modelBuilder.Entity<StudentAssignment>()
                .HasOne(sa => sa.Student)
                .WithMany(s => s.StudentAssignments)
                .HasForeignKey(sa => sa.StudentId);

            // Lecture - Course relationship
            modelBuilder.Entity<Lecture>()
                .HasOne(l => l.Course)
                .WithMany(c => c.Lectures)
                .HasForeignKey(l => l.CourseId);

            // LectureAttendance - Lecture/Student relationship
            modelBuilder.Entity<LectureAttendance>()
                .HasOne(la => la.Lecture)
                .WithMany(l => l.LectureAttendances)
                .HasForeignKey(la => la.LectureId);

            modelBuilder.Entity<LectureAttendance>()
                .HasOne(la => la.Student)
                .WithMany(s => s.LectureAttendances)
                .HasForeignKey(la => la.StudentId);

            // ScheduleDay - Schedule/WeekDay relationship
            modelBuilder.Entity<ScheduleDay>()
                .HasOne(sd => sd.Schedule)
                .WithMany(s => s.ScheduleDays)
                .HasForeignKey(sd => sd.ScheduleId);

            modelBuilder.Entity<ScheduleDay>()
                .HasOne(sd => sd.WeekDay)
                .WithMany(wd => wd.ScheduleDays)
                .HasForeignKey(sd => sd.WeekDayId);

            modelBuilder.Entity<IdentityRole>().HasData(
           new IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
           new IdentityRole { Id = "2", Name = "Teacher", NormalizedName = "TEACHER" },
           new IdentityRole { Id = "3", Name = "Student", NormalizedName = "STUDENT" }
            );


        }

    }
}
