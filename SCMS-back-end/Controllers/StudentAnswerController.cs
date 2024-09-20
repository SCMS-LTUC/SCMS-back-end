using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Import this to use EF Core methods
using SCMS_back_end.Data;
using SCMS_back_end.Models;
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
            await _studentAnswerRepository.SaveAsync();
            return CreatedAtAction(nameof(GetStudentAnswer), new { id = studentAnswer.Id }, studentAnswer);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateStudentAnswer([FromBody] StudentAnswer studentAnswer)
        {
            // Ensure the body contains valid data
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { Errors = errors });
            }

            // Ensure the entity exists before updating
            if (studentAnswer.Id == 0)
            {
                return BadRequest("StudentAnswer ID is required.");
            }

            var existingStudentAnswer = await _studentAnswerRepository.GetByIdAsync(studentAnswer.Id);
            if (existingStudentAnswer == null)
            {
                return NotFound();
            }

            // Update the existing entity
            existingStudentAnswer.QuizId = studentAnswer.QuizId;
            existingStudentAnswer.QuestionId = studentAnswer.QuestionId;
            existingStudentAnswer.SelectedAnswerOptionId = studentAnswer.SelectedAnswerOptionId;
            existingStudentAnswer.StudentId = studentAnswer.StudentId;
            existingStudentAnswer.SubmittedAt = studentAnswer.SubmittedAt;

            // Save changes
            await _studentAnswerRepository.UpdateAsync(existingStudentAnswer);
            await _studentAnswerRepository.SaveAsync();

            // Return success response
            return Ok(new { Message = "StudentAnswer updated successfully." });
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudentAnswer(int id)
        {
            await _studentAnswerRepository.DeleteAsync(id);
            await _studentAnswerRepository.SaveAsync();
            return NoContent();
        }
    }
}
