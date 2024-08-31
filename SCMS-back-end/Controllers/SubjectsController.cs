using Microsoft.AspNetCore.Mvc;
using SCMS_back_end.Models;
using SCMS_back_end.Repositories.Interfaces;

namespace SCMS_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectController : ControllerBase
    {
        private readonly ISubject _subject;

        public SubjectController(ISubject subject)
        {
            _subject = subject;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Subject>>> GetAllSubjects()
        {
            var subjects = await _subject.GetAllSubjectsAsync();
            return Ok(subjects);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Subject>> GetSubjectById(int id)
        {
            var subject = await _subject.GetSubjectByIdAsync(id);
            if (subject == null)
            {
                return NotFound();
            }
            return Ok(subject);
        }

        [HttpPost]
        public async Task<ActionResult<Subject>> AddSubject(Subject subject)
        {
            var createdSubject = await _subject.AddSubjectAsync(subject);
            return CreatedAtAction(nameof(GetSubjectById), new { id = createdSubject.SubjectId }, createdSubject);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSubject(int id, Subject subject)
        {
            if (id != subject.SubjectId)
            {
                return BadRequest();
            }

            var updatedSubject = await _subject.UpdateSubjectAsync(id, subject);
            if (updatedSubject == null)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            await _subject.DeleteSubjectAsync(id);
            return NoContent();
        }
    }
}
