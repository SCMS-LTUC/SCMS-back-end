using SCMS_back_end.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SCMS_back_end.Repositories.Interfaces;

namespace SCMS_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionRepository _questionService;

        public QuestionController(IQuestionRepository questionService)
        {
            _questionService = questionService;
        }

        [HttpGet("{quizId}")]
        public async Task<IActionResult> GetQuestionsByQuizId(int quizId)
        {
            var questions = await _questionService.GetQuestionsByQuizIdAsync(quizId);
            return Ok(questions);
        }

        [HttpGet("question/{questionId}")]
        public async Task<IActionResult> GetQuestionById(int questionId)
        {
            var question = await _questionService.GetQuestionByIdAsync(questionId);
            if (question == null)
                return NotFound();

            return Ok(question);
        }

        [HttpPost]
        public async Task<IActionResult> AddQuestion([FromBody] Question question)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _questionService.AddQuestionAsync(question);
            return CreatedAtAction(nameof(GetQuestionById), new { questionId = question.QuestionId }, question);
        }

        [HttpPut("{questionId}")]
        public async Task<IActionResult> UpdateQuestion(int questionId, [FromBody] Question question)
        {
            if (questionId != question.QuestionId)
                return BadRequest();

            await _questionService.UpdateQuestionAsync(question);
            return NoContent();
        }

        [HttpDelete("{questionId}")]
        public async Task<IActionResult> DeleteQuestion(int questionId)
        {
            await _questionService.DeleteQuestionAsync(questionId);
            return NoContent();
        }
    }
}
