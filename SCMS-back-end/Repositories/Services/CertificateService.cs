using Microsoft.EntityFrameworkCore;
using SCMS_back_end.Data;
using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto.Request;
using SCMS_back_end.Repositories.Interfaces;
using System.IO;

namespace SCMS_back_end.Repositories.Services
{
    public class CertificateService : ICertificate
    {
        private readonly StudyCenterDbContext _context;
        public CertificateService(StudyCenterDbContext context)
        {
            _context = context;
        }
        public async Task<Certificate>  GetCertificateByStudentIdAndCourseId(int studentId, int courseId)
        {

            return await _context.Certificates.FirstOrDefaultAsync(c => c.StudentId == studentId && c.CourseId == courseId);
        }

        public async Task PostCertificate(DtoCertificateRequest dto)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Certificates");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var filePath = Path.Combine(uploadsFolder, dto.CertificateFile.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.CertificateFile.CopyToAsync(stream);
            }

            var newCertificate = new Certificate
            {
                StudentId = dto.StudentId,
                CourseId = dto.CourseId,
                CompletionDate = dto.CompletionDate,
                CertificatePath = filePath
            };

            _context.Certificates.Add(newCertificate);
            await _context.SaveChangesAsync();
        }
    }
}
