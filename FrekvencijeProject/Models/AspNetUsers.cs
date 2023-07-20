using System;
using System.Collections.Generic;

namespace FrekvencijeProject.Models
{
   public class AspNetUsers
    {
        public string Id { get; set; }

        #nullable enable

        public string? UserName { get; set; }

        public string? NormalizedEmail { get; set; }
            
        #nullable disable

        public bool EmailConfirmed { get; set; }

        #nullable enable

        public string? PasswordHash { get; set; }

        public string? SecurityStamp { get; set; }

        public string? ConcurrencyStamp { get; set; }

        public string? PhoneNumber { get; set; } // Updated to nullable

        #nullable disable

        public bool PhoneNumberConfirmed { get; set; } 

        public bool TwoFactorEnabled { get; set; }

        #nullable enable

        public DateTimeOffset? LockoutEnd { get; set; } // Updated to nullable

        #nullable disable

        public bool LockoutEnabled { get; set; }

        public int AccessFailedCount { get; set; }

        #nullable enable

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        #nullable disable

        public ICollection<AspNetUserRoles> Roles { get; set; }
    }
}
