using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.Models.Document
{
    public class DocumentsDb
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DocumentsId { get; set; }

        public string Doc_number { get; set; }

        public string Title_of_doc { get; set; }

        public string Hyperlink { get; set; }

        public string Type_of_doc { get; set; }

        public string Group_doc { get; set; }

        public string Low_freq { get; set; }

        public string High_freq { get; set; }

        public string Application { get; set; }

        public string Comment { get; set; }

        public string ValidFrom { get; set; }

        public string Expiry { get; set; }



        //public string Application2 { get; set; }

        //public string Application3 { get; set; }

        //public string Application4 { get; set; }

        //public string Application5 { get; set; }

        //public string Application6 { get; set; }

        //public string Application7 { get; set; }

        //public string Application8 { get; set; }

        //public string Application9 { get; set; }

        //public string Application10 { get; set; }

        //public string Application11 { get; set; }

        //public string Application12 { get; set; }

    }
}
