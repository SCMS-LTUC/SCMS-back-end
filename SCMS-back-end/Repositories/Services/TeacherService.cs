using Microsoft.EntityFrameworkCore;
using SCMS_back_end.Data;
using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto.Request.Assignment;
using SCMS_back_end.Models.Dto.Request.Teacher;
using SCMS_back_end.Models.Dto.Response.Teacher;
using SCMS_back_end.Repositories.Interfaces;

namespace SCMS_back_end.Repositories.Services
{
    public class TeacherService:ITeacher
    {
        private readonly StudyCenterDbContext _context;

        public TeacherService(StudyCenterDbContext context)
        {
            _context = context;
        }

        public async Task<DtoAddTeacherResponse> AddTeacher(DtoAddTeacherRequest Teacher)
        {
            var NewTeacher = new Teacher()
            {
                FullName = Teacher.FullName,
                CourseLoad = Teacher.CourseLoad,
                PhoneNumber = Teacher.PhoneNumber,
                //DepartmentId = Teacher.DepartmentId,
                UserId = Teacher.UserId

            };
            _context.Teachers.Add(NewTeacher);
            await _context.SaveChangesAsync();

            var Response = new DtoAddTeacherResponse()
            {
                FullName = Teacher.FullName,
                CourseLoad = Teacher.CourseLoad,
                PhoneNumber = Teacher.PhoneNumber,
            };

            return Response;
        }

        public async Task<List<DtoGetAllTeacherRequest>> GetAllTeachers()
        {
            var Teachers =await _context.Teachers.ToListAsync();
            var dtoTeachers = Teachers.Select(t => new DtoGetAllTeacherRequest
            {
                TeacherId = t.TeacherId,
                FullName = t.FullName,
                PhoneNumber = t.PhoneNumber,
                CourseLoad=t.CourseLoad,
               // DepartmentId=t.DepartmentId,
                //UserId = t.UserId   
                
            }).ToList();
            return dtoTeachers;
        }

        public async Task<DtoGetTeacherInfoByIDRequest> GetTeacherInfoByID(int TeacherID)
        {
            var Teacher = await _context.Teachers.FirstOrDefaultAsync(x => x.TeacherId == TeacherID);

            if (Teacher == null)
            {
                throw new KeyNotFoundException($"Teacher with ID {TeacherID} not found.");
            }
            var TeacherDto = new DtoGetTeacherInfoByIDRequest()
            {
                FullName =Teacher.FullName,
                CourseLoad=Teacher.CourseLoad,
                PhoneNumber = Teacher.PhoneNumber,
                DepartmentId = Teacher.DepartmentId,
            };
            return TeacherDto;
        }

        public async Task<DtoUpdateTeacherInfoByAdminRequest> UpdateTeacherInfoByID(int TeacherID, DtoUpdateTeacherInfoByAdminRequest Teacher)
        {
            var TeacherToUpdate = await _context.Teachers.FirstOrDefaultAsync(x=>x.TeacherId == TeacherID);

            if (TeacherToUpdate == null)
            {
                throw new ArgumentException("Invalid Assignment ID", nameof(TeacherID));
            }

            TeacherToUpdate.PhoneNumber = Teacher.PhoneNumber;
            TeacherToUpdate.CourseLoad = Teacher.CourseLoad;
          

            return Teacher;

        }


    }
}
