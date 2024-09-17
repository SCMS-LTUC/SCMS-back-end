using SCMS_back_end.Models;

namespace SCMS_back_end.Repositories.Interfaces
{
    public interface IQuestionRepository
    {
        Task<IEnumerable<Question>> GetQuestionsByQuizIdAsync(int quizId);
        Task<Question> GetQuestionByIdAsync(int questionId);
        Task AddQuestionAsync(Question question);
        Task UpdateQuestionAsync(Question question);
        Task DeleteQuestionAsync(int questionId);
    }
}
