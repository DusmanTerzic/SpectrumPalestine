using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.Models.Json
{
    public class AsiaPacific
    {
        public List<FreqBandSearchNew> ListOfFreqBand { get; set; }

        public string FrequencySizeValue { get; set; }

        public string FrequencyTableValue { get; set; }

        public long lowJson { get; set; }

        public long highJson { get; set; }

        public int sizeFrequency { get; set; }
        public bool isAllValues { get; set; }

        public List<ApplicationConvert> AppItemsList { get; set; }
    }
}
