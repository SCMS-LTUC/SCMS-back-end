using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SCMS_back_end.Models;
using SCMS_back_end.Repositories.Interfaces;
using SCMS_back_end.Models.Dto.Request;

namespace SCMS_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartment _departmentService;
        public DepartmentController(IDepartment context)
        {
            _departmentService = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Department>>> GetAllDepartments()
        {
            var songs = await _departmentService.GetAllDepartmentsAsync();
            return Ok(songs);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Department>> GetDepartment(int id)
        {
            return Ok(await _departmentService.GetDepartmentByIdAsync(id));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Department>> PutDepartment(int id, string DepartmentName)
        {
            return Ok(await _departmentService.UpdateDepartmentAsync(id, DepartmentName));
        }

        [HttpPost]
        public async Task<ActionResult<User>> PostDepartment(string DepartmentName)
        {
            var department = await _departmentService.AddDepartmentAsync(DepartmentName);
            return Ok(department);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            await _departmentService.DeleteDepartmentAsync(id);
            return Ok();
        }


    }
}
