using SCMS_back_end.Models;
using SCMS_back_end.Data;
using SCMS_back_end.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SCMS_back_end.Repositories.Services
{
    public class IdentitySubjectServices : ISubject
    {
        private readonly StudyCenterDbContext _context;
        private readonly object updatedSubject;
        private object UpdateSubject;

        public IdentitySubjectServices(StudyCenterDbContext context)
        {
            _context = context;
        }
        public async Task<Subject> AddSubjectAsync(Subject subject)
        {
            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync();
            return subject;
        }

        public Task DeleteSubjectAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Subject>> GetAllSubjectsAsync()
        {
            return await _context.Subjects.ToListAsync();
        }

        public async Task<Subject> GetSubjectByIdAsync(int id)
        {
            return await _context.Subjects.FindAsync(id);
            
        }

        public async Task<Subject> UpdateSubjectAsync(int id, Subject subject)
        {
            var exitingSubject = await _context.Subjects.FindAsync(id);
            if(exitingSubject != null)
            {
                exitingSubject.Name = subject.Name;
                exitingSubject.DepartmentId = subject.DepartmentId;
                await _context.SaveChangesAsync();
                return exitingSubject;
            }
            return null; 
        }
    }
}
