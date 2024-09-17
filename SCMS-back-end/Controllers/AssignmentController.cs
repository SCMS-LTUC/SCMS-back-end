using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto.Request.Assignment;
using SCMS_back_end.Models.Dto.Response.Assignment;
using SCMS_back_end.Repositories.Interfaces;
using SCMS_back_end.Repositories.Services;
using System.Runtime.InteropServices;

namespace SCMS_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignmentController : ControllerBase
    {

        private readonly IAssignment _context;
        public AssignmentController(IAssignment context)
        {
            _context = context;
        }

        // POST: api/Assignment
        //[Authorize(Roles ="Teacher")]
        [HttpPost]
        public async Task<ActionResult<DtoAddAssignmentResponse>> PostAssignment(DtoAddAssignmentRequest Assignment)
        {
           var response= await _context.AddAssignment(Assignment);
            return Ok(response);
        }

        //[Authorize(Roles ="Teacher")]
        [HttpGet("courses/{courseId}/assignments")]
        public async Task<ActionResult> GetAssignmentsByCourseID(int courseId)
        {
            var AllAssignments = await _context.GetAllAssignmentsByCourseID(courseId);

            if (AllAssignments == null)
                return NotFound();

            return Ok(AllAssignments);
        }

        //[Authorize(Roles ="Teacher","Student")]
        // GET: api/Assignment/5
        [HttpGet("{assignmentId}")]
        public async Task<ActionResult<DtoAddAssignmentResponse>> GetAssignment(int assignmentId)
        {
            var assignmentInfo = await _context.GetAllAssignmentInfoByAssignmentID(assignmentId);

            if (assignmentInfo == null)
            {
                return NotFound(new { Message = "Assignment not found." });
            }

            return Ok(assignmentInfo);
        }

        // PUT: api/Assignment/5
        //[Authorize(Roles ="Teacher")]
        [HttpPut("{assignmentId}")]
        public async Task<ActionResult<DtoUpdateAssignmentResponse>> PutAssignment(int assignmentId, DtoUpdateAssignmentRequest Assignment)
        {
            var Response = await _context.UpdateAssignmentByID(assignmentId, Assignment);
            return Ok(Response);
        }

        [HttpDelete("{assignmentId}")]
        public async Task<IActionResult> DeleteAssignment(int AssignmentID)
        {
            await _context.DeleteAssignment(AssignmentID);
            return NoContent();
        }

        [Authorize(Roles ="Student")]
        [HttpGet("courses/{courseId}/student/assignments")]
        public async Task<ActionResult<List<DtoStudentAssignmentResponse>>> GetStudentAssignmentsForCourse(int courseId)
        {
            try
            {
                var AllAssignments = await _context.GetStudentAssignmentsByCourseId(courseId, User);
                if (AllAssignments == null)
                    return NotFound("No assignments found for this course.");

                return Ok(AllAssignments);
            }
            catch(InvalidOperationException ex) 
            {
                return NotFound(ex.Message);
            }

        }

        //[Authorize(Roles ="Teacher")]
        [HttpGet("{assignmentId}/submissions")]
        public async Task<ActionResult<List<DtoStudentSubmissionResponse>>> GetStudentsSubmissionForAssignment(int assignmentId)
        {
            try
            {
                var AllStudents = await _context.GetStudentsSubmissionByAssignmentId(assignmentId);
                if (AllStudents == null)
                    return NotFound();

                return Ok(AllStudents);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

    }
}
