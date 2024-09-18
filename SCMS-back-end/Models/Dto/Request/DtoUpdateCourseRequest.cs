namespace SCMS_back_end.Models.Dto.Request
{
    public class DtoUpdateCourseRequest
    {
        public int? TeacherId { get; set; }
        public int SubjectId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public int Capacity { get; set; }
        //public int Level { get; set; }
    }
}
