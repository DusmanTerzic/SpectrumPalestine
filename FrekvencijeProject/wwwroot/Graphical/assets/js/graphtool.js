var isGettingPopoverContent = false;
var graphContainerWidth = -1;
var minWidth = -1;
var graphContainers;
var graphTitles;
var isInRenderMode = false;
var isRangeVisible = true;
var renderTexts = {
    low: "Set low frequency from graph by selecting an allocation",
    high: "Set high frequency from graph by selecting an allocation",
    ready: "Select create render to download table. A new tab/window will be opened"
};
var footnoteMap;
var openPopoverAllocation;
var render = false;
var ranges = {
    low: null,
    high: null
};
var timer;

function changeTableWidth(value) {
    var zoom = graphContainerWidth * (1 + (parseInt(value) / 100));
    jQuery(graphContainers).width(zoom + "px");
    jQuery(".suwala-doubleScroll-scroll").width(zoom + "px");
}
String.prototype.width = function (font) {
    var f = font || '12px arial',
        o = $('<div>' + this + '</div>').css({
            'position': 'absolute',
            'float': 'left',
            'white-space': 'nowrap',
            'visibility': 'hidden',
            'font': f
        }).appendTo($('body')),
        w = o.width();
    o.remove();
    return w;
}
jQuery(document).ready(function () {
    if (!render) {
        initSlider();
        initDoubleScroll();
        initHighlight();
        initSearchExpand();
    }
    minWidth = "1374.8 kHz".width() * 3;
    graphContainers = jQuery('[name="graph_container"]');
    jQuery(graphContainers).mouseleave(function () {
        setTimeout(function () {
            if (jQuery(".popover:hover").length == 0) {
                closeOpenPopOver();
            }
        }, 500);
    });
    graphTitles = jQuery(".graph-title");
    graphContainerWidth = jQuery(graphContainers[0]).width();
    changeTableWidth(0);
    setChangeListenerForSearchOptions();
    setChangeListenerForCompare();
    jQuery("#compareCheckBox").trigger("change");
    setChangeListenerForBands();
    setChangeListenerForFrequencyRange();
    if (!render) {
        $(slider).slider("value", maxZoom / 2);
        changeTableWidth(maxZoom / 2);
    }
});

function setChangeListenerForSearchOptions() {
    jQuery("input[name='searchOption']").change(function () {
        var langs = languages[jQuery(this).val()]
        var select = jQuery("#languages");
        jQuery(select).empty();
        $.each(langs, function (index, lang) {
            $option = $("<option></option>").attr("value", lang).text(lang);
            select.append($option);
        });
    });
}

function setChangeListenerForBands() {
    jQuery("#bands").change(function () {
        frequencyBandChanged();
    });
}

function frequencyBandChanged() {
    var selectedItem = jQuery("#bands").find(":selected");
    var low = jQuery(selectedItem).attr("low");
    var high = jQuery(selectedItem).attr("high");
    jQuery("#lowRange").val(low);
    jQuery("#highRange").val(high);
}

function frequencyRangeChanged() {
    var low = jQuery("#frequencyRangeInputs input[name='low']").val();
    var high = jQuery("#frequencyRangeInputs input[name='high']").val();
    var unit = jQuery("#frequencyRangeInputs select[name='unit']").val();
    jQuery("#lowRange").val(low + " " + unit);
    jQuery("#highRange").val(high + " " + unit);
}

function toggleFrequencySelectors() {
    if (jQuery("#specifyRangeCheckbox").is(":checked")) {
        jQuery("#bands").prop("disabled", true);
        jQuery("#frequencyRangeInputs input").prop("disabled", false);
        frequencyRangeChanged();
    } else {
        jQuery("#bands").prop("disabled", false);
        jQuery("#frequencyRangeInputs input").prop("disabled", true);
        frequencyBandChanged();
    }
}

function setChangeListenerForFrequencyRange() {
    toggleFrequencySelectors();
    jQuery("#specifyRangeCheckbox").change(function () {
        toggleFrequencySelectors();
    });
    jQuery("#frequencyRangeInputs input").change(function () {
        frequencyRangeChanged();
    });
    jQuery("#frequencyRangeInputs select").change(function () {
        frequencyRangeChanged();
    });
}

function setChangeListenerForCompare() {
    jQuery("#compareCheckBox").change(function () {
        if (jQuery(this).is(":checked")) {
            jQuery("#user").prop("multiple", true);
        } else {
            jQuery("#user").prop("multiple", false);
        }
    });
}

function handleSetRangeForRender(popover) {
    debugger;
    if (ranges["low"] == null) {
        ranges["low"] = jQuery(popover).attr("lowRange");
        var group = jQuery("#lowRangeForRender");
        jQuery(group).show("fast");
        jQuery(group).find("p").text(ranges["low"]);
        jQuery("#lowRange").val(ranges["low"]);
        jQuery("#exportLow").val(ranges["low"]);
    } else if (ranges["high"] == null) {
        ranges["high"] = jQuery(popover).attr("highRange");
        var group = jQuery("#highRangeForRender");
        jQuery(group).show("fast");
        jQuery(group).find("p").text(ranges["high"]);
        jQuery("#highRange").val(ranges["high"]);
        jQuery("#exportHigh").val(ranges["high"]);
    }
    setRenderMessage();
    toogleRenderButton();
}

function toogleRenderButton() {
    if (ranges["low"] != null && ranges["high"] != null) {
        jQuery("#submitRenderButton").show("fast");
        jQuery("#export").show("fast");
        var selectedUsers = jQuery("#frequency_table").val();
        //var selectedUsers = jQuery("#user").val();
        if (Array.isArray(selectedUsers)) {
            selectedUsers = selectedUsers.join();
        }
        jQuery("#exportUsers").val(selectedUsers);
    } else {
        jQuery("#export").hide();
        jQuery("#submitRenderButton").hide();
        jQuery("#action").val("search");
    }
}

function closeOpenPopOver() {
    if (openPopoverAllocation != null) {
        jQuery(openPopoverAllocation).popover("hide");
        openPopoverAllocation = null;
        return true;
    }
    return false;
}

function itemHover(allocation) {
    var attr = jQuery(allocation).attr('faded');
    if (typeof attr !== typeof undefined && attr == true) {
        closeOpenPopOver();
        return;
    }
    setTimeout(function () {
        if (jQuery(".popover:hover").length == 0) {
            allocationSelected(allocation, false);
        }
    }, 500);
}

function allocationSelected(allocation, isRenderMode) {
    if (render) {
        return;
    }
    if (isRenderMode) {
        return handleSetRangeForRender(allocation);
    }
    if (allocation == openPopoverAllocation) {
        closeOpenPopOver();
        return;
    } else {
        closeOpenPopOver();
    }
    var allocationId = jQuery(allocation).attr("id");
    var range = jQuery(allocation).attr("range");
    var status = jQuery(allocation).attr("status");
    var comment = jQuery(allocation).attr("comment");
    var term = jQuery(allocation).attr("term");
    var allocationContent = "<dl style='font-size: small'>";
    allocationContent += "<dt>Range</dt>";
    allocationContent += "<dd>" + range + "</dd>";
    if (status) {
        allocationContent += "<dt>Status</dt>";
        allocationContent += "<dd>" + status + "</dd>";
    }
    if (comment && comment.length > 0) {
        allocationContent += "<dt>Comment</dt>";
        allocationContent += "<dd>" + comment + "</dd>";
    }
    allocationContent += getFootnoteList(allocation);
    allocationContent += "</dl>";
    jQuery(allocation).popover({
        html: true,
        placement: 'top',
        title: "<span class='text-info'><strong>" +
            term +
            "</strong></span>" +
            "&nbsp;&nbsp;<button type='button' id='close' class='close' onclick='closeOpenPopOver();'>&times;</button>",
        container: 'body',
        content: allocationContent
    }).popover("show");
    openPopoverAllocation = allocation;
    open
}

function itemSelected(allocation) {
    allocationSelected(allocation, isInRenderMode);
}

function showFootNoteModal(key) {
    var text = footnoteMap[key]
    jQuery("#footnote-modal-title").text(key);
    jQuery("#footnote-modal-body").text(text);
    jQuery("#footnote-modal").modal("show");
}

function getFootnoteList(popover) {
    var footnotesString = jQuery(popover).attr("footnotes");
    if (typeof footnotesString == 'undefined' || footnotesString.length == 0) {
        return "";
    }
    footnoteMap = getFootNoteMap(footnotesString);
    var list = "<dt>Footnotes</dt>";
    list += "<dd>";
    list += "<ul class=\"list-unstyled\">"
    jQuery.each(footnoteMap, function (key, value) {
        list += "<li>";
        list += "<a href=\"javascript:showFootNoteModal('" + key + "');\">" +
            key + "</a>";
        list += "</li>"
    });
    list += "</ul>"
    list += "</dd>";
    return list;
}

function getFootNoteMap(footnotesString) {
    var map = {};
    var footnotes = footnotesString.split("@");
    for (var int = 0; int < footnotes.length; int++) {
        var footnote = footnotes[int];
        footnote = footnote.split("??");
        var number = footnote[0];
        var text = footnote[1];
        map[number] = text;
    }
    return map;
}

function toggleRenderImage() {
    isInRenderMode = !isInRenderMode;
    closeOpenPopOver();
    ranges["low"] = null;
    ranges["high"] = null;
    setFormToSearch();
    setRenderMessage();
    toogleRenderButton();
    clearRenderRanges();
    jQuery("#bands").trigger("change");
    jQuery("#form").removeAttr("target");
    jQuery("#action").val("render");
    if (isInRenderMode) {
        jQuery("#showRenderOptionsButton").hide("fast");
    } else {
        jQuery("#showRenderOptionsButton").show("fast");
    }
    jQuery("#renderTableMessage").toggle("fast");
    $('#graphs').find('p').first().text('Your text here');
}

function clearRenderRanges() {
    var group = jQuery("#lowRangeForRender");
    jQuery(group).hide();
    jQuery(group).find("p").text(ranges["low"]);
    group = jQuery("#highRangeForRender");
    jQuery(group).hide();
}

function setRenderMessage() {
    if (ranges["low"] == null) {
        jQuery("#renderMessage").text(renderTexts['low']);
    } else if (ranges["high"] == null) {
        jQuery("#renderMessage").text(renderTexts['high']);
    } else {
        jQuery("#renderMessage").text(renderTexts['ready']);
    }
}

function highlightSelectAction(selectedTermId) {
    if (selectedTermId == -1 || typeof selectedTermId == 'undefined') {
        jQuery('[name="cell-content-item"]').each(function () {
            jQuery(this).fadeTo("slow", 1);
            jQuery(this).removeAttr("faded");
        });
    } else {
        var relevant = getDecendants(selectedTermId);
        relevant.push(selectedTermId);
        jQuery('[name="cell-content-item"]').each(function () {
            if (relevant.includes(jQuery(this).attr("termId"))) {
                jQuery(this).fadeTo("slow", 1);
                jQuery(this).removeAttr("faded");
            } else {
                jQuery(this).fadeTo("slow", 0.15);
                jQuery(this).attr("faded", "true");
            }
        });
    }
}

function getDecendants(selectedTermId) {
    var decendants = [];
    var children = [];
    jQuery('[name="cell-content-item"]').each(function () {
        if (jQuery(this).attr("parentTermId") == selectedTermId) {
            var termId = jQuery(this).attr("termId");
            decendants.push(termId);
            children.push(termId);
        }
    });
    if (children.length != 0) {
        jQuery('[name="cell-content-item"]').each(function () {
            if (children.includes(jQuery(this).attr("parentTermId"))) {
                decendants.push(jQuery(this).attr("termId"));
            }
        });
    }
    return decendants;
}

function resetHighLight() {
    jQuery("#highlight").val("");
    highlightSelectAction("-1");
}

function createRender() {
    debugger;
    if (isIE() && isIE() < 9 && isBrowserWarningEnabled()) {
        alert("Your browser is not supported. Please update your browser");
    } else {
        setFormToRender();
        jQuery("#form").submit();
        toggleRenderImage();
    }

}

function setFormToSearch() {
    jQuery("#action").val("search");
    var form = jQuery("#form");
    jQuery(form).removeAttr("target");
    jQuery(form).removeAttr("action");
}

function setFormToRender() {
    jQuery("#action").val("render");
    var form = jQuery("#form");
    jQuery(form).attr("target", "_blank");
    jQuery(form).attr("action", "https://efis.cept.org/views2/graphToolForRender.jsp");
}

function initDoubleScroll() {
    if (users > 0) {
        jQuery(".double-scroll").doubleScroll();
    } else {
        var scroller = jQuery(".double-scroll");
        jQuery(scroller).css("overflow-x", "scroll");
        /* jQuery(scroller).css("overflow-y", "hidden");*/
        jQuery(scroller).css("margin-bottom", "20px");
        jQuery(scroller).css("margin-bottom", "20px");
        jQuery(scroller).addClass("row");
    }
}

function initSlider() {
    var slider = $("#slider");
    $("#slider").slider({
        range: "min",
        min: 0,
        max: maxZoom,
        value: 0,
        slide: function (event, ui) {
            changeTableWidth(ui.value);
        }
    });
}

function initHighlight() {
    $("#highlight")._autocomplete({
        lookup: terms,
        onSelect: function (suggestion) {
            highlightSelectAction(suggestion.data.termid);
        },
        formatResult: function (suggestion, currentValue) {
            var regex = '(' +
                currentValue.replace(/[\-\[\]\/\{\}\(\)\*\+\?\.\\\^\$\|]/g, "\\$&") + ')';
            return suggestion.data.indentation +
                suggestion.value.replace(new RegExp(regex, 'gi'), '<strong>$1<\/strong>');
        }
    });
}

function initSearchExpand() {
    jQuery("#expanderSearchHead").click(function () {
        jQuery("#form").slideToggle();
        jQuery("#expanderSearchSign").toggleClass("glyphicon-collapse-up");
    });
}
String.prototype.replaceAll = function (str1, str2, ignore) {
    return this.replace(new RegExp(str1.replace(/([\,\!\\\^\$\{\}\[\]\(\)\.\*\+\?\|\<\>\-\&])/g, function (c) {
        return "\\" + c;
    }), "g" + (ignore ? "i" : "")), str2);
};

$(document).ready(function () {
    // Enable popovers everywhere
    $('[data-bs-toggle="popover"]').popover();
});
