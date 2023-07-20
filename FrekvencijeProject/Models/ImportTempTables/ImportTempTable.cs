using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.Models.ImportTempTables
{
    public class ImportTempTable
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ntfa_id { get; set; }

        public string lower_freq { get; set; }


        public string higher_freq { get; set; }

        public string itu_reg { get; set; }


        public string itu_reg_freq { get; set; }

        public string main_reg { get; set; }

        public string main_reg_freq { get; set; }

        public string document { get; set; }

        public string application { get; set; }

        public string standard { get; set; }

        public string notes { get; set; }
        
    }
}
