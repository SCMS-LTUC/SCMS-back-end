using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SCMS_back_end.Data;
using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto.Request;
using SCMS_back_end.Models.Dto.Response;
namespace SCMS_back_end.Services
{
    public class StudentAssignmentsService : IStudentAssignments
    {
        private readonly StudyCenterDbContext _context;

        public StudentAssignmentsService(StudyCenterDbContext context)
        {
            _context = context;
        }

        // Student Submission Method
        public async Task<StudentAssignment> AddStudentAssignmentSubmissionAsync(StudentAssignmentSubmissionDtoRequest dto)
        {
            var assignmentExists = await _context.Assignments
                .AnyAsync(a => a.AssignmentId == dto.AssignmentId);

            if (!assignmentExists)
            {
                throw new Exception("Assignment not found. Please provide a valid AssignmentId.");
            }

            var existingRecord = await _context.StudentAssignments
                .FirstOrDefaultAsync(sa => sa.StudentId == dto.StudentId && sa.AssignmentId == dto.AssignmentId);

            if (existingRecord != null && !string.IsNullOrEmpty(existingRecord.Submission))
            {
                throw new Exception("Submission already exists. You cannot submit more than once.");
            }

            // Handle file upload
            string filePath = null;
            if (dto.File != null)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                filePath = Path.Combine(uploadsFolder, dto.File.FileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.File.CopyToAsync(fileStream);
                }
            }

            if (existingRecord != null)
            {
                existingRecord.Submission = dto.Submission;
                existingRecord.SubmissionDate = DateTime.Now;
                if (!string.IsNullOrEmpty(filePath))
                {
                    existingRecord.FilePath = filePath; // Store file path
                }
                _context.StudentAssignments.Update(existingRecord);
            }
            else
            {
                var newAssignment = new StudentAssignment
                {
                    AssignmentId = dto.AssignmentId,
                    StudentId = dto.StudentId,
                    SubmissionDate = DateTime.Now,
                    Submission = dto.Submission,
                    FilePath = filePath // Store file path
                };

                await _context.StudentAssignments.AddAsync(newAssignment);
                existingRecord = newAssignment;
            }

            await _context.SaveChangesAsync();
            return existingRecord;
        }
        // Teacher Feedback Method
        public async Task<StudentAssignment> AddStudentAssignmentFeedbackAsync(TeacherAssignmentFeedbackDtoRequest dto)
        {
            var studentAssignment = await _context.StudentAssignments.FindAsync(dto.StudentAssignmentId);

            if (studentAssignment == null)
            {
                throw new Exception("Student assignment not found.");
            }

            if (dto.Grade.HasValue)
            {
                studentAssignment.Grade = dto.Grade.Value;
            }

            if (!string.IsNullOrEmpty(dto.Feedback))
            {
                studentAssignment.Feedback = dto.Feedback;
            }

            _context.StudentAssignments.Update(studentAssignment);
            await _context.SaveChangesAsync();

            return studentAssignment;
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

        public async Task<StudentAssignmentDtoResponse> GetStudentAssignmentByIdAsync(int studentAssignmentId)
        {
            var studentAssignment = await _context.StudentAssignments
                .Include(sa => sa.Assignment)
                .Include(sa => sa.Student)
                .FirstOrDefaultAsync(sa => sa.StudentAssignmentId == studentAssignmentId);

            if (studentAssignment == null)
            {
                return null;
            }

            var responseDto = new StudentAssignmentDtoResponse
            {
                StudentAssignmentId = studentAssignment.StudentAssignmentId,
                AssignmentId = studentAssignment.AssignmentId,
                StudentId = studentAssignment.StudentId,
                SubmissionDate = studentAssignment.SubmissionDate, // No error now
                Submission = studentAssignment.Submission,
                Grade = studentAssignment.Grade,
                Feedback = studentAssignment.Feedback,
                FilePath = studentAssignment.FilePath
            };

            return responseDto;
        }

        public async Task<StudentAssignmentDtoResponse> GetStudentAssignmentByAssignmentAndStudentAsync(int assignmentId, int studentId)
        {
            var studentAssignment = await _context.StudentAssignments
                .Include(sa => sa.Assignment)
                .Include(sa => sa.Student)
                .FirstOrDefaultAsync(sa => sa.AssignmentId == assignmentId && sa.StudentId == studentId);

            if (studentAssignment == null)
            {
                return null;
            }

            var responseDto = new StudentAssignmentDtoResponse
            {
                StudentAssignmentId = studentAssignment.StudentAssignmentId,
                AssignmentId = studentAssignment.AssignmentId,
                StudentId = studentAssignment.StudentId,
                SubmissionDate = studentAssignment.SubmissionDate, // No error now
                Submission = studentAssignment.Submission,
                Grade = studentAssignment.Grade,
                Feedback = studentAssignment.Feedback,
                FilePath = studentAssignment.FilePath
            };

            return responseDto;
        }
    }
}
    