using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.Models.RightOfUse
{
    public class ImportTempRightOfUse
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RightOfUseId { get; set; }

        public string Duplex { get; set; }

        public string DownLowerFrequency { get; set; }

        public string DownUpperFrequency { get; set; }

        public string UpLinkLowerFrequency { get; set; }

        public string UpLinkUpperFrequency { get; set; }

        public string Application { get; set; }

        public string Technology { get; set; }

        public string LicenceHolder { get; set; }

        public string LicenceHolderLink { get; set; }

        public string StartDate { get; set; }

        public string ExpiryDate { get; set; }

        public string Location { get; set; }

        public string LocationLink { get; set; }

        public string SpectrumTrading { get; set; }

        public string Country { get; set; }

        public string ShortComment { get; set; }

        public bool isDeletedApp { get; set; }
        
    }
}
