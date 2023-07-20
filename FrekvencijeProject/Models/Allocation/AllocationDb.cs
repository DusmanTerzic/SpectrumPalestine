using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.JSON.Allocations
{
    public class AllocationDb
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AllocationId { get; set; }
        public int? AllocationTermId { get; set; }
        [ForeignKey("AllocationTermId")]
        public AllocationTermDb allocationTerm { get; set; }
        public bool primary { get; set; }
        // 
        public string colorCode { get; set; }
        public int? TermId { get; set; }
        public int? AllocationRangeId { get; set; }
        [ForeignKey("AllocationRangeId")]
        public AllocationRangeDb allocationRangeDb { get; set; }

        public ICollection<FootnoteAllocation> footnotes { get; set; }

    }
}
