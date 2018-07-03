$(document).ready(function () {
    
    if (uploadedFile) {
        $(".formToggle").each(function (index) {
            $(this).prop("Disabled", "false");
        });
    } else {
        $(".formToggle").each(function (index) {
            $(this).attr("Disabled", "true");
        });
    }

});