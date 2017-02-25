﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RegisteredUsers.aspx.cs" Inherits="Uploader.RegisteredUsers" %>

<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="Scripts/jquery-2.1.1.js" type="text/javascript"></script>
    <script src="Scripts/jquery-ui.js" type="text/javascript"></script>
    <script src="Scripts/RegisteredUsers.js" type="text/javascript"></script>
    <script src="Scripts/grid.locale-en.js" type="text/javascript"></script>
    <script src="Scripts/jquery.jqGrid.js" type="text/javascript"></script>

    <link href="Style/jquery-ui.css" rel="stylesheet" type="text/css" />
    <link href="Style/Common.css" rel="Stylesheet" type="text/css" />
    <link type="text/css" rel="stylesheet" media="screen" href="Style/jquery-ui-1.9.2.custom.css" />
    <link type="text/css" rel="stylesheet" media="screen" href="Style/ui.jqgrid.css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
      <center>
    <div id="dvRegisteredUsers dvSideBar">
   <table id="dvUsersGrid"><tr><td></td></tr></table> 
    <div id="dvPaging"></div> 
        </div>
    </center>

</asp:Content>
