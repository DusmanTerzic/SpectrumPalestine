using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.JSON.Footnote
{
    public class FootnoteRange
    {
        public long frequencyMin { get; set; }
        public long frequencyMax { get; set; }
        public List<FootnoteF> footnotes { get; set; }
    }
}
