using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.Models
{
    public class SearchHomeModel
    {


        public SearchHomeModel()
        {
        //    Generals = new List<General>() {
        //new General { Text = "General", Value = 1 },
        //new General { Text = "Allocations", Value = 2 },
        //new General { Text = "Applications", Value = 3 },
        //new General { Text = "Documents", Value = 4 },
        //new General { Text = "Interfaces", Value = 5 },
        //new General { Text = "Right of use", Value = 6 }
        //};
           
        }

        
        
        [Display(Name = "Search by")]
        public string General { get; set; }

        
        public string GeneralValue { get; set; }

        public List<SelectListItem> GeneralsList = new List<SelectListItem>
    {
        new SelectListItem { Text = "General", Value = "1" },
        new SelectListItem { Text = "Allocations", Value = "2" },
        new SelectListItem { Text = "Applications", Value = "3" },
        new SelectListItem { Text = "Documents", Value = "4" },
        new SelectListItem { Text = "Interfaces", Value = "5" },
        new SelectListItem { Text = "Right of use", Value = "6" }
    };
        [DataType(DataType.Text)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }



        [DataType(DataType.Text)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }


        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}
