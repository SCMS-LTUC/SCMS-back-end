using SCMS_back_end.Models;
using System.Threading.Tasks;
using SCMS_back_end.Models.Dto.Request;

namespace SCMS_back_end.Services
{
    public interface IStudentAssignments
    {
        Task<StudentAssignment> AddOrUpdateStudentAssignmentAsync(StudentAssignmentDtoRequest studentAssignment);
        Task<StudentAssignment> UpdateStudentAssignmentAsync(int studentAssignmentId, int? grade, string feedback);
        Task<StudentAssignment> GetStudentAssignmentByIdAsync(int studentAssignmentId);
        Task<StudentAssignment> GetStudentAssignmentByAssignmentAndStudentAsync(int assignmentId, int studentId);
    }
}
