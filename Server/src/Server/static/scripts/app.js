//Intercooler.updateIgnored = function(dest, src) {
//    $("[ic-src='"+dest+"'][ic-deps='ignore']").each(function () {
//        if (src[0] !== this) {
//            Intercooler.triggerRequest($(this));
//        }
//    });
//}

$(document)
    .on("beforeAjaxSend.ic", function (evt, settings) {
        $('form.disabled').find('[type="submit"]').attr('disabled', true)
        settings.xhrFields = { withCredentials: true }
    })
    //.on('success.ic', function (event, elt, _, __, xhr, ___) {
    //    var trigger = "X-IC-Trigger";
    //    if (xhr.getResponseHeader(trigger)) {
    //        var pathsToRefresh = xhr.getResponseHeader(trigger).split(",");
    //        $.each(pathsToRefresh, function (_, str) {
    //            Intercooler.updateIgnored(str.replace(/ /g, ""), elt);
    //        });
    //    }
    //})
    .on('resetForm', function (event, target) {
        var $form = $(target)
        $form.get()[0].reset()
        $form.find('[type="submit"]').attr('disabled', false)
    });

var Track = {
    pipe: (...fs) => arg => fs.reduce((x, f) => f(x), arg)
}

