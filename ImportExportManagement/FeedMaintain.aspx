<%@ Page Language="vb" AutoEventWireup="false" Codebehind="FeedMaintain.aspx.vb" Inherits="ImportExportManagement.FeedMaintain" ValidateRequest="False"%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
	<head>
		<title runat="server" id="PageCaption"></title>
		<meta content="Microsoft Visual Studio.NET 7.0" name="GENERATOR">
		<meta content="Visual Basic 7.0" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<link title="BVIStyle" href="BVI.css" type="text/css" rel="stylesheet">
		<link href="menu.css" rel="stylesheet">
		<script language="JavaScript" src="menu.js"></script>
		<script language="JavaScript" src="menu_items.js"></script>
		<script language="JavaScript" src="menu_tpl.js"></script>
		<script language="JavaScript" src="calendar2.js"></script>
	</head>
	<body>
		<form id="form1" name="form1" action="FeedMaintain.aspx" method="post" runat="server">
			<input type="hidden" name="hdAction">
			<asp:literal id="litHiddens" runat="server" enableviewstate="False"></asp:literal>
			<table class="PrimaryTbl" style="LEFT: 140px; POSITION: absolute; TOP: 14px" cellspacing="0"
				cellpadding="0" width="650" border="0">
				<tr style="DISPLAY: none">
					<td width="250">&nbsp;</td>
					<td>&nbsp;
					</td>
				</tr>
				<tr>
					<td class="PrimaryTblTitle" colspan="2"><asp:literal id="litHeading" runat="server"></asp:literal></td>
				</tr>
				<tr>
					<td class="cellseparator" colspan="2"></td>
				</tr>
				<tr>
					<td colspan="2">&nbsp;</td>
				</tr>
				<tr>
					<td class="Cell9Reg" align="left">Client:</td>
					<td><asp:dropdownlist id="ddClientID" runat="server"></asp:dropdownlist><asp:textbox id="txtClientID" runat="server" maxlength="50" width="300px"></asp:textbox></td>
				</tr>
				<tr>
					<td class="Cell9Reg" align="left">Process Name:</td>
					<td><asp:textbox id="txtProcessName" runat="server" maxlength="50" width="300px"></asp:textbox></td>
				</tr>
				<asp:placeholder id="phExport" runat="server">
					<tr>
						<td class="Cell9Reg" align="left">Destination:</td>
						<td>
							<asp:dropdownlist id="ddDestinationID" runat="server"></asp:dropdownlist>
							<asp:textbox id="txtDestinationID" runat="server" width="300px" maxlength="10"></asp:textbox></td>
					</tr>
				</asp:placeholder>
				<tr>
					<td class="Cell9Reg" align="left">Carrier:</td>
					<td><asp:dropdownlist id="ddCarrierID" runat="server"></asp:dropdownlist><asp:textbox id="txtCarrierID" runat="server" width="300px"></asp:textbox></td>
				</tr>
				<tr>
					<td class="Cell9Reg" align="left">Frequency:</td>
					<td><asp:dropdownlist id="ddFrequencyID" runat="server"></asp:dropdownlist><asp:textbox id="txtFrequencyID" runat="server" maxlength="50" width="300px"></asp:textbox></td>
				</tr>
				<tr>
					<td class="Cell9Reg" align="left">Status:</td>
					<td><asp:radiobuttonlist id="rbActiveInd" style="FONT: 9pt Arial, Helvetica, sans-serif" runat="server" repeatdirection="Horizontal">
							<asp:listitem value="1">Active</asp:listitem>
							<asp:listitem value="0">Inactive</asp:listitem>
						</asp:radiobuttonlist><asp:textbox id="txtActiveInd" runat="server" maxlength="50" width="300px"></asp:textbox></td>
				</tr>
				<tr>
					<td class="Cell9Reg" align="left">Scheduled:</td>
					<td><asp:radiobuttonlist id="rbScheduledInd" style="FONT: 9pt Arial, Helvetica, sans-serif" runat="server"
							repeatdirection="Horizontal">
							<asp:listitem value="1">Yes</asp:listitem>
							<asp:listitem value="0">No</asp:listitem>
						</asp:radiobuttonlist><asp:textbox id="txtScheduledInd" runat="server" maxlength="50" width="300px"></asp:textbox></td>
				</tr>
				<tr>
					<td class="Cell9Reg" align="left">Carrier Sends Receipt:</td>
					<td><asp:radiobuttonlist id="rbReceiptInd" style="FONT: 9pt Arial, Helvetica, sans-serif" runat="server"
							repeatdirection="Horizontal">
							<asp:listitem value="1">Yes</asp:listitem>
							<asp:listitem value="0">No</asp:listitem>
						</asp:radiobuttonlist><asp:textbox id="txtReceiptInd" runat="server" maxlength="50" width="300px"></asp:textbox></td>
				</tr>
				<tr>
					<td class="Cell9Reg" align="left"><asp:label id="lblBVIOrCarrierSendsErrorReport" runat="server"></asp:label></td>
					<td><asp:radiobuttonlist id="rbBVIOrCarrierSendsErrorReportInd" style="FONT: 9pt Arial, Helvetica, sans-serif"
							runat="server" repeatdirection="Horizontal">
							<asp:listitem value="1">Yes</asp:listitem>
							<asp:listitem value="0">No</asp:listitem>
						</asp:radiobuttonlist><asp:textbox id="txtBVIOrCarrierSendsErrorReportInd" runat="server" maxlength="50" width="300px"></asp:textbox></td>
				</tr>
				<tr>
					<td class="Cell9Reg" align="left">Error Report Handler:</td>
					<td><asp:dropdownlist id="ddErrorReportHandler" runat="server"></asp:dropdownlist><asp:textbox id="txtErrorReportHandler" runat="server" maxlength="100" width="300px"></asp:textbox></td>
				</tr>
				<tr>
					<td class="Cell9Reg" align="left">Error Report Source:</td>
					<td><asp:textbox id="txtErrorReportSource" runat="server" maxlength="100" width="300px"></asp:textbox></td>
				</tr>
				<tr>
					<td class="Cell9Reg" align="left">File Name:</td>
					<td><asp:textbox id="txtFileName" runat="server" maxlength="100" width="300px"></asp:textbox></td>
				</tr>
				<tr>
					<td class="Cell9Reg" align="left">File Location:</td>
					<td><asp:textbox id="txtFileLocation" runat="server" maxlength="150" width="300px"></asp:textbox></td>
				</tr>
				<tr>
					<td class="Cell9Reg" align="left">File Destination:</td>
					<td><asp:textbox id="txtFileDestLocation" runat="server" maxlength="150" width="300px"></asp:textbox></td>
				</tr>
				<tr>
					<td class="Cell9Reg" align="left">Email To:</td>
					<td><asp:textbox id="txtEmailTo" runat="server" maxlength="150" width="300px"></asp:textbox></td>
				</tr>
				<tr>
					<td class="Cell9Reg" align="left">Email Attachment:</td>
					<td><asp:radiobuttonlist id="rbEmailAttachmentInd" style="FONT: 9pt Arial, Helvetica, sans-serif" runat="server"
							repeatdirection="Horizontal">
							<asp:listitem value="1">Yes</asp:listitem>
							<asp:listitem value="0">No</asp:listitem>
						</asp:radiobuttonlist><asp:textbox id="txtEmailAttachmentInd" runat="server" maxlength="15" width="300px"></asp:textbox></td>
				</tr>
				<tr>
					<td class="Cell9Reg" align="left">FTP User Name:</td>
					<td><asp:textbox id="txtFTPUserName" runat="server" maxlength="50" width="300px"></asp:textbox></td>
				</tr>
				<tr>
					<td class="Cell9Reg" align="left">FTP Password:</td>
					<td><asp:textbox id="txtFTPPassword" runat="server" maxlength="50" width="300px"></asp:textbox></td>
				</tr>
				<tr>
					<td class="Cell9Reg" align="left">PGP Key:</td>
					<td><asp:textbox id="txtPGPKey" runat="server" maxlength="50" width="300px"></asp:textbox></td>
				</tr>
				<tr>
					<td class="Cell9Reg" align="left">Notes:</td>
					<td><asp:textbox id="txtNotes" runat="server" width="300px" textmode="MultiLine" rows="4"></asp:textbox></td>
				</tr>
				<tr>
					<td class="Cell9Reg" align="left">Developer:</td>
					<td><asp:dropdownlist id="ddDeveloper" runat="server"></asp:dropdownlist><asp:textbox id="txtDeveloper" runat="server" maxlength="50" width="300px"></asp:textbox></td>
				</tr>
				<asp:placeholder id="phQASection" runat="server">
					<tr>
						<td colspan="2">&nbsp;</td>
					</tr>
					<tr>
						<td class="Cell9Reg" align="left">QA POC:</td>
						<td>
							<asp:dropdownlist id="ddQAPOC" runat="server"></asp:dropdownlist>
							<asp:textbox id="txtQAPOC" runat="server" width="300px" maxlength="50"></asp:textbox></td>
					</tr>
					<tr>
						<td class="Cell9Reg" align="left">QA Status:</td>
						<td>
							<asp:textbox id="txtQAStatus" runat="server" width="300px" maxlength="100"></asp:textbox></td>
					</tr>
					<tr>
						<td class="Cell9Reg" align="left">QA Notes:</td>
						<td>
							<asp:textbox id="txtQANotes" runat="server" width="300px" maxlength="100"></asp:textbox></td>
					</tr>
					<tr>
						<td class="Cell9Reg" align="left">QA DC Scheduled:</td>
						<td>
							<asp:radiobuttonlist id="rbQADCScheduledInd" style="FONT: 9pt Arial, Helvetica, sans-serif" runat="server"
								repeatdirection="Horizontal">
								<asp:listitem value="1">Yes</asp:listitem>
								<asp:listitem value="0">No</asp:listitem>
							</asp:radiobuttonlist>
							<asp:textbox id="txtQADCScheduledInd" runat="server" width="300px" maxlength="15"></asp:textbox></td>
					</tr>
					<tr>
						<td class="Cell9Reg" align="left">QA DC Last Mod Date:</td>
						<td class="Cell9Reg">
							<asp:textbox id="txtQADCLastModDate" runat="server" width="112px" maxlength="50" readonly="True"></asp:textbox>&nbsp;&nbsp;
							<asp:label id="lblQADCLastModDateLink" runat="server">
								<a href="javascript:GetDate('QADCLastModDate')">Get Date</a></asp:label></td>
					</tr>
				</asp:placeholder>
				<tr>
					<td colspan="2">&nbsp;</td>
				</tr>
				<tr>
					<td align="center" colspan="2"><asp:literal id="litUpdate" runat="server" enableviewstate="False"></asp:literal>&nbsp;&nbsp;<input onclick="ReturnToParentPage()" type="button" value="Return"></td>
				</tr>
				<tr>
					<td colspan="2">&nbsp;</td>
				</tr>
			</table>
			<asp:literal id="litResponseAction" runat="server" enableviewstate="False"></asp:literal><asp:label id="lblCurrentRights" runat="server"></asp:label>
			<asp:literal id="litEnviro" runat="server" enableviewstate="False"></asp:literal>
			<script language="javascript">	
				document.write("<table style='background:#eeeedd;PADDING-LEFT: 4px;FONT: 8pt Arial, Helvetica, sans-serif; POSITION: absolute;TOP: 14px' cellSpacing='0' cellPadding='0' width='125' border='0'><tr><td width='20'>User:</td><td>" + form1.hdLoggedInUserID.value + "</td></tr><tr><td>Site:</td><td WRAP=HARD>" + form1.hdDBHost.value + "</td></tr></table>")			
				new menuitems("feedlist1", form1.currentrights.value);
				new tpl("v", "admin");
				new menu (MENU_ITEMS, MENU_TPL);				
			</script>
			<asp:literal id="litMsg" runat="server" enableviewstate="False"></asp:literal>
			<script type="text/javascript">
			function legallength(txtarea, legalmax)  {	
				if (txtarea.getAttribute && txtarea.value.length>legalmax) {
					txtarea.value=txtarea.value.substring(0,legalmax)
				}
			}
							
			function Update()  {
				form1.hdAction.value = "update"
				form1.submit()
			}
			
			function ReturnToParentPage()  {
				form1.hdAction.value = "return"
				form1.submit()
			}		
			
			function GetDate(vIn)  {
				eval(vIn).popup()
			}		
			
			function ClientSelectionChanged()  {
				form1.hdAction.value = "ClientSelectionChanged"
				form1.submit()
			}				
			
			</script>
		</form>
		<asp:literal id="litCalendar" runat="server" enableviewstate="False"></asp:literal></TD>
	</body>
</html>
