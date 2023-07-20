function showWarning(id,onClose){showPopUpWithUrl("/views2/warningPopup.jsp?siteContentId="+id,onClose);}
function showHelp(id){showPopUpWithUrl("/views2/helpPopup.jsp?siteContentId="+id);}
function showUserInfo(id){showPopUpWithUrl("/view/printRightOfUseInfo.do?id="+id);}
function getDiv(onClose){var div=createModalElement();jQuery(div).on('hidden.bs.modal',function(e){jQuery(div).remove();if(onClose!==undefined){onClose();}});return div;}
function showPopUpWithUrlNoBackDrop(url){var div=getDiv();initModalNoBackdrop(div,url);}
function initModalNoBackdrop(div,url){jQuery(div).modal({backdrop:'static',keyboard:false,remote:url});}
function showPopUpWithUrl(url,onClose){var div=getDiv(onClose);initModal(div,url);}
function initModal(div,url){jQuery(div).modal({remote:url});}
function createModalElement(){var div=document.createElement("div");jQuery(div).addClass("modal fade");jQuery(div).attr("id","generic-popup-modal");jQuery("body").append(div);return div;}