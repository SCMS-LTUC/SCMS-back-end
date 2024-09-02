namespace SCMS_back_end.Models.Dto.Request
{
    public class DtoCreateCourseWTRequest
    {
        public int SubjectId { get; set; }
        public int ScheduleId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public int Level { get; set; }
    }
}
