namespace SCMS_back_end.Models.Dto.Request
{
    public class DtoCreateCourseWTRequest
    {
        public int SubjectId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public List<int> WeekDays { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public int Level { get; set; }
    }
}
