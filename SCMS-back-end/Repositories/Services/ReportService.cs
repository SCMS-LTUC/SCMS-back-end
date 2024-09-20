using Microsoft.EntityFrameworkCore;
using SCMS_back_end.Data;
using SCMS_back_end.Models.Dto.Response;
using SCMS_back_end.Repositories.Interfaces;
namespace SCMS_back_end.Repositories.Services
{
    public class ReportService : IReport
    {
        private readonly StudyCenterDbContext _context;

        public ReportService(StudyCenterDbContext context)
        {
            _context = context;
        }

        public async Task<StudentEnrollmentOverview> GetStudentEnrollmentOverview()
        {
            var totalStudents = await _context.Students.CountAsync();
            var activeCourses = await _context.Courses.CountAsync(c => c.Schedule.EndDate >= DateTime.Now);
            var averageEnrollmentPerCourse = totalStudents / (double)activeCourses;

            return new StudentEnrollmentOverview
            {
                TotalStudents = totalStudents,
                ActiveCourses = activeCourses,
                AverageEnrollmentPerCourse = averageEnrollmentPerCourse
            };
        }

        public async Task<CoursePerformanceOverview> GetCoursePerformanceOverview()
        {
            var passingStudents = await _context.StudentCourses.CountAsync(sc => sc.AverageGrades >= 50);
            var failingStudents = await _context.StudentCourses.CountAsync(sc => sc.AverageGrades < 50 && sc.AverageGrades > 0);
            var droppingStudents = await _context.StudentCourses.CountAsync(sc => sc.AverageGrades == 0);
            var averageGrade = await _context.StudentCourses.AverageAsync(sc => (double?)sc.AverageGrades) ?? 0;


            var topCourses = await _context.Courses.Select(c => new
                                                   {
                                                       c.ClassName,
                                                       AverageGrade = c.StudentCourses.Select(sc => (double?)sc.AverageGrades ?? 0).DefaultIfEmpty(0).Average()
                                                   })
                                                   .OrderByDescending(c => c.AverageGrade)
                                                   .Take(2)
                                                   .ToListAsync();

            var underperformingCourses = await _context.Courses.Select(c => new
                                                               {
                                                                   c.ClassName,
                                                                   AverageGrade = c.StudentCourses.Select(sc => (double?)sc.AverageGrades ?? 0).DefaultIfEmpty(0).Average()
                                                               })
                                                               .OrderBy(c => c.AverageGrade)
                                                               .Take(2)
                                                               .ToListAsync();

            return new CoursePerformanceOverview
            {
                PassingStudents = passingStudents,
                FailingStudents = failingStudents,
                DroppingStudents = droppingStudents,
                AverageGrade = averageGrade,
                TopPerformingCourses = topCourses.Select(c => new CoursePerformance { CourseName = c.ClassName, AverageGrade = c.AverageGrade }).ToList(),
                UnderperformingCourses = underperformingCourses.Select(c => new CoursePerformance { CourseName = c.ClassName, AverageGrade = c.AverageGrade }).ToList()
            };
        }

        public async Task<InstructorEffectivenessOverview> GetInstructorEffectivenessOverview()
        {
            var instructorCourses = await _context.Teachers
                .Select(t => new
                {
                    t.FullName,
                    CourseCount = t.Courses.Count,
                    AverageStudentPerformance = t.Courses.SelectMany(c => c.StudentCourses)
                                                         .Average(sc => sc.AverageGrades)
                })
                .ToListAsync();

            var topInstructors = instructorCourses
                .OrderByDescending(i => i.AverageStudentPerformance)
                .Take(2)
                .ToList();

            return new InstructorEffectivenessOverview
            {
                InstructorCourses = instructorCourses.Select(i => new InstructorCourseCount { InstructorName = i.FullName, CourseCount = i.CourseCount }).ToList(),
                AverageStudentPerformance = instructorCourses.Select(i => new InstructorPerformance { InstructorName = i.FullName, AveragePerformance = i.AverageStudentPerformance }).ToList(),
                TopInstructors = topInstructors.Select(i => new InstructorPerformance { InstructorName = i.FullName, AveragePerformance = i.AverageStudentPerformance }).ToList()
            };
        }

        public async Task GetAssignmentAndAttendanceOverview()
        {
            throw new NotImplementedException();
        }

        public async Task<DepartmentalActivityOverview> GetDepartmentalActivityOverview()
        {
            var departments = await _context.Departments
                .Select(d => new
                {
                    d.Name,
                    TeacherCount = d.Teachers.Count,
                    SubjectCount = d.Subjects.Count,
                    StudentCount = d.Teachers.SelectMany(t => t.Courses)
                                             .SelectMany(c => c.StudentCourses)
                                             .Select(sc => sc.StudentId)
                                             .Distinct()
                                             .Count()
                })
                .ToListAsync();

            return new DepartmentalActivityOverview
            {
                DepartmentActivities = departments.Select(d => new DepartmentActivity
                {
                    DepartmentName = d.Name,
                    TeacherCount = d.TeacherCount,
                    SubjectCount = d.SubjectCount,
                    StudentCount = d.StudentCount
                }).ToList()
            };
        }

        public async Task<SystemHealthCheckOverview> GetSystemHealthCheckOverview()
        {
            var courses = await _context.Courses
                .Select(c => new
                {
                    c.ClassName,
                    c.Capacity,
                    EnrollmentCount = c.StudentCourses.Count
                })
                .ToListAsync();

            var underCapacityCourses = courses.Count(c => c.EnrollmentCount < c.Capacity);
            var fullCapacityCourses = courses.Count(c => c.EnrollmentCount == c.Capacity);
            var averageUtilization = courses.Average(c => (double)c.EnrollmentCount / c.Capacity * 100);

            return new SystemHealthCheckOverview
            {
                UnderCapacityCourses = underCapacityCourses,
                FullCapacityCourses = fullCapacityCourses,
                AverageCourseUtilization = averageUtilization
            };
        }
    }
}
