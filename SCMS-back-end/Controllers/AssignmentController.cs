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
        public async Task<ActionResult<DtoAddAssignmentResponse>> GetAssignment(int id)
        {
            var assignmentInfo = await _context.GetAllAssignmentInfoByAssignmentID(id);

            if (assignmentInfo == null)
            {
                return NotFound(new { Message = "Assignment not found." });
            }

            return Ok(assignmentInfo);
        }

        // PUT: api/Assignment/5
        //[Authorize(Roles ="Teacher")]
        [HttpPut("{assignmentId}")]
        public async Task<ActionResult<DtoUpdateAssignmentResponse>> PutAssignment(int ID, DtoUpdateAssignmentRequest Assignment)
        {
            var Response = await _context.UpdateAssignmentByID(ID, Assignment);
            return Ok(Response);
        }

        [HttpDelete("{assignmentId}")]
        public async Task<IActionResult> DeleteAssignment(int AssignmentID)
        {
            await _context.DeleteAssignment(AssignmentID);
            return NoContent();
        }

        //rename the url 
        //[Authorize(Roles ="Student")]
        [HttpGet("[action]/{CourseId}")]
        public async Task<ActionResult<List<DtoStudentAssignmentResponse>>> GetAllStudentAssignmentsInCourse(int StudentID, int CourseId)
        {
            var AllAssignments = await _context.GetStudentAssignmentsByCourseId(CourseId, StudentID);

            if (AllAssignments == null)
                return NotFound();

            return Ok(AllAssignments);
        }

        //[Authorize(Roles ="Teacher")]
        [HttpGet("assignments/{assignmentId}/submissions")]
        public async Task<ActionResult<List<DtoStudentSubmissionResponse>>> GetAllStudentSubmissionForAssignment(int AssignmentId)
        {
            var AllStudents = await _context.GetAllStudentsSubmissionByAssignmentId(AssignmentId);

            if (AllStudents == null)
                return NotFound();

            return Ok(AllStudents);
        }

    }
}
