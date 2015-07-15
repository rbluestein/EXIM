<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ErrorPage.aspx.vb" Inherits="ImportExportManagement.ErrorPage" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
	<head>
		<title runat="server" id="PageCaption"></title>
		<meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" content="Visual Basic .NET 7.1">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<link title="BVIStyle" href="BVI.css" type="text/css" rel="stylesheet">
	</head>
	<body ms_positioning="GridLayout">
		<form id="Form1" method="post" runat="server">
			<table class="PrimaryTbl" style="Z-INDEX: 100; LEFT: 0px; POSITION: absolute; TOP: 0px"
				cellspacing="0" cellpadding="0" width="650" border="0">
				<tr>
					<td class="PrimaryTblTitle">An error has occurred:</td>
				</tr>
				<tr>
					<td>&nbsp;</td>
				</tr>
				<tr>
					<td class="Cell9Reg">
						<asp:literal id="litError" runat="server" enableviewstate="false"></asp:literal></td>
				</tr>
				<tr>
					<td></td>
				</tr>
			</table>
			<asp:literal id="litEnviro" runat="server" enableviewstate="False"></asp:literal>
		</form>
	</body>
</html>
