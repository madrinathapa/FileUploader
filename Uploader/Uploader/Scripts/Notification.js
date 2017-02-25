$(function () {

    var notification = $.connection.notificationHub;

    _initialized = false,
       _init = function (options, cb) {
           options = options || {};
           $.connection.hub.logging = options.debug;
       }
    $.connection.hub.logging = true;
    notification.client.foo = function () { };

    //function to Brodcast  Notifcation Message 

    //notification.client.notify = function (FileID) {

    //    alert("file was Downloaded" + FileID);
    notification.client.broadcastMessage = function (FileID, Name, IsAdmin, MachineName, IpAddress, DownloadDate, fileName) {

        // alert("file was Downloaded" + FileID);
        if ($("#lblUserName").text() == Name) {

           // $('.notificationStyle').attr("src", "Image/globe_red.png").addClass("activeNotification").removeClass("deactiveNotification");

            $('.top-right').notify({
                message: { text: fileName+" File Was Downloaded From " + MachineName + "[" + IpAddress + "]" },
                type:"bangTidy",
                fadeOut: { enabled: true, delay: 10000 },
                closable:false
            }).show();
            var value = $("#notification_count").text();
            value = parseInt(value);

            $("#notification_count").text(value + 1) ;
        }
    };
    //};
    $.connection.hub.start().done(function () {

     

    });

    //$("#navbarNotification").click(function () {
    //    ShowNotificationDropDown();
    //});

    //$("#btnNotification").click(function () {
    //    ShowNotificationDropDown();
    //});

    


    function ShowNotificationDropDown()
    {
        if ($("#liNotificationDropdown").hasClass("open")) {
            $("#liNotificationDropdown").removeClass("open");
        }
        else {
            $("#liNotificationDropdown").addClass("open")
        }

    }




});


$(document).ready(function () {
    $("#notificationLink").click(function () {
        $("#notificationContainer").fadeToggle(300);
        $("#notification_count").fadeOut("slow");
        return false;
    });

    //Document Click hiding the popup 
    $(document).click(function () {
        $("#notificationContainer").hide();
    });

    //Popup on click
    $("#notificationContainer").click(function () {
        return false;
    });

});
