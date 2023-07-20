using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.Models.ImportTempTables
{
    public class ImportTempAllocationTable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int allocation_id { get; set; }

        public string lower_freq { get; set; }


        public string higher_freq { get; set; }

        public string itu_reg { get; set; }


        public string itu_reg_freq { get; set; }
    }
}
