using System.ComponentModel.DataAnnotations;

namespace SCMS_back_end.Models
{
    public class Admin
    {
        [Key]
        public int AdminId { get; set; }
        public string UserId { get; set; }

        public User User { get; set; } = new User(); // Navigation property
    }

}
