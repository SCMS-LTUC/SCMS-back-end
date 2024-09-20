using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using SCMS_back_end.Data;
using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto.Request;
using SCMS_back_end.Models.Dto.Response;
using SCMS_back_end.Repositories.Interfaces;
using System.Security.Claims;
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
            }
        }
        public async Task<Course> CreateCourseWithoutTeacher(DtoCreateCourseWTRequest courseRequest)
        {
            // Check if the classroom exists
            var classroom = await _context.Classrooms.FindAsync(courseRequest.ClassroomId);
            if (classroom == null)
            {
                throw new Exception("Classroom not found");
            }

            // Check if the course capacity is less than or equal to the classroom capacity
            if (courseRequest.Capacity > classroom.Capacity)
            {
                throw new Exception("Course capacity exceeds classroom capacity");
            }

            // Check if the classroom is available
            var overlappingCourses = await _context.Courses
                .Include(c => c.Schedule)
                .ThenInclude(s => s.ScheduleDays)
                .Where(c => c.ClassroomId == courseRequest.ClassroomId &&
                            c.Schedule.StartDate < courseRequest.EndDate &&
                            c.Schedule.EndDate > courseRequest.StartDate &&
                            c.Schedule.ScheduleDays.Any(sd => courseRequest.WeekDays.Contains(sd.WeekDayId)) &&
                            c.Schedule.StartTime < courseRequest.EndTime &&
                            c.Schedule.EndTime > courseRequest.StartTime)
                .ToListAsync();

            if (overlappingCourses.Any())
            {
                throw new Exception("Classroom is not available at the specified time");
            }

            // Create the schedule
            var schedule = new Schedule
            {
                StartDate = courseRequest.StartDate,
                EndDate = courseRequest.EndDate,
                StartTime = courseRequest.StartTime,
                EndTime = courseRequest.EndTime,
                ScheduleDays = courseRequest.WeekDays.Select(id => new ScheduleDay { WeekDayId = id }).ToList()
            };

            await _context.Schedules.AddAsync(schedule);
            await _context.SaveChangesAsync();

            // Create the course
            var course = new Course
            {
                SubjectId = courseRequest.SubjectId,
                ClassName = courseRequest.ClassName,
                Capacity = courseRequest.Capacity,
                //Level = courseRequest.Level,
                ScheduleId = schedule.ScheduleId,
                ClassroomId = courseRequest.ClassroomId
            };

            await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();

            // Add lectures
            var lectureService = new LectureService(_context);
            await lectureService.AddLecturesAsync(course.CourseId);

            return course;
        }
        public async Task DeleteCourse(int courseId)
        {
            var course = await _context.Courses
                .Include(c => c.StudentCourses)
                .FirstOrDefaultAsync(c => c.CourseId == courseId);
            var scheduleDays = await _context.ScheduleDays
                .Include(sd => sd.Schedule)
                .ThenInclude(s => s.Course)
                .Where(sd => sd.Schedule.Course.CourseId == courseId)
                .ToListAsync();
            var schedule = await _context.Schedules
                .Include(s => s.Course)
                .FirstOrDefaultAsync(s => s.Course.CourseId == courseId);
               


            if (course == null) return;

            if(course.StudentCourses.Any())
            {
                throw new InvalidOperationException($"Cannot delete course {courseId}: students are enrolled.");
            }
            _context.ScheduleDays.RemoveRange(scheduleDays);
            _context.Schedules.Remove(schedule);
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
        }
        public async Task<List<DtoCourseResponse>> GetAllCourses()
        {
            var courses = await _context.Courses.ToListAsync();
            var courseResponse = new List<DtoCourseResponse>();
            foreach (var course in courses)
            {
                var courseRes = await GetCourseById(course.CourseId);
                courseResponse.Add(courseRes);
            }
            return courseResponse;
        }
        public async Task<DtoCourseResponse> GetCourseById(int courseId)
        {
            var course = await _context.Courses
        .Include(c => c.Teacher)
        .Include(c => c.Subject)
        .Include(c => c.Schedule)
        .ThenInclude(s => s.ScheduleDays)
        .ThenInclude(sd => sd.WeekDay)
        .Include(c => c.Classroom)
        .FirstOrDefaultAsync(c => c.CourseId == courseId);

            if (course == null)
            {
                throw new Exception("Course not found");
            }

            var courseDays = course.Schedule.ScheduleDays
                .Select(sd => sd.WeekDay.Name)
                .ToList();

            var courseResponse = new DtoCourseResponse
            {
                TeacherName = course.Teacher?.FullName,
                SubjectName = course.Subject?.Name,
                StartDate = course.Schedule.StartDate,
                EndDate = course.Schedule.EndDate,
                StartTime = course.Schedule.StartTime,
                EndTime = course.Schedule.EndTime,
                Days = courseDays,
                ClassName = course.ClassName,
                Capacity = course.Capacity,
                ClassroomNumber = course.Classroom?.RoomNumber // Include classroom name
            };

            return courseResponse;
        }
        public async Task<List<DtoCourseResponse>> GetCoursesNotStarted()
        {
            var courses = await _context.Courses.Include(c => c.Teacher)
                                                .Include(c => c.Subject)
                                                .Include(c => c.Schedule)
                                                .ThenInclude(s => s.ScheduleDays)
                                                .ThenInclude(sd => sd.WeekDay)
                                                .Where(c => c.Schedule.StartDate > DateTime.Now)
                                                .ToListAsync();

            var courseResponse = new List<DtoCourseResponse>();
            foreach (var course in courses)
            {
                var courseDays = course.Schedule.ScheduleDays
                    .Select(sd => sd.WeekDay.Name)
                    .ToList();

                var courseRes = new DtoCourseResponse
                {
                    TeacherName = course.Teacher?.FullName ?? "N/A",
                    SubjectName = course.Subject?.Name ?? "N/A",
                    StartDate = course.Schedule.StartDate,
                    EndDate = course.Schedule.EndDate,
                    StartTime = course.Schedule.StartTime,
                    EndTime = course.Schedule.EndTime,
                    Days = courseDays,
                    ClassName = course.ClassName,
                    Capacity = course.Capacity,
                };

                courseResponse.Add(courseRes);
            }
            return courseResponse;
        }
        public async Task<List<DtoCourseResponse>> GetCoursesOfStudent(ClaimsPrincipal userPrincipal)
        {
            var userIdClaim = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userIdClaim);
            if (student == null)
            {
                throw new Exception("Student not found");
            }
            var courses = await _context.Courses.Include(c => c.Teacher)
                                                .Include(c => c.Subject)
                                                .Include(c => c.Schedule)
                                                .ThenInclude(s => s.ScheduleDays)
                                                .ThenInclude(sd => sd.WeekDay)
                                                .Where(c => c.StudentCourses.Any(s => s.StudentId == student.StudentId))
                                                .ToListAsync();

            var courseResponses = new List<DtoCourseResponse>();
            foreach (var course in courses)
            {
                var courseDays = course.Schedule.ScheduleDays
                    .Select(sd => sd.WeekDay.Name)
                    .ToList();

                var courseRes = new DtoCourseResponse
                {
                    TeacherName = course.Teacher?.FullName ?? "N/A",
                    SubjectName = course.Subject?.Name ?? "N/A",
                    StartDate = course.Schedule.StartDate,
                    EndDate = course.Schedule.EndDate,
                    StartTime = course.Schedule.StartTime,
                    EndTime = course.Schedule.EndTime,
                    Days = courseDays,
                    ClassName = course.ClassName,
                    Capacity = course.Capacity,
                };

                courseResponses.Add(courseRes);
            }
            return courseResponses;
        }
        public async Task<List<DtoCourseResponse>> GetCoursesOfTeacher(ClaimsPrincipal userPrincipal)
        {
            var userIdClaim = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == userIdClaim);
            if (teacher == null)
            {
                throw new Exception("Teacher not found");
            }
            var courses = await _context.Courses.Where(c => c.TeacherId == teacher.TeacherId).ToListAsync();
            var courseResponse = new List<DtoCourseResponse>();
            foreach (var course in courses)
            {
                var courseRes = await GetCourseById(course.CourseId);
                courseResponse.Add(courseRes);
            }
            return courseResponse;
        }

        public async Task<List<DtoCourseResponse>> GetCurrentCoursesOfStudent(ClaimsPrincipal userPrincipal)
        {
            var userIdClaim = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userIdClaim);
            if (student == null)
            {
                throw new Exception("Student not found");
            }
            var currentCourses = await _context.Courses.Include(c => c.Teacher)
                                                       .Include(c => c.Subject)
                                                       .Include(c => c.Schedule)
                                                       .ThenInclude(s => s.ScheduleDays)
                                                       .ThenInclude(sd => sd.WeekDay)
                                                       .Where(c => c.StudentCourses.Any(s => s.StudentId == student.StudentId) && c.Schedule.StartDate <= DateTime.Now && c.Schedule.EndDate >= DateTime.Now)
                                                       .ToListAsync();

            var currentCourseResponses = new List<DtoCourseResponse>();
            foreach (var course in currentCourses)
            {
                var courseDays = course.Schedule.ScheduleDays
                    .Select(sd => sd.WeekDay.Name)
                    .ToList();

                var courseRes = new DtoCourseResponse
                {
                    TeacherName = course.Teacher?.FullName ?? "N/A",
                    SubjectName = course.Subject?.Name ?? "N/A",
                    StartDate = course.Schedule.StartDate,
                    EndDate = course.Schedule.EndDate,
                    StartTime = course.Schedule.StartTime,
                    EndTime = course.Schedule.EndTime,
                    Days = courseDays,
                    ClassName = course.ClassName,
                    Capacity = course.Capacity,
                };

                currentCourseResponses.Add(courseRes);
            }
            return currentCourseResponses;
        }
        public async Task<List<DtoCourseResponse>> GetCurrentCoursesOfTeacher(ClaimsPrincipal userPrincipal)
        {
            var userIdClaim = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == userIdClaim);
            if (teacher == null)
            {
                throw new Exception("Teacher not found");
            }
            var currentCourses = await _context.Courses.Include(c => c.Teacher)
                                                       .Include(c => c.Subject)
                                                       .Include(c => c.Schedule)
                                                       .ThenInclude(s => s.ScheduleDays)
                                                       .ThenInclude(sd => sd.WeekDay)
                                                       .Where(c => c.TeacherId == teacher.TeacherId && c.Schedule.StartDate <= DateTime.Now && c.Schedule.EndDate >= DateTime.Now)
                                                       .ToListAsync();


            var currentCourseResponses = new List<DtoCourseResponse>();
            foreach (var course in currentCourses)
            {
                var courseDays = course.Schedule.ScheduleDays
                    .Select(sd => sd.WeekDay.Name)
                    .ToList();

                var courseRes = new DtoCourseResponse
                {
                    TeacherName = course.Teacher?.FullName ?? "N/A",
                    SubjectName = course.Subject?.Name ?? "N/A",
                    StartDate = course.Schedule.StartDate,
                    EndDate = course.Schedule.EndDate,
                    StartTime = course.Schedule.StartTime,
                    EndTime = course.Schedule.EndTime,
                    Days = courseDays,
                    ClassName = course.ClassName,
                    Capacity = course.Capacity,
                };

                currentCourseResponses.Add(courseRes);
            }
            return currentCourseResponses;
        }
        public async Task<List<DtoPreviousCourseResponse>> GetPreviousCoursesOfStudent(ClaimsPrincipal userPrincipal)
        {
            var userIdClaim = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userIdClaim);
            if (student == null)
            {
                throw new Exception("Student not found");
            }
            var previousCourses = await _context.Courses.Include(c => c.Teacher)
                                                        .Include(c => c.Subject)
                                                        .Include(c => c.Schedule)
                                                        .ThenInclude(s => s.ScheduleDays)
                                                        .ThenInclude(sd => sd.WeekDay)
                                                        .Where(c => c.StudentCourses.Any(s => s.StudentId == studentId) && c.Schedule.EndDate < DateTime.Now)
                                                        .ToListAsync();

            var previousCourseResponses = new List<DtoPreviousCourseResponse>();
            foreach (var course in previousCourses)
            {
                var courseDays = course.Schedule.ScheduleDays
                    .Select(sd => sd.WeekDay.Name)
                    .ToList();

                var courseRes = new DtoPreviousCourseResponse
                {
                    TeacherName = course.Teacher?.FullName ?? "N/A",
                    SubjectName = course.Subject?.Name ?? "N/A",
                    CourseName = course.ClassName,
                    Grade = course.StudentCourses.FirstOrDefault(sc => sc.StudentId == studentId)?.AverageGrades ?? 0,
                    Status = course.StudentCourses.FirstOrDefault(sc => sc.StudentId == studentId)?.Status ?? "N/A"
                };

                previousCourseResponses.Add(courseRes);
            }
            return previousCourseResponses;
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
            int sum = 0;
            foreach (var grade in courseGrades)
            {
                sum += grade.Grade ?? 0;
            }
            double average = sum / courseGrades.Count;
            studentCourse.AverageGrades = (int)average;
            if (studentCourse.Status != "Drop")
                studentCourse.Status = average >= 50 ? "Pass" : "Fail";
            await _context.SaveChangesAsync();
        }
        public async Task<Course> UpdateCourseInformation(int courseId, DtoUpdateCourseRequest courseRequest)
        {
            var course = await _context.Courses
                .Include(c => c.Subject)
                .Include(c => c.StudentCourses)
                .Include(c => c.Schedule)
                .ThenInclude(s => s.ScheduleDays)
                .FirstOrDefaultAsync(c => c.CourseId == courseId);
            

            if (course == null)
            {
                throw new Exception("Course not found");
            }

            //if (courseRequest.SubjectId != 0)
            //{
            //    var subjectExists = await _context.Subjects.AnyAsync(s => s.SubjectId == courseRequest.SubjectId);
            //    if (!subjectExists)
            //    {
            //        throw new Exception("Subject not found");
            //    }
            //    course.SubjectId = courseRequest.SubjectId;
            //}

            //if (courseRequest.Level != 0)
            //{
            //    course.Level = courseRequest.Level;
            //}
            if (courseRequest.Capacity != 0)
            {
                if (courseRequest.Capacity < course.StudentCourses.Count)
                    throw new Exception("Capacity cannot be decreased below the number of registered students.");
                course.Capacity = courseRequest.Capacity;
            }

            if (courseRequest.TeacherId.HasValue /*&& courseRequest.TeacherId!=0*/)
            {
                var teacher = await _context.Teachers
                    .Include(t => t.Courses)
                    .ThenInclude(c => c.Schedule)
                    .ThenInclude(s => s.ScheduleDays)
                    .FirstOrDefaultAsync(t => t.TeacherId == courseRequest.TeacherId.Value);

                var TeacherCourses = await _context.Courses
                .Include(c => c.Subject)
                .Include(c => c.Schedule)
                .ThenInclude(s => s.ScheduleDays)
                .Where(c => c.TeacherId == courseRequest.TeacherId.Value && c.Schedule.StartDate < course.Schedule.EndDate &&
                c.Schedule.EndDate > course.Schedule.StartDate).ToListAsync();


                if (teacher == null)
                {
                    throw new Exception("Teacher not found");
                }

                foreach (var teacherCourse in TeacherCourses)
                {
                    if (teacherCourse.CourseId != courseId && teacherCourse.Schedule != null)
                    {
                        // check if there is an overlap between dates 
                        //if (teacherCourse.Schedule.StartDate < course.Schedule.EndDate &&
                        //    teacherCourse.Schedule.EndDate > course.Schedule.StartDate)
                        //{
                            //var overlap = teacherCourse.Schedule.ScheduleDays.Any(sd => course.Schedule.ScheduleDays.Select(cs => cs.WeekDayId).Contains(sd.WeekDayId)) &&
                            //              teacherCourse.Schedule.StartTime == course.Schedule.StartTime &&
                            //              teacherCourse.Schedule.EndTime == course.Schedule.EndTime;

                            //if (overlap)
                            //{
                            //    throw new Exception("Teacher has a conflicting course schedule.");
                            //}

                            // check if there is an overlap between days 
                            var course1Days = teacherCourse.Schedule.ScheduleDays.Select(sd => sd.WeekDayId).ToList();
                            var course2Days = course.Schedule.ScheduleDays.Select(sd => sd.WeekDayId).ToList();
                            var commonDays = course1Days.Intersect(course2Days).ToList();
                        
                            if (commonDays.Any())
                            {
                                // check if there is an overlap between times 
                                var overlap = teacherCourse.Schedule.StartTime < course.Schedule.EndTime &&
                                     teacherCourse.Schedule.EndTime > course.Schedule.StartTime;

                                if (overlap)
                                {
                                    throw new Exception("Teacher has a conflicting course schedule.");
                                }
                            }

                        //}
                        
                    }
                }

                if (TeacherCourses.Count >= teacher.CourseLoad)
                {
                    throw new Exception("Teacher has reached the maximum course load.");
                }

                var courseDepartment = await _context.Departments
                    .FirstOrDefaultAsync(d => d.DepartmentId == course.Subject.DepartmentId);
                var teacherDepartment = await _context.Departments
                    .FirstOrDefaultAsync(d => d.DepartmentId == teacher.DepartmentId);

                if (courseDepartment == null || teacherDepartment == null || courseDepartment.DepartmentId != teacherDepartment.DepartmentId)
                {
                    throw new Exception("Teacher's department does not match the course department.");
                }

                course.TeacherId = courseRequest.TeacherId;
            }
            /*
            //if (courseRequest.StartDate.HasValue)
            //{
            //    course.Schedule.StartDate = courseRequest.StartDate.Value;
            //}
            //if (courseRequest.EndDate.HasValue)
            //{
            //    course.Schedule.EndDate = courseRequest.EndDate.Value;
            //}
            //if (courseRequest.StartTime.HasValue)
            //{
            //    course.Schedule.StartTime = courseRequest.StartTime.Value;
            //}
            //if (courseRequest.EndTime.HasValue)
            //{
            //    course.Schedule.EndTime = courseRequest.EndTime.Value;
            //}

            //if (courseRequest.WeekDays != null && courseRequest.WeekDays.Any())
            //{
            //    var existingScheduleDays = await _context.ScheduleDays.Where(sd => sd.ScheduleId == course.Schedule.ScheduleId).ToListAsync();
            //    _context.ScheduleDays.RemoveRange(existingScheduleDays);

            //    var newScheduleDays = courseRequest.WeekDays.Select(weekDayId => new ScheduleDay
            //    {
            //        WeekDayId = weekDayId,
            //        ScheduleId = course.Schedule.ScheduleId
            //    }).ToList();

            //    await _context.ScheduleDays.AddRangeAsync(newScheduleDays);
            //}
            */
            await _context.SaveChangesAsync();

            return course;
        }
    }
}
