<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ScheduleMaintain.aspx.vb" Inherits="ImportExportManagement.ScheduleMaintain"%>
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
		<script language="JavaScript" src="DayPicker.js"></script>
		<script language="JavaScript" src="TimePicker.js"></script>
	</head>
	<body>
		<form id="form1" name="form1" action="ScheduleMaintain.aspx" method="post" runat="server">
			<input type="hidden" name="hdAction"><input type='hidden' name='hdSubAction'>
			<asp:literal id="litHiddens" runat="server" enableviewstate="False"></asp:literal>
			<table class="PrimaryTbl" style="LEFT: 140px; POSITION: absolute; TOP: 14px" cellspacing="0"
				cellpadding="0" width="650" border="0">
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
				<tr valign="top">
					<td class="Cell9Reg" align="left" width="120">Client:</td>
					<td><asp:textbox id="txtClientID" runat="server" maxlength="50" width="250px"></asp:textbox></td>
				</tr>
				<tr>
					<td class="Cell9Reg" align="left">Process Name:</td>
					<td><asp:textbox id="txtProcessName" runat="server" maxlength="50" width="250px"></asp:textbox></td>
				</tr>
				<asp:placeholder id="phExport" runat="server" enableviewstate="False">
					<tr>
						<td class="Cell9Reg" align="left">Destination:</td>
						<td>
							<asp:textbox id="txtDestinationID" runat="server" width="250px" maxlength="50"></asp:textbox></td>
					</tr>
				</asp:placeholder>
				<tr>
					<td colspan="2">&nbsp;</td>
				</tr>
				<tr>
					<td class="Cell9Reg" align="left">Status:</td>
					<td><asp:radiobuttonlist id="rbActiveInd" style="FONT: 9pt Arial, Helvetica, sans-serif" runat="server" repeatdirection="Horizontal">
							<asp:listitem value="1">Active</asp:listitem>
							<asp:listitem value="0">Inactive</asp:listitem>
						</asp:radiobuttonlist><asp:textbox id="txtActiveInd" runat="server" maxlength="50" width="250px"></asp:textbox></td>
				</tr>
				<tr>
					<td class="Cell9Reg" style="WIDTH: 120px" align="left">Start Date:</td>
					<td class="Cell9Reg"><asp:textbox id="txtStartDate" runat="server" maxlength="50" width="112px" readonly="True"></asp:textbox>&nbsp;&nbsp;
						<asp:label id="lblStartDateLink" runat="server">
							<a href="javascript:GetDate('StartDate')">Get Date</a></asp:label></td>
				</tr>
				<tr>
					<td class="Cell9Reg" style="WIDTH: 120px" align="left" width="120">End Date:</td>
					<td class="Cell9Reg"><asp:textbox id="txtEndDate" runat="server" maxlength="50" width="112px" readonly="True"></asp:textbox>&nbsp;&nbsp;
						<asp:label id="lblEndDateLink" runat="server">
							<a href="javascript:GetDate('EndDate')">Get Date</a></asp:label></td>
				</tr>
				<tr>
					<td class="Cell9Reg" style="WIDTH: 120px" align="left">Day of Week:</td>
					<td><asp:textbox id="txtDayOfWeek" runat="server" maxlength="50"></asp:textbox>
						<asp:label id="lblDayOfWeekLink" runat="server">
							<a href="javascript:GetDay('DayPicker')">Get Day</a></asp:label>
					</td>
				</tr>
				<asp:placeholder id="plDayOfMonth" runat="server" visible="False">
					<tr>
						<td class="Cell9Reg" style="WIDTH: 120px" align="left">Day of Month:</td>
						<td>
							<asp:textbox id="txtDayOfMonth" runat="server" maxlength="50"></asp:textbox></td>
					</tr>
				</asp:placeholder>
				<tr>
					<td class="Cell9Reg" style="WIDTH: 120px" align="left">Date of Month:</td>
					<td>
						<asp:dropdownlist id="ddDateOfMonth" runat="server"></asp:dropdownlist><asp:textbox id="txtDateOfMonth" runat="server" maxlength="50"></asp:textbox></td>
				</tr>
				<asp:placeholder id="plTimeOfDay" runat="server" visible="False">
					<tr>
						<td class="Cell9Reg" style="WIDTH: 120px" align="left">Time of Day:</td>
						<td>
							<asp:textbox id="txtTimeOfDay" runat="server" maxlength="50"></asp:textbox>
							<asp:label id="lblTimeOfDayLink" runat="server">
								<a href="javascript:GetTime('TimePicker')">Get Time</a></asp:label></td>
					</tr>
				</asp:placeholder>
				<tr>
					<td class="Cell9Reg" style="WIDTH: 120px" align="left">Num Days for Receipt:</td>
					<td>
						<asp:textbox id="txtNumDaysForReceipt" runat="server" maxlength="3"></asp:textbox></td>
				</tr>
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
				eval(vIn).popup()
				//form1.hdAction.value = "GetDate"			
				//form1.hdSubAction.value = vIn
				//form1.submit()
			}		
			
			function ClearDate(vIn)  {
				form1.hdAction.value = "ClearDate"			
				form1.hdSubAction.value = vIn
				form1.submit()
			}	
			
			function GetDay(vIn) {
				eval(vIn).popup()
			}
			
			function GetTime(vIn) {
				eval(vIn).popup()
			}	
			
			function AllowNumericOnly(tb) {
				var ValidChars = "0123456789";
				var IsNumber=1
				var Char;
				for (i = 0; i < tb.value.length && IsNumber == 1; i++)    { 
					Char = tb.value.charAt(i); 
					if (ValidChars.indexOf(Char) == -1) {
						IsNumber = 0
					}
				}
					if (IsNumber == 0)  {
						tb.value = tb.value.substring(0, tb.value.length - 1)
					}
			}					
			
			</script>
		</form>
		<script language='javascript'>
			var StartDate = new calendar2(document.forms['form1'].elements['txtStartDate']); 
			StartDate.year_scroll = true; StartDate.time_comp = false;
				
			var EndDate = new calendar2(document.forms['form1'].elements['txtEndDate']); 
			EndDate.year_scroll = true; EndDate.time_comp = false;	
				
			var DayPicker = new DayPicker(document.forms['form1'].elements['txtDayOfWeek']); 
			
			var TimePicker = new TimePicker(document.forms['form1'].elements['txtTimeOfDay']); 			
		</script>
	</body>
</html>
