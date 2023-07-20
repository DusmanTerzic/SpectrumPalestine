using FrekvencijeProject.Models.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.Models.Ajax
{
    public class Footer
    {
        public string footTitle { get; set; }
        public string footDiscription { get; set; }
    }
    public class ApplicationView
    {
        public string orientation { get; set; }
        public long low { get; set; }

        public string LowView { get; set; }

        public long high { get; set; }

        public string HighView { get; set; }

        //public string Comment { get; set; }

        public List<ApplicationConvert> Application { get; set; }
        public List<Footer> footers = new List<Footer>();
        public string regionName { get; set; }

        public string regionCode { get; set; }
        public string colorCode { get; set; }

        public List<SelectListItem> FrequencyTablesList { get; set; }

        public List<SelectListItem> FrequencySizesList { get; set; }

        public string FrequencySizeValue { get; set; }

        public string FrequencyTableValue { get; set; }

        public long lowJson { get; set; }

        public long highJson { get; set; }

        public int sizeFrequency { get; set; }
        public bool isAllValues { get; set; }
        //public List<int> DocumentId { get; set; }
        //public List<int> StandardId { get; set; }
    }
}
