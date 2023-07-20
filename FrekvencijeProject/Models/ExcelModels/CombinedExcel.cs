using System.Collections.Generic;

namespace FrekvencijeProject.Models.ExcelModels
{
    public class RegionData
    {
        public string regionCode { get; set; }
        public int regionId { get; set; }
        public string regionName { get; set; }
        public List<AllocationData> allocations { get; set; }
    }

    public class AllocationData
    {
        public string allocation { get; set; }
        public List<FootnoteData> footnotes { get; set; }
    }

    public class FootnoteData
    {
        public string footnoteName { get; set; }

        public bool isBand { get; set; }
    }

    public class ApplicationData
    {
        public string Name { get; set; }
        public string Comment { get; set; }
    }

    public class CombinedExcel
    {
        public long low { get; set; }
        public long high { get; set; }
        public string LowView { get; set; }
        public string HighView { get; set; }
        public List<RegionData> regions { get; set; }
        public List<ApplicationData> appNames { get; set; }
    }
}
