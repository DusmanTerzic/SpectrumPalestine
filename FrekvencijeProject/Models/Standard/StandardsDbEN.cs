using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.Models.Standard
{
    public class StandardsDbEN
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Standard_id { get; set; }

        public string Etsi_standard { get; set; }

        public string Part { get; set; }


        public string Version { get; set; }
        public string Title_doc { get; set; }
        public string Hypelink { get; set; }
        public string Type_of_Document { get; set; }
        public string Group_doc { get; set; }
        public string Low_freq { get; set; }
        public string High_freq { get; set; }
        public string Application { get; set; }
        public string Comment { get; set; }

        public string ValidFrom { get; set; }

        public string Expiry { get; set; }
    }
}
