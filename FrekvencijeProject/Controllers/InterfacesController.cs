using FrekvencijeProject.Models;
using FrekvencijeProject.Models.Interfaces;
using FrekvencijeProject.Models.Json;
using GemBox.Spreadsheet;
using iTextSharp.tool.xml.html.head;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.Controllers
{
    public class InterfacesController : Controller
    {
        object user;

        IConfiguration configuration;

        private readonly ApplicationDBContext _conApp;
        private readonly AllocationDBContext _conAll;
        
        private readonly ImportTempInterfacesDBContext _conInterfaces;

        [Display(Name = "Freq. Range")]
        public string Frequency { get; set; }

        [Display(Name = "From")]
        public string FromText { get; set; }
        public string From { get; set; }

        [Display(Name = "To")]
        public string ToText { get; set; }

        public string To { get; set; }

        [Display(Name = "Application")]
        public string ApplicationTable { get; set; }
        public string ApplicationFirstValue { get; set; }


        [Display(Name = "Frequency Table")]
        public string FrequencyTable { get; set; }

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
            new SelectListItem { Text = "Wireless audio/multimedia", Value = "251" }
        };

        public string AllocationFirstValue { get; set; }


        public List<SelectListItem> AllocationFirstList = new List<SelectListItem>();


        public string AllocationSecondValue { get; set; }


        public List<SelectListItem> AllocationSecondList = new List<SelectListItem>();


        public string AllocationThirdValue { get; set; }


        public List<SelectListItem> AllocationThirdList = new List<SelectListItem>();


        public string AllocationAllValue { get; set; }


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

        public List<SelectListItem> AllocationAllList = new List<SelectListItem>();

        public List<InterfacesView> InterfacesList = new List<InterfacesView>();
        

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


        [HttpPost]
        public JsonResult OrderByHierarchicalAll()
        {
            var values = _conAll.AllocationTermDb.Where(x => x.Number != null && x._PRIMARY == false).ToList();
            var newApplicationOrderBy = values.OrderBy(x => x.Number);
            return Json(newApplicationOrderBy.ToList(), new System.Text.Json.JsonSerializerOptions());
        }


        [HttpPost]
        public JsonResult OrderByAlphabeticalAll()
        {
            var values = _conAll.AllocationTermDb.Where(x => x.Number != null && x._PRIMARY == false).ToList();
            var newApplicationOrderBy = values.OrderBy(x => x.name);
            return Json(newApplicationOrderBy.ToList(), new System.Text.Json.JsonSerializerOptions());
        }

        [HttpPost]
        public JsonResult GeneralSearch(int FrequencySizeVal,string FrequencyTableVal)
        {
            var listOfInterfaces = _conInterfaces.ImportTempInterfaces.ToList();
            var tempFreq = FrequencyTablesList.Where(p => p.Text.Equals(FrequencyTableVal)).First().Value;
            FrequencyTablesList.FirstOrDefault(p => p.Value.Equals(tempFreq)).Selected = true;
            
            
            foreach (var tempInterfaces in listOfInterfaces)
            {
                //Debug.WriteLine("im here:" + tempInterfaces.Application+"=="+tempInterfaces.RadiocommunicationService);
                InterfacesView inter = new InterfacesView();
                inter.Country = tempInterfaces.Country;
                inter.RadiocommunicationService = tempInterfaces.RadiocommunicationService;
                inter.Application = tempInterfaces.Application;
                inter.FrequencyBand = tempInterfaces.LowerFrequency + "-" + tempInterfaces.UpperFrequency;
                inter.Channeling = tempInterfaces.Channeling;
                inter.OccupiedBandwidth = tempInterfaces.Modulation;
                inter.DirectionSeparation = tempInterfaces.DirectionSeparation;
                inter.TransmitPower = tempInterfaces.TransmitPower;
                inter.Notes = tempInterfaces.Remarks;
                InterfacesList.Add(inter);
            }
            
            FrequencySizesList.FirstOrDefault(p => p.Value.Equals(""+FrequencySizeVal)).Selected = true;
            InterfacesList[0].FrequencySizesList = FrequencySizesList;
            InterfacesList[0].FrequencyTablesList = FrequencyTablesList;
            InterfacesList[0].FrequencySizeValue = ""+FrequencySizeVal;

            return Json(InterfacesList, new System.Text.Json.JsonSerializerOptions());
        }

        public IActionResult Index()
        {
            if (TempData["inter"] != null)
            {
                bool value = (bool)TempData["inter"];
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
                    
                    var listOfInterfaces = _conInterfaces.ImportTempInterfaces.ToList();
                    
                    foreach(var tempInterfaces in listOfInterfaces)
                    {
                        
                        InterfacesView inter = new InterfacesView();
                        inter.Country = tempInterfaces.Country;
                        inter.RadiocommunicationService = tempInterfaces.RadiocommunicationService;
                        inter.Application = tempInterfaces.Application;
                        inter.FrequencyBand = tempInterfaces.LowerFrequency + "-" + tempInterfaces.UpperFrequency;
                        inter.Channeling = tempInterfaces.Channeling;
                        inter.OccupiedBandwidth = tempInterfaces.Modulation;
                        inter.DirectionSeparation = tempInterfaces.DirectionSeparation;
                        inter.TransmitPower = tempInterfaces.TransmitPower;
                        inter.Notes = tempInterfaces.Remarks;
                        InterfacesList.Add(inter);
                    }
                    InterfacesList[0].FrequencySizesList = FrequencySizesList;
                    InterfacesList[0].FrequencyTablesList = FrequencyTablesList;
                    InterfacesList[0].FrequencySizeValue = "" + FrequencySizeValue;
                }
            }
            return View(InterfacesList);
        }

        private byte[] GenerateExcel(List<InterfacesView> ints)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                // Set column widths
                worksheet.Column(1).Width = 12;  // Column A 
                worksheet.Column(2).Width = 25;  // Column B
                worksheet.Column(3).Width = 45;  // Column C
                worksheet.Column(4).Width = 20;  // Column D
                worksheet.Column(5).Width = 40;  // Column E
                worksheet.Column(6).Width = 20;  // Column F
                worksheet.Column(7).Width = 30;  // Column G
                worksheet.Column(8).Width = 35;  // Column H

                // Merge cells and write title
                worksheet.Cells["A1:H2"].Merge = true;
                worksheet.Cells["A1"].Value = "Interfaces";
                worksheet.Cells["A1:H2"].Style.Font.Color.SetColor(Color.FromArgb(0x92, 0xD0, 0x50));
                worksheet.Cells["A1"].Style.Font.Size = 16;
                worksheet.Cells["A1"].Style.Font.Name = "Calibri";
                worksheet.Cells["A1"].Style.Font.Bold = true;
                worksheet.Cells["A1"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center; // Align in the middle

                //Format the headers for the table
                OfficeOpenXml.Style.ExcelStyle headerStyle;
                worksheet.Cells["A3"].Value = "Country";
                worksheet.Cells["B3"].Value = "Frequency band";
                worksheet.Cells["C3"].Value = "Application";
                worksheet.Cells["D3"].Value = "Radiocommunication service";
                worksheet.Cells["E3"].Value = "Channeling";
                worksheet.Cells["F3"].Value = "Occupied bandwidth";
                worksheet.Cells["G3"].Value = "Direction/Separation";
                worksheet.Cells["H3"].Value = "Transmit power";
                headerStyle = worksheet.Cells["A3:H3"].Style;

                headerStyle.Font.Bold = true;
                headerStyle.Font.Italic = true;
                headerStyle.Font.Name = "Calibri";
                headerStyle.Font.Size = 14;
                headerStyle.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                headerStyle.Fill.BackgroundColor.SetColor(Color.FromArgb(0xC6, 0xE0, 0xB4));

                bool swap = true;

                int currentRow = 4;
                for (int i = 0; i < ints.Count(); i++) //iterate over the list of bands
                {
                    int firstRow = currentRow;
                    //postavlja se prvi red (vrijednosti za low i high)
                    worksheet.Cells["A" + currentRow].Value = ints[i].Country;
                    worksheet.Cells["B" + currentRow].Value = ints[i].FrequencyBand;
                    worksheet.Cells["C" + currentRow].Value = ints[i].Application;
                    worksheet.Cells["D" + currentRow].Value = ints[i].RadiocommunicationService;
                    worksheet.Cells["E" + currentRow].Value = ints[i].Channeling;
                    worksheet.Cells["F" + currentRow].Value = ints[i].OccupiedBandwidth;
                    worksheet.Cells["G" + currentRow].Value = ints[i].DirectionSeparation;

                    string temp = ints[i].TransmitPower;
                    while (temp.Length > 40)
                    {
                        int lastSpaceIndex = temp.Substring(0, 40).LastIndexOf(' ');
                        if (lastSpaceIndex <= 0)
                        {
                            // No space found within the substring or at the beginning,
                            // split at the exact character
                            lastSpaceIndex = 40;
                        }

                        worksheet.Cells["H" + currentRow].Value = temp.Substring(0, lastSpaceIndex);
                        temp = temp.Substring(lastSpaceIndex).TrimStart();
                        currentRow++;
                    }
                    worksheet.Cells["H" + currentRow].Value = temp; // Store the remaining part of the comment

                    string cells = "";
                    //postavimo boju za ovaj red iz freqBanda
                    cells = "A" + firstRow + ":H" + currentRow;
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

        private byte[] GenerateExcel150(List<InterfacesView> ints)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                // Set column widths
                worksheet.Column(1).Width = 12;  // Column A 
                worksheet.Column(2).Width = 25;  // Column B
                worksheet.Column(3).Width = 45;  // Column C
                worksheet.Column(4).Width = 20;  // Column D
                worksheet.Column(5).Width = 40;  // Column E
                worksheet.Column(6).Width = 20;  // Column F
                worksheet.Column(7).Width = 30;  // Column G
                worksheet.Column(8).Width = 35;  // Column H

                // Merge cells and write title
                worksheet.Cells["A1:H2"].Merge = true;
                worksheet.Cells["A1"].Value = "Interfaces";
                worksheet.Cells["A1:H2"].Style.Font.Color.SetColor(Color.FromArgb(0x92, 0xD0, 0x50));
                worksheet.Cells["A1"].Style.Font.Size = 16;
                worksheet.Cells["A1"].Style.Font.Name = "Calibri";
                worksheet.Cells["A1"].Style.Font.Bold = true;
                worksheet.Cells["A1"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center; // Align in the middle

                //Format the headers for the table
                OfficeOpenXml.Style.ExcelStyle headerStyle;
                worksheet.Cells["A3"].Value = "Country";
                worksheet.Cells["B3"].Value = "Frequency band";
                worksheet.Cells["C3"].Value = "Application";
                worksheet.Cells["D3"].Value = "Radiocommunication service";
                worksheet.Cells["E3"].Value = "Channeling";
                worksheet.Cells["F3"].Value = "Occupied bandwidth";
                worksheet.Cells["G3"].Value = "Direction/Separation";
                worksheet.Cells["H3"].Value = "Transmit power";
                headerStyle = worksheet.Cells["A3:H3"].Style;

                headerStyle.Font.Bold = true;
                headerStyle.Font.Italic = true;
                headerStyle.Font.Name = "Calibri";
                headerStyle.Font.Size = 14;
                headerStyle.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                headerStyle.Fill.BackgroundColor.SetColor(Color.FromArgb(0xC6, 0xE0, 0xB4));

                bool swap = true;

                int currentRow = 4;
                for (int i = 0; i < ints.Count(); i++) //iterate over the list of bands
                {
                    if (currentRow > 100) break;

                    int firstRow = currentRow;
                    //postavlja se prvi red (vrijednosti za low i high)
                    worksheet.Cells["A" + currentRow].Value = ints[i].Country;
                    worksheet.Cells["B" + currentRow].Value = ints[i].FrequencyBand;
                    worksheet.Cells["C" + currentRow].Value = ints[i].Application;
                    worksheet.Cells["D" + currentRow].Value = ints[i].RadiocommunicationService;
                    worksheet.Cells["E" + currentRow].Value = ints[i].Channeling;
                    worksheet.Cells["F" + currentRow].Value = ints[i].OccupiedBandwidth;
                    worksheet.Cells["G" + currentRow].Value = ints[i].DirectionSeparation;

                    string temp = ints[i].TransmitPower;
                    while (temp.Length > 40)
                    {
                        int lastSpaceIndex = temp.Substring(0, 40).LastIndexOf(' ');
                        if (lastSpaceIndex <= 0)
                        {
                            // No space found within the substring or at the beginning,
                            // split at the exact character
                            lastSpaceIndex = 40;
                        }

                        worksheet.Cells["H" + currentRow].Value = temp.Substring(0, lastSpaceIndex);
                        temp = temp.Substring(lastSpaceIndex).TrimStart();
                        currentRow++;
                    }
                    worksheet.Cells["H" + currentRow].Value = temp; // Store the remaining part of the comment

                    string cells = "";
                    //postavimo boju za ovaj red iz freqBanda
                    cells = "A" + firstRow + ":H" + currentRow;
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
        [Route("Interfaces/DownloadExcel")]
        public byte[] DownloadExcel( int FrequencySizeVal, string FrequencyTableVal)
        {
            JsonResult interfaces = GeneralSearch(FrequencySizeVal, FrequencyTableVal);
            string jsonResult = JsonConvert.SerializeObject(interfaces.Value);
            List<InterfacesView> result = JsonConvert.DeserializeObject<List<InterfacesView>>(jsonResult);
            return GenerateExcel(result);
        }

        //[EnableQuery]
        [HttpPost]
        [Route("Interfaces/DownloadPDF")]
        public byte[] DownloadPDF(int FrequencySizeVal, string FrequencyTableVal)
        {
            // List<AsiaPacific> allocs = getPrintData(AllVal1, AllVal2, AllVal3, AllVal4, FromVal, ToVal, FrequencySizeVal, FreqTable, FreqTableVal, "");
            JsonResult interfaces = GeneralSearch(FrequencySizeVal, FrequencyTableVal);
            string jsonResult = JsonConvert.SerializeObject(interfaces.Value);
            List<InterfacesView> result = JsonConvert.DeserializeObject<List<InterfacesView>>(jsonResult);
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


        public InterfacesController(
       IHttpContextAccessor httpContextAccessor,
        ApplicationDBContext conApp, IConfiguration configuration, AllocationDBContext conAll, ImportTempInterfacesDBContext conInterfaces)
        {

            user = httpContextAccessor.HttpContext.User;
            _conApp = conApp;
            this.configuration = configuration;
           
            _conAll = conAll;
            _conInterfaces = conInterfaces;
            var AllocationFirstListTemp = _conAll.AllocationTermDb.Where(x => x.Layer == 1 && x._PRIMARY == false).OrderBy(x => x.name).ToList();
            var inc = 1;
            foreach(var allTemp in AllocationFirstListTemp)
            {
                if(inc == 1)
                {
                    SelectListItem tt = new SelectListItem
                    {
                        Text = "<all allocation terms>",
                        Value = "" + inc
                    };
                    AllocationFirstList.Add(tt);
                    inc++;
                }
                else
                {
                    SelectListItem tt = new SelectListItem
                    {
                        Text = allTemp.name,
                        Value = "" + inc
                    };
                    AllocationFirstList.Add(tt);
                    inc++;
                }
                
            }

            inc = 1;

            var AllocationSecondListTemp = _conAll.AllocationTermDb.Where(x => x.Layer == 2 && x._PRIMARY == false).OrderBy(x => x.name).ToList();

            foreach (var allTemp in AllocationSecondListTemp)
            {
                if (inc == 1)
                {
                    SelectListItem tt = new SelectListItem
                    {
                        Text = "<all level 2 allocation terms>",
                        Value = "" + inc
                    };
                    AllocationSecondList.Add(tt);
                    inc++;
                }
                else
                {
                    SelectListItem tt = new SelectListItem
                    {
                        Text = allTemp.name,
                        Value = "" + inc
                    };
                    AllocationSecondList.Add(tt);
                    inc++;
                }
            }

            var AllocationThirdListTemp = _conAll.AllocationTermDb.Where(x => x.Layer == 3 && x._PRIMARY == false).OrderBy(x => x.name).ToList();

            inc = 1;
            foreach (var allTemp in AllocationThirdListTemp)
            {
                if (inc == 1)
                {
                    SelectListItem tt = new SelectListItem
                    {
                        Text = "<all level 3 allocation terms>",
                        Value = "" + inc
                    };
                    AllocationThirdList.Add(tt);
                    inc++;
                }
                else
                {
                    SelectListItem tt = new SelectListItem
                    {
                        Text = allTemp.name,
                        Value = "" + inc
                    };
                    AllocationThirdList.Add(tt);
                    inc++;
                }
            }

            var values = _conAll.AllocationTermDb.Where(x => x.Number != null && x._PRIMARY == false).ToList();
            var newApplicationOrderBy = values.OrderBy(x => x.name);
            int incAll = 1;
            foreach(var allTemp in newApplicationOrderBy)
            {
                if (incAll == 1)
                {
                    SelectListItem tt = new SelectListItem
                    {
                        Text = "<all allocation terms>",
                        Value = "" + incAll
                    };
                    AllocationAllList.Add(tt);
                    incAll++;
                }
                else
                {
                    SelectListItem tt = new SelectListItem
                    {
                        Text = allTemp.name,
                        Value = "" + incAll
                    };
                    AllocationAllList.Add(tt);
                    incAll++;
                }
            }

            var ApplicationList = _conApp.RootApplicationTermsDB.Where(x => x.Number != null  ).ToList();

            var ApplicationTempAplhabetic = ApplicationList.OrderBy(x => x.name);

            inc = 1;
            ApplicationAllList.Clear();
            foreach (var tempApp in ApplicationTempAplhabetic)
            {
                if(inc == 1)
                {
                    SelectListItem tt = new SelectListItem
                    {
                        Text = "<all application terms>",
                        Value = "" + inc
                    };
                    ApplicationAllList.Add(tt);
                    inc++;
                }
                else
                {
                    SelectListItem tt = new SelectListItem
                    {
                        Text = tempApp.name,
                        Value = "" + inc
                    };
                    ApplicationAllList.Add(tt);
                    inc++;
                }
            }

            inc = 1;

        }
    }
}
