using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto.Request;

namespace SCMS_back_end.Repositories.Interfaces
{
    public interface ICertificate
    {
        // Method to post a certificate
        Task PostCertificate(DtoCertificateRequest dto);

        // Method to get a certificate by student ID and course ID
        public Task<Certificate> GetCertificateByStudentIdAndCourseId(int studentId, int courseId);
    }
}
