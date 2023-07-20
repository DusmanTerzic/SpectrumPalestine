using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.JSON.Allocations
{
    public class AllocationTermDb
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AllocationTermId { get; set; }
        public string name { get; set; }
        public int Layer { get; set; }
        public int first_up_lvl_id { get; set; }
        public int second_up_lvl_id { get; set; }
        public bool _PRIMARY { get; set; }

        public int? Number { get; set; }
        public string ColorCode { get; set; }
    }
}
