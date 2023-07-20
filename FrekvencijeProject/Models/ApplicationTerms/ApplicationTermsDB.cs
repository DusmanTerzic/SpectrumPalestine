using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.Models.ApplicationTerms
{
    public class ApplicationTermsDB
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ApplicationTermsDBId { get; set; }
        public string name { get; set; }
    }
}
