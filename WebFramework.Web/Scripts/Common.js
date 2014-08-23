$().ready(function () {
    setTimezoneCookie();
    handleAjaxMessages();
    displayMessages();
});
function displayMessages() {
    var messagewrapper = $("#messagewrapper");
    var messageboxstatusbar = $("#messageboxstatusbar");
    var messageboxmodal = $("#messageboxmodal");
    if (messagewrapper.children().length > 0) {
        if (messageboxstatusbar && messageboxstatusbar.children().length > 0) {
            var timeoutId;
            messagewrapper.mouseenter(function () {
                if (timeoutId) {
                    clearTimeout(timeoutId);
                    messagewrapper.stop(true).css('opacity', 1).show();
                }
            }).mouseleave(function () {
                timeoutId = setTimeout(function () {
                    messagewrapper.slideUp("slow");
                }, 5000);
            });
            messagewrapper.show();
            // display status message for 5 sec only
            timeoutId = setTimeout(function () {
                messagewrapper.slideUp("slow");
            }, 5000);
            $(document).click(function () {
                clearMessages();
            });
        }
        if (messageboxmodal && messageboxmodal.children().length > 0) {
            messageboxmodal.dialog({
                bgiframe: true,
                autoOpen: false,
                modal: true,
                title: 'Message',
                close: function (event, ui) {
                    try {
                        $(this).dialog('destroy').remove();
                        clearMessages();
                        return true;
                    }
                    catch (e) {
                        return true;
                    }
                }
            });
            messageboxmodal.dialog("open");
            messagewrapper.show();
        }
    }
    else {
        messagewrapper.hide();
    }
}

function clearMessages() {
    $("#messagewrapper").fadeOut(500, function () {
        $("#messagewrapper").empty();
    });
}

function handleAjaxMessages() {
    $(document).ajaxSuccess(function (event, request) {
        if (request.getResponseHeader('FORCE_REDIRECT') !== null) {
            window.location = request.getResponseHeader('FORCE_REDIRECT');
            return;
        }
        checkAndHandleMessageFromHeader(request);
    }).ajaxError(function (event, request) {
        if (request.getResponseHeader('FORCE_REDIRECT') !== null) {
            window.location = request.getResponseHeader('FORCE_REDIRECT');
            return;
        }
        var responseMessage, exception;
        try {//Error handling for POST calls
            var jsonResult = JSON.parse(request.responseText);
            if (jsonResult && jsonResult.Message) {
                responseMessage = jsonResult.Message;
                if (jsonResult.ExceptionMessage) {
                    exception = "Message: " + jsonResult.Message + " Exception: " + jsonResult.ExceptionMessage;
                    if (jsonResult.StackTrace)
                        exception += jsonResult.StackTrace;
                }
            }
        }

        catch (ex) {//Error handling for GET calls
            if (request.responseText)
                responseMessage = request.responseText;
            else
                responseMessage = "Status: '" + request.statusText + "'. Error code: " + request.status;
        }
        if (exception)
            log.error(exception);
        else
            log.error(responseMessage);
        //var message = '<div id="messagebox" behavior=' + '2' + ' class="messagebox ' + "error" + '">' + responseMessage + '</div>';
        //displayMessage(message, "error", 2);
    });
}

function checkAndHandleMessageFromHeader(request) {
    var msg = request.getResponseHeader('X-Message');
    if (msg) {
        displayMessage(msg);
    }
    msg = request.getResponseHeader('X-ModalMessage');
    if (msg) {
        displayModalMessage(msg);
    }
}
function displayMessage(message) {
    var jsonResult = JSON.parse(message);
    if (jsonResult) {
        jQuery('<div/>', {
            id: 'messageboxstatusbar',
            class: 'messagebox'
        }).appendTo('#messagewrapper');
        var messageboxstatusbar = $("#messageboxstatusbar");
        //var messageboxmodal = $("#messageboxmodal");
        var loaded = false;
        //if ((messageboxstatusbar && messageboxstatusbar.children().length > 0) || (messageboxmodal && messageboxmodal.children().length > 0)) {
        //    loaded = true;
        //}
        $.each(jsonResult, function (i, item) {
            if ((messageboxstatusbar && messageboxstatusbar.children().length > 0) && item.Key) {
                if (messageboxstatusbar.children('div[key=' + item.Key + ']').length != 0)
                    return true;
            }
            jQuery('<div/>', {
                class: 'messagebox ' + item.Type,
                text: item.Message,
                key: item.Key
            }).appendTo('#messageboxstatusbar');
        });
        displayMessages();
    }
}
function displayModalMessage(message) {
    var jsonResult = JSON.parse(message);
    if (jsonResult) {
        jQuery('<div/>', {
            id: 'messageboxmodal',
            class: 'messagebox'
        }).appendTo('#messagewrapper');
        //var messageboxstatusbar = $("#messageboxstatusbar");
        var messageboxmodal = $("#messageboxmodal");
        var loaded = false;
        //if ((messageboxstatusbar && messageboxstatusbar.children().length > 0) || (messageboxmodal && messageboxmodal.children().length > 0)) {
        //    loaded = true;
        //}
        $.each(jsonResult, function (i, item) {
            if ((messageboxmodal && messageboxmodal.children().length > 0) && item.Key) {
                if (messageboxmodal.children('div[key=' + item.Key + ']').length != 0)
                    return true;
            }
            jQuery('<div/>', {
                class: 'messagebox ' + item.Type,
                text: item.Message,
                key: item.Key
            }).appendTo('#messageboxmodal');
        });
        displayMessages();
    }
}
function setTimezoneCookie() {

    var timezone_cookie = "timezoneid";

    // if the timezone cookie not exists create one.
    if (!$.cookie(timezone_cookie)) {

        //// check if the browser supports cookie
        //var test_cookie = 'test cookie';
        //$.cookie(test_cookie, true);

        //// browser supports cookie
        //if ($.cookie(test_cookie)) {

        // delete the test cookie
        //$.cookie(test_cookie, null);

        // create a new cookie
        $.cookie(timezone_cookie, jstz.determine().name(), { path: '/' });

        // re-load the page
        location.reload();
        //}
    }
        // if the current timezone and the one stored in cookie are different
        // then store the new timezone in the cookie and refresh the page.
    else {

        var storedTimezone = $.cookie(timezone_cookie);
        var currentTimezone = jstz.determine().name();

        // user may have changed the timezone
        if (storedTimezone !== currentTimezone) {
            $.cookie(timezone_cookie, currentTimezone, { path: '/' });
            location.reload();
        }
    }
}
function getTimezoneName() {
    var timezone_cookie = "timezonename";
    if (!$.cookie(timezone_cookie))
        $.ajax({
            type: 'GET',
            url: baseUrl + 'api/common/timezone/',
            async: false,
            crossDomain: false,
            success: function (data) {
                var timezone_cookie = "timezonename";

                // if the timezone cookie not exists create one.
                if (!$.cookie(timezone_cookie)) {
                    $.cookie(timezone_cookie, data, { path: '/' });
                }
                    // if the current timezone and the one stored in cookie are different
                    // then store the new timezone in the cookie and refresh the page.
                else {

                    var storedTimezone = $.cookie(timezone_cookie);
                    var currentTimezone = data;

                    // user may have changed the timezone
                    if (storedTimezone !== currentTimezone) {
                        $.cookie(timezone_cookie, currentTimezone, { path: '/' });
                    }
                }
            },
            error: function (request, status, error) {
                log.error('Cannot retrieve timezone name. ' + error);
                $.cookie(timezone_cookie, '', { path: '/' });
            }
        });
    if ($.cookie(timezone_cookie))
        return $.cookie(timezone_cookie);
    else
        return '';
}