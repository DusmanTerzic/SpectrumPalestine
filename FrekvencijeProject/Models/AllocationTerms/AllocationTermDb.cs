using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.Models.AllocationTerms
{
    public class AllocationTermDb
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AllocationTermId { get; set; }
        public string name { get; set; }
    }
}
