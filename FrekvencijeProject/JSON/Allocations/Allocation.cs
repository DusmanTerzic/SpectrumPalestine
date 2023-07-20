using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.JSON.Allocations
{
    public class Allocation
    {
        public AllocationTerm allocationTerm { get; set; }
        public bool primary { get; set; }
        public List<Footnote> footnotes { get; set; }
    }
}
