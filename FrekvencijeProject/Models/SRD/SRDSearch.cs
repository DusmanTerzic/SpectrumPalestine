using FrekvencijeProject.Models.Document;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using iTextSharp.text;
using System.Collections.Generic;

namespace FrekvencijeProject.Models.SRD
{
    public class SRDSearch
    {
        public string DocumentNumber { get; set; }

        public string DocumentTitle { get; set; }

        public double Low { get; set; }

        public double High { get; set; }

        public string LowView { get; set; }
        public string HighView { get; set; }

        public string BandNote { get; set; }

        public string Country { get; set; }


        public List<string> Standards { get; set; }

        public List<string> StdLinks { get; set; }

        public string Standard { get; set; }

        public string Identifier { get; set; }

        public string Power { get; set; }


        public string FreqIssue { get; set; }

        public string stdParams { get; set; }

        public string Spectrum { get; set; }
        public string Modulation { get; set; }
        public string ECC_ERC { get; set; }

        public string ECC_ERCLink { get; set; }
        public string Note1 { get; set; }
        public string Note2 { get; set; }
        public string Note3 { get; set; }
        public string Note4 { get; set; }
        public string Note5 { get; set; }
        public string Note6 { get; set; }
        public string Note7 { get; set; }
        public string Note8 { get; set; }
    }
}
