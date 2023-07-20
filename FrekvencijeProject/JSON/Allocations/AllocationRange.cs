using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.JSON.Allocations
{
    public class AllocationRange
    {
        public long low { get; set; }
        public long high { get; set; }
        public List<Allocation> allocations { get; set; }
    }
}
