using SCMS_back_end.Data;
using SCMS_back_end.Models;
using SCMS_back_end.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SCMS_back_end.Models.Dto.Request;

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
            // Ensure related entities are loaded
            await _context.StudentAnswers.AddAsync(studentAnswer);
            await _context.SaveChangesAsync(); // Save the new answer first

            // Update the numberOfCorrectAnswers if the selected answer is correct
            if (studentAnswer.SelectedAnswerOption.IsCorrect)
            {
                // Fetch the quiz result for the student and the quiz
                var quizResult = await _context.QuizResults
                    .FirstOrDefaultAsync(qr => qr.StudentId == studentAnswer.StudentId
                                            && qr.QuizId == studentAnswer.Question.QuizId);

                // If a quiz result exists, update the correct answers count
                if (quizResult != null)
                {
                    quizResult.NumbersOfCorrectAnswers++;
                    await _context.SaveChangesAsync(); // Save the updated quiz result
                }
            }
        }


        public async Task UpdateAsync(UpdateStudentAnswerRequestDto studentAnswer)
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

        public async Task<QuizResult> GetFinalScoreAsync(int studentId, int quizId)
        {
            // Ensure related entities are included
            var quizResult = await _context.QuizResults
                .Include(qr => qr.Quiz)
                .ThenInclude(q => q.Questions)
                .FirstOrDefaultAsync(qr => qr.StudentId == studentId && qr.QuizId == quizId);

            if (quizResult != null && quizResult.Quiz != null)
            {
                // Perform division with proper casting to avoid integer division
                var totalQuestions = quizResult.Quiz.Questions.Count;
                if (totalQuestions > 0)
                {
                    quizResult.Score = (int)Math.Round((decimal)quizResult.NumbersOfCorrectAnswers / totalQuestions * quizResult.Quiz.Mark, 2);
                }

                // Optionally save changes if you are updating the score in the database
                await _context.SaveChangesAsync();
            }

            return quizResult;
        }

    }
}