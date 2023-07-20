using FrekvencijeProject.Models;
using FrekvencijeProject.Models.Interfaces;
using FrekvencijeProject.Models.RightOfUse;
using GemBox.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FrekvencijeProject.Controllers
{
    public class RightOfUseController : Controller
    {
        object user;


        IConfiguration configuration;

        private readonly ApplicationDBContext _conApp;
        private readonly AllocationDBContext _conAll;
        
        private readonly ImportTempRightOfUseDBContext _conRightOfUse;

        [Display(Name = "Freq. Range")]
        public string Frequency { get; set; }

        [Display(Name = "From")]
        public string FromText { get; set; }
        public string From { get; set; }

        [Display(Name = "To")]
        public string ToText { get; set; }

        public string To { get; set; }


        public string FrequencySizeValue { get; set; }

        public List<SelectListItem> FrequencySizesList = new List<SelectListItem>
        {
            new SelectListItem { Text = "Hz", Value = "1" },
            new SelectListItem { Text = "kHz", Value = "2" },
            new SelectListItem { Text = "MHz", Value = "3", Selected = true},
            new SelectListItem { Text = "GHz", Value = "4" }
        };


        public string FrequencytableValue { get; set; }

        public List<SelectListItem> FrequencyTablesList = new List<SelectListItem>
        {

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
            new SelectListItem { Text = "Qatar", Value = "16" },
            new SelectListItem { Text = "Saudi Arabia", Value = "17" },
            new SelectListItem { Text = "Somalia", Value = "18" },
            new SelectListItem { Text = "Sudan", Value = "19" },
            new SelectListItem { Text = "Syria", Value = "20" },
            new SelectListItem { Text = "Tunisia", Value = "21" },
            new SelectListItem { Text = "United Arab Emirates", Value = "22" },
            new SelectListItem { Text = "Yemen", Value = "23" }

        };


        public List<RightOfUseView> RightOfUseList = new List<RightOfUseView>();

        public string ApplicationAllValue { get; set; }


        [Display(Name = "Frequency Table")]
        public string FrequencyTable { get; set; }


        [HttpPost]
        public JsonResult OrderByHierarchical()
        {
            var values = _conApp.RootApplicationTermsDB.Where(x => x.Number != null).ToList();
            var newApplicationOrderBy = values.OrderBy(x => x.Number);
            return Json(newApplicationOrderBy.ToList(), new System.Text.Json.JsonSerializerOptions());
        }

        [HttpPost]
        public JsonResult OrderByAlphabetical()
        {
            var values = _conApp.RootApplicationTermsDB.Where(x => x.Number != null).ToList();
            var newApplicationOrderBy = values.OrderBy(x => x.name);
            return Json(newApplicationOrderBy.ToList(), new System.Text.Json.JsonSerializerOptions());
        }

        public IActionResult Index()
        {
            if (TempData["rightOfUse"] != null)
            {
                bool value = (bool)TempData["rightOfUse"];
                if (value == true)
                {

                    string from = (string)TempData["tempFrom"];
                    string to = (string)TempData["tempTo"];


                    long valueTo = long.Parse(to);
                    long valueFrom = long.Parse(from);
                    FrequencySizeValue = (string)TempData["FrequencySize"];
                    if (FrequencySizeValue == "1")
                    {
                        TempData["toTempValue"] = valueTo;
                        TempData["fromTempValue"] = valueFrom;

                    }
                    else if (FrequencySizeValue == "2")
                    {
                        //1000.0 khz
                        long result = valueTo / 1000;
                        TempData["toTempValue"] = result;

                        long resultFrom = valueFrom / 1000;
                        TempData["fromTempValue"] = resultFrom;

                    }
                    else if (FrequencySizeValue == "3")
                    {

                        //1000000.0 mhz
                        long result = valueTo / 1000000;
                        TempData["toTempValue"] = result;


                        long resultFrom = valueFrom / 1000000;
                        TempData["fromTempValue"] = resultFrom;
                    }
                    else if (FrequencySizeValue == "4")
                    {
                        //1000000000.0 GHz
                        long result = valueTo / 1000000000;
                        TempData["toTempValue"] = result;

                        long resultFrom = valueFrom / 1000000000;
                        TempData["fromTempValue"] = resultFrom;
                    }

                    this.FrequencytableValue = (string)TempData["FrequencytableValue"];

                    //string tempValueFreq = FrequencyTablesList.Where(p => p.Value.Equals(this.FrequencytableValue)).First().Text;


                    FrequencyTablesList.FirstOrDefault(p => p.Value.Equals(this.FrequencytableValue)).Selected = true;

                    FrequencySizesList.FirstOrDefault(p => p.Value.Equals(this.FrequencySizeValue)).Selected = true;
                    //Debug.WriteLine("www inside:"+valueTo+"=="+ this.FrequencytableValue);

                    var listOfRightOfUse = _conRightOfUse.ImportTempRightOfUse.ToList();

                    foreach (var tempRightOfUse in listOfRightOfUse)
                    {

                        RightOfUseView right = new RightOfUseView();
                        right.Country = tempRightOfUse.Country;
                        right.Duplex = tempRightOfUse.Duplex;
                        right.FrequencyBand = "DL:"+ tempRightOfUse.DownLowerFrequency +"-"+tempRightOfUse.DownUpperFrequency + " UL:"+tempRightOfUse.UpLinkLowerFrequency+"-"+tempRightOfUse.UpLinkUpperFrequency;
                        right.Application = tempRightOfUse.Application;
                        right.Technology = tempRightOfUse.Technology;
                        right.LicenceHolder = tempRightOfUse.LicenceHolder;
                        right.LicenceHolderLink = tempRightOfUse.LicenceHolderLink;
                        right.Location = tempRightOfUse.Location;
                        right.LocationLink = tempRightOfUse.LocationLink;
                        right.StartDate = tempRightOfUse.StartDate;
                        right.ExpiryDate = tempRightOfUse.ExpiryDate;
                        right.SpectrumTrading = tempRightOfUse.SpectrumTrading;
                        right.ShortComment = tempRightOfUse.ShortComment;
                        RightOfUseList.Add(right);
                    }
                    RightOfUseList[0].FrequencySizesList = FrequencySizesList;
                    RightOfUseList[0].FrequencyTablesList = FrequencyTablesList;
                    RightOfUseList[0].FrequencySizeValue = "" + FrequencySizeValue;
                }
            }
            return View(RightOfUseList);
            
        }


        [HttpPost]
        public JsonResult GeneralSearch(int FrequencySizeVal, string FrequencyTableVal)
        {
            var listOfRightOfUse = _conRightOfUse.ImportTempRightOfUse.ToList();
            var tempFreq = FrequencyTablesList.Where(p => p.Text.Equals(FrequencyTableVal)).First().Value;
            FrequencyTablesList.FirstOrDefault(p => p.Value.Equals(tempFreq)).Selected = true;

            RightOfUseList.Clear();
            foreach (var tempRightOfUse in listOfRightOfUse)
            {
                //Debug.WriteLine("im here:" + tempInterfaces.Application+"=="+tempInterfaces.RadiocommunicationService);
                RightOfUseView right = new RightOfUseView();
                right.Country = tempRightOfUse.Country;
                right.Duplex = tempRightOfUse.Duplex;
                right.FrequencyBand = "DL:" + tempRightOfUse.DownLowerFrequency + "-" + tempRightOfUse.DownUpperFrequency + " UL:" + tempRightOfUse.UpLinkLowerFrequency + "-" + tempRightOfUse.UpLinkUpperFrequency;
                right.Application = tempRightOfUse.Application;
                right.Technology = tempRightOfUse.Technology;
                right.LicenceHolder = tempRightOfUse.LicenceHolder;
                right.LicenceHolderLink = tempRightOfUse.LicenceHolderLink;
                right.Location = tempRightOfUse.Location;
                right.LocationLink = tempRightOfUse.LocationLink;
                right.StartDate = tempRightOfUse.StartDate;
                right.ExpiryDate = tempRightOfUse.ExpiryDate;
                right.SpectrumTrading = tempRightOfUse.SpectrumTrading;
                right.ShortComment = tempRightOfUse.ShortComment;
                RightOfUseList.Add(right);
            }

            FrequencySizesList.FirstOrDefault(p => p.Value.Equals("" + FrequencySizeVal)).Selected = true;
            RightOfUseList[0].FrequencySizesList = FrequencySizesList;
            RightOfUseList[0].FrequencyTablesList = FrequencyTablesList;
            RightOfUseList[0].FrequencySizeValue = "" + FrequencySizeVal;
            Debug.WriteLine("count:"+ RightOfUseList.Count);
            return Json(RightOfUseList, new System.Text.Json.JsonSerializerOptions());
        }

        private byte[] GenerateExcel(List<RightOfUseView> rights)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                // Set column widths
                worksheet.Column(1).Width = 12;  // Column A 
                worksheet.Column(2).Width = 10;  // Column B
                worksheet.Column(3).Width = 30;  // Column C
                worksheet.Column(4).Width = 20;  // Column D
                worksheet.Column(5).Width = 20;  // Column E
                worksheet.Column(6).Width = 30;  // Column F
                worksheet.Column(7).Width = 30;  // Column G
                worksheet.Column(8).Width = 25;  // Column H
                worksheet.Column(9).Width = 20;  // Column I
                worksheet.Column(10).Width = 20;  // Column J

                // Merge cells and write title
                worksheet.Cells["A1:J2"].Merge = true;
                worksheet.Cells["A1"].Value = "Rights Of Use";
                worksheet.Cells["A1:J2"].Style.Font.Color.SetColor(Color.FromArgb(0x92, 0xD0, 0x50));
                worksheet.Cells["A1"].Style.Font.Size = 16;
                worksheet.Cells["A1"].Style.Font.Name = "Calibri";
                worksheet.Cells["A1"].Style.Font.Bold = true;
                worksheet.Cells["A1"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center; // Align in the middle

                //Format the headers for the table
                OfficeOpenXml.Style.ExcelStyle headerStyle;
                worksheet.Cells["A3"].Value = "Country";
                worksheet.Cells["B3"].Value = "Duplex";
                worksheet.Cells["C3"].Value = "Frequency band";
                worksheet.Cells["D3"].Value = "Application";
                worksheet.Cells["E3"].Value = "License holder";
                worksheet.Cells["F3"].Value = "Start date";
                worksheet.Cells["G3"].Value = "Expiry date";
                worksheet.Cells["H3"].Value = "Transmitter location";
                worksheet.Cells["I3"].Value = "Spectrum trading";
                worksheet.Cells["J3"].Value = "Short comments";
                headerStyle = worksheet.Cells["A3:J3"].Style;

                headerStyle.Font.Bold = true;
                headerStyle.Font.Italic = true;
                headerStyle.Font.Name = "Calibri";
                headerStyle.Font.Size = 14;
                headerStyle.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                headerStyle.Fill.BackgroundColor.SetColor(Color.FromArgb(0xC6, 0xE0, 0xB4));

                bool swap = true;

                int currentRow = 4;
                for (int i = 0; i < rights.Count(); i++) //iterate over the list of bands
                {
                    int firstRow = currentRow;
                    //postavlja se prvi red (vrijednosti za low i high)
                    worksheet.Cells["A" + currentRow].Value = rights[i].Country;
                    worksheet.Cells["B" + currentRow].Value = rights[i].Duplex;
                    worksheet.Cells["D" + currentRow].Value = rights[i].Application;

                    worksheet.Cells["E" + currentRow].Value = rights[i].LicenceHolder;
                    if (rights[i].LicenceHolderLink != "")
                    {
                        string url = rights[i].LicenceHolderLink;
                        if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                        {
                            url = "http://" + url;
                        }
                        worksheet.Cells["E" + currentRow].Hyperlink = new Uri(url);

                        var cellStyle = worksheet.Cells["E" + currentRow].Style;
                        cellStyle.Font.Color.SetColor(Color.Blue);
                        cellStyle.Font.UnderLine = true;
                    }

                    worksheet.Cells["F" + currentRow].Value = rights[i].StartDate;
                    worksheet.Cells["G" + currentRow].Value = rights[i].ExpiryDate;

                    worksheet.Cells["H" + currentRow].Value = rights[i].Location;
                    if (rights[i].LocationLink != "")
                    {
                        string url = rights[i].LocationLink;
                        if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                        {
                            url = "http://" + url;
                        }
                        worksheet.Cells["H" + currentRow].Hyperlink = new Uri(url);

                        var cellStyle = worksheet.Cells["H" + currentRow].Style;
                        cellStyle.Font.Color.SetColor(Color.Blue);
                        cellStyle.Font.UnderLine = true;
                    }

                    worksheet.Cells["I" + currentRow].Value = rights[i].SpectrumTrading;
                    worksheet.Cells["J" + currentRow].Value = rights[i].ShortComment;

                    int index = rights[i].FrequencyBand.IndexOf('U');
                    string part1 = rights[i].FrequencyBand;
                    worksheet.Cells["C" + currentRow].Value = part1;
                    if (index >= 0)
                    {
                        part1 = rights[i].FrequencyBand.Substring(0, index).Trim();
                        string part2 = rights[i].FrequencyBand.Substring(index).Trim();

                        worksheet.Cells["C" + currentRow].Value = part1;

                        currentRow++;
                        worksheet.Cells["C" + currentRow].Value = part2;
                    }

                    string cells = "";
                    //postavimo boju za ovaj red iz freqBanda
                    cells = "A" + firstRow + ":J" + currentRow;
                    worksheet.Cells[cells].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                    if (swap) //da bi tabela bila striped
                        worksheet.Cells[cells].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0xDD, 0xEB, 0xF7));
                    else
                        worksheet.Cells[cells].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0xFF, 0xFF, 0xFF));

                    worksheet.Cells[cells].Style.Font.Size = 11;
                    worksheet.Cells[cells].Style.Font.Name = "Calibri";

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

        private byte[] GenerateExcel150(List<RightOfUseView> rights)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                // Set column widths
                worksheet.Column(1).Width = 12;  // Column A 
                worksheet.Column(2).Width = 10;  // Column B
                worksheet.Column(3).Width = 30;  // Column C
                worksheet.Column(4).Width = 20;  // Column D
                worksheet.Column(5).Width = 20;  // Column E
                worksheet.Column(6).Width = 30;  // Column F
                worksheet.Column(7).Width = 30;  // Column G
                worksheet.Column(8).Width = 25;  // Column H
                worksheet.Column(9).Width = 20;  // Column I
                worksheet.Column(10).Width = 20;  // Column J

                // Merge cells and write title
                worksheet.Cells["A1:J2"].Merge = true;
                worksheet.Cells["A1"].Value = "Rights Of Use";
                worksheet.Cells["A1:J2"].Style.Font.Color.SetColor(Color.FromArgb(0x92, 0xD0, 0x50));
                worksheet.Cells["A1"].Style.Font.Size = 16;
                worksheet.Cells["A1"].Style.Font.Name = "Calibri";
                worksheet.Cells["A1"].Style.Font.Bold = true;
                worksheet.Cells["A1"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center; // Align in the middle

                //Format the headers for the table
                OfficeOpenXml.Style.ExcelStyle headerStyle;
                worksheet.Cells["A3"].Value = "Country";
                worksheet.Cells["B3"].Value = "Duplex";
                worksheet.Cells["C3"].Value = "Frequency band";
                worksheet.Cells["D3"].Value = "Application";
                worksheet.Cells["E3"].Value = "License holder";
                worksheet.Cells["F3"].Value = "Start date";
                worksheet.Cells["G3"].Value = "Expiry date";
                worksheet.Cells["H3"].Value = "Transmitter location";
                worksheet.Cells["I3"].Value = "Spectrum trading";
                worksheet.Cells["J3"].Value = "Short comments";
                headerStyle = worksheet.Cells["A3:J3"].Style;

                headerStyle.Font.Bold = true;
                headerStyle.Font.Italic = true;
                headerStyle.Font.Name = "Calibri";
                headerStyle.Font.Size = 14;
                headerStyle.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                headerStyle.Fill.BackgroundColor.SetColor(Color.FromArgb(0xC6, 0xE0, 0xB4));

                bool swap = true;

                int currentRow = 4;
                for (int i = 0; i < rights.Count(); i++) //iterate over the list of bands
                {
                    int firstRow = currentRow;
                    //postavlja se prvi red (vrijednosti za low i high)
                    worksheet.Cells["A" + currentRow].Value = rights[i].Country;
                    worksheet.Cells["B" + currentRow].Value = rights[i].Duplex;
                    worksheet.Cells["D" + currentRow].Value = rights[i].Application;

                    worksheet.Cells["E" + currentRow].Value = rights[i].LicenceHolder;
                    if (rights[i].LicenceHolderLink != "")
                    {
                        string url = rights[i].LicenceHolderLink;
                        if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                        {
                            url = "http://" + url;
                        }
                        worksheet.Cells["E" + currentRow].Hyperlink = new Uri(url);

                        var cellStyle = worksheet.Cells["E" + currentRow].Style;
                        cellStyle.Font.Color.SetColor(Color.Blue);
                        cellStyle.Font.UnderLine = true;
                    }

                    worksheet.Cells["F" + currentRow].Value = rights[i].StartDate;
                    worksheet.Cells["G" + currentRow].Value = rights[i].ExpiryDate;

                    worksheet.Cells["H" + currentRow].Value = rights[i].Location;
                    if (rights[i].LocationLink != "")
                    {
                        string url = rights[i].LocationLink;
                        if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                        {
                            url = "http://" + url;
                        }
                        worksheet.Cells["H" + currentRow].Hyperlink = new Uri(url);

                        var cellStyle = worksheet.Cells["H" + currentRow].Style;
                        cellStyle.Font.Color.SetColor(Color.Blue);
                        cellStyle.Font.UnderLine = true;
                    }

                    worksheet.Cells["I" + currentRow].Value = rights[i].SpectrumTrading;
                    worksheet.Cells["J" + currentRow].Value = rights[i].ShortComment;

                    int index = rights[i].FrequencyBand.IndexOf('U');
                    string part1 = rights[i].FrequencyBand;
                    worksheet.Cells["C" + currentRow].Value = part1;
                    if (index >= 0)
                    {
                        part1 = rights[i].FrequencyBand.Substring(0, index).Trim();
                        string part2 = rights[i].FrequencyBand.Substring(index).Trim();

                        worksheet.Cells["C" + currentRow].Value = part1;

                        currentRow++;
                        worksheet.Cells["C" + currentRow].Value = part2;
                    }

                    string cells = "";
                    //postavimo boju za ovaj red iz freqBanda
                    cells = "A" + firstRow + ":J" + currentRow;
                    worksheet.Cells[cells].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                    if (swap) //da bi tabela bila striped
                        worksheet.Cells[cells].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0xDD, 0xEB, 0xF7));
                    else
                        worksheet.Cells[cells].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0xFF, 0xFF, 0xFF));

                    worksheet.Cells[cells].Style.Font.Size = 11;
                    worksheet.Cells[cells].Style.Font.Name = "Calibri";

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
        [Route("RightOfUse/DownloadExcel")]
        public byte[] DownloadExcel(int FrequencySizeVal, string FrequencyTableVal)
        {
            JsonResult rights = GeneralSearch(FrequencySizeVal, FrequencyTableVal);
            string jsonResult = JsonConvert.SerializeObject(rights.Value);
            List<RightOfUseView> result = JsonConvert.DeserializeObject<List<RightOfUseView>>(jsonResult);
            return GenerateExcel(result);
        }

        //[EnableQuery]
        [HttpPost]
        [Route("RightOfUse/DownloadPDF")]
        public byte[] DownloadPDF(int FrequencySizeVal, string FrequencyTableVal)
        {
            // List<AsiaPacific> allocs = getPrintData(AllVal1, AllVal2, AllVal3, AllVal4, FromVal, ToVal, FrequencySizeVal, FreqTable, FreqTableVal, "");
            JsonResult interfaces = GeneralSearch(FrequencySizeVal, FrequencyTableVal);
            string jsonResult = JsonConvert.SerializeObject(interfaces.Value);
            List<RightOfUseView> result = JsonConvert.DeserializeObject<List<RightOfUseView>>(jsonResult);
            byte[] excelBytes = GenerateExcel150(result);

            SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
            var workbook = ExcelFile.Load(new MemoryStream(excelBytes));

            var limitedWorkbook = new ExcelFile();

            var worksheet = workbook.Worksheets[0];
            var limitedWorksheet = limitedWorkbook.Worksheets.AddCopy("name", worksheet);

            using (MemoryStream pdfStream = new MemoryStream())
            {
                limitedWorksheet.PrintOptions.FitWorksheetWidthToPages = 1;
                limitedWorkbook.Save(pdfStream, SaveOptions.PdfDefault);

                byte[] pdfByteArray = pdfStream.ToArray();

                return pdfByteArray;
            }

            return null;
        }


        public RightOfUseController(
       IHttpContextAccessor httpContextAccessor,
        ApplicationDBContext conApp, IConfiguration configuration, AllocationDBContext conAll,
        ImportTempRightOfUseDBContext conRightOfUse)
        {

            user = httpContextAccessor.HttpContext.User;
            _conApp = conApp;
            this.configuration = configuration;
            _conAll = conAll;
            _conRightOfUse = conRightOfUse;

        }


       
     }
}
