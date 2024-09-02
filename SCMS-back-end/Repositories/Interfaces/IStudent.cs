using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto.Request;
using SCMS_back_end.Models.Dto.Response;

namespace SCMS_back_end.Repositories.Interfaces
{
    public interface IStudent
    {
        Task DeleteStudentAsync(int id);
        Task DropStudentFromCourseAsync(int studentId, int courseId);
        Task EnrollStudentInCourseAsync(int studentId, int courseId);
        Task<IEnumerable<StudentDtoResponse>> GetAllStudentsAsync();
        Task<StudentDtoResponse> GetStudentByIdAsync(int id);
        Task<IEnumerable<StudentDtoResponse>> GetStudentsByCourseIdAsync(int courseId);
        Task UpdateStudentAsync(int id, StudentDtoRequest student);
    }
}
