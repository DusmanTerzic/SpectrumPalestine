using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.Models.Interfaces
{
    public class InterfacesView
    {

        public string Country { get; set; }

        [Display(Name = "Radiocommunication Service")]
        public string RadiocommunicationService { get; set; }

        public string Application { get; set; }

        [Display(Name = "Frequency band")]
        
        public string FrequencyBand { get; set; }

        [Display(Name = "Channeling")]
        public string Channeling { get; set; }


        [Display(Name = "Occupied bandwidth")]
        public string OccupiedBandwidth { get; set; }


        [Display(Name = "Direction / Separation")]
        public string DirectionSeparation { get; set; }

        [Display(Name = "Transmit Power")]
        public string TransmitPower { get; set; }


       
        public string Notes { get; set; }


        public List<SelectListItem> FrequencyTablesList { get; set; }

        public string FrequencySizeValue { get; set; }

        public List<SelectListItem> FrequencySizesList { get; set; }

    }
}
