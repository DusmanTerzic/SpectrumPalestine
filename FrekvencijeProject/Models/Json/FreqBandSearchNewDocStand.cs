using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.Models.Json
{
    public class FreqBandSearchNewDocStand
    {
        public long low { get; set; }

        public string LowView { get; set; }

        public long high { get; set; }

        public string HighView { get; set; }

        public string Application { get; set; }

        

        public string Allocation { get; set; }

        public bool isPrimary { get; set; }

        public List<FootnoteJsonConvert> Footnote { get; set; }


        public int DocumentId { get; set; }
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

        public List<ApplicationConvert> AppItemsList { get; set; }


        public int StandardId { get; set; }

        public string Etsi_standard { get; set; }

        public string Title_docS { get; set; }

        public string HypelinkS { get; set; }

    }
}
