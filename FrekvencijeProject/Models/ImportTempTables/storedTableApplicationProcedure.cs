using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.Models.ImportTempTables
{
    public class storedTableApplicationProcedure
    {
        public int ApplicationId { get; set; }

        public int? ApplicationRangeId { get; set; }

        public int? ApplicationTermId { get; set; }

        public string? comment { get; set; }

        public int? documents_Id { get; set; }

        public int? Standard_id { get; set; }

        public long low { get; set; }

        public long high { get; set; }
      
        public int? RootApplicationDBId { get; set; }

        public string? LowView { get; set; }

        public string? HighView { get; set; }
        public string? name { get; set; }
      
        public int? regionId { get; set; }

        public string? regionName { get; set; }

        public string? regionCode { get; set; }

        public bool isDeletedApp { get; set; }

    }
}
