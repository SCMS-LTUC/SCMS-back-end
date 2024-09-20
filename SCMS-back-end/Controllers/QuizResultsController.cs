using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto;
using SCMS_back_end.Repositories.Interfaces;

namespace SCMS_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizResultsController : ControllerBase
    {
        private readonly IQuizResultRepository _quizResultRepository;

        public QuizResultsController(IQuizResultRepository quizResultRepository)
        {
            _quizResultRepository = quizResultRepository;
        }

        // GET: api/QuizResults/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuizResultById(int id)
        {
            var quizResult = await _quizResultRepository.GetByIdAsync(id);
            if (quizResult == null)
            {
                return NotFound();
            }
            return Ok(quizResult);
        }

        // GET: api/QuizResults/student/{studentId}
        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetQuizResultsByStudent(int studentId)
        {
            var quizResults = await _quizResultRepository.GetByStudentIdAsync(studentId);
            return Ok(quizResults);
        }

        // GET: api/QuizResults/quiz/{quizId}
        [HttpGet("quiz/{quizId}")]
        public async Task<IActionResult> GetQuizResultsByQuiz(int quizId)
        {
            var quizResults = await _quizResultRepository.GetByQuizIdAsync(quizId);
            return Ok(quizResults);
        }

        // POST: api/QuizResults
        [HttpPost]
        public async Task<IActionResult> CreateQuizResult([FromBody] QuizResultRequestDto quizResultDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var quizResult = new QuizResult
            {
                QuizId = quizResultDto.QuizId,
                StudentId = quizResultDto.StudentId,
                Score = quizResultDto.Score,
                TotalQuestions = quizResultDto.TotalQuestions,
                SubmittedAt = quizResultDto.SubmittedAt
            };

            await _quizResultRepository.AddAsync(quizResult);
            return CreatedAtAction(nameof(GetQuizResultById), new { id = quizResult.Id }, quizResult);
        }

        // PUT: api/QuizResults/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuizResult(int id, [FromBody] QuizResultRequestDto quizResultDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var quizResult = await _quizResultRepository.GetByIdAsync(id);
            if (quizResult == null)
            {
                return NotFound();
            }

            quizResult.Score = quizResultDto.Score;
            quizResult.TotalQuestions = quizResultDto.TotalQuestions;
            quizResult.SubmittedAt = quizResultDto.SubmittedAt;

            await _quizResultRepository.UpdateAsync(quizResult);
            return NoContent();
        }

        // DELETE: api/QuizResults/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuizResult(int id)
        {
            var quizResult = await _quizResultRepository.GetByIdAsync(id);
            if (quizResult == null)
            {
                return NotFound();
            }

            await _quizResultRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
