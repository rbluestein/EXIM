<%@ Page Language="vb" AutoEventWireup="false" Codebehind="DayPicker.aspx.vb" Inherits="ImportExportManagement.DayPicker" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
  <head>
    <title>Day Picker</title>
    <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" content="Visual Basic .NET 7.1">
    <meta name=vs_defaultClientScript content="JavaScript">
    <meta name=vs_targetSchema content="http://schemas.microsoft.com/intellisense/ie5">
		<style type="text/css">
<!--
					#table1 {
							border:1px solid #000;
					}
					#table1 td {
							background-color:#ccc;
							border:1px solid #000;
					}
					a#link {
							display:block;
							width:100%;
							height:100%;
							font-family:verdana,arial,helvetica,sans-serif;
							font-size:16px;
							color:#666;
							text-decoration:none;
					}
					a#link #span1 {
							display:block;
							padding:10px;
							text-decoration:underline;
					}
					a#link #span2 {
							display:block;
							padding:10px;
					}
					a#link:hover {
							background-color:#666;
							color:#ccc;
					}
//-->
		</style>    
  </head>
  <body MS_POSITIONING="GridLayout">

    <form id="form1" method="post" runat="server">
		<script language='javascript'>
				var re_id = new RegExp('id=(\\d+)');
				var num_id = (re_id.exec(String(window.location))  ? new Number(RegExp.$1) : 0);		
    			var obj_caller = (window.opener ? window.opener.daypickers[num_id] : null);
		</script>
    
    <table id='tbl'>
		<tr id='YesRow'>
			<td bgcolor=#33ff66><a style="text-decoration:none;color:black;font-family:arial;" href="javascript:fnSelect(0)">Sun</a></td>
			<td bgcolor=#33ff66><a style="text-decoration:none;color:black;font-family:arial;"  href="javascript:fnSelect(1)">Mon</a></td>
			<td bgcolor=#33ff66><a style="text-decoration:none;color:black;font-family:arial;"  href="javascript:fnSelect(2)">Tue</a></td>
			<td bgcolor=#33ff66><a style="text-decoration:none;color:black;font-family:arial;"  href="javascript:fnSelect(3)">Wed</a></td>
			<td bgcolor=#33ff66><a style="text-decoration:none;color:black;font-family:arial;"  href="javascript:fnSelect(4)">Thu</a></td>
			<td  bgcolor=#33ff66><a style="text-decoration:none;color:black;font-family:arial;"  href="javascript:fnSelect(5)">Fri</a></td>
			<td bgcolor=#33ff66><a style="text-decoration:none;color:black;font-family:arial;"  href="javascript:fnSelect(6)">Sat</a></td>
			<td bgcolor=#33ff66><a style="text-decoration:none;color:black;font-family:arial;"  href="javascript:fnSelect(7)">UponReceipt</a></td>												
		</tr>	
    </table>
    <br>
    <table border=1px cellpadding=0 cellspacing=0>
		<tr><td  style="BORDER-COLLAPSE: collapse;border-color:black;" bgcolor=#cccccc><a style="text-decoration:none;color:black;font-family:arial;" href="javascript:fnClose()">Done</a></td>
		</tr>
    </table>
    <script language='javascript'>
		var ARR_DAYS = [0, 0, 0, 0, 0, 0, 0, 0]
	
		if (obj_caller.target.value.length > 0) {
			if (obj_caller.target.value.indexOf('Sun') > -1) {
				ARR_DAYS[0] = 1
			}
			if (obj_caller.target.value.indexOf('Mon') > -1) {
				ARR_DAYS[1] = 1
			}	
			if (obj_caller.target.value.indexOf('Tue') > -1) {
				ARR_DAYS[2] = 1
			}
			if (obj_caller.target.value.indexOf('Wed') > -1) {
				ARR_DAYS[3] = 1
			}	
			if (obj_caller.target.value.indexOf('Thu') > -1) {
				ARR_DAYS[4] = 1
			}
			if (obj_caller.target.value.indexOf('Fri') > -1) {
				ARR_DAYS[5] = 1
			}	
			if (obj_caller.target.value.indexOf('Sat') > -1) {
				ARR_DAYS[6] = 1
			}
			if (obj_caller.target.value.indexOf('Upon Receipt') > -1) {
				ARR_DAYS[7] = 1
			}																					
		}	
	

		var mybody = document.getElementsByTagName("body") [0];
		var myform = document.getElementById("form1")
		var mytable = document.getElementById("tbl");
		var myYesRow = document.getElementById("YesRow");
		var myNoRow = document.getElementById("NoRow");		

		var myYesCells = myYesRow.getElementsByTagName("td");
		for (i=0;i<myYesCells.length;i++)  {
			if (ARR_DAYS[i] == 0)  {
				myYesCells[i].style.background = "#ff3366"	
			} else {
				myYesCells[i].style.background = "#33cc33"	
			}
		}				
		
		function fnSelect(vIn)  {
			if (ARR_DAYS[vIn] == 0) {
				ARR_DAYS[vIn] = 1
			}  else {
				ARR_DAYS[vIn] = 0	
			}
		
			for (i=0;i<myYesCells.length;i++)  {
				if (ARR_DAYS[i] == 0)  {
					myYesCells[i].style.background = "#ff3366"	
				} else {
					myYesCells[i].style.background = "#33cc33"	
				}
			}	
		}
		
		function fnClose()  {
		
			var NAMES = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Upon Receipt"];	
			obj_caller.target.value = ""
		
			for (i=0;i<ARR_DAYS.length;i++)  {
				if (ARR_DAYS[i] == 1) {
					obj_caller.target.value = obj_caller.target.value + NAMES[i] + ', '
				}
			}
			if (obj_caller.target.value.length > 0) {
				obj_caller.target.value = obj_caller.target.value.substring(0, obj_caller.target.value.length - 2)
			}
			window.close();
		}		
	    </script>
    </form>
  </body>
</html>
