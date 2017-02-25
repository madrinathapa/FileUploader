
$(function () {
    $("#txtCaptcha").val("");
    $("#txtFirstName").val("");
    $("#txtLastName").val("");
    $("#txtSignUpEmail").val("");
    $("#txtSignUpPassword").val("");
    $("#txtSignUpConfirmPassword").val("");

    function RedirectToLogin() {
        var url = window.location.pathname;
        var len = url.split("/").length;
        var splittedUrl = url.split("/");
        splittedUrl[len - 1] = "Login.aspx";
        window.location.href = window.location.origin + splittedUrl.join("/");
    }

    $("#lnkLogin").on("click", RedirectToLogin);

    $("#lnkSuccessfulLogin").on("click", RedirectToLogin);

   

    var chromeBrowser = navigator.userAgent.toLocaleLowerCase();
    if (chromeBrowser.indexOf("chrome") > 0) {
        $("#dvRegister").keypress(function (e) {
            if (e.keyCode == 32 || e.keyCode == 60 || e.keyCode == 62) {
                return false;
            }
        });

    }
    else {
        $("#dvRegister").keypress(function (e) {
            if (e.key == "<" || e.key == ">" || e.key == " ") {
                return false;
            }
        });
    }

    $("#lnkLogin").on("keypress", function (e) {
        if (e.keyCode == 13) {
            if (document.activeElement.id == "lnkLogin") {
                $("#lnkLogin").click();
            }
        }

    });

    $("body").on("keypress", function (e) {
        if (e.keyCode == 13) {
            $("#btnRegister").click();
        }

    });
});