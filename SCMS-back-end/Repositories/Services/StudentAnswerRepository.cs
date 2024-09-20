using SCMS_back_end.Data;
using SCMS_back_end.Models;
using SCMS_back_end.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SCMS_back_end.Repositories.Services
{
    public class StudentAnswerRepository : IStudentAnswerRepository
    {
        private readonly StudyCenterDbContext _context;

        public StudentAnswerRepository(StudyCenterDbContext context)
        {
            _context = context;
        }

        public async Task<StudentAnswer> GetByIdAsync(int id)
        {
            return await _context.StudentAnswers.FindAsync(id);
        }

        public async Task<IEnumerable<StudentAnswer>> GetByStudentIdAsync(int studentId)
        {
            return await _context.StudentAnswers
                                 .Where(sa => sa.StudentId == studentId)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<StudentAnswer>> GetByQuizIdAsync(int quizId)
        {
            return await _context.StudentAnswers
                                 .Where(sa => sa.QuizId == quizId)
                                 .ToListAsync();
        }

        public async Task AddAsync(StudentAnswer studentAnswer)
        {
            await _context.StudentAnswers.AddAsync(studentAnswer);
        }

        public async Task UpdateAsync(StudentAnswer studentAnswer)
        {
            var existingStudentAnswer = await _context.StudentAnswers.FindAsync(studentAnswer.Id);
            if (existingStudentAnswer != null)
            {
                // Update properties
                existingStudentAnswer.QuizId = studentAnswer.QuizId;
                existingStudentAnswer.QuestionId = studentAnswer.QuestionId;
                existingStudentAnswer.SelectedAnswerOptionId = studentAnswer.SelectedAnswerOptionId;
                existingStudentAnswer.StudentId = studentAnswer.StudentId;
                existingStudentAnswer.SubmittedAt = studentAnswer.SubmittedAt;

                _context.StudentAnswers.Update(existingStudentAnswer);
            }
            else
            {
                throw new KeyNotFoundException("StudentAnswer not found");
            }
        }


        public async Task DeleteAsync(int id)
        {
            var studentAnswer = await _context.StudentAnswers.FindAsync(id);
            if (studentAnswer != null)
            {
                _context.StudentAnswers.Remove(studentAnswer);
            }
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}