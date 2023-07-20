using FrekvencijeProject.Models.Allocation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.JSON.Allocations
{
    
    public class FootnoteAllocation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FootnoteId { get; set; }

        public int id { get; set; }
        public string name { get; set; }

        public bool isBand { get; set; }

        public int? AllocationId { get; set; }
        [ForeignKey("AllocationId")]
        public AllocationDb allocation { get; set; }

        public int? FootDescId { get; set; }
        [ForeignKey("FootDescId")]
        public Footnote_description foot_desc { get; set; }
    }
}
