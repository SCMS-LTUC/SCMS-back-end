using Microsoft.AspNetCore.Authorization;
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
        private readonly ILectureAttendance _attendance;

        public LectureAttendanceController(ILectureAttendance attendance)
        {
            _attendance = attendance;
        }


        // POST: api/LectureAttendance
        [Authorize(Roles ="Teacher")]
        [HttpPost]
        public async Task<ActionResult<LectureAttendance>> AddLectureAttendance(AddLectureAttendanceReqDto Attendance)
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


        [Authorize(Roles ="Teacher")]
        [HttpGet("{CourseId}")]
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


        [Authorize(Roles ="Student")]
        [HttpGet("/Student/{CourseId}")]
        public async Task<List<DtoGetAbsenceDateResponse>> GetAbsenceDate(int CourseId)
        {
            try
            {
                var response = await _attendance.GetAbsenceDate(CourseId, User);
                return response;
            }

            catch (Exception ex)
            {


                return new List<DtoGetAbsenceDateResponse>();
            }
          
        }
        
    }
}
