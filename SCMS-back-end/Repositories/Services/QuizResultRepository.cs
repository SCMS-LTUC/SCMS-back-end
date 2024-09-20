using Microsoft.EntityFrameworkCore;
using SCMS_back_end.Data;
using SCMS_back_end.Models;
using SCMS_back_end.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCMS_back_end.Repositories.Services
{
    public class QuizResultRepository : IQuizResultRepository
    {
        private readonly StudyCenterDbContext _context;

        public QuizResultRepository(StudyCenterDbContext context)
        {
            _context = context;
        }

        public async Task<QuizResult> GetByIdAsync(int id)
        {
            return await _context.QuizResults
                .Include(qr => qr.Quiz)
                .Include(qr => qr.Student)
                .FirstOrDefaultAsync(qr => qr.Id == id);
        }

        public async Task<IEnumerable<QuizResult>> GetByStudentIdAsync(int studentId)
        {
            return await _context.QuizResults
                .Include(qr => qr.Quiz)
                .Where(qr => qr.StudentId == studentId)
                .ToListAsync();
        }

        public async Task<IEnumerable<QuizResult>> GetByQuizIdAsync(int quizId)
        {
            return await _context.QuizResults
                .Include(qr => qr.Student)
                .Where(qr => qr.QuizId == quizId)
                .ToListAsync();
        }

        public async Task AddAsync(QuizResult quizResult)
        {
            await _context.QuizResults.AddAsync(quizResult);
            await SaveAsync();
        }

        public async Task UpdateAsync(QuizResult quizResult)
        {
            _context.QuizResults.Update(quizResult);
            await SaveAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var quizResult = await GetByIdAsync(id);
            if (quizResult != null)
            {
                _context.QuizResults.Remove(quizResult);
                await SaveAsync();
            }
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
