using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.Models.Application
{
    public class RootApplicationTermsDB
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ApplicationTermsDBId { get; set; }
        public string name { get; set; }

        public int? Layer { get; set; }

        public int? first_up_lvl_id { get; set; }


        public int? second_up_lvl_id { get; set; }


        public int? Number { get; set; }
    }
}
