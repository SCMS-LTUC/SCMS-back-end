using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto.Request;

namespace SCMS_back_end.Repositories.Interfaces
{
    public interface IStudent
    {
        void AddStudent(StudentDtoRequest student);
        void DeleteStudent(int id);
        void DropStudentFromCourse(int studentId, int courseId);
        void EnrollStudentInCourse(int studentId, int courseId);
        IEnumerable<Student> GetAllStudents();
        Student GetStudentById(int id);
        IEnumerable<Student> GetStudentsByCourseId(int courseId);
        void UpdateStudent(int id, StudentDtoRequest student);
    }
}
