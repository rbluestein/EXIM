<%@ Page Language="vb" AutoEventWireup="false" Codebehind="TimePicker.aspx.vb" Inherits="ImportExportManagement.TimePicker"%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
  <head>
    <title>Time Picker</title>
    <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" content="Visual Basic .NET 7.1">
    <meta name=vs_defaultClientScript content="JavaScript">
    <meta name=vs_targetSchema content="http://schemas.microsoft.com/intellisense/ie5">   
  </head>
  <body MS_POSITIONING="GridLayout">

    <form id="form1" method="post" runat="server">
		<script language='javascript'>
				var re_id = new RegExp('id=(\\d+)');
				var num_id = (re_id.exec(String(window.location))  ? new Number(RegExp.$1) : 0);		
    			var obj_caller = (window.opener ? window.opener.timepickers[num_id] : null);
		</script>
    
    <table id='tbl'>
		<tr id='AMRow'>
			<td bgcolor=#33ff66><a style="text-decoration:none;color:black;font-family:arial;" href="javascript:fnSelect(0)">mid</a></td>
			<td bgcolor=#33ff66><a style="text-decoration:none;color:black;font-family:arial;"  href="javascript:fnSelect(1)">1am</a></td>
			<td bgcolor=#33ff66><a style="text-decoration:none;color:black;font-family:arial;"  href="javascript:fnSelect(2)">2am</a></td>
			<td bgcolor=#33ff66><a style="text-decoration:none;color:black;font-family:arial;"  href="javascript:fnSelect(3)">3am</a></td>
			<td bgcolor=#33ff66><a style="text-decoration:none;color:black;font-family:arial;"  href="javascript:fnSelect(4)">4am</a></td>
			<td  bgcolor=#33ff66><a style="text-decoration:none;color:black;font-family:arial;"  href="javascript:fnSelect(5)">5am</a></td>
			<td bgcolor=#33ff66><a style="text-decoration:none;color:black;font-family:arial;"  href="javascript:fnSelect(6)">6am</a></td>
			<td bgcolor=#33ff66><a style="text-decoration:none;color:black;font-family:arial;"  href="javascript:fnSelect(7)">7am</a></td>
			<td bgcolor=#33ff66><a style="text-decoration:none;color:black;font-family:arial;"  href="javascript:fnSelect(8)">8am</a></td>
			<td bgcolor=#33ff66><a style="text-decoration:none;color:black;font-family:arial;"  href="javascript:fnSelect(9)">9am</a></td>
			<td  bgcolor=#33ff66><a style="text-decoration:none;color:black;font-family:arial;"  href="javascript:fnSelect(10)">10am</a></td>
			<td bgcolor=#33ff66><a style="text-decoration:none;color:black;font-family:arial;"  href="javascript:fnSelect(11)">11am</a></td>												
		</tr>	
		<tr id='PMRow'>
			<td bgcolor=#33ff66><a style="text-decoration:none;color:black;font-family:arial;" href="javascript:fnSelect(12)">noon</a></td>
			<td bgcolor=#33ff66><a style="text-decoration:none;color:black;font-family:arial;"  href="javascript:fnSelect(13)">1pm</a></td>
			<td bgcolor=#33ff66><a style="text-decoration:none;color:black;font-family:arial;"  href="javascript:fnSelect(14)">2pm</a></td>
			<td bgcolor=#33ff66><a style="text-decoration:none;color:black;font-family:arial;"  href="javascript:fnSelect(15)">3pm</a></td>
			<td bgcolor=#33ff66><a style="text-decoration:none;color:black;font-family:arial;"  href="javascript:fnSelect(16)">4pm</a></td>
			<td  bgcolor=#33ff66><a style="text-decoration:none;color:black;font-family:arial;"  href="javascript:fnSelect(17)">5pm</a></td>
			<td bgcolor=#33ff66><a style="text-decoration:none;color:black;font-family:arial;"  href="javascript:fnSelect(18)">6pm</a></td>
			<td bgcolor=#33ff66><a style="text-decoration:none;color:black;font-family:arial;"  href="javascript:fnSelect(19)">7pm</a></td>
			<td bgcolor=#33ff66><a style="text-decoration:none;color:black;font-family:arial;"  href="javascript:fnSelect(20)">8pm</a></td>
			<td bgcolor=#33ff66><a style="text-decoration:none;color:black;font-family:arial;"  href="javascript:fnSelect(21)">9pm</a></td>
			<td  bgcolor=#33ff66><a style="text-decoration:none;color:black;font-family:arial;"  href="javascript:fnSelect(22)">10pm</a></td>
			<td bgcolor=#33ff66><a style="text-decoration:none;color:black;font-family:arial;"  href="javascript:fnSelect(23)">11pm</a></td>												
		</tr>			
    </table>
    <br>
    <table border=1px cellpadding=0 cellspacing=0>
		<tr><td  style="BORDER-COLLAPSE: collapse;border-color:black;" bgcolor=#cccccc><a style="text-decoration:none;color:black;font-family:arial;" href="javascript:fnClose()">Done</a></td>
		</tr>
    </table>
    <script language='javascript'>
		var TimeIdx = -1;
		var ARR_TIMES = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];

		if (obj_caller.target.value.length > 0) {
			if (obj_caller.target.value.indexOf('Midnight') > -1) {
				TimeIdx = 0		
			}
			if (obj_caller.target.value.indexOf('1am') > -1) {
				TimeIdx = 1			
			}	
			if (obj_caller.target.value.indexOf('2am') > -1) {
				TimeIdx = 2				
			}
			if (obj_caller.target.value.indexOf('3am') > -1) {
				TimeIdx = 3				
			}	
			if (obj_caller.target.value.indexOf('4am') > -1) {
				TimeIdx = 4				
			}
			if (obj_caller.target.value.indexOf('5am') > -1) {
				TimeIdx = 5				
			}	
			if (obj_caller.target.value.indexOf('6am') > -1) {
				TimeIdx = 6				
			}		
			if (obj_caller.target.value.indexOf('7am') > -1) {
				TimeIdx = 7				
			}	
			if (obj_caller.target.value.indexOf('8am') > -1) {
				TimeIdx = 8			
			}
			if (obj_caller.target.value.indexOf('9am') > -1) {
				TimeIdx = 9				
			}	
			if (obj_caller.target.value.indexOf('10am')  > -1) {
				TimeIdx = 10				
			}
			if (obj_caller.target.value.indexOf('11am') > -1) {
				TimeIdx = 11				
			}	
			if (obj_caller.target.value.indexOf('Noon') > -1) {
				TimeIdx = 12							
			}	
			if (obj_caller.target.value.indexOf('1pm') == 0) {
				TimeIdx = 13				
			}	
			if (obj_caller.target.value.indexOf('2pm') > -1) {
				TimeIdx = 14				
			}
			if (obj_caller.target.value.indexOf('3pm') > -1) {
				TimeIdx = 15				
			}		
			if (obj_caller.target.value.indexOf('4pm') > -1) {
				TimeIdx = 16				
			}
			if (obj_caller.target.value.indexOf('5pm') > -1) {
				TimeIdx = 17				
			}	
			if (obj_caller.target.value.indexOf('6pm') > -1) {
				TimeIdx = 18				
			}
			if (obj_caller.target.value.indexOf('7pm') > -1) {
				TimeIdx = 19				
			}	
			if (obj_caller.target.value.indexOf('8pm') > -1) {
				TimeIdx = 20				
			}
			if (obj_caller.target.value.indexOf('9pm') > -1) {
				TimeIdx = 21				
			}	
			if (obj_caller.target.value.indexOf('10pm') > -1) {
				TimeIdx = 22							
			}
			if (obj_caller.target.value.indexOf('11pm') > -1) {
				TimeIdx = 23		
			}																								
		}	
		
		ARR_TIMES[TimeIdx] = 1

		var mybody = document.getElementsByTagName("body") [0];
		var myform = document.getElementById("form1")
		var mytable = document.getElementById("tbl");
		var myAMRow = document.getElementById("AMRow");
		var myPMRow = document.getElementById("PMRow");		
		var myAMCells = myAMRow.getElementsByTagName("td");
		var myPMCells = myPMRow.getElementsByTagName("td");

		SetBackground();


		function SetBackground()  {
		
			for (i=0;i<12;i++)  {
				if (ARR_TIMES[i] == 0)  {
					myAMCells[i].style.background = "#ff3366"	
				} else {
					myAMCells[i].style.background = "#33cc33"
				}
			}
			
			for (i=0;i<12;i++)  {
				if (ARR_TIMES[i+12] == 0)  {
					myPMCells[i].style.background = "#ff3366"	
				} else {
					myPMCells[i].style.background = "#33cc33"
				}
			}						
		
		}
		
		function fnSelect(vIn)  {
			for (i=0;i<24;i++)  {
				ARR_TIMES[i] = 0
			}
			ARR_TIMES[vIn] = 1
			TimeIdx = vIn
			SetBackground();
		}
		
		function fnClose()  {
			var ReturnValue;
			
			if (TimeIdx == -1)  {
				ReturnValue = "";
			}
			else if (TimeIdx == 0) 	{
				ReturnValue = "Midnight";
			}
			else if (TimeIdx == 12)  {
				ReturnValue = "Noon";
			}
			else if (TimeIdx < 12)  {
				ReturnValue = TimeIdx + 'am'
			}
			else {
				ReturnValue = TimeIdx - 12 + 'pm'
			}
				
			obj_caller.target.value = ReturnValue;	
			window.close();
		}		
	    </script>
    </form>
  </body>
</html>
