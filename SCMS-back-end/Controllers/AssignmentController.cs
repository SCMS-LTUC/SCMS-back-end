﻿using Microsoft.AspNetCore.Authorization;
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
        [HttpPost("[action]")]
        public async Task<ActionResult<DtoAddAssignmentResponse>> AddAssignment(DtoAddAssignmentRequest Assignment)
        {
           var response= await _context.AddAssignment(Assignment);
            return Ok(response);
        }

        //[Authorize(Roles ="Teacher")]
        [HttpGet("GetAllAssignmentsByCourse/{id}")]
        public async Task<ActionResult> GetAllAssignmentsByCourseID(int id)
        {
            var AllAssignments = await _context.GetAllAssignmentsByCourseID(id);

            if (AllAssignments == null)
                return NotFound();

            return Ok(AllAssignments);
        }

        //[Authorize(Roles ="Teacher","Student")]
        // GET: api/Assignment/5
        [HttpGet("[action]/{id}")]
        public async Task<ActionResult<DtoAddAssignmentResponse>> GetAssignmentInfoByID(int id)
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
        [HttpPut("[action]/{ID}")]
        public async Task<ActionResult<DtoUpdateAssignmentResponse>> UpdateAssignmentInfo(int ID, DtoUpdateAssignmentRequest Assignment)
        {
            var Response = await _context.UpdateAssignmentByID(ID, Assignment);
            return Ok(Response);
        }

        [HttpDelete("{AssignmentID}")]
        public async Task DeleteDepartment(int AssignmentID)
        {
            await _context.DeleteAssignment(AssignmentID);
           // return Ok();
        }

        //[Authorize(Roles ="Student")]
        [HttpGet("[action]/{CourseId}")]
        public async Task<ActionResult<List<DtoGetAllStudentAssignmentsRequest>>> GetAllStudentAssignments(int StudentID)
        {
            var AllAssignments = await _context.GetAllStudentAssignments(StudentID);

            if (AllAssignments == null)
                return NotFound();

            return Ok(AllAssignments);
        }

        //[Authorize(Roles ="Teacher")]
        [HttpGet("[action]/{CourseID}")]
        public async Task<ActionResult<List<DtoGetAllStudentRquest>>> GetAllStudent(int CourseID)
        {
            var AllStudents = await _context.GetAllStudentAssignmentByCourseID(CourseID);

            if (AllStudents == null)
                return NotFound();

            return Ok(AllStudents);
        }

    }
}
