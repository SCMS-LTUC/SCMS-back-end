using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto.Request.LectureAttendance;
using SCMS_back_end.Models.Dto.Response.LectureAttendance;
using SCMS_back_end.Repositories.Interfaces;

namespace SCMS_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LectureAttendanceController : ControllerBase
    {
        private readonly IlectureAttendance _attendance;

        public LectureAttendanceController(IlectureAttendance attendance)
        {
            _attendance = attendance;
        }


        // POST: api/LectureAttendance
        //[Authorize(Roles ="Teacher")]
        [HttpPost("[action]")]
        public async Task<ActionResult<DtoAddLectureAttendanceResponse>> AddLectureAttendance(DtoAddLectureAttendanceRequest Attendance)
        {
            //var response = await _attendance.AddLectureAttendance(Attendance);
            //return Ok(response);

            try
            {
                var response = await _attendance.AddLectureAttendance(Attendance);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }


        //[Authorize(Roles ="Student")]
        [HttpGet("[action]/{CourseId}")]
        public async Task<List<DtoGetAbsenceRateAndStudentResponse>> GetStudentAndAbsenceRate(int CourseId)
        {
            //var response = await _attendance.GetStudentAndAbsenceRate(CourseId);
            //return response;

            try
            {
                var response = await _attendance.GetStudentAndAbsenceRate(CourseId);
                return response;
            }

            catch (Exception ex)
            {
                return new List<DtoGetAbsenceRateAndStudentResponse>();
            }
        }


        //[Authorize(Roles ="Student")]
        [HttpGet("[action]/{CourseId}/{StudentId}")]
        public async Task<List<DtoGetAbsenceDateResponse>> GetAbsenceDate(int CourseId, int StudentId)
        {
            try
            {
                var response = await _attendance.GetAbsenceDate(CourseId, StudentId);
                return response;
            }

            catch (Exception ex)
            {


                return new List<DtoGetAbsenceDateResponse>();
            }
          
        }

        //[HttpPost("[action]")]
        //public async Task<ActionResult<DtoLectureAttendanceResponse>> GetStudentAndAbsenceRate(DtoAddLectureAttendanceRequest request)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        // Return a BadRequest response with validation errors
        //        return BadRequest(ModelState);
        //    }

        //    try
        //    {
        //        var response = await _attendance.GetStudentAndAbsenceRate(request);
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception (consider using a logging framework)
        //        // Return a generic error response
        //        return StatusCode(500, "An unexpected error occurred.");
        //    }
        //}
    }
}
