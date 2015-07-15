<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Feedlist.aspx.vb" Inherits="ImportExportManagement.Feedlist" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
	<head>
		<title runat="server" id="PageCaption"></title>
		<meta http-equiv="Content-Type" content="text/html; charset=windows-1252">
		<meta content="Microsoft Visual Studio.NET 7.0" name="GENERATOR">
		<meta content="Visual Basic 7.0" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<link title="BVIStyle" href="BVI.css" type="text/css" rel="stylesheet">
		<link href="menu.css" rel="stylesheet">
		<script language="JavaScript" src="menu.js"></script>
		<script language="JavaScript" src="menu_items.js"></script>
		<script language="JavaScript" src="menu_tpl.js"></script>
	</head>
	<body>
		<form id="form1" name="form1" action="Feedlist.aspx" method="post" runat="server">
			<input type="hidden" name="hdAction"> <input type="hidden" name="hdSortField"> <input type='hidden' name='hdFilterShowHideToggle' id='hdFilterShowHideToggle' value='0'>
			<input type="hidden" name="hdProcessTypeID">
			<asp:literal id="litHiddens" runat="server" enableviewstate="False"></asp:literal>
			<table class="PrimaryTbl" style="LEFT: 140px; POSITION: absolute; TOP: 14px" cellspacing="0"
				cellpadding="0" width="650" border="0">
				<tr>
					<td class="PrimaryTblTitle">
						<asp:literal id="litHeading" runat="server"></asp:literal>
					</td>
				</tr>
				<tr>
					<td class="CellSeparator"></td>
				</tr>
				<tr>
					<td><asp:literal id="litDG" runat="server" enableviewstate="False"></asp:literal></td>
				</tr>
			</table>
			<asp:label id="lblCurrentRights" runat="server"></asp:label>
			<asp:literal id="litEnviro" runat="server" enableviewstate="False"></asp:literal>
			<script language='javascript'>	
				document.write("<table style='background:#eeeedd;PADDING-LEFT: 4px;FONT: 8pt Arial, Helvetica, sans-serif; POSITION: absolute;TOP: 14px' cellSpacing='0' cellPadding='0' width='125' border='0'><tr><td width='20'>User:</td><td>" + form1.hdLoggedInUserID.value + "</td></tr><tr><td>Site:</td><td WRAP=HARD>" + form1.hdDBHost.value + "</td></tr></table>")					
				new menuitems("feedlist1", form1.currentrights.value);
				new tpl("v", "admin");
				new menu (MENU_ITEMS, MENU_TPL);				
			</script>
			<asp:literal id="litMsg" runat="server" enableviewstate="False"></asp:literal><asp:literal id="litFilterHiddens" runat="server" enableviewstate="False"></asp:literal>
			<script language="javascript"> 
			function Sort(vField) {
				form1.hdAction.value = "Sort"
				form1.hdSortField.value = vField
				form1.submit()
			}
	
			function ToggleShowFilter()  {
				form1.hdFilterShowHideToggle.value = 1
				form1.hdAction.value = "ApplyFilter"
				form1.submit()
			}				
	
			function ApplyFilter()
			{		
				form1.hdAction.value = "ApplyFilter"
				form1.submit()				
			}
			
			function Update(vUserID)
			{
				form1.hdAction.value = "Update"
				form1.hdUserID.value = vUserID
				form1.submit()
			}	
			
			function NewImport() {
				form1.hdAction.value = "NewImport"
				form1.hdProcessTypeID.value = "Import"
				form1.submit()
			}			
			
			function NewExport() {
				form1.hdAction.value = "NewExport"
				form1.hdProcessTypeID.value = "Export"
				form1.submit()
			}													 						
						
			function ExistingFeed(vActiveClientID, vProcessName, vDestinationID, vProcessTypeID)
			{
				form1.hdAction.value = "ExistingFeed"
				form1.hdActiveClientID.value = vActiveClientID
				form1.hdProcessName.value = vProcessName
				form1.hdDestinationID.value = vDestinationID
				form1.hdProcessTypeID.value = vProcessTypeID
				form1.submit()
			}
				
			function NewSchedule(vClientID, vProcessName, vDestinationID, vProcessTypeID) {	
			form1.hdAction.value = "NewSchedule"
			form1.hdActiveClientID.value = vClientID
			form1.hdProcessName.value = vProcessName
			form1.hdDestinationID.value = vDestinationID
			form1.hdProcessTypeID.value = vProcessTypeID
			form1.submit()
			}	
			
			function ExistingSchedule(vClientID, vProcessName, vDestinationID, vStartDate, vProcessTypeID) {
				form1.hdAction.value = "ExistingSchedule"
				form1.hdActiveClientID.value = vClientID
				form1.hdProcessName.value = vProcessName
				form1.hdDestinationID.value = vDestinationID
				form1.hdStartDate.value = vStartDate
				form1.hdProcessTypeID.value = vProcessTypeID
				form1.submit()
			}							
			
			function SubmitOnEnterKey(e) {
				var keypressevent = e ? e : window.event
				if (keypressevent.keyCode == 13) {	
					form1.hdAction.value = "ApplyFilter"						 	
					form1.submit()
				}			
			}		
			function ReturnToParentPage()  {
				form1.hdAction.value = "return"
				form1.submit()
			}	
			
			function ShowHideSubTable(vSubTableInd, vSubTableClientID, vSubTableProcessName, vSubTableDestinationID)  {		
				form1.hdSubTableInd.value = vSubTableInd
				form1.hdSubTableClientID.value = vSubTableClientID
				form1.hdSubTableProcessName.value = vSubTableProcessName
				form1.hdSubTableDestinationID.value = vSubTableDestinationID		
				form1.hdAction.value = "ShowHideSubTable"		
				form1.submit()
			}				
	
	/*	
			function ShowHideSubTable(vSubTableInd, vSubTableState, vLicenseNumber, vEffectiveDate)  {
				form1.hdSubTableInd.value = vSubTableInd
				form1.hdSubTableState.value = vSubTableState
				form1.hdLicenseNumber.value = vLicenseNumber
				form1.hdEffectiveDate.value = vEffectiveDate
				form1.hdAction.value = "ShowHideSubTable"
				form1.submit()
			}	
	
	*/		
			function DeleteFeed(vActiveClientID, vProcessName, vDestinationID)  {
				var OKToDelete = confirm("Are you sure you wish to delete this feed?");
				if (OKToDelete == true) {
					form1.hdAction.value = "DeleteFeed"
					form1.hdActiveClientID.value = vActiveClientID
					form1.hdProcessName.value = vProcessName
					form1.hdDestinationID.value = vDestinationID					
					form1.submit()		
				}		
			}
			
			function DeleteSchedule(vActiveClientID, vProcessName, vDestinationID, vStartDate)  {
				var OKToDelete = confirm("Are you sure you wish to delete this schedule?");
				if (OKToDelete == true) {
					form1.hdAction.value = "DeleteSchedule"
					form1.hdActiveClientID.value = vActiveClientID
					form1.hdProcessName.value = vProcessName
					form1.hdDestinationID.value = vDestinationID			
					form1.hdStartDate.value = vStartDate				
					form1.submit()		
				}		
			}								
																									
			</script>
		</form>
	</body>
</html>
