using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto.Request;
using SCMS_back_end.Models.Dto.Response;

namespace SCMS_back_end.Repositories.Interfaces
{
    public interface ICourse
    {
        //Create Course withouth teacher
        public Task<Course> CreateCourseWithoutTeacher(DtoCreateCourseWTRequest course);
        
        //Update Course Information
        public Task<Course> UpdateCourseInformation(int courseId, DtoUpdateCourseRequest course);

        // Get Course by Id
        public Task<DtoCourseResponse> GetCourseById(int courseId);

        // Get All Courses
        public Task<List<DtoCourseResponse>> GetAllCourses();
        
        // Get Courses that hasn't started yet
        public Task<List<DtoCourseResponse>> GetCoursesNotStarted();

        // Delete Course
        public Task<bool> DeleteCourse(int courseId);

        // Get previous courses of a student
        public Task<List<DtoPreviousCourseResponse>> GetPreviousCoursesOfStudent(int studentId);

        // Get courses of a student
        public Task<List<DtoCourseResponse>> GetCoursesOfStudent(int studentId);

        // Get current courses of a student
        public Task<List<DtoCourseResponse>> GetCurrentCoursesOfStudent(int studentId);

        // Get courses of a teacher
        public Task<List<DtoCourseResponse>> GetCoursesOfTeacher(int teacherId);

        // Get current courses of a teacher
        public Task<List<DtoCourseResponse>> GetCurrentCoursesOfTeacher(int teacherId);

        // Calculate the average of a grades of a course
        public Task CalculateAverageGrade(int courseId);
    }
}
