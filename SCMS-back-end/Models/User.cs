﻿using Microsoft.AspNetCore.Identity;
using NuGet.DependencyResolver;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace SCMS_back_end.Models
{
    public class User: IdentityUser
    {
        //[AllowNull]
        //public string Address { get; set; } = string.Empty;
        //public Admin Admin { get; set; } = new Admin();// Navigation property
        public Teacher? Teacher { get; set; } // Navigation property
        public Student? Student { get; set; } // Navigation property
    }

}
