
using SCMS_back_end.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SCMS_back_end.Repositories.Interfaces;

namespace SCMS_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnswerOptionController : ControllerBase
    {
        private readonly IAnswerOptionRepository _answerOptionService;

        public AnswerOptionController(IAnswerOptionRepository answerOptionService)
        {
            _answerOptionService = answerOptionService;
        }

        [HttpGet("{questionId}")]
        public async Task<IActionResult> GetAnswerOptionsByQuestionId(int questionId)
        {
            var answerOptions = await _answerOptionService.GetAnswerOptionsByQuestionIdAsync(questionId);
            return Ok(answerOptions);
        }

        [HttpGet("answer/{answerOptionId}")]
        public async Task<IActionResult> GetAnswerOptionById(int answerOptionId)
        {
            var answerOption = await _answerOptionService.GetAnswerOptionByIdAsync(answerOptionId);
            if (answerOption == null)
                return NotFound();

            return Ok(answerOption);
        }

        [HttpPost]
        public async Task<IActionResult> AddAnswerOption([FromBody] AnswerOption answerOption)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _answerOptionService.AddAnswerOptionAsync(answerOption);
            return CreatedAtAction(nameof(GetAnswerOptionById), new { answerOptionId = answerOption.AnswerOptionId }, answerOption);
        }

        [HttpPut("{answerOptionId}")]
        public async Task<IActionResult> UpdateAnswerOption(int answerOptionId, [FromBody] AnswerOption answerOption)
        {
            if (answerOptionId != answerOption.AnswerOptionId)
                return BadRequest();

            await _answerOptionService.UpdateAnswerOptionAsync(answerOption);
            return NoContent();
        }

        [HttpDelete("{answerOptionId}")]
        public async Task<IActionResult> DeleteAnswerOption(int answerOptionId)
        {
            await _answerOptionService.DeleteAnswerOptionAsync(answerOptionId);
            return NoContent();
        }
    }
}
