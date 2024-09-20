using SCMS_back_end.Models;

namespace SCMS_back_end.Repositories.Interfaces
{
    public interface IStudentAnswerRepository
    {
        Task<StudentAnswer> GetByIdAsync(int id);
        Task<IEnumerable<StudentAnswer>> GetByStudentIdAsync(int studentId);
        Task<IEnumerable<StudentAnswer>> GetByQuizIdAsync(int quizId);
        Task AddAsync(StudentAnswer studentAnswer);
        Task UpdateAsync(StudentAnswer studentAnswer);
        Task DeleteAsync(int id);
        Task SaveAsync();
    }
}
