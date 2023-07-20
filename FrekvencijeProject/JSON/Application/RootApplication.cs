using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.JSON.Application
{
    public class RootApplication
    {
        public int regionId { get; set; }
        public string regionName { get; set; }
        public List<ApplicationRange> applicationRanges { get; set; }
        public string regionCode { get; set; }
    }
}
