using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.Models.Ajax
{
    public class ApplicationRequest
    {
        [DisplayName("General")]
        public string GeneralVal { get; set; }
        [DisplayName("From")]
        public string FromVal { get; set; }
        [DisplayName("To")]
        public string ToVal { get; set; }
        [DisplayName("Frequency size")]
        public string FrequencySizeVal { get; set; }
        [DisplayName("Frequency table")]
        public string FrequencyTableVal { get; set; }

        
    }
}
