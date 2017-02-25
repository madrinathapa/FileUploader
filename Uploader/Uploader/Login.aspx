<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Uploader.Login" %>

<%@ Register Assembly="MSCaptcha" Namespace="MSCaptcha" TagPrefix="cc1" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link id="Link1" runat="server" rel="shortcut icon" href="favicon.ico" type="image/x-icon"/>
    <link id="Link2" runat="server" rel="icon" href="favicon.ico" type="image/ico"/>
    <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1">
    <script src="Scripts/jquery-2.1.1.js"></script>
    <script src="Style/dist/js/bootstrap.js"></script>
    <script src="Scripts/jquery-ui.js"></script>
    <script src="Scripts/Login.js"></script>
    <link href="Style/jquery-ui.css" rel="stylesheet" />
    <link href="Style/Common.css" rel="Stylesheet" type="text/css" />
    <link href="Style/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link type="text/css" rel="stylesheet" media="screen" href="Style/jquery-ui-1.9.2.custom.css" />
    <title></title>
</head>
<body>
    <form id="frmLogin" runat="server" class="form-horizontal">
        <asp:HiddenField ID="hidIsLogin" Value="1" runat="server" />
        <nav class="navbar navbar-fixed-top  ui-widget-header">
            <div class="container">
                <div class="navbar-header">
                    <button aria-controls="navbar" aria-expanded="false" data-target="#navbar" data-toggle="collapse" class="navbar-toggle collapsed" type="button">
                        <span class="sr-only">Toggle navigation</span>
                        <span class="icon-bar"></span>
                        <span class="icon-bar"></span>
                        <span class="icon-bar"></span>
                    </button>
                    <div >
                <asp:Image  ID="Image1" ImageUrl="~/Image/Logo.png" style="padding-top:3px; height:45px;" runat="server" />
                </div>
                </div>
            </div>
        </nav>

        <center>
        <div class="Holder row ">
              <div  class="col-lg-4 col-md-4 ">
                  </div>
             <div id="dvLogin" runat="server" class="col-lg-4 col-md-4 noPadding">
                <h2>Login</h2>
                <br />
                <div class="form-group">
                    <div class="col-lg-2 col-md-2"></div>
                    <div class="col-lg-8 col-md-8">
                        <asp:TextBox ID="txtEmail" class="form-control" placeholder="Email*" runat="server" TabIndex="1"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="EmailRequiredValidator" runat="server" CssClass="DivGeneral"
                            ForeColor="red" ControlToValidate="txtEmail" ErrorMessage="Email is required" Display="Dynamic" ></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="EmailFormatValidator" runat="server" CssClass="DivGeneral" Display="Dynamic"
                            ForeColor="red" ErrorMessage="Invalid Email" ControlToValidate="txtEmail" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
                    </div>
                    <div class="col-lg-2 col-md-2"></div>
                </div>

                 <div class="form-group">
                     <div class="col-lg-2 col-md-2"></div>
                    <div class="col-lg-8 col-md-8">
                        <asp:TextBox ID="txtPassword" class="form-control" type="password" placeholder="Password*" runat="server" EnableViewState="False" TextMode="Password" TabIndex="2"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rqvPassword" runat="server" CssClass="DivGeneral"
                            ForeColor="red" ControlToValidate="txtPassword" ErrorMessage="Password is required" Display="Dynamic" ></asp:RequiredFieldValidator>
                    </div>
                     <div class="col-lg-2 col-md-2"></div>
                </div>

                 <div id="divErrorMessage" runat="server" style="color: red;" class="DivGeneral" ></div>                
                  <div class="form-group">
                      <div class="col-lg-2 col-md-2"></div>
                    <div class="col-lg-8 col-md-8">
                           <asp:Button ID="btnLogin" class="btn btn-success btn-block" runat="server" Text="Login" OnClick="btnLogin_Click" TabIndex="3" />
                    </div>
                      <div class="col-lg-2 col-md-2"></div>
                  </div>
                  <div class="form-group">
                          <div class="col-lg-2 col-md-2"></div>
                    <div class="col-lg-8 col-md-8">
                        <asp:CheckBox ID="chkRememberMe" runat="server" Text="Remember me" CssClass="DivGeneral" TabIndex="4" />
                        <a id="lnkSignUp" class="btn btn-link" style="text-align: center;" tabindex="5">Register </a>
                    </div>
                          <div class="col-lg-2 col-md-2"></div>
                          </div>
                                   
                </div>
               
             <div  class="col-lg-4 col-md-4 ">
                  </div>
        </div>
        </center>
    </form>
</body>
</html>
