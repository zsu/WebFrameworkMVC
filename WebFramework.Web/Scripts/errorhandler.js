var log = log4javascript.getLogger(); //create the root object
//add a popup appender
//var popUpAppender = new log4javascript.PopUpAppender();
//log.addAppender(popUpAppender);
//add the quintessential Ajax appender
var ajaxAppender = new log4javascript.AjaxAppender(baseUrl + "api/log/");
ajaxAppender.setLayout(new log4javascript.JsonLayout());
ajaxAppender.addHeader("Content-Type", "application/json; charset=utf-8");
log.addAppender(ajaxAppender);
var gOldOnError = window.onerror;
window.onerror = function myErrorHandler(errorMsg, url, lineNumber) {
    var oldState = log4javascript.isEnabled();
    if (!oldState) log4javascript.setEnabled(true);
    return log.error(errorMsg + " [line:" + lineNumber + "] " + url);
    log4javascript.setEnabled(oldState);
}