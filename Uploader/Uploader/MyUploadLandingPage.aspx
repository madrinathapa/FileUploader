﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MyUploadLandingPage.aspx.cs" Inherits="Uploader.MyUploadLandingPage" %>
<%@ MasterType VirtualPath="~/Site.Master" %> 
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
   <div class="row dvSideBar">
        <div class="col-md-5 Upload-Dotted-Border">
            <div class="row">
            <div class="col-md-12 Font-File-Upload hidden-xs">Drag & Drop to upload files. Drag multiple files to create a single zip file.</div>
            <div class="col-md-6">
            <div id ="dvUpload" >
            <input type="file" name="FileUpload" id="htmlUpldFileUploadControl" title="Upload File"  multiple="multiple"/>
            </div>
            </div> 
            <div class="col-md-6" >
            <span id="spnAlertFileInput" style="visibility:hidden; color:#306D32"></span>
            </div>
            </div>
        <div class="Upload-Line-Spacing"></div>
        </div>
        <div class="col-md-1" style="width: 1%;"></div> 
        <div class="col-md-5 Upload-Dotted-Border" >
            <div class="row">
            <div class="col-md-12 Font-File-Upload">Search your files</div>
            <div class="col-md-8"><asp:TextBox ID="txtSearchFileMain" runat="server" class="form-control"></asp:TextBox></div>
            <div class="col-md-4">
            <asp:Button ID="btnSearchFiles" ToolTip="Search Files" runat="server" Text="Search files"   class="btn btn-primary" OnClick="btnSearchFiles_Click" />
            </div>
            <div class="col-md-12 Upload-Line-Spacing"></div>
            </div>
        </div>
        <div class="col-md-1"></div>
    </div>
    
    <div class="Upload-Line-Spacing"></div>
    <button id="btnDelete" type="button" class="btn btn-success" style="display:none;">Delete</button>
    <div class="row dvSideBar">
        <div class="col-md-12" id="dvMain" runat="server">
            <asp:Label Visible="true" ID="lblNoData" runat="server" Text="We couldn't find any match!" CssClass="maindiv"></asp:Label>
    <asp:ListView ID="lvFile" runat="server" OnItemCommand="lvFile_ItemCommand"  GroupItemCount="2" GroupPlaceholderID="groupPlaceholder" OnItemDataBound="lvFile_ItemDataBound">
        <LayoutTemplate>
        <div runat="server" class="row" >
            <div runat="server" id="groupPlaceholder">
            </div>
        </div>
        </LayoutTemplate>

        <GroupTemplate>
        <div runat="server" >
            <asp:PlaceHolder runat="server" ID="itemPlaceholder"></asp:PlaceHolder>
         </div>
        </GroupTemplate>
       
        <ItemTemplate>
             <div class="col-md-5 col-sm-12 dvFile-padding" id="dvFileUploadItemTemplate" runat="server" >
            <div class="row table-bordered BackGround-color-File">
                <div class="col-md-12" style="height: 10px;"></div>
                <div class="col-md-1 col-xs-2 col-sm-2"><asp:ImageButton Class="editTitle" ID="imgEdit" ToolTip="Edit" data-target="#modalEdit" runat="server" style="height:15px; width:15px;" ImageUrl="~/Image/edit.png"  OnClientClick='<%# ReturnChangeFileNameString(Eval("FileId"),Eval("NewFileName"))%> '/></div>
                <div class="col-md-6 col-xs-10 col-sm-10 WrapFree FileName" onclick="DownloadFile(this);"><%# GetShortName(Eval("NewFileName")) %></div>

                <div class="col-md-1 col-xs-2 col-sm-2">
                        <input type="image" onclick="return GenerateUrl(this);" src="Image/URL.png" title="Generate URL">
                       <asp:HiddenField ID="hdnFileUrl" runat="server" Value='' />
              </div>

                <div class="col-md-1 col-xs-2 col-sm-2">
                    <asp:Label ID="Label1" runat="server" Text='<%# Eval("FileId") %>' CssClass="fileId" Style="display:none;"></asp:Label>
                    <asp:ImageButton ID="imgBtnFileDownload" OnClientClick="return UpdateDownloadLog(this);" ToolTip="Download" runat="server" ImageUrl="~/Image/Download.png" CommandArgument='<%#Eval("FileId") %>' CommandName="DownloadFile"/>
                </div>

                <div class="col-md-1 col-xs-2 col-sm-2" id="dvLock" runat="server">
                    <asp:ImageButton ID="imgBtnLock" runat="server" ToolTip="Lock" Style="height: 15px; width: 15px;" ImageUrl="~/Image/Unlock.png" OnClientClick='<%# String.Format("LockFile(\"{0}\"); return false;",Eval("FileId").ToString())%> ' />
                </div>

                <div class="col-md-1 col-xs-2 col-sm-2" id="dvUnlock" runat="server">
                    <asp:ImageButton ID="imgBtnUnlock" data-toggle="dropdown" ToolTip="Unlock" class="dropdown-toggle" ImageUrl="~/Image/Lock.png" runat="server" Style="height: 15px; width: 15px;" />
                    <ul class="dropdown-menu">
                        <li><a href="#" onclick='<%# Eval("FileId", "UnlockFile({0});return false;") %>'>Unlock</a></li>
                        <li><a href="#" onclick='<%# Eval("FileId", "ViewPassword({0});return false;") %>'>View Password</a></li>
                        <li><a href="#"onclick='<%# String.Format("ChangePassword({0}, \"{1}\"); return false;",Eval("FileId"),Convert.ToString(Eval("FilePassword")))%> '>Change Password</a></li>
                    </ul>
                </div>
              
                <div class="col-md-1 col-xs-2 col-sm-2"> 
                    <asp:HiddenField ID="hdnFileName" value='<%# ReturnEmptyIfNull(Eval("OriginalFileName"))%> ' runat="server" />
                    <asp:ImageButton ID="imgBtnFileDelete" runat="server" ToolTip="Delete" ImageUrl="~/Image/Delete.png" OnClientClick='<%# String.Format("DeleteFile(\"{0}\", this); return false;",Eval("FileId").ToString())%> '/>
                </div>

                <div style="height: 10px;" class="col-md-12 hidden-xs hidden-sm"></div>
                <div class="row">
                    <div class="col-md-5 col-xs-5 col-sm-5">
                        <div id="dvFileBox" class="file-box" runat="server">
                            <img id="imgThumb" class="thumbimage" src="//:0"  runat="server" />
                            <div id="divThumb" class="extension-info" runat="server"></div>
                        </div>
                    </div>
                    <div class="col-md-7 col-xs-7 col-sm-7">
                        <div class="row">
                                <div class="col-md-12 col-xs-12 col-sm-12"><asp:Image ID="imgDownloadCount" runat="server" ImageUrl="~/Image/DownloadCount.png" />
                                <span class="Font-sm-span spnDownloadCount"> <%#Eval("DownloadCount") %> </span>
                            </div>
                            
                            <div class="col-md-12 col-xs-12 col-sm-12"><asp:Image ID="imgCalendar" runat="server" ImageUrl="~/Image/Calendar.png" />
                                <span class="WrapFree Font-sm-span"><%#Eval("UploadDate") %></span>
                            </div>
                            
                            <div class="col-md-12 col-xs-12 col-sm-12"><asp:Image ID="imgFileSize" runat="server" ImageUrl="~/Image/Size.png" />
                                <span class="WrapFree Font-sm-span"> <%#Eval("FileSize") %> </span>
                            </div>
                           </div>
                    </div>
                </div>
                <div style="height: 10px;" class="col-md-12"></div>
                </div>
            </div>
             <div class="col-md-1" style="width:1%;"></div>
        </ItemTemplate>
    </asp:ListView>
</div>
    </div>

    <div class="modal fade bs-example-modal-sm" id="dvModalGetUrl" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-sm">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                     <h4 class="modal-title" id="head"></h4>
                </div>
                <div class="modal-body">
                    <div class="form-group">
                        <label for="recipient-name" class="control-label" id="lblModalGetUrl">Download link</label>
                        <input type="text" class="form-control" id="txtFileUrl"/>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade bs-example-modal-sm" id="dvModalEdit" tabindex="-1" role="dialog" aria-labelledby="modalEditLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    <h4 class="modal-title" id="exampleModalLabel">Set title for your share</h4>
                </div>
                <div class="modal-body">
                        <div class="form-group">
                            <label for="recipient-name" class="control-label">Title</label>
                            <input class="form-control" id="txtNewName" />
                        </div>
                </div>
                <div class="modal-footer">

                    <div id="dvWarning" class="alert alert-danger" hidden="hidden">
                        <span aria-hidden="true" class="glyphicon glyphicon-exclamation-sign"></span>
                        <span id="spnInfo">Title changed successfully!</span>
                    </div>
                    <button type="button" class="btn btn-primary" id="btnChangeName">Ok</button>
                    <label for="recipient-name" class="control-label"></label>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade bs-example-modal-sm" id="modalLockFile" tabindex="-1" role="dialog" aria-labelledby="modalLockFileLabel" aria-hidden="true">
        <div class="modal-dialog modal-sm">
            <div class="modal-content ">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    <h4 class="modal-title">Set password</h4>
                </div>
                <div class="modal-body">
                            <label for="recipient-name" class="control-label">Password</label>
                            <input class="form-control" type="password" id="txtFilePassword"  maxlength="16" />
                </div>
                <div class="modal-footer">

                    <div id="dvLockFile" class="alert alert-danger" hidden="hidden">
                        <span aria-hidden="true" class="glyphicon glyphicon-exclamation-sign"></span>
                        <span id="spnLockFile">Password set successfully!</span>
                    </div>
                    <button type="button" class="btn btn-primary" id="btnLockFile">Ok</button>
                    <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>

                    <label for="recipient-name" class="control-label"></label>
                </div>
            </div>
        </div>
    </div>

     <div class="modal fade bs-example-modal-sm" id="modalChangeFilePwd" tabindex="-1" role="dialog" aria-labelledby="modalLockFileLabel" aria-hidden="true">
        <div class="modal-dialog modal-sm">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    <h4 class="modal-title">Change password</h4>
                </div>
                <div class="modal-body">
                        <div class="form-group">
                            <label for="recipient-name" class="control-label">Current Password</label>
                            <input class="form-control" type="password" id="txtExistingPassword" />
                            <label for="recipient-name" class="control-label">New Password</label>
                            <input class="form-control" type="password" id="txtNewFilePassword" />
                        </div>
                </div>
                <div class="modal-footer">

                    <div id="dvChangePwd" class="alert alert-danger" hidden="hidden">
                        <span aria-hidden="true" class="glyphicon glyphicon-exclamation-sign"></span>
                        <span id="spnChangePwd">Password Changed successfully!</span>
                    </div>
                    <button type="button" class="btn btn-primary" id="btnChangeFilePwd">Ok</button>
                    <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>

                    <label for="recipient-name" class="control-label"></label>
                </div>
            </div>
        </div>
    </div>

     <div class="modal fade bs-example-modal-sm" id="dvModalDeleteFile" tabindex="-1" role="dialog" aria-labelledby="modalEditLabel" aria-hidden="true">
        <div class="modal-dialog modal-sm">
            <div class="modal-content">
                <div class="modal-body">
                     <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    <h4 class="modal-title" id="H1">Remove file?</h4>
                </div>
                        <div class="form-group">
                           Are you sure you want to delete this file?
                        </div>
                     <div id="dvDeleteInfo" class="alert alert-danger" hidden="hidden">
                        <span aria-hidden="true" class="glyphicon glyphicon-exclamation-sign"></span>
                        <span id="spnDeleteInfo"></span>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-primary" id="btnDeleteFile">Ok</button>
                    <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>
     <asp:HiddenField ID="hdnFileId"  runat="server" />
     <asp:HiddenField ID="hdnFileIdTinyUrl"  runat="server" />
    
</asp:Content>
