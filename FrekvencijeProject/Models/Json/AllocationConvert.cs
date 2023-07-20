using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.Models.Json
{
    public class AllocationConvert
    {
        public string Allocation { get; set; }

        public string regionName { get; set; }

        public List<FootnoteJsonConvert> Footnote { get; set; }

        public List<FootnoteJsonConvert> BandFootnote { get; set; }
    }
}
