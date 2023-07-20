using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.Models.Json
{
    public class AllocationFootnoteSearch
    {
        public long low { get; set; }


        public long high { get; set; }
        public string Allocation { get; set; }

        public string Footnote { get; set; }

        public string FootnoteDesc { get; set; }
    }
}
