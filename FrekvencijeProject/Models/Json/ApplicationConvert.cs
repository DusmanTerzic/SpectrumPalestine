using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.Models.Json
{
    public class ApplicationConvert
    {
        public string Application { get; set; }

        public bool isDeletedApp { get; set; }

        public string Comment { get; set; }
        public string colorCode { get; set; }
        public int? TermId { get; set; }

        //public List<int> DocumentId { get; set; }

        //public List<int> StandardId { get; set; }

        public List<DocumentConvertNew> DocumentsAditional { get; set; }


        public List<StandardsConvertNew> StandardsAditional { get; set; }

        public List<DocumentsConvert> Documents { get; set; }

        public List<StandardsConvert> Standards { get; set; }

    }
}
