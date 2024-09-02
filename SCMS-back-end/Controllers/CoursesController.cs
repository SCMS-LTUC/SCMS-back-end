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

        // GET: api/Courses
        [HttpGet]
        public async Task<ActionResult<List<DtoCourseResponse>>> GetCourses()
        {
            var courses = await _course.GetAllCourses();
            return Ok(courses);
        }

        // GET: api/Courses/NotStarted
        [HttpGet("NotStarted")]
        public async Task<ActionResult<List<DtoCourseResponse>>> GetCoursesNotStarted()
        {
            var courses = await _course.GetCoursesNotStarted();
            return Ok(courses);
        }

        // GET: api/Courses/5/PreviousCourses
        [HttpGet("{id}/PreviousCourses")]
        public async Task<ActionResult<List<DtoPreviousCourseResponse>>> GetPreviousCoursesOfStudent(int id)
        {
            var courses = await _course.GetPreviousCoursesOfStudent(id);
            return Ok(courses);
        }

        private bool CourseExists(int id)
        {
            return _course.GetCourseById(id) != null;
        }
    }
}
