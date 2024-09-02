using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto.Request;

namespace SCMS_back_end.Repositories.Interfaces
{
    public interface IDepartment
    {
        Task<IEnumerable<DtoDepartmentRequest>> GetAllDepartmentsAsync();
        Task<DtoDepartmentRequest> GetDepartmentByIdAsync(int id);
        Task AddDepartmentAsync(string DepartmentName);
        Task UpdateDepartmentAsync(DtoDepartmentRequest Department);
        Task DeleteDepartmentAsync(int id);
    }
}
