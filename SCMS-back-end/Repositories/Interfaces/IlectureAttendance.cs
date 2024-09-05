using SCMS_back_end.Models.Dto.Request.LectureAttendance;
using SCMS_back_end.Models.Dto.Response.LectureAttendance;

namespace SCMS_back_end.Repositories.Interfaces
{
    public interface IlectureAttendance
    {
        public Task<DtoAddLectureAttendanceResponse> AddLectureAttendance(DtoAddLectureAttendanceRequest Attendance);
       
        public Task<List<DtoGetAbsenceDateResponse>> GetAbsenceDate(int CourseId, int StudentId);

        public Task<List<DtoGetAbsenceRateAndStudentResponse>> GetStudentAndAbsenceRate(int CourseId);
       
    }
}
