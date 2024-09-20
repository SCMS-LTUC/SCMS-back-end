
using SCMS_back_end.Models;
using Microsoft.EntityFrameworkCore;
using SCMS_back_end.Data;
using SCMS_back_end.Repositories.Interfaces;
using SCMS_back_end.Models.Dto.Request;

namespace SCMS_back_end.Services
{
    public class QuizService : IQuizRepository
    {
        private readonly StudyCenterDbContext _context;

        public QuizService(StudyCenterDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Quiz>> GetAllQuizzesAsync()
        {
            return await _context.Quizzes.Include(q => q.Questions).ToListAsync();
        }

        public async Task<Quiz> GetQuizByIdAsync(int quizId)
        {
            return await _context.Quizzes
                                 .Include(q => q.Questions)
                                 .FirstOrDefaultAsync(q => q.QuizId == quizId);
        }

        public async Task AddQuizAsync(Quiz quiz)
        {
            _context.Quizzes.Add(quiz);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateQuizAsync(int quizId, QuizUpdateDto quizDto)
        {
            var existingQuiz = await _context.Quizzes.FindAsync(quizId);
            if (existingQuiz == null)
            {
                throw new KeyNotFoundException("Quiz not found");
            }

            // Update the properties
            existingQuiz.Title = quizDto.Title;
            existingQuiz.Duration = quizDto.Duration;
            existingQuiz.IsVisible = quizDto.IsVisible;
            existingQuiz.CourseId = quizDto.CourseId;

            await _context.SaveChangesAsync();
        }






        public async Task DeleteQuizAsync(int quizId)
        {
            var quiz = await GetQuizByIdAsync(quizId);
            if (quiz != null)
            {
                _context.Quizzes.Remove(quiz);
                await _context.SaveChangesAsync();
            }
        }
    }
}
