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


        public async Task<DtoAddAssignmentResponse> AddAssignment(DtoAddAssignmentRequest AssignmentDto)
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



        public async Task<DtoAddAssignmentResponse> GetAssignmentInfoByID(int AssignmentID)
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


    }
}
