using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto.Request.Announcement;
using SCMS_back_end.Models.Dto.Request.Assignment;
using SCMS_back_end.Models.Dto.Response;
using SCMS_back_end.Models.Dto.Response.Announcement;
using SCMS_back_end.Models.Dto.Response.Assignment;
using SCMS_back_end.Repositories.Interfaces;

namespace SCMS_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnnouncementController : ControllerBase
    {
        private readonly IAnnouncement _Announcement;
        public AnnouncementController(IAnnouncement context)
        {
            _Announcement = context;
        }
        // POST: api/Announcement
        //[Authorize(Roles ="Admin")]
        [HttpPost]
        public async Task<ActionResult<DtoPostAnnouncementByAdmin>> PostAnnouncementByAdmin(DtoPostAnnouncementByAdmin Announcement)
        {
            var response = await _Announcement.PostAnnouncementByAdmin(Announcement);
            // return Ok(response);
            return Announcement;
        }

        // POST: api/Announcement
        //[Authorize(Roles ="Teacher")]
        [HttpPost("[Action]/{CourseID}")]
        public async Task<ActionResult<DtoPostAnnouncementByTeacher>> PostAnnouncementByTeacher(DtoPostAnnouncementByTeacher Announcement, int CourseID)
        {
            var response = await _Announcement.PostAnnouncementByTeacher(Announcement, CourseID);
            // return Ok(response);
            return Announcement;
        }


        //[Authorize(Roles = "Teacher")]
        [HttpGet("[Action]/{AudinceID}")]
        public async Task<ActionResult<List<DtoGetAnnouncementRes>>> GetAllTeacherAnnouncement(int AudinceID)
        {
            try
            {
                var AllAnnouncement = await _Announcement.GetAllTeacherAnnouncement(AudinceID);
                if (AllAnnouncement == null)
                    return NotFound("No Announcement found .");

                return Ok(AllAnnouncement);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }


        }



        //[Authorize(Roles = "Studnet")]
        [HttpGet("[Action]/{AudinceID}")]
        public async Task<ActionResult<List<DtoGetAnnouncementRes>>> GetAllStudentAnnouncement(int AudinceID)
        {
            try
            {
                var AllAnnouncement = await _Announcement.GetAllStudentAnnouncement(AudinceID);
                if (AllAnnouncement == null)
                    return NotFound("No Announcement found .");

                return Ok(AllAnnouncement);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }


        }

       
        // GET: api/Announcement/id
        [HttpGet("{CourseID}")]
        public async Task<ActionResult<DtoGetAnnouncementRes>> GetCourse(int CourseID)
        {
            var course = await _Announcement.GetAnnouncementByCourseID(CourseID);

            if (course == null)
            {
                return NotFound();
            }

            return Ok(course);
        }

    }
}
