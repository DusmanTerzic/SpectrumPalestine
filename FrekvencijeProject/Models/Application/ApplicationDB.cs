using FrekvencijeProject.Models.Document;
using FrekvencijeProject.Models.Standard;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.Models.Application
{
    public class ApplicationDB
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ApplicationId { get; set; }
        public int? ApplicationRangeId { get; set; }
        [ForeignKey("ApplicationRangeId")]
        public ApplicationRangeDB applicationRange { get; set; }
        public int? ApplicationTermId { get; set; }
        [ForeignKey("ApplicationTermId")]
        public RootApplicationTermsDB applicationTerm { get; set; }
        public string comment { get; set; }
        public int? documents_Id { get; set; }
        [ForeignKey("documents_Id")]
        public DocumentsDb DocumentsDb { get; set; }
        public int? Standard_id { get; set; }
        [ForeignKey("Standard_id")]
        public StandardsDb standard { get; set; }
        public bool isDeletedApp { get; set; }
        public string colorCode { get; set; }
        public int? TermId { get; set; }
    }
}
