using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto.Request.Teacher;
using SCMS_back_end.Models.Dto.Response;
using SCMS_back_end.Models.Dto.Response.Teacher;
using SCMS_back_end.Repositories.Interfaces;

namespace SCMS_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherController : ControllerBase
    {
        private readonly ITeacher teacher;

        public TeacherController(ITeacher teacher)
        {
            this.teacher = teacher;
        }

        //// POST: api/Teacher
        ////[Authorize(Roles ="Admin")]
        //[HttpPost("[action]")]
        //public async Task<ActionResult<DtoAddTeacherResponse>> AddTeacher(DtoAddTeacherRequest Teacher)
        //{
        //    var Response = await teacher.AddTeacher(Teacher);
        //    return Ok(Response);
        //}



        // PUT: api/Teacher/5
        //[Authorize(Roles ="Admin")]
        [HttpPut("[action]/{ID}")]
        public async Task<IActionResult> UpdateTeacherInfo(int ID, DtoUpdateTeacherInfoByAdminRequest Teacher)
        {
            var ter= await teacher.UpdateTeacherInfoByID(ID, Teacher);
            
            return Ok();
        }

        //[Authorize(Roles ="Admin")]
        // GET: api/Teacher/5
        [HttpGet("[action]/{id}")]
        public async Task<ActionResult<DtoGetTeacherInfoByIDRequest>> GetTeacherInfoByID(int id)
        {
            var TeacherInfo = await teacher.GetTeacherInfoByID(id);

            if (TeacherInfo == null)
            {
                return NotFound(new { Message = "Teacher not found." });
            }

            return Ok(TeacherInfo);
        }

        // GET: api/Teachers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DtoGetAllTeacherRequest>>> GetAllTeachers()
        {
            return await teacher.GetAllTeachers();
        }
    }
}
