using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto.Request.Assignment;
using SCMS_back_end.Models.Dto.Response;
using SCMS_back_end.Models.Dto.Response.Assignment;
using System.Security.Claims;

namespace SCMS_back_end.Repositories.Interfaces
{
    public interface IAssignment
    {
        Task<DtoLectureAttendanceResponse> AddAssignment(DtoAddAssignmentRequest Assignment);

        Task<List<DtoLectureAttendanceResponse>> GetAllAssignmentsByCourseID(int CourseID);

        Task<DtoUpdateAssignmentResponse> UpdateAssignmentByID(int AssignmentID, DtoUpdateAssignmentRequest Assignment);

        Task<DtoLectureAttendanceResponse> GetAllAssignmentInfoByAssignmentID(int AssignmentID);

        Task DeleteAssignment(int AssignmentID);

        Task<List<DtoStudentAssignmentResponse>> GetStudentAssignmentsByCourseId(int courseId, ClaimsPrincipal userPrincipal);

        Task<List<DtoStudentSubmissionResponse>> GetStudentsSubmissionByAssignmentId(int assignmentId);
    }
}
