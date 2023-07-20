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
using FrekvencijeProject.Models.Ajax;

namespace FrekvencijeProject.Controllers
{
    public class AllocationSearchController : Controller
    {
        object user;
        private readonly IViewRenderService _viewRenderService;

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
        public JsonResult SearchFirstLevel(string AllVal, string FromVal, string ToVal, int FrequencySizeVal, string FreqTable, string[] FreqTableVal)
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
        public JsonResult SearchSecondLevel(string AllVal, string FromVal, string ToVal, int FrequencySizeVal, string FreqTable)
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
        public JsonResult SearchFourthLevel(string AllVal, string FromVal, string ToVal, int FrequencySizeVal, string FreqTable)
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
        public JsonResult SearchOnlyValues(string FromVal, string ToVal, int FrequencySizeVal, string FreqTable, string[] FreqTableVal)
        {

            //try
            //{

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


            int sizeFrequency = FrequencySizeVal;
            long result = 0;
            if (tempFrom == 0 && tempTo == 0)
            {
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
            }
            if (FreqTableVal.Length == 1)
            {
                listOfAsiaPacific[0].isAllValues = true;
                if (tempFrom == 0 && tempTo == 0)
                {
                    listOfAsiaPacific[0].lowJson = 0;
                    listOfAsiaPacific[0].highJson = result;
                }
                else
                {
                    listOfAsiaPacific[0].lowJson = tempFrom;
                    listOfAsiaPacific[0].highJson = tempTo;
                }
                listOfAsiaPacific[0].sizeFrequency = sizeFrequency;
                return Json(listOfAsiaPacific, new System.Text.Json.JsonSerializerOptions());
            }
            else
            {

                List<AsiaPacificSingleTable> listGeneral = new List<AsiaPacificSingleTable>();
                List<long> listOfLowFreq = new List<long>();
                foreach (var tempLow in listOfAsiaPacific)
                {
                    var duplicates = tempLow.ListOfFreqBand.OrderBy(e => e.low).ToList();

                    //Debug.WriteLine("oooppp"+ duplicates.Count());

                    foreach (var d in duplicates)
                    {

                        List<string> listOfAllocation = new List<string>();
                        List<FootnoteJsonConvert> listOfFootnote = new List<FootnoteJsonConvert>();
                        List<FootnoteJsonConvert> listOfBandFootnote = new List<FootnoteJsonConvert>();
                        List<string> tempListOfFootnote = new List<string>();
                        List<string> listOfComments = new List<string>();

                        listOfAllocation.Add(d.Allocation);
                        listOfComments.Add(d.Comment);

                        //Debug.WriteLine("before if:" + d.low+":::"+d.regionName);
                        if (!listOfLowFreq.Contains(d.low))
                        {
                            //Debug.WriteLine("start:");
                            AsiaPacificSingleTable ge = new AsiaPacificSingleTable();
                            listOfLowFreq.Add(d.low);
                            ge.low = d.low;
                            ge.high = d.high;
                            ge.LowView = d.LowView;
                            ge.HighView = d.HighView;
                            ge.AllocationList = new List<AllocationConvert>();

                            AllocationConvert al = new AllocationConvert();
                            al.Allocation = "";
                            al.regionName = d.regionName;
                            al.Footnote = d.Footnote;
                            al.BandFootnote = d.BandFootnote;



                            ge.AllocationList.Add(al);
                            ge.Allocation = d.Allocation;
                            ge.colorCode = d.colorCode;
                            ge.TermId = d.TermId;
                            ge.Comment = d.Comment;
                            ge.regionCode = d.regionCode;
                            ge.regionName = d.regionName;
                            //ge.Footnote = ge.Footnote.OrderByDescending(t => t.isPrimary).ToList();
                            listGeneral.Add(ge);
                            //Debug.WriteLine("here");
                        }
                        else
                        {

                            var temp = listGeneral.Find(x => x.low == d.low);
                            //Debug.WriteLine("low::"+temp.low+"==count:"+ temp.AllocationList.Count+":count footnote:" + temp.AllocationList[0].Footnote.Count + "::" + d.Footnote.Count);
                            //Debug.WriteLine("region name:" + d.regionName+"======================");
                            AllocationConvert al = new AllocationConvert();
                            al.Allocation = d.Allocation;
                            al.regionName = d.regionName;
                            al.Footnote = new List<FootnoteJsonConvert>();
                            al.Footnote = d.Footnote;
                            al.BandFootnote = d.BandFootnote;
                            if (!temp.AllocationList.Contains(al))
                            {
                                listGeneral.Find(x => x.low == d.low).AllocationList.Add(al);
                            }

                        }

                    }
                }
                listGeneral[0].isAllValues = true;
                if (tempFrom == 0 && tempTo == 0)
                {
                    listGeneral[0].lowJson = 0;
                    listGeneral[0].highJson = result;
                }
                else
                {

                    listGeneral[0].lowJson = long.Parse(FromVal);
                    listGeneral[0].highJson = long.Parse(ToVal);
                }
                listGeneral[0].sizeFrequency = sizeFrequency;
                return Json(listGeneral, new System.Text.Json.JsonSerializerOptions());

            }


            //listGeneral = listGeneral.OrderBy(e => e.low).ToList();
            //listGeneral[0].isAllValues = true;
            //listGeneral[0].lowJson = 0;
            //listGeneral[0].highJson = result;
            //listGeneral[0].sizeFrequency = sizeFrequency;

            //}
            //catch (Exception ex)
            //{


            //    Response.StatusCode = (int)HttpStatusCode.BadRequest;
            //    List<string> errors = new List<string>();

            //    errors.Add(ex.Message);
            //    Debug.WriteLine("hello:" + ex.Message);
            //    return Json(errors);
            //}
        }

        public string AllocationFourthValue { get; set; }

        public AllocationSearchController(
       IHttpContextAccessor httpContextAccessor,
       AllocationDBContext conAll, IViewRenderService viewRenderService
       )
        {

            user = httpContextAccessor.HttpContext.User;
            _conAll = conAll;
            _viewRenderService = viewRenderService;
        }

        //[EnableQuery]


        [HttpGet]
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

                    string[] FreqTableVal = (string[])TempData["FrequencytableValue"];
                    //Debug.WriteLine("pp:" + FreqTableVal.Length+"test:"+ TempData["FrequencytableValue"]);
                    //string tempValueFreq = FrequencyTablesList.Where(p => p.Value.Equals(FrequencytableValue)).First().Text;
                    //FrequencyTablesList.FirstOrDefault(p => p.Value.Equals(FrequencytableValue)).Selected = true;
                    List<AsiaPacific> listOfAsiaPacific = new List<AsiaPacific>();
                    //var listGeneral = asa.SearchValues(_conAll, tempFrom, tempTo, tempFreq);
                    List<AsiaPacificSingleTable> listGeneralAsia = new List<AsiaPacificSingleTable>();
                    long tempFrom = long.Parse(from);
                    long tempTo = long.Parse(to);
                    if (FreqTableVal.Count() == 1)
                    {
                        Debug.WriteLine("inside of this:" + FreqTableVal.ElementAt(0));
                        string tempValueFreq = FrequencyTablesList.Where(p => p.Value.Equals(FreqTableVal.ElementAt(0))).First().Text;
                        Debug.WriteLine("rest of testing:" + tempValueFreq + "::" + tempFrom + "==" + tempTo);
                        FrequencyTablesList.FirstOrDefault(p => p.Value.Equals(FreqTableVal.ElementAt(0))).Selected = true;
                        ViewBag.Table = "single";
                        if (tempFrom == 0 && tempTo == 0)
                        {

                            AllocationSearchActions asActions = new AllocationSearchActions();
                            var listGeneral = asActions.SearchValuesAll(_conAll, tempValueFreq);
                            AsiaPacific asia = new AsiaPacific();
                            asia.ListOfFreqBand = new List<FreqBandSearchNew>();
                            asia.ListOfFreqBand.AddRange(listGeneral.OrderBy(e => e.low).ToList());


                            listOfAsiaPacific.Add(asia);

                            listOfAsiaPacific[0].FrequencyTablesList = FrequencyTablesList;
                            listOfAsiaPacific[0].FrequencyTableValue = FreqTableVal.ElementAt(0);
                            listOfAsiaPacific[0].FrequencySizeValue = FrequencySizeValue;
                            var tupleModel = new Tuple<List<AsiaPacific>, List<AsiaPacificSingleTable>>(listOfAsiaPacific, listGeneralAsia);
                            return View(tupleModel);

                        }
                        else if (tempFrom == 0 && tempTo != 0)
                        {


                            AllocationSearchActions asActions = new AllocationSearchActions();
                            var listGeneral = asActions.SearchValuesZeroTo(_conAll, "" + tempFrom, "" + tempTo, tempValueFreq);
                            //var NewListGeneral = listGeneral.OrderBy(e => e.low).ToList();
                            //NewListGeneral[0].FrequencyTablesList = FrequencyTablesList;
                            //NewListGeneral[0].FrequencyTableValue = FreqTableVal[0];
                            //NewListGeneral[0].FrequencySizeValue = FrequencySizeValue;
                            //return View(NewListGeneral);
                            AsiaPacific asia = new AsiaPacific();
                            asia.ListOfFreqBand = new List<FreqBandSearchNew>();
                            asia.ListOfFreqBand.AddRange(listGeneral.OrderBy(e => e.low).ToList());


                            listOfAsiaPacific.Add(asia);

                            listOfAsiaPacific[0].FrequencyTablesList = FrequencyTablesList;
                            listOfAsiaPacific[0].FrequencyTableValue = FreqTableVal.ElementAt(0);
                            listOfAsiaPacific[0].FrequencySizeValue = FrequencySizeValue;

                            var tupleModel = new Tuple<List<AsiaPacific>, List<AsiaPacificSingleTable>>(listOfAsiaPacific, listGeneralAsia);
                            return View(tupleModel);
                        }
                        else if (tempFrom != 0 && tempTo != 0)
                        {


                            AllocationSearchActions alSa = new AllocationSearchActions();
                            var listGeneral = alSa.SearchValuesFromTo(_conAll, "" + tempFrom, "" + tempTo, tempValueFreq, FrequencyTablesList, FreqTableVal.ElementAt(0));

                            AsiaPacific asia = new AsiaPacific();
                            asia.ListOfFreqBand = new List<FreqBandSearchNew>();
                            asia.ListOfFreqBand.AddRange(listGeneral.OrderBy(e => e.low).ToList());


                            listOfAsiaPacific.Add(asia);

                            listOfAsiaPacific[0].FrequencyTablesList = FrequencyTablesList;
                            listOfAsiaPacific[0].FrequencyTableValue = FreqTableVal.ElementAt(0);
                            listOfAsiaPacific[0].FrequencySizeValue = FrequencySizeValue;
                            var tupleModel = new Tuple<List<AsiaPacific>, List<AsiaPacificSingleTable>>(listOfAsiaPacific, listGeneralAsia);
                            return View(tupleModel);

                        }

                    }
                    else
                    {
                        ViewBag.Table = "multiple";
                        for (int i = 0; i < FreqTableVal.Count(); i++)
                        {

                            AllocationSearchActions asa = new AllocationSearchActions();
                            var listGeneralElse = asa.SearchValues(_conAll, tempFrom, tempTo, FreqTableVal.ElementAt(i));

                            AsiaPacific asia = new AsiaPacific();
                            asia.ListOfFreqBand = new List<FreqBandSearchNew>();
                            asia.ListOfFreqBand.AddRange(listGeneralElse.OrderBy(e => e.low).ToList());
                            listOfAsiaPacific.Add(asia);
                        }

                        List<long> listOfLowFreq = new List<long>();
                        foreach (var tempLow in listOfAsiaPacific)
                        {
                            var duplicates = tempLow.ListOfFreqBand.OrderBy(e => e.low).ToList();

                            //Debug.WriteLine("oooppp"+ duplicates.Count());

                            foreach (var d in duplicates)
                            {

                                List<string> listOfAllocation = new List<string>();
                                List<FootnoteJsonConvert> listOfFootnote = new List<FootnoteJsonConvert>();
                                List<FootnoteJsonConvert> listOfBandFootnote = new List<FootnoteJsonConvert>();
                                List<string> tempListOfFootnote = new List<string>();
                                List<string> listOfComments = new List<string>();

                                listOfAllocation.Add(d.Allocation);
                                listOfComments.Add(d.Comment);

                                //Debug.WriteLine("before if:" + d.low+":::"+d.regionName);
                                if (!listOfLowFreq.Contains(d.low))
                                {
                                    //Debug.WriteLine("start:");
                                    AsiaPacificSingleTable ge = new AsiaPacificSingleTable();
                                    listOfLowFreq.Add(d.low);
                                    ge.low = d.low;
                                    ge.high = d.high;
                                    ge.LowView = d.LowView;
                                    ge.HighView = d.HighView;
                                    ge.AllocationList = new List<AllocationConvert>();

                                    AllocationConvert al = new AllocationConvert();
                                    al.Allocation = "";
                                    al.regionName = d.regionName;
                                    al.Footnote = d.Footnote;
                                    al.BandFootnote = d.BandFootnote;



                                    ge.AllocationList.Add(al);
                                    ge.Allocation = d.Allocation;
                                    ge.colorCode = d.colorCode;
                                    ge.TermId = d.TermId;
                                    ge.Comment = d.Comment;
                                    ge.regionCode = d.regionCode;
                                    ge.regionName = d.regionName;
                                    //ge.Footnote = ge.Footnote.OrderByDescending(t => t.isPrimary).ToList();
                                    listGeneralAsia.Add(ge);
                                    //Debug.WriteLine("here");
                                }
                                else
                                {

                                    var temp = listGeneralAsia.Find(x => x.low == d.low);
                                    //Debug.WriteLine("low::"+temp.low+"==count:"+ temp.AllocationList.Count+":count footnote:" + temp.AllocationList[0].Footnote.Count + "::" + d.Footnote.Count);
                                    //Debug.WriteLine("region name:" + d.regionName+"======================");
                                    AllocationConvert al = new AllocationConvert();
                                    al.Allocation = d.Allocation;
                                    al.regionName = d.regionName;
                                    al.Footnote = new List<FootnoteJsonConvert>();
                                    al.Footnote = d.Footnote;
                                    al.BandFootnote = d.BandFootnote;
                                    if (!temp.AllocationList.Contains(al))
                                    {
                                        listGeneralAsia.Find(x => x.low == d.low).AllocationList.Add(al);
                                    }

                                }

                            }
                        }
                        listGeneralAsia[0].isAllValues = true;
                        listGeneralAsia[0].FrequencyTablesList = FrequencyTablesList;
                        listGeneralAsia[0].FrequencyTableValue = FreqTableVal.ElementAt(0);
                        listGeneralAsia[0].FrequencySizeValue = FrequencySizeValue;

                        var tupleModel = new Tuple<List<AsiaPacific>, List<AsiaPacificSingleTable>>(listOfAsiaPacific, listGeneralAsia);

                        return View(tupleModel);

                    }

                }
                else
                {
                    //Debug.WriteLine("tu sam:");

                    return View();
                }
                //Debug.WriteLine("values:");

                //Debug.WriteLine("value:"+TempData["pp"]);

            }
            else
            {
                return View();
            }
            return View();
        }
        public IActionResult GraphicalSearch()
        {
            ViewBag.GraphicalSearch = "Allocation";
            return View();
        }
        public async Task<IActionResult> CompareSearch(string FromVal, string ToVal, int FrequencySizeVal, string[] FrequencyTableVal, string orientation)
        {
            if (FromVal == "3000" && ToVal == "30" && FrequencySizeVal == 3)
            {
                FromVal = "3";
                ToVal = "30";
                FrequencySizeVal = 4;
            }
            if (FromVal == "3000" && ToVal == "30" && FrequencySizeVal == 2)
            {
                FromVal = "3";
                ToVal = "30";
                FrequencySizeVal = 3;
            }
            AllocationSearchActions asa = new AllocationSearchActions();
            List<AsiaPacific> listOfAsiaPacific = new List<AsiaPacific>();
            try
            {
                string tempValueAllocation = AllocationAll;
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
                        tempTo = valueTo;
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

                if (tempFrom == 0 && tempTo == 0)
                {
                    listOfAsiaPacific = asa.getlistOfAsiaPacific(_conAll, FrequencyTableVal, tempFrom, tempTo);
                    if (listOfAsiaPacific.Count > 0)
                    {
                        listOfAsiaPacific[0].ListOfFreqBand[0].orientation = orientation;
                        listOfAsiaPacific[1].ListOfFreqBand[0].orientation = orientation;
                    }
                    var AsiaPacific = await _viewRenderService.RenderToStringAsync("_CompareAllocationGraphicalGrid", listOfAsiaPacific);
                    return Json(AsiaPacific, new System.Text.Json.JsonSerializerOptions());
                }
                else if (tempFrom == 0 && tempTo != 0)
                {
                    listOfAsiaPacific = asa.getlistOfAsiaPacific(_conAll, FrequencyTableVal, tempFrom, tempTo);
                    if (listOfAsiaPacific.Count > 0)
                    {
                        listOfAsiaPacific[0].ListOfFreqBand[0].orientation = orientation;
                        listOfAsiaPacific[1].ListOfFreqBand[0].orientation = orientation;
                    }
                    var AsiaPacific = await _viewRenderService.RenderToStringAsync("_CompareAllocationGraphicalGrid", listOfAsiaPacific);
                    return Json(AsiaPacific, new System.Text.Json.JsonSerializerOptions());
                }
                else if (tempFrom != 0 && tempTo != 0)
                {
                    listOfAsiaPacific = asa.getlistOfAsiaPacific(_conAll, FrequencyTableVal, tempFrom, tempTo);
                    if (listOfAsiaPacific.Count > 0)
                    {
                        listOfAsiaPacific.Reverse();
                        listOfAsiaPacific[0].ListOfFreqBand[0].orientation = orientation;
                        listOfAsiaPacific[1].ListOfFreqBand[0].orientation = orientation;
                    }
                    var AsiaPacific = await _viewRenderService.RenderToStringAsync("_CompareAllocationGraphicalGrid", listOfAsiaPacific);
                    return Json(AsiaPacific, new System.Text.Json.JsonSerializerOptions());
                }
            }
            catch (Exception)
            {
                throw;
            }
            return Json("");
        }
    }


}
