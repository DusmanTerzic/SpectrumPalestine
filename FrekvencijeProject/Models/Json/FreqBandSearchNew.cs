using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.Models.Json
{
    public class FreqBandSearchNew
    {
        public string orientation { get; set; }
        public long low { get; set; }

        public string LowView { get; set; }

        public long high { get; set; }

        public string HighView { get; set; }

        public string Application { get; set; }

        public string Allocation { get; set; }

        public bool isPrimary { get; set; }

        public List<FootnoteJsonConvert> Footnote { get; set; }

        public string Doc_number { get; set; }
        public string Title_of_doc { get; set; }
        public string Hyperlink { get; set; }


        public List<FootnoteJsonConvert> BandFootnote { get; set; }

        public string Comment { get; set; }

        public string regionName { get; set; }

        public string regionCode { get; set; }

        public List<SelectListItem> FrequencyTablesList { get; set; }

        public List<SelectListItem> FrequencySizesList { get; set; }

        public string FrequencySizeValue { get; set; }

        public string FrequencyTableValue { get; set; }

        public long lowJson { get; set; }

        public long highJson { get; set; }

        public int sizeFrequency { get; set; }
        public bool isAllValues { get; set; }
        public string colorCode { get; set; }
        public int? TermId { get; set; }
        public int? count { get; set; }
        public List<ApplicationConvert> AppItemsList { get; set; }
    }
}
