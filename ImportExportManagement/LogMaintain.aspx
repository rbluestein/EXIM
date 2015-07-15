<%@ Page Language="vb" AutoEventWireup="false" Codebehind="LogMaintain.aspx.vb" Inherits="ImportExportManagement.LogMaintain" ValidateRequest=False%>
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
		<form id="form1" name="form1" action="LogMaintain.aspx" method="post" runat="server">
			<input type="hidden" name="hdAction"><input type="hidden" name="hdSubAction">
			<table class="PrimaryTbl" style="LEFT: 140px; POSITION: absolute; TOP: 14px" cellspacing="0"
				cellpadding="0" width="650" border="0">
				<tbody>
					<tr style="DISPLAY: none">
						<td width="120">&nbsp;</td>
						<td>&nbsp;</td>
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
						<td colspan="2">&nbsp;</td>
					</tr>
					<tr>
						<td class="Cell9Reg" align="left" width="120">Client:</td>
						<td><asp:textbox id="txtClientID" runat="server" maxlength="50" width="250px"></asp:textbox></td>
					</tr>
					<tr>
						<td class="Cell9Reg" align="left" width="120">Process Type:</td>
						<td><asp:textbox id="txtProcessTypeID" runat="server" maxlength="50" width="250px"></asp:textbox></td>
					</tr>
					<tr>
						<td class="Cell9Reg" align="left" width="120">File ID:</td>
						<td><asp:textbox id="txtFileID" runat="server" maxlength="50" width="250px"></asp:textbox></td>
					</tr>
					<tr>
						<td class="Cell9Reg" align="left" width="120">rowguid:</td>
						<td><asp:textbox id="txtrowguid" runat="server" maxlength="50" width="250px"></asp:textbox></td>
					</tr>
					<tr>
						<td colspan="2">&nbsp;</td>
					</tr>
					<tr>
						<td class="Cell9Reg" align="left" width="120">File Posted Date:</td>
						<td class="Cell9Reg"><asp:textbox id="txtFilePostedDate" runat="server" maxlength="50" width="112px"></asp:textbox>&nbsp;&nbsp;
							<asp:label id="lblFilePostedDateLink" runat="server">
								<a href="javascript:GetDate('FilePostedDate')">Get Date</a></asp:label></td>
					</tr>
					<tr>
						<td class="Cell9Reg" align="left" width="120">File Posted By:</td>
						<td class="Cell9Reg"><asp:dropdownlist id="ddFilePostedBy" runat="server"></asp:dropdownlist><asp:textbox id="txtFilePostedBy" runat="server" maxlength="50" width="250px"></asp:textbox></td>
					</tr>
					<tr>
						<td class="Cell9Reg" align="left" width="120">Receipt Recd Date:</td>
						<td class="Cell9Reg"><asp:textbox id="txtFileReceiptReceivedDate" runat="server" maxlength="50" width="112px"></asp:textbox>&nbsp;&nbsp;
							<asp:label id="lblFileReceiptReceivedDateLink" runat="server">
								<a href="javascript:GetDate('FileReceiptReceivedDate')">Get Date</a></asp:label></td>
					</tr>
					<tr>
						<td class="Cell9Reg" align="left" width="120">Receipt&nbsp;Recd By:</td>
						<td class="Cell9Reg"><asp:dropdownlist id="ddFileReceiptReceivedBy" runat="server"></asp:dropdownlist><asp:textbox id="txtFileReceiptReceivedBy" runat="server" maxlength="50" width="250px"></asp:textbox></td>
					</tr>
					<tr>
						<td class="Cell9Reg" align="left" width="120">Error Rpt Done Date:</td>
						<td class="Cell9Reg"><asp:textbox id="txtErrorReportDoneDate" runat="server" maxlength="50" width="112px"></asp:textbox>&nbsp;&nbsp;
							<asp:label id="lblErrorReportDoneDateLink" runat="server">
								<a href="javascript:GetDate('ErrorReportDoneDate')">Get Date</a></asp:label></td>
					</tr>
					<tr>
						<td class="Cell9Reg" align="left" width="120">Error Rpt Done By:</td>
						<td class="Cell9Reg"><asp:dropdownlist id="ddErrorReportDoneBy" runat="server"></asp:dropdownlist><asp:textbox id="txtErrorReportDoneBy" runat="server" maxlength="50" width="250px"></asp:textbox></td>
					</tr>
					<tr>
						<td colspan="2">&nbsp;</td>
					</tr>
					<asp:placeholder id="ExportPanel" runat="server">
						<tr>
							<td class="Cell9Reg" align="left" width="120">File Send Date:</td>
							<td class="Cell9Reg">
								<asp:textbox id="txtExpFileSendDate" runat="server" width="250px" maxlength="50"></asp:textbox></td>
						</tr>
						<tr>
							<td class="Cell9Reg" align="left">Destination:</td>
							<td class="Cell9Reg">
								<asp:textbox id="txtExpDestinationCode" runat="server" width="250px" maxlength="50"></asp:textbox></td>
						</tr>
						<tr>
							<td class="Cell9Reg" align="left">File Name:</td>
							<td class="Cell9Reg">
								<asp:textbox id="txtExpFileName" runat="server" width="250px" maxlength="50"></asp:textbox></td>
						</tr>
						<tr>
							<td class="Cell9Reg" align="left">File Location:</td>
							<td class="Cell9Reg">
								<asp:textbox id="txtExpFileLocation" runat="server" width="504px" maxlength="50"></asp:textbox></td>
						</tr>
						<tr>
							<td class="Cell9Reg" align="left">Complete Datetime:</td>
							<td class="Cell9Reg">
								<asp:textbox id="txtExpCompleteDatetime" runat="server" width="250px" maxlength="50"></asp:textbox></td>
						</tr>
						<tr>
							<td class="Cell9Reg" align="left">Extract Date From:</td>
							<td class="Cell9Reg">
								<asp:textbox id="txtExpExtractDateFrom" runat="server" width="250px" maxlength="50"></asp:textbox></td>
						</tr>
						<tr>
							<td class="Cell9Reg" align="left">Extract Date To:</td>
							<td class="Cell9Reg">
								<asp:textbox id="txtExpExtractDateTo" runat="server" width="250px" maxlength="50"></asp:textbox></td>
						</tr>
					</asp:placeholder>
					<asp:placeholder id="ImportPanel" runat="server">
						<tr>
							<td class="Cell9Reg" align="left" width="120">File Receive Date:</td>
							<td class="Cell9Reg">
								<asp:textbox id="txtImpFileReceiveDate" runat="server" width="250px" maxlength="50"></asp:textbox></td>
						</tr>
						<tr>
							<td class="Cell9Reg" align="left">File Type Code:</td>
							<td class="Cell9Reg">
								<asp:textbox id="txtImpFileTypeCode" runat="server" width="250px" maxlength="50"></asp:textbox></td>
						</tr>
						<tr>
							<td class="Cell9Reg" align="left">File Location:</td>
							<td class="Cell9Reg">
								<asp:textbox id="txtImpFileLocation" runat="server" width="504px" maxlength="50"></asp:textbox></td>
						</tr>
						<tr>
							<td class="Cell9Reg" align="left">File Import Date:</td>
							<td class="Cell9Reg">
								<asp:textbox id="txtImpFileImportDate" runat="server" width="250px" maxlength="50"></asp:textbox></td>
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
				</tbody>
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
			<asp:literal id="litServerTime" runat="server" enableviewstate="False"></asp:literal>
			<script type="text/javascript">
				function Update()  {
					form1.hdAction.value = "update"
					form1.submit()
				}
				
				function ReturnToParentPage()  {
					form1.hdAction.value = "return"
					form1.submit()
				}
				
			function ChangeYear(vIn)  {
				form1.hdAction.value = vIn
				form1.submit()
			}
			
			function GetDate(vIn)  {
				//cal5.popup()
				eval(vIn).popup()
				//form1.hdAction.value = "GetDate"			
				//form1.hdSubAction.value = vIn
				//form1.submit()
			}											
			
			</script>
		</form>
		<script language="javascript">
			//var cal5 = new calendar2(document.forms['form1'].elements['txtFilePostedDate']); 
			//cal5.year_scroll = true; cal5.time_comp = false;
		
			var FilePostedDate = new calendar2(document.forms['form1'].elements['txtFilePostedDate']); 
			FilePostedDate.year_scroll = true; FilePostedDate.time_comp = true;
			
			var FileReceiptReceivedDate = new calendar2(document.forms['form1'].elements['txtFileReceiptReceivedDate']); 
			FileReceiptReceivedDate.year_scroll = true; FileReceiptReceivedDate.time_comp = false;
			
			var ErrorReportDoneDate = new calendar2(document.forms['form1'].elements['txtErrorReportDoneDate']); 
			ErrorReportDoneDate.year_scroll = true; ErrorReportDoneDate.time_comp = false;									
			
		</script>
	</body>
</html>
