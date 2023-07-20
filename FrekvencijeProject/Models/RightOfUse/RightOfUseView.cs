using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.Models.RightOfUse
{
    public class RightOfUseView
    {
        public string Country { get; set; }
        public string Duplex { get; set; }

        [Display(Name = "Frequency band")]
        public string FrequencyBand { get; set; }

        public string Application { get; set; }

        public string Technology { get; set; }
        [Display(Name = "Licence holder")]
        
        public string LicenceHolder { get; set; }

        public string LicenceHolderLink { get; set; }

        [Display(Name = "Start date")]
        public string StartDate { get; set; }
        [Display(Name = "Expiry date")]
        public string ExpiryDate { get; set; }
        [Display(Name = "Transmitter location information")]
        public string Location { get; set; }

        public string LocationLink { get; set; }


        [Display(Name = "Spectrum Trading")]
        public string SpectrumTrading { get; set; }

        [Display(Name = "Short comment")]
        public string ShortComment { get; set; }

        public List<SelectListItem> FrequencyTablesList { get; set; }

        public string FrequencySizeValue { get; set; }

        public List<SelectListItem> FrequencySizesList { get; set; }
    }
}
