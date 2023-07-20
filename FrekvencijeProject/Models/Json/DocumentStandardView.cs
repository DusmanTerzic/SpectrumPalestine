using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.Models.Json
{
    public class DocumentStandardView
    {
        public string CombineTitle { get; set; }


        public string Link { get; set; }

        public string FrequencyBand { get; set; }


        public string Type { get; set; }


        public string Application { get; set; }

        public string Notes { get; set; }

        public List<SelectListItem> FrequencyTablesList { get; set; }

        public string FrequencySizeValue { get; set; }

        public List<SelectListItem> FrequencySizesList { get; set; }
    }
}
