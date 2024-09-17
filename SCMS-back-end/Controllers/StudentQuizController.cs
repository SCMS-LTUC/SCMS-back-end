
using SCMS_back_end.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SCMS_back_end.Repositories.Interfaces;

namespace SCMS_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentQuizController : ControllerBase
    {
        private readonly IStudentQuizRepository _studentQuizService;

        public StudentQuizController(IStudentQuizRepository studentQuizService)
        {
            _studentQuizService = studentQuizService;
        }

        [HttpGet("{studentId}")]
        public async Task<IActionResult> GetQuizzesByStudentId(int studentId)
        {
            var studentQuizzes = await _studentQuizService.GetQuizzesByStudentIdAsync(studentId);
            return Ok(studentQuizzes);
        }

        [HttpGet("studentquiz/{studentQuizId}")]
        public async Task<IActionResult> GetStudentQuizById(int studentQuizId)
        {
            var studentQuiz = await _studentQuizService.GetStudentQuizByIdAsync(studentQuizId);
            if (studentQuiz == null)
                return NotFound();

            return Ok(studentQuiz);
        }

        [HttpPost]
        public async Task<IActionResult> AddStudentQuiz([FromBody] StudentQuiz studentQuiz)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _studentQuizService.AddStudentQuizAsync(studentQuiz);
            return CreatedAtAction(nameof(GetStudentQuizById), new { studentQuizId = studentQuiz.StudentQuizId }, studentQuiz);
        }

        [HttpPut("{studentQuizId}")]
        public async Task<IActionResult> UpdateStudentQuiz(int studentQuizId, [FromBody] StudentQuiz studentQuiz)
        {
            if (studentQuizId != studentQuiz.StudentQuizId)
                return BadRequest();

            await _studentQuizService.UpdateStudentQuizAsync(studentQuiz);
            return NoContent();
        }

        [HttpDelete("{studentQuizId}")]
        public async Task<IActionResult> DeleteStudentQuiz(int studentQuizId)
        {
            await _studentQuizService.DeleteStudentQuizAsync(studentQuizId);
            return NoContent();
        }
    }
}
