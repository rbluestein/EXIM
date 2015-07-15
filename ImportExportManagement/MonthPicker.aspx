<%@ Page Language="vb" AutoEventWireup="false" Codebehind="MonthPicker.aspx.vb" Inherits="ImportExportManagement.MonthPicker"%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<HTML>
	<HEAD>
		<title>WebForm1</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="Visual Basic .NET 7.1" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<style type="text/css">TABLE { BORDER-RIGHT: blue 1px solid; BORDER-TOP: blue 1px solid; BACKGROUND: #b0e0e6; FONT: 9pt Arial, Helvetica, sans-serif; BORDER-LEFT: blue 1px solid; BORDER-BOTTOM: blue 1px solid; BORDER-COLLAPSE: collapse }
	TD { BORDER-RIGHT: blue 1px solid; BORDER-TOP: blue 1px solid; BACKGROUND: #b0e0e6; FONT: 9pt Arial, Helvetica, sans-serif; BORDER-LEFT: blue 1px solid; BORDER-BOTTOM: blue 1px solid; BORDER-COLLAPSE: collapse }
	TD { VERTICAL-ALIGN: top; align: center }
	A { DISPLAY: block; WIDTH: 100%; COLOR: blue; FONT-FAMILY: 8pt Arial, Helvetica, sans-serif; HEIGHT: 100%; TEXT-DECORATION: none }
	A:hover { BACKGROUND: yellow }
	SPAN { COLOR: blue; FONT-FAMILY: 8pt Arial, Verdana, Helvetica, sans-serif }
		</style>
		
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="form1" method="post" runat="server">
			<input type='hidden' id='hdCurMonth'><input type='hidden' id='hdCurYear'><input type='hidden' id='hdInitial' value='1'>
			<input type='hidden' id='hdFromMonth'><input type='hidden' id='hdFromYear'><input type='hidden' id='hdToMonth'><input type='hidden' id='hdToYear'>
			<script language="JavaScript">
			
				var re_id = new RegExp('id=(\\d+)');
				var num_id = (re_id.exec(String(window.location))  ? new Number(RegExp.$1) : 0);
				var obj_caller = (window.opener ? window.opener.monthpickers[num_id] : null);
				
				function fnClose()  {
					fnSetSourceControl()
					window.close();
				}			

			
				var ARR_MONTHS = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];			
			
				if (form1.hdInitial.value == 1)  {	
					var ArrValues = obj_caller.target.value.split("|")						
					form1.hdFromMonth.value = ArrValues[0]			
					form1.hdFromYear.value = ArrValues[1]	
					form1.hdToMonth.value = ArrValues[2]	
					form1.hdToYear.value = ArrValues[3]												
					form1.hdInitial.value = "0"
				}			
											
				function fnSelectMonth(vSelect, vValue)  {
					fnAdjustYear(vSelect)			
					if (vSelect == 0)  {				
						form1.hdFromMonth.value = vValue
					} else {
						form1.hdToMonth.value = vValue				
					}			
					fnShowDate(vSelect)
					//fnSetSourceControl()
				}
							
				function fnSelectYear(vSelect, vValue)  {
					fnAdjustMonth(vSelect)				
					if (vSelect == 0) {	
						if (form1.hdFromYear.value == '')  {
							fnAdjustYear(0)			
						}  else {
							form1.hdFromYear.value = parseInt(form1.hdFromYear.value) + vValue										
						}				
					}	
					if (vSelect == 1) {	
						if (form1.hdToYear.value == '')  {
							fnAdjustYear(1)			
						}  else {
							form1.hdToYear.value = parseInt(form1.hdToYear.value) + vValue										
						}			
					}						
					fnShowDate(vSelect)					
					//fnSetSourceControl()			
				}		
				
				function fnShowDate(vSelect) 
				 {
					if (vSelect == 0) {
						if (form1.hdFromMonth.value == "" && form1.hdFromYear.value == "") {
							form1.txtFromMonthYear.value = ""
						} else {
							form1.txtFromMonthYear.value  = ARR_MONTHS[form1.hdFromMonth.value] + ' ' + form1.hdFromYear.value	
						}
					}
					
					if (vSelect == 1) {
						if (form1.hdToMonth.value == "" && form1.hdToYear.value == "") {
							form1.txtToMonthYear.value = ""
						} else {
							form1.txtToMonthYear.value  = ARR_MONTHS[form1.hdToMonth.value] + ' ' + form1.hdToYear.value	
						}
					}					
				
				}		
				
				function fnClear(vSelect)  {
					if (vSelect == 0)  {		
						form1.hdFromMonth.value = ''
						form1.hdFromYear.value = ''				
					} else {		
						form1.hdToMonth.value = ''
						form1.hdToYear.value = ''	
					}
					fnShowDate(vSelect)					
					//fnSetSourceControl()		
				}	
				
				function fnAdjustMonth(vSelect) 	{
					if (vSelect == 0  && form1.hdFromMonth.value == "")	{
						form1.hdFromMonth.value = 0
					} 
					if (vSelect == 1 && form1.hdToMonth.value == '')	{
						form1.hdToMonth.value = 0
					}												
				}
				
				function fnAdjustYear(vSelect)  {
					if (vSelect == 0 && form1.hdFromYear.value == '') {
							var d = new Date()
							form1.hdFromYear.value = d.getFullYear()
					}	
					if  (vSelect == 1 && form1.hdToYear.value == '') 	{
							var d = new Date()
							form1.hdToYear.value = d.getFullYear()				
					}			
				}			
																							
				function fnSetSourceControl() {
						obj_caller.target.value = form1.hdFromMonth.value + '|' + form1.hdFromYear.value + '|' + form1.hdToMonth.value + '|' + form1.hdToYear.value
				}	
				
					/////  FROM DATE  /////
				// Year menu
				document.write('<table>')
				document.write('<tr><td><a  href="javascript:fnSelectYear(0, -1)">Prev Year</a></td><td><a href="javascript:fnSelectYear(0, 1)">Next Year</a></td></tr>')
				document.write('</table>')				

				// Month menu
				var MonthNum
				document.write('<table>')
				for (var p=0; p<1;p++)  {
						document.write('<tr>')
						for (var n=0; n<12; n++)  {
								document.write('<td><a href="javascript:fnSelectMonth(0,' + n + ')">'+ ARR_MONTHS[n] + ' </a></td>');								
								MonthNum = MonthNum + 1
						}
						document.write('</tr></table>');
				}
				
				// Display selected month and year
				var ShowDate =  String(ARR_MONTHS[form1.hdFromMonth.value]) + '&nbsp;' + String(form1.hdFromYear.value)
				document.write('<table><tr><td><input type="text" id="txtFromMonthYear" readonly="readonly" style="border-style:None;border-Left:None;border-right:None;border-top:none;border-bottom:none;" value=' + ShowDate + '></td><table>')
				
				// Clear month and year selection
				document.write('<table<tr><td><a  href="javascript:fnClear(0)">Clear</a></td></tr></table>')
				document.write('<br>')

				
				/////  TO DATE  /////
				// Year menu
				document.write('<table>')
				document.write('<tr>')
				document.write('<td><a  href="javascript:fnSelectYear(1, -1)">Prev Year</a></td><td><a href="javascript:fnSelectYear(1, 1)">Next Year</a>')
				document.write('</td></tr>')
				document.write('</table>')				

				// Month menu
				var MonthNum
				document.write('<table>')
				for (var p=0; p<1;p++)  {
						document.write('<tr>')
						for (var n=0; n<12; n++)  {
								document.write('<td><a href="javascript:fnSelectMonth(1,' + n + ')">'+ ARR_MONTHS[n] + ' </a></td>');								
								MonthNum = MonthNum + 1
						}
						document.write('</tr></table>');
				}
				
				// Display selected month and year
				var ShowDate =  String(ARR_MONTHS[form1.hdFromMonth.value]) + '&nbsp;' + String(form1.hdFromYear.value)
				document.write('<table><tr><td><input type="text" id="txtToMonthYear" readonly="readonly" style="border-style:None;border-Left:None;border-right:None;border-top:none;border-bottom:none;" value=' + ShowDate + '></td></tr><table>')
				
			
				// Clear month and year selection
				document.write('<table<tr><td><a  href="javascript:fnClear(1)">Clear</a></td>')
				document.write('</tr></table>')	
				document.write('<br>')						
				
				// Close window
				document.write('<table><tr><td><a  href="javascript:fnClose()">Done</a></td></tr></table>')	
				
				fnShowDate(0)
				fnShowDate(1)							
				
			</script>
		</form>
	</body>
</HTML>
