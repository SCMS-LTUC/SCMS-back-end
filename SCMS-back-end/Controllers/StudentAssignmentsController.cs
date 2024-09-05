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

        //[Authorize(Roles = "Teacher,Student")]
        [HttpPost("add")]
        public async Task<IActionResult> AddOrUpdateStudentAssignment([FromBody] StudentAssignmentDtoRequest studentAssignmentDto)
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
