<%@ Page Language="vb" AutoEventWireup="false" Codebehind="LogWorklist.aspx.vb" Inherits="ImportExportManagement.LogWorklist"%>
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
		<script language="JavaScript" src="MonthPicker.js"></script>
	</head>
	<body onload="fnLoadProcessTypeID()">
		<form id="form1" name="form1" action="LogWorklist.aspx" method="post" runat="server">
			<input type="hidden" name="hdAction"> <input type="hidden" name="hdSortField"> <input type='hidden' name='hdFilterShowHideToggle' id='hdFilterShowHideToggle' value='0'>
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
			<asp:literal id="litHiddens" runat="server" enableviewstate="False"></asp:literal><asp:label id="lblCurrentRights" runat="server"></asp:label>
			<asp:literal id="litEnviro" runat="server" enableviewstate="False"></asp:literal>
			<script language="javascript">			
				document.write("<table style='background:#eeeedd;PADDING-LEFT: 4px;FONT: 8pt Arial, Helvetica, sans-serif; POSITION: absolute;TOP: 14px' cellSpacing='0' cellPadding='0' width='125' border='0'><tr><td width='20'>User:</td><td>" + form1.hdLoggedInUserID.value + "</td></tr><tr><td>Site:</td><td WRAP=HARD>" + form1.hdDBHost.value + "</td></tr></table>")			
				new menuitems("feedlist1", form1.currentrights.value);
				new tpl("v", "admin");
				new menu (MENU_ITEMS, MENU_TPL);				
			</script>
			<asp:literal id="litMsg" runat="server" enableviewstate="False"></asp:literal><asp:literal id="litFilterHiddens" runat="server" enableviewstate="False"></asp:literal>
			<script language="javascript"> 
					function ApplyFilter()
					{
						oListbox = document.getElementById("ddClientID")		
						if (oListbox.selectedIndex == 0) {
							alert("Selection required.")
						} else {	
							form1.hdAction.value = "ApplyFilter"
							form1.hdActiveClientValue.value =  form1.ddClientID.value
		     				form1.hdActiveProcessTypeID.value = form1.ddProcessTypeID.value
							form1.submit()
						}
					}

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
			
					function Update(vUserID)
					{
						form1.hdAction.value = "Update"
						form1.hdUserID.value = vUserID
						form1.submit()
					}
					
					function LogMaintain(vFileID) {
						form1.hdAction.value = "LogMaintain"
						form1.hdFileID.value = vFileID
						form1.submit()
					}					
			
					function fnLoadProcessTypeID()  {			
							PopulateProcessTypeID()		
							
							// Process type selected value 
							oProcessTypeID = document.getElementById("ddProcessTypeID")			
							for (var i = 0; i<oProcessTypeID.options.length; i++)  {
								if (oProcessTypeID.options[i].value == form1.hdProcessTypeIDSelectedFilterValue.value) {
									oProcessTypeID.selectedIndex = i
								}
							}							
					}			
					
					function ProcessTypeIDSelect() {
						//oListbox = document.getElementById("ddProcessTypeID")				
						//form1.hdProcessTypeIDSelectedFilterValue.value =  oListbox.getAttribute("value")
					}
																											
					function ClientSelect()  {	
						// Save the selected value, e.g., BureauVeritas|Import|Export
						//form1.hdClientIDSelectedFilterValue.value = form1.ddClientID.value
						PopulateProcessTypeID()			
						//form1.hdProcessTypeIDSelectedFilterValue.value =  oListbox.getAttribute("value")							
					}					
					
					function PopulateProcessTypeID() {
						temp = form1.ddClientID.value.split("|")
						oListbox = document.getElementById("ddProcessTypeID")
						ClearItems(oListbox)
						//AddItem(oListbox, "", "0")
						for (var i = 1; i<temp.length; i++)  {
							sName = temp[i]
							sValue = temp[i]	
							AddItem(oListbox, sName, sValue)							
						}				
					}		
					
					function ClearItems(oListbox) {
							for (var i = oListbox.options.length-1; i >=0 ; i--) {
								oListbox.remove(0)
							}			
					}										

					function AddItem(oListbox, sName, sValue) {
						var oOption = document.createElement("option")
						oOption.appendChild(document.createTextNode(sName))
						oOption.setAttribute("value", sValue)
						oListbox.appendChild(oOption)
					}		
												
					function GetDateRange()  {
							vIn = "MonthPicker"
							eval(vIn).popup()				
					}													
			</script>
		</form>
		<script language='javascript'>
					var MonthPicker = new MonthPicker(document.forms['form1'].elements['hdDateRange']); 
		</script>
	</body>
</html>
