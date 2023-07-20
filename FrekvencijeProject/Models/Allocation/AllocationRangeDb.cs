using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.JSON.Allocations
{
    public class AllocationRangeDb
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AllocationRangeId { get; set; }

        public long low { get; set; }
        public long high { get; set; }


        public int? RootAllocationDBId { get; set; }
        [ForeignKey("RootAllocationDBId")]
        public RootAllocationDB rootAllocation { get; set; }

        public ICollection<AllocationDb> allocationsDb { get; set; }

        public string LowView { get; set; }

        public string HighView { get; set; }


    }
}
