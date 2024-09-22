﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Import this to use EF Core methods
using SCMS_back_end.Data;
using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto.Request;
using SCMS_back_end.Repositories.Interfaces;
using SCMS_back_end.Repositories.Services;

namespace SCMS_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentAnswerController : ControllerBase
    {
        private readonly IStudentAnswerRepository _studentAnswerRepository;
        private readonly StudyCenterDbContext _context; // Add this line

        public StudentAnswerController(IStudentAnswerRepository studentAnswerRepository, StudyCenterDbContext context)
        {
            _studentAnswerRepository = studentAnswerRepository;
            _context = context; // Initialize the context
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StudentAnswer>> GetStudentAnswer(int id)
        {
            var studentAnswer = await _studentAnswerRepository.GetByIdAsync(id);
            if (studentAnswer == null)
            {
                return NotFound();
            }
            return Ok(studentAnswer);
        }

        [HttpGet("student/{studentId}")]
        public async Task<ActionResult<IEnumerable<StudentAnswer>>> GetStudentAnswersByStudent(int studentId)
        {
            var studentAnswers = await _studentAnswerRepository.GetByStudentIdAsync(studentId);
            return Ok(studentAnswers);
        }

        [HttpGet("quiz/{quizId}")]
        public async Task<ActionResult<IEnumerable<StudentAnswer>>> GetStudentAnswersByQuiz(int quizId)
        {
            var studentAnswers = await _studentAnswerRepository.GetByQuizIdAsync(quizId);
            return Ok(studentAnswers);
        }

        
        [HttpPost]
        public async Task<ActionResult> CreateStudentAnswer([FromBody] StudentAnswer studentAnswer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if the referenced Student exists
            var studentExists = await _context.Students.AnyAsync(s => s.StudentId == studentAnswer.StudentId);
            if (!studentExists)
            {
                return BadRequest($"Student with ID {studentAnswer.StudentId} does not exist.");
            }

            // Check if the referenced Quiz exists
            var quizExists = await _context.Quizzes.AnyAsync(q => q.QuizId == studentAnswer.QuizId);
            if (!quizExists)
            {
                return BadRequest($"Quiz with ID {studentAnswer.QuizId} does not exist.");
            }

            // Check if the referenced Question exists
            var questionExists = await _context.Questions.AnyAsync(q => q.QuestionId == studentAnswer.QuestionId);
            if (!questionExists)
            {
                return BadRequest($"Question with ID {studentAnswer.QuestionId} does not exist.");
            }

            // Check if the referenced AnswerOption exists
            var answerOptionExists = await _context.AnswerOptions.AnyAsync(a => a.AnswerOptionId == studentAnswer.SelectedAnswerOptionId);
            if (!answerOptionExists)
            {
                return BadRequest($"Answer option with ID {studentAnswer.SelectedAnswerOptionId} does not exist.");
            }

            // All checks passed, add the student answer
            await _studentAnswerRepository.AddAsync(studentAnswer);
            return CreatedAtAction(nameof(GetStudentAnswer), new { id = studentAnswer.Id }, studentAnswer);
        }


        [HttpPut]
        public async Task<IActionResult> UpdateStudentAnswer([FromBody] UpdateStudentAnswerRequestDto studentAnswerDto)
        {
            // Ensure the body contains valid data
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { Errors = errors });
            }

            try
            {
                // Call the service method to update the student answer
                await _studentAnswerRepository.UpdateAsync(studentAnswerDto);
                await _studentAnswerRepository.SaveAsync();

                // Return success response
                return Ok(new { Message = "StudentAnswer updated successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while updating the student answer.", Details = ex.Message });
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudentAnswer(int id)
        {
            await _studentAnswerRepository.DeleteAsync(id);
            await _studentAnswerRepository.SaveAsync();
            return NoContent();
        }


        // POST: api/StudentAnswer/calculate-score
        [HttpPost("calculate-score")]
        public async Task<ActionResult> CalculateScore(int studentId, int quizId)
        {
            var (correctAnswers, totalQuestions, quizMark) = await _studentAnswerRepository.CalculateScoreAsync(studentId, quizId);

            return Ok(new
            {
                CorrectAnswers = correctAnswers,
                TotalQuestions = totalQuestions,
                QuizMark = quizMark,
                Message = "Score calculated successfully."
            });
        }



        // GET: api/StudentAnswer/get-saved-score
        [HttpGet("get-saved-score")]
        public async Task<ActionResult<(int correctAnswersCount, int totalQuestionsCount, int quizMark, int score, IEnumerable<StudentAnswer> studentAnswers)>> GetSavedScore(int studentId, int quizId)
        {
            var result = await _studentAnswerRepository.GetSavedScoreAsync(studentId, quizId);

            // Check if there are any answers or quiz result
            if (result.score == 0) // Adjust this check based on your requirements
            {
                return NotFound("Score not found");
            }

            return Ok(result);
        }


    }
}
