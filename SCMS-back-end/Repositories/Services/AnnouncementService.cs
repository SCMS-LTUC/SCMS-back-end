using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SCMS_back_end.Data;
using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto.Request.Announcement;
using SCMS_back_end.Models.Dto.Response.Announcement;
using SCMS_back_end.Models.Dto.Response.Assignment;
using SCMS_back_end.Repositories.Interfaces;
using SendGrid.Helpers.Mail;

namespace SCMS_back_end.Repositories.Services
{
    public class AnnouncementService : IAnnouncement
    {
        private readonly StudyCenterDbContext _context;
     
        public AnnouncementService(StudyCenterDbContext context)
        {
            _context = context;
        }

        public async Task<List<DtoGetAnnouncementRes>> GetAllStudentAnnouncement(int AudinceID)
        {
            var announcements = await _context.Announcements
               .Where(a => a.AudienceId == AudinceID)
               .Select(a => new DtoGetAnnouncementRes
               {
                   Title = a.Title,
                   Content = a.Content,
                   CreatedAt = a.CreatedAt,
                   UserId = a.UserId
               })
               .ToListAsync();


            if (!announcements.Any())
            {
                throw new ArgumentException("Invalid Teacher Announcements with Audince ID", nameof(AudinceID));
            }

            return announcements;
        }

        public async Task<List<DtoGetAnnouncementRes>> GetAllTeacherAnnouncement(int AudinceID)
        {
            
            var announcements = await _context.Announcements
                .Where(a => a.AudienceId == AudinceID)  
                .Select(a => new DtoGetAnnouncementRes
                {
                    Title = a.Title,
                    Content = a.Content,
                    CreatedAt = a.CreatedAt,
                    UserId = a.UserId
                })
                .ToListAsync();

           
            if (!announcements.Any())
            {
                throw new ArgumentException("Invalid Teacher Announcements with Audince ID", nameof(AudinceID));
            }

            return announcements;
        }

        public async Task<DtoGetAnnouncementRes> GetAnnouncementByCourseID(int courseId)
        {
            var Announcement = _context.CourseAnnouncements
                 .Where(x => x.CourseId == courseId)
                 .Include(x => x.Announcement)
                 .Select(a => new DtoGetAnnouncementRes
                 {
                     Title = a.Announcement.Title,
                     Content = a.Announcement.Content,
                     CreatedAt = a.Announcement.CreatedAt,
                     UserId = a.Announcement.UserId
                 }).FirstOrDefault();
            
            if (Announcement == null)
            {
                throw new ArgumentException("No announcements found for the given Course ID", nameof(courseId));
            }

            return Announcement;
        }

        public async Task<DtoPostAnnouncementByAdmin> PostAnnouncementByAdmin(DtoPostAnnouncementByAdmin Announcement)
        {

            var NewAnnouncement = new Announcement()
            {
               
                UserId = Announcement.UserId,
                Title = Announcement.Title,
                Content = Announcement.Content,
                CreatedAt = Announcement.CreatedAt,
                AudienceId = Announcement.AudienceId,

            };

            _context.Announcements.Add(NewAnnouncement);
            await _context.SaveChangesAsync();

            var Response = new DtoPostAnnouncementByAdmin()
            {
                UserId = Announcement.UserId,
                Title = Announcement.Title,
                Content = Announcement.Content,
                CreatedAt = Announcement.CreatedAt,
                AudienceId = Announcement.AudienceId,
            };

            return Response;
        }


        public async Task<DtoPostAnnouncementByTeacher> PostAnnouncementByTeacher(DtoPostAnnouncementByTeacher Announcement, int CourseId)
        {
           
            var courseExists = await _context.Courses.AnyAsync(c => c.CourseId == CourseId);
          
            if (!courseExists)
            {
                throw new ArgumentException("Invalid Course ID", nameof(CourseId));
               
            }

           
            var newAnnouncement = new Announcement
            {
                Title = Announcement.Title,
                Content =Announcement.Content,
                CreatedAt = DateTime.Now,
                UserId = Announcement.UserId,  
                AudienceId = Announcement.AudienceId
            };

            
            _context.Announcements.Add(newAnnouncement);
            await _context.SaveChangesAsync();

            
            var newCourseAnnouncement = new CourseAnnouncement
            {
                CourseId = Announcement.CourseId,
                AnnouncementId = newAnnouncement.AnnouncementId
            };

           
            _context.CourseAnnouncements.Add(newCourseAnnouncement);
            await _context.SaveChangesAsync();

            return Announcement;
        }
    }
}
