using FrekvencijeProject.JSON.Allocations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.Models.Allocation
{
    public class Footnote_description
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_foot_desc { get; set; }
        public string name { get; set; }
        public string text_desc { get; set; }
        
        public string type { get; set; }

        public bool relevance { get; set; }


        
        public ICollection<FootnoteAllocation> FootnoteAllocationsDb { get; set; }
    }
}
