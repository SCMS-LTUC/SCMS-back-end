using Microsoft.AspNetCore.Identity;
using NuGet.DependencyResolver;
using System.ComponentModel.DataAnnotations;

namespace SCMS_back_end.Models
{
    public class User:IdentityUser
    {
        public string Address { get; set; } = string.Empty;
        public Admin Admin { get; set; } = new Admin();// Navigation property
        public Teacher Teacher { get; set; } = new Teacher();// Navigation property
        public Student Student { get; set; } = new Student(); // Navigation property
    }

}
