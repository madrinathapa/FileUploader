<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Admin.aspx.cs" Inherits="Uploader.Admin" %>

<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    
    <script src="Scripts/jquery-2.1.1.js" type="text/javascript"></script>
    <script src="Scripts/jquery-ui.js" type="text/javascript"></script>
    <script src="Scripts/Admin.js" type="text/javascript"></script>
    <script src="Scripts/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/jquery.jqGrid.js" type="text/javascript"></script>

    <link href="Style/jquery-ui.css" rel="stylesheet" type="text/css" />
    <link href="Style/Common.css" rel="Stylesheet" type="text/css" />
    <link type="text/css" rel="stylesheet" media="screen" href="Style/jquery-ui-1.9.2.custom.css" />
    <link type="text/css" rel="stylesheet" media="screen" href="Style/ui.jqgrid.css" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <center>
    <div id="divMain dvSideBar">
   <asp:Button ID="btnDelete"  CssClass="btn btn-success" runat="server" OnClientClick="DeleteFile(); return false;"   style="margin-right:95%" Text="Delete" />
   <table id="divGrid"><tr><td></td></tr></table> 
    <div id="divPaging"></div> 
        </div>
    </center>

    <div class="modal fade" id="myWarningModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    <h3 class="modal-title" id="exampleModalLabel">Warning!</h3>
                </div>
                <div class="modal-body">
                    <div class="form-group">
                        <label for="recipient-name" class="control-label">Please select a file to delete</label>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script>
        function DeleteFile() {
            selectedRows = $("#divGrid").jqGrid("getGridParam", "selarrrow");
            if (selectedRows != "") {
                $.ajax({
                    type: "POST",
                    url: 'UploaderService.svc/DeleteFile',
                    data: '{"selectedRows":"' + selectedRows.toString() + '"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    cache: false,
                    async: true,
                    success: function () {
                        $("#divGrid").trigger("reloadGrid");
                    }
                });
            }
            else {
                jQuery.noConflict();
                (function ($) {
                    $(function () {
                        $('#myWarningModal').modal('show');
                    });
                })(jQuery);
            }
        }
    </script>
</asp:Content>
