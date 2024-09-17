using SCMS_back_end.Models;

namespace SCMS_back_end.Repositories.Interfaces
{
    public interface IStudentQuizRepository
    {
        Task<IEnumerable<StudentQuiz>> GetQuizzesByStudentIdAsync(int studentId);
        Task<StudentQuiz> GetStudentQuizByIdAsync(int studentQuizId);
        Task AddStudentQuizAsync(StudentQuiz studentQuiz);
        Task UpdateStudentQuizAsync(StudentQuiz studentQuiz);
        Task DeleteStudentQuizAsync(int studentQuizId);
    }
}
