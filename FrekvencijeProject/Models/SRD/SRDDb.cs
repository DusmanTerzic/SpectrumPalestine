using FrekvencijeProject.Models.Document;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FrekvencijeProject.Models.SRD
{
    public class SRDDb
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? DocumentId { get; set; }

        [ForeignKey("DocumentId")]
        public DocumentsDb Document { get; set; }

        public string BandNote { get; set; }

        public string Power { get; set; }

        public string Standards { get; set; }

        public string StdParams { get; set; }

        public string FreqIssue { get; set; }

        public string Identifier { get; set; }
        public string Country { get; set; }

        public string Spectrum { get; set; }
        public string Modulation { get; set; }
        public string ECC_ERC { get; set; }
        public string Note1 { get; set; }
        public string Note2 { get; set; }
        public string Note3 { get; set; }
        public string Note4 { get; set; }
        public string Note5 { get; set; }
        public string Note6 { get; set; }
        public string Note7 { get; set; }
        public string Note8 { get; set; }

        // Navigation property for the foreign key relationship
    }
}
