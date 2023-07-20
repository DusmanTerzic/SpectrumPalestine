using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.Models.Application
{
    public class RootApplicationDB
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RootApplicationDBId { get; set; }
        public int regionId { get; set; }
        public string regionName { get; set; }
        public ICollection<ApplicationRangeDB> applicationRanges { get; set; }
        public string regionCode { get; set; }
    }
}
