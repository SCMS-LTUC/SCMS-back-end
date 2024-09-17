using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SCMS_back_end.Data;
using SCMS_back_end.Models;
using SCMS_back_end.Repositories.Interfaces;
using SCMS_back_end.Models.Dto.Request;
using SCMS_back_end.Models.Dto.Response;
using SCMS_back_end.Repositories.Services;

namespace SCMS_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ICourse _course;

        public CoursesController(ICourse course)
        {
            _course = course;
        }

        // POST: api/Courses
        [HttpPost]
        public async Task<ActionResult<Course>> PostCourse(DtoCreateCourseWTRequest course)
        {
            var newCourse = await _course.CreateCourseWithoutTeacher(course);
            return Ok(newCourse);
        }
        
        // Tested
        // PUT: api/Courses/5
        [HttpPut("{id}")]
        public async Task<ActionResult<Course>> PutCourse(int id, DtoUpdateCourseRequest course)
        {
            var updatedCourse = await _course.UpdateCourseInformation(id, course);

            if (updatedCourse == null)
            {
                return NotFound();
            }

            return Ok(updatedCourse);
        }

        // Tested
        // GET: api/Courses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DtoCourseResponse>> GetCourse(int id)
        {
            var course = await _course.GetCourseById(id);

            if (course == null)
            {
                return NotFound();
            }

            return Ok(course);
        }
        
        // Tested
        // GET: api/Courses
        [HttpGet]
        public async Task<ActionResult<List<DtoCourseResponse>>> GetCourses()
        {
            var courses = await _course.GetAllCourses();
            return Ok(courses);
        }

        // Tested
        // GET: api/Courses/NotStarted
        [HttpGet("NotStarted")]
        public async Task<ActionResult<List<DtoCourseResponse>>> GetCoursesNotStarted()
        {
            var courses = await _course.GetCoursesNotStarted();
            return Ok(courses);
        }

        // GET: api/Courses/Student/5/PreviousCourses
        [HttpGet("Student/{id}/PreviousCourses")]
        public async Task<ActionResult<List<DtoPreviousCourseResponse>>> GetPreviousCoursesOfStudent(int id)
        {
            var courses = await _course.GetPreviousCoursesOfStudent(id);
            return Ok(courses);
        }

        // GET: api/Courses/Student/5/AllCourses
        [HttpGet("Student/{id}/AllCourses")]
        public async Task<ActionResult<List<DtoCourseResponse>>> GetCoursesOfStudent(int id)
        {
            var courses = await _course.GetCoursesOfStudent(id);
            return Ok(courses);
        }

        // GET: api/Courses/Student/5/CurrentCourses
        [HttpGet("Student/{id}/CurrentCourses")]
        public async Task<ActionResult<List<DtoCourseResponse>>> GetCurrentCoursesOfStudent(int id)
        {
            var courses = await _course.GetCurrentCoursesOfStudent(id);
            return Ok(courses);
        }

        // GET: api/Courses/Teacher/5/AllCourses
        [HttpGet("Teacher/{id}/AllCourses")]
        public async Task<ActionResult<List<DtoCourseResponse>>> GetCoursesOfTeacher(int id)
        {
            var courses = await _course.GetCoursesOfTeacher(id);
            return Ok(courses);
        }

        // GET: api/Courses/Teacher/5/CurrentCourses
        [HttpGet("Teacher/{id}/CurrentCourses")]
        public async Task<ActionResult<List<DtoCourseResponse>>> GetCurrentCoursesOfTeacher(int id)
        {
            var courses = await _course.GetCurrentCoursesOfTeacher(id);
            return Ok(courses);
        }

        // POST: api/Courses/5/CalculateAverageGrade
        [HttpPost("{id}/CalculateAverageGrade")]
        public async Task<ActionResult> CalculateAverageGrade(int id)
        {
            await _course.CalculateAverageGrade(id);
            return Ok();
        }
        private bool CourseExists(int id)
        {
            return _course.GetCourseById(id) != null;
        }

        // Tested
        //Delete: api/Courses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _course.GetCourseById(id);
            if (course == null) return NotFound();
            try
            {
                await _course.DeleteCourse(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
