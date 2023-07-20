using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.JSON.Allocations
{
    public class RootAllocationDB
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RootAllocationDBId { get; set; }
        public ICollection<AllocationRangeDb> allocationRanges { get; set; }
        public int regionId { get; set; }
        public string regionName { get; set; }
        public string regionCode { get; set; }


    }
}
