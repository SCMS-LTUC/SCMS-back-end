using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using SCMS_back_end.Data;
using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto.Request.Assignment;
using SCMS_back_end.Models.Dto.Request.Teacher;
using SCMS_back_end.Models.Dto.Response.Assignment;
using SCMS_back_end.Repositories.Interfaces;
using System.Security.Claims;

namespace SCMS_back_end.Repositories.Services
{
    public class AssignmentService : IAssignment
    {
        private readonly StudyCenterDbContext _context;
        private UserManager<User> _userManager;

        public AssignmentService(StudyCenterDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        public async Task<DtoAddAssignmentResponse> AddAssignment(DtoAddAssignmentRequest AssignmentDto)
        {
            var NewAssignment = new Assignment()
            {
               // assignmentId = AssignmentDto.assignmentId,
                CourseId = AssignmentDto.CourseId,
                AssignmentName = AssignmentDto.AssignmentName,
                DueDate = AssignmentDto.DueDate,
                Description = AssignmentDto.Description,
                Visible = AssignmentDto.Visible,

            };

            _context.Assignments.Add(NewAssignment);
            await _context.SaveChangesAsync();

            var Response = new DtoAddAssignmentResponse()
            {
                AssignmentName = AssignmentDto.AssignmentName,
                DueDate = AssignmentDto.DueDate,
                Description = AssignmentDto.Description,
                Visible = AssignmentDto.Visible,
            };

            return Response;
        }

        public async Task<List<DtoAddAssignmentResponse>> GetAllAssignmentsByCourseID(int CourseID)
        {
            var allAssignments = await _context.Courses
             .Where(x => x.CourseId == CourseID)
             .SelectMany(x => x.Assignments)
             .ToListAsync();


            //if(allAssignments.Count<=0)
            //{
            //   throw new ArgumentException("Invalid Course ID", nameof(CourseID));
            //}

            // Map assignmentsWithStudentAssignment to DTOs
            var assignmentDtos = allAssignments.Select(a => new DtoAddAssignmentResponse
            {
                //assignmentId = s.assignmentId,
                AssignmentName = a.AssignmentName,
                DueDate = a.DueDate,
                Description = a.Description,
                Visible = a.Visible
            }).ToList();

            return assignmentDtos;

        }

        public async Task<DtoUpdateAssignmentResponse> UpdateAssignmentByID(int AssignmentID, DtoUpdateAssignmentRequest AssignmentDto)
        {
            var Assignment = await _context.Assignments.FirstOrDefaultAsync(x => x.AssignmentId == AssignmentID);

            if (Assignment == null)
            {
                throw new ArgumentException("Invalid Assignment ID", nameof(AssignmentID));
            }

            if (string.IsNullOrEmpty(AssignmentDto.AssignmentName))
                Assignment.AssignmentName = AssignmentDto.AssignmentName;

            if (AssignmentDto.DueDate != Convert.ToDateTime("01/01/0001 00:00:00"))
                Assignment.DueDate = AssignmentDto.DueDate;

            if (string.IsNullOrEmpty(AssignmentDto.Description))
                Assignment.Description = AssignmentDto.Description;

            Assignment.Visible = AssignmentDto.Visible;

            await _context.SaveChangesAsync();

            var Response = new DtoUpdateAssignmentResponse()
            {
                AssignmentName = Assignment.AssignmentName,
                DueDate = AssignmentDto.DueDate,
                Description = AssignmentDto.Description,
                Visible = Assignment.Visible
            };
            return Response;
        }

        public async Task<DtoAddAssignmentResponse> GetAllAssignmentInfoByAssignmentID(int AssignmentID)
        {
            var Assignment = await _context.Assignments.FirstOrDefaultAsync(x => x.AssignmentId == AssignmentID);

            if (Assignment == null)
            {
                throw new KeyNotFoundException($"Song with ID {AssignmentID} not found.");
            }
            var AssignmentDto = new DtoAddAssignmentResponse()
            {
                AssignmentName = Assignment.AssignmentName,
                DueDate = Assignment.DueDate,
                Description = Assignment.Description,
                Visible = Assignment.Visible,
            };
            return AssignmentDto;

        }
        public async Task DeleteAssignment(int AssignmentID)
        {
            var AssignmentToDelete =await _context.Assignments
                .FirstOrDefaultAsync(x => x.AssignmentId == AssignmentID);

            if(AssignmentToDelete==null)
            {
                throw new Exception("Assignment not found.");
            }

            _context.Assignments.Remove(AssignmentToDelete);
            await _context.SaveChangesAsync();

            var Response = new DtoDeleteAssignmentResponse()
            {
                AssignmentName = AssignmentToDelete.AssignmentName,
                DueDate = AssignmentToDelete.DueDate,
                Description = AssignmentToDelete.Description,

            };
           
        }

        public async Task<List<DtoStudentAssignmentResponse>> GetStudentAssignmentsByCourseId(int courseId, ClaimsPrincipal userPrincipal)
        {

            // get all assignmentsWithStudentAssignment in s course 
            // get the student assignment record for each assignment 
            var user = await _userManager.GetUserAsync(userPrincipal);
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == user.Id);
            if (student == null)
            {
                throw new InvalidOperationException("Student not found.");
            }
            var assignmentsWithStudentAssignment = await _context.Assignments
                .Include(a => a.StudentAssignments)
                .Where(a => a.CourseId == courseId)
                .Select(a => new DtoStudentAssignmentResponse {
                    AssignmentId = a.AssignmentId,
                    AssignmentName = a.AssignmentName,
                    DueDate = a.DueDate,
                    StudentAssignment = a.StudentAssignments
                .Where(sa => sa.AssignmentId == a.AssignmentId && sa.StudentId == student.StudentId)
                .Select(sa => new DtoStudentAssignmentDetails
                {
                    StudentAssignmentId = sa.StudentAssignmentId,
                    Grade = sa.Grade,
                    Feedback = sa.Feedback,
                    SubmissionDate = sa.SubmissionDate
                }).FirstOrDefault()
                })
                .ToListAsync();
            
            return assignmentsWithStudentAssignment;
        }
        
        public async Task<List<DtoStudentSubmissionResponse>> GetAllStudentsSubmissionByAssignmentId(int assignmentId)
        {
            //get all studentsWithStudentAssignment with the student assignment record for each student 

            //get studentsWithStudentAssignment in s course 
            //get student assignment reocrd for each student by student id and assignment id 
            /* var assignment= _context.Assignments.FirstOrDefault(a => a.AssignmentId == assignmentId);
             if(assignment == null)
             {
                 throw new InvalidOperationException("Assignment not found.");
             }

             var studentsWithStudentAssignment = await _context.Students
                 .Include(s => s.StudentAssignments)
                 .Where()

             var allStudents = new List<DtoStudentSubmissionResponse>();
             foreach (var s in studentsWithStudentAssignment)
             {
                 allStudents.Add(new DtoStudentSubmissionResponse
                 { 
                     StudentId= s.StudentId,
                     FullName = s.FullName,
                     StudentAssignment= await _GetStudentAssignment(s.StudentId, assignmentId)
                 });
             }
             return allStudents;*/
            return null;
        }
        private async Task<DtoStudentAssignmentDetails> _GetStudentAssignment(int studentId, int assignmentId)
        {
            var studentAssignment = await _context.StudentAssignments
                .FirstOrDefaultAsync(sc => sc.AssignmentId == assignmentId && sc.StudentId == studentId);
            if (studentAssignment != null)
                return new DtoStudentAssignmentDetails
                {
                    StudentAssignmentId = studentAssignment.StudentAssignmentId,
                    SubmissionDate = studentAssignment.SubmissionDate,
                    Grade = studentAssignment.Grade,
                    Feedback = studentAssignment.Feedback
                };
            return null;
        }

    }
}
