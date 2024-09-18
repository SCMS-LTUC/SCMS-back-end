namespace SCMS_back_end.Models.Dto.Response
{
    public class DtoCourseResponse
    {
        public string TeacherName { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public List<string> Days { get; set; } = new List<string>();
        public string ClassName { get; set; } = string.Empty;
        public int Capacity { get; set; }
        //public int Level { get; set; }
    }
}
