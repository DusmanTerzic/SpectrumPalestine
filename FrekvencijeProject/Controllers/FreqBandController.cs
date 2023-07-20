using FrekvencijeProject.Controllers.Actions;
using FrekvencijeProject.Models;
using FrekvencijeProject.Models.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.IO;
using FrekvencijeProject.Models.ExcelModels;
using OfficeOpenXml;
using System.Drawing;
using iTextSharp.text.pdf;
using System.Drawing.Imaging;
using iTextSharp.text;
using iTextSharp.text.pdf.qrcode;
using GemBox.Spreadsheet;
using System.Threading.Tasks;

namespace FrekvencijeProject.Controllers
{
    public class FreqBandController : Controller
    {
       
        object user;
        private readonly ApplicationDBContext _conApp;
        private readonly AllocationDBContext _conAll;
        [Display(Name = "Freq. Range")]
        public string Frequency { get; set; }

        [Display(Name = "From")]
        public string FromText { get; set; }
        public string From { get; set; }

        [Display(Name = "To")]
        public string ToText { get; set; }

        public string To { get; set; }

        [Display(Name = "Frequency Table")]
        public string FrequencyTable { get; set; }

        public string FrequencySizeValue { get; set; }

        public List<SelectListItem> FrequencySizesList = new List<SelectListItem>
        {
            new SelectListItem { Text = "Hz", Value = "1" },
            new SelectListItem { Text = "kHz", Value = "2" },
            new SelectListItem { Text = "MHz", Value = "3",Selected = true},
            new SelectListItem { Text = "GHz", Value = "4" }
        };

        public string FrequencytableValue { get; set; }

        public List<SelectListItem> FrequencyTablesList = new List<SelectListItem>
        {
            //new SelectListItem { Text = "--Arab States (ACA)--", Value = "1" },
            new SelectListItem { Text = "--Palestine (PSE)--", Value = "1" },
            new SelectListItem { Text = "- ITU (Region 1) -", Value = "2" },
            new SelectListItem { Text = "Algeria", Value = "3" },
            new SelectListItem { Text = "Bahrain", Value = "4" },
            new SelectListItem { Text = "Comoros", Value = "5" },
            new SelectListItem { Text = "Djibouti", Value = "6" },
            new SelectListItem { Text = "Egypt", Value = "7" },
            new SelectListItem { Text = "Iraq", Value = "8" },
            new SelectListItem { Text = "Jordan", Value = "9" },
            new SelectListItem { Text = "Kuwait", Value = "10" },
            new SelectListItem { Text = "Lebanon", Value = "11" },
            new SelectListItem { Text = "Libya", Value = "12" },
            new SelectListItem { Text = "Mauritania", Value = "13" },
            new SelectListItem { Text = "Morocco", Value = "14" },
            new SelectListItem { Text = "Oman", Value = "15" },
            //new SelectListItem { Text = "Palestine", Value = "16" },
            new SelectListItem { Text = "Qatar", Value = "16" },
            new SelectListItem { Text = "Saudi Arabia", Value = "17" },
            new SelectListItem { Text = "Somalia", Value = "18" },
            new SelectListItem { Text = "Sudan", Value = "19" },
            new SelectListItem { Text = "Syria", Value = "20" },
            new SelectListItem { Text = "Tunisia", Value = "21" },
            new SelectListItem { Text = "United Arab Emirates", Value = "22" },
            new SelectListItem { Text = "Yemen", Value = "23" }

        };

        public FreqBandController(
        IHttpContextAccessor httpContextAccessor,
         ApplicationDBContext conApp,
        AllocationDBContext conAll)
        {
            
            user = httpContextAccessor.HttpContext.User;
            _conApp = conApp;
            _conAll = conAll;
        }

        private List<CombinedExcel> getSearchResults(string fromVal, string toVal, string frequencySizeVal, string frequencyTableVal)
        {
            var from = fromVal;
            var to = toVal;
            var frequencySizeValue = "" + frequencySizeVal;
            var frequencytableValue = "" + frequencyTableVal;

            long tempFrom = 0;
            long tempTo = 0;
            if (from != null)
            {
                tempFrom = long.Parse(from);
                from = "" + tempFrom;
            }
            if (to != null)
            {
                tempTo = long.Parse(to);
                to = "" + tempTo;
            }

            if (frequencySizeValue == "2")
            {
                if (from != null)
                {
                    var s = string.Concat(from, "000");

                    long value = long.Parse(s);
                    tempFrom = value;
                    from = "" + tempFrom;
                }

                if (to != null)
                {
                    var s = string.Concat(to, "000");

                    long valueTo = long.Parse(s);
                    tempTo = valueTo;
                    to = "" + tempTo;
                }
            }
            else if (frequencySizeValue == "3")
            {
                if (from != null)
                {
                    var s = string.Concat(from, "000000");

                    long value = long.Parse(s);
                    tempFrom = value;

                    from = "" + tempFrom;
                }

                if (to != null)
                {
                    var s = string.Concat(to, "000000");

                    long value = long.Parse(s);
                    tempTo = value;
                    to = "" + tempTo;
                }

            }
            else if (frequencySizeValue == "4")
            {
                if (from != null)
                {
                    var s = string.Concat(from, "000000000");

                    long value = long.Parse(s);
                    tempFrom = value;

                    from = "" + tempFrom;
                }

                if (to != null)
                {
                    var s = string.Concat(to, "000000000");

                    long value = long.Parse(s);
                    tempTo = value;
                    to = "" + tempTo;
                }

            }

            var allocQuery = (from allo in _conAll.AllocationDb
                              join alr in _conAll.AllocationRangeDb on allo.AllocationRangeId equals alr.AllocationRangeId
                              join alt in _conAll.AllocationTermDb on allo.AllocationTermId equals alt.AllocationTermId
                              join ra in _conAll.RootAllocationDB on alr.RootAllocationDBId equals ra.RootAllocationDBId
                              join fa in _conAll.FootnoteAllocation on allo.AllocationId equals fa.AllocationId
                              join fd in _conAll.Footnote_description on fa.FootDescId equals fd.id_foot_desc
                              select new AllocationExcel
                              {
                                  low = alr.low,
                                  high = alr.high,
                                  LowView = alr.LowView,
                                  HighView = alr.HighView,
                                  regionId = ra.regionId,
                                  regionName = ra.regionName,
                                  regionCode = ra.regionCode,
                                  allocation = alt.name,
                                  footnote = fa.name,
                                  isBand = fa.isBand
                              })
                              .Where(p => p.low >= tempFrom && p.high <= tempTo).
                              ToList().OrderBy(p => p.low);

            var appQuery = (from app in _conApp.Application
                            join ar in _conApp.ApplicationRange on app.ApplicationRangeId equals ar.ApplicationRangeId
                            join apt in _conApp.RootApplicationTermsDB on app.ApplicationTermId equals apt.ApplicationTermsDBId
                            select new ApplicationExcel
                            {
                                low = ar.low,
                                high = ar.high,
                                LowView = ar.LowView,
                                HighView = ar.HighView,
                                name = apt.name,
                                comment = app.comment
                            })
                            .Where(p => p.low >= tempFrom && p.high <= tempTo)
                            .ToList().OrderBy(p => p.low);



            var allocList = allocQuery.GroupBy(p => new { p.low, p.high, p.LowView, p.HighView })
                                    .Select(group => new
                                    {
                                        low = group.Key.low,
                                        high = group.Key.high,
                                        LowView = group.Key.LowView,
                                        HighView = group.Key.HighView,
                                        regions = group
                                            .GroupBy(p => new { p.regionCode, p.regionId, p.regionName })
                                            .Select(regionGroup => new
                                            {
                                                regionCode = regionGroup.Key.regionCode,
                                                regionId = regionGroup.Key.regionId,
                                                regionName = regionGroup.Key.regionName,
                                                allocations = regionGroup
                                                    .GroupBy(p => p.allocation)
                                                    .Select(allocationGroup => new
                                                    {
                                                        allocation = allocationGroup.Key,
                                                        footnotes = allocationGroup
                                                            .GroupBy(p => new { p.footnote, p.isBand })
                                                            .Select(footnoteGroup => new
                                                            {
                                                                footnoteName = footnoteGroup.Key.footnote,
                                                                footnoteIsBand = footnoteGroup.Key.isBand
                                                            })
                                                            .ToList()
                                                    })
                                                    .ToList()
                                            })
                                            .ToList()
                                    })
                                    .ToList();

            var appList = appQuery
                        .GroupBy(p => new { p.low, p.high, p.LowView, p.HighView })
                        .Select(group => new
                        {
                            low = group.Key.low,
                            high = group.Key.high,
                            lowView = group.Key.LowView,
                            highView = group.Key.HighView,
                            appNames = group
                                .Select(p => new
                                {
                                    Name = p.name,
                                    Comment = p.comment
                                })
                                .GroupBy(p => p.Name)
                                .Select(nameGroup => nameGroup.First())
                                .ToList()
                        })
                        .ToList();

            var combinedData = allocList
                                .GroupJoin(appList,
                                    allocation => new { allocation.low, allocation.high },
                                    application => new { application.low, application.high },
                                    (allocation, application) => new
                                    {
                                        allocation.low,
                                        allocation.high,
                                        allocation.LowView,
                                        allocation.HighView,
                                        allocation.regions,
                                        appNames = application.SelectMany(app => app.appNames).ToList()
                                    })
                                .ToList();

            var filteredData = combinedData
                            .Where(data => data.low >= tempFrom && data.high <= tempTo)
                            .ToList();

            var combinedDataList = filteredData.Select(item => new CombinedExcel
            {
                low = item.low,
                high = item.high,
                LowView = item.LowView,
                HighView = item.HighView,
                regions = item.regions.OrderBy(region => region.regionId).Select(region => new RegionData
                {
                    regionCode = region.regionCode,
                    regionId = region.regionId,
                    regionName = region.regionName,
                    allocations = region.allocations.Select(allocation => new AllocationData
                    {
                        allocation = allocation.allocation,
                        footnotes = allocation.footnotes.Select(footnote => new FootnoteData
                        {
                            footnoteName = footnote.footnoteName,
                            isBand = footnote.footnoteIsBand
                        }).ToList()
                    }).ToList()
                }).ToList(),
                appNames = item.appNames.Select(app => new ApplicationData
                {
                    Name = app.Name,
                    Comment = app.Comment
                }).ToList()
            }).ToList();

            return combinedDataList;
        }

        // Custom comparison function for sorting by the numeric part
        private int ComparePSExStrings(string s1, string s2)
        {
            int n1 = int.Parse(s1.Substring(3)); // Extract numeric part from s1
            int n2 = int.Parse(s2.Substring(3)); // Extract numeric part from s2

            return n1.CompareTo(n2); // Compare the numeric parts
        }

        private class CustomAllocComparer : IComparer<AllocationData>
        {
            public int Compare(AllocationData x, AllocationData y)
            {
                if (x.allocation == null && y.allocation == null)
                {
                    return 0;
                }
                else if (x.allocation == null)
                {
                    return -1;
                }
                else if (y.allocation == null)
                {
                    return 1;
                }

                bool xHasMultipleCaps = x.allocation.Count(char.IsUpper) > 1;
                bool yHasMultipleCaps = y.allocation.Count(char.IsUpper) > 1;

                if (xHasMultipleCaps && !yHasMultipleCaps)
                {
                    return -1;
                }
                else if (!xHasMultipleCaps && yHasMultipleCaps)
                {
                    return 1;
                }
                else if (xHasMultipleCaps && yHasMultipleCaps)
                {
                    return string.Compare(x.allocation, y.allocation, StringComparison.Ordinal);
                }
                else
                {
                    return string.Compare(x.allocation, y.allocation, StringComparison.Ordinal);
                }
            }
        }

        private byte[] GenerateExcel(string fromVal, string toVal, string frequencySizeVal, string frequencyTableVal)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                // Set column widths
                worksheet.Column(1).Width = 25;  // Column A - Allocations for region
                worksheet.Column(2).Width = 15;  // Column B - To merge with A
                worksheet.Column(3).Width = 40;   // Column C - Applications
                worksheet.Column(4).Width = 70;  // Column D - Notes

                // Merge cells and write title
                worksheet.Cells["A1:D2"].Merge = true;
                worksheet.Cells["A1"].Value = "Table of National Frequency Allocations and Applications";
                worksheet.Cells["A1:D2"].Style.Font.Color.SetColor(Color.FromArgb(0x92, 0xD0, 0x50));
                worksheet.Cells["A1"].Style.Font.Size = 16;
                worksheet.Cells["A1"].Style.Font.Name = "Calibri";
                worksheet.Cells["A1"].Style.Font.Bold = true;
                worksheet.Cells["A1"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center; // Align in the middle

                string title = "";
                if (frequencyTableVal == "1")
                {
                    title = "National Allocations";
                }
                else
                {
                    title = "ITU RR Region 1 Allocations";
                }

                // Format the third row
                worksheet.Cells["A3"].Value = title;
                worksheet.Cells["A3:B3"].Merge = true;
                worksheet.Cells["C3"].Value = "Applications";
                worksheet.Cells["D3"].Value = "Notes";

                var headerStyle = worksheet.Cells["A3:D3"].Style;
                headerStyle.Font.Bold = true;
                headerStyle.Font.Italic = true;
                headerStyle.Font.Name = "Calibri";
                headerStyle.Font.Size = 14;
                headerStyle.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                headerStyle.Fill.BackgroundColor.SetColor(Color.FromArgb(0xC6, 0xE0, 0xB4));

                //get the data for the table
                List<CombinedExcel> freqBands = getSearchResults(fromVal, toVal, frequencySizeVal, frequencyTableVal);
                bool swap = true;

                int currentRow = 4;
                int regionId = int.Parse(frequencyTableVal);

                foreach (CombinedExcel item in freqBands)
                {
                    int firstRow = currentRow; //cuvamo zbog stila

                    //postavlja se prvi red (vrijednosti za low i high)
                    worksheet.Cells["A" + currentRow + ":B" + currentRow].Merge = true;
                    worksheet.Cells["A" + currentRow].Value = item.LowView + " - " + item.HighView;

                    currentRow += 2;//red pauze pa idu podaci
                    int rowApp = currentRow;
                    int rowReg = currentRow;

                    int maxRow = currentRow;


                    foreach (RegionData reg in item.regions)
                    {
                        if (reg.regionId != regionId) continue; //gledamo za onaj region koji je nas 

                        List<string> psesStart = new List<string>();
                        List<string> bandsStart = new List<string>();
                        reg.allocations.Sort(new CustomAllocComparer());

                        foreach (AllocationData allocItem in reg.allocations)
                        {
                            worksheet.Cells["A" + rowReg].Value = allocItem.allocation;
                            worksheet.Cells["A" + rowReg + ":B" + rowReg].Merge = true;

                            foreach (FootnoteData footnoteItem in allocItem.footnotes)
                            {
                                if (footnoteItem.footnoteName.Contains("PSE")) //ako ima PSE pisemo ga desno
                                {
                                    psesStart.Add(footnoteItem.footnoteName);
                                }
                                else if (!footnoteItem.isBand) //ako jeste band pisemo je odvojeno
                                {
                                    bandsStart.Add(footnoteItem.footnoteName);
                                }
                                else
                                {
                                    if (worksheet.Cells["A" + rowReg].Value.ToString().Length + footnoteItem.footnoteName.Length > 35)
                                    {
                                        //da ne bi prelazilo u sledecu kolonu
                                        rowReg++;
                                        worksheet.Cells["A" + rowReg].Value += footnoteItem.footnoteName;
                                    }
                                    else
                                    {
                                        worksheet.Cells["A" + rowReg].Value += "    " + footnoteItem.footnoteName; //dodati provjeru za strlen i prelazak u novi red
                                    }

                                }
                            }
                            rowReg++;
                        }

                        List<string> pses = psesStart.Distinct().ToList();
                        pses.Sort(ComparePSExStrings);

                        List<string> bands = bandsStart.Distinct().ToList();

                        for (int i = 0; i < System.Math.Max(pses.Count(), bands.Count()); i++)
                        {
                            if (bands.Count() > i)
                            {
                                worksheet.Cells["A" + rowReg].Value = bands[i];
                            }
                            if (pses.Count() > i)
                            {
                                //worksheet.Cells["B" + rowReg].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["B" + rowReg].Style.Font.Bold = true;
                                worksheet.Cells["B" + rowReg].Value = pses[i];
                            }
                            rowReg++;
                        }
                    }

                    maxRow = rowReg;

                    //postavljanje za aplikacije
                    foreach (ApplicationData appItem in item.appNames)
                    {
                        worksheet.Cells["C" + rowApp].Value = appItem.Name;
                        string temp = appItem.Comment;
                        while (temp.Length > 70)
                        {
                            int lastSpaceIndex = temp.Substring(0, 70).LastIndexOf(' ');
                            if (lastSpaceIndex <= 0)
                            {
                                // No space found within the substring or at the beginning,
                                // split at the exact character
                                lastSpaceIndex = 70;
                            }

                            worksheet.Cells["D" + rowApp].Value = temp.Substring(0, lastSpaceIndex);
                            temp = temp.Substring(lastSpaceIndex).TrimStart();
                            rowApp++;
                        }
                        worksheet.Cells["D" + rowApp].Value = temp; // Store the remaining part of the comment
                        rowApp++;
                    }


                    currentRow = System.Math.Max(maxRow, rowApp);

                    //postavimo boju za ovaj red iz freqBanda
                    string cells = "A" + firstRow + ":D" + currentRow;
                    worksheet.Cells[cells].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                    if (swap) //da bi tabela bila striped
                        worksheet.Cells[cells].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0xDD, 0xEB, 0xF7));
                    else
                        worksheet.Cells[cells].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0xFF, 0xFF, 0xFF));

                    worksheet.Cells[cells].Style.Font.Size = 11;
                    worksheet.Cells[cells].Style.Font.Name = "Calibri";
                    worksheet.Cells["A" + firstRow].Style.Font.Bold = true;
                    worksheet.Cells["A" + firstRow].Style.Font.Size = 14;

                    swap = !swap;
                    currentRow++;
                }


                var memoryStream = new MemoryStream();
                package.SaveAs(memoryStream);
                memoryStream.Position = 0;

                var fileBytes = memoryStream.ToArray();

                return fileBytes;
            }
        }

        private byte[] GenerateExcel150(string fromVal, string toVal, string frequencySizeVal, string frequencyTableVal)
        {

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                // Set column widths
                worksheet.Column(1).Width = 25;  // Column A - Allocations for region
                worksheet.Column(2).Width = 15;  // Column B - To merge with A
                worksheet.Column(3).Width = 40;   // Column C - Applications
                worksheet.Column(4).Width = 70;  // Column D - Notes

                // Merge cells and write title
                worksheet.Cells["A1:D2"].Merge = true;
                worksheet.Cells["A1"].Value = "Table of National Frequency Allocations and Applications";
                worksheet.Cells["A1:D2"].Style.Font.Color.SetColor(Color.FromArgb(0x92, 0xD0, 0x50));
                worksheet.Cells["A1"].Style.Font.Size = 16;
                worksheet.Cells["A1"].Style.Font.Name = "Calibri";
                worksheet.Cells["A1"].Style.Font.Bold = true;
                worksheet.Cells["A1"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center; // Align in the middle

                string title = "";
                if (frequencyTableVal == "1")
                {
                    title = "National Allocations";
                }
                else
                {
                    title = "ITU RR Region 1 Allocations";
                }

                // Format the third row
                worksheet.Cells["A3"].Value = title;
                worksheet.Cells["A3:B3"].Merge = true;
                worksheet.Cells["C3"].Value = "Applications";
                worksheet.Cells["D3"].Value = "Notes";

                var headerStyle = worksheet.Cells["A3:D3"].Style;
                headerStyle.Font.Bold = true;
                headerStyle.Font.Italic = true;
                headerStyle.Font.Name = "Calibri";
                headerStyle.Font.Size = 14;
                headerStyle.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                headerStyle.Fill.BackgroundColor.SetColor(Color.FromArgb(0xC6, 0xE0, 0xB4));

                //get the data for the table
                List<CombinedExcel> freqBands = getSearchResults(fromVal, toVal, frequencySizeVal, frequencyTableVal);
                bool swap = true;

                int currentRow = 4;
                int regionId = int.Parse(frequencyTableVal);

                foreach (CombinedExcel item in freqBands)
                {
                    if (currentRow > 100) break;
                    int firstRow = currentRow; //cuvamo zbog stila

                    //postavlja se prvi red (vrijednosti za low i high)
                    worksheet.Cells["A" + currentRow + ":B" + currentRow].Merge = true;
                    worksheet.Cells["A" + currentRow].Value = item.LowView + " - " + item.HighView;

                    currentRow += 2;//red pauze pa idu podaci
                    int rowApp = currentRow;
                    int rowReg = currentRow;

                    int maxRow = currentRow;


                    foreach (RegionData reg in item.regions)
                    {
                        if (reg.regionId != regionId) continue; //gledamo za onaj region koji je nas 

                        List<string> psesStart = new List<string>();
                        List<string> bandsStart = new List<string>();

                        foreach (AllocationData allocItem in reg.allocations)
                        {
                            worksheet.Cells["A" + rowReg].Value = allocItem.allocation;
                            worksheet.Cells["A" + rowReg + ":B" + rowReg].Merge = true;

                            foreach (FootnoteData footnoteItem in allocItem.footnotes)
                            {
                                if (footnoteItem.footnoteName.Contains("PSE")) //ako ima PSE pisemo ga desno
                                {
                                    psesStart.Add(footnoteItem.footnoteName);
                                }
                                else if (!footnoteItem.isBand) //ako jeste band pisemo je odvojeno
                                {
                                    bandsStart.Add(footnoteItem.footnoteName);
                                }
                                else
                                {
                                    if (worksheet.Cells["A" + rowReg].Value.ToString().Length + footnoteItem.footnoteName.Length > 35)
                                    {
                                        //da ne bi prelazilo u sledecu kolonu
                                        rowReg++;
                                        worksheet.Cells["A" + rowReg].Value += footnoteItem.footnoteName;
                                    }
                                    else
                                    {
                                        worksheet.Cells["A" + rowReg].Value += "    " + footnoteItem.footnoteName; //dodati provjeru za strlen i prelazak u novi red
                                    }

                                }
                            }
                            rowReg++;
                        }

                        List<string> pses = psesStart.Distinct().ToList();
                        pses.Sort(ComparePSExStrings);

                        List<string> bands = bandsStart.Distinct().ToList();

                        for (int i = 0; i < System.Math.Max(pses.Count(), bands.Count()); i++)
                        {
                            if (bands.Count() > i)
                            {
                                worksheet.Cells["A" + rowReg].Value = bands[i];
                            }
                            if (pses.Count() > i)
                            {
                                //worksheet.Cells["B" + rowReg].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells["B" + rowReg].Style.Font.Bold = true;
                                worksheet.Cells["B" + rowReg].Value = pses[i];
                            }
                            rowReg++;
                        }
                    }

                    maxRow = rowReg;

                    //postavljanje za aplikacije
                    foreach (ApplicationData appItem in item.appNames)
                    {
                        worksheet.Cells["C" + rowApp].Value = appItem.Name;
                        string temp = appItem.Comment;
                        while (temp.Length > 70)
                        {
                            int lastSpaceIndex = temp.Substring(0, 70).LastIndexOf(' ');
                            if (lastSpaceIndex <= 0)
                            {
                                // No space found within the substring or at the beginning,
                                // split at the exact character
                                lastSpaceIndex = 70;
                            }

                            worksheet.Cells["D" + rowApp].Value = temp.Substring(0, lastSpaceIndex);
                            temp = temp.Substring(lastSpaceIndex).TrimStart();
                            rowApp++;
                        }
                        worksheet.Cells["D" + rowApp].Value = temp; // Store the remaining part of the comment
                        rowApp++;
                    }

                    currentRow = System.Math.Max(maxRow, rowApp);

                    //postavimo boju za ovaj red iz freqBanda
                    string cells = "A" + firstRow + ":D" + currentRow;
                    worksheet.Cells[cells].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                    if (swap) //da bi tabela bila striped
                        worksheet.Cells[cells].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0xDD, 0xEB, 0xF7));
                    else
                        worksheet.Cells[cells].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0xFF, 0xFF, 0xFF));

                    worksheet.Cells[cells].Style.Font.Size = 11;
                    worksheet.Cells[cells].Style.Font.Name = "Calibri";
                    worksheet.Cells["A" + firstRow].Style.Font.Bold = true;
                    worksheet.Cells["A" + firstRow].Style.Font.Size = 14;

                    swap = !swap;
                    currentRow++;
                }


                var memoryStream = new MemoryStream();
                package.SaveAs(memoryStream);
                memoryStream.Position = 0;

                var fileBytes = memoryStream.ToArray();

                return fileBytes;
            }
        }


        [HttpPost]
        [Route("FreqBand/DownloadExcel")]
        public byte[] DownloadExcel(string fromVal, string toVal, string frequencySizeVal, string frequencyTableVal)
        {
            return GenerateExcel(fromVal, toVal, frequencySizeVal, frequencyTableVal);
        }

        [HttpPost]
        [Route("FreqBand/DownloadPDF")]
        public byte[] DownloadPDF(string fromVal, string toVal, string frequencySizeVal, string frequencyTableVal)
        {
            byte[] excelBytes = GenerateExcel150(fromVal, toVal, frequencySizeVal, frequencyTableVal);
            SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
            // Load the Excel byte array
            var workbook = ExcelFile.Load(new MemoryStream(excelBytes));

            // Create a new workbook for the limited rows
            var limitedWorkbook = new ExcelFile();

            // Copy the desired worksheet to the limited workbook
            var worksheet = workbook.Worksheets[0]; // Assuming you're working with the first worksheet
            var limitedWorksheet = limitedWorkbook.Worksheets.AddCopy("name", worksheet);



            // Create a memory stream to store the PDF
            using (MemoryStream pdfStream = new MemoryStream())
            {
                limitedWorksheet.PrintOptions.FitWorksheetWidthToPages = 1;

                // Save the limited workbook as PDF to the memory stream
                limitedWorkbook.Save(pdfStream, SaveOptions.PdfDefault);

                // Retrieve the PDF byte array
                byte[] pdfByteArray = pdfStream.ToArray();

                return pdfByteArray;

                // Use the pdfByteArray as needed
                // ...
            }
        }

        public IActionResult Index()
        {
            //execute this condition when user search general from home or start page.
            if (TempData["general"] != null)
            {
               
                bool value = (bool)TempData["general"];
                if (value == true)
                {
                    //Debug.WriteLine("prosao"+ value);
                    string from = (string)TempData["tempFrom"];
                    string to = (string)TempData["tempTo"];
                    
                    FrequencytableValue = (string)TempData["FrequencytableValue"];
                    FrequencyTablesList.FirstOrDefault(p => p.Value.Equals(this.FrequencytableValue)).Selected = true; 
                    //FrequencyTablesList.ForEach(x => x.Selected = x.Value.Contains(FrequencytableValue));
                    //temp.Selected = true;

                    //TempData["FrequencytableVal"] = FrequencytableValue;
                    //List<AllocationSearch> listAl = JsonConvert.DeserializeObject<List<AllocationSearch>>(TempData["allocations"].ToString());
                    //Debug.WriteLine("after al:"+ from+","+to);
                    long tempFrom = long.Parse(from);
                    long tempTo = long.Parse(to);
                    FrequencySizeValue = (string)TempData["FrequencySize"];
                    //string TempFrequencySizeValue = (string)TempData["TempFrequencySizeValue"];
                     //TempData["FrequencySizeValue"]
                    //TempData["FrequencytableVal"] = FrequencySizeValue;
                    //Debug.WriteLine("val:" + FrequencySizeValue);
                    //FrequencySizesList.FirstOrDefault(p => p.Value.Equals(FrequencySizeValue)).Selected = true;
                    //Debug.WriteLine("ttt:" + FrequencySizesList.FirstOrDefault(p => p.Value.Equals(this.FrequencySizeValue)).Selected);
                    ViewBag.FreqSize = FrequencySizesList;
                    if(FrequencySizeValue == "1")
                    {
                        TempData["fromTempValue"] = tempFrom;
                        TempData["toTempValue"] = tempTo;
                    }
                    else if (FrequencySizeValue == "2")
                    {
                        //1000.0 khz
                        long result = tempTo / 1000;
                        TempData["toTempValue"] = result;

                        long resultFrom = tempFrom / 1000;
                        TempData["fromTempValue"] = resultFrom;

                    }
                    else if (FrequencySizeValue == "3")
                    {

                        //1000000.0 mhz
                        long result = tempTo / 1000000;
                        TempData["toTempValue"] = result;


                        long resultFrom = tempFrom / 1000000;
                        TempData["fromTempValue"] = resultFrom;
                    }
                    else if (FrequencySizeValue == "4")
                    {
                        //1000000000.0 GHz
                        long result = tempTo / 1000000000;
                        TempData["toTempValue"] = result;

                        long resultFrom = tempFrom / 1000000000;
                        TempData["fromTempValue"] = resultFrom;
                    }
                    if (tempFrom == 0 && tempTo == 0)
                    {
                        //Debug.WriteLine("nula");
                        
                        FreqBandSearchActions freqBand = new FreqBandSearchActions();
                        var listGeneral = freqBand.SearchFreqBand(_conAll, _conApp, tempFrom, tempTo, FrequencytableValue);
                        //listGeneral[0].FrequencyTablesList = FrequencyTablesList;
                        //listGeneral[0].FrequencySizesList = FrequencySizesList;
                        //listGeneral[0].FrequencySizeValue = FrequencySizeValue;
                        //return View(listGeneral.OrderBy(e => e.low).ToList());
                        var NewListGeneral = listGeneral.OrderBy(e => e.low).ToList();

                        


                        NewListGeneral[0].FrequencyTablesList = FrequencyTablesList;
                        NewListGeneral[0].FrequencySizesList = FrequencySizesList;
                        NewListGeneral[0].FrequencySizeValue = FrequencySizeValue;
                        return View(NewListGeneral);

                    }
                    else if (tempFrom == 0 && tempTo != 0)
                    {
                        //Debug.WriteLine("idemo sad");
                        
                        FreqBandSearchActions freqBand = new FreqBandSearchActions();
                        var listGeneral = freqBand.SearchFreqBand(_conAll, _conApp, tempFrom, tempTo, FrequencytableValue);
                        //listGeneral[0].FrequencyTablesList = FrequencyTablesList;
                        //listGeneral[0].FrequencySizesList = FrequencySizesList;
                        //listGeneral[0].FrequencySizeValue = FrequencySizeValue;
                        //return View(listGeneral.OrderBy(e => e.low).ToList());
                        var NewListGeneral = listGeneral.OrderBy(e => e.low).ToList();
                        if(NewListGeneral.Count == 0)
                        {
                            FreqBandSearchNew pp = new FreqBandSearchNew()
                            {
                                Footnote = new List<FootnoteJsonConvert>(),
                                BandFootnote = new List<FootnoteJsonConvert>(),
                                FrequencyTablesList = FrequencyTablesList,
                                FrequencySizesList = FrequencySizesList,
                                FrequencySizeValue = FrequencySizeValue
                            };
                            NewListGeneral.Add(pp);
                        }
                        
                         NewListGeneral[0].FrequencyTablesList = FrequencyTablesList;
                         NewListGeneral[0].FrequencySizesList = FrequencySizesList;
                         NewListGeneral[0].FrequencySizeValue = FrequencySizeValue;
                        
                        
                        return View(NewListGeneral);
                    }
                    else if (tempFrom != 0 && tempTo != 0)
                    {
                        //tu je stao
                        //Debug.WriteLine("razlicite vrijednosti");
                        FreqBandSearchActions freqBand = new FreqBandSearchActions();
                        var listGeneral = freqBand.SearchFreqBand(_conAll, _conApp, tempFrom, tempTo, FrequencytableValue);
                        var NewListGeneral = listGeneral.OrderBy(e => e.low).ToList();
                        NewListGeneral[0].FrequencyTablesList = FrequencyTablesList;
                        NewListGeneral[0].FrequencySizesList = FrequencySizesList;
                        NewListGeneral[0].FrequencySizeValue = FrequencySizeValue;
                        return View(NewListGeneral);
                        //listGeneral[0].FrequencyTablesList = FrequencyTablesList;
                        //listGeneral[0].FrequencySizesList = FrequencySizesList;
                        //listGeneral[0].FrequencySizeValue = FrequencySizeValue;
                        //return View(listGeneral.OrderBy(e => e.low).ToList());
                  
                    }
                    //Debug.WriteLine("kraj");
                    ViewBag.PDFLink = Url.Action("DownloadPDF");

                    return View();
                }
                else
                {
                    //Debug.WriteLine("proba");
                    ViewBag.PDFLink = Url.Action("DownloadPDF");

                    return View();
                }
            }
            else
            {
                //Debug.WriteLine("jos!");
                ViewBag.PDFLink = Url.Action("DownloadPDF");

                return View();
            }
                   
        }

        [HttpPost]
        //metod for searching freq band using jquery or javasript.
        public JsonResult GeneralSearch( string FromVal, string ToVal, int FrequencySizeVal, int FrequencyTableVal)
        {

           // Debug.WriteLine("testiranje:" );
            From = FromVal;
            To = ToVal;
            FrequencySizeValue = "" + FrequencySizeVal;
            FrequencytableValue = "" + FrequencyTableVal;

            long tempFrom = 0;
            long tempTo = 0;
            if (From != null)
            {
                tempFrom = long.Parse(From);
                From = "" + tempFrom;
            }
            if (To != null)
            {
                tempTo = long.Parse(To);
                To = "" + tempTo;
            }

            if (FrequencySizeValue == "2")
            {
                if (From != null)
                {
                    var s = string.Concat(From, "000");

                    long value = long.Parse(s);
                    tempFrom = value;
                    From = "" + tempFrom;
                }

                if (To != null)
                {
                    var s = string.Concat(To, "000");


                    long valueTo = long.Parse(s);
                    // Debug.WriteLine("value:" + valueTo);
                    tempTo = valueTo;
                    // Debug.WriteLine("value 2:" + tempTo);
                    To = "" + tempTo;
                }
            }
            else if (FrequencySizeValue == "3")
            {
                if (From != null)
                {
                    var s = string.Concat(From, "000000");

                    long value = long.Parse(s);
                    tempFrom = value;

                    From = "" + tempFrom;
                }

                if (To != null)
                {
                    var s = string.Concat(To, "000000");

                    long value = long.Parse(s);
                    tempTo = value;
                    To = "" + tempTo;
                }

            }
            else if (FrequencySizeValue == "4")
            {
                if (From != null)
                {
                    var s = string.Concat(From, "000000000");

                    long value = long.Parse(s);
                    tempFrom = value;

                    From = "" + tempFrom;
                }

                if (To != null)
                {
                    var s = string.Concat(To, "000000000");

                    long value = long.Parse(s);
                    tempTo = value;
                    To = "" + tempTo;
                }

            }


            if (From != "" && To != "")
            {
                if (FrequencytableValue != "0")
                {

                    FreqBandSearchActions freqBand = new FreqBandSearchActions();
                    var listGeneral = freqBand.SearchFreqBand(_conAll, _conApp, tempFrom, tempTo, FrequencytableValue);
                    if (tempFrom == 0 && tempTo == 0) {
                        var BestproductSale = _conApp.ApplicationRange.ToList()
                                  .GroupBy(x => x.high)
                                  .Select(grp => new
                                  {
                                      High = grp.ToList().FirstOrDefault().high

                                  })
                                  .OrderByDescending(x => x.High).ToList();

                        long HighTemp = BestproductSale[0].High;

                        var HighView = (from all in _conApp.ApplicationRange
                                        where all.high == BestproductSale[0].High
                                        select new
                                        {
                                            all.HighView
                                        }
                        ).FirstOrDefault();


                        int sizeFrequency = 0;
                        long result = 0;
                        if (HighView.HighView.Contains("GHz"))
                        {
                            result = HighTemp / 1000000000;

                            sizeFrequency = 4;
                        }
                        else if (HighView.HighView.Contains("MHz"))
                        {
                            result = HighTemp / 1000000;

                            sizeFrequency = 3;
                        }
                        else if (HighView.HighView.Contains("kHz"))
                        {
                            result = HighTemp / 1000;
                            sizeFrequency = 2;
                        }
                        else if (HighView.HighView.Contains("Hz"))
                        {
                            sizeFrequency = 1;
                        }

                        var newList = listGeneral.OrderBy(e => e.low).ToList();
                        newList[0].lowJson = 0;
                        newList[0].highJson = result;
                        newList[0].isAllValues = true;
                        newList[0].sizeFrequency = sizeFrequency;
                        return Json(newList, new System.Text.Json.JsonSerializerOptions());
                    }
                    return Json(listGeneral.OrderBy(e => e.low).ToList(), new System.Text.Json.JsonSerializerOptions());
                }
            }
            return Json("");
        }
        }
}
