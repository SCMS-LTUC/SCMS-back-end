using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using SCMS_back_end.Data;
using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto.Request;
using SCMS_back_end.Models.Dto.Response;
using SCMS_back_end.Repositories.Interfaces;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace SCMS_back_end.Repositories.Services
{
    public class CourseService : ICourse
    {
        private readonly StudyCenterDbContext _context;
        public CourseService(StudyCenterDbContext context)
        {
            _context = context;
        }

        public async Task CalculateAverageGrade(int courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
            {
                throw new Exception("Course not found");
            }
            var studentCourses = await _context.StudentCourses.Where(sc => sc.CourseId == courseId).ToListAsync();
            foreach (var studentCourse in studentCourses)
            {
                await CalculateStudentGrade(courseId, studentCourse.StudentId);
                //var student = await _context.Students.FindAsync(studentCourse.StudentId);
                //var studentGrades = await _context.StudentAssignments.Where(sa => sa.StudentId == student.StudentId).ToListAsync();
                //var courseAssignments = await _context.Assignments.Where(a => a.CourseId == courseId).ToListAsync();
                //var courseGrades = studentGrades.Where(sg => courseAssignments.Select(ca => ca.AssignmentId).Contains(sg.AssignmentId)).ToList();
                //double sum = 0;
                //foreach (var grade in courseGrades)
                //{
                //    sum += grade.Grade;
                //}
                //double average = sum / courseGrades.Count;
                //DtoCourseStudentGrade courseStudentGrade = new DtoCourseStudentGrade
                //{
                //    CourseName = course.ClassName,
                //    StudentName = student.FullName,
                //    Grade = average
                //};
                //courseStudentGrades.Add(courseStudentGrade);
                //var courseGrades = studentGrades.Where(sg => sg.AssignmentId == courseId).ToList();
                //double sum = 0;
                //foreach (var grade in courseGrades)
                //{
                //    sum += grade.Grade;
                //}
                //double average = sum / courseGrades.Count;
                //DtoCourseStudentGrade courseStudentGrade = new DtoCourseStudentGrade
                //{
                //    CourseName = course.ClassName,
                //    StudentName = student.FullName,
                //    Grade = average.Grade
                //};
            }
        }

        public async Task<Course> CreateCourseWithoutTeacher(DtoCreateCourseWTRequest course)
        {
            var subject = await _context.Subjects.FindAsync(course.SubjectId);
            if (subject == null)
            {
                throw new Exception("Subject not found");
            }
            var department = await _context.Departments.FindAsync(subject.DepartmentId);
            if (department == null)
            {
                throw new Exception("Department not found");
            }
            var schedule = await _context.Schedules.FindAsync(course.ScheduleId);
            if (schedule == null)
            {
                throw new Exception("Schedule not found");
            }
            var scheduleDay = await _context.ScheduleDays.FindAsync(schedule.ScheduleId);
            if (scheduleDay == null)
            {
                throw new Exception("ScheduleDay not found");
            }
            var newCourse = new Course
            {
                SubjectId = course.SubjectId,
                ScheduleId = course.ScheduleId,
                ClassName = course.ClassName,
                Capacity = course.Capacity,
                Level = course.Level
            };
            _context.Courses.Add(newCourse);
            await _context.SaveChangesAsync();
            return newCourse;
        }

        public Task<bool> DeleteCourse(int courseId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<DtoCourseResponse>> GetAllCourses()
        {
            var courses = await _context.Courses.ToListAsync();
            var courseResponse = new List<DtoCourseResponse>();
            foreach (var course in courses)
            {
                //var teacher = await _context.Teachers.FindAsync(course.TeacherId);
                //var courseScheduleDays = await _context.ScheduleDays.Select(sd => sd.WeekDayId == course.ScheduleId).ToListAsync();
                //var courseDays = new List<string>();
                //foreach (var day in courseScheduleDays)
                //{
                //    var weekDay = await _context.WeekDays.FindAsync(day);
                //    courseDays.Add(weekDay.Name);
                //}
                //DtoCourseResponse courseRes = new DtoCourseResponse
                //{
                //    TeacherName = teacher.FullName,
                //    SubjectName = course.Subject.Name,
                //    StartDate = course.Schedule.StartDate,
                //    EndDate = course.Schedule.EndDate,
                //    StartTime = course.Schedule.StartTime,
                //    EndTime = course.Schedule.EndTime,
                //    Days = courseDays,
                //    ClassName = course.ClassName,
                //    Capacity = course.Capacity,
                //    Level = course.Level
                //};
                var courseRes = await GetCourseById(course.CourseId);
                courseResponse.Add(courseRes);
            }
            return courseResponse;
        }

        public async Task<DtoCourseResponse> GetCourseById(int courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
            {
                throw new Exception("Course not found");
            }
            var teacher = await _context.Teachers.FindAsync(course.TeacherId);
            if (teacher == null)
            {
                throw new Exception("Teacher not found");
            }
            var courseScheduleDays = await _context.ScheduleDays.Where(sd => sd.WeekDayId == course.ScheduleId).ToListAsync();
            var courseDays = new List<string>();
            foreach (var day in courseScheduleDays)
            {
                var weekDay = await _context.WeekDays.FindAsync(day.WeekDayId);
                if (weekDay == null)
                {
                    throw new Exception("WeekDay not found");
                }
                courseDays.Add(weekDay.Name);
            }
            DtoCourseResponse courseResponse = new DtoCourseResponse
            {
                TeacherName = teacher.FullName,
                SubjectName = course.Subject.Name,
                StartDate = course.Schedule.StartDate,
                EndDate = course.Schedule.EndDate,
                StartTime = course.Schedule.StartTime,
                EndTime = course.Schedule.EndTime,
                Days = courseDays,
                ClassName = course.ClassName,
                Capacity = course.Capacity,
                Level = course.Level
            };
            return courseResponse;
        }

        public async Task<List<DtoCourseResponse>> GetCoursesNotStarted()
        {
            var courses = await _context.Courses.ToListAsync();
            var courseResponse = new List<DtoCourseResponse>();
            foreach (var course in courses)
            {
                if (course.Schedule.StartDate > DateTime.Now)
                {

                    //var teacher = await _context.Teachers.FindAsync(course.TeacherId);
                    //var courseScheduleDays = await _context.ScheduleDays.Select(sd => sd.WeekDayId == course.ScheduleId).ToListAsync();
                    //var courseDays = new List<string>();
                    //foreach (var day in courseScheduleDays)
                    //{
                    //    var weekDay = await _context.WeekDays.FindAsync(day);
                    //    courseDays.Add(weekDay.Name);
                    //}
                    //DtoCourseResponse courseRes = new DtoCourseResponse
                    //{
                    //    TeacherName = teacher.FullName,
                    //    SubjectName = course.Subject.Name,
                    //    StartDate = course.Schedule.StartDate,
                    //    EndDate = course.Schedule.EndDate,
                    //    StartTime = course.Schedule.StartTime,
                    //    EndTime = course.Schedule.EndTime,
                    //    Days = courseDays,
                    //    ClassName = course.ClassName,
                    //    Capacity = course.Capacity,
                    //    Level = course.Level
                    //};
                    var courseRes = await GetCourseById(course.CourseId);
                    courseResponse.Add(courseRes);
                }
            }
            return courseResponse;
        }

        public async Task<List<DtoCourseResponse>> GetCoursesOfStudent(int studentId)
        {
            var student = await _context.Students.FindAsync(studentId);
            if (student == null)
            {
                throw new Exception("Student not found");
            }
            var courses = await _context.StudentCourses.Where(sc => sc.StudentId == studentId).ToListAsync();
            var courseResponse = new List<DtoCourseResponse>();
            foreach (var course in courses)
            {
                var courseRes = await GetCourseById(course.CourseId);
                courseResponse.Add(courseRes);
            }
            return courseResponse;
        }

        public async Task<List<DtoCourseResponse>> GetCoursesOfTeacher(int teacherId)
        {
            var teacher = await _context.Teachers.FindAsync(teacherId);
            if (teacher == null)
            {
                throw new Exception("Teacher not found");
            }
            var courses = await _context.Courses.Where(c => c.TeacherId == teacherId).ToListAsync();
            var courseResponse = new List<DtoCourseResponse>();
            foreach (var course in courses)
            {
                var courseRes = await GetCourseById(course.CourseId);
                courseResponse.Add(courseRes);
            }
            return courseResponse;
        }

        public async Task<List<DtoCourseResponse>> GetCurrentCoursesOfStudent(int studentId)
        {
            var student = await _context.Students.FindAsync(studentId);
            if (student == null)
            {
                throw new Exception("Student not found");
            }
            var courses = await _context.StudentCourses.Where(sc => sc.StudentId == studentId).ToListAsync();
            var courseResponse = new List<DtoCourseResponse>();
            foreach (var course in courses)
            {
                if (course.Course.Schedule.StartDate < DateTime.Now && course.Course.Schedule.EndDate > DateTime.Now)
                {
                    var courseRes = await GetCourseById(course.CourseId);
                    courseResponse.Add(courseRes);
                }
            }
            return courseResponse;
        }

        public async Task<List<DtoCourseResponse>> GetCurrentCoursesOfTeacher(int teacherId)
        {
            var teacher = await _context.Teachers.FindAsync(teacherId);
            if (teacher == null)
            {
                throw new Exception("Teacher not found");
            }
            var courses = await _context.Courses.Where(c => c.TeacherId == teacherId).ToListAsync();
            var courseResponse = new List<DtoCourseResponse>();
            foreach (var course in courses)
            {
                if (course.Schedule.StartDate < DateTime.Now && course.Schedule.EndDate > DateTime.Now)
                {
                    var courseRes = await GetCourseById(course.CourseId);
                    courseResponse.Add(courseRes);
                }
            }
            return courseResponse;
        }

        public async Task<List<DtoPreviousCourseResponse>> GetPreviousCoursesOfStudent(int studentId)
        {
            var student = await _context.Students.FindAsync(studentId);
            if (student == null)
            {
                throw new Exception("Student not found");
            }
            var courses = await _context.StudentCourses.Where(sc => sc.StudentId == studentId).ToListAsync();
            var courseResponse = new List<DtoPreviousCourseResponse>();
            foreach (var course in courses)
            {
                if (course.Course.Schedule.EndDate < DateTime.Now)
                {
                    var courseRes = new DtoPreviousCourseResponse
                    {
                        CourseName = course.Course.ClassName,
                        TeacherName = course.Course.Teacher.FullName,
                        SubjectName = course.Course.Subject.Name,
                        Grade = course.AverageGrades,
                        Level = course.Course.Level,
                        Status = course.Status
                    };
                    courseResponse.Add(courseRes);
                }
            }
            return courseResponse;
        }
        public async Task CalculateStudentGrade(int courseId, int studentId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
            {
                throw new Exception("Course not found");
            }
            var student = await _context.Students.FindAsync(studentId);
            if (student == null)
            {
                throw new Exception("Student not found");
            }
            var studentCourse = await _context.StudentCourses.Where(sc => sc.CourseId == courseId && sc.StudentId == studentId).FirstOrDefaultAsync();
            if (studentCourse == null)
            {
                throw new Exception("Student is not enrolled in this course");
            }
            var studentGrades = await _context.StudentAssignments.Where(sa => sa.StudentId == studentId).ToListAsync();
            var courseAssignments = await _context.Assignments.Where(a => a.CourseId == courseId).ToListAsync();
            var courseGrades = studentGrades.Where(sg => courseAssignments.Select(ca => ca.AssignmentId).Contains(sg.AssignmentId)).ToList();
            double sum = 0;
            foreach (var grade in courseGrades)
            {
                sum += grade.Grade;
            }
            double average = sum / courseGrades.Count;
            studentCourse.AverageGrades = (int)average;
            studentCourse.Status = average >= 50 ? "Pass" : "Fail";
            await _context.SaveChangesAsync();
        }

        public async Task<Course> UpdateCourseInformation(int courseId, DtoUpdateCourseRequest course)
        {
            var courseToUpdate = await _context.Courses.FindAsync(courseId);
            if (courseToUpdate == null)
            {
                throw new Exception("Course not found");
            }
            var teacher = await _context.Teachers.FindAsync(course.TeacherId);
            if (teacher == null)
            {
                throw new Exception("Teacher not found");
            }
            if (teacher.CourseLoad >= 3)
            {
                throw new Exception("Teacher has reached the maximum course load");
            }
            var subject = await _context.Subjects.FindAsync(course.SubjectId);
            if (subject == null)
            {
                throw new Exception("Subject not found");
            }
            var department = await _context.Departments.FindAsync(subject.DepartmentId);
            if (department == null)
            {
                throw new Exception("Department not found");
            }
            if (department.DepartmentId != teacher.DepartmentId)
            {
                throw new Exception("Teacher and Subject are not in the same department");
            }
            var schedule = await _context.Schedules.FindAsync(course.ScheduleId);
            if (schedule == null)
            {
                throw new Exception("Schedule not found");
            }
            var scheduleDay = await _context.ScheduleDays.FindAsync(schedule.ScheduleId);
            if (scheduleDay == null)
            {
                throw new Exception("ScheduleDay not found");
            }
            //var teacherCoursesTime = teacher.Courses.Select(c => c.ScheduleId)
            //                                        .Select(s => _context.Schedules.Find(s).ScheduleId)
            //                                        .Select(sd => _context.ScheduleDays.Find(sd).WeekDayId).ToList();

            //var teacherCoursesStartTime = teacher.Courses.Select(c => c.ScheduleId)
            //                                        .Select(s => _context.Schedules.Find(s).StartTime).ToList();

            //if (teacherCoursesTime.Contains(scheduleDay.WeekDayId))
            //{
            //    if (teacherCoursesStartTime.Contains(schedule.StartTime))
            //    {
            //        throw new Exception("Teacher has a course at the same time");
            //    }
            //}
            foreach (var Tcourse in teacher.Courses)
            {
                var courseSchedule = await _context.Schedules.FindAsync(Tcourse.ScheduleId);
                var courseScheduleDay = await _context.ScheduleDays.FindAsync(courseSchedule.ScheduleId);
                if (courseScheduleDay.WeekDayId == scheduleDay.WeekDayId)
                {
                    if (courseSchedule.StartTime == schedule.StartTime)
                    {
                        throw new Exception("Teacher has a course at the same time");
                    }
                }
            }
            courseToUpdate.TeacherId = course.TeacherId;
            courseToUpdate.SubjectId = course.SubjectId;
            courseToUpdate.ScheduleId = course.ScheduleId;
            courseToUpdate.ClassName = course.ClassName;
            courseToUpdate.Capacity = course.Capacity;
            courseToUpdate.Level = course.Level;
            await _context.SaveChangesAsync();
            return courseToUpdate;
        }
    }
}
