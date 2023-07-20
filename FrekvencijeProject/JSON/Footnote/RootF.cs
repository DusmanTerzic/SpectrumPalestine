using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.JSON.Footnote
{
    public class RootF
    {
        public int regionId { get; set; }
        public string regionName { get; set; }
        public string regionCode { get; set; }
        public List<FootnoteRange> footnoteRanges { get; set; }
    }
}
