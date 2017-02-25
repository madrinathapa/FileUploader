<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DownloadFile.aspx.cs" Inherits="Uploader.DownloadFile" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
       <script src="Scripts/jquery-2.1.1.js"></script>
    <script src="Style/dist/js/bootstrap.js"></script>
     <link id="Link1" runat="server" rel="shortcut icon" href="favicon.ico" type="image/x-icon"/>
    <link id="Link2" runat="server" rel="icon" href="favicon.ico" type="image/ico"/>
      <link href="Style/dist/css/bootstrap.min.css" rel="stylesheet" />
    <script type="text/javascript">
        $(document).ready(function () {
            $("#dvModalLock").on('shown.bs.modal', function (e) {
                $("#txtFilePassword").focus();
            });
        });
        function DownloadLockedFile() {
            var filePwd = $("#txtFilePassword").val();
            var fileId = $("#hdnFileId").val();
            if (filePwd != "") {
                var srvUrl = "UploaderService.svc/DownloadLockedFile"
                $.ajax({
                    type: "POST",
                    url: srvUrl,
                    data: '{"filePwd":"' + filePwd + '", "fileId":"' + fileId + '"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    cache: false,
                    async: true,
                    success: function (msg) {
                        if (msg.d == true) {
                            $("#txtFilePassword").val("");
                            $("#dvLockFile").removeClass("alert-danger");
                            $("#dvLockFile").addClass("alert-success");
                            $("#dvLockFile").show();
                            $('#dvModalLock').modal('hide');
                        }
                        else {
                            $("#spnLockFile").text("Enter correct password!");
                            $("#dvLockFile").show();
                        }
                        $("#txtFilePassword").val("");
                    }
               });
            }
            else {
                $("#spnLockFile").text("Enter the password!");
                $("#dvLockFile").show();
                $("#txtFilePassword").val("");
            }
        }

        function ShowModalBox() {
            $('#dvModalLock').modal('show');
        }

        function ShowModalBoxWithMessage() {
            $('#dvModalLock').modal('show');
            $("#spnLockFile").text("Please enter correct password!");
            $("#dvLockFile").show();
        }

        function HideModalBox() {
            setTimeout(function () { $('#dvModalLock').modal('hide'); }, 2000);
        }

        $(function () {
            $("#txtFilePassword").on("keypress", function (e) {
                var key = e.which;
                if (key == 13) {
                    $('#btnUnlock').click();
                    return false;
                }
            });
        });

        function hideModal()
        {
            $('#dvModalLock').modal('hide');
            return true;
        }
</script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:HiddenField ID="hdnFileId" runat="server" />
        <asp:HiddenField ID="hdnFilePassword" runat="server" />
    <div class="modal fade bs-example-modal-sm" id="dvModalLock" tabindex="-1" role="dialog" aria-labelledby="dvModalLockLabel" aria-hidden="true">
        <div class="modal-dialog modal-sm">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    <h4 class="modal-title">Enter the password</h4>
                </div>
                <div class="modal-body">
                        <div class="form-group">
                            <label for="recipient-name" class="control-label">Password</label>
                            <input class="form-control" id="txtFilePassword" type="password" runat="server" autocomplete="off"/>
                        </div>
                </div>
                <div class="modal-footer">
                    <div hidden="hidden" class="alert alert-danger" id="dvLockFile">
                        <span class="glyphicon glyphicon-exclamation-sign" aria-hidden="true"></span>
                        <span id="spnLockFile"></span>
                    </div>
                    <asp:Button runat="server" CssClass="btn btn-primary" OnClientClick="return hideModal();" Text="Download" OnClick="btnUnlock_ServerClick" ID="btnUnlock"/>
                    <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
                    <label for="recipient-name" class="control-label"></label>
                </div>
            </div>
        </div>
    </div>
    </div>
    </form>
</body>
      
</html>

