using Microsoft.EntityFrameworkCore;
using SCMS_back_end.Data;
using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto.Request;
using SCMS_back_end.Repositories.Interfaces;

namespace SCMS_back_end.Repositories.Services
{
    public class DepartmentService : IDepartment
    {
        StudyCenterDbContext _context;
        public DepartmentService(StudyCenterDbContext context)
        {
            _context = context;
        }
        public async Task<DtoDepartment> AddDepartmentAsync(string DepartmentName)
        {
            try
            {
                var department = new Department { Name = DepartmentName };
                await _context.Departments.AddAsync(department).ConfigureAwait(false);
                await _context.SaveChangesAsync().ConfigureAwait(false);
                return new DtoDepartment
                {
                    DepartmentId = department.DepartmentId,
                    DepartmentName = department.Name
                };
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new ApplicationException($"Error adding department: {DepartmentName}", ex);
            }
        }

        private async Task<bool> _IsThereCurrentCoursesInDepartment(int departmentId)
        {
            var result = await _context.Departments.Where(d => d.DepartmentId == departmentId)
                    .SelectMany(d => d.Subjects).SelectMany(s => s.Courses)
                    .Where(c => c.Schedule.EndDate > DateTime.Now).ToListAsync();
            return result.Any();
        }
        public async Task DeleteDepartmentAsync(int id)
        {
            try
            {
                var department = await _context.Departments.FindAsync(id).ConfigureAwait(false);
                
                if (department != null && ! await _IsThereCurrentCoursesInDepartment(department.DepartmentId))
                {
                    _context.Departments.Remove(department);
                    await _context.SaveChangesAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new ApplicationException($"Error deleting department with ID: {id}", ex);
            }
        }

        public async Task<IEnumerable<DtoDepartment>> GetAllDepartmentsAsync()
        {
            try
            {
                var departments = await _context.Departments.ToListAsync().ConfigureAwait(false);
                var departmentsDto = departments.Select(d => new DtoDepartment
                {
                    DepartmentId = d.DepartmentId,
                    DepartmentName = d.Name
                }).ToList();
                return departmentsDto;
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new ApplicationException("Error retrieving all departments", ex);
            }
        }

        public async Task<DtoDepartment> GetDepartmentByIdAsync(int id)
        {
            try
            {
                var department = await _context.Departments.FindAsync(id).ConfigureAwait(false);
                if (department != null)
                {
                    return new DtoDepartment
                    {
                        DepartmentId = department.DepartmentId,
                        DepartmentName = department.Name
                    };
                }
                return null;
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new ApplicationException($"Error retrieving department with ID: {id}", ex);
            }
        }

        public async Task<DtoDepartment> UpdateDepartmentAsync(int id, string DepartmentName)
        {
            try
            {
                var department = await _context.Departments.FindAsync(id).ConfigureAwait(false);
                if (department != null)
                {
                    department.Name = DepartmentName;
                    _context.Departments.Update(department);
                    await _context.SaveChangesAsync().ConfigureAwait(false);
                    return new DtoDepartment
                    {
                        DepartmentId = department.DepartmentId,
                        DepartmentName = department.Name
                    };
                }
                return null;
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new ApplicationException($"Error updating department with ID: {id}", ex);
            }
        }
    }
}
