
using SCMS_back_end.Models;
using Microsoft.EntityFrameworkCore;
using SCMS_back_end.Data;
using SCMS_back_end.Repositories.Interfaces;

namespace SCMS_back_end.Services
{
    public class StudentQuizService : IStudentQuizRepository
    {
        private readonly StudyCenterDbContext _context;

        public StudentQuizService(StudyCenterDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StudentQuiz>> GetQuizzesByStudentIdAsync(int studentId)
        {
            return await _context.StudentQuizzes
                                 .Where(sq => sq.StudentId == studentId)
                                 .Include(sq => sq.Quiz)
                                 .ToListAsync();
        }

        public async Task<StudentQuiz> GetStudentQuizByIdAsync(int studentQuizId)
        {
            return await _context.StudentQuizzes
                                 .Include(sq => sq.Quiz)
                                 .FirstOrDefaultAsync(sq => sq.StudentQuizId == studentQuizId);
        }

        public async Task AddStudentQuizAsync(StudentQuiz studentQuiz)
        {
            _context.StudentQuizzes.Add(studentQuiz);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateStudentQuizAsync(StudentQuiz studentQuiz)
        {
            _context.StudentQuizzes.Update(studentQuiz);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteStudentQuizAsync(int studentQuizId)
        {
            var studentQuiz = await GetStudentQuizByIdAsync(studentQuizId);
            if (studentQuiz != null)
            {
                _context.StudentQuizzes.Remove(studentQuiz);
                await _context.SaveChangesAsync();
            }
        }
    }
}
