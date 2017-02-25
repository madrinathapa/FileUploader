$(document).ready(function () {

    $("#lnkSignUp").on("keypress", function (e) {
        if (e.keyCode == 13) {
            if (document.activeElement.id == "lnkSignUp") {
                $("#lnkSignUp").click();
            }
        }

    });

    $("#chkRememberMe").on("keypress", function (e) {
        if (e.keyCode == 13) {
            if (document.activeElement.id == "chkRememberMe") {
                $("#chkRememberMe").click();
            }
        }

    });

    $("#lnkSignUp").on("click", function () {
        var url = window.location.pathname;
        var len = url.split("/").length;
        var splittedUrl = url.split("/");
        splittedUrl[len - 1] = "Register.aspx";
        window.location.href = window.location.origin + splittedUrl.join("/");
    });

    $("#dvLogin").keypress(function (e) {
        if (e.key == "<" || e.key == ">" || e.key == " ") {
            return false;
        }
    });

    $("#imgRefresh").on("click", function () {
        $("#txtCaptcha").val("");
    });

    $("#ResetPassword").click(function () {
        $("#dialog").dialog("open");
    });

});


$("#divLogin :text").keypress(function (e) {
    var key = e.which;
    if (key == 13) 
    {
        $('#btnLogin').click();
        return false;
    }
});

