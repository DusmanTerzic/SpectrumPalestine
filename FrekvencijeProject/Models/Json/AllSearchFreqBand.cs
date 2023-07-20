﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.Models.Json
{
    public class AllSearchFreqBand
    {
        public long low { get; set; }
        public string LowView { get; set; }
        public long high { get; set; }
        public string HighView { get; set; }
        public string Application { get; set; }
        public bool isDeletedApp { get; set; }
        public string Comment { get; set; }
        public string colorCode { get; set; }
        public int? TermId { get; set; }
        public string Doc_number { get; set; }
        public string Title_of_doc { get; set; }
        public string Hyperlink { get; set; }
        public string Allocation { get; set; }
        public bool isBand { get; set; }
        public string Footnote { get; set; }
        public string FootnoteDesc { get; set; }
        public bool Primary { get; set; }
        public int regionId { get; set; }
        public string regionName { get; set; }
        public string regionCode { get; set; }
        public int? documents_Id { get; set; }
        public int? Standard_id { get; set; }
    }
}
