using FrekvencijeProject.Models;
using FrekvencijeProject.Models.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Diagnostics;
using System.Globalization;
using NHibernate.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FrekvencijeProject.Controllers.Actions;
using FrekvencijeProject.Models.ExcelModels;
using GemBox.Spreadsheet;
using System.IO;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using OfficeOpenXml;
using System.Drawing;
using iTextSharp.tool.xml.html.head;
using static iTextSharp.text.pdf.AcroFields;
using FrekvencijeProject.Models.Ajax;

namespace FrekvencijeProject.Controllers
{
    public class AllocationSearchController : Controller
    {
        object user;

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
            new SelectListItem { Text = "MHz", Value = "3", Selected = true},
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
            //new SelectListItem { Text = "Palestine", Value = "16",Selected = true },
            new SelectListItem { Text = "Qatar", Value = "16" },
            new SelectListItem { Text = "Saudi Arabia", Value = "17" },
            new SelectListItem { Text = "Somalia", Value = "18" },
            new SelectListItem { Text = "Sudan", Value = "19" },
            new SelectListItem { Text = "Syria", Value = "20" },
            new SelectListItem { Text = "Tunisia", Value = "21" },
            new SelectListItem { Text = "United Arab Emirates", Value = "22" },
            new SelectListItem { Text = "Yemen", Value = "23" },

        };

        [Display(Name = "Allocation")]
        public string AllocationTable { get; set; }

        public string AllocationFirstValue { get; set; }

        public List<SelectListItem> AllocationFirstList = new List<SelectListItem>
        {
            new SelectListItem { Text = "<all allocation terms>", Value = "1" },
            new SelectListItem { Text = "Amateur", Value = "2" },
            new SelectListItem { Text = "Amateur-Satellite", Value = "3" },
            new SelectListItem { Text = "Broadcasting", Value = "4" },
            new SelectListItem { Text = "Broadcasting-Satellite", Value = "5" },
            new SelectListItem { Text = "Earth Exploration-Satellite", Value = "6" },
            new SelectListItem { Text = "Fixed", Value = "7" },
            new SelectListItem { Text = "Fixed-Satellite", Value = "8" },
            new SelectListItem { Text = "Inter-Satellite", Value = "9" },
            new SelectListItem { Text = "Meteorological Aids", Value = "10" },
            new SelectListItem { Text = "Mobile", Value = "11" },
            new SelectListItem { Text = "Mobile-Satellite", Value = "12" },
            new SelectListItem { Text = "Not allocated", Value = "13" },
            new SelectListItem { Text = "Radio Astronomy", Value = "14" },
            new SelectListItem { Text = "Radiodetermination", Value = "15" },
            new SelectListItem { Text = "Radiodetermination-Satellite", Value = "16" },
            new SelectListItem { Text = "Space Operation", Value = "17" },
            new SelectListItem { Text = "Space Reaserch", Value = "18" },
            new SelectListItem { Text = "Standard Frequency and Time Signal", Value = "19" },
            new SelectListItem { Text = "Standard Frequency and Time Signal-Satellite", Value = "20" }
        };

        public string AllocationAll { get; set; }

        public string AllocationSecondValue { get; set; }

        public List<SelectListItem> AllocationSecondValueList = new List<SelectListItem>
        {
            new SelectListItem { Text = "<all level 2 allocation terms>", Value = "1" },
            new SelectListItem { Text = "Aeronauitical Mobile", Value = "2" },
            new SelectListItem { Text = "Aeronauitical Mobile-Satellite", Value = "3" },
            new SelectListItem { Text = "Amateur-Satellite (Earth-to-space)", Value = "4" },
            new SelectListItem { Text = "Amateur-Satellite (space-to-Eaerth)", Value = "5" },
            new SelectListItem { Text = "Earth Exploration-Satellite (Earth-to-space)", Value = "6" },
            new SelectListItem { Text = "Earth Exploration-Satellite (Earth-to-space) (space-to-space)", Value = "7" },
            new SelectListItem { Text = "Earth Exploration-Satellite (active)", Value = "8" },
            new SelectListItem { Text = "Earth Exploration-Satellite (pasive)", Value = "9" },
            new SelectListItem { Text = "Earth Exploration-Satellite (space-to-Earth)", Value = "10" },
            new SelectListItem { Text = "Earth Exploration-Satellite (space-to-Earth) (space-to-space)", Value = "11" },
            new SelectListItem { Text = "Fixed-Satellite (Earth-to-space)", Value = "12" },
            new SelectListItem { Text = "Fixed-Satellite (Earth-to-space) (space-to-Earth)", Value = "13" },
            new SelectListItem { Text = "Fixed-Satellite (space-to-Earth)", Value = "14" },
            new SelectListItem { Text = "Fixed-Satellite (space-to-Earth) (Earth-to-space)", Value = "15" },
            new SelectListItem { Text = "Land Mobile", Value = "16" },
            new SelectListItem { Text = "Land Mobile-Satellite", Value = "17" },
            new SelectListItem { Text = "Maritime Mobile", Value = "18" },
            new SelectListItem { Text = "Maritime Mobile-Satellite", Value = "19" },
            new SelectListItem { Text = "Meteorological-Satellite", Value = "20" },
            new SelectListItem { Text = "Mobile (distress and calling)", Value = "21" },
            new SelectListItem { Text = "Mobile (distress and safety)", Value = "22" },
            new SelectListItem { Text = "Mobile (distress, safety and calling)", Value = "23" },
            new SelectListItem { Text = "Mobile except aeronautical mobile", Value = "24" },
            new SelectListItem { Text = "Mobile except aeronautical mobile (R)", Value = "25" },
            new SelectListItem { Text = "Mobile-Satellite (Earth-to-space)", Value = "26" },
            new SelectListItem { Text = "Mobile-Satellite (space-to-Earth)", Value = "27" },
            new SelectListItem { Text = "Mobile-Satellite except aeronautical mobile-satellite (Earth-to-space)", Value = "28" },
            new SelectListItem { Text = "Mobile-Satellite except aeronautical mobile-satellite (space-to-Earth)", Value = "29" },
            new SelectListItem { Text = "Mobile-Satellite except aeronautical mobile-satellite", Value = "30" },
            new SelectListItem { Text = "Mobile-Satellite except aeronautical mobile-satellite (R)", Value = "31" },
            new SelectListItem { Text = "Mobile-Satellite except maritime mobile-satellite (space-to-Earth)", Value = "32" },
            new SelectListItem { Text = "Radiodetermination-Satellite (Earth-to-space)", Value = "33" },
            new SelectListItem { Text = "Radiodetermination-Satellite (space-to-Earth)", Value = "34" },
            new SelectListItem { Text = "Radiolocation", Value = "35" },
            new SelectListItem { Text = "Radiolocation-Satellite", Value = "36" },
            new SelectListItem { Text = "Radionavigation", Value = "37" },
            new SelectListItem { Text = "Radionavigation-Satellite", Value = "38" },
            new SelectListItem { Text = "Space Operation (Earth-to-space)", Value = "39" },
            new SelectListItem { Text = "Space Operation (Earth-to-space) (space-to-Earth)", Value = "40" },
            new SelectListItem { Text = "Space Operation (Earth-to-space) (space-to-space)", Value = "41" },
            new SelectListItem { Text = "Space Operation (satellite identification)", Value = "42" },
            new SelectListItem { Text = "Space Operation (space-to-Earth)", Value = "43" },
            new SelectListItem { Text = "Space Operation (space-to-Earth) (space-to-space)", Value = "44" },
            new SelectListItem { Text = "Space Research (active)", Value = "45" },
            new SelectListItem { Text = "Space Research (deep space)", Value = "46" },
            new SelectListItem { Text = "Space Research (passive)", Value = "47" },
            new SelectListItem { Text = "Space Research (space-to-Earth)", Value = "48" },
            new SelectListItem { Text = "Space Research (space-to-Earth) (space-to-space)", Value = "49" },
            new SelectListItem { Text = "Space Research (space-to-space)", Value = "50" },
            new SelectListItem { Text = "Standard Frequency and Time Signal-Satellite (Earth-to-space)", Value = "51" },
            new SelectListItem { Text = "Standard Frequency and Time Signal-Satellite (space-to-Earth)", Value = "52" },
            new SelectListItem { Text = "Standard Frequency and time signal (10 000 kHz)", Value = "53" },
            new SelectListItem { Text = "Standard Frequency and time signal (15 000 kHz)", Value = "54" },
            new SelectListItem { Text = "Standard Frequency and time signal (2 500 kHz)", Value = "55" },
            new SelectListItem { Text = "Standard Frequency and time signal (20 000 kHz)", Value = "56" },
            new SelectListItem { Text = "Standard Frequency and time signal (20 kHz)", Value = "57" },
            new SelectListItem { Text = "Standard Frequency and time signal (25 000 kHz)", Value = "58" },
            new SelectListItem { Text = "Standard Frequency and time signal (5 000 kHz)", Value = "59" },
            new SelectListItem { Text = "Standard Frequency and time signal-satellite (400.1 MHz)", Value = "60" }
        };

        public string AllocationThirdValue { get; set; }

        public List<SelectListItem> AllocationThirdValueList = new List<SelectListItem>
        {
            new SelectListItem { Text = "<all level 3 allocation terms>", Value = "1" },
            new SelectListItem { Text = "Aeronauitical Mobile (OR)", Value = "2" },
            new SelectListItem { Text = "Aeronauitical Mobile (R)", Value = "3" },
            new SelectListItem { Text = "Aeronauitical Mobile-Satellite (OR)", Value = "4" },
            new SelectListItem { Text = "Aeronauitical Mobile-Satellite (R)", Value = "5" },
            new SelectListItem { Text = "Aeronauitical Mobile-Satellite (R) (Earth-to-space)", Value = "6" },
            new SelectListItem { Text = "Aeronauitical Mobile-Satellite (R)  (space-to-Earth)", Value = "7" },
            new SelectListItem { Text = "Aeronauitical Radionavigation", Value = "8" },
            new SelectListItem { Text = "Aeronauitical Radionavigation-Satellite", Value = "9" },
            new SelectListItem { Text = "Land Mobile-Satellite (Earth-to-space)", Value = "10" },
            new SelectListItem { Text = "Land Mobile-Satellite (space-to-Earth) ", Value = "11" },
            new SelectListItem { Text = "Maritime Mobile (distress and calling via DSC)", Value = "12" },
            new SelectListItem { Text = "Maritime Mobile (distress and calling)", Value = "13" },
            new SelectListItem { Text = "Maritime Mobile (distress, safety and calling)", Value = "14" },
            new SelectListItem { Text = "Maritime Mobile-Satellite (Earth-to-space)", Value = "15" },
            new SelectListItem { Text = "Maritime Mobile-Satellite (space-to-Earth)", Value = "16" },
            new SelectListItem { Text = "Maritime Radionavigation", Value = "17" },
            new SelectListItem { Text = "Maritime Radionavigation (radiobeacons)", Value = "18" },
            new SelectListItem { Text = "Maritime Radionavigation-Satellite", Value = "19" },
            new SelectListItem { Text = "Meteorological-Satellite (Earth-to-space)", Value = "20" },
            new SelectListItem { Text = "Meteorological-Satellite (space-to-Earth)", Value = "21" },
            new SelectListItem { Text = "Radiolocation-Satellite (Earth-to-space)", Value = "22" },
            new SelectListItem { Text = "Radiolocation-Satellite (space-to-Earth)", Value = "23" },
            new SelectListItem { Text = "Radionavigation-Satellite (Earth-to-space)", Value = "24" },
            new SelectListItem { Text = "Radionavigation-Satellite (Earth-to-space) (space-to-space)", Value = "25" },
            new SelectListItem { Text = "Radionavigation-Satellite (space-to-space)", Value = "26" },
            new SelectListItem { Text = "Space Research (deep space) (Earth-to-space)", Value = "27" },
            new SelectListItem { Text = "Space Research (deep space) (space-to-Earth)", Value = "28" }
        };
        //method for searching allocation on first level
        [HttpPost]
        public JsonResult SearchFirstLevel(string AllVal, string FromVal, string ToVal, int FrequencySizeVal, string FreqTable,string[] FreqTableVal)
        {
            //for (int i = 0; i < FreqTableVal.Length; i++)
            //{
            //    Debug.WriteLine("bal:" + FreqTableVal[i]);
            //    var tempFreqText = FrequencyTablesList.Where(p => p.Value.Equals(FreqTableVal[i])).First().Text;
            //    Debug.WriteLine("power text:" + tempFreqText);
            //}
            string tempFreq = "";
            if (FreqTableVal.Length == 1)
            {
                tempFreq = FrequencyTablesList.Where(p => p.Text.Equals(FreqTable)).First().Value;
            }
            AllocationAll = "" + AllVal;
            //Debug.WriteLine("vrijednost:" + AllocationAll);
            //taxt from menu of allocation on first level
            string tempValueAllocation = AllocationFirstList.Where(p => p.Value.Equals(AllocationAll)).First().Text;
            //take value from frequency table which can be Hz, MHz, GHz.
            //var tempFreq = FrequencyTablesList.Where(p => p.Text.Equals(FreqTable)).First().Value;
            //Debug.WriteLine("vrijednost:" + FreqTable);
            From = FromVal;
            To = ToVal;
            FrequencySizeValue = "" + FrequencySizeVal;
            long tempFrom = 0;
            long tempTo = 0;
            //converting the Hz, MHz, GHz
            if (From != null)
            {
                tempFrom = long.Parse(From);
                From = "" + tempFrom;
            }
            if(To != null)
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
                    // tempFrom = long.Parse(From) * long.Parse(From) * long.Parse(From) *
                    //long.Parse(From) * long.Parse(From) *
                    //long.Parse(From) * long.Parse(From) * long.Parse(From) * long.Parse(From);
                    // From = "" + tempFrom;
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
                    //tempTo = long.Parse(To) * long.Parse(To) * long.Parse(To) *
                    //long.Parse(To) * long.Parse(To) *
                    //long.Parse(To) * long.Parse(To) * long.Parse(To) * long.Parse(To);
                    To = "" + tempTo;
                }

            }
            //Debug.WriteLine("proba");
            if (tempFrom == 0 && tempTo == 0)
            {
                List<AsiaPacific> listOfAsiaPacific = new List<AsiaPacific>();
                if (FreqTableVal.Length == 1)
                {

                    AllocationSearchActions asa = new AllocationSearchActions();

                    var listGeneral = asa.FirstLevelSearch(_conAll, tempFrom, tempTo, tempValueAllocation, FreqTableVal[0]);
                    //Debug.WriteLine("power text:" + listGeneral.Count);
                    
                    AsiaPacific asia = new AsiaPacific();
                    asia.ListOfFreqBand = new List<FreqBandSearchNew>();
                    asia.ListOfFreqBand.AddRange(listGeneral.OrderBy(e => e.low).ToList());
                    listOfAsiaPacific.Add(asia);

                    return Json(listOfAsiaPacific, new System.Text.Json.JsonSerializerOptions());
                }
                else
                {
                    
                    for (int i = 0; i < FreqTableVal.Length; i++)
                    {

                        AllocationSearchActions asa = new AllocationSearchActions();
                        List<FreqBandSearchNew> listGeneral = asa.FirstLevelSearch(_conAll, tempFrom, tempTo, tempValueAllocation, FreqTableVal[i]);

                        AsiaPacific asia = new AsiaPacific();
                        asia.ListOfFreqBand = new List<FreqBandSearchNew>();
                        asia.ListOfFreqBand.AddRange(listGeneral.OrderBy(e => e.low).ToList());
                        listOfAsiaPacific.Add(asia);

                    }
                    
                    return Json(listOfAsiaPacific, new System.Text.Json.JsonSerializerOptions());
                }

                
                //AllocationSearchActions asa = new AllocationSearchActions();
                //var listGeneral = asa.FirstLevelSearch(_conAll, tempFrom,tempTo, tempValueAllocation, tempFreq);
                //return Json(listGeneral.OrderBy(e => e.low).ToList(), new System.Text.Json.JsonSerializerOptions());

            }
            else if (tempFrom == 0 && tempTo != 0)
            {
                List<AsiaPacific> listOfAsiaPacific = new List<AsiaPacific>();
                if (FreqTableVal.Length == 1)
                {

                    AllocationSearchActions asa = new AllocationSearchActions();

                    var listGeneral = asa.FirstLevelSearch(_conAll, tempFrom, tempTo, tempValueAllocation, FreqTableVal[0]);
                    //Debug.WriteLine("power text:" + listGeneral.Count);

                    AsiaPacific asia = new AsiaPacific();
                    asia.ListOfFreqBand = new List<FreqBandSearchNew>();
                    asia.ListOfFreqBand.AddRange(listGeneral.OrderBy(e => e.low).ToList());
                    listOfAsiaPacific.Add(asia);

                    return Json(listOfAsiaPacific, new System.Text.Json.JsonSerializerOptions());
                }
                else
                {

                    for (int i = 0; i < FreqTableVal.Length; i++)
                    {

                        AllocationSearchActions asa = new AllocationSearchActions();
                        List<FreqBandSearchNew> listGeneral = asa.FirstLevelSearch(_conAll, tempFrom, tempTo, tempValueAllocation, FreqTableVal[i]);

                        AsiaPacific asia = new AsiaPacific();
                        asia.ListOfFreqBand = new List<FreqBandSearchNew>();
                        asia.ListOfFreqBand.AddRange(listGeneral.OrderBy(e => e.low).ToList());
                        listOfAsiaPacific.Add(asia);

                    }

                    return Json(listOfAsiaPacific, new System.Text.Json.JsonSerializerOptions());
                }
            }
            else if (tempFrom != 0 && tempTo != 0)
            {
                List<AsiaPacific> listOfAsiaPacific = new List<AsiaPacific>();
                if (FreqTableVal.Length == 1)
                {

                    AllocationSearchActions asa = new AllocationSearchActions();

                    var listGeneral = asa.FirstLevelSearch(_conAll, tempFrom, tempTo, tempValueAllocation, FreqTableVal[0]);
                    //Debug.WriteLine("power text:" + listGeneral.Count);

                    AsiaPacific asia = new AsiaPacific();
                    asia.ListOfFreqBand = new List<FreqBandSearchNew>();
                    asia.ListOfFreqBand.AddRange(listGeneral.OrderBy(e => e.low).ToList());
                    listOfAsiaPacific.Add(asia);

                    return Json(listOfAsiaPacific, new System.Text.Json.JsonSerializerOptions());
                }
                else
                {

                    for (int i = 0; i < FreqTableVal.Length; i++)
                    {

                        AllocationSearchActions asa = new AllocationSearchActions();
                        List<FreqBandSearchNew> listGeneral = asa.FirstLevelSearch(_conAll, tempFrom, tempTo, tempValueAllocation, FreqTableVal[i]);

                        AsiaPacific asia = new AsiaPacific();
                        asia.ListOfFreqBand = new List<FreqBandSearchNew>();
                        asia.ListOfFreqBand.AddRange(listGeneral.OrderBy(e => e.low).ToList());
                        listOfAsiaPacific.Add(asia);

                    }

                    return Json(listOfAsiaPacific, new System.Text.Json.JsonSerializerOptions());
                }
            }
            return Json("");

            
        }
        //method for searching allocation on second level
        [HttpPost]
        public JsonResult SearchSecondLevel(string AllVal, string FromVal, string ToVal, int FrequencySizeVal,string FreqTable)
        {
            //Debug.WriteLine("bal");
            AllocationAll = "" + AllVal;
            //Debug.WriteLine("vrijednost:" + AllocationAll);
            //taxt from menu of allocation on second level
            string tempValueAllocation = AllocationSecondValueList.Where(p => p.Value.Equals(AllocationAll)).First().Text;
            //take value from frequency table which can be Hz, MHz, GHz.
            var tempFreq = FrequencyTablesList.Where(p => p.Text.Equals(FreqTable)).First().Value;
            AllocationAll = "" + AllVal;
            //Debug.WriteLine("vrijednost2:" + AllocationAll);
            
            //Debug.WriteLine("vrij:");
            From = FromVal;
            To = ToVal;
            FrequencySizeValue = "" + FrequencySizeVal;
            long tempFrom = 0;
            long tempTo = 0;
            //converting the Hz, MHz, GHz
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
                    // tempFrom = long.Parse(From) * long.Parse(From) * long.Parse(From) *
                    //long.Parse(From) * long.Parse(From) *
                    //long.Parse(From) * long.Parse(From) * long.Parse(From) * long.Parse(From);
                    // From = "" + tempFrom;
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
                    //tempTo = long.Parse(To) * long.Parse(To) * long.Parse(To) *
                    //long.Parse(To) * long.Parse(To) *
                    //long.Parse(To) * long.Parse(To) * long.Parse(To) * long.Parse(To);
                    To = "" + tempTo;
                }

            }

            if (tempFrom == 0 && tempTo == 0)
            {
                AllocationSearchActions asa = new AllocationSearchActions();
                var listGeneral = asa.SecondLevelSearch(_conAll, tempFrom, tempTo, tempValueAllocation, tempFreq);
                return Json(listGeneral.OrderBy(e => e.low).ToList(), new System.Text.Json.JsonSerializerOptions());
            }
            else if (tempFrom == 0 && tempTo != 0)
            {
                AllocationSearchActions asa = new AllocationSearchActions();
                var listGeneral = asa.SecondLevelSearch(_conAll, tempFrom, tempTo, tempValueAllocation, tempFreq);
                return Json(listGeneral.OrderBy(e => e.low).ToList(), new System.Text.Json.JsonSerializerOptions());
           
            }
            else if (tempFrom != 0 && tempTo != 0)
            {
                AllocationSearchActions asa = new AllocationSearchActions();
                var listGeneral = asa.SecondLevelSearch(_conAll, tempFrom, tempTo, tempValueAllocation, tempFreq);
                return Json(listGeneral.OrderBy(e => e.low).ToList(), new System.Text.Json.JsonSerializerOptions());
          
            }
            return Json("");

            
        }
        //method for searching allocation on third level
        [HttpPost]
        public JsonResult SearchThirdLevel(string AllVal, string FromVal, string ToVal, int FrequencySizeVal, string FreqTable)
        {
            AllocationAll = "" + AllVal;
            //Debug.WriteLine("vrijednost3:" + AllocationAll);
            //taxt from menu of allocation on third level
            string tempValueAllocation = AllocationThirdValueList.Where(p => p.Value.Equals(AllocationAll)).First().Text;
            //Debug.WriteLine("vrijeme:"+ FrequencySizeVal+","+ tempValue);
            //converting the Hz, MHz, GHz
            var tempFreq = FrequencyTablesList.Where(p => p.Text.Equals(FreqTable)).First().Value;
            From = FromVal;
            To = ToVal;
            FrequencySizeValue = "" + FrequencySizeVal;
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
                    // tempFrom = long.Parse(From) * long.Parse(From) * long.Parse(From) *
                    //long.Parse(From) * long.Parse(From) *
                    //long.Parse(From) * long.Parse(From) * long.Parse(From) * long.Parse(From);
                    // From = "" + tempFrom;
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
                    //tempTo = long.Parse(To) * long.Parse(To) * long.Parse(To) *
                    //long.Parse(To) * long.Parse(To) *
                    //long.Parse(To) * long.Parse(To) * long.Parse(To) * long.Parse(To);
                    To = "" + tempTo;
                }

            }

            if (tempFrom == 0 && tempTo == 0)
            {
                AllocationSearchActions asa = new AllocationSearchActions();
                var listGeneral = asa.SecondLevelSearch(_conAll, tempFrom, tempTo, tempValueAllocation, tempFreq);
                return Json(listGeneral.OrderBy(e => e.low).ToList(), new System.Text.Json.JsonSerializerOptions());
      
            }
            else if (tempFrom == 0 && tempTo != 0)
            {
                AllocationSearchActions asa = new AllocationSearchActions();
                var listGeneral = asa.SecondLevelSearch(_conAll, tempFrom, tempTo, tempValueAllocation, tempFreq);
                return Json(listGeneral.OrderBy(e => e.low).ToList(), new System.Text.Json.JsonSerializerOptions());
          
            }
            else if (tempFrom != 0 && tempTo != 0)
            {
                AllocationSearchActions asa = new AllocationSearchActions();
                var listGeneral = asa.SecondLevelSearch(_conAll, tempFrom, tempTo, tempValueAllocation, tempFreq);
                return Json(listGeneral.OrderBy(e => e.low).ToList(), new System.Text.Json.JsonSerializerOptions());
           
            }
            return Json("");
        }

        //method for searching allocation on fourth level
        [HttpPost]
        public JsonResult SearchFourthLevel(string AllVal, string FromVal, string ToVal, int FrequencySizeVal,string FreqTable)
        {
            //Debug.WriteLine("bal");
            //AllocationAll = "" + AllVal;
            //Debug.WriteLine("vrijednost:" + AllocationAll);
           // string tempValueAllocation = AllocationFourthValue.Where(p => p.Value.Equals(AllocationAll)).First().Text;
            var tempFreq = FrequencyTablesList.Where(p => p.Text.Equals(FreqTable)).First().Value;
            
            AllocationAll = "" + AllVal;
            //Debug.WriteLine("vrijednost4:" + AllocationAll);
            //string tempValue = Allocation.Where(p => p.Value.Equals(AllocationAll)).First().Text;
          
            From = FromVal;
            To = ToVal;
            FrequencySizeValue = "" + FrequencySizeVal;
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
                    // tempFrom = long.Parse(From) * long.Parse(From) * long.Parse(From) *
                    //long.Parse(From) * long.Parse(From) *
                    //long.Parse(From) * long.Parse(From) * long.Parse(From) * long.Parse(From);
                    // From = "" + tempFrom;
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
                    //tempTo = long.Parse(To) * long.Parse(To) * long.Parse(To) *
                    //long.Parse(To) * long.Parse(To) *
                    //long.Parse(To) * long.Parse(To) * long.Parse(To) * long.Parse(To);
                    To = "" + tempTo;
                }

            }

            if (tempFrom == 0 && tempTo == 0)
            {
                
                AllocationSearchActions asa = new AllocationSearchActions();
                var listGeneral = asa.SecondLevelSearch(_conAll, tempFrom, tempTo, AllocationAll, tempFreq);
                return Json(listGeneral.OrderBy(e => e.low).ToList(), new System.Text.Json.JsonSerializerOptions());


            }
            else if (tempFrom == 0 && tempTo != 0)
            {
                AllocationSearchActions asa = new AllocationSearchActions();
                var listGeneral = asa.SecondLevelSearch(_conAll, tempFrom, tempTo, AllocationAll, tempFreq);
                return Json(listGeneral.OrderBy(e => e.low).ToList(), new System.Text.Json.JsonSerializerOptions());

            }
            else if (tempFrom != 0 && tempTo != 0)
            {
                AllocationSearchActions asa = new AllocationSearchActions();
                var listGeneral = asa.SecondLevelSearch(_conAll, tempFrom, tempTo, AllocationAll, tempFreq);
                return Json(listGeneral.OrderBy(e => e.low).ToList(), new System.Text.Json.JsonSerializerOptions());

            }

            return Json("");
        }

        //method for searching allocation on without levels only values
        [HttpPost]
        public JsonResult SearchOnlyValues(string FromVal, string ToVal, int FrequencySizeVal, string FreqTable,string[] FreqTableVal)
        {
            //Debug.WriteLine("Pozvao je ovo");
            try
            {

                //var tempFreq = FrequencyTablesList.Where(p => p.Text.Equals(FreqTable)).First().Value;
                From = FromVal;
                To = ToVal;
                FrequencySizeValue = "" + FrequencySizeVal;
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
                List<AsiaPacific> listOfAsiaPacific = new List<AsiaPacific>();
                //var listGeneral = asa.SearchValues(_conAll, tempFrom, tempTo, tempFreq);
                //Debug.WriteLine("pp:" + FreqTableVal.Length);
                if (FreqTableVal.Length == 1)
                {
                    
                    AllocationSearchActions asa = new AllocationSearchActions();
                    var listGeneral = asa.SearchValues(_conAll, tempFrom, tempTo, FreqTableVal[0]);
                    AsiaPacific asia = new AsiaPacific();
                    asia.ListOfFreqBand = new List<FreqBandSearchNew>();
                    asia.ListOfFreqBand.AddRange(listGeneral.OrderBy(e => e.low).ToList());
                    listOfAsiaPacific.Add(asia);
                }
                else
                {
                    for (int i = 0; i < FreqTableVal.Length; i++)
                    {
                        
                        AllocationSearchActions asa = new AllocationSearchActions();
                        var listGeneral = asa.SearchValues(_conAll, tempFrom, tempTo, FreqTableVal[i]);
                        AsiaPacific asia = new AsiaPacific();
                        asia.ListOfFreqBand = new List<FreqBandSearchNew>();
                        asia.ListOfFreqBand.AddRange(listGeneral.OrderBy(e => e.low).ToList());
                        listOfAsiaPacific.Add(asia);

                    }
                }
                
                

                var BestproductSale = _conAll.AllocationRangeDb.ToList()
                               .GroupBy(x => x.high)
                               .Select(grp => new
                               {
                                   High = grp.ToList().FirstOrDefault().high

                               })
                               .OrderByDescending(x => x.High).ToList();

                long HighTemp = BestproductSale[0].High;

                var HighView = (from all in _conAll.AllocationRangeDb
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
                //listGeneral = listGeneral.OrderBy(e => e.low).ToList();
                //listGeneral[0].isAllValues = true;
                //listGeneral[0].lowJson = 0;
                //listGeneral[0].highJson = result;
                //listGeneral[0].sizeFrequency = sizeFrequency;
                listOfAsiaPacific[0].isAllValues = true;
                listOfAsiaPacific[0].lowJson = 0;
                listOfAsiaPacific[0].highJson = result;
                listOfAsiaPacific[0].sizeFrequency = sizeFrequency;
                return Json(listOfAsiaPacific, new System.Text.Json.JsonSerializerOptions());
                
                
            }
            catch (Exception ex)
            {
                

                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                List<string> errors = new List<string>();

                errors.Add(ex.Message);

                return Json(errors);
            }
        }

        public string AllocationFourthValue { get; set; }

        public AllocationSearchController(
       IHttpContextAccessor httpContextAccessor,
       AllocationDBContext conAll)
        {

            user = httpContextAccessor.HttpContext.User;
            _conAll = conAll;
            
        }

        private List<AsiaPacific> getPrintData(string AllVal1, string AllVal2, string AllVal3, string AllVal4, string FromVal, string ToVal, int FrequencySizeVal, string FreqTable, string[] FreqTableVal)
        {
            JsonResult data = null;
            List<AsiaPacific> allocs;
            if (AllVal1 != "1")
            {
                data = SearchFirstLevel(AllVal1, FromVal, ToVal, FrequencySizeVal, FreqTable, FreqTableVal);
                string jsonResult = JsonConvert.SerializeObject(data.Value);
                allocs = JsonConvert.DeserializeObject<List<AsiaPacific>>(jsonResult);
            }
            else if (AllVal2 != "1")
            {
                data = SearchSecondLevel(AllVal2, FromVal, ToVal, FrequencySizeVal, FreqTable);
                string jsonResult = JsonConvert.SerializeObject(data.Value);
                allocs = JsonConvert.DeserializeObject<List<AsiaPacific>>(jsonResult);
            }
            else if (AllVal3 != "1")
            {
                data = SearchThirdLevel(AllVal3, FromVal, ToVal, FrequencySizeVal, FreqTable);
                string jsonResult = JsonConvert.SerializeObject(data.Value);
                allocs = JsonConvert.DeserializeObject<List<AsiaPacific>>(jsonResult);
            }
            else if (AllVal4 != "<all allocation terms >")
            {
                data = SearchFourthLevel(AllVal4, FromVal, ToVal, FrequencySizeVal, FreqTable);
                string jsonResult = JsonConvert.SerializeObject(data.Value);
                allocs = JsonConvert.DeserializeObject<List<AsiaPacific>>(jsonResult);
            }
            else
            {
                data = SearchOnlyValues(FromVal, ToVal, FrequencySizeVal, FreqTable, FreqTableVal);
                string jsonResult = JsonConvert.SerializeObject(data.Value);
                allocs = JsonConvert.DeserializeObject<List<AsiaPacific>>(jsonResult);
            }

            return allocs;
        }

        private class CustomStringComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                if (x == null && y == null)
                {
                    return 0;
                }
                else if (x == null)
                {
                    return -1;
                }
                else if (y == null)
                {
                    return 1;
                }

                bool xHasMultipleCaps = x.Count(char.IsUpper) > 1;
                bool yHasMultipleCaps = y.Count(char.IsUpper) > 1;

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
                    return string.Compare(x, y, StringComparison.Ordinal);
                }
                else
                {
                    return string.Compare(x, y, StringComparison.Ordinal);
                }
            }
        }

        private byte[] GenerateExcel(List<AsiaPacific> allocs)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                // Set column widths
                worksheet.Column(1).Width = 30;  // Column A - Allocations for first region
                worksheet.Column(2).Width = 20;  // Column A - Allocations for first region

                if (allocs.Count() > 1)
                {
                    worksheet.Column(3).Width = 30;  // Column C - Allocations for 2nd region
                    worksheet.Column(4).Width = 20;  // Column C - Allocations for 2nd region
                }
                else worksheet.Column(3).Width = 0;

                // Merge cells and write title
                worksheet.Cells["A1:D2"].Merge = true;
                worksheet.Cells["A1"].Value = "Allocations";
                worksheet.Cells["A1:D2"].Style.Font.Color.SetColor(Color.FromArgb(0x92, 0xD0, 0x50));
                worksheet.Cells["A1"].Style.Font.Size = 16;
                worksheet.Cells["A1"].Style.Font.Name = "Calibri";
                worksheet.Cells["A1"].Style.Font.Bold = true;
                worksheet.Cells["A1"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center; // Align in the middle

                string title0 = "";
                string title1 = "";

                bool twoRegs = false;
                AsiaPacific temp;
                if (allocs.Count > 1)
                {
                    //ITU needs to go first
                    temp = allocs[0];
                    allocs[0] = allocs[1];
                    allocs[1] = temp;
                }

                if (allocs[0].ListOfFreqBand[0].regionCode == "PSE")
                {
                    title0 = "National Allocations";
                }
                else
                {
                    title0 = "ITU RR Region 1 Allocations";
                }

                if (allocs.Count() > 1)
                {
                    twoRegs = true;
                    if (allocs[1].ListOfFreqBand[1].regionCode == "PSE")
                    {
                        title1 = "National Allocations";
                    }
                    else
                    {
                        title1 = "ITU RR Region 1 Allocations";
                    }
                }


                // Format the third row
                OfficeOpenXml.Style.ExcelStyle headerStyle;
                worksheet.Cells["A3"].Value = title0;
                if (twoRegs)
                {
                    worksheet.Cells["A3:B3"].Merge = true;
                    worksheet.Cells["C3:D3"].Merge = true;
                    worksheet.Cells["C3"].Value = title1;

                    headerStyle = worksheet.Cells["A3:D3"].Style;
                } else
                {
                    worksheet.Cells["A3:B3"].Merge = true;

                    headerStyle = worksheet.Cells["A3:B3"].Style;
                }

                headerStyle.Font.Bold = true;
                headerStyle.Font.Italic = true;
                headerStyle.Font.Name = "Calibri";
                headerStyle.Font.Size = 14;
                headerStyle.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                headerStyle.Fill.BackgroundColor.SetColor(Color.FromArgb(0xC6, 0xE0, 0xB4));

                bool swap = true;

                int currentRow = 4;
                

                for(int i = 0; i< allocs[0].ListOfFreqBand.Count(); i++) //iterate over the list of bands
                {
                    int firstRow = currentRow;
                    //postavlja se prvi red (vrijednosti za low i high)
                    worksheet.Cells["A" + currentRow + ":B" + currentRow].Merge = true;
                    worksheet.Cells["A" + currentRow].Value = allocs[0].ListOfFreqBand[i].LowView + " - " + allocs[0].ListOfFreqBand[i].HighView;

                    currentRow += 2;//red pauze pa idu podaci
                    int maxRow = currentRow;

                    string[] allocs1 = allocs[0].ListOfFreqBand[i].Allocation.Split(',').Select(s => s.Trim()).ToArray();
                    allocs1 = allocs1.OrderBy(s => s, new CustomStringComparer())
                          .ToArray();

                    foreach (string alloc in allocs1)
                    {
                        if(alloc != "")
                        {
                            string allocView = alloc;
                            worksheet.Cells["A" + currentRow].Value = alloc;
                            worksheet.Cells["A" + currentRow + ":B" + currentRow].Merge = true;
                            foreach (FootnoteJsonConvert footnote in allocs[0].ListOfFreqBand[i].Footnote)
                            {
                                if (footnote.Allocation == alloc) //if footnote descr isn't empty
                                {
                                    if (footnote.Footnote != "")
                                    {
                                        allocView += "  " + footnote.Footnote;
                                        if (worksheet.Cells["A" + currentRow].Value.ToString().Length + footnote.Footnote.Length > 50)
                                        {
                                            //so that it doesn't overflow to the next column
                                            currentRow++;
                                            worksheet.Cells["A" + currentRow].Value += footnote.Footnote;
                                        }
                                        else
                                        {
                                            worksheet.Cells["A" + currentRow].Value += "    " + footnote.Footnote;
                                        }
                                    }
                                }
                            }
                            currentRow++;
                        }
                    }
                    int psesRow = currentRow;
                    int bandRow = currentRow;
                    foreach (FootnoteJsonConvert footnote in allocs[0].ListOfFreqBand[i].BandFootnote)
                    {
                        if (footnote.Footnote != "")
                        {
                            if (footnote.Footnote.Contains("PSE"))
                            {
                                worksheet.Cells["B" + psesRow].Value += footnote.Footnote;
                                worksheet.Cells["B" + psesRow].Style.Font.Bold = true;
                                psesRow++;
                            }
                            else
                            {
                                worksheet.Cells["A" + bandRow].Value += footnote.Footnote;
                                bandRow++;
                            }
                        }
                    }

                    currentRow = Math.Max(psesRow, bandRow);

                    if (twoRegs)
                    {
                        string[] allocs2 = allocs[1].ListOfFreqBand[i].Allocation.Split(',').Select(s => s.Trim()).ToArray();
                        allocs2 = allocs2.OrderBy(s => s, new CustomStringComparer())
                          .ToArray();
                        maxRow = currentRow;
                        currentRow = firstRow+2;

                        foreach (string alloc in allocs2)
                        {
                            if(alloc != "")
                            {
                                string allocView = alloc;
                                worksheet.Cells["C" + currentRow].Value = alloc;
                                worksheet.Cells["C" + currentRow + ":D" + currentRow].Merge = true;
                                foreach (FootnoteJsonConvert footnote in allocs[1].ListOfFreqBand[i].Footnote)
                                {
                                    if (footnote.Allocation == alloc) //if footnote descr isn't empty
                                    {
                                        if (footnote.Footnote != "")
                                        {
                                            allocView += "  " + footnote.Footnote;
                                            if (worksheet.Cells["C" + currentRow].Value.ToString().Length + footnote.Footnote.Length > 50)
                                            {
                                                //so that it doesn't overflow to the next column
                                                currentRow++;
                                                worksheet.Cells["C" + currentRow].Value += footnote.Footnote;
                                            }
                                            else
                                            {
                                                worksheet.Cells["C" + currentRow].Value += "    " + footnote.Footnote;
                                            }
                                        }
                                    }
                                }
                                currentRow++;
                            }
                        }

                        psesRow = currentRow;
                        bandRow = currentRow;
                        foreach (FootnoteJsonConvert footnote in allocs[1].ListOfFreqBand[i].BandFootnote)
                        {
                            if (footnote.Footnote != "")
                            {
                                if (footnote.Footnote.Contains("PSE"))
                                {
                                    worksheet.Cells["D" + psesRow].Value += footnote.Footnote;
                                    worksheet.Cells["D" + psesRow].Style.Font.Bold = true;
                                    psesRow++;
                                }
                                else
                                {
                                    worksheet.Cells["C" + bandRow].Value += footnote.Footnote;
                                    bandRow++;
                                }
                            }
                        }
                        currentRow = Math.Max(psesRow, bandRow);
                    }
                    currentRow = Math.Max(maxRow, currentRow);

                    string cells = "";
                    //postavimo boju za ovaj red iz freqBanda
                    if (twoRegs)
                    {
                        cells = "A" + firstRow + ":D" + currentRow;
                    }
                    else
                    {
                        cells = "A" + firstRow + ":B" + currentRow;
                    }
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

        private byte[] GenerateExcel150(List<AsiaPacific> allocs)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                // Set column widths
                worksheet.Column(1).Width = 30;  // Column A - Allocations for first region
                worksheet.Column(2).Width = 20;  // Column A - Allocations for first region

                if (allocs.Count() > 1)
                {
                    worksheet.Column(3).Width = 30;  // Column C - Allocations for 2nd region
                    worksheet.Column(4).Width = 20;  // Column C - Allocations for 2nd region
                }
                else worksheet.Column(3).Width = 0;

                // Merge cells and write title
                worksheet.Cells["A1:D2"].Merge = true;
                worksheet.Cells["A1"].Value = "Allocations";
                worksheet.Cells["A1:D2"].Style.Font.Color.SetColor(Color.FromArgb(0x92, 0xD0, 0x50));
                worksheet.Cells["A1"].Style.Font.Size = 16;
                worksheet.Cells["A1"].Style.Font.Name = "Calibri";
                worksheet.Cells["A1"].Style.Font.Bold = true;
                worksheet.Cells["A1"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center; // Align in the middle

                string title0 = "";
                string title1 = "";

                bool twoRegs = false;
                AsiaPacific temp;
                if (allocs.Count > 1)
                {
                    //ITU needs to go first
                    temp = allocs[0];
                    allocs[0] = allocs[1];
                    allocs[1] = temp;
                }

                if (allocs[0].ListOfFreqBand[0].regionCode == "PSE")
                {
                    title0 = "National Allocations";
                }
                else
                {
                    title0 = "ITU RR Region 1 Allocations";
                }

                if (allocs.Count() > 1)
                {
                    twoRegs = true;
                    if (allocs[1].ListOfFreqBand[1].regionCode == "PSE")
                    {
                        title1 = "National Allocations";
                    }
                    else
                    {
                        title1 = "ITU RR Region 1 Allocations";
                    }
                }


                // Format the third row
                OfficeOpenXml.Style.ExcelStyle headerStyle;
                worksheet.Cells["A3"].Value = title0;
                if (twoRegs)
                {
                    worksheet.Cells["A3:B3"].Merge = true;
                    worksheet.Cells["C3:D3"].Merge = true;
                    worksheet.Cells["C3"].Value = title1;

                    headerStyle = worksheet.Cells["A3:D3"].Style;
                }
                else
                {
                    worksheet.Cells["A3:B3"].Merge = true;

                    headerStyle = worksheet.Cells["A3:B3"].Style;
                }

                headerStyle.Font.Bold = true;
                headerStyle.Font.Italic = true;
                headerStyle.Font.Name = "Calibri";
                headerStyle.Font.Size = 14;
                headerStyle.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                headerStyle.Fill.BackgroundColor.SetColor(Color.FromArgb(0xC6, 0xE0, 0xB4));

                bool swap = true;

                int currentRow = 4;


                for (int i = 0; i < allocs[0].ListOfFreqBand.Count(); i++) //iterate over the list of bands
                {
                    if (currentRow > 100)
                    {
                        break;
                    }
                    int firstRow = currentRow;
                    //postavlja se prvi red (vrijednosti za low i high)
                    worksheet.Cells["A" + currentRow + ":B" + currentRow].Merge = true;
                    worksheet.Cells["A" + currentRow].Value = allocs[0].ListOfFreqBand[i].LowView + " - " + allocs[0].ListOfFreqBand[i].HighView;

                    currentRow += 2;//red pauze pa idu podaci
                    int maxRow = currentRow;

                    string[] allocs1 = allocs[0].ListOfFreqBand[i].Allocation.Split(',').Select(s => s.Trim()).ToArray();
                    allocs1 = allocs1.OrderBy(s => s, new CustomStringComparer())
                          .ToArray();

                    foreach (string alloc in allocs1)
                    {
                        if (alloc != "")
                        {
                            string allocView = alloc;
                            worksheet.Cells["A" + currentRow].Value = alloc;
                            worksheet.Cells["A" + currentRow + ":B" + currentRow].Merge = true;
                            foreach (FootnoteJsonConvert footnote in allocs[0].ListOfFreqBand[i].Footnote)
                            {
                                if (footnote.Allocation == alloc) //if footnote descr isn't empty
                                {
                                    if (footnote.Footnote != "")
                                    {
                                        allocView += "  " + footnote.Footnote;
                                        if (worksheet.Cells["A" + currentRow].Value.ToString().Length + footnote.Footnote.Length > 50)
                                        {
                                            //so that it doesn't overflow to the next column
                                            currentRow++;
                                            worksheet.Cells["A" + currentRow].Value += footnote.Footnote;
                                        }
                                        else
                                        {
                                            worksheet.Cells["A" + currentRow].Value += "    " + footnote.Footnote;
                                        }
                                    }
                                }
                            }
                            currentRow++;
                        }
                    }
                    int psesRow = currentRow;
                    int bandRow = currentRow;
                    foreach (FootnoteJsonConvert footnote in allocs[0].ListOfFreqBand[i].BandFootnote)
                    {
                        if (footnote.Footnote != "")
                        {
                            if (footnote.Footnote.Contains("PSE"))
                            {
                                worksheet.Cells["B" + psesRow].Value += footnote.Footnote;
                                worksheet.Cells["B" + psesRow].Style.Font.Bold = true;
                                psesRow++;
                            }
                            else
                            {
                                worksheet.Cells["A" + bandRow].Value += footnote.Footnote;
                                bandRow++;
                            }
                        }
                    }
                    currentRow = Math.Max(psesRow, bandRow);

                    if (twoRegs)
                    {
                        string[] allocs2 = allocs[1].ListOfFreqBand[i].Allocation.Split(',').Select(s => s.Trim()).ToArray();
                        allocs2 = allocs2.OrderBy(s => s, new CustomStringComparer())
                          .ToArray();
                        maxRow = currentRow;
                        currentRow = firstRow + 2;

                        foreach (string alloc in allocs2)
                        {
                            if (alloc != "")
                            {
                                string allocView = alloc;
                                worksheet.Cells["C" + currentRow].Value = alloc;
                                worksheet.Cells["C" + currentRow + ":D" + currentRow].Merge = true;
                                foreach (FootnoteJsonConvert footnote in allocs[1].ListOfFreqBand[i].Footnote)
                                {
                                    if (footnote.Allocation == alloc) //if footnote descr isn't empty
                                    {
                                        if (footnote.Footnote != "")
                                        {
                                            allocView += "  " + footnote.Footnote;
                                            if (worksheet.Cells["C" + currentRow].Value.ToString().Length + footnote.Footnote.Length > 50)
                                            {
                                                //so that it doesn't overflow to the next column
                                                currentRow++;
                                                worksheet.Cells["C" + currentRow].Value += footnote.Footnote;
                                            }
                                            else
                                            {
                                                worksheet.Cells["C" + currentRow].Value += "    " + footnote.Footnote;
                                            }
                                        }
                                    }
                                }
                                currentRow++;
                            }
                        }

                        psesRow = currentRow;
                        bandRow = currentRow;
                        foreach (FootnoteJsonConvert footnote in allocs[1].ListOfFreqBand[i].BandFootnote)
                        {
                            if (footnote.Footnote != "")
                            {
                                if (footnote.Footnote.Contains("PSE"))
                                {
                                    worksheet.Cells["D" + psesRow].Value += footnote.Footnote;
                                    worksheet.Cells["D" + psesRow].Style.Font.Bold = true;
                                    psesRow++;
                                }
                                else
                                {
                                    worksheet.Cells["C" + bandRow].Value += footnote.Footnote;
                                    bandRow++;
                                }
                            }
                        }
                        currentRow = Math.Max(psesRow, bandRow);
                    }
                    currentRow = Math.Max(maxRow, currentRow);

                    string cells = "";
                    //postavimo boju za ovaj red iz freqBanda
                    if (twoRegs)
                    {
                        cells = "A" + firstRow + ":D" + currentRow;
                    }
                    else
                    {
                        cells = "A" + firstRow + ":B" + currentRow;
                    }
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
        [Route("AllocationSearch/DownloadExcel")]
        public byte[] DownloadExcel(string AllVal1, string AllVal2, string AllVal3, string AllVal4, string FromVal, string ToVal, int FrequencySizeVal, string FreqTable, string[] FreqTableVal)
        {
            List<AsiaPacific> allocs = getPrintData(AllVal1, AllVal2, AllVal3, AllVal4, FromVal, ToVal, FrequencySizeVal, FreqTable, FreqTableVal);
            return GenerateExcel(allocs);
        }

        //[EnableQuery]
        [HttpPost]
        [Route("AllocationSearch/DownloadPDF")]
        public byte[] DownloadPDF(string AllVal1, string AllVal2, string AllVal3, string AllVal4, string FromVal, string ToVal, int FrequencySizeVal, string FreqTable, string[] FreqTableVal)
        {
            List<AsiaPacific> allocs = getPrintData(AllVal1, AllVal2, AllVal3, AllVal4, FromVal, ToVal, FrequencySizeVal, FreqTable, FreqTableVal);
            byte[] excelBytes = GenerateExcel150(allocs);
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


        public IActionResult Index()
        {
            //Debug.WriteLine("vv");
            //execute this condition when user search allocation from home or start page.
            if (TempData["al"] != null)
            {
                bool value = (bool)TempData["al"];
                if (value == true)
                {
                    //Debug.WriteLine(" al");
                    string from = (string)TempData["tempFrom"];
                    string to = (string)TempData["tempTo"];

                   
                    long valueTo = long.Parse(to);
                    long valueFrom = long.Parse(from);
                    FrequencySizeValue = (string)TempData["FrequencySize"];
                    //take value from frequency size.
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
                    if (this.FrequencytableValue == "0")
                    {
                        return View();
                    }
                    
                   
                    FrequencytableValue = (string)TempData["FrequencytableValue"];
                    string tempValueFreq = FrequencyTablesList.Where(p => p.Value.Equals(FrequencytableValue)).First().Text;
                    FrequencyTablesList.FirstOrDefault(p => p.Value.Equals(FrequencytableValue)).Selected = true;
                    //List<AllocationSearch> listAl = JsonConvert.DeserializeObject<List<AllocationSearch>>(TempData["allocations"].ToString());
                    //Debug.WriteLine("after al:"+ FrequencytableValue+","+ tempValue);
                    long tempFrom = long.Parse(from);
                    long tempTo = long.Parse(to);
                    if (tempFrom == 0 && tempTo == 0)
                    {

                        AllocationSearchActions asActions = new AllocationSearchActions();
                        var listGeneral = asActions.SearchValuesAll(_conAll, tempValueFreq);
                        var NewListGeneral = listGeneral.OrderBy(e => e.low).ToList();
                        NewListGeneral[0].FrequencyTablesList = FrequencyTablesList;
                        NewListGeneral[0].FrequencyTableValue = FrequencytableValue;
                        NewListGeneral[0].FrequencySizeValue = FrequencySizeValue;
                        return View(NewListGeneral);

                    }
                    else if (tempFrom == 0 && tempTo != 0)
                    {
                        //Debug.WriteLine("nula je");
                       
                        AllocationSearchActions asActions = new AllocationSearchActions();
                        var listGeneral = asActions.SearchValuesZeroTo(_conAll, ""+tempFrom,""+ tempTo,tempValueFreq);
                        var NewListGeneral = listGeneral.OrderBy(e => e.low).ToList();
                        NewListGeneral[0].FrequencyTablesList = FrequencyTablesList;
                        NewListGeneral[0].FrequencyTableValue = FrequencytableValue;
                        NewListGeneral[0].FrequencySizeValue = FrequencySizeValue;
                        return View(NewListGeneral);
                    }
                    else if (tempFrom != 0 && tempTo != 0)
                    {


                        AllocationSearchActions alSa = new AllocationSearchActions();
                        var listGeneral = alSa.SearchValuesFromTo(_conAll, "" + tempFrom, "" + tempTo, tempValueFreq, FrequencyTablesList, FrequencytableValue);
                        //Debug.WriteLine("vvv:" + listGeneral[0].FrequencyTablesList.Count);
                        //listGeneral[0].FrequencyTablesList = new List<SelectListItem>();
                        //listGeneral[0].FrequencyTablesList = FrequencyTablesList;

                      
                        //listGeneral[0].FrequencyTableValue = FrequencytableValue;
                        //Debug.WriteLine("count:" + FrequencyTablesList.Count + "===" + tempFrom);
                       
                        var NewListGeneral = listGeneral.OrderBy(e => e.low).ToList();
                        NewListGeneral[0].FrequencyTablesList = FrequencyTablesList;
                        NewListGeneral[0].FrequencyTableValue = FrequencytableValue;
                        NewListGeneral[0].FrequencySizeValue = FrequencySizeValue;
                        return View(NewListGeneral);

                    }

                }
                else
                {
                    return View();
                }
            }
            else
            {
                //Debug.WriteLine("tu sam:");
                
                return View();
            }
            //Debug.WriteLine("values:");
            return View();
            //Debug.WriteLine("value:"+TempData["pp"]);

        }
        
        
    }

    
}
