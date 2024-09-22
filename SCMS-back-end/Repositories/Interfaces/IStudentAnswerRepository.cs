using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto.Request;

namespace SCMS_back_end.Repositories.Interfaces
{
    public interface IStudentAnswerRepository
    {
        Task<StudentAnswer> GetByIdAsync(int id);
        Task<IEnumerable<StudentAnswer>> GetByStudentIdAsync(int studentId);
        Task<IEnumerable<StudentAnswer>> GetByQuizIdAsync(int quizId);
        Task AddAsync(StudentAnswer studentAnswer);
        Task UpdateAsync(UpdateStudentAnswerRequestDto studentAnswer);
        Task DeleteAsync(int id);
        //Task<QuizResult> GetFinalScoreAsync(int studentId , int quizId ); // For Score
        Task<(int correctAnswersCount, int totalQuestionsCount, int quizMark)> CalculateScoreAsync(int studentId, int quizId);
        Task<(int correctAnswersCount, int totalQuestionsCount, int quizMark, int score, IEnumerable<StudentAnswer> studentAnswers)> GetSavedScoreAsync(int studentId, int quizId);


        Task SaveAsync();
    }
}
