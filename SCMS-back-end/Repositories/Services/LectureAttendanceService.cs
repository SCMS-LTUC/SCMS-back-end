using Azure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json;
using SCMS_back_end.Data;
using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto.Request.LectureAttendance;
using SCMS_back_end.Models.Dto.Response.LectureAttendance;
using SCMS_back_end.Repositories.Interfaces;
using System.Globalization;

namespace SCMS_back_end.Repositories.Services
{
    public class LectureAttendanceService : IlectureAttendance
    {
        private readonly StudyCenterDbContext _context;

        public LectureAttendanceService(StudyCenterDbContext context)
        {
            _context = context;
        }

        /*  public async Task<DtoAddLectureAttendanceResponse> AddLectureAttendance(DtoAddLectureAttendanceRequest Attendance)
          {

              var LectureAttendance = new LectureAttendance()
              {
                  LectureId = Attendance.LectureId,
                  StudentId = Attendance.StudentId,
                  Status = Attendance.Status,
              };

              _context.LectureAttendances.Add(LectureAttendance);
              await _context.SaveChangesAsync();

              var Response = new DtoAddLectureAttendanceResponse()
              {

                  LectureId = Attendance.LectureId,
                  StudentId = Attendance.StudentId,
                  Status = Attendance.Status,
              };

              return Response;


          }*/


        public async Task<DtoAddLectureAttendanceResponse>AddLectureAttendance(DtoAddLectureAttendanceRequest Attendance)
        {

            var lecture = await _context.Lectures
                                .FirstOrDefaultAsync(l => l.LectureId == Attendance.LectureId);

            if (lecture == null)
            {
                throw new Exception("Course does not exist.");
            }

            var LectureAttendance = new LectureAttendance()
            {
                LectureId = Attendance.LectureId,
                StudentId = Attendance.StudentId,
                Status = Attendance.Status,
            };

            _context.LectureAttendances.Add(LectureAttendance);
            await _context.SaveChangesAsync();


            var Response = new DtoAddLectureAttendanceResponse()
            {
                LectureId = Attendance.LectureId,
                StudentId = Attendance.StudentId,
                Status = Attendance.Status,
            };

            return Response;
        }

        /* public async Task<List<DtoGetAbsenceRateAndStudentResponse>> GetStudentAndAbsenceRate(int courseId)
         {

             var totalLectures = await _context.Lectures
                 .Where(x => x.CourseId == courseId)
                 .CountAsync();


             var studentAbsenceDataQuery = _context.LectureAttendances
                 .Where(x => x.Lecture.CourseId == courseId)
                 .Include(x => x.Student)
                 .GroupBy(x => new { x.Student.StudentId, x.Student.FullName })
                 .Select(g => new
                 {
                     StudentID = g.Key.StudentId,
                     StudentName = g.Key.FullName,
                     AbsentCount = g.Count(x => x.Status == "Absence"),
                     AbsenceRate = totalLectures > 0 ? ((double)g.Count(x => x.Status == "Absence") / totalLectures) * 100 : 0
                 });


             var studentAbsenceData = await studentAbsenceDataQuery.ToListAsync();

             var responses = studentAbsenceData.Select(data => new DtoGetAbsenceRateAndStudentResponse
             {
                 StudentID= data.StudentID,
                 FullName = data.StudentName,
               //  AbsenceRateForTheStudent=(float)data.AbsenceRate,
                AbsenceRateForTheStudent = Math.Round(data.AbsenceRate, 2),

             }).ToList();

             return responses;
         }*/


        public async Task<List<DtoGetAbsenceRateAndStudentResponse>> GetStudentAndAbsenceRate(int courseId)
        {
            try
            {
               
                var courseExists = await _context.Courses.AnyAsync(c => c.CourseId == courseId);
                if (!courseExists)
                {
                    throw new Exception("Course does not exist.");
                }

               
                var totalLectures = await _context.Lectures
                    .Where(x => x.CourseId == courseId)
                    .CountAsync();

                
                if (totalLectures == 0)
                {
                    throw new Exception("No lectures found for the given course.");
                }

                
                var studentAbsenceDataQuery = _context.LectureAttendances
                    .Where(x => x.Lecture.CourseId == courseId)
                    .Include(x => x.Student)
                    .GroupBy(x => new { x.Student.StudentId, x.Student.FullName })
                    .Select(g => new
                    {
                        StudentID = g.Key.StudentId,
                        StudentName = g.Key.FullName,
                        AbsentCount = g.Count(x => x.Status == "Absence"),
                        AbsenceRate = totalLectures > 0
                            ? ((double)g.Count(x => x.Status == "Absence") / totalLectures) * 100
                            : 0
                    });

               
                var studentAbsenceData = await studentAbsenceDataQuery.ToListAsync();

               
                if (!studentAbsenceData.Any())
                {
                    throw new Exception("No attendance data found for students in the given course.");
                }

                
                var responses = studentAbsenceData.Select(data => new DtoGetAbsenceRateAndStudentResponse
                {
                    StudentID = data.StudentID,
                    FullName = data.StudentName,
                    AbsenceRateForTheStudent = Math.Round(data.AbsenceRate, 2),
                }).ToList();

                return responses;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred: {ex.Message}");
            }
        }


        /*   public async Task<List<DtoGetAbsenceRateAndStudentResponse>> GetStudentAndAbsenceRate(int courseId, int StudentID)
        { 
      
        var GetAllLecturesInSpecificClass = await _context.Lectures
            .Where(x => x.CourseId == courseId)
            .CountAsync();

        
        var studentAbsenceData = await _context.LectureAttendances
            .Where(x => x.Lecture.CourseId == courseId)
            .Include(x => x.Student)
            .GroupBy(x => new { x.Student.StudentId, x.Student.FullName })
            .Select(g => new
            {
              
                StudentID = g.Key.StudentId,
                StudentName = g.Key.FullName,
                AbsentCount = g.Count(x => x.Status=="Absence"),
                  
                AbsenceRateForTheStudent = GetAllLecturesInSpecificClass > 0 ? ((double)g.Count(x => x.Status == "Absence") / GetAllLecturesInSpecificClass) * 100 : 0
               
            })
            .ToListAsync();

            var responses = studentAbsenceData.Select(data => new DtoGetAbsenceRateAndStudentResponse
            {
                FullName = data.StudentName,
                AbsenceRateForTheStudent = Math.Round(data.AbsenceRateForTheStudent,2)

            }).ToList();

            return responses;
        }*/

        /*public async Task<List<DtoGetAbsenceRateAndStudentResponse>> GetStudentAndAbsenceRate(int courseId,int StudentID)
        {
            // Fetch the total number of classes for the course
            var totalClasses = await _context.Lectures
                .Where(l => l.CourseId == courseId)
                .CountAsync(); 

            // Fetch student names and their absence rates
            var studentAbsenceData = await _context.LectureAttendances
                .Where(x => x.Lecture.CourseId == courseId )
                .Include(x => x.Student)
                .GroupBy(x => new { x.Student.StudentId, x.Student.FullName })
               
                .Select(g => new
                {
                    StudentID = g.Key.StudentId,
                    StudentName = g.Key.FullName,
                    AbsentCount = g.Count(), // Number of absences
                    AbsenceRateForTheStudent = (double)g.Count() / totalClasses * 100 // Calculate absence rate as a percentage
                })
                .ToListAsync();

            // Map to response DTO
            var responses = studentAbsenceData.Select(data => new DtoGetAbsenceRateAndStudentResponse
            {
                FullName = data.StudentName,
                AbsenceRateForTheStudent = data.AbsenceRateForTheStudent
            }).ToList();

            return responses;
        }*/


        /*  public async Task<List<DtoGetAbsenceDateResponse>> GetAbsenceDate(int CourseId, int StudentId)
          {

              var Date = await _context.LectureAttendances
                 .Where(x => x.StudentId == StudentId && x.Lecture.CourseId==CourseId)
                 .Include(x => x.Lecture)
                 .Select(x => x.Lecture.LectureDate)
                  .ToListAsync();

              var responses = Date.Select(date => new DtoGetAbsenceDateResponse
              {
                  LectureDate = date
              }).ToList();

              return responses;

          }*/


        public async Task<List<DtoGetAbsenceDateResponse>> GetAbsenceDate(int CourseId, int StudentId)
        {
            
            var courseExists = await _context.Courses.AnyAsync(c => c.CourseId == CourseId);
            if (!courseExists)
            {
                throw new Exception("Course does not exist.");
            }

            
            var studentExists = await _context.Students.AnyAsync(s => s.StudentId == StudentId);
            if (!studentExists)
            {
                throw new Exception("Student does not exist.");
            }

          
            var dates = await _context.LectureAttendances
                .Where(x => x.StudentId == StudentId && x.Lecture.CourseId == CourseId)
                .Include(x => x.Lecture)
                .Select(x => x.Lecture.LectureDate)
                .ToListAsync();

            
            if (!dates.Any())
            {
                throw new Exception("No absences found for the given student in this course.");
            }

           
            var responses = dates.Select(date => new DtoGetAbsenceDateResponse
            {
                LectureDate = date
            }).ToList();

            return responses;
        }


        //public async Task<DtoAddLectureAttendanceResponse> GetStudentAndAbsenceRate(DtoAddLectureAttendanceRequest Attendance)
        //{
        //    var LectureAttendance = new LectureAttendance()
        //    {
        //        LectureId = Attendance.LectureId,
        //        StudentId = Attendance.StudentId,
        //        Status = Attendance.Status,
        //    };

        //    _context.LectureAttendances.Add(LectureAttendance);
        //    await _context.SaveChangesAsync();

        //    var Response = new DtoAddLectureAttendanceResponse()
        //    {

        //        LectureId = Attendance.LectureId,
        //        StudentId = Attendance.StudentId,
        //        Status = Attendance.Status,
        //    };

        //    return Response;

        //}


        //public async Task<DtoGetAbsenceDateRequest> GetAbsenceDate(int CourseId, int StudentId)
        //{
        //    var Date = await _context.LectureAttendances
        //       .Where(x => x.StudentId == StudentId && x.Lecture.CourseId == CourseId)
        //       .Include(x => x.Lecture)
        //       .Select(x => x.Lecture.LectureDate)
        //        .ToListAsync();

        //    var Response = new DtoGetAbsenceDateResponse()
        //    {
        //        LectureDate = Date
        //    };

        //    //.Select(x => new DtoGetAbsenceDateResponse()
        //    // {
        //    //     LectureDate = x.Lecture.LectureDate
        //    // });

        //    return Response;
        //}
    }
}
