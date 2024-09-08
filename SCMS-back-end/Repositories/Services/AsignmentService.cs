using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using SCMS_back_end.Data;
using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto.Request.Assignment;
using SCMS_back_end.Models.Dto.Request.Teacher;
using SCMS_back_end.Models.Dto.Response.Assignment;
using SCMS_back_end.Repositories.Interfaces;

namespace SCMS_back_end.Repositories.Services
{
    public class AsignmentService : IAssignment
    {
        private readonly StudyCenterDbContext _context;

        public AsignmentService(StudyCenterDbContext context)
        {
            _context = context;
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

            // Map assignments to DTOs
            var assignmentDtos = allAssignments.Select(a => new DtoAddAssignmentResponse
            {
                //assignmentId = a.assignmentId,
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
                CourseId = Assignment.CourseId,
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

        public async Task<List<DtoGetAllStudentAssignmentsRequest>> GetStudentAssignmentsByCourseId(int courseId, int studentId)
        {
            /*//, int AssignmentID, int CourseID
            var Assignment = await _context.Courses.Where(x => x.CourseId == CourseID)
                .SelectMany(x => x.Assignments)
                .Include(x=>x.StudentAssignments)
                .ToListAsync();

            if (Assignment.Count <= 0)
            {
                throw new ArgumentException("Invalid Course ID", nameof(CourseID));
            }

            var assignmentDtos = Assignment.Select(a => new DtoGetAllStudentAssignmentsRequest
            {
                AssignmentId = a.AssignmentId,
                AssignmentName = a.AssignmentName,
                DueDate = a.DueDate,
                StudentAssignments = a.StudentAssignments.Select(sa => new DtoStudentAssignmentResponse
                {
                    StudentAssignmentId = sa.StudentAssignmentId,
                    Feedback = sa.Feedback,
                    Grade = sa.Grade
                }).ToList()

            }).ToList();

            return assignmentDtos;*/

            // get all assignments in a course 
            // get the student assignment record for each assignment 
            var assignments= await _context.Assignments
                .Where(a => a.CourseId==courseId)
                .ToListAsync();

            var allAssignments = new List<DtoGetAllStudentAssignmentsRequest>();
            foreach (var a in assignments)
            {
                allAssignments.Add(new DtoGetAllStudentAssignmentsRequest
                {
                    AssignmentId = a.AssignmentId,
                    AssignmentName = a.AssignmentName,
                    DueDate= a.DueDate,
                    StudentAssignment = await _GetStudentAssignment(studentId, a.AssignmentId)
                });
            }
            return allAssignments;
        }

        /* public async Task<List<DtoGetAllStudentRquest>> GetAllStudentsSubmissionByAssignmentId(int CourseID)
         {
           var Student=await _context.Courses.Where(x=>x.CourseId==CourseID)
                 .SelectMany(x=>x.StudentCourses)
                .Include(x=>x.Student)
                 //.Include(x=>x.StudentId)
                 .Select(x => x.Student)
                 //.Include(x=>x.StudentAssignment)
                 .ToListAsync();

             if (Student.Count <= 0)
             {
                 throw new ArgumentException("Invalid Course ID", nameof(CourseID));
             }

             var assignmentDtos = Student.Select(a => new DtoGetAllStudentRquest()
             {
                 FullName = a.FullName,

                 StudentAssignment = a.StudentAssignment.Select(sa => new DtoStudentAssignmentResponse
                 {
                     StudentAssignmentId = sa.StudentAssignmentId,
                     Feedback = sa.Feedback,
                     Grade = sa.Grade
                 }).ToList()

             }).ToList();

             return assignmentDtos;
         }*/
        
        public async Task<List<DtoGetAllStudentRquest>> GetAllStudentsSubmissionByAssignmentId(int assignmentId)
        {
            //get all students with the student assignment record for each student 
            /*
            //var assignmentId= CourseID;
            //by assignment Id

            //var students = await _context.StudentCourses
            //    .Where(sc => sc.CourseId == CourseID)
            //    .Include(sc => sc.Student) 
            //    .ThenInclude(s => s.StudentAssignment) 
            //    .Select(sc => sc.Student) 
            //    .ToListAsync();

            //if (students.Count <= 0)
            //{
            //    throw new ArgumentException("Invalid Course ID", nameof(CourseID));
            //}

            //var studentDtos = students.Select(s => new DtoGetAllStudentRquest
            //{
            //    StudentId= s.StudentId,
            //    FullName = s.FullName,
            //    StudentAssignment = new DtoStudentAssignmentResponse
            //    {
            //        StudentAssignmentId = s.StudentAssignment,
            //        SubmissionDate = sa.SubmissionDate,   
            //        Feedback = sa.Feedback,
            //        Grade = sa.Grade
            //    }
            //}).ToList();

            var course = _context.Courses
                .Include(c => c.Assignments)
                .Include(c => c.StudentCourses)
                .FirstOrDefaultAsync(c => c.Assignments.Any(a => a.AssignmentId == assignmentId));


            //var students= await _context.Students
            //    .Include(x=>x.StudentAssignment)
            //    .where

            // return studentDtos;*/

            //get students in acourse 
            //get student assignment reocrd for each student by student id and assignment id 

            var students = await _context.StudentCourses
                .Include(sc => sc.Course)
                .ThenInclude(c => c.Assignments)
                .Where(sc => sc.Course.Assignments.Any(a => a.AssignmentId == assignmentId))
                .Select(sc => sc.Student).ToListAsync();

            var allStudents = new List<DtoGetAllStudentRquest>();
            foreach (var s in students)
            {
                allStudents.Add(new DtoGetAllStudentRquest
                { 
                    StudentId= s.StudentId,
                    FullName = s.FullName,
                    StudentAssignment= await _GetStudentAssignment(s.StudentId, assignmentId)
                });
            }
            return allStudents;
        }
        private async Task<DtoStudentAssignmentResponse> _GetStudentAssignment(int studentId, int assignmentId)
        {
            var studentAssignment = await _context.StudentAssignments
                .FirstOrDefaultAsync(sc => sc.AssignmentId == assignmentId && sc.StudentId == studentId);
            if (studentAssignment != null)
                return new DtoStudentAssignmentResponse
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
