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
        public async Task AddDepartmentAsync(string DepartmentName)
        {
            var department= new Department{Name = DepartmentName};
           await _context.Departments.AddAsync(department);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDepartmentAsync(int id)
        {
            var department= _context.Departments.FindAsync(id);
            if (department != null)
            {
                //await _context.Departments.Remove(department);
            }
        }

        public async Task<IEnumerable<DtoDepartmentRequest>> GetAllDepartmentsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<DtoDepartmentRequest> GetDepartmentByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateDepartmentAsync(DtoDepartmentRequest Department)
        {
            throw new NotImplementedException();
        }
    }
}
