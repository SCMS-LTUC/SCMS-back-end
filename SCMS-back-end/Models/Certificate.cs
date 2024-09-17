namespace SCMS_back_end.Models
{
    public class Certificate
    {
        public int CertificateId { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public DateTime CompletionDate { get; set; }
        public string CertificatePath { get; set; } = string.Empty;
        public Student Student { get; set; }
        public Course Course { get; set; }
    }
}
