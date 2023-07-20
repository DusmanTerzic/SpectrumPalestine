using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.Models.Json
{
    public class FootnoteJsonConvert
    {
        public string Allocation { get; set; }
        public string Footnote { get; set; }

        public string FootnoteDesc { get; set; }

        public bool isBand { get; set; }

        public bool isPrimary { get; set; }
        public string colorCode { get; set; }
        public int? TermId { get; set; }
    }
}
