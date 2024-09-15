using Microsoft.AspNetCore.Mvc;
using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto.Request;
using SCMS_back_end.Services;
using System.Threading.Tasks;

namespace SCMS_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentAssignmentsController : ControllerBase
    {
        private readonly IStudentAssignments _studentAssignmentsService;

        public StudentAssignmentsController(IStudentAssignments studentAssignmentsService)
        {
            _studentAssignmentsService = studentAssignmentsService;
        }
        [HttpGet("download/{studentAssignmentId}")]
        public async Task<IActionResult> DownloadFile(int studentAssignmentId)
        {
            var studentAssignment = await _studentAssignmentsService.GetStudentAssignmentByIdAsync(studentAssignmentId);
            if (studentAssignment == null || string.IsNullOrEmpty(studentAssignment.Submission))
            {
                return NotFound("File not found or no submission.");
            }

            var filePath = studentAssignment.Submission;

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File does not exist.");
            }

            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            var contentType = "APPLICATION/octet-stream";
            var fileName = Path.GetFileName(filePath);

            return File(memory, contentType, fileName);
        }

        //[Authorize(Roles = "Teacher,Student")]
        [HttpPost("add")]
        public async Task<IActionResult> AddOrUpdateStudentAssignment([FromForm] StudentAssignmentDtoRequest studentAssignmentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _studentAssignmentsService.AddOrUpdateStudentAssignmentAsync(studentAssignmentDto);
                return CreatedAtAction(nameof(GetStudentAssignmentById), new { id = result.StudentAssignmentId }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        //[Authorize(Roles = "Teacher")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateStudentAssignment(int id, [FromBody] UpdateStudentAssignmentRequest request)
        {
            if (id <= 0 || request == null)
            {
                return BadRequest("Invalid input data.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _studentAssignmentsService.UpdateStudentAssignmentAsync(id, request.Grade, request.Feedback);
                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //[Authorize(Roles = "Teacher,Student")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudentAssignmentById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid id parameter.");
            }

            try
            {
                var result = await _studentAssignmentsService.GetStudentAssignmentByIdAsync(id);
                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //[Authorize(Roles = "Teacher,Student")]
        [HttpGet("byAssignmentAndStudent")]
        public async Task<IActionResult> GetStudentAssignmentByAssignmentAndStudent([FromQuery] int assignmentId, [FromQuery] int studentId)
        {
            if (assignmentId <= 0 || studentId <= 0)
            {
                return BadRequest("Invalid assignmentId or studentId parameter.");
            }

            try
            {
                var result = await _studentAssignmentsService.GetStudentAssignmentByAssignmentAndStudentAsync(assignmentId, studentId);
                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
