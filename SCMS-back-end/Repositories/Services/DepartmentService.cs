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
            var department= new Department{Name = DepartmentName};
           await _context.Departments.AddAsync(department);
            await _context.SaveChangesAsync();
            return new DtoDepartment
            {
                DepartmentId = department.DepartmentId,
                DepartmentName = department.Name
            };
        }

        public async Task DeleteDepartmentAsync(int id)
        {
            var department= await _context.Departments.FindAsync(id);
            if (department != null)
            {
                 _context.Departments.Remove(department);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<DtoDepartment>> GetAllDepartmentsAsync()
        {
            var departments= await _context.Departments.ToListAsync();
            List<DtoDepartment> departmentsDto= new List<DtoDepartment>();
            foreach (var d in departments)
            {
                departmentsDto.Add(
                    new DtoDepartment
                    {
                        DepartmentId= d.DepartmentId,
                        DepartmentName=d.Name
                    }
                    );
            }
            return departmentsDto;
        }

        public async Task<DtoDepartment> GetDepartmentByIdAsync(int id)
        {
            var department = await _context.Departments.FindAsync(id);
           if(department != null)
           {
                return new DtoDepartment
                {
                    DepartmentId = department.DepartmentId,
                    DepartmentName = department.Name
                };
           }
            return null;
        }

        public async Task<DtoDepartment> UpdateDepartmentAsync(int id, string DepartmentName)
        {
            var department = await _context.Departments.FindAsync(id);
            if(department != null)
            {
                department.Name = DepartmentName;
                _context.Departments.Update(department);
                await _context.SaveChangesAsync();
                return new DtoDepartment
                {
                    DepartmentId = department.DepartmentId,
                    DepartmentName = department.Name
                };
            }
            return null;
        }
    }
}
