using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SCMS_back_end.Data;
using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto.Request;
namespace SCMS_back_end.Services
{
    public class StudentAssignmentsService : IStudentAssignments
    {
        private readonly StudyCenterDbContext _context;

        public StudentAssignmentsService(StudyCenterDbContext context)
        {
            _context = context;
        }

        public async Task<StudentAssignment> AddOrUpdateStudentAssignmentAsync(StudentAssignmentDtoRequest studentAssignmentDto)
        {
            // Validate if the AssignmentId exists
            var assignmentExists = await _context.Assignments
                .AnyAsync(a => a.AssignmentId == studentAssignmentDto.AssignmentId);

            if (!assignmentExists)
            {
                throw new Exception("Assignment not found. Please provide a valid AssignmentId.");
            }

            var existingRecord = await _context.StudentAssignments
                .FirstOrDefaultAsync(sa => sa.StudentId == studentAssignmentDto.StudentId && sa.AssignmentId == studentAssignmentDto.AssignmentId);

            if (existingRecord != null)
            {
                // Update existing record based on which fields are provided
                if (!string.IsNullOrEmpty(studentAssignmentDto.Submission))
                {
                    existingRecord.Submission = studentAssignmentDto.Submission;
                    existingRecord.SubmissionDate = studentAssignmentDto.SubmissionDate ?? DateTime.Now;
                }

                if (studentAssignmentDto.Grade.HasValue)
                {
                    existingRecord.Grade = studentAssignmentDto.Grade.Value;
                    existingRecord.Feedback = studentAssignmentDto.Feedback;
                }

                _context.StudentAssignments.Update(existingRecord);
            }
            else
            {
                // Add new record
                var newAssignment = new StudentAssignment
                {
                    AssignmentId = studentAssignmentDto.AssignmentId,
                    StudentId = studentAssignmentDto.StudentId,
                    SubmissionDate = studentAssignmentDto.SubmissionDate ?? DateTime.Now,
                    Submission = studentAssignmentDto.Submission,
                    Grade = studentAssignmentDto.Grade ?? 0,
                    Feedback = studentAssignmentDto.Feedback
                };

                await _context.StudentAssignments.AddAsync(newAssignment);
                existingRecord = newAssignment;
            }

            await _context.SaveChangesAsync();
            return existingRecord;
        }


        public async Task<StudentAssignment> UpdateStudentAssignmentAsync(int studentAssignmentId, int? grade, string feedback)
        {
            var studentAssignment = await _context.StudentAssignments.FindAsync(studentAssignmentId);

            if (studentAssignment != null)
            {
                if (grade.HasValue)
                    studentAssignment.Grade = grade.Value;

                if (!string.IsNullOrEmpty(feedback))
                    studentAssignment.Feedback = feedback;

                _context.StudentAssignments.Update(studentAssignment);
                await _context.SaveChangesAsync();
            }

            return studentAssignment;
        }

        public async Task<StudentAssignment> GetStudentAssignmentByIdAsync(int studentAssignmentId)
        {
            return await _context.StudentAssignments
                .Include(sa => sa.Assignment)
                .Include(sa => sa.Student)
                .FirstOrDefaultAsync(sa => sa.StudentAssignmentId == studentAssignmentId);
        }

        public async Task<StudentAssignment> GetStudentAssignmentByAssignmentAndStudentAsync(int assignmentId, int studentId)
        {
            return await _context.StudentAssignments
                .Include(sa => sa.Assignment)
                .Include(sa => sa.Student)
                .FirstOrDefaultAsync(sa => sa.AssignmentId == assignmentId && sa.StudentId == studentId);
        }
    }
}
