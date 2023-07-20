using FrekvencijeProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FrekvencijeProject.Models.SRD;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System;
using FrekvencijeProject.Models.ExcelModels;
using GemBox.Spreadsheet;
using OfficeOpenXml;
using System.Drawing;
using System.IO;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using NHibernate.Util;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;

namespace FrekvencijeProject.Controllers
{
    public class SRDController : Controller
    {
        object user;
        private readonly SRDContext _conSRD;
        IConfiguration _configuration;

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

        public string AnnexValue { get; set; }

        public List<SelectListItem> AnnexList = new List<SelectListItem>
        {
            new SelectListItem { Text = "Annex 1 - Non-specific Short Range Devices", Value = "1" },
            new SelectListItem { Text = "Annex 2 - Tracking, Tracing and Data acquisition", Value = "2" },
            new SelectListItem { Text = "Annex 3 - Wideband Data Transmission systems", Value = "3" },
            new SelectListItem { Text = "Annex 4 - Railway applications", Value = "4" },
            new SelectListItem { Text = "Annex 5 - Transport and Traffic Telematics (TTT)", Value = "5" },
            new SelectListItem { Text = "Annex 6 - Radiodetermination applications", Value = "6" },
            new SelectListItem { Text = "Annex 7 - Alarms", Value = "7" },
            new SelectListItem { Text = "Annex 8 - Model Control", Value = "8" },
            new SelectListItem { Text = "Annex 9 - Inductive applications", Value = "9" },
            new SelectListItem { Text = "Annex 10 - Radio microphones applications", Value = "10" },
            new SelectListItem { Text = "Annex 11 - Radio frequency identification applications", Value = "11" },
            new SelectListItem { Text = "Annex 12 - Active Medical Implants and their associated peripherals", Value = "12" },
            new SelectListItem { Text = "Annex 13 - Medical data acquisition", Value = "13" },
            new SelectListItem { Text = "Annex A - The applications not covered by the Annexes 1 to 13", Value = "14" }
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

        public SRDController(IHttpContextAccessor httpContextAccessor, IConfiguration configuration,
        SRDContext conSRD, ImportTempTableContext conImport)
        {
            user = httpContextAccessor.HttpContext.User;
            _conSRD = conSRD;
            _configuration = configuration;
        }



        [HttpPost]
        [Route("SRD/SearchNow")]
        public JsonResult SearchNow(string FromVal, string ToVal, int FrequencySizeVal, int FrequencyTableVal, int AnnexVal)
        {
            List<SRDSearch> lst = GetResults(FromVal, ToVal, FrequencySizeVal, FrequencyTableVal, AnnexVal);
            return Json(lst.ToList(), new System.Text.Json.JsonSerializerOptions());
        }

        private List<string> transformRows(string temp, int val)
        {
            List<string> rows = new List<string>();
            if (temp == null || temp.Length == 0) {
                rows.Add("");
                return rows;
            }

            while (temp.Length > val)
            {
                int lastSpaceIndex = temp.Substring(0, val).LastIndexOf(' ');
                if (lastSpaceIndex <= 0)
                {
                    lastSpaceIndex = val;
                }

                rows.Add(temp.Substring(0, lastSpaceIndex));
                temp = temp.Substring(lastSpaceIndex).TrimStart();
            }

            rows.Add(temp); //add the rows that are left, or the whole string if it was less than 30
            return rows;
        }

        private byte[] GenerateExcel(string FromVal, string ToVal, int FrequencySizeVal, int FrequencyTableVal, int AnnexVal)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                // Set column widths
                worksheet.Column(1).Width = 5;  // Column A 
                worksheet.Column(2).Width = 22;  // Column B
                worksheet.Column(3).Width = 30;  // Column C
                worksheet.Column(4).Width = 30;  // Column D
                worksheet.Column(5).Width = 30;  // Column E
                worksheet.Column(6).Width = 30;  // Column F
                worksheet.Column(7).Width = 30;  // Column G

                // Merge cells and write title
                worksheet.Cells["A1:G2"].Merge = true;
                worksheet.Cells["A1"].Value = "Short Range Devices";
                worksheet.Cells["A1:G2"].Style.Font.Color.SetColor(Color.FromArgb(0x92, 0xD0, 0x50));
                worksheet.Cells["A1"].Style.Font.Size = 16;
                worksheet.Cells["A1"].Style.Font.Name = "Calibri";
                worksheet.Cells["A1"].Style.Font.Bold = true;
                worksheet.Cells["A1"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center; // Align in the middle

                //Format the headers for the table
                OfficeOpenXml.Style.ExcelStyle headerStyle;
                worksheet.Cells["A3"].Value = "Id";
                worksheet.Cells["B3"].Value = "Frequency band";
                worksheet.Cells["C3"].Value = "Power / Magnetic Field";
                worksheet.Cells["D3"].Value = "Spectrum access and mitigation requirements";
                worksheet.Cells["E3"].Value = "Modulation/Bandwidth";
                worksheet.Cells["F3"].Value = "ECC/ERC";
                worksheet.Cells["G3"].Value = "Notes";

                headerStyle = worksheet.Cells["A3:G3"].Style;

                headerStyle.Font.Bold = true;
                headerStyle.Font.Italic = true;
                headerStyle.Font.Name = "Calibri";
                headerStyle.Font.Size = 14;
                headerStyle.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                headerStyle.Fill.BackgroundColor.SetColor(Color.FromArgb(0xC6, 0xE0, 0xB4));

                string[] notes = new string[8];
                List<Issue> issues = new List<Issue>();
                List<Std> stds = new List<Std>();
                List<Param> param = new List<Param>();

                bool swap = true;

                List<SRDSearch> lst = GetResults(FromVal, ToVal, FrequencySizeVal, FrequencyTableVal, AnnexVal);

                int currentRow = 4;
                for (int i = 0; i < lst.Count(); i++) //iterate over the list of bands
                {
                    int firstRow = currentRow;
                    int maxRow = currentRow;
                    int tempRow = currentRow;

                    //obrada listi
                    if (notes[0] == null && lst[i].Note1 != "")
                    {
                        notes[0] = lst[i].Note1;
                    }

                    if (notes[1] == null && lst[i].Note2 != "")
                    {
                        notes[1] = lst[i].Note2;
                    }
                    if (notes[2] == null && lst[i].Note3 != "")
                    {
                        notes[2] = lst[i].Note3;
                    }
                    if (notes[3] == null && lst[i].Note4 != "")
                    {
                        notes[3] = lst[i].Note4;
                    }
                    if (notes[4] == null && lst[i].Note5 != "")
                    {
                        notes[4] = lst[i].Note5;
                    }
                    if (notes[5] == null && lst[i].Note6 != "")
                    {
                        notes[5] = lst[i].Note6;
                    }
                    if (notes[6] == null && lst[i].Note7 != "")
                    {
                        notes[6] = lst[i].Note7;
                    }
                    if (notes[7] == null && lst[i].Note8 != "")
                    {
                        notes[7] = lst[i].Note8;
                    }

                    //sum standards
                    foreach (string standard in lst[i].Standards)
                    {
                        Std existingStd = stds.FirstOrDefault(std => std.Name == standard);

                        if (existingStd != null)
                        {
                            // If an Std object already exists, add the identifier to its list of identifiers
                            existingStd.ids.Add(lst[i].Identifier);
                        }
                        else if (standard != "")
                        {
                            // If no Std object exists, create a new one and add it to the list with the identifier
                            Std newStd = new Std
                            {
                                Name = standard,
                                ids = new List<string> { lst[i].Identifier }
                            };
                            stds.Add(newStd);
                        }
                    }

                    //sum issues
                    Issue existingIssue = issues.FirstOrDefault(iss => iss.Name == lst[i].FreqIssue);

                    if (existingIssue != null)
                    {
                        // If an Std object already exists, add the identifier to its list of identifiers
                        existingIssue.ids.Add(lst[i].Identifier);
                    }
                    else if (lst[i].FreqIssue != "")
                    {
                        // If no Std object exists, create a new one and add it to the list with the identifier
                        Issue newIss = new Issue
                        {
                            Name = lst[i].FreqIssue,
                            ids = new List<string> { lst[i].Identifier }
                        };
                        issues.Add(newIss);
                    }

                    Param existingParam = param.FirstOrDefault(par => par.text == lst[i].stdParams);

                    if (existingParam == null && lst[i].stdParams != "")
                    {
                        Param newParam = new Param
                        {
                            text = lst[i].stdParams,
                            standard = lst[i].Standards.First()
                        };
                        param.Add(newParam);
                    }

                    //postavlja se prvi red (vrijednosti za low i high)
                    worksheet.Cells["A" + currentRow].Value = lst[i].Identifier;
                    worksheet.Cells["B" + currentRow].Value = lst[i].LowView + " - " + lst[i].HighView;

                    foreach (string str in transformRows(lst[i].Power, 34))
                    {
                        worksheet.Cells["C" + tempRow].Value = str;
                        tempRow++;
                    }
                    if (tempRow > maxRow) maxRow = tempRow;
                    tempRow = currentRow;

                    foreach (string str in transformRows(lst[i].Spectrum, 34))
                    {
                        worksheet.Cells["D" + tempRow].Value = str;
                        tempRow++;
                    }
                    if (tempRow > maxRow) maxRow = tempRow;
                    tempRow = currentRow;

                    foreach (string str in transformRows(lst[i].Modulation, 34))
                    {
                        worksheet.Cells["E" + tempRow].Value = str;
                        tempRow++;
                    }
                    if (tempRow > maxRow) maxRow = tempRow;
                    tempRow = currentRow;

                    worksheet.Cells["F" + currentRow].Value = lst[i].ECC_ERC;
                    if (lst[i].ECC_ERCLink != "")
                    {
                        string url = lst[i].ECC_ERCLink;
                        if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                        {
                            url = "http://" + url;
                        }
                        worksheet.Cells["F" + currentRow].Hyperlink = new Uri(url);

                        var cellStyle = worksheet.Cells["F" + currentRow].Style;
                        cellStyle.Font.Color.SetColor(Color.Blue);
                        cellStyle.Font.UnderLine = true;
                    }

                    foreach (string str in transformRows(lst[i].BandNote, 34))
                    {
                        worksheet.Cells["G" + tempRow].Value = str;
                        tempRow++;
                    }
                    if (tempRow > maxRow) maxRow = tempRow;
                    tempRow = currentRow;

                    //postavimo boju za ovaj red iz freqBanda
                    string cells = "A" + firstRow + ":G" + maxRow;
                    worksheet.Cells[cells].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                    if (swap) //da bi tabela bila striped
                        worksheet.Cells[cells].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0xDD, 0xEB, 0xF7));
                    else
                        worksheet.Cells[cells].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0xFF, 0xFF, 0xFF));

                    worksheet.Cells[cells].Style.Font.Size = 11;
                    worksheet.Cells[cells].Style.Font.Name = "Calibri";

                    swap = !swap;
                    currentRow = maxRow + 1;
                }
                currentRow++;

                //rest of the data
                //Format the headers for the table
                worksheet.Cells["A" + currentRow + ":B" + currentRow].Merge = true;
                worksheet.Cells["C" + currentRow + ":G" + currentRow].Merge = true;

                OfficeOpenXml.Style.ExcelStyle headerStyleStds;
                worksheet.Cells["A" + currentRow].Value = "Harmonized Standard";
                worksheet.Cells["C" + currentRow].Value = "Identifiers";


                headerStyleStds = worksheet.Cells["A" + currentRow + ":C" + currentRow].Style;

                headerStyleStds.Font.Bold = true;
                headerStyleStds.Font.Italic = true;
                headerStyleStds.Font.Name = "Calibri";
                headerStyleStds.Font.Size = 14;
                headerStyleStds.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                headerStyleStds.Fill.BackgroundColor.SetColor(Color.FromArgb(0xC6, 0xE0, 0xB4));
                currentRow++;

                foreach (Std std in stds)
                {
                    worksheet.Cells["A" + currentRow].Value = std.Name;

                    string ids = "";
                    foreach (string id in std.ids)
                    {
                        ids += id + " ";
                    }

                    worksheet.Cells["C" + currentRow].Value = ids;
                    currentRow++;
                }

                currentRow++;


                //rest of the data
                //Format the headers for the table
                worksheet.Cells["A" + currentRow + ":G" + currentRow].Merge = true;

                OfficeOpenXml.Style.ExcelStyle headerStyleParams;
                worksheet.Cells["A" + currentRow].Value = "Technical parameters also referred to in the harmonised standard";


                headerStyleParams = worksheet.Cells["A" + currentRow + ":G" + currentRow].Style;

                headerStyleParams.Font.Bold = true;
                headerStyleParams.Font.Italic = true;
                headerStyleParams.Font.Name = "Calibri";
                headerStyleParams.Font.Size = 14;
                headerStyleParams.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                headerStyleParams.Fill.BackgroundColor.SetColor(Color.FromArgb(0xC6, 0xE0, 0xB4));
                currentRow++;

                foreach (Param par in param)
                {
                    foreach (string str in transformRows(par.text, 150))
                    {
                        worksheet.Cells["A" + currentRow].Value = str;
                        currentRow++;
                    }
                    currentRow++;
                }

                currentRow++;

                //rest of the data
                //Format the headers for the table
                worksheet.Cells["A" + currentRow + ":C" + currentRow].Merge = true;
                worksheet.Cells["D" + currentRow + ":G" + currentRow].Merge = true;

                OfficeOpenXml.Style.ExcelStyle headerStyleIss;
                worksheet.Cells["A" + currentRow].Value = "Frequency Issue";
                worksheet.Cells["D" + currentRow].Value = "Identifiers";


                headerStyleIss = worksheet.Cells["A" + currentRow + ":G" + currentRow].Style;

                headerStyleIss.Font.Bold = true;
                headerStyleIss.Font.Italic = true;
                headerStyleIss.Font.Name = "Calibri";
                headerStyleIss.Font.Size = 14;
                headerStyleIss.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                headerStyleIss.Fill.BackgroundColor.SetColor(Color.FromArgb(0xC6, 0xE0, 0xB4));
                currentRow++;

                foreach (Issue iss in issues)
                {
                    string ids = "";
                    foreach (string id in iss.ids)
                    {
                        ids += id + " ";
                    }
                    worksheet.Cells["D" + currentRow].Value = ids;

                    worksheet.Cells["A" + currentRow].Value = iss.Name;
                    foreach (string str in transformRows(iss.Name, 65))
                    {
                        worksheet.Cells["A" + currentRow].Value = str;
                        currentRow++;
                    }

                    currentRow++;
                }

                currentRow++;

                //rest of the data
                //Format the headers for the table
                worksheet.Cells["B" + currentRow + ":G" + currentRow].Merge = true;

                OfficeOpenXml.Style.ExcelStyle headerStyleNotes;
                worksheet.Cells["A" + currentRow].Value = "No.";
                worksheet.Cells["B" + currentRow].Value = "Note";


                headerStyleIss = worksheet.Cells["A" + currentRow + ":G" + currentRow].Style;

                headerStyleIss.Font.Bold = true;
                headerStyleIss.Font.Italic = true;
                headerStyleIss.Font.Name = "Calibri";
                headerStyleIss.Font.Size = 14;
                headerStyleIss.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                headerStyleIss.Fill.BackgroundColor.SetColor(Color.FromArgb(0xC6, 0xE0, 0xB4));
                currentRow++;

                int ind = 1;
                foreach (string note in notes)
                {
                    if (note == null) continue;
                    worksheet.Cells["A" + currentRow].Value = ind++;
                    foreach (string str in transformRows(note, 150))
                    {
                        worksheet.Cells["B" + currentRow].Value = str;
                        currentRow++;
                    }

                    currentRow++;
                }

                currentRow++;


                var memoryStream = new MemoryStream();
                package.SaveAs(memoryStream);
                memoryStream.Position = 0;

                var fileBytes = memoryStream.ToArray();
                return fileBytes;
            }
        }

        private class Std
        {
            public string Name { get; set; }

            public List<string> ids { get; set; }

            public Std()
            {
                ids = new List<string>();
            }
        }

        private class Issue
        {
            public string Name { get; set; }

            public List<string> ids { get; set; }

            public Issue()
            {
                ids = new List<string>();
            }
        }

        private class Param
        {
            public string text { get; set; }

            public string standard { get; set; }
        }

        private byte[] GenerateExcel150(string FromVal, string ToVal, int FrequencySizeVal, int FrequencyTableVal, int AnnexVal)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                // Set column widths
                worksheet.Column(1).Width = 5;  // Column A 
                worksheet.Column(2).Width = 22;  // Column B
                worksheet.Column(3).Width = 30;  // Column C
                worksheet.Column(4).Width = 30;  // Column D
                worksheet.Column(5).Width = 30;  // Column E
                worksheet.Column(6).Width = 30;  // Column F
                worksheet.Column(7).Width = 30;  // Column G

                // Merge cells and write title
                worksheet.Cells["A1:G2"].Merge = true;
                worksheet.Cells["A1"].Value = "Short Range Devices";
                worksheet.Cells["A1:G2"].Style.Font.Color.SetColor(Color.FromArgb(0x92, 0xD0, 0x50));
                worksheet.Cells["A1"].Style.Font.Size = 16;
                worksheet.Cells["A1"].Style.Font.Name = "Calibri";
                worksheet.Cells["A1"].Style.Font.Bold = true;
                worksheet.Cells["A1"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center; // Align in the middle

                //Format the headers for the table
                OfficeOpenXml.Style.ExcelStyle headerStyle;
                worksheet.Cells["A3"].Value = "Id";
                worksheet.Cells["B3"].Value = "Frequency band";
                worksheet.Cells["C3"].Value = "Power / Magnetic Field";
                worksheet.Cells["D3"].Value = "Spectrum access and mitigation requirements";
                worksheet.Cells["E3"].Value = "Modulation/Bandwidth";
                worksheet.Cells["F3"].Value = "ECC/ERC";
                worksheet.Cells["G3"].Value = "Notes";

                headerStyle = worksheet.Cells["A3:G3"].Style;

                headerStyle.Font.Bold = true;
                headerStyle.Font.Italic = true;
                headerStyle.Font.Name = "Calibri";
                headerStyle.Font.Size = 14;
                headerStyle.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                headerStyle.Fill.BackgroundColor.SetColor(Color.FromArgb(0xC6, 0xE0, 0xB4));

                string[] notes = new string[8];
                List<Issue> issues = new List<Issue>();
                List<Std> stds = new List<Std>();
                List<Param> param = new List<Param>();

                bool swap = true;

                List<SRDSearch> lst = GetResults(FromVal, ToVal, FrequencySizeVal, FrequencyTableVal, AnnexVal);

                int currentRow = 4;
                for (int i = 0; i < lst.Count(); i++) //iterate over the list of bands
                {
                    if (currentRow > 40) break;
                    int firstRow = currentRow;
                    int maxRow = currentRow;
                    int tempRow = currentRow;

                    //obrada listi
                    if (notes[0] == null && lst[i].Note1 != "")
                    {
                        notes[0] = lst[i].Note1;
                    }

                    if (notes[1] == null && lst[i].Note2 != "")
                    {
                        notes[1] = lst[i].Note2;
                    }
                    if (notes[2] == null && lst[i].Note3 != "")
                    {
                        notes[2] = lst[i].Note3;
                    }
                    if (notes[3] == null && lst[i].Note4 != "")
                    {
                        notes[3] = lst[i].Note4;
                    }
                    if (notes[4] == null && lst[i].Note5 != "")
                    {
                        notes[4] = lst[i].Note5;
                    }
                    if (notes[5] == null && lst[i].Note6 != "")
                    {
                        notes[5] = lst[i].Note6;
                    }
                    if (notes[6] == null && lst[i].Note7 != "")
                    {
                        notes[6] = lst[i].Note7;
                    }
                    if (notes[7] == null && lst[i].Note8 != "")
                    {
                        notes[7] = lst[i].Note8;
                    }

                    //sum standards
                    foreach (string standard in lst[i].Standards)
                    {
                        Std existingStd = stds.FirstOrDefault(std => std.Name == standard);

                        if (existingStd != null)
                        {
                            // If an Std object already exists, add the identifier to its list of identifiers
                            existingStd.ids.Add(lst[i].Identifier);
                        }
                        else if (standard != "")
                        {
                            // If no Std object exists, create a new one and add it to the list with the identifier
                            Std newStd = new Std
                            {
                                Name = standard,
                                ids = new List<string> { lst[i].Identifier }
                            };
                            stds.Add(newStd);
                        }
                    }

                    //sum issues
                    Issue existingIssue = issues.FirstOrDefault(iss => iss.Name == lst[i].FreqIssue);

                    if (existingIssue != null)
                    {
                        // If an Std object already exists, add the identifier to its list of identifiers
                        existingIssue.ids.Add(lst[i].Identifier);
                    }
                    else if (lst[i].FreqIssue != "")
                    {
                        // If no Std object exists, create a new one and add it to the list with the identifier
                        Issue newIss = new Issue
                        {
                            Name = lst[i].FreqIssue,
                            ids = new List<string> { lst[i].Identifier }
                        };
                        issues.Add(newIss);
                    }

                    Param existingParam = param.FirstOrDefault(par => par.text == lst[i].stdParams);

                    if (existingParam == null && lst[i].stdParams != "")
                    {
                        Param newParam = new Param
                        {
                            text = lst[i].stdParams,
                            standard = lst[i].Standards.First()
                        };
                        param.Add(newParam);
                    }

                    //postavlja se prvi red (vrijednosti za low i high)
                    worksheet.Cells["A" + currentRow].Value = lst[i].Identifier;
                    worksheet.Cells["B" + currentRow].Value = lst[i].LowView + " - " + lst[i].HighView;

                    foreach (string str in transformRows(lst[i].Power, 34))
                    {
                        worksheet.Cells["C" + tempRow].Value = str;
                        tempRow++;
                    }
                    if (tempRow > maxRow) maxRow = tempRow;
                    tempRow = currentRow;

                    foreach (string str in transformRows(lst[i].Spectrum, 34))
                    {
                        worksheet.Cells["D" + tempRow].Value = str;
                        tempRow++;
                    }
                    if (tempRow > maxRow) maxRow = tempRow;
                    tempRow = currentRow;

                    foreach (string str in transformRows(lst[i].Modulation, 34))
                    {
                        worksheet.Cells["E" + tempRow].Value = str;
                        tempRow++;
                    }
                    if (tempRow > maxRow) maxRow = tempRow;
                    tempRow = currentRow;

                    worksheet.Cells["F" + currentRow].Value = lst[i].ECC_ERC;
                    if (lst[i].ECC_ERCLink != "")
                    {
                        string url = lst[i].ECC_ERCLink;
                        if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                        {
                            url = "http://" + url;
                        }
                        worksheet.Cells["F" + currentRow].Hyperlink = new Uri(url);

                        var cellStyle = worksheet.Cells["F" + currentRow].Style;
                        cellStyle.Font.Color.SetColor(Color.Blue);
                        cellStyle.Font.UnderLine = true;
                    }

                    foreach (string str in transformRows(lst[i].BandNote, 34))
                    {
                        worksheet.Cells["G" + tempRow].Value = str;
                        tempRow++;
                    }
                    if (tempRow > maxRow) maxRow = tempRow;
                    tempRow = currentRow;

                    //postavimo boju za ovaj red iz freqBanda
                    string cells = "A" + firstRow + ":G" + maxRow;
                    worksheet.Cells[cells].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                    if (swap) //da bi tabela bila striped
                        worksheet.Cells[cells].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0xDD, 0xEB, 0xF7));
                    else
                        worksheet.Cells[cells].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0xFF, 0xFF, 0xFF));

                    worksheet.Cells[cells].Style.Font.Size = 11;
                    worksheet.Cells[cells].Style.Font.Name = "Calibri";

                    swap = !swap;
                    currentRow = maxRow + 1;
                }
                currentRow++;

                //rest of the data
                //Format the headers for the table
                worksheet.Cells["A" + currentRow + ":B" + currentRow].Merge = true;
                worksheet.Cells["C" + currentRow + ":G" + currentRow].Merge = true;

                OfficeOpenXml.Style.ExcelStyle headerStyleStds;
                worksheet.Cells["A" + currentRow].Value = "Harmonized Standard";
                worksheet.Cells["C" + currentRow].Value = "Identifiers";


                headerStyleStds = worksheet.Cells["A" + currentRow + ":C" + currentRow].Style;

                headerStyleStds.Font.Bold = true;
                headerStyleStds.Font.Italic = true;
                headerStyleStds.Font.Name = "Calibri";
                headerStyleStds.Font.Size = 14;
                headerStyleStds.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                headerStyleStds.Fill.BackgroundColor.SetColor(Color.FromArgb(0xC6, 0xE0, 0xB4));
                currentRow++;

                foreach (Std std in stds)
                {
                    worksheet.Cells["A" + currentRow].Value = std.Name;

                    string ids = "";
                    foreach (string id in std.ids)
                    {
                        ids += id + " ";
                    }

                    worksheet.Cells["C" + currentRow].Value = ids;
                    currentRow++;
                }

                currentRow++;


                //rest of the data
                //Format the headers for the table
                worksheet.Cells["A" + currentRow + ":G" + currentRow].Merge = true;

                OfficeOpenXml.Style.ExcelStyle headerStyleParams;
                worksheet.Cells["A" + currentRow].Value = "Technical parameters also referred to in the harmonised standard";


                headerStyleParams = worksheet.Cells["A" + currentRow + ":G" + currentRow].Style;

                headerStyleParams.Font.Bold = true;
                headerStyleParams.Font.Italic = true;
                headerStyleParams.Font.Name = "Calibri";
                headerStyleParams.Font.Size = 14;
                headerStyleParams.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                headerStyleParams.Fill.BackgroundColor.SetColor(Color.FromArgb(0xC6, 0xE0, 0xB4));
                currentRow++;

                foreach (Param par in param)
                {
                    foreach (string str in transformRows(par.text, 150))
                    {
                        worksheet.Cells["A" + currentRow].Value = str;
                        currentRow++;
                    }
                    currentRow++;
                }

                currentRow++;

                //rest of the data
                //Format the headers for the table
                worksheet.Cells["A" + currentRow + ":C" + currentRow].Merge = true;
                worksheet.Cells["D" + currentRow + ":G" + currentRow].Merge = true;

                OfficeOpenXml.Style.ExcelStyle headerStyleIss;
                worksheet.Cells["A" + currentRow].Value = "Frequency Issue";
                worksheet.Cells["D" + currentRow].Value = "Identifiers";


                headerStyleIss = worksheet.Cells["A" + currentRow + ":G" + currentRow].Style;

                headerStyleIss.Font.Bold = true;
                headerStyleIss.Font.Italic = true;
                headerStyleIss.Font.Name = "Calibri";
                headerStyleIss.Font.Size = 14;
                headerStyleIss.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                headerStyleIss.Fill.BackgroundColor.SetColor(Color.FromArgb(0xC6, 0xE0, 0xB4));
                currentRow++;

                foreach (Issue iss in issues)
                {
                    string ids = "";
                    foreach (string id in iss.ids)
                    {
                        ids += id + " ";
                    }
                    worksheet.Cells["D" + currentRow].Value = ids;

                    worksheet.Cells["A" + currentRow].Value = iss.Name;
                    foreach (string str in transformRows(iss.Name, 65))
                    {
                        worksheet.Cells["A" + currentRow].Value = str;
                        currentRow++;
                    }

                    currentRow++;
                }

                currentRow++;

                //rest of the data
                //Format the headers for the table
                worksheet.Cells["B" + currentRow + ":G" + currentRow].Merge = true;

                OfficeOpenXml.Style.ExcelStyle headerStyleNotes;
                worksheet.Cells["A" + currentRow].Value = "No.";
                worksheet.Cells["B" + currentRow].Value = "Note";


                headerStyleIss = worksheet.Cells["A" + currentRow + ":G" + currentRow].Style;

                headerStyleIss.Font.Bold = true;
                headerStyleIss.Font.Italic = true;
                headerStyleIss.Font.Name = "Calibri";
                headerStyleIss.Font.Size = 14;
                headerStyleIss.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                headerStyleIss.Fill.BackgroundColor.SetColor(Color.FromArgb(0xC6, 0xE0, 0xB4));
                currentRow++;

                int ind = 1;
                foreach (string note in notes)
                {
                    if (note == null) continue;
                    worksheet.Cells["A" + currentRow].Value = ind++;
                    foreach (string str in transformRows(note, 150))
                    {
                        worksheet.Cells["B" + currentRow].Value = str;
                        currentRow++;
                    }

                    currentRow++;
                }

                currentRow++;


                var memoryStream = new MemoryStream();
                package.SaveAs(memoryStream);
                memoryStream.Position = 0;

                var fileBytes = memoryStream.ToArray();
                return fileBytes;
            }
        }


        [HttpPost]
        [Route("SRD/DownloadExcel")]
        public byte[] DownloadExcel(string FromVal, string ToVal, int FrequencySizeVal, int FrequencyTableVal, int AnnexVal)
        {
            return GenerateExcel(FromVal, ToVal, FrequencySizeVal, FrequencyTableVal, AnnexVal);
        }

        [HttpPost]
        [Route("SRD/DownloadPDF")]
        public byte[] DownloadPDF(string FromVal, string ToVal, int FrequencySizeVal, int FrequencyTableVal, int AnnexVal)
        {
            byte[] excelBytes = GenerateExcel150(FromVal, ToVal, FrequencySizeVal, FrequencyTableVal, AnnexVal);
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

        private List<SRDSearch> GetResults(string FromVal, string ToVal, int FrequencySizeVal, int FrequencyTableVal, int AnnexVal)
        {
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

            List<SRDDb> data = _conSRD.SRDDb.Include(e => e.Document).ToList();

            List<SRDSearch> transformedData = transformData(data, tempFrom, tempTo, FrequencyTableVal, AnnexVal);


            return transformedData;
        }

        private double ConvertToHz(string frequencyString)
        {
            double frequency = 0.0;

            // Parse the input string to extract the numeric value and unit
            string valueString = string.Empty;
            string unitString = string.Empty;

            foreach (char c in frequencyString)
            {
                if (Char.IsDigit(c) || c == '.')
                {
                    valueString += c;
                }
                else
                {
                    unitString += c;
                }
            }

            // Convert the numeric value to double
            if (double.TryParse(valueString, out double value))
            {
                // Convert the unit to Hz based on the provided string
                switch (unitString.Trim().ToLower())
                {
                    case "hz":
                        frequency = value;
                        break;
                    case "khz":
                        frequency = value * 1000.0;
                        break;
                    case "mhz":
                        frequency = value * 1000000.0;
                        break;
                    case "ghz":
                        frequency = value * 1000000000.0;
                        break;
                    default:
                        // Unsupported unit, return 0
                        break;
                }
            }

            return frequency;
        }

        private string getStdLink(string std)
        {
            return "";
        }

        private string getEccLink(string val)
        {
            return "";
        }

        private List<SRDSearch> transformData(List<SRDDb> data, long from, long to, int table, int annex)
        {
            List<SRDSearch> lst = new List<SRDSearch>();
            foreach(SRDDb srd in data) {
                string country = "";

                //update later when we have more countries
                if(table == 1)
                {
                    country = "PSE";
                } else
                {
                    country = "ITU";
                }

                double lowValue = ConvertToHz(srd.Document.Low_freq);
                double highValue = ConvertToHz(srd.Document.High_freq);
                string annexString;
                if(annex != 14)
                {
                    annexString = "Annex " + annex;
                } else
                {
                    annexString = "Annex A";
                }

                //filtriranje, dodati i za region
                if (from != 0 && lowValue < from) continue;
                if (to != 0 && highValue > to) continue;
                if (annex != 0 && !srd.Document.Doc_number.EndsWith(annexString)) continue;
                if (srd.Country != country && table != 0) continue;

                string application = srd.Document.Application;

                //izvuci standarde kao listu
                List<string> standards = srd.Standards.Split(",").ToList();
                List<string> stdLinks = new List<string>();
                foreach(string standard in standards) { 
                    stdLinks.Add(getStdLink(standard));
                }

                //izvuci link za ECC_ERC
                Models.Document.DocumentsDb dataLink = _conSRD.DocumentsDb
                                    .SingleOrDefault(e => e.Application == srd.Document.Application &&
                                    e.Low_freq == srd.Document.Low_freq &&
                                    e.High_freq == srd.Document.High_freq &&
                                    e.Doc_number == srd.ECC_ERC);

                lst.Add(new SRDSearch
                {
                    Country = srd.Country,
                    Power = srd.Power,
                    stdParams = srd.StdParams,
                    FreqIssue = srd.FreqIssue,
                    BandNote = srd.BandNote,
                    Identifier = srd.Identifier,
                    StdLinks = stdLinks,
                    ECC_ERCLink = dataLink!=null ? dataLink.Hyperlink : "",
                    Standards = standards,
                    DocumentNumber = srd.Document.Doc_number,
                    DocumentTitle = srd.Document.Title_of_doc,
                    ECC_ERC = srd.ECC_ERC,
                    Modulation = srd.Modulation,
                    Spectrum = srd.Spectrum,
                    Note1 = srd.Note1,
                    Note2 = srd.Note2,  
                    Note3 = srd.Note3,
                    Note4 = srd.Note4,
                    Note5 = srd.Note5,
                    Note6 = srd.Note6,
                    Note7 = srd.Note7,
                    Note8 = srd.Note8,
                    LowView = srd.Document.Low_freq,
                    HighView = srd.Document.High_freq,
                    Low = ConvertToHz(srd.Document.Low_freq),
                    High = ConvertToHz(srd.Document.High_freq)
                });
            }

            return lst;
        }


        public IActionResult Index()
        {
            return View();
        }
    }
}
