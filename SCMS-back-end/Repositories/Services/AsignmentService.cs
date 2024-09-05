﻿using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using SCMS_back_end.Data;
using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto.Request.Assignment;
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


        public async Task<DtoLectureAttendanceResponse> AddAssignment(DtoAddAssignmentRequest AssignmentDto)
        {
            var NewAssignment = new Assignment()
            {
               // AssignmentId = AssignmentDto.AssignmentId,
                CourseId = AssignmentDto.CourseId,
                AssignmentName = AssignmentDto.AssignmentName,
                DueDate = AssignmentDto.DueDate,
                Description = AssignmentDto.Description,
                Visible = AssignmentDto.Visible,

            };

            _context.Assignments.Add(NewAssignment);
            await _context.SaveChangesAsync();

            var Response = new DtoLectureAttendanceResponse()
            {
                AssignmentName = AssignmentDto.AssignmentName,
                DueDate = AssignmentDto.DueDate,
                Description = AssignmentDto.Description,
                Visible = AssignmentDto.Visible,
            };

            return Response;
        }

        public async Task<List<DtoLectureAttendanceResponse>> GetAllAssignmentsByCourseID(int CourseID)
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
            var assignmentDtos = allAssignments.Select(a => new DtoLectureAttendanceResponse
            {
                //AssignmentId = a.AssignmentId,
                AssignmentName = a.AssignmentName,
                DueDate = a.DueDate,
                Description = a.Description
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

            Assignment.AssignmentName = AssignmentDto.AssignmentName;
            Assignment.DueDate = AssignmentDto.DueDate;
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



        public async Task<DtoLectureAttendanceResponse> GetAllAssignmentInfoByAssignmentID(int AssignmentID)
        {
            var Assignment = await _context.Assignments.FirstOrDefaultAsync(x => x.AssignmentId == AssignmentID);

            if (Assignment == null)
            {
                throw new KeyNotFoundException($"Song with ID {AssignmentID} not found.");
            }
            var AssignmentDto = new DtoLectureAttendanceResponse()
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

        public async Task<List<DtoGetAllStudentAssignmentsRequest>> GetAllStudentAssignments(int CourseID)
        {
            //, int AssignmentID, int CourseID
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

            return assignmentDtos;

        }

        /* public async Task<List<DtoGetAllStudentRquest>> GetAllStudentAssignmentByCourseID(int CourseID)
         {
           var Student=await _context.Courses.Where(x=>x.CourseId==CourseID)
                 .SelectMany(x=>x.StudentCourses)
                .Include(x=>x.Student)
                 //.Include(x=>x.StudentId)
                 .Select(x => x.Student)
                 //.Include(x=>x.StudentAssignments)
                 .ToListAsync();

             if (Student.Count <= 0)
             {
                 throw new ArgumentException("Invalid Course ID", nameof(CourseID));
             }

             var assignmentDtos = Student.Select(a => new DtoGetAllStudentRquest()
             {
                 FullName = a.FullName,

                 StudentAssignments = a.StudentAssignments.Select(sa => new DtoStudentAssignmentResponse
                 {
                     StudentAssignmentId = sa.StudentAssignmentId,
                     Feedback = sa.Feedback,
                     Grade = sa.Grade
                 }).ToList()

             }).ToList();

             return assignmentDtos;
         }*/
        
        public async Task<List<DtoGetAllStudentRquest>> GetAllStudentAssignmentByCourseID(int CourseID)
        {
           
            var students = await _context.StudentCourses
                .Where(sc => sc.CourseId == CourseID)
                .Include(sc => sc.Student) 
                    .ThenInclude(s => s.StudentAssignments) 
                .Select(sc => sc.Student) 
                .ToListAsync();

            if (students.Count <= 0)
            {
                throw new ArgumentException("Invalid Course ID", nameof(CourseID));
            }

           
            var studentDtos = students.Select(s => new DtoGetAllStudentRquest
            {
                FullName = s.FullName,
                StudentAssignments = s.StudentAssignments.Select(sa => new DtoStudentAssignmentResponse
                {
                    StudentAssignmentId = sa.StudentAssignmentId,
                       
                    Feedback = sa.Feedback,
                    Grade = sa.Grade
                }).ToList()
            }).ToList();

            return studentDtos;
        }



    }
}
