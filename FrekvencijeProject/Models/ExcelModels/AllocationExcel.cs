namespace FrekvencijeProject.Models.ExcelModels
{
    public class AllocationExcel
    {
        public long low { get; set; }

        public string LowView { get; set; }

        public long high { get; set; }

        public string HighView { get; set; }

        public string regionName { get; set; }

        public string regionCode { get; set; }

        public int regionId { get; set; }

        public string allocation { get; set; }

        public string footnote { get; set; }

        public bool isBand { get; set; }
    }
}
