using SCMS_back_end.Models;
using SCMS_back_end.Repositories.Interfaces;

namespace SCMS_back_end.Repositories.Services
{
    public class CertificateService : ICertificate
    {
        public async Certificate GetCertificateByStudentIdAndCourseId(int studentId, int courseId)
        {
        }

        public async void PostCertificate(Certificate certificate)
        {
        }
    }
}
