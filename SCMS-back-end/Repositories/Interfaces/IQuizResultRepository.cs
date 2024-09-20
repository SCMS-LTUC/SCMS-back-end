using SCMS_back_end.Models;

namespace SCMS_back_end.Repositories.Interfaces
{
    public interface IQuizResultRepository
    {
        Task<QuizResult> GetByIdAsync(int id);
        Task<IEnumerable<QuizResult>> GetByStudentIdAsync(int studentId);
        Task<IEnumerable<QuizResult>> GetByQuizIdAsync(int quizId);
        Task AddAsync(QuizResult quizResult);
        Task UpdateAsync(QuizResult quizResult);
        Task DeleteAsync(int id);
        Task SaveAsync();
    }
}
