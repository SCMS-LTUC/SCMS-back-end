﻿using Microsoft.AspNetCore.Identity;
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

        // For Quiz
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<AnswerOption> AnswerOptions { get; set; }
        public DbSet<StudentAnswer> StudentAnswers { get; set; }
        public DbSet<QuizResult> QuizResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Quiz>()
                .HasOne(q => q.Course)
                .WithMany(c => c.Quizzes)
                .HasForeignKey(q => q.CourseId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for quizzes when a course is deleted

            // One-to-many relationships
            modelBuilder.Entity<Question>()
                .HasOne(q => q.Quiz)
                .WithMany(qz => qz.Questions)
                .HasForeignKey(q => q.QuizId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for quiz questions

            modelBuilder.Entity<AnswerOption>()
                .HasOne(ao => ao.Question)
                .WithMany(q => q.AnswerOptions)
                .HasForeignKey(ao => ao.QuestionId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for answer options


            // Define relationships for StudentAnswer
            modelBuilder.Entity<StudentAnswer>()
                .HasOne(sa => sa.Quiz)
                .WithMany(q => q.StudentAnswers)
                .HasForeignKey(sa => sa.QuizId)
                .OnDelete(DeleteBehavior.Restrict); // Restrict cascading delete

            modelBuilder.Entity<StudentAnswer>()
                .HasOne(sa => sa.Question)
                .WithMany(q => q.StudentAnswers)
                .HasForeignKey(sa => sa.QuestionId)
                .OnDelete(DeleteBehavior.Restrict); // Restrict cascading delete

            modelBuilder.Entity<StudentAnswer>()
                .HasOne(sa => sa.SelectedAnswerOption)
                .WithMany(ao => ao.StudentAnswers)
                .HasForeignKey(sa => sa.SelectedAnswerOptionId)
                .OnDelete(DeleteBehavior.Restrict); // Restrict cascading delete

            modelBuilder.Entity<StudentAnswer>()
                .HasOne(sa => sa.Student)
                .WithMany() // Assuming many-to-one relationship
                .HasForeignKey(sa => sa.StudentId)
                .OnDelete(DeleteBehavior.Restrict); // Restrict cascading delete

            // Define relationships for QuizResult
            modelBuilder.Entity<QuizResult>()
                .HasOne(qr => qr.Quiz)
                .WithMany() // Assuming one result per student per quiz
                .HasForeignKey(qr => qr.QuizId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for quiz results

            modelBuilder.Entity<QuizResult>()
                .HasOne(qr => qr.Student)
                .WithMany() // Assuming a many-to-one relation
                .HasForeignKey(qr => qr.StudentId)
                .OnDelete(DeleteBehavior.Restrict); // Restrict cascading delete

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
                .HasForeignKey<Teacher>(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Student)
                .WithOne(s => s.User)
                .HasForeignKey<Student>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Departments - Teacher relationship
            modelBuilder.Entity<Teacher>()
                .HasOne(t => t.Department)
                .WithMany(d => d.Teachers)
                .HasForeignKey(t => t.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Departments - Subject relationship
            modelBuilder.Entity<Subject>()
                .HasOne(s => s.Department)
                .WithMany(d => d.Subjects)
                .HasForeignKey(s => s.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Course - Teacher/Subject/Schedule relationship
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Teacher)
                .WithMany(t => t.Courses)
                .HasForeignKey(c => c.TeacherId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Course>()
                .HasOne(c => c.Subject)
                .WithMany(s => s.Courses)
                .HasForeignKey(c => c.SubjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Course>()
                .HasOne(c => c.Schedule)
                .WithOne(s => s.Course)
                .HasForeignKey<Course>(c => c.ScheduleId)
                .OnDelete(DeleteBehavior.Cascade);

            // StudentCourse - Student/Course relationship
            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.Student)
                .WithMany(s => s.StudentCourses)
                .HasForeignKey(sc => sc.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.Course)
                .WithMany(c => c.StudentCourses)
                .HasForeignKey(sc => sc.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Assignment - Course relationship
            modelBuilder.Entity<Assignment>()
                .HasOne(a => a.Course)
                .WithMany(c => c.Assignments)
                .HasForeignKey(a => a.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // StudentAssignment - Assignment/Student relationship
            modelBuilder.Entity<StudentAssignment>()
                .HasOne(sa => sa.Assignment)
                .WithMany(a => a.StudentAssignments)
                .HasForeignKey(sa => sa.AssignmentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StudentAssignment>()
                .HasOne(sa => sa.Student)
                .WithMany(s => s.StudentAssignments)
                .HasForeignKey(sa => sa.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Lecture - Course relationship
            modelBuilder.Entity<Lecture>()
                .HasOne(l => l.Course)
                .WithMany(c => c.Lectures)
                .HasForeignKey(l => l.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // LectureAttendance - Lecture/Student relationship
            modelBuilder.Entity<LectureAttendance>()
                .HasOne(la => la.Lecture)
                .WithMany(l => l.LectureAttendances)
                .HasForeignKey(la => la.LectureId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LectureAttendance>()
                .HasOne(la => la.Student)
                .WithMany(s => s.LectureAttendances)
                .HasForeignKey(la => la.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            // ScheduleDay - Schedule/WeekDay relationship
            modelBuilder.Entity<ScheduleDay>()
                .HasOne(sd => sd.Schedule)
                .WithMany(s => s.ScheduleDays)
                .HasForeignKey(sd => sd.ScheduleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ScheduleDay>()
                .HasOne(sd => sd.WeekDay)
                .WithMany(wd => wd.ScheduleDays)
                .HasForeignKey(sd => sd.WeekDayId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<IdentityRole>().HasData(
           new IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
           new IdentityRole { Id = "2", Name = "Teacher", NormalizedName = "TEACHER" },
           new IdentityRole { Id = "3", Name = "Student", NormalizedName = "STUDENT" }
            );

            //modelBuilder.Entity<WeekDay>().HasData(
            //    new WeekDay { WeekDayId = 1,   Name="Saturday" },
            //    new WeekDay { WeekDayId = 2, Name = "Sunday" },
            //    new WeekDay { WeekDayId = 3, Name = "Monday" },
            //    new WeekDay { WeekDayId = 4, Name = "Tuesday" },
            //    new WeekDay { WeekDayId = 5, Name = "Wednesday" },
            //    new WeekDay { WeekDayId = 6, Name = "Thursday" },
            //    new WeekDay { WeekDayId = 7, Name = "Friday" }

            //    );

        }

    }
}
