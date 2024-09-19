using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto.Request.Announcement;
using SCMS_back_end.Models.Dto.Response;
using SCMS_back_end.Models.Dto.Response.Announcement;
using SCMS_back_end.Models.Dto.Response.Assignment;

namespace SCMS_back_end.Repositories.Interfaces
{
    public interface IAnnouncement
    {
        Task<DtoPostAnnouncementByAdmin> PostAnnouncementByAdmin(DtoPostAnnouncementByAdmin Announcement);

        Task<DtoPostAnnouncementByTeacher> PostAnnouncementByTeacher(DtoPostAnnouncementByTeacher Announcement,int CourseId);

        Task<List<DtoGetAnnouncementRes>> GetAllTeacherAnnouncement(int AudinceID);

        Task<List<DtoGetAnnouncementRes>> GetAllStudentAnnouncement(int AudinceID);

        public Task<DtoGetAnnouncementRes> GetAnnouncementByCourseID(int courseId);
    }
}
