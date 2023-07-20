using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.Models.Ajax
{
    public class AllocationView
    {
        
        public long low { get; set; }
        
        public long high { get; set; }

        public string Term { get; set; }

        public string Footnote { get; set; }

        public string RegionName { get; set; }

        public string RegionCode { get; set; }


    }
}
