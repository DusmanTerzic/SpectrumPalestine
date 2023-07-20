using FrekvencijeProject.Controllers.Actions;
using FrekvencijeProject.Models;
using FrekvencijeProject.Models.Json;
using GemBox.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.Controllers
{
    public class DocumentController : Controller
    {
        object user;
        private readonly ApplicationDBContext _conApp;
        private readonly ImportTempTableContext _conImport;
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


        public List<SelectListItem> AllRegulatoryList = new List<SelectListItem>
        {
            new SelectListItem { Text = "<all document types - regulatory and informative documents>", Value = "1" },
            new SelectListItem { Text = "ECC Decs/Recs", Value = "2" },
            new SelectListItem { Text = "Harmonised Standards", Value = "3" },
            new SelectListItem { Text = "Licensing info", Value = "4" },
            new SelectListItem { Text = "NTFA", Value = "5" },
            new SelectListItem { Text = "Other", Value = "6" },
            new SelectListItem { Text = "RE subclass", Value = "7" },
            new SelectListItem { Text = "RIS Models", Value = "8" }
        };

        public List<SelectListItem> AllInformativeList = new List<SelectListItem>
        {
            new SelectListItem { Text = "<all document types - regulatory and informative documents>", Value = "1" },
            new SelectListItem { Text = "ECC-ECO", Value = "2" },
            new SelectListItem { Text = "National", Value = "3" },
            new SelectListItem { Text = "Third party", Value = "4" }
        };

        public string AllDocumentTypesValue { get; set; }


        public List<SelectListItem> ApplicationFirstList = new List<SelectListItem>
        {
            new SelectListItem { Text = "<all application terms>", Value = "1" },
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

        public string ApplicationFirstValue { get; set; }


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
            new SelectListItem { Text = "Medical implants", Value = "94" },
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


        public DocumentController(IHttpContextAccessor httpContextAccessor, IConfiguration configuration,
        ApplicationDBContext conApp, ImportTempTableContext conImport)
        {
            user = httpContextAccessor.HttpContext.User;
            _conApp = conApp;
            _conImport = conImport;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            if (TempData["doc"] != null)
            {
                bool value = (bool)TempData["doc"];
                if (value == true)
                {
                    //Debug.WriteLine(" doc");
                    string from = (string)TempData["tempFrom"];
                    string to = (string)TempData["tempTo"];

                    
                    long valueTo = long.Parse(to);
                    long valueFrom = long.Parse(from);
                    FrequencySizeValue = (string)TempData["FrequencySize"];

                    //take value from frequency size.
                    long result = 0;
                    long resultFrom = 0;
                    if (FrequencySizeValue == "1")
                    {
                        TempData["toTempValue"] = valueTo;
                        TempData["fromTempValue"] = valueFrom;
                        result = valueTo;
                        resultFrom = valueFrom;

                    }
                    else if (FrequencySizeValue == "2")
                    {
                        result = valueTo / 1000;
                        TempData["toTempValue"] = result;

                        resultFrom = valueFrom / 1000;
                        TempData["fromTempValue"] = resultFrom;

                    }
                    else if (FrequencySizeValue == "3")
                    {

                        //1000000.0 mhz
                        result = valueTo / 1000000;

                        

                        //Debug.WriteLine("here:" + valueTo + "==" + result);
                        TempData["toTempValue"] = result;

                        resultFrom = valueFrom / 1000000;
                        TempData["fromTempValue"] = resultFrom;
                    }
                    else if (FrequencySizeValue == "4")
                    {
                        //1000000000.0 GHz
                        

                        result = valueTo / 1000000000;
                        TempData["toTempValue"] = result;

                        resultFrom = valueFrom / 1000000000;
                        TempData["fromTempValue"] = resultFrom;
                    }

                     //Debug.WriteLine("pp:" + FrequencySizeValue+"::"+ resultFrom + "=="+ result);
                    ;
                    this.FrequencytableValue = (string)TempData["FrequencytableValue"];
                    if (this.FrequencytableValue == "0")
                    {
                        
                        return View();
                    }


                    //FrequencytableValue = (string)TempData["FrequencytableValue"];
                    string tempValueFreq = FrequencyTablesList.Where(p => p.Value.Equals(FrequencytableValue)).First().Text;
                    FrequencyTablesList.FirstOrDefault(p => p.Value.Equals(FrequencytableValue)).Selected = true;
                    //List<AllocationSearch> listAl = JsonConvert.DeserializeObject<List<AllocationSearch>>(TempData["allocations"].ToString());
                    //Debug.WriteLine("after al:"+ FrequencytableValue+","+ tempValue);
                    FrequencySizesList.FirstOrDefault(x => x.Value.Equals(FrequencySizeValue)).Selected = true;

                    ViewBag.FreqSize = FrequencySizesList;

                    if (valueFrom == 0 && valueTo == 0)
                    {
                        DocumentActions da = new DocumentActions();
                        var listOfAll = da.SearchAllDocuments(_conApp, FrequencyTablesList.Where(p => p.Text.Equals(tempValueFreq)).First().Value);
                        listOfAll[0].FrequencyTablesList = FrequencyTablesList;
                        listOfAll[0].FrequencySizeValue = FrequencySizeValue;
                        listOfAll[0].FrequencySizesList = FrequencySizesList;
                        return View(listOfAll.ToList());
                    }
                    else
                    {
                        DocumentActions da = new DocumentActions();
                        var listOfDoc = da.SearchAllDocumentsFromTo(_conApp, FrequencyTablesList.Where(p => p.Text.Equals(tempValueFreq)).First().Value, valueFrom, valueTo);
                        listOfDoc[0].FrequencyTablesList = FrequencyTablesList;
                        listOfDoc[0].FrequencySizeValue = FrequencySizeValue;
                        listOfDoc[0].FrequencySizesList = FrequencySizesList;
                        return View(listOfDoc.ToList());
                    }
                    
                    return View();

                }

                
                else
                {
                    return View();
                }
            }
            else
            {
               
                return View();
            }
            //Debug.WriteLine("values:");
            
            //Debug.WriteLine("value:"+TempData["pp"]);

        }

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
        public JsonResult SearchAllDoc(string FrequencyTableVal)
        {
            //Debug.WriteLine("ttt:" + DocumentType+"=="+RadioValue);
            
            DocumentActions da = new DocumentActions();
            var listOfAll = da.SearchAllDocuments(_conApp, FrequencyTablesList.Where(p => p.Text.Equals(FrequencyTableVal)).First().Value);

            return Json(listOfAll.ToList(), new System.Text.Json.JsonSerializerOptions());
           
        }


        [HttpPost]
        public JsonResult SearchAllDocType(string FrequencyTableVal, string DocumentType, string RadioValue,string AbbrValue)
        {
            //Debug.WriteLine("ttt:" + DocumentType+"=="+RadioValue);
            String Document = "";
            if (RadioValue == "R")
            {
                Document = AllRegulatoryList.Where(x => x.Value.Equals(DocumentType)).First().Text;
            }
            else
            {
                Document = AllInformativeList.Where(y => y.Value.Equals(DocumentType)).First().Text;
            }

            DocumentActions da = new DocumentActions();
            var listOfAll = da.SearchAllDocumentsByType(_conApp, FrequencyTablesList.Where(p => p.Text.Equals(FrequencyTableVal)).First().Value, Document);
            if (AbbrValue == "Alphabetical")
            {

                return Json(listOfAll.OrderBy(t => t.CombineTitle).ToList(), new System.Text.Json.JsonSerializerOptions());
            }
            else
            {
                return Json(listOfAll.ToList(), new System.Text.Json.JsonSerializerOptions());
            }

        }

        [HttpPost]
        public JsonResult SearchDocumentByType(string FrequencyTableVal, long tempFrom, long tempTo, string FrequencySizeVal, string DocumentType, string RadioValue ,string AbbrValue)
        {
            String Document = "";
            if (RadioValue == "R")
            {
                Document = AllRegulatoryList.Where(x => x.Value.Equals(DocumentType)).First().Text;
            }
            else
            {
                Document = AllInformativeList.Where(y => y.Value.Equals(DocumentType)).First().Text;
            }

            long FromTT = 0;
            long ToTT = 0;
            string From = "";
            string To = "";
            //Debug.WriteLine("qqq:" + FrequencySizeVal);
            if (FrequencySizeVal == "1")
            {
                if (tempFrom != null)
                {
                    FromTT = tempFrom;
                    From = "" + FromTT;
                }
                if (tempTo != null)
                {
                    ToTT = tempTo;
                    To = "" + ToTT;
                }

            }
            else if (FrequencySizeVal == "2")
            {
                if (tempFrom != null)
                {
                    var s = string.Concat(tempFrom, "000");

                    long value = long.Parse(s);
                    FromTT = value;
                    From = "" + FromTT;
                }

                if (tempTo != null)
                {
                    var s = string.Concat(tempTo, "000");


                    long valueTo = long.Parse(s);
                    // Debug.WriteLine("value:" + valueTo);
                    ToTT = valueTo;
                    // Debug.WriteLine("value 2:" + tempTo);
                    To = "" + ToTT;
                }
            }
            else if (FrequencySizeVal == "3")
            {
                if (tempFrom != null)
                {
                    var s = string.Concat(tempFrom, "000000");

                    long value = long.Parse(s);
                    FromTT = value;

                    From = "" + FromTT;
                }

                if (tempTo != null)
                {
                    var s = string.Concat(tempTo, "000000");

                    long value = long.Parse(s);
                    ToTT = value;
                    To = "" + ToTT;
                }

            }
            else if (FrequencySizeVal == "4")
            {
                if (tempFrom != null)
                {
                    var s = string.Concat(tempFrom, "000000000");

                    long value = long.Parse(s);
                    FromTT = value;

                    From = "" + FromTT;
                }

                if (tempTo != null)
                {
                    var s = string.Concat(tempTo, "000000000");

                    long value = long.Parse(s);
                    ToTT = value;
                    To = "" + ToTT;
                }

            }


            DocumentActions da = new DocumentActions();
            var listOfDoc = da.SearchAllDocumentsFromToByType(_conApp, FrequencyTablesList.Where(p => p.Text.Equals(FrequencyTableVal)).First().Value, FromTT, ToTT,Document);

            if (AbbrValue == "Alphabetical")
            {
                //Debug.WriteLine("www:" + AbbrValue);
                return Json(listOfDoc.OrderBy(t => t.CombineTitle).ToList(), new System.Text.Json.JsonSerializerOptions());
            }
            else
            {
                //Debug.WriteLine("value:" + AbbrValue);
                return Json(listOfDoc.ToList(), new System.Text.Json.JsonSerializerOptions());
            }



        }

            [HttpPost]
        public JsonResult SearchDocument(string FrequencyTableVal, long tempFrom, long tempTo, string FrequencySizeVal)
        {
            long FromTT = 0;
            long ToTT = 0;
            string From = "";
            string To = "";
            //Debug.WriteLine("qqq:" + FrequencySizeVal);
            if (FrequencySizeVal == "1")
            {
                if (tempFrom != null)
                {
                    FromTT = tempFrom;
                    From = "" + FromTT;
                }
                if (tempTo != null)
                {
                    ToTT = tempTo;
                    To = "" + ToTT;
                }

            }
            else if (FrequencySizeVal == "2")
            {
                if (tempFrom != null)
                {
                    var s = string.Concat(tempFrom, "000");

                    long value = long.Parse(s);
                    FromTT = value;
                    From = "" + FromTT;
                }

                if (tempTo != null)
                {
                    var s = string.Concat(tempTo, "000");


                    long valueTo = long.Parse(s);
                    // Debug.WriteLine("value:" + valueTo);
                    ToTT = valueTo;
                    // Debug.WriteLine("value 2:" + tempTo);
                    To = "" + ToTT;
                }
            }
            else if (FrequencySizeVal == "3")
            {
                if (tempFrom != null)
                {
                    var s = string.Concat(tempFrom, "000000");

                    long value = long.Parse(s);
                    FromTT = value;

                    From = "" + FromTT;
                }

                if (tempTo != null)
                {
                    var s = string.Concat(tempTo, "000000");

                    long value = long.Parse(s);
                    ToTT = value;
                    To = "" + ToTT;
                }

            }
            else if (FrequencySizeVal == "4")
            {
                if (tempFrom != null)
                {
                    var s = string.Concat(tempFrom, "000000000");

                    long value = long.Parse(s);
                    FromTT = value;

                    From = "" + FromTT;
                }

                if (tempTo != null)
                {
                    var s = string.Concat(tempTo, "000000000");

                    long value = long.Parse(s);
                    ToTT = value;
                    To = "" + ToTT;
                }

            }


            DocumentActions da = new DocumentActions();
            var listOfDoc = da.SearchAllDocumentsFromTo(_conApp, FrequencyTablesList.Where(p => p.Text.Equals(FrequencyTableVal)).First().Value, FromTT, ToTT);
            
            return Json(listOfDoc.ToList(), new System.Text.Json.JsonSerializerOptions());

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
                data = SearchOnFirstLevel(FromVal, ToVal, FrequencySizeVal.ToString(), FreqTable, ApplicationVal, "", 0);
                string jsonResult = JsonConvert.SerializeObject(data.Value);
                allocs = JsonConvert.DeserializeObject<List<AsiaPacific>>(jsonResult);
            }
            /*else if (AllVal2 != "1")
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
        public JsonResult SearchOnFirstLevel(string FromVal, string ToVal, string FrequencySizeVal, string FrequencyTableVal, string ApplicatonVal,string DocumentsType,int LevelApp)
        {
            SelectListItem App;
            if (LevelApp == 1)
            {
                App = ApplicationFirstList.Where(x => x.Value == ApplicatonVal).SingleOrDefault();
            }else if(LevelApp == 2)
            {
                App = ApplicationSecondList.Where(x => x.Value == ApplicatonVal).SingleOrDefault();
            }
            else
            {
                App = ApplicationThirdList.Where(x => x.Value == ApplicatonVal).SingleOrDefault();
            }
            long tempFrom = 0;
            long tempTo = 0;
            if (FromVal != null && ToVal != null)
            {
                tempFrom = long.Parse(FromVal);
                tempTo = long.Parse(ToVal);
            }

            long FromTT = 0;
            long ToTT = 0;
            
            //Debug.WriteLine("qqq:" + FrequencySizeVal);
            if (FrequencySizeVal == "1")
            {
                if (tempFrom != null)
                {
                    FromTT = tempFrom;
                    
                }
                if (tempTo != null)
                {
                    ToTT = tempTo;
                    
                }

            }
            else if (FrequencySizeVal == "2")
            {
                if (tempFrom != null)
                {
                    var s = string.Concat(tempFrom, "000");

                    long value = long.Parse(s);
                    FromTT = value;
                    
                }

                if (tempTo != null)
                {
                    var s = string.Concat(tempTo, "000");


                    long valueTo = long.Parse(s);
                    // Debug.WriteLine("value:" + valueTo);
                    ToTT = valueTo;
                    // Debug.WriteLine("value 2:" + tempTo);
                    
                }
            }
            else if (FrequencySizeVal == "3")
            {
                if (tempFrom != null)
                {
                    var s = string.Concat(tempFrom, "000000");

                    long value = long.Parse(s);
                    FromTT = value;

                    
                }

                if (tempTo != null)
                {
                    var s = string.Concat(tempTo, "000000");

                    long value = long.Parse(s);
                    ToTT = value;
                    
                }

            }
            else if (FrequencySizeVal == "4")
            {
                if (tempFrom != null)
                {
                    var s = string.Concat(tempFrom, "000000000");

                    long value = long.Parse(s);
                    FromTT = value;

                    
                }

                if (tempTo != null)
                {
                    var s = string.Concat(tempTo, "000000000");

                    long value = long.Parse(s);
                    ToTT = value;
                    
                }

            }



            ApplicationSearchActions asa = new ApplicationSearchActions();
            //Debug.WriteLine("test:" + ApplicatonVal);
            //var listGeneral =  asa.SearchAppOnSecondLevel(_conApp, tempFrom, tempTo, tempFreq, tempApplication);
            if (FromTT == 0 && ToTT == 0)
            {

                //var listGeneral = asa.SearchAppOnSecondLevelProcedure(configuration, tempApplication,_conImport,_conApp);
                var listGeneral = asa.SearchAppOnSecondLevelProcedureNew(_configuration, App.Text, _conImport, _conApp);
                //return Json(listGeneral.OrderBy(e => e.low).ToList(), new System.Text.Json.JsonSerializerOptions());
                List<DocumentStandardView> listOfDocumentsStand = new List<DocumentStandardView>();

                foreach (var values in listGeneral.OrderBy(e => e.low).ToList())
                {
                    values.AppItemsList = values.AppItemsList.OrderBy(x => x.Application).ToList();
                    foreach (var tempAppItemList in values.AppItemsList)
                    {

                        foreach (var tempDoc in tempAppItemList.Documents)
                        {
                            if (tempDoc.DocumentIt > 0)
                            {
                                DocumentStandardView doc = new DocumentStandardView();

                                var ll = (from ww in _conApp.DocumentsDb
                                          select new
                                          {
                                              ww.DocumentsId,
                                              ww.Doc_number,
                                              ww.Title_of_doc,
                                              ww.Hyperlink,
                                              ww.Group_doc

                                          }).Where(x => x.DocumentsId == tempDoc.DocumentIt).SingleOrDefault();


                                doc.Application = tempAppItemList.Application;
                                doc.CombineTitle = tempDoc.Doc_number + " " + tempDoc.Title_of_doc;
                                doc.Notes = tempAppItemList.Comment;
                                doc.FrequencyBand = values.LowView + " - " + values.HighView;
                                doc.Link = tempDoc.Hyperlink;
                                doc.Type = ll.Group_doc;
                                //Debug.WriteLine("documents:" + doc.Notes);
                                listOfDocumentsStand.Add(doc);
                            }
                        }

                        foreach (var tempStand in tempAppItemList.Standards)
                        {
                            if (tempStand.StandardId > 0)
                            {
                                DocumentStandardView stand = new DocumentStandardView();
                                var Standard = (from ww in _conApp.StandardsDb
                                                select new
                                                {
                                                    ww.Standard_id,
                                                    ww.Etsi_standard,
                                                    ww.Title_doc,
                                                    ww.Hypelink,
                                                    ww.Group_doc,
                                                    ww.Low_freq,
                                                    ww.High_freq,
                                                    ww.Application
                                                }).Where(x => x.Standard_id == tempStand.StandardId).SingleOrDefault();

                                stand.Application = tempAppItemList.Application;
                                stand.CombineTitle = tempStand.Etsi_standard + " " + tempStand.Title_docS;
                                stand.Notes =   tempAppItemList.Comment ;
                                //Debug.WriteLine("standards:" + stand.Notes);
                                stand.FrequencyBand = values.LowView + " - " + values.HighView;
                                stand.Link = tempStand.HyperlinkS;
                                stand.Type = Standard.Group_doc;
                                listOfDocumentsStand.Add(stand);
                            }
                        }
                    }
                }
                //Debug.WriteLine("done" + listOfDocumentsStand.Count);
                
                return Json(listOfDocumentsStand.ToList(), new System.Text.Json.JsonSerializerOptions());
            }
            else if (FromTT == 0 && ToTT != 0)
            {
                //var listGeneral = asa.SearchAppOnSecondLevelProcedureFromZero(tempFrom, tempTo, configuration, tempApplication, _conImport, _conApp);
                //var listGeneral = asa.SearchAppOnSecondLevelProcedureFromZeroNew(tempFrom, tempTo, configuration, tempApplication, _conImport, _conApp);
                var listGeneral = asa.SearchAppOnSecondLevelProcedureFromZeroNew(FromTT, ToTT, _configuration, App.Text, _conImport, _conApp);
                //return Json(listGeneral.OrderBy(e => e.low).ToList(), new System.Text.Json.JsonSerializerOptions());
                List<DocumentStandardView> listOfDocumentsStand = new List<DocumentStandardView>();

                foreach (var values in listGeneral.OrderBy(e => e.low).ToList())
                {
                    values.AppItemsList = values.AppItemsList.OrderBy(x => x.Application).ToList();
                    foreach (var tempAppItemList in values.AppItemsList)
                    {

                        foreach (var tempDoc in tempAppItemList.Documents)
                        {
                            if (tempDoc.DocumentIt > 0)
                            {
                                DocumentStandardView doc = new DocumentStandardView();

                                var ll = (from ww in _conApp.DocumentsDb
                                          select new
                                          {
                                              ww.DocumentsId,
                                              ww.Doc_number,
                                              ww.Title_of_doc,
                                              ww.Hyperlink,
                                              ww.Group_doc

                                          }).Where(x => x.DocumentsId == tempDoc.DocumentIt).SingleOrDefault();


                                doc.Application = tempAppItemList.Application;
                                doc.CombineTitle = tempDoc.Doc_number + " " + tempDoc.Title_of_doc;
                                doc.Notes =  tempAppItemList.Comment ;
                                doc.FrequencyBand = values.LowView + " - " + values.HighView;
                                doc.Link = tempDoc.Hyperlink;
                                doc.Type = ll.Group_doc;
                                //Debug.WriteLine("documents:" + doc.Notes);
                                listOfDocumentsStand.Add(doc);
                            }
                        }

                        foreach (var tempStand in tempAppItemList.Standards)
                        {
                            if (tempStand.StandardId > 0)
                            {
                                DocumentStandardView stand = new DocumentStandardView();
                                var Standard = (from ww in _conApp.StandardsDb
                                                select new
                                                {
                                                    ww.Standard_id,
                                                    ww.Etsi_standard,
                                                    ww.Title_doc,
                                                    ww.Hypelink,
                                                    ww.Group_doc,
                                                    ww.Low_freq,
                                                    ww.High_freq,
                                                    ww.Application
                                                }).Where(x => x.Standard_id == tempStand.StandardId).SingleOrDefault();

                                stand.Application = tempAppItemList.Application;
                                stand.CombineTitle = tempStand.Etsi_standard + " " + tempStand.Title_docS;
                                stand.Notes =  tempAppItemList.Comment ;
                                stand.FrequencyBand = values.LowView + " - " + values.HighView;
                                stand.Link = tempStand.HyperlinkS;
                                stand.Type = Standard.Group_doc;
                                //Debug.WriteLine("standards:" + stand.Notes);
                                listOfDocumentsStand.Add(stand);
                            }
                        }
                    }
                }
                return Json(listOfDocumentsStand.ToList(), new System.Text.Json.JsonSerializerOptions());

            }
            else if (FromTT != 0 && ToTT != 0)
            {

                //var listGeneral = asa.SearchAppOnSecondLevelProcedureFromLowHigh(tempFrom, tempTo, configuration, tempApplication, _conImport, _conApp);
                var listGeneral = asa.SearchAppOnSecondLevelProcedureFromLowHighNew(FromTT, ToTT, _configuration, App.Text, _conImport, _conApp);
                //return Json(listGeneral.OrderBy(e => e.low).ToList(), new System.Text.Json.JsonSerializerOptions());
                List<DocumentStandardView> listOfDocumentsStand = new List<DocumentStandardView>();

                foreach (var values in listGeneral.OrderBy(e => e.low).ToList())
                {
                    values.AppItemsList = values.AppItemsList.OrderBy(x => x.Application).ToList();
                    foreach (var tempAppItemList in values.AppItemsList)
                    {

                        foreach (var tempDoc in tempAppItemList.Documents)
                        {
                            if (tempDoc.DocumentIt > 0)
                            {
                                DocumentStandardView doc = new DocumentStandardView();

                                var ll = (from ww in _conApp.DocumentsDb
                                          select new
                                          {
                                              ww.DocumentsId,
                                              ww.Doc_number,
                                              ww.Title_of_doc,
                                              ww.Hyperlink,
                                              ww.Group_doc

                                          }).Where(x => x.DocumentsId == tempDoc.DocumentIt).SingleOrDefault();


                                doc.Application = tempAppItemList.Application;
                                doc.CombineTitle = tempDoc.Doc_number + " " + tempDoc.Title_of_doc;
                                doc.Notes = tempAppItemList.Comment;
                                doc.FrequencyBand = values.LowView + " - " + values.HighView;
                                doc.Link = tempDoc.Hyperlink;
                                doc.Type = ll.Group_doc;
                                //Debug.WriteLine("documents:" + doc.Notes);
                                listOfDocumentsStand.Add(doc);
                            }
                        }

                        foreach (var tempStand in tempAppItemList.Standards)
                        {
                            if (tempStand.StandardId > 0)
                            {
                                DocumentStandardView stand = new DocumentStandardView();
                                var Standard = (from ww in _conApp.StandardsDb
                                                select new
                                                {
                                                    ww.Standard_id,
                                                    ww.Etsi_standard,
                                                    ww.Title_doc,
                                                    ww.Hypelink,
                                                    ww.Group_doc,
                                                    ww.Low_freq,
                                                    ww.High_freq,
                                                    ww.Application
                                                }).Where(x => x.Standard_id == tempStand.StandardId).SingleOrDefault();

                                stand.Application = tempAppItemList.Application;
                                stand.CombineTitle = tempStand.Etsi_standard + " " + tempStand.Title_docS;
                                stand.Notes = tempAppItemList.Comment;
                                stand.FrequencyBand = values.LowView + " - " + values.HighView;
                                stand.Link = tempStand.HyperlinkS;
                                stand.Type = Standard.Group_doc;
                                //Debug.WriteLine("standards:" + stand.Notes);
                                listOfDocumentsStand.Add(stand);
                            }
                        }
                    }
                }
                return Json(listOfDocumentsStand.ToList(), new System.Text.Json.JsonSerializerOptions());

            }

            return null;


            // Debug.WriteLine("the method is just called"+ App.Text);

            //Debug.WriteLine("www:" + listOfDocumentsStandards.Count);
            //return Json(listOfDocumentsStandards.OrderBy(e => e.CombineTitle).ToList(), new System.Text.Json.JsonSerializerOptions());
            
        }

        }
}
