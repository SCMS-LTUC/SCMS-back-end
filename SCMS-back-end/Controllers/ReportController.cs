using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SCMS_back_end.Repositories.Interfaces;
using SCMS_back_end.Repositories.Services;
using System.Threading.Tasks;

namespace SCMS_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Admin")]
    public class ReportController : ControllerBase
    {
        private readonly IReport _reportService;
        public ReportController(IReport reportService)
        {
            _reportService = reportService;
        }
        [HttpGet("student-enrollment-overview")]
        public async Task<IActionResult> GetStudentEnrollmentOverview()
        {
            var result = await _reportService.GetStudentEnrollmentOverview();
            return Ok(result);
        }

        [HttpGet("course-performance-overview")]
        public async Task<IActionResult> GetCoursePerformanceOverview()
        {
            var result = await _reportService.GetCoursePerformanceOverview();
            return Ok(result);
        }

        [HttpGet("instructor-effectiveness-overview")]
        public async Task<IActionResult> GetInstructorEffectivenessOverview()
        {
            var result = await _reportService.GetInstructorEffectivenessOverview();
            return Ok(result);
        }

        [HttpGet("assignment-and-attendance-overview")]
        public async Task<IActionResult> GetAssignmentAndAttendanceOverview()
        {
            await _reportService.GetAssignmentAndAttendanceOverview();
            return Ok();
        }

        [HttpGet("departmental-activity-overview")]
        public async Task<IActionResult> GetDepartmentalActivityOverview()
        {
            var result = await _reportService.GetDepartmentalActivityOverview();
            return Ok(result);
        }

        [HttpGet("system-health-check-overview")]
        public async Task<IActionResult> GetSystemHealthCheckOverview()
        {
            var result = await _reportService.GetSystemHealthCheckOverview();
            return Ok(result);
        }
    }
}
