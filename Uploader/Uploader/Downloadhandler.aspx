<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Downloadhandler.aspx.cs" Inherits="Uploader.Downloadhandler" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>handler</title>
           <script src="Scripts/jquery-2.1.1.js"></script>
    <script src="Scripts/jquery.signalR-2.2.0.min.js"></script>
    <script src='<%: ResolveClientUrl("~/signalr/hubs") %>'></script>
    <script type="text/javascript">

        $(document).ready(function () {

            PushNotification();

        });

        function PushNotification() {

            //alert("I ran Upto Here");
            //Declare a proxy to Reference a Hub
            var notification = $.connection.notificationHub;

            //Start a Connection
            $.connection.hub.start().done(function () {
                notification.server.send($("#hdnDownloadFileId").val());
                //$("#hdnFileId").val()
               // alert("I ran Upto Here 2 ");


            });
            notification.client.broadcastmessage = function (fileid) {

                //alert("file was downloaded" + fileid);
            };
        }

    </script>
</head>
<body>
        <iframe id="frmDownloadhandler" src="" style="width:100%;height:100%" frameborder='0' runat="server"></iframe>
    <form id="formHidden" runat="server">
        <asp:HiddenField ID="hdnDownloadFileId" runat="server" /></form>
</body>
</html>
