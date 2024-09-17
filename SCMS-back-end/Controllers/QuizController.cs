using SCMS_back_end.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SCMS_back_end.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using SCMS_back_end.Data;
using SCMS_back_end.Models.Dto.Request;
using SCMS_back_end.Models.Dto.Response;
using SCMS_back_end.Services;
using System;
using System.Threading.Tasks;

namespace SCMS_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly IQuizRepository _quizService;

        public QuizController(IQuizRepository quizService)
        {
            _quizService = quizService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllQuizzes()
        {
            var quizzes = await _quizService.GetAllQuizzesAsync();
            return Ok(quizzes);
        }

        [HttpGet("{quizId}")]
        public async Task<IActionResult> GetQuizById(int quizId)
        {
            var quiz = await _quizService.GetQuizByIdAsync(quizId);
            if (quiz == null)
                return NotFound();

            return Ok(quiz);
        }

        [HttpPost]
        public async Task<IActionResult> AddQuiz([FromBody] Quiz quiz)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _quizService.AddQuizAsync(quiz);
            return CreatedAtAction(nameof(GetQuizById), new { quizId = quiz.QuizId }, quiz);
        }

        [HttpPut("{quizId}")]
        public async Task<IActionResult> UpdateQuiz(int quizId, [FromBody] Quiz quiz)
        {
            if (quizId != quiz.QuizId)
                return BadRequest();

            await _quizService.UpdateQuizAsync(quiz);
            return NoContent();
        }

        [HttpDelete("{quizId}")]
        public async Task<IActionResult> DeleteQuiz(int quizId)
        {
            await _quizService.DeleteQuizAsync(quizId);
            return NoContent();
        }
    }
}
