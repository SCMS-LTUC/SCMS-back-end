using SCMS_back_end.Models.Dto.Response;

namespace SCMS_back_end.Repositories.Interfaces
{
    public interface IReport
    {
        Task<StudentEnrollmentOverview> GetStudentEnrollmentOverview();
        Task<CoursePerformanceOverview> GetCoursePerformanceOverview();
        Task<InstructorEffectivenessOverview> GetInstructorEffectivenessOverview();
        Task GetAssignmentAndAttendanceOverview();
        Task<DepartmentalActivityOverview> GetDepartmentalActivityOverview();
        Task<SystemHealthCheckOverview> GetSystemHealthCheckOverview();

    }
}
