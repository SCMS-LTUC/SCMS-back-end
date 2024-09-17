
using SCMS_back_end.Models;
using Microsoft.EntityFrameworkCore;
using SCMS_back_end.Data;
using SCMS_back_end.Repositories.Interfaces;

namespace SCMS_back_end.Services
{
    public class QuestionService : IQuestionRepository
    {
        private readonly StudyCenterDbContext _context;

        public QuestionService(StudyCenterDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Question>> GetQuestionsByQuizIdAsync(int quizId)
        {
            return await _context.Questions
                                 .Where(q => q.QuizId == quizId)
                                 .Include(q => q.AnswerOptions)
                                 .ToListAsync();
        }

        public async Task<Question> GetQuestionByIdAsync(int questionId)
        {
            return await _context.Questions
                                 .Include(q => q.AnswerOptions)
                                 .FirstOrDefaultAsync(q => q.QuestionId == questionId);
        }

        public async Task AddQuestionAsync(Question question)
        {
            _context.Questions.Add(question);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateQuestionAsync(Question question)
        {
            _context.Questions.Update(question);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteQuestionAsync(int questionId)
        {
            var question = await GetQuestionByIdAsync(questionId);
            if (question != null)
            {
                _context.Questions.Remove(question);
                await _context.SaveChangesAsync();
            }
        }
    }
}
