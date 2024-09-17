namespace SCMS_back_end.Models
{
    public class CourseQuiz
    {
        public int CourseId { get; set; }
        public Course Course { get; set; }

        public int QuizId { get; set; }
        public Quiz Quiz { get; set; }
    }
}
