using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.Models.Application
{
    public class ApplicationRangeDB
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ApplicationRangeId { get; set; }

        public long low { get; set; }
        public long high { get; set; }

        public int? RootApplicationDBId { get; set; }
        [ForeignKey("RootApplicationDBId")]
        public RootApplicationDB RootApplication { get; set; }

        public ICollection<ApplicationDB> applicationDb { get; set; }

        public string LowView { get; set; }

        public string HighView { get; set; }
    }
}
