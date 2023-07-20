using FrekvencijeProject.Controllers.Actions;
using FrekvencijeProject.Models;
using FrekvencijeProject.Models.Ajax;
using FrekvencijeProject.Models.Json;
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
using System.Net;
using System.Threading.Tasks;

namespace FrekvencijeProject.Controllers
{
    public class ApplicationSearchController : Controller
    {
        object user;
        private readonly IViewRenderService _viewRenderService;
        private readonly AllocationDBContext _conAll;

        private readonly ApplicationDBContext _conApp;
        private readonly ImportTempTableContext _conImport;
     
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

        [Display(Name = "Application")]
        public string ApplicationTable { get; set; }

        public string ApplicationFirstValue { get; set; }

        public List<SelectListItem> ApplicationFirstList = new List<SelectListItem>
        {
            new SelectListItem { Text = "<all application terms>", Value = "1",Selected = true },
            new SelectListItem { Text = "-", Value = "2" },
            new SelectListItem { Text = "Aeronautical", Value = "3" },
            new SelectListItem { Text = "Broadcasting", Value = "4" },
            new SelectListItem { Text = "Defence systems", Value = "5" },
            new SelectListItem { Text = "Fixed", Value = "6" },
            new SelectListItem { Text = "Land mobile", Value = "7" },
            new SelectListItem { Text = "Maritime", Value = "8" },
            new SelectListItem { Text = "Meteorology", Value = "9" },
            new SelectListItem { Text = "Other", Value = "10" },
            new SelectListItem { Text = "PMSE", Value = "11" },
            new SelectListItem { Text = "Radio astronomy", Value = "12" },
            new SelectListItem { Text = "Radiolocation (civil)", Value = "13" },
            new SelectListItem { Text = "Satellite systems (civil)", Value = "14" },
            new SelectListItem { Text = "Short Range Devices", Value = "15" },
            new SelectListItem { Text = "TRA-ECS", Value = "16" }
        };

        public string ApplicationSecondValue { get; set; }

        public List<SelectListItem> ApplicationSecondList = new List<SelectListItem>
        {
            new SelectListItem { Text = "<all level 2 application terms>", Value = "1",Selected = true },
            new SelectListItem { Text = "Active medical implants", Value = "2" },
            new SelectListItem { Text = "Aeronautical communications", Value = "3" },
            new SelectListItem { Text = "Aeronautical emergency", Value = "4" },
            new SelectListItem { Text = "Aeronautical military systems", Value = "5" },
            new SelectListItem { Text = "Aeronautical navigation", Value = "6" },
            new SelectListItem { Text = "Aeronautical radar", Value = "7" },
            new SelectListItem { Text = "Aeronautical satcoms", Value = "8" },
            new SelectListItem { Text = "Aeronautical surveillance", Value = "9" },
            new SelectListItem { Text = "Aeronautical telemetry/telecommand", Value = "10" },
            new SelectListItem { Text = "Alarms", Value = "11" },
            new SelectListItem { Text = "Amateur", Value = "12" },
            new SelectListItem { Text = "Amateur-satellite", Value = "13" },
            new SelectListItem { Text = "Audio PMSE", Value = "14" },
            new SelectListItem { Text = "BWA", Value = "15" },
            new SelectListItem { Text = "Broadcasting (satellite)", Value = "16" },
            new SelectListItem { Text = "Broadcasting (terrestrial)", Value = "17" },
            new SelectListItem { Text = "CB radio", Value = "18" },
            new SelectListItem { Text = "Continuum measurements", Value = "19" },
            new SelectListItem { Text = "Cordless communications for voice and data", Value = "20" },
            new SelectListItem { Text = "D-GPS", Value = "21" },
            new SelectListItem { Text = "Digital cellular", Value = "22" },
            new SelectListItem { Text = "Earth exploration-satellite", Value = "23" },
            new SelectListItem { Text = "FSS Earth stations", Value = "24" },
            new SelectListItem { Text = "Feeder links", Value = "25" },
            new SelectListItem { Text = "GMDSS", Value = "26" },
            new SelectListItem { Text = "GNSS Pseudolites", Value = "27" },
            new SelectListItem { Text = "GNSS Repeater", Value = "28" },
            new SelectListItem { Text = "HAPS", Value = "29" },
            new SelectListItem { Text = "ISM", Value = "30" },
            new SelectListItem { Text = "ITS", Value = "31" },
            new SelectListItem { Text = "Inductive applications", Value = "32" },
            new SelectListItem { Text = "Inland waterway communications", Value = "33" },
            new SelectListItem { Text = "Inter-satellite links", Value = "34" },
            new SelectListItem { Text = "Land military systems", Value = "35" },
            new SelectListItem { Text = "Land radionavigation", Value = "36" },
            new SelectListItem { Text = "Light detection systems", Value = "37" },
            new SelectListItem { Text = "MBR", Value = "38" },
            new SelectListItem { Text = "MFCN", Value = "39" },
            new SelectListItem { Text = "MFCN", Value = "40" },
            new SelectListItem { Text = "MSS Earth stations", Value = "41" },
            new SelectListItem { Text = "Maritime communications", Value = "42" },
            new SelectListItem { Text = "Maritime military systems", Value = "43" },
            new SelectListItem { Text = "Maritime navigation", Value = "44" },
            new SelectListItem { Text = "Maritime radar", Value = "45" },
            new SelectListItem { Text = "Meteor scatter communications", Value = "46" },
            new SelectListItem { Text = "Meteorological aids (military)", Value = "47" },
            new SelectListItem { Text = "Meteorological satcoms", Value = "48" },
            new SelectListItem { Text = "Model control", Value = "49" },
            new SelectListItem { Text = "Non-beam WPT", Value = "50" },
            new SelectListItem { Text = "Non-specific SRDs", Value = "51" },
            new SelectListItem { Text = "Oceanographic buoys", Value = "52" },
            new SelectListItem { Text = "PMR/PAMR", Value = "53" },
            new SelectListItem { Text = "PPDR", Value = "54" },
            new SelectListItem { Text = "Paging", Value = "55" },
            new SelectListItem { Text = "Point-to-Multipoint", Value = "56" },
            new SelectListItem { Text = "Point-to-Point", Value = "57" },
            new SelectListItem { Text = "RFID", Value = "58" },
            new SelectListItem { Text = "RMR", Value = "59" },
            new SelectListItem { Text = "Radio microphones and ALD", Value = "60" },
            new SelectListItem { Text = "Radiodetermination applications", Value = "61" },
            new SelectListItem { Text = "Radiolocation (civil)", Value = "62" },
            new SelectListItem { Text = "Radiolocation (military)", Value = "63" },
            new SelectListItem { Text = "Railway applications", Value = "64" },
            new SelectListItem { Text = "Satellite navigation systems", Value = "65" },
            new SelectListItem { Text = "Satellite systems (military)", Value = "66" },
            new SelectListItem { Text = "Service links", Value = "67" },
            new SelectListItem { Text = "Sondes", Value = "68" },
            new SelectListItem { Text = "Space operations", Value = "69" },
            new SelectListItem { Text = "Space research", Value = "70" },
            new SelectListItem { Text = "Spectral line observations", Value = "71" },
            new SelectListItem { Text = "Standard frequency and time-signal", Value = "72" },
            new SelectListItem { Text = "Standard frequency and time-signal-satellite", Value = "73" },
            new SelectListItem { Text = "TTT", Value = "74" },
            new SelectListItem { Text = "Telemtry/Telecommand (civil)", Value = "75" },
            new SelectListItem { Text = "Telemtry/Telecommand (military)", Value = "76" },
            new SelectListItem { Text = "Tracking systems", Value = "77" },
            new SelectListItem { Text = "Tracking, tracing and data acquisition", Value = "78" },
            new SelectListItem { Text = "UAS", Value = "79" },
            new SelectListItem { Text = "UWB applications", Value = "80" },
            new SelectListItem { Text = "VLBI observations", Value = "81" },
            new SelectListItem { Text = "Video PMSE", Value = "82" },
            new SelectListItem { Text = "Weather radar", Value = "83" },
            new SelectListItem { Text = "Weather satellites", Value = "84" },
            new SelectListItem { Text = "Wideband data transmission systems", Value = "85" },
            new SelectListItem { Text = "Wind profilers", Value = "86" },
            new SelectListItem { Text = "Wirless audio/multimedia", Value = "87" }
        };


        public string ApplicationThirdValue { get; set; }

        public List<SelectListItem> ApplicationThirdList = new List<SelectListItem>
        {
            new SelectListItem { Text = "<all level 3 application terms>", Value = "1",Selected = true },
            new SelectListItem { Text = "ADS", Value = "2" },
            new SelectListItem { Text = "AES", Value = "3" },
            new SelectListItem { Text = "AGA communications (civil)", Value = "4" },
            new SelectListItem { Text = "AGA communications (military)", Value = "5" },
            new SelectListItem { Text = "AIS", Value = "6" },
            new SelectListItem { Text = "ALD", Value = "7" },
            new SelectListItem { Text = "ALS", Value = "8" },
            new SelectListItem { Text = "AM sound analogue", Value = "9" },
            new SelectListItem { Text = "AMRD Group A", Value = "10" },
            new SelectListItem { Text = "AMRD Group B", Value = "11" },
            new SelectListItem { Text = "ASDE", Value = "12" },
            new SelectListItem { Text = "Active sensor (satellite)", Value = "13" },
            new SelectListItem { Text = "Aeronautical satcoms", Value = "14" },
            new SelectListItem { Text = "Aeronautical telecommand", Value = "15" },
            new SelectListItem { Text = "Aeronautical telemetry", Value = "16" },
            new SelectListItem { Text = "Air-defence radar", Value = "17" },
            new SelectListItem { Text = "Airborne Video Links", Value = "18" },
            new SelectListItem { Text = "Airborne doppler navigation aids", Value = "19" },
            new SelectListItem { Text = "Airborne weather radar", Value = "20" },
            new SelectListItem { Text = "Altimeters", Value = "21" },
            new SelectListItem { Text = "Animal tracking", Value = "22" },
            new SelectListItem { Text = "Asset tracking and tracing", Value = "23" },
            new SelectListItem { Text = "Audio links", Value = "24" },
            new SelectListItem { Text = "Automotive radar", Value = "25" },
            new SelectListItem { Text = "BBDR", Value = "26" },
            new SelectListItem { Text = "BFWA", Value = "27" },
            new SelectListItem { Text = "BMA", Value = "28" },
            new SelectListItem { Text = "Baby monitoring", Value = "29" },
            new SelectListItem { Text = "Beacons (aeronautical)", Value = "30" },
            new SelectListItem { Text = "Beacons (maritime)", Value = "31" },
            new SelectListItem { Text = "CGC", Value = "32" },
            new SelectListItem { Text = "Communcation applications", Value = "33" },
            new SelectListItem { Text = "Cordless cameras", Value = "34" },
            new SelectListItem { Text = "Cordless headphones and loudspeakers", Value = "35" },
            new SelectListItem { Text = "DA2GC", Value = "36" },
            new SelectListItem { Text = "DECT", Value = "37" },
            new SelectListItem { Text = "DME", Value = "38" },
            new SelectListItem { Text = "DRM", Value = "39" },
            new SelectListItem { Text = "DSB/SSB AM CB / CEPT PR 27", Value = "40" },
            new SelectListItem { Text = "DSC", Value = "41" },
            new SelectListItem { Text = "DVB-T", Value = "42" },
            new SelectListItem { Text = "DVB-T2", Value = "43" },
            new SelectListItem { Text = "Deep space (satellite)", Value = "44" },
            new SelectListItem { Text = "Detection of movement and alert", Value = "45" },
            new SelectListItem { Text = "ELT", Value = "46" },
            new SelectListItem { Text = "EPIRBs", Value = "47" },
            new SelectListItem { Text = "ESIM", Value = "48" },
            new SelectListItem { Text = "ESV", Value = "49" },
            new SelectListItem { Text = "Earth exploration-satellite (military)", Value = "50" },
            new SelectListItem { Text = "Emergency detection", Value = "51" },
            new SelectListItem { Text = "Eurobalise", Value = "52" },
            new SelectListItem { Text = "Euroloop", Value = "53" },
            new SelectListItem { Text = "FM sound analogue", Value = "54" },
            new SelectListItem { Text = "FRMCS", Value = "55" },
            new SelectListItem { Text = "FWA", Value = "56" },
            new SelectListItem { Text = "Fixed radio relay (military)", Value = "57" },
            new SelectListItem { Text = "Flying model control", Value = "58" },
            new SelectListItem { Text = "GALILEO", Value = "59" },
            new SelectListItem { Text = "GBAS", Value = "60" },
            new SelectListItem { Text = "GBSAR", Value = "61" },
            new SelectListItem { Text = "GLONASS", Value = "62" },
            new SelectListItem { Text = "GPR/WPR", Value = "63" },
            new SelectListItem { Text = "GPS", Value = "64" },
            new SelectListItem { Text = "GSM", Value = "65" },
            new SelectListItem { Text = "GSM-R", Value = "66" },
            new SelectListItem { Text = "GSO ESOMPs", Value = "67" },
            new SelectListItem { Text = "HEST", Value = "68" },
            new SelectListItem { Text = "IFF", Value = "69" },
            new SelectListItem { Text = "ILS", Value = "70" },
            new SelectListItem { Text = "IMT", Value = "71" },
            new SelectListItem { Text = "IMT-2000 satellite component", Value = "72" },
            new SelectListItem { Text = "INMARSAT", Value = "73" },
            new SelectListItem { Text = "INMARSAT C", Value = "74" },
            new SelectListItem { Text = "In-ear monitors", Value = "75" },
            new SelectListItem { Text = "Inland waterway communications", Value = "76" },
            new SelectListItem { Text = "Inland waterway radar", Value = "77" },
            new SelectListItem { Text = "JTIDS/MIDS", Value = "78" },
            new SelectListItem { Text = "LAES", Value = "79" },
            new SelectListItem { Text = "LEST", Value = "80" },
            new SelectListItem { Text = "LP FM Transmitter", Value = "81" },
            new SelectListItem { Text = "LP-AMI", Value = "82" },
            new SelectListItem { Text = "LPR", Value = "83" },
            new SelectListItem { Text = "LT2", Value = "84" },
            new SelectListItem { Text = "Loran C", Value = "85" },
            new SelectListItem { Text = "MBANS", Value = "86" },
            new SelectListItem { Text = "MCA", Value = "87" },
            new SelectListItem { Text = "MCV", Value = "88" },
            new SelectListItem { Text = "MLS", Value = "89" },
            new SelectListItem { Text = "MSI", Value = "90" },
            new SelectListItem { Text = "MWS", Value = "91" },
            new SelectListItem { Text = "Maritime radar", Value = "92" },
            new SelectListItem { Text = "Maritime Sensing", Value = "93" },
            new SelectListItem { Text = "Active medical implants", Value = "94" },
            new SelectListItem { Text = "Medical telemetry", Value = "95" },
            new SelectListItem { Text = "Meter reading", Value = "96" },
            new SelectListItem { Text = "NAVTEX", Value = "97" },
            new SelectListItem { Text = "NGSO ESOMPs", Value = "98" },
            new SelectListItem { Text = "NGSO FSS", Value = "99" },
            new SelectListItem { Text = "NMR", Value = "100" },
            new SelectListItem { Text = "NP2M", Value = "101" },
            new SelectListItem { Text = "Narrow band analogue voice devices", Value = "102" },
            new SelectListItem { Text = "Obstacle detection radar", Value = "103" },
            new SelectListItem { Text = "On-board communications", Value = "104" },
            new SelectListItem { Text = "On-site pagging", Value = "105" },
            new SelectListItem { Text = "PAMR", Value = "106" },
            new SelectListItem { Text = "PLB", Value = "107" },
            new SelectListItem { Text = "PMR", Value = "108" },
            new SelectListItem { Text = "PMR 446", Value = "109" },
            new SelectListItem { Text = "POCSAG", Value = "110" },
            new SelectListItem { Text = "Passive sensors (satellite)", Value = "111" },
            new SelectListItem { Text = "Personal hearing aids", Value = "112" },
            new SelectListItem { Text = "Primary radar", Value = "113" },
            new SelectListItem { Text = "Private fixed networks", Value = "114" },
            new SelectListItem { Text = "Public fixed networks", Value = "115" },
            new SelectListItem { Text = "RLAN", Value = "116" },
            new SelectListItem { Text = "RTE", Value = "117" },
            new SelectListItem { Text = "Radio microphones", Value = "118" },
            new SelectListItem { Text = "Road tolling", Value = "119" },
            new SelectListItem { Text = "S-PCS", Value = "120" },
            new SelectListItem { Text = "SAR (communications)", Value = "121" },
            new SelectListItem { Text = "SAR (navigation)", Value = "122" },
            new SelectListItem { Text = "SIT/SUT", Value = "123" },
            new SelectListItem { Text = "SNG", Value = "124" },
            new SelectListItem { Text = "SRR", Value = "125" },
            new SelectListItem { Text = "SSR", Value = "126" },
            new SelectListItem { Text = "Satellite TV", Value = "127" },
            new SelectListItem { Text = "Satellite communications (military)", Value = "128" },
            new SelectListItem { Text = "Satellite radio", Value = "129" },
            new SelectListItem { Text = "Scanning telemetry", Value = "130" },
            new SelectListItem { Text = "Social alarms", Value = "131" },
            new SelectListItem { Text = "Sonobuoy", Value = "132" },
            new SelectListItem { Text = "Synthetic aperture radar", Value = "133" },
            new SelectListItem { Text = "T-DAB", Value = "134" },
            new SelectListItem { Text = "T-DAB+", Value = "135" },
            new SelectListItem { Text = "TACAN-DME", Value = "136" },
            new SelectListItem { Text = "TETRA", Value = "137" },
            new SelectListItem { Text = "TETRAPOL", Value = "138" },
            new SelectListItem { Text = "TLPR", Value = "139" },
            new SelectListItem { Text = "TV analogue (terrestrial)", Value = "140" },
            new SelectListItem { Text = "Tactical mobile", Value = "141" },
            new SelectListItem { Text = "Tactical radar", Value = "142" },
            new SelectListItem { Text = "Tactical radio relay", Value = "143" },
            new SelectListItem { Text = "Talkback", Value = "144" },
            new SelectListItem { Text = "Talkback pocket unit", Value = "145" },
            new SelectListItem { Text = "Telecommand (military)", Value = "146" },
            new SelectListItem { Text = "Telemetry (civil)", Value = "147" },
            new SelectListItem { Text = "Telemetry (military)", Value = "148" },
            new SelectListItem { Text = "ULP-AID", Value = "149" },
            new SelectListItem { Text = "ULP-AMI", Value = "150" },
            new SelectListItem { Text = "ULP-MMI", Value = "151" },
            new SelectListItem { Text = "ULP-WMCE", Value = "152" },
            new SelectListItem { Text = "Unplanned, uncoordinated fixed links", Value = "153" },
            new SelectListItem { Text = "VOR", Value = "154" },
            new SelectListItem { Text = "VSAT", Value = "155" },
            new SelectListItem { Text = "Vehicle and infrastructure radar", Value = "156" },
            new SelectListItem { Text = "Video links", Value = "157" },
            new SelectListItem { Text = "WAIC", Value = "158" },
            new SelectListItem { Text = "WIA", Value = "159" },
            new SelectListItem { Text = "Weather satellites", Value = "160" },
            new SelectListItem { Text = "Wide area paging", Value = "161" }
        };

        public string ApplicationAllValue { get; set; }

        public List<SelectListItem> ApplicationAllList = new List<SelectListItem>
        {
            new SelectListItem { Text = "<all application terms>", Value = "1",Selected = true },
            new SelectListItem { Text = "-", Value = "2" },
            new SelectListItem { Text = "Active medical implants", Value = "3" },
            new SelectListItem { Text = "ADS", Value = "4" },
            new SelectListItem { Text = "Aeronautical", Value = "5" },
            new SelectListItem { Text = "Aeronautical communications", Value = "6" },
            new SelectListItem { Text = "Aeronautical emergency", Value = "7" },
            new SelectListItem { Text = "Aeronautical military systems", Value = "8" },
            new SelectListItem { Text = "Aeronautical navigation", Value = "9" },
            new SelectListItem { Text = "Aeronautical radar", Value = "10" },
            new SelectListItem { Text = "Aeronautical satcoms", Value = "11" },
            new SelectListItem { Text = "Aeronautical surveillance", Value = "12" },
            new SelectListItem { Text = "Aeronautical telecommand", Value = "13" },
            new SelectListItem { Text = "Aeronautical telemetry", Value = "14" },
            new SelectListItem { Text = "Aeronautical telemetry/telecommand", Value = "15" },
            new SelectListItem { Text = "AES", Value = "16" },
            new SelectListItem { Text = "AGA communications (civil)", Value = "17" },
            new SelectListItem { Text = "AGA communications (military)", Value = "18" },
            new SelectListItem { Text = "Air-defence radar", Value = "19" },
            new SelectListItem { Text = "Airborne doppler navigation aids", Value = "20" },
            new SelectListItem { Text = "Airborne weather radar", Value = "21" },
            new SelectListItem { Text = "AIS", Value = "22" },
            new SelectListItem { Text = "Alarms", Value = "23" },
            new SelectListItem { Text = "ALD", Value = "24" },
            new SelectListItem { Text = "ALS", Value = "25" },
            new SelectListItem { Text = "Altimeters", Value = "26" },
            new SelectListItem { Text = "AM sound analogue", Value = "27" },
            new SelectListItem { Text = "Amatuer", Value = "28" },
            new SelectListItem { Text = "Amatuer-satellite", Value = "29" },
            new SelectListItem { Text = "AMRD Group A", Value = "30" },
            new SelectListItem { Text = "AMRD Group B", Value = "31" },
            new SelectListItem { Text = "Animal tracking", Value = "32" },
            new SelectListItem { Text = "ASDE", Value = "33" },
            new SelectListItem { Text = "Asset tracking and tracing", Value = "34" },
            new SelectListItem { Text = "Audio links", Value = "35" },
            new SelectListItem { Text = "Audio PMSE", Value = "36" },
            new SelectListItem { Text = "Automotive radar", Value = "37" },
            new SelectListItem { Text = "Baby monitoring", Value = "38" },
            new SelectListItem { Text = "BBDR", Value = "39" },
            new SelectListItem { Text = "Beacons (aeronautical)", Value = "40" },
            new SelectListItem { Text = "Beacons (maritime)", Value = "41" },
            new SelectListItem { Text = "BFWA", Value = "42" },
            new SelectListItem { Text = "BMA", Value = "43" },
            new SelectListItem { Text = "Broadcasting", Value = "44" },
            new SelectListItem { Text = "Broadcasting (satellite)", Value = "45" },
            new SelectListItem { Text = "Broadcasting (terrestrial)", Value = "46" },
            new SelectListItem { Text = "BWA", Value = "47" },
            new SelectListItem { Text = "CB radio", Value = "48" },
            new SelectListItem { Text = "CGC", Value = "49" },
            new SelectListItem { Text = "Communication applications", Value = "50" },
            new SelectListItem { Text = "Continuum measurements", Value = "51" },
            new SelectListItem { Text = "Cordless cameras", Value = "52" },
            new SelectListItem { Text = "Cordless communicatons for voice and data", Value = "53" },
            new SelectListItem { Text = "Cordless headphones and loudspeakers", Value = "54" },
            new SelectListItem { Text = "D-GPS", Value = "55" },
            new SelectListItem { Text = "DA2GC", Value = "56" },
            new SelectListItem { Text = "DECT", Value = "57" },
            new SelectListItem { Text = "Deep space (satellite)", Value = "58" },
            new SelectListItem { Text = "Defence systems", Value = "59" },
            new SelectListItem { Text = "Detection of movement and alert", Value = "60" },
            new SelectListItem { Text = "Digital celluar", Value = "61" },
            new SelectListItem { Text = "DME", Value = "62" },
            new SelectListItem { Text = "DRM", Value = "63" },
            new SelectListItem { Text = "DSB/SSB AM CB / CEPT PR 27", Value = "64" },
            new SelectListItem { Text = "DSC", Value = "65" },
            new SelectListItem { Text = "DVB-T", Value = "66" },
            new SelectListItem { Text = "DVB-T2", Value = "67" },
            new SelectListItem { Text = "Earth exploration-satellite", Value = "68" },
            new SelectListItem { Text = "Earth exploration-satellite (military)", Value = "69" },
            new SelectListItem { Text = "ELT", Value = "70" },
            new SelectListItem { Text = "Emergency detection", Value = "71" },
            new SelectListItem { Text = "EPIRBs", Value = "72" },
            new SelectListItem { Text = "ESIM", Value = "73" },
            new SelectListItem { Text = "ESV", Value = "74" },
            new SelectListItem { Text = "Eurobalise", Value = "75" },
            new SelectListItem { Text = "Euroloop", Value = "76" },
            new SelectListItem { Text = "Feeder links", Value = "77" },
            new SelectListItem { Text = "Fixed", Value = "78" },
            new SelectListItem { Text = "Fixed radio relay (military)", Value = "79" },
            new SelectListItem { Text = "Flying model control", Value = "80" },
            new SelectListItem { Text = "FM sound analogue", Value = "81" },
            new SelectListItem { Text = "FRMCS", Value = "82" },
            new SelectListItem { Text = "FSS Earth stations", Value = "83" },
            new SelectListItem { Text = "FWA", Value = "84" },
            new SelectListItem { Text = "GALILEO", Value = "85" },
            new SelectListItem { Text = "GBAS", Value = "86" },
            new SelectListItem { Text = "GBSAR", Value = "87" },
            new SelectListItem { Text = "GLONASS", Value = "88" },
            new SelectListItem { Text = "GMDSS", Value = "89" },
            new SelectListItem { Text = "GNSS Pseudolites", Value = "90" },
            new SelectListItem { Text = "GNSS Repeater", Value = "91" },
            new SelectListItem { Text = "GPR/WPR", Value = "92" },
            new SelectListItem { Text = "GPS", Value = "93" },
            new SelectListItem { Text = "GSM", Value = "94" },
            new SelectListItem { Text = "GSM-R", Value = "95" },
            new SelectListItem { Text = "GSO ESOMPs", Value = "96" },
            new SelectListItem { Text = "HAPS", Value = "97" },
            new SelectListItem { Text = "HEST", Value = "98" },
            new SelectListItem { Text = "IFF", Value = "99" },
            new SelectListItem { Text = "ILS", Value = "100" },
            new SelectListItem { Text = "IMT", Value = "101" },
            new SelectListItem { Text = "IMT-2000 satellite component", Value = "102" },
            new SelectListItem { Text = "In-ear monitors", Value = "103" },
            new SelectListItem { Text = "Inductive applications", Value = "104" },
            new SelectListItem { Text = "Inland waterway communications", Value = "105" },
            new SelectListItem { Text = "Inland waterway radar", Value = "106" },
            new SelectListItem { Text = "INMARSAT", Value = "107" },
            new SelectListItem { Text = "INMARSAT C", Value = "108" },
            new SelectListItem { Text = "Inter-satellite links", Value = "109" },
            new SelectListItem { Text = "ISM", Value = "110" },
            new SelectListItem { Text = "ITS", Value = "111" },
            new SelectListItem { Text = "JTIDS/MIDS", Value = "112" },
            new SelectListItem { Text = "LAES", Value = "113" },
            new SelectListItem { Text = "Land millitary systems", Value = "114" },
            new SelectListItem { Text = "Land mobile", Value = "115" },
            new SelectListItem { Text = "Land radionavigation", Value = "116" },
            new SelectListItem { Text = "LEST", Value = "117" },
            new SelectListItem { Text = "Lightning detection systems", Value = "118" },
            new SelectListItem { Text = "Loran C", Value = "119" },
            new SelectListItem { Text = "LP FM Transmitter", Value = "120" },
            new SelectListItem { Text = "LP-AMI", Value = "121" },
            new SelectListItem { Text = "LPR", Value = "122" },
            new SelectListItem { Text = "LT2", Value = "123" },
            new SelectListItem { Text = "Maritime", Value = "124" },
            new SelectListItem { Text = "Maritime communications", Value = "125" },
            new SelectListItem { Text = "Maritime millitary systems", Value = "126" },
            new SelectListItem { Text = "Maritime navigation", Value = "127" },
            new SelectListItem { Text = "Maritime radar", Value = "128" },
            new SelectListItem { Text = "Material Sensing", Value = "129" },
            new SelectListItem { Text = "MBANS", Value = "130" },
            new SelectListItem { Text = "MBR", Value = "131" },
            new SelectListItem { Text = "MCA", Value = "132" },
            new SelectListItem { Text = "MCV", Value = "133" },
            new SelectListItem { Text = "Medical Data Acquisition", Value = "134" },
            new SelectListItem { Text = "Medical implants", Value = "135" },
            new SelectListItem { Text = "Medical telemetry", Value = "136" },
            new SelectListItem { Text = "Meteor scatter communications", Value = "137" },
            new SelectListItem { Text = "Meteorological aids (military)", Value = "138" },
            new SelectListItem { Text = "Meteorological satcoms", Value = "139" },
            new SelectListItem { Text = "Meteorology", Value = "140" },
            new SelectListItem { Text = "Meter reading", Value = "141" },
            new SelectListItem { Text = "MFCN", Value = "142" },
            new SelectListItem { Text = "MLS", Value = "143" },
            new SelectListItem { Text = "Model control", Value = "144" },
            new SelectListItem { Text = "MSI", Value = "145" },
            new SelectListItem { Text = "MSS Earth stations", Value = "146" },
            new SelectListItem { Text = "MWS", Value = "147" },
            new SelectListItem { Text = "Narrow band anologue voice devices", Value = "148" },
            new SelectListItem { Text = "NAVTEX", Value = "149" },
            new SelectListItem { Text = "NGSO ESOMPs", Value = "150" },
            new SelectListItem { Text = "NGSO FSS", Value = "151" },
            new SelectListItem { Text = "NMR", Value = "152" },
            new SelectListItem { Text = "Non-beam WPT", Value = "153" },
            new SelectListItem { Text = "Non-specific SRDs", Value = "154" },
            new SelectListItem { Text = "NP2M", Value = "155" },
            new SelectListItem { Text = "Obstacle detection radar", Value = "156" },
            new SelectListItem { Text = "Oceanographic buoys", Value = "157" },
            new SelectListItem { Text = "On-board communications", Value = "158" },
            new SelectListItem { Text = "On-site paging", Value = "159" },
            new SelectListItem { Text = "Other", Value = "160" },
            new SelectListItem { Text = "Paging", Value = "161" },
            new SelectListItem { Text = "PAMR", Value = "162" },
            new SelectListItem { Text = "Passive sensors (satellite)", Value = "163" },
            new SelectListItem { Text = "Personal hearing aids", Value = "164" },
            new SelectListItem { Text = "PLB", Value = "165" },
            new SelectListItem { Text = "PMR", Value = "166" },
            new SelectListItem { Text = "PMR/PAMR", Value = "167" },
            new SelectListItem { Text = "PMSE", Value = "168" },
            new SelectListItem { Text = "POCSAG", Value = "169" },
            new SelectListItem { Text = "Point-to-Multipoint", Value = "170" },
            new SelectListItem { Text = "Point-to-point", Value = "171" },
            new SelectListItem { Text = "PPDR", Value = "172" },
            new SelectListItem { Text = "Primary radar", Value = "173" },
            new SelectListItem { Text = "Private fixed networks", Value = "174" },
            new SelectListItem { Text = "Public fixed networks", Value = "175" },
            new SelectListItem { Text = "Radio astronomy", Value = "176" },
            new SelectListItem { Text = "Radio microphones", Value = "177" },
            new SelectListItem { Text = "Radio microphones and ALD", Value = "178" },
            new SelectListItem { Text = "Radiodetermination applications", Value = "179" },
            new SelectListItem { Text = "Radiolocation (civil)", Value = "180" },
            new SelectListItem { Text = "Radiolocation (military)", Value = "181" },
            new SelectListItem { Text = "Railway applications", Value = "182" },
            new SelectListItem { Text = "RFID", Value = "183" },
            new SelectListItem { Text = "RLAN", Value = "184" },
            new SelectListItem { Text = "RMR", Value = "185" },
            new SelectListItem { Text = "Road tolling", Value = "186" },
            new SelectListItem { Text = "RTE", Value = "187" },
            new SelectListItem { Text = "S-PCS", Value = "188" },
            new SelectListItem { Text = "SAR (communications)", Value = "189" },
            new SelectListItem { Text = "SAR navigation", Value = "190" },
            new SelectListItem { Text = "Satellite communications (military)", Value = "191" },
            new SelectListItem { Text = "Satellite navigation systems", Value = "192" },
            new SelectListItem { Text = "Satellite radio", Value = "193" },
            new SelectListItem { Text = "Satellite systems (civil)", Value = "194" },
            new SelectListItem { Text = "Satellite systems (military)", Value = "195" },
            new SelectListItem { Text = "Satellite TV", Value = "196" },
            new SelectListItem { Text = "Scanning telemetry", Value = "197" },
            new SelectListItem { Text = "Service links", Value = "198" },
            new SelectListItem { Text = "Short Range Devices", Value = "199" },
            new SelectListItem { Text = "SIT/SUT", Value = "200" },
            new SelectListItem { Text = "SNG", Value = "201" },
            new SelectListItem { Text = "Social alarms", Value = "202" },
            new SelectListItem { Text = "Sondes", Value = "203" },
            new SelectListItem { Text = "Sonobuoy", Value = "204" },
            new SelectListItem { Text = "Space operations", Value = "205" },
            new SelectListItem { Text = "Space research", Value = "206" },
            new SelectListItem { Text = "Spectral line observations", Value = "207" },
            new SelectListItem { Text = "SRR", Value = "208" },
            new SelectListItem { Text = "SSR", Value = "209" },
            new SelectListItem { Text = "Standard frequency and time signal", Value = "210" },
            new SelectListItem { Text = "Standard frequency and time signal-satellite", Value = "211" },
            new SelectListItem { Text = "Synthetic aperature radar", Value = "212" },
            new SelectListItem { Text = "T-DAB", Value = "213" },
            new SelectListItem { Text = "T-DAB+", Value = "214" },
            new SelectListItem { Text = "TACAN-DME", Value = "215" },
            new SelectListItem { Text = "Tactical mobile", Value = "216" },
            new SelectListItem { Text = "Tactical radar", Value = "217" },
            new SelectListItem { Text = "Tactical radio relay", Value = "218" },
            new SelectListItem { Text = "Talkback", Value = "219" },
            new SelectListItem { Text = "Talkback pocket unit", Value = "220" },
            new SelectListItem { Text = "Telecommand (military)", Value = "221" },
            new SelectListItem { Text = "Telemetry (civil)", Value = "222" },
            new SelectListItem { Text = "Telemetry (military)", Value = "223" },
            new SelectListItem { Text = "Telemetry/Telecommand (civil)", Value = "224" },
            new SelectListItem { Text = "Telemetry/Telecommand (military)", Value = "225" },
            new SelectListItem { Text = "TETRA", Value = "226" },
            new SelectListItem { Text = "TETRAPOL", Value = "227" },
            new SelectListItem { Text = "TLPR", Value = "228" },
            new SelectListItem { Text = "TRA-ECS", Value = "229" },
            new SelectListItem { Text = "Tracking systems", Value = "230" },
            new SelectListItem { Text = "Tracking, tracing and data acquisition", Value = "231" },
            new SelectListItem { Text = "TTT", Value = "232" },
            new SelectListItem { Text = "TV analogue (terrestrial)", Value = "233" },
            new SelectListItem { Text = "UAS", Value = "234" },
            new SelectListItem { Text = "ULP-AID", Value = "235" },
            new SelectListItem { Text = "ULP-AMI", Value = "236" },
            new SelectListItem { Text = "ULP-MMI", Value = "237" },
            new SelectListItem { Text = "ULP-WMCE", Value = "238" },
            new SelectListItem { Text = "Unplanned, uncoordinated fixed links", Value = "239" },
            new SelectListItem { Text = "UWB applications", Value = "240" },
            new SelectListItem { Text = "Vehicle and infrastructure radar", Value = "241" },
            new SelectListItem { Text = "Video links", Value = "242" },
            new SelectListItem { Text = "Video PMSE", Value = "243" },
            new SelectListItem { Text = "VLBI observations", Value = "244" },
            new SelectListItem { Text = "VOR", Value = "245" },
            new SelectListItem { Text = "VSAT", Value = "246" },
            new SelectListItem { Text = "WIA", Value = "247" },
            new SelectListItem { Text = "Wide are paging", Value = "248" },
            new SelectListItem { Text = "Wideband data transmission systems", Value = "249" },
            new SelectListItem { Text = "Wind profilers", Value = "250" },
            new SelectListItem { Text = "Wireless audio/multimedia", Value = "251" },
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

        IConfiguration configuration;

        public IActionResult Index()
        {
            //execute this condition when user search application from home or start page.
            if (TempData["app"] != null)
            {
                bool value = (bool)TempData["app"];
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
                    if (this.FrequencytableValue == "0")
                    {
                        return View();
                    }
                    //string tempValueFreq = FrequencyTablesList.Where(p => p.Value.Equals(this.FrequencytableValue)).First().Text;


                    FrequencyTablesList.FirstOrDefault(p => p.Value.Equals(this.FrequencytableValue)).Selected = true;

                    FrequencySizesList.FirstOrDefault(p => p.Value.Equals(this.FrequencySizeValue)).Selected = true;
                    //Debug.WriteLine("www inside:"+valueTo+"=="+ this.FrequencytableValue);
                    if (valueFrom == 0 && valueTo == 0)
                    {

                        ApplicationSearchActions asActions = new ApplicationSearchActions();
                        // var listGeneral = asActions.SearchAppAll(_conApp,  "" + this.FrequencytableValue);
                        //var listGeneral = asActions.SearchAppAllNew(_conApp, "" + this.FrequencytableValue); 
                        var listGeneral = asActions.SearchAppAllNewPerfomance(_conApp, "" + this.FrequencytableValue);
                        listGeneral = listGeneral.OrderBy(e => e.low).ToList();



                        listGeneral[0].FrequencyTablesList = FrequencyTablesList;
                        listGeneral[0].FrequencySizeValue = FrequencySizeValue;
                        listGeneral[0].FrequencySizesList = FrequencySizesList;

                        //Debug.WriteLine("count:" + FrequencySizesList.Count + ":size:" + FrequencySizeValue+"");
                        return View(listGeneral);

                    }
                    else if (valueFrom == 0 && valueTo != 0)
                    {
                        ApplicationSearchActions asActions = new ApplicationSearchActions();
                        //var listGeneral = asActions.SearchAppAllZeroTo(_conApp, valueFrom, valueTo, "" + this.FrequencytableValue);
                        //var listGeneral = asActions.SearchAppAllZeroToNew(_conApp, valueFrom, valueTo, "" + this.FrequencytableValue);
                        var listGeneral = asActions.SearchAppAllZeroToNewPerfomance(_conApp, valueFrom, valueTo, "" + this.FrequencytableValue);
                        listGeneral = listGeneral.OrderBy(e => e.low).ToList();
                        listGeneral[0].FrequencyTablesList = FrequencyTablesList;
                        listGeneral[0].FrequencySizeValue = FrequencySizeValue;
                        listGeneral[0].FrequencySizesList = FrequencySizesList;
                        return View(listGeneral);
                    }
                    else if (valueFrom != 0 && valueTo != 0)
                    {
                        ApplicationSearchActions asActions = new ApplicationSearchActions();
                        //var listGeneral = asActions.SearchAppAllFromTo(_conApp, valueFrom, valueTo, "" + this.FrequencytableValue);
                        //var listGeneral = asActions.SearchAppAllFromToNew(_conApp, valueFrom, valueTo, "" + this.FrequencytableValue);
                        var listGeneral = asActions.SearchAppAllFromToNewPerfomance(_conApp, valueFrom, valueTo, "" + this.FrequencytableValue);
                        listGeneral = listGeneral.OrderBy(e => e.low).ToList();
                        listGeneral[0].FrequencyTablesList = FrequencyTablesList;
                        listGeneral[0].FrequencySizeValue = FrequencySizeValue;
                        listGeneral[0].FrequencySizesList = FrequencySizesList;
                        return View(listGeneral);
                    }
                }
            }
            return View();
        }

        [HttpPost]
        //metod for searching application using jquery or javasript.
        public JsonResult GeneralSearch(string FromVal, string ToVal, int FrequencySizeVal, string FrequencyTableVal)
        {
            From = FromVal;
            To = ToVal;
            FrequencySizeValue = "" + FrequencySizeVal;
            FrequencytableValue = "" + FrequencyTableVal;
            //Debug.WriteLine("shou" + FrequencyTableVal);
            var tempFreq = FrequencyTablesList.Where(p => p.Text.Equals(FrequencyTableVal)).First().Value;
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
            if (tempFrom == 0 && tempTo == 0)
            {
                ApplicationSearchActions asa = new ApplicationSearchActions();
                //asa.SearchAppAll(_conApp, "" + FrequencyTableVal);
                //old code for searching all apps
                //var listGeneral = asa.SearchAppAll(_conApp, "" + tempFreq);
                //var listGeneral = asa.SearchAppAllNew(_conApp, "" + tempFreq);
                var listGeneral = asa.SearchAppAllNewPerfomance(_conApp, "" + tempFreq);

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
                listGeneral = listGeneral.OrderBy(e => e.low).ToList();
                listGeneral[0].isAllValues = true;
                listGeneral[0].lowJson = 0;
                listGeneral[0].highJson = result;
                listGeneral[0].sizeFrequency = sizeFrequency;
                //Debug.WriteLine("tu sam:" + listGeneral.Count);
                return Json(listGeneral, new System.Text.Json.JsonSerializerOptions());
            }
            else if (tempFrom == 0 && tempTo != 0)
            {
                ApplicationSearchActions asa = new ApplicationSearchActions();
                //old code for searching app from zero to high
                //var listGeneral = asa.SearchAppAllZeroTo(_conApp,tempFrom,tempTo, "" + tempFreq);
                //var listGeneral = asa.SearchAppAllZeroToNew(_conApp, tempFrom, tempTo, "" + tempFreq);
                var listGeneral = asa.SearchAppAllZeroToNewPerfomance(_conApp, tempFrom, tempTo, "" + tempFreq);
                return Json(listGeneral.OrderBy(e => e.low).ToList(), new System.Text.Json.JsonSerializerOptions());
            }
            else if (tempFrom != 0 && tempTo != 0)
            {
                ApplicationSearchActions asa = new ApplicationSearchActions();
                //old code for searching
                //var listGeneral = asa.SearchAppAllFromTo(_conApp, tempFrom, tempTo, "" + tempFreq);
                //var listGeneral = asa.SearchAppAllFromToNew(_conApp, tempFrom, tempTo, "" + tempFreq);
                var listGeneral = asa.SearchAppAllFromToNewPerfomance(_conApp, tempFrom, tempTo, "" + tempFreq);
                return Json(listGeneral.OrderBy(e => e.low).ToList(), new System.Text.Json.JsonSerializerOptions());
            }

            return null;

        }

        //using the jquery to search on second level of application.
        [HttpPost]
        public JsonResult SearchOnSecondLevel(string FromVal, string ToVal, int FrequencySizeVal, string FrequencyTableVal, string ApplicatonVal)
        {
            From = FromVal;
            To = ToVal;
            FrequencySizeValue = "" + FrequencySizeVal;
            FrequencytableValue = "" + FrequencyTableVal;

            var tempFreq = FrequencyTablesList.Where(p => p.Text.Equals(FrequencyTableVal)).First().Value;
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
            //Debug.WriteLine("shou=" + tempFrom+":"+tempTo);
            string tempApplication = ApplicationSecondList.Where(p => p.Value.Equals(ApplicatonVal)).First().Text;
            ApplicationSearchActions asa = new ApplicationSearchActions();
            //Debug.WriteLine("test:" + ApplicatonVal);
            //var listGeneral =  asa.SearchAppOnSecondLevel(_conApp, tempFrom, tempTo, tempFreq, tempApplication);
            if (tempFrom == 0 && tempTo == 0)
            {

                //var listGeneral = asa.SearchAppOnSecondLevelProcedure(configuration, tempApplication,_conImport,_conApp);
                //var listGeneral = asa.SearchAppOnSecondLevelProcedureNew(configuration, tempApplication, _conImport, _conApp);
                var listGeneral = asa.SearchAppOnSecondLevelProcedureNewPerfomance(configuration, tempApplication, _conImport, _conApp);

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


                listGeneral = listGeneral.OrderBy(e => e.low).ToList();
                listGeneral[0].isAllValues = true;
                listGeneral[0].lowJson = 0;
                listGeneral[0].highJson = result;
                listGeneral[0].sizeFrequency = sizeFrequency;
                return Json(listGeneral, new System.Text.Json.JsonSerializerOptions());
            }
            else if (tempFrom == 0 && tempTo != 0)
            {
                //var listGeneral = asa.SearchAppOnSecondLevelProcedureFromZero(tempFrom, tempTo, configuration, tempApplication, _conImport, _conApp);
                //var listGeneral = asa.SearchAppOnSecondLevelProcedureFromZeroNew(tempFrom, tempTo, configuration, tempApplication, _conImport, _conApp);
                var listGeneral = asa.SearchAppOnSecondLevelProcedureFromZeroNewPerfomance(tempFrom, tempTo, configuration, tempApplication, _conImport, _conApp);
                return Json(listGeneral.OrderBy(e => e.low).ToList(), new System.Text.Json.JsonSerializerOptions());
            }
            else if (tempFrom != 0 && tempTo != 0)
            {

                //var listGeneral = asa.SearchAppOnSecondLevelProcedureFromLowHigh(tempFrom, tempTo, configuration, tempApplication, _conImport, _conApp);
                //var listGeneral = asa.SearchAppOnSecondLevelProcedureFromLowHighNew(tempFrom, tempTo, configuration, tempApplication, _conImport, _conApp);
                var listGeneral = asa.SearchAppOnSecondLevelProcedureFromLowHighNewPerfomance(tempFrom, tempTo, configuration, tempApplication, _conImport, _conApp);
                return Json(listGeneral.OrderBy(e => e.low).ToList(), new System.Text.Json.JsonSerializerOptions());

            }

            return null;
        }

        [HttpPost]
        public JsonResult SearchDocsAndStandards(string docId, string StandId, bool isThereDocs, bool isThereStands)
        {
            //Debug.WriteLine("test:" + docId+"=="+ StandId);

            List<AppDocStandView> appDocsStandList = new List<AppDocStandView>();
            if (isThereDocs)
            {
                if (docId != "")
                {
                    if (docId.Contains(","))
                    {
                        var Docs = docId.Split(",");
                        foreach (var doc in Docs)
                        {

                            var appQuery = (from all in _conApp.DocumentsDb
                                            where all.DocumentsId == int.Parse(doc)
                                            select new
                                            {
                                                CombineTitle = all.Doc_number + " " + all.Title_of_doc,
                                                Hyperlink = all.Hyperlink,
                                                TypeOfDoc = all.Type_of_doc
                                            }).SingleOrDefault();
                            if (appQuery.TypeOfDoc == "R")
                            {
                                AppDocStandView temp = new AppDocStandView();
                                temp.CombineTitle = appQuery.CombineTitle;
                                temp.Link = appQuery.Hyperlink;
                                appDocsStandList.Add(temp);
                            }
                        }
                    }
                    else
                    {
                        var appQuery = (from all in _conApp.DocumentsDb
                                        where all.DocumentsId == int.Parse(docId)
                                        select new
                                        {
                                            CombineTitle = all.Doc_number + " " + all.Title_of_doc,
                                            Hyperlink = all.Hyperlink,
                                            TypeOfDoc = all.Type_of_doc
                                        }).SingleOrDefault();
                        if (appQuery.TypeOfDoc == "R")
                        {
                            AppDocStandView temp = new AppDocStandView();
                            temp.CombineTitle = appQuery.CombineTitle;
                            temp.Link = appQuery.Hyperlink;
                            appDocsStandList.Add(temp);
                        }
                    }
                }
            }
            if (isThereStands)
            {
                if (StandId != "")
                {
                    if (StandId.Contains(","))
                    {
                        var Stands = StandId.Split(",");
                        foreach (var doc in Stands)
                        {


                            var appQuery = (from all in _conApp.StandardsDb
                                            where all.Standard_id == int.Parse(doc)
                                            select new
                                            {
                                                CombineTitle = all.Etsi_standard + " " + all.Title_doc,
                                                Hyperlink = all.Hypelink,
                                                TypeOfDocs = all.Type_of_Document
                                            }).SingleOrDefault();
                            if (appQuery.TypeOfDocs == "R")
                            {
                                AppDocStandView temp = new AppDocStandView();
                                temp.CombineTitle = appQuery.CombineTitle;
                                temp.Link = appQuery.Hyperlink;
                                appDocsStandList.Add(temp);
                            }
                        }
                    }
                    else
                    {
                        var appQuery = (from all in _conApp.StandardsDb
                                        where all.Standard_id == int.Parse(StandId)
                                        select new
                                        {
                                            CombineTitle = all.Etsi_standard + " " + all.Title_doc,
                                            Hyperlink = all.Hypelink,
                                            TypeOfDocs = all.Type_of_Document
                                        }).SingleOrDefault();
                        if (appQuery.TypeOfDocs == "R")
                        {
                            AppDocStandView temp = new AppDocStandView();
                            temp.CombineTitle = appQuery.CombineTitle;
                            temp.Link = appQuery.Hyperlink;
                            appDocsStandList.Add(temp);
                        }
                    }
                }

            }
            //Debug.WriteLine("test:"+appDocsStandList.Count);
            return Json(appDocsStandList.ToList(), new System.Text.Json.JsonSerializerOptions());
        }

        [HttpPost]
        public JsonResult SearchDocById(string docId)
        {
            var appQuery = (from all in _conApp.DocumentsDb
                            where all.DocumentsId == int.Parse(docId)
                            select new
                            {
                                TypeOfDoc = all.Type_of_doc
                            }).SingleOrDefault();
            //Debug.WriteLine("im here:" + appQuery.TypeOfDoc);
            if (appQuery.TypeOfDoc == "R")
            {

                return Json(appQuery.TypeOfDoc);
            }
            else if (appQuery.TypeOfDoc == "I")
            {
                return Json(appQuery.TypeOfDoc);
            }
            else
            {
                return null;
            }

        }

        [HttpPost]
        public JsonResult SearchStandById(string StandId)
        {
            var appQuery = (from all in _conApp.StandardsDb
                            where all.Standard_id == int.Parse(StandId)
                            select new
                            {

                                TypeOfDoc = all.Type_of_Document
                            }).SingleOrDefault();
            //Debug.WriteLine("im here:" + appQuery.TypeOfDoc);
            if (appQuery.TypeOfDoc == "R")
            {

                return Json(appQuery.TypeOfDoc);
            }
            else if (appQuery.TypeOfDoc == "I")
            {
                return Json(appQuery.TypeOfDoc);
            }
            else
            {
                return null;
            }

        }


        private byte[] GenerateExcel(List<AsiaPacific> allocs)
        {
            return null;
        }

        private byte[] GenerateExcel150(List<AsiaPacific> allocs)
        {
            return null;
        }

        private List<AsiaPacific> getPrintData(string AllVal1, string AllVal2, string AllVal3, string AllVal4, string FromVal, string ToVal, int FrequencySizeVal, string FreqTable, string[] FreqTableVal, string ApplicationVal)
        {
            JsonResult data = null;
            List<AsiaPacific> allocs = null;
            if (AllVal1 != "1")
            {
                data = SearchOnFirstLevel( FromVal, ToVal, FrequencySizeVal, FreqTable, ApplicationVal);
                string jsonResult = JsonConvert.SerializeObject(data.Value);
                allocs = JsonConvert.DeserializeObject<List<AsiaPacific>>(jsonResult);
            }
            else if (AllVal2 != "1")
            {
                data = SearchOnSecondLevel(FromVal, ToVal, FrequencySizeVal, FreqTable, ApplicationVal);
                string jsonResult = JsonConvert.SerializeObject(data.Value);
                allocs = JsonConvert.DeserializeObject<List<AsiaPacific>>(jsonResult);
            }
            else if (AllVal3 != "1")
            {
                data = SearchOnThirdLevel(FromVal, ToVal, FrequencySizeVal, FreqTable, ApplicationVal);
                string jsonResult = JsonConvert.SerializeObject(data.Value);
                allocs = JsonConvert.DeserializeObject<List<AsiaPacific>>(jsonResult);
            }
            /*
            else if (AllVal4 != "<all allocation terms >")
            {
                data = SearchOnFourthLevel(AllVal4, FromVal, ToVal, FrequencySizeVal, FreqTable);
                string jsonResult = JsonConvert.SerializeObject(data.Value);
                allocs = JsonConvert.DeserializeObject<List<AsiaPacific>>(jsonResult);
            }
            else
            {
                data = SearchOnlyValues(FromVal, ToVal, FrequencySizeVal, FreqTable, FreqTableVal);
                string jsonResult = JsonConvert.SerializeObject(data.Value);
                allocs = JsonConvert.DeserializeObject<List<AsiaPacific>>(jsonResult);
            }*/

            return allocs;
        }

        [HttpPost]
        [Route("AllocationSearch/DownloadExcel")]
        public byte[] DownloadExcel(string AllVal1, string AllVal2, string AllVal3, string AllVal4, string FromVal, string ToVal, int FrequencySizeVal, string FreqTable, string[] FreqTableVal)
        {
            List<AsiaPacific> allocs = getPrintData(AllVal1, AllVal2, AllVal3, AllVal4, FromVal, ToVal, FrequencySizeVal, FreqTable, FreqTableVal, "");
            return GenerateExcel(allocs);
        }

        //[EnableQuery]
        [HttpPost]
        [Route("AllocationSearch/DownloadPDF")]
        public byte[] DownloadPDF(string AllVal1, string AllVal2, string AllVal3, string AllVal4, string FromVal, string ToVal, int FrequencySizeVal, string FreqTable, string[] FreqTableVal)
        {
            List<AsiaPacific> allocs = getPrintData(AllVal1, AllVal2, AllVal3, AllVal4, FromVal, ToVal, FrequencySizeVal, FreqTable, FreqTableVal, "");
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

        [HttpPost]
        public JsonResult SearchDocsAndStandardsInformative(string docId, string StandId, bool isThereDocs, bool isThereStands)
        {
            //Debug.WriteLine("test:" + docId+"=="+ StandId);

            List<AppDocStandView> appDocsStandList = new List<AppDocStandView>();
            if (isThereDocs)
            {
                if (docId != "")
                {
                    if (docId.Contains(","))
                    {
                        var Docs = docId.Split(",");
                        foreach (var doc in Docs)
                        {

                            var appQuery = (from all in _conApp.DocumentsDb
                                            where all.DocumentsId == int.Parse(doc)
                                            select new
                                            {
                                                CombineTitle = all.Doc_number + " " + all.Title_of_doc,
                                                Hyperlink = all.Hyperlink,
                                                TypeOfDoc = all.Type_of_doc
                                            }).SingleOrDefault();
                            if (appQuery.TypeOfDoc == "I")
                            {
                                AppDocStandView temp = new AppDocStandView();
                                temp.CombineTitle = appQuery.CombineTitle;
                                temp.Link = appQuery.Hyperlink;
                                appDocsStandList.Add(temp);
                            }
                        }
                    }
                    else
                    {
                        var appQuery = (from all in _conApp.DocumentsDb
                                        where all.DocumentsId == int.Parse(docId)
                                        select new
                                        {
                                            CombineTitle = all.Doc_number + " " + all.Title_of_doc,
                                            Hyperlink = all.Hyperlink,
                                            TypeOfDoc = all.Type_of_doc
                                        }).SingleOrDefault();
                        if (appQuery.TypeOfDoc == "I")
                        {
                            AppDocStandView temp = new AppDocStandView();
                            temp.CombineTitle = appQuery.CombineTitle;
                            temp.Link = appQuery.Hyperlink;
                            appDocsStandList.Add(temp);
                        }
                    }
                }
            }
            if (isThereStands)
            {
                if (StandId != "")
                {
                    if (StandId.Contains(","))
                    {
                        var Stands = StandId.Split(",");
                        foreach (var doc in Stands)
                        {


                            var appQuery = (from all in _conApp.StandardsDb
                                            where all.Standard_id == int.Parse(doc)
                                            select new
                                            {
                                                CombineTitle = all.Etsi_standard + " " + all.Title_doc,
                                                Hyperlink = all.Hypelink,
                                                TypeOfDocs = all.Type_of_Document
                                            }).SingleOrDefault();
                            if (appQuery.TypeOfDocs == "I")
                            {
                                AppDocStandView temp = new AppDocStandView();
                                temp.CombineTitle = appQuery.CombineTitle;
                                temp.Link = appQuery.Hyperlink;
                                appDocsStandList.Add(temp);
                            }
                        }
                    }
                    else
                    {
                        var appQuery = (from all in _conApp.StandardsDb
                                        where all.Standard_id == int.Parse(StandId)
                                        select new
                                        {
                                            CombineTitle = all.Etsi_standard + " " + all.Title_doc,
                                            Hyperlink = all.Hypelink,
                                            TypeOfDocs = all.Type_of_Document
                                        }).SingleOrDefault();
                        if (appQuery.TypeOfDocs == "I")
                        {
                            AppDocStandView temp = new AppDocStandView();
                            temp.CombineTitle = appQuery.CombineTitle;
                            temp.Link = appQuery.Hyperlink;
                            appDocsStandList.Add(temp);
                        }
                    }
                }

            }
            //Debug.WriteLine("test:"+appDocsStandList.Count);
            return Json(appDocsStandList.ToList(), new System.Text.Json.JsonSerializerOptions());
        }

        //using the jquery of search on first level.
        [HttpPost]
        public JsonResult SearchOnFirstLevel(string FromVal, string ToVal, int FrequencySizeVal, string FrequencyTableVal, string ApplicatonVal)
        {
            From = FromVal;
            To = ToVal;
            FrequencySizeValue = "" + FrequencySizeVal;
            FrequencytableValue = "" + FrequencyTableVal;

            var tempFreq = FrequencyTablesList.Where(p => p.Text.Equals(FrequencyTableVal)).First().Value;
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
            //Debug.WriteLine("shou=" + tempFrom+":"+tempTo);
            string tempApplication = ApplicationFirstList.Where(p => p.Value.Equals(ApplicatonVal)).First().Text;
            ApplicationSearchActions asa = new ApplicationSearchActions();
            //Debug.WriteLine("test:" + ApplicatonVal);
            //var listGeneral =  asa.SearchAppOnSecondLevel(_conApp, tempFrom, tempTo, tempFreq, tempApplication);
            if (tempFrom == 0 && tempTo == 0)
            {
                //var listGeneral = asa.SearchAppOnSecondLevelProcedure(configuration, tempApplication,_conImport, _conApp);
                //var listGeneral = asa.SearchAppOnSecondLevelProcedureNew(configuration, tempApplication, _conImport, _conApp);
                var listGeneral = asa.SearchAppOnSecondLevelProcedureNewPerfomance(configuration, tempApplication, _conImport, _conApp);
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


                listGeneral = listGeneral.OrderBy(e => e.low).ToList();
                listGeneral[0].isAllValues = true;
                listGeneral[0].lowJson = 0;
                listGeneral[0].highJson = result;
                listGeneral[0].sizeFrequency = sizeFrequency;
                return Json(listGeneral, new System.Text.Json.JsonSerializerOptions());
            }
            else if (tempFrom == 0 && tempTo != 0)
            {
                //var listGeneral = asa.SearchAppOnSecondLevelProcedureFromZero(tempFrom, tempTo, configuration, tempApplication, _conImport, _conApp);
                //var listGeneral = asa.SearchAppOnSecondLevelProcedureFromZeroNew(tempFrom, tempTo, configuration, tempApplication, _conImport, _conApp);
                var listGeneral = asa.SearchAppOnSecondLevelProcedureFromZeroNewPerfomance(tempFrom, tempTo, configuration, tempApplication, _conImport, _conApp);
                return Json(listGeneral.OrderBy(e => e.low).ToList(), new System.Text.Json.JsonSerializerOptions());
            }
            else if (tempFrom != 0 && tempTo != 0)
            {

                // var listGeneral = asa.SearchAppOnSecondLevelProcedureFromLowHigh(tempFrom, tempTo, configuration, tempApplication, _conImport, _conApp);
                //var listGeneral = asa.SearchAppOnSecondLevelProcedureFromLowHighNew(tempFrom, tempTo, configuration, tempApplication, _conImport, _conApp);
                var listGeneral = asa.SearchAppOnSecondLevelProcedureFromLowHighNewPerfomance(tempFrom, tempTo, configuration, tempApplication, _conImport, _conApp);
                return Json(listGeneral.OrderBy(e => e.low).ToList(), new System.Text.Json.JsonSerializerOptions());

            }

            return null;
        }

        //using the jquey of search on third level.
        [HttpPost]
        public JsonResult SearchOnThirdLevel(string FromVal, string ToVal, int FrequencySizeVal, string FrequencyTableVal, string ApplicatonVal)
        {
            From = FromVal;
            To = ToVal;
            FrequencySizeValue = "" + FrequencySizeVal;
            FrequencytableValue = "" + FrequencyTableVal;

            var tempFreq = FrequencyTablesList.Where(p => p.Text.Equals(FrequencyTableVal)).First().Value;
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
            //Debug.WriteLine("shou=" + tempFrom+":"+tempTo);
            string tempApplication = ApplicationThirdList.Where(p => p.Value.Equals(ApplicatonVal)).First().Text;
            ApplicationSearchActions asa = new ApplicationSearchActions();
            //Debug.WriteLine("test:" + ApplicatonVal);
            //var listGeneral =  asa.SearchAppOnSecondLevel(_conApp, tempFrom, tempTo, tempFreq, tempApplication);
            if (tempFrom == 0 && tempTo == 0)
            {
                //var listGeneral = asa.SearchAppOnSecondLevelProcedure(configuration, tempApplication,_conImport, _conApp);
                //var listGeneral = asa.SearchAppOnSecondLevelProcedureNew(configuration, tempApplication, _conImport, _conApp);
                var listGeneral = asa.SearchAppOnSecondLevelProcedureNewPerfomance(configuration, tempApplication, _conImport, _conApp);
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


                listGeneral = listGeneral.OrderBy(e => e.low).ToList();
                listGeneral[0].isAllValues = true;
                listGeneral[0].lowJson = 0;
                listGeneral[0].highJson = result;
                listGeneral[0].sizeFrequency = sizeFrequency;
                return Json(listGeneral, new System.Text.Json.JsonSerializerOptions());

            }
            else if (tempFrom == 0 && tempTo != 0)
            {
                //var listGeneral = asa.SearchAppOnSecondLevelProcedureFromZeroNew(tempFrom, tempTo, configuration, tempApplication, _conImport, _conApp);
                var listGeneral = asa.SearchAppOnSecondLevelProcedureFromZeroNewPerfomance(tempFrom, tempTo, configuration, tempApplication, _conImport, _conApp);
                return Json(listGeneral.OrderBy(e => e.low).ToList(), new System.Text.Json.JsonSerializerOptions());
            }
            else if (tempFrom != 0 && tempTo != 0)
            {

                //var listGeneral = asa.SearchAppOnSecondLevelProcedureFromLowHigh(tempFrom, tempTo, configuration, tempApplication, _conImport, _conApp);
                //var listGeneral = asa.SearchAppOnSecondLevelProcedureFromLowHighNew(tempFrom, tempTo, configuration, tempApplication, _conImport, _conApp);
                var listGeneral = asa.SearchAppOnSecondLevelProcedureFromLowHighNewPerfomance(tempFrom, tempTo, configuration, tempApplication, _conImport, _conApp);
                return Json(listGeneral.OrderBy(e => e.low).ToList(), new System.Text.Json.JsonSerializerOptions());

            }

            return null;
        }


      


        public async Task<IActionResult> GraphicalApplicationSearchGrid(string FromVal, string ToVal, int FrequencySizeVal, string FrequencyTableVal, string orientation)
        {
            var Generalsearch = GetApplicationData(FromVal, ToVal, FrequencySizeVal, FrequencyTableVal, orientation);
            if (Generalsearch.Count > 0)
                Generalsearch[0].orientation = orientation;
            var graphicalGridData = await _viewRenderService.RenderToStringAsync("_ApplicationGraphicalGrid", Generalsearch);
            return Json(graphicalGridData, new System.Text.Json.JsonSerializerOptions());
        }
        public async Task<IActionResult> GraphicalAllowCationSearchGrid(string FromVal, string ToVal, int FrequencySizeVal, string FrequencyTableVal, string orientation)
        {
            try
            {
                var Generalsearch = GetAllocationData(FromVal, ToVal, FrequencySizeVal, FrequencyTableVal, orientation);
                if (Generalsearch.Count > 0)
                    Generalsearch[0].orientation = orientation;
                var graphicalGridData = await _viewRenderService.RenderToStringAsync("_AllocationGraphicalGrid", Generalsearch);
                return Json(graphicalGridData, new System.Text.Json.JsonSerializerOptions());
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                List<string> errors = new List<string>();

                errors.Add(ex.Message);

                return Json(errors);
            }
        }
        public List<FreqBandSearchNew> GetAllocationData(string FromVal, string ToVal, int FrequencySizeVal, string FrequencyTableVal, string orientation)
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

                List<FreqBandSearchNew> Generalsearch = new List<FreqBandSearchNew>();
            try
            {
                var tempFreq = FrequencyTablesList.Where(p => p.Text.Equals(FrequencyTableVal)).First().Value;
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
                AllocationSearchActions asa = new AllocationSearchActions();
                var listGeneral = asa.SearchValues(_conAll, tempFrom, tempTo, tempFreq);


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
                listGeneral = listGeneral.OrderBy(e => e.low).ToList();
                if (listGeneral.Count > 0)
                {
                    listGeneral[0].isAllValues = true;
                    listGeneral[0].lowJson = 0;
                    listGeneral[0].highJson = result;
                    listGeneral[0].sizeFrequency = sizeFrequency;
                }
                Generalsearch = listGeneral.OrderBy(x => x.low).ToList();
            }
            catch (Exception)
            {
                throw;
            }
            return Generalsearch;
        }
        public List<ApplicationView> GetApplicationData(string FromVal, string ToVal, int FrequencySizeVal, string FrequencyTableVal, string orientation)
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


            List<ApplicationView> Generalsearch = new List<ApplicationView>();
            try
            {
                List<ApplicationView> listGeneral = new List<ApplicationView>();
                From = FromVal;
                To = ToVal;
                FrequencySizeValue = "" + FrequencySizeVal;
                FrequencytableValue = "" + FrequencyTableVal;
                var tempFreq = FrequencyTablesList.Where(p => p.Text.Equals(FrequencyTableVal)).First().Value;
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
                    ApplicationSearchActions asa = new ApplicationSearchActions();
                    listGeneral = asa.SearchAppAllNewPerfomance(_conApp, "" + tempFreq);
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
                    if (listGeneral.Count > 0)
                    {
                        listGeneral = listGeneral.OrderBy(e => e.low).ToList();
                        listGeneral[0].isAllValues = true;
                        listGeneral[0].lowJson = 0;
                        listGeneral[0].highJson = result;
                        listGeneral[0].sizeFrequency = sizeFrequency;
                    }

                }
                else if (tempFrom == 0 && tempTo != 0)
                {
                    ApplicationSearchActions asa = new ApplicationSearchActions();
                    //listGeneral = asa.SearchAppAllZeroToNewPerfomance(_conApp, tempFrom, tempTo, "" + tempFreq);
                    listGeneral = asa.SearchAppAllFromToNewPerfomance(_conApp, tempFrom, tempTo, "" + tempFreq);
                }
                else if (tempFrom != 0 && tempTo != 0)
                {
                    ApplicationSearchActions asa = new ApplicationSearchActions();
                    listGeneral = asa.SearchAppAllFromToNewPerfomance(_conApp, tempFrom, tempTo, "" + tempFreq);
                }

                Generalsearch = listGeneral.OrderBy(x => x.low).ToList();
            }
            catch (Exception)
            {
                throw;
            }
            return Generalsearch;
        }



        

        public ApplicationSearchController(
                                            IHttpContextAccessor httpContextAccessor,
                                            ApplicationDBContext conApp,
                                            IConfiguration configuration,
                                            ImportTempTableContext conImport,
                                            AllocationDBContext conAll,
                                            IViewRenderService viewRenderService
                                            )
        {

            user = httpContextAccessor.HttpContext.User;
            _conApp = conApp;
            this.configuration = configuration;
            _conImport = conImport;
            _viewRenderService = viewRenderService;
            _conAll = conAll;
        }

        public IActionResult GraphicalSearch()
        {
            ViewBag.GraphicalSearch = "Application";
            return View();
        }
    }
}

