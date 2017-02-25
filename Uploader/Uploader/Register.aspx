<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="Uploader.Register" %>
<%@ Register Assembly="MSCaptcha" Namespace="MSCaptcha" TagPrefix="cc1" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link id="Link1" runat="server" rel="shortcut icon" href="favicon.ico" type="image/x-icon"/>
    <link id="Link2" runat="server" rel="icon" href="favicon.ico" type="image/ico"/>
    <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1">
     <script src="Scripts/jquery-2.1.1.js"></script>
    <script src="Style/dist/js/bootstrap.js"></script>
     <link href="Style/Common.css" rel="Stylesheet" type="text/css" />
    <link href="Style/dist/css/bootstrap.min.css" rel="stylesheet" />
    <script src="Scripts/Register.js"></script>
    <title>File Uploader</title>
</head>
<body>
    <form id="frmRegister" runat="server" class="form-horizontal" autocomplete="off">
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
            <div class="col-lg-4 col-md-4">
           <div class="form-group " id="dvSuccessfulAlert" runat="server" Visible="false"> 
                    <div class="alert alert-info row">
                        <div class="col-md-12">
                            <span aria-hidden="true" class="glyphicon glyphicon-exclamation-sign"></span>      
                            <span id="spnChangePwd">You have Successfully registred.</span>
                        </div>
                        <div class="col-md-12"> 
                            <a style="" class="btn btn-link" id="lnkSuccessfulLogin">Please click here to login.</a>
                        </div>
                    </div>
                </div>
            <div id="dvRegister" runat="server" class=" noPadding">
                <h2>Register</h2>
                <br />
                <div class="form-group">
                    <div class="col-lg-2 col-md-2"></div>
                    <div class="col-lg-8 col-md-8">
                      <asp:TextBox ID="txtFirstName" class="form-control" placeholder="First Name*" runat="server" TabIndex="1"></asp:TextBox>
                          <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" CssClass="DivGeneral"  Display="Dynamic"
                            ForeColor="red" ControlToValidate="txtFirstName" ErrorMessage="First name is required"></asp:RequiredFieldValidator>
                    </div>
                    <div class="col-lg-2 col-md-2"></div>
                </div>
                <div class="form-group">
                    <div class="col-lg-2 col-md-2"></div>
                    <div class="col-lg-8 col-md-8">
                        <asp:TextBox ID="txtLastName" class="form-control" placeholder="Last Name*" runat="server" TabIndex="2"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" Display="Dynamic" CssClass="DivGeneral"
                            ForeColor="red" ControlToValidate="txtLastName" ErrorMessage="Last name is required"></asp:RequiredFieldValidator>
                    </div>
                    <div class="col-lg-2 col-md-2"></div>
                </div>
                <div class="form-group">
                    <div class="col-lg-2 col-md-2"></div>
                    <div class="col-lg-8 col-md-8">
                        <asp:TextBox ID="txtSignUpEmail" class="form-control" placeholder="Email*" runat="server" TabIndex="3"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" CssClass="DivGeneral"
                            ForeColor="red" ControlToValidate="txtSignUpEmail" ErrorMessage="Email is required" Display="Dynamic"></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" CssClass="DivGeneral"
                            ForeColor="red" Display="Dynamic" ErrorMessage="Invalid Email" ControlToValidate="txtSignUpEmail" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
                    </div>
                    <div class="col-lg-2 col-md-2"></div>
                </div>
                <div class="form-group">
                    <div class="col-lg-2 col-md-2"></div>
                    <div class="col-lg-8 col-md-8">
                        <asp:TextBox ID="txtSignUpPassword" class="form-control" placeholder="Password*" type="password" TextMode="Password" runat="server" TabIndex="4"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rqvSignUpPassword" Display="Dynamic"  CssClass="DivGeneral" runat="server" ForeColor="red" ErrorMessage="Enter password" ControlToValidate="txtSignUpPassword"></asp:RequiredFieldValidator>
                    </div>
                    <div class="col-lg-2 col-md-2"></div>
                </div>
                <div class="form-group">
                    <div class="col-lg-2 col-md-2"></div>
                    <div class="col-lg-8 col-md-8">
                        <asp:TextBox ID="txtSignUpConfirmPassword" class="form-control" placeholder="Re-enter password*" type="password" TextMode="Password" runat="server" TabIndex="5"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rqvSignUpConfirmPassword"  Display="Dynamic"  CssClass="DivGeneral" runat="server" ForeColor="red" ErrorMessage="Re-enter password" ControlToValidate="txtSignUpConfirmPassword"></asp:RequiredFieldValidator>
                        <asp:CompareValidator ID="CompareValidator1" runat="server" ForeColor="red" ErrorMessage="Passwords did not match!" ControlToCompare="txtSignUpPassword" ControlToValidate="txtSignUpConfirmPassword" Operator="Equal" Display="Dynamic"></asp:CompareValidator>
                    </div>
                    <div class="col-lg-2 col-md-2"></div>
                </div>
                <div class="form-group">
                    <div class="col-lg-3 col-md-3"></div>
                    <div class="col-lg-4 col-md-4">
                        <cc1:captchacontrol ID="SignUpCaptcha" runat="server" CaptchaBackgroundNoise="Low" CaptchaLength="5"
                                    CaptchaHeight="50" CaptchaWidth="150" CaptchaMinTimeout="5" CaptchaMaxTimeout="240"
                                    FontColor="black" NoiseColor="#B1B1B1" />
									</div>
                    <div class="col-lg-1 col-md-1"></div>
									<div class="col-lg-2 col-md-2">
                         <asp:ImageButton ID="imgRefresh" ImageUrl="Image/Refresh.png" runat="server" CausesValidation="false" TabIndex="6"/>
                    </div>
                    <div class="col-lg-2 col-md-2"></div>
                </div>
                <div class="form-group">
                     <div class="col-lg-2 col-md-2"></div>
                    <div class="col-lg-8 col-md-8">
                        <asp:TextBox ID="txtCaptcha" placeholder="Enter the text" runat="server" class="form-control" TabIndex="7"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rqvCaptcha" Display="Dynamic"  CssClass="DivGeneral" runat="server" ForeColor="red" ErrorMessage="Enter captcha" ControlToValidate="txtCaptcha"></asp:RequiredFieldValidator>
                        <asp:CustomValidator ID="CustomValidatorCaptcha" ForeColor="red" ErrorMessage="Invalid. Please try again." OnServerValidate="ValidateCaptcha"
                                    runat="server" Display="Dynamic" />
                    </div>
                     <div class="col-lg-2 col-md-2"></div>
                </div>
                <div id="divExistingUser" style="color: red;" runat="server" class="DivGeneral"></div>
                <div class="form-group">
                    <div class="col-lg-2 col-md-2"></div>
                    <div class="col-lg-8 col-md-8">
                       <asp:Button ID="btnRegister" class="btn btn-success btn-block" runat="server" Text="Register" OnClick="btnRegister_Click" TabIndex="8" />
                                <a id="lnkLogin" class="btn btn-link"  tabindex="9">Login </a>
                    </div>
                    <div class="col-lg-2 col-md-2"></div>
                </div>
            </div>
            </div>
            <div  class="col-lg-4 col-md-4 ">
            </div>
        </div>
        </center>
    </form>
</body>
</html>
