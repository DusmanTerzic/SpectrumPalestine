function FormValidFrequency(event) {
    var isNS4 = (navigator.appName == "Netscape") ? 1 : 0;
    if (!isNS4) {
        event.returnValue = false;
        if ((event.keyCode >= 48 && event.keyCode <= 57)
            || (event.keyCode == 13 || event.keyCode == 44 || event.keyCode == 46)) {
            event.returnValue = true;
        }
        return event.returnValue;
    }
    return ((event.which >= 48 && event.which <= 57) || (event.which == 8
        || event.which == 0 || event.which == 13 || event.which == 127
        || event.which == 44 || event.which == 46));
}

function isEmpty(fieldname) {
    return jQuery("input[name=" + fieldname + "]").val() == "";
}

function isIE() {
    var undef, v = 3, div = document.createElement('div'), all = div
        .getElementsByTagName('i');

    while (div.innerHTML = '<!--[if gt IE ' + (++v)
        + ']><i></i><![endif]-->', all[0]);

    return v > 4 ? v : undef;
}
$(document).keyup(function (event) {
    if (event.keyCode == 27) {
        cClick();
    }
});

window._monsido = window._monsido || {
    token: "psd_qypslZ1n705cBt5H0g",
    statistics: {
        enabled: true,
        documentTracking: {
            enabled: false,
            documentCls: "monsido_download",
            documentIgnoreCls: "monsido_ignore_download",
            documentExt: [],
        },
    },
};
window._monsidoConsentManagerConfig = window._monsidoConsentManagerConfig || {
    token: "psd_qypslZ1n705cBt5H0g",
    privacyRegulation: ["gdpr"],
    settings: {
        manualStartup: false,
        hideOnAccepted: true,
        perCategoryConsent: true,
        explicitRejectOption: false,
        hasOverlay: false,
    },
    i18n: {
        languages: ["en_US"],
        defaultLanguage: "en_US"
    },
    theme: {
        buttonColor: "#783CE2",
        buttonTextColor: "#ffffff",
        iconPictureUrl: "cookie",
        iconShape: "circle",
        position: "bottom-right",
    },
    //links: {
    //    cookiePolicyUrl: "https://cept.org/eco/eco-in-brief/data-protection-policies-gdpr",
    //    optOutUrl: "",
    //},
};

var languages = { Allocation: ["Albanian", "Bosnian", "Bulgarian", "Croatian", "Czech", "Danish", "Dutch", "English", "Estonian", "Finnish", "French", "Georgian", "German", "Greek", "Hungarian", "Icelandic", "Italian", "Latvian", "Lithuanian", "Norwegian", "Polish", "Portuguese", "Romanian", "Russian", "Serbian", "Slovak", "Slovenian", "Spanish", "Swedish", "Turkish"], Application: ["Albanian", "Czech", "Danish", "English", "Estonian", "Finnish", "French", "German", "Greek", "Hungarian", "Portuguese", "Russian", "Slovak", "Slovenian", "Spanish"] };
var topLevelWidthInPercent = 0.746268656716418 / 100;
var selectedLanguage = "English";
var users = 1
//var maxZoom = 3350.0;
var maxZoom = 20.0;
var terms = [
    {
        value: "Amateur",
        data: {
            termid: "1",
            indentation: ""
        }
    },
    {
        value: "Amateur-Satellite",
        data: {
            termid: "3",
            indentation: ""
        }
    },
    {
        value: "Amateur-Satellite (Earth-to-space)",
        data: {
            termid: "4",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Amateur-Satellite (space-to-Earth)",
        data: {
            termid: "5",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Broadcasting",
        data: {
            termid: "6",
            indentation: ""
        }
    },
    {
        value: "Broadcasting-Satellite",
        data: {
            termid: "7",
            indentation: ""
        }
    },
    {
        value: "Earth Exploration-Satellite",
        data: {
            termid: "8",
            indentation: ""
        }
    },
    {
        value: "Earth Exploration-Satellite (active)",
        data: {
            termid: "9",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Earth Exploration-Satellite (Earth-to-space)",
        data: {
            termid: "10",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Earth Exploration-Satellite (Earth-to-space) (space-to-space)",
        data: {
            termid: "11",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Earth Exploration-Satellite (passive)",
        data: {
            termid: "12",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Earth Exploration-Satellite (space-to-Earth)",
        data: {
            termid: "13",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Earth Exploration-Satellite (space-to-Earth) (space-to-space)",
        data: {
            termid: "14",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Meteorological-Satellite",
        data: {
            termid: "15",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Meteorological-Satellite (Earth-to-space)",
        data: {
            termid: "16",
            indentation: "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Meteorological-Satellite (space-to-Earth)",
        data: {
            termid: "17",
            indentation: "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Fixed",
        data: {
            termid: "18",
            indentation: ""
        }
    },
    {
        value: "Fixed-Satellite",
        data: {
            termid: "19",
            indentation: ""
        }
    },
    {
        value: "Fixed-Satellite (Earth-to-space)",
        data: {
            termid: "20",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Fixed-Satellite (space-to-Earth)",
        data: {
            termid: "21",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Fixed-Satellite (space-to-Earth) (Earth-to-space)",
        data: {
            termid: "22",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Fixed-Satellite (Earth-to-space) (space-to-Earth)",
        data: {
            termid: "106",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Inter-Satellite",
        data: {
            termid: "23",
            indentation: ""
        }
    },
    {
        value: "Meteorological Aids",
        data: {
            termid: "24",
            indentation: ""
        }
    },
    {
        value: "Mobile",
        data: {
            termid: "25",
            indentation: ""
        }
    },
    {
        value: "Aeronautical Mobile",
        data: {
            termid: "26",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Aeronautical Mobile (OR)",
        data: {
            termid: "27",
            indentation: "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Aeronautical Mobile (R)",
        data: {
            termid: "28",
            indentation: "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Land Mobile",
        data: {
            termid: "29",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Maritime Mobile",
        data: {
            termid: "30",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Maritime Mobile (distress and calling)",
        data: {
            termid: "31",
            indentation: "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Maritime Mobile (distress and safety)",
        data: {
            termid: "98",
            indentation: "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Maritime Mobile (distress, safety and calling)",
        data: {
            termid: "99",
            indentation: "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Maritime Mobile (distress and calling via DSC)",
        data: {
            termid: "115",
            indentation: "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Mobile (distress and calling)",
        data: {
            termid: "32",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Mobile except aeronautical mobile",
        data: {
            termid: "33",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Mobile except aeronautical mobile (R)",
        data: {
            termid: "34",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Mobile (distress and safety)",
        data: {
            termid: "96",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Mobile (distress, safety and calling)",
        data: {
            termid: "97",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Mobile-Satellite",
        data: {
            termid: "35",
            indentation: ""
        }
    },
    {
        value: "Aeronautical Mobile-Satellite",
        data: {
            termid: "36",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Aeronautical Mobile-Satellite (OR)",
        data: {
            termid: "37",
            indentation: "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Aeronautical Mobile-Satellite (R)",
        data: {
            termid: "38",
            indentation: "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Aeronautical Mobile-Satellite (R) (space-to-Earth)",
        data: {
            termid: "39",
            indentation: "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Aeronautical Mobile-Satellite (R) (Earth-to-space)",
        data: {
            termid: "128",
            indentation: "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Land Mobile-Satellite",
        data: {
            termid: "40",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Land Mobile-Satellite (Earth-to-space)",
        data: {
            termid: "41",
            indentation: "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Land Mobile-Satellite (Earth-to-space)",
        data: {
            termid: "107",
            indentation: "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Land Mobile-Satellite (space-to-Earth)",
        data: {
            termid: "114",
            indentation: "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Land Mobile-Satellite",
        data: {
            termid: "42",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Maritime Mobile-Satellite",
        data: {
            termid: "43",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Maritime Mobile-Satellite (Earth-to-space)",
        data: {
            termid: "44",
            indentation: "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Maritime Mobile-Satellite (space-to-Earth)",
        data: {
            termid: "45",
            indentation: "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Mobile-Satellite (Earth-to-space)",
        data: {
            termid: "46",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Mobile-Satellite except aeronautical mobile-satellite (Earth-to-space)",
        data: {
            termid: "47",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Mobile-Satellite (space-to-Earth)",
        data: {
            termid: "48",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Mobile-Satellite except aeronautical mobile-satellite (space-to-Earth) ",
        data: {
            termid: "49",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Mobile-satellite except aeronautical mobile-satellite",
        data: {
            termid: "94",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Mobile-satellite except aeronautical mobile-satellite (R)",
        data: {
            termid: "100",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Mobile-satellite except maritime mobile satellite (space-to-Earth)",
        data: {
            termid: "129",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Not allocated",
        data: {
            termid: "50",
            indentation: ""
        }
    },
    {
        value: "Radio Astronomy",
        data: {
            termid: "51",
            indentation: ""
        }
    },
    {
        value: "Radiodetermination",
        data: {
            termid: "52",
            indentation: ""
        }
    },
    {
        value: "Radiolocation",
        data: {
            termid: "53",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Radionavigation",
        data: {
            termid: "54",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Aeronautical Radionavigation",
        data: {
            termid: "55",
            indentation: "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Maritime Radionavigation",
        data: {
            termid: "56",
            indentation: "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Maritime Radionavigation (radiobeacons)",
        data: {
            termid: "116",
            indentation: "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Radiodetermination-Satellite",
        data: {
            termid: "118",
            indentation: ""
        }
    },
    {
        value: "Radionavigation-Satellite",
        data: {
            termid: "101",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Radionavigation-Satellite (space-to-Earth)",
        data: {
            termid: "67",
            indentation: "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Radionavigation-Satellite (space-to-space)",
        data: {
            termid: "69",
            indentation: "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Aeronautical Radionavigation-Satellite",
        data: {
            termid: "103",
            indentation: "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Maritime Radionavigation-Satellite",
        data: {
            termid: "104",
            indentation: "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Radionavigation-Satellite (Earth-to-space)",
        data: {
            termid: "110",
            indentation: "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Radionavigation-Satellite (space-to-Earth) (space-to-space)",
        data: {
            termid: "111",
            indentation: "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Radionavigation-Satellite (Earth-to-space) (space-to-space)",
        data: {
            termid: "112",
            indentation: "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Radiolocation-Satellite",
        data: {
            termid: "102",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Radiolocation-Satellite (Earth-to-space)",
        data: {
            termid: "61",
            indentation: "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Radiolocation-Satellite (space-to-Earth)",
        data: {
            termid: "62",
            indentation: "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Radiodetermination-Satellite (space-to-Earth)",
        data: {
            termid: "108",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Radiodetermination-Satellite (Earth-to-space)",
        data: {
            termid: "109",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Space Operation",
        data: {
            termid: "70",
            indentation: ""
        }
    },
    {
        value: "Space Operation (Earth-to-space)",
        data: {
            termid: "71",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Space Operation (Earth-to-space) (space-to-Earth)",
        data: {
            termid: "72",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Space Operation (Earth-to-space) (space-to-space)",
        data: {
            termid: "73",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Space Operation (satellite identification)",
        data: {
            termid: "74",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Space Operation (space-to-Earth)",
        data: {
            termid: "75",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Space Operation (space-to-Earth) (space-to-space)",
        data: {
            termid: "76",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Space Research",
        data: {
            termid: "77",
            indentation: ""
        }
    },
    {
        value: "Space Research (active)",
        data: {
            termid: "78",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Space Research (deep space)",
        data: {
            termid: "79",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Space Research (deep space) (Earth-to-space)",
        data: {
            termid: "80",
            indentation: "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Space Research (deep space) (space-to-Earth)",
        data: {
            termid: "81",
            indentation: "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Space Research (Earth-to-space)",
        data: {
            termid: "82",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Space Research (Earth-to-space) (space-to-space)",
        data: {
            termid: "83",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Space Research (passive)",
        data: {
            termid: "84",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Space Research (space-to-Earth)",
        data: {
            termid: "85",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Space Research (space-to-Earth) (space-to-space)",
        data: {
            termid: "86",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Space Research (space-to-space)",
        data: {
            termid: "87",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Standard Frequency and Time Signal",
        data: {
            termid: "88",
            indentation: ""
        }
    },
    {
        value: "Standard frequency and time signal (20 kHz)",
        data: {
            termid: "119",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Standard frequency and time signal (2 500 kHz)",
        data: {
            termid: "120",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Standard frequency and time signal (5 000 kHz)",
        data: {
            termid: "121",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Standard frequency and time signal (10 000 kHz)",
        data: {
            termid: "122",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Standard frequency and time signal (15 000 kHz)",
        data: {
            termid: "123",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Standard frequency and time signal (20 000 kHz)",
        data: {
            termid: "124",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Standard frequency and time signal (25 000 kHz)",
        data: {
            termid: "125",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Standard Frequency and Time Signal-Satellite",
        data: {
            termid: "89",
            indentation: ""
        }
    },
    {
        value: "Standard Frequency and Time Signal-Satellite (Earth-to-space)",
        data: {
            termid: "90",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Standard Frequency and Time Signal-Satellite (space-to-Earth)",
        data: {
            termid: "105",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    },
    {
        value: "Standard frequency and time signal-satellite (400.1 MHz)",
        data: {
            termid: "126",
            indentation: "&nbsp;&nbsp;&nbsp;"
        }
    }
]
