using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.Models
{
    public class AspNetUserRoles
    {
        [Key]
        public string UserId { get; set; }
        [Key]
        public string RoleId { get; set; }

        public AspNetRoles _Role { get; set; }


        public AspNetUsers _User { get; set; }

    }
}
