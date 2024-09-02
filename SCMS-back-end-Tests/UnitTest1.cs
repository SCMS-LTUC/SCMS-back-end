using Moq;
using SCMS_back_end.Data;
using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto.Request;
using SCMS_back_end.Models.Dto.Response;
using SCMS_back_end.Repositories.Services;
using Microsoft.EntityFrameworkCore;

namespace SCMS_back_end_Tests
{
    public class UnitTest1
    {
        private readonly StudyCenterDbContext _context;
        private readonly CourseService _courseService;

        public UnitTest1()
        {
            var options = new DbContextOptionsBuilder<StudyCenterDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new StudyCenterDbContext(options);
            _courseService = new CourseService(_context);
        }
        [Fact]
        public async Task CreateCourseWithoutTeacher_ShouldReturnCourse()
        {
            // Arrange
            var subject = new Subject { SubjectId = 1, DepartmentId = 1 };
            var department = new Department { DepartmentId = 1 };
            var schedule = new Schedule { ScheduleId = 1 };
            var scheduleDay = new ScheduleDay { ScheduleId = 1 };


            var courseRequest = new DtoCreateCourseWTRequest
            {
                SubjectId = 1,
                ScheduleId = 1,
                ClassName = "Math 101",
                Capacity = 30,
                Level = 1
            };
            await _context.Subjects.AddAsync(subject);
            await _context.Departments.AddAsync(department);
            await _context.Schedules.AddAsync(schedule);
            await _context.ScheduleDays.AddAsync(scheduleDay);
            await _context.SaveChangesAsync();

            // Act
            var result = await _courseService.CreateCourseWithoutTeacher(courseRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(courseRequest.ClassName, result.ClassName);
            Assert.Equal(courseRequest.Capacity, result.Capacity);
            Assert.Equal(courseRequest.Level, result.Level);
        }
        [Fact]
        public async Task UpdateCourseInformation_ShouldUpdateCourse()
        {
            // Arrange
            var subject = new Subject { SubjectId = 1, DepartmentId = 1 };
            var department = new Department { DepartmentId = 1 };
            var teacher = new Teacher { TeacherId = 1,DepartmentId = 1 };
            var schedule = new Schedule { ScheduleId = 1 };
            var scheduleDay = new ScheduleDay { ScheduleId = 1 };

            var course = new Course
            {
                CourseId = 1,
                ClassName = "Math 101",
                Capacity = 30,
                Level = 1,
                SubjectId = 1,
                ScheduleId = 1
            };

            await _context.Teachers.AddAsync(teacher);
            await _context.Subjects.AddAsync(subject);
            await _context.Departments.AddAsync(department);
            await _context.Schedules.AddAsync(schedule);
            await _context.ScheduleDays.AddAsync(scheduleDay);
            await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();

            var updateRequest = new DtoUpdateCourseRequest
            {
                TeacherId = 1,
                SubjectId = 1,
                ScheduleId = 1,
                ClassName = "Advanced Math 101",
                Capacity = 40,
                Level = 2
            };

            // Act
            var result = await _courseService.UpdateCourseInformation(course.CourseId, updateRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(updateRequest.ClassName, result.ClassName);
            Assert.Equal(updateRequest.TeacherId, result.TeacherId);
            Assert.Equal(updateRequest.Capacity, result.Capacity);
            Assert.Equal(updateRequest.Level, result.Level);
        }
        [Fact]
        public async Task GetCourseById_ShouldReturnCourse()
        {
            // Arrange
            var subject = new Subject { SubjectId = 1, DepartmentId = 1 };
            var department = new Department { DepartmentId = 1 };
            var teacher = new Teacher { TeacherId = 1, DepartmentId = 1 };
            var schedule = new Schedule { ScheduleId = 1 };
            var scheduleDay = new ScheduleDay { ScheduleId = 1 };

            var course = new Course
            {
                CourseId = 1,
                TeacherId = 1,
                ClassName = "Math 101",
                Capacity = 30,
                Level = 1,
                SubjectId = 1,
                ScheduleId = 1
            };

            await _context.Teachers.AddAsync(teacher);
            await _context.Subjects.AddAsync(subject);
            await _context.Departments.AddAsync(department);
            await _context.Schedules.AddAsync(schedule);
            await _context.ScheduleDays.AddAsync(scheduleDay);
            await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();

            // Act
            var result = await _courseService.GetCourseById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(course.ClassName, result.ClassName);
            Assert.Equal(course.Capacity, result.Capacity);
            Assert.Equal(course.Level, result.Level);
        }
        [Fact]
        public async Task GetAllCourses_ShouldReturnAllCourses()
        {
            // Arrange
            var subject = new Subject { SubjectId = 1, DepartmentId = 1 };
            var department = new Department { DepartmentId = 1 };
            var teacher = new Teacher { TeacherId = 1, DepartmentId = 1 };
            var schedule = new Schedule { ScheduleId = 1, StartDate = DateTime.Now.AddDays(-1), EndDate = DateTime.Now.AddDays(1), StartTime = TimeSpan.FromHours(9), EndTime = TimeSpan.FromHours(10) };
            var scheduleDay = new ScheduleDay { ScheduleId = 1, WeekDayId = 1 };
            var weekDay = new WeekDay { WeekDayId = 1, Name = "Monday" };

            var course = new Course
            {
                CourseId = 1,
                TeacherId = 1,
                ClassName = "Math 101",
                Capacity = 30,
                Level = 1,
                SubjectId = 1,
                ScheduleId = 1
            };

            await _context.Teachers.AddAsync(teacher);
            await _context.Subjects.AddAsync(subject);
            await _context.Departments.AddAsync(department);
            await _context.Schedules.AddAsync(schedule);
            await _context.ScheduleDays.AddAsync(scheduleDay);
            await _context.WeekDays.AddAsync(weekDay);
            await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();

            // Act
            var result = await _courseService.GetAllCourses();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(course.ClassName, result[0].ClassName);
            Assert.Equal(course.Capacity, result[0].Capacity);
            Assert.Equal(course.Level, result[0].Level);
        }
        [Fact]
        public async Task GetCoursesNotStarted_ShouldReturnCoursesNotStarted()
        {
            // Arrange
            var subject = new Subject { SubjectId = 1, DepartmentId = 1 };
            var department = new Department { DepartmentId = 1 };
            var teacher = new Teacher { TeacherId = 1, DepartmentId = 1 };
            var scheduleStarted = new Schedule { ScheduleId = 1, StartDate = DateTime.Now.AddDays(-1), EndDate = DateTime.Now.AddDays(1), StartTime = TimeSpan.FromHours(9), EndTime = TimeSpan.FromHours(10) };
            var scheduleNotStarted = new Schedule { ScheduleId = 2, StartDate = DateTime.Now.AddDays(1), EndDate = DateTime.Now.AddDays(2), StartTime = TimeSpan.FromHours(9), EndTime = TimeSpan.FromHours(10) };
            var scheduleDay = new ScheduleDay { ScheduleId = 1, WeekDayId = 1 };
            var weekDay = new WeekDay { WeekDayId = 1, Name = "Monday" };

            var courseStarted = new Course
            {
                CourseId = 1,
                TeacherId = 1,
                ClassName = "Math 101",
                Capacity = 30,
                Level = 1,
                SubjectId = 1,
                ScheduleId = 1
            };

            var courseNotStarted = new Course
            {
                CourseId = 2,
                TeacherId = 1,
                ClassName = "Science 101",
                Capacity = 25,
                Level = 1,
                SubjectId = 1,
                ScheduleId = 2
            };

            await _context.Teachers.AddAsync(teacher);
            await _context.Subjects.AddAsync(subject);
            await _context.Departments.AddAsync(department);
            await _context.Schedules.AddAsync(scheduleStarted);
            await _context.Schedules.AddAsync(scheduleNotStarted);
            await _context.ScheduleDays.AddAsync(scheduleDay);
            await _context.WeekDays.AddAsync(weekDay);
            await _context.Courses.AddAsync(courseStarted);
            await _context.Courses.AddAsync(courseNotStarted);
            await _context.SaveChangesAsync();

            // Act
            var result = await _courseService.GetCoursesNotStarted();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(courseNotStarted.ClassName, result[0].ClassName);
            Assert.Equal(courseNotStarted.Capacity, result[0].Capacity);
            Assert.Equal(courseNotStarted.Level, result[0].Level);
        }
        [Fact]
        public async Task GetPreviousCoursesOfStudent_ShouldReturnPreviousCourses()
        {
            // Arrange
            var student = new Student { StudentId = 1, FullName = "John Doe" };
            var subject = new Subject { SubjectId = 1, DepartmentId = 1 };
            var department = new Department { DepartmentId = 1 };
            var teacher = new Teacher { TeacherId = 1, DepartmentId = 1 };
            var scheduleEnded = new Schedule { ScheduleId = 1, StartDate = DateTime.Now.AddMonths(-2), EndDate = DateTime.Now.AddMonths(-1), StartTime = TimeSpan.FromHours(9), EndTime = TimeSpan.FromHours(10) };
            var scheduleOngoing = new Schedule { ScheduleId = 2, StartDate = DateTime.Now.AddDays(-1), EndDate = DateTime.Now.AddDays(1), StartTime = TimeSpan.FromHours(9), EndTime = TimeSpan.FromHours(10) };
            var scheduleDay = new ScheduleDay { ScheduleId = 1, WeekDayId = 1 };
            var weekDay = new WeekDay { WeekDayId = 1, Name = "Monday" };

            var courseEnded = new Course
            {
                CourseId = 1,
                TeacherId = 1,
                ClassName = "Math 101",
                Capacity = 30,
                Level = 1,
                SubjectId = 1,
                ScheduleId = 1
            };

            var courseOngoing = new Course
            {
                CourseId = 2,
                TeacherId = 1,
                ClassName = "Science 101",
                Capacity = 25,
                Level = 1,
                SubjectId = 1,
                ScheduleId = 2
            };

            var studentCourseEnded = new StudentCourse { StudentId = 1, CourseId = 1, AverageGrades = 65, Status = "Pass"};
            var studentCourseOngoing = new StudentCourse { StudentId = 1, CourseId = 2, AverageGrades = 95, Status = "Pass" };

            await _context.Students.AddAsync(student);
            await _context.Teachers.AddAsync(teacher);
            await _context.Subjects.AddAsync(subject);
            await _context.Departments.AddAsync(department);
            await _context.Schedules.AddAsync(scheduleEnded);
            await _context.Schedules.AddAsync(scheduleOngoing);
            await _context.ScheduleDays.AddAsync(scheduleDay);
            await _context.WeekDays.AddAsync(weekDay);
            await _context.Courses.AddAsync(courseEnded);
            await _context.Courses.AddAsync(courseOngoing);
            await _context.StudentCourses.AddAsync(studentCourseEnded);
            await _context.StudentCourses.AddAsync(studentCourseOngoing);
            await _context.SaveChangesAsync();

            // Act
            var result = await _courseService.GetPreviousCoursesOfStudent(1);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(courseEnded.ClassName, result[0].CourseName);
            Assert.Equal(studentCourseEnded.AverageGrades, result[0].Grade);
            Assert.Equal(studentCourseEnded.Status, result[0].Status);
            Assert.Equal(courseEnded.Level, result[0].Level);
        }
        [Fact]
        public async Task GetCoursesOfStudent_ShouldReturnCoursesOfStudent()
        {
            // Arrange
            var student = new Student { StudentId = 1, FullName = "John Doe" };
            var subject = new Subject { SubjectId = 1, DepartmentId = 1 };
            var department = new Department { DepartmentId = 1 };
            var teacher = new Teacher { TeacherId = 1, DepartmentId = 1 };
            var schedule = new Schedule { ScheduleId = 1, StartDate = DateTime.Now.AddDays(-1), EndDate = DateTime.Now.AddDays(1), StartTime = TimeSpan.FromHours(9), EndTime = TimeSpan.FromHours(10) };
            var scheduleDay = new ScheduleDay { ScheduleId = 1, WeekDayId = 1 };
            var weekDay = new WeekDay { WeekDayId = 1, Name = "Monday" };

            var course1 = new Course
            {
                CourseId = 1,
                TeacherId = 1,
                ClassName = "Math 101",
                Capacity = 30,
                Level = 1,
                SubjectId = 1,
                ScheduleId = 1
            };

            var course2 = new Course
            {
                CourseId = 2,
                TeacherId = 1,
                ClassName = "Science 101",
                Capacity = 25,
                Level = 1,
                SubjectId = 1,
                ScheduleId = 1
            };

            var studentCourse1 = new StudentCourse { StudentId = 1, CourseId = 1 };
            var studentCourse2 = new StudentCourse { StudentId = 1, CourseId = 2 };

            await _context.Students.AddAsync(student);
            await _context.Teachers.AddAsync(teacher);
            await _context.Subjects.AddAsync(subject);
            await _context.Departments.AddAsync(department);
            await _context.Schedules.AddAsync(schedule);
            await _context.ScheduleDays.AddAsync(scheduleDay);
            await _context.WeekDays.AddAsync(weekDay);
            await _context.Courses.AddAsync(course1);
            await _context.Courses.AddAsync(course2);
            await _context.StudentCourses.AddAsync(studentCourse1);
            await _context.StudentCourses.AddAsync(studentCourse2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _courseService.GetCoursesOfStudent(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, r => r.ClassName == course1.ClassName);
            Assert.Contains(result, r => r.ClassName == course2.ClassName);
        }
        [Fact]
        public async Task GetCurrentCoursesOfStudent_ShouldReturnCurrentCourses()
        {
            // Arrange
            var student = new Student { StudentId = 1, FullName = "John Doe" };
            var subject = new Subject { SubjectId = 1, DepartmentId = 1 };
            var department = new Department { DepartmentId = 1 };
            var teacher = new Teacher { TeacherId = 1, DepartmentId = 1 };
            var scheduleOngoing = new Schedule { ScheduleId = 1, StartDate = DateTime.Now.AddDays(-1), EndDate = DateTime.Now.AddDays(1), StartTime = TimeSpan.FromHours(9), EndTime = TimeSpan.FromHours(10) };
            var scheduleEnded = new Schedule { ScheduleId = 2, StartDate = DateTime.Now.AddMonths(-2), EndDate = DateTime.Now.AddMonths(-1), StartTime = TimeSpan.FromHours(9), EndTime = TimeSpan.FromHours(10) };
            var scheduleDay = new ScheduleDay { ScheduleId = 1, WeekDayId = 1 };
            var weekDay = new WeekDay { WeekDayId = 1, Name = "Monday" };

            var courseOngoing = new Course
            {
                CourseId = 1,
                TeacherId = 1,
                ClassName = "Math 101",
                Capacity = 30,
                Level = 1,
                SubjectId = 1,
                ScheduleId = 1
            };

            var courseEnded = new Course
            {
                CourseId = 2,
                TeacherId = 1,
                ClassName = "Science 101",
                Capacity = 25,
                Level = 1,
                SubjectId = 1,
                ScheduleId = 2
            };

            var studentCourseOngoing = new StudentCourse { StudentId = 1, CourseId = 1 };
            var studentCourseEnded = new StudentCourse { StudentId = 1, CourseId = 2 };

            await _context.Students.AddAsync(student);
            await _context.Teachers.AddAsync(teacher);
            await _context.Subjects.AddAsync(subject);
            await _context.Departments.AddAsync(department);
            await _context.Schedules.AddAsync(scheduleOngoing);
            await _context.Schedules.AddAsync(scheduleEnded);
            await _context.ScheduleDays.AddAsync(scheduleDay);
            await _context.WeekDays.AddAsync(weekDay);
            await _context.Courses.AddAsync(courseOngoing);
            await _context.Courses.AddAsync(courseEnded);
            await _context.StudentCourses.AddAsync(studentCourseOngoing);
            await _context.StudentCourses.AddAsync(studentCourseEnded);
            await _context.SaveChangesAsync();

            // Act
            var result = await _courseService.GetCurrentCoursesOfStudent(1);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(courseOngoing.ClassName, result[0].ClassName);
            Assert.Equal(courseOngoing.Capacity, result[0].Capacity);
            Assert.Equal(courseOngoing.Level, result[0].Level);
        }
    }
}