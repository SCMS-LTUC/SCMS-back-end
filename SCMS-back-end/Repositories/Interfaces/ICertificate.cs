using SCMS_back_end.Models;

namespace SCMS_back_end.Repositories.Interfaces
{
    public interface ICertificate
    {
        // Method to post a certificate
        void PostCertificate(Certificate certificate);

        // Method to get a certificate by student ID and course ID
        Certificate GetCertificateByStudentIdAndCourseId(int studentId, int courseId);
    }
}
