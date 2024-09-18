namespace SCMS_back_end.Models.Dto.Request
{
    public class DtoCertificateRequest
    {
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public DateTimeOffset CompletionDate { get; set; }
        public IFormFile CertificateFile { get; set; }
    }
}
