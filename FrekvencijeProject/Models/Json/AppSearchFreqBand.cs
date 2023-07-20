using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.Models.Json
{
    public class AppSearchFreqBand
    {
        public long low { get; set; }

        public string LowView { get; set; }

        public long high { get; set; }

        public string HighView { get; set; }

        public string Application { get; set; }

        public string Comment { get; set; }

        public int regionId { get; set; }

        public string regionName { get; set; }

        public string regionCode { get; set; }
    }
}
