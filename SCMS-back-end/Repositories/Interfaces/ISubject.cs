using SCMS_back_end.Models;

namespace SCMS_back_end.Repositories.Interfaces
{
    public interface ISubject
    {
        Task<IEnumerable<Subject>> GetAllSubjectsAsync();
        Task<Subject> GetSubjectByIdAsync(int id);
        Task<Subject> AddSubjectAsync(Subject subject);
        Task<Subject> UpdateSubjectAsync(int id, Subject subject);
        Task DeleteSubjectAsync(int id);
        
    }
}
