$(document).on("beforeAjaxSend.ic", function (evt, settings) {
    settings.xhrFields = {withCredentials: true};
});
