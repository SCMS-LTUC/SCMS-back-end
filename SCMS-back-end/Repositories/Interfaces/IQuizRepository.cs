using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto.Request;

namespace SCMS_back_end.Repositories.Interfaces
{
    public interface IQuizRepository
    {
        Task<IEnumerable<Quiz>> GetAllQuizzesAsync();
        Task<Quiz> GetQuizByIdAsync(int quizId);
        Task AddQuizAsync(Quiz quiz);
        Task UpdateQuizAsync(int quizId, QuizUpdateDto quizDto);
        Task DeleteQuizAsync(int quizId);
    }
}
