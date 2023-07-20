using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.JSON.Allocations
{
    public class Root
    {
        public List<AllocationRange> allocationRanges { get; set; }

        public int regionId { get; set; }
        public string regionName { get; set; }
        public string regionCode { get; set; }
    }
}
