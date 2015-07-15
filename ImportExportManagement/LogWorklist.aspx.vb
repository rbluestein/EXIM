Imports System.Data.SqlClient

Public Class LogWorklist
    Inherits System.Web.UI.Page

#Region " Declarations "
    Protected PageCaption As HtmlGenericControl
    Private Rights As RightsClass
    Protected WithEvents litDG As System.Web.UI.WebControls.Literal
    Protected WithEvents dgOrgList As System.Web.UI.WebControls.DataGrid
    Protected WithEvents litMsg As System.Web.UI.WebControls.Literal
    Protected WithEvents litFilterHiddens As System.Web.UI.WebControls.Literal
    Protected WithEvents litHeading As System.Web.UI.WebControls.Literal
    Protected WithEvents lblCurrentRights As System.Web.UI.WebControls.Label
    Protected WithEvents litHiddens As System.Web.UI.WebControls.Literal
    Protected WithEvents lblEnrollerName As System.Web.UI.WebControls.Label

    Private AppSession As New AppSession
    Private Common As Common
    'Dim cFileID As String
    'Dim cClientIDSelectedFilterValue As String
    'Dim cActiveClientValue As String = String.Empty
    'Dim cProcessTypeIDSelectedFilterValue As String
    'Dim cActiveProcessTypeID As String
    'Dim cActiveClientIDOnly As String
    'Dim cFilterOnResponse As String
    Dim cFilterShowHideToggle As String
    Protected WithEvents litEnviro As System.Web.UI.WebControls.Literal

    Private Sess As LogWorklistSession
#End Region

#Region " Web Form Designer Generated Code "

    'This call is required by the Web Form Designer.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: This method call is required by the Web Form Designer
        'Do not modify it using the code editor.
        InitializeComponent()
    End Sub

#End Region

    Private Function ServerTime() As String
        Dim sb As New System.Text.StringBuilder
        Dim HBGTime As DateTime

        HBGTime = Common.GetServerDateTime
        sb.Append("<script type='text/javascript'>" & vbCrLf)
        sb.Append("var ServerTime = new Array();" & vbCrLf)
        sb.Append("ServerTime[0] = '" & HBGTime.Year & "';" & vbCrLf)
        sb.Append("ServerTime[1] = '" & HBGTime.Month & "';" & vbCrLf)
        sb.Append("ServerTime[2] = '" & HBGTime.Day & "';" & vbCrLf)
        sb.Append("ServerTime[3] = '" & HBGTime.Hour & "';" & vbCrLf)
        sb.Append("ServerTime[4] = '" & HBGTime.Minute & "';" & vbCrLf)
        sb.Append("ServerTime[5] = '" & HBGTime.Second & "';" & vbCrLf)
        sb.Append("ServerTime[6] = '" & HBGTime.Millisecond & "';" & vbCrLf)
        sb.Append("</script>" & vbCrLf)

        'for (i=0;i<mycars.length;i++)
        '{
        'document.write(mycars[i] + "<br />");
        '}
        '</script>
    End Function

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim PageMode As PageMode
        Dim Results As Results
        Dim Action As String
        Dim DG As DG
        Dim LoggedInUserID As String

        Try

            Common = Session("Common")

            ' ___ Initialize the app session
            LoggedInUserID = AppSession.Init(Page)
            Common.LoggedInUserID = LoggedInUserID

            ' ___ Right check
            Dim RightsRqd(0) As String
            Rights = New RightsClass(LoggedInUserID, Page)
            RightsRqd.SetValue(Rights.HistoryView, 0)
            Rights.HasSufficientRights(RightsRqd, True, Page)
            lblCurrentRights.Text = Common.GetCurRightsHidden(Rights.RightsColl)

            ' ___ Get the page session object
            Sess = Session("LogWorklistSession")

            ' ___ Determine the page mode
            If Page.IsPostBack AndAlso Request.Form("__EVENTTARGET") = "" Then
                PageMode = PageMode.Postback
            Else
                Select Case Request.QueryString("CalledBy")
                    Case "Child"
                        PageMode = PageMode.ReturnFromChild
                    Case "Other"
                        If Sess.PageInitiallyLoaded Then
                            PageMode = PageMode.CalledByOther
                        Else
                            PageMode = PageMode.Initial
                            Sess.PageInitiallyLoaded = True
                        End If
                    Case Else
                        PageMode = PageMode.Initial
                        Sess.PageInitiallyLoaded = True
                End Select
            End If

            ' ___ Load the page session variables
            LoadVariables(PageMode)

            ' ___ Initialize the datagrid
            DG = DefineDataGrid("Log")

            ' ___ Execute action
            Select Case PageMode
                Case PageMode.Initial
                    DisplayPage(PageMode, DG, DG.OrderByType.Initial)

                Case PageMode.Postback
                    Action = Request.Form("hdAction")
                    Select Case Action
                        Case "LogMaintain"
                            Response.Redirect("LogMaintain.aspx?CallType=Existing")

                        Case "Sort"
                            DisplayPage(PageMode, DG, DG.OrderByType.Field, Request.Form("hdSortField"))

                        Case "ApplyFilter"
                            DisplayPage(PageMode, DG, DG.OrderByType.Recurring)
                    End Select

                Case PageMode.ReturnFromChild, PageMode.CalledByOther
                    DisplayPage(PageMode, DG, DG.OrderByType.ReturnToPage)
                    If Sess.PageReturnOnLoadMessage <> Nothing Then
                        litMsg.Text = "<script language='javascript'>alert('" & Sess.PageReturnOnLoadMessage & "')</script>"
                        Sess.PageReturnOnLoadMessage = Nothing
                    End If
            End Select

            ' ___ Display enviroment
            PageCaption.InnerHtml = Common.GetPageCaption
            litEnviro.Text = "<input type='hidden' name='hdLoggedInUserID' value='" & LoggedInUserID & "'><input type='hidden' name='hdDBHost'  value='hbg-sql'>"

        Catch ex As Exception
            Throw New Exception("Error #102: LogWorklist Page_Load. " & ex.Message)
        End Try
    End Sub

    Private Sub LoadVariables(ByVal PageMode As PageMode)
        Dim Box As Object

        Try

            Select Case PageMode
                Case PageMode.Initial
                    Sess.FileId = String.Empty
                    Sess.ActiveClientValue = String.Empty
                    Sess.ActiveProcessTypeID = String.Empty
                    Sess.ClientIDSelectedFilterValue = String.Empty
                    Sess.ProcessTypeIDSelectedFilterValue = String.Empty
                    Sess.ActiveClientIDOnly = String.Empty
                    Sess.DateRange = "||||"

                Case PageMode.ReturnFromChild, PageMode.CalledByOther
                    Sess.ClientIDSelectedFilterValue = Sess.ActiveClientValue
                    Sess.ProcessTypeIDSelectedFilterValue = Sess.ActiveProcessTypeID
                    Box = Split(Sess.ActiveClientValue, "|")
                    Sess.ActiveClientIDOnly = Box(0)

                Case PageMode.Postback
                    Sess.FileId = Request.Form("hdFileID")
                    Sess.ActiveClientValue = Request.Form("hdActiveClientValue")
                    Sess.ActiveProcessTypeID = Request.Form("hdActiveProcessTypeID")
                    Sess.ClientIDSelectedFilterValue = Request.Form("ddClientID")
                    Sess.ProcessTypeIDSelectedFilterValue = Request.Form("ddProcessTypeID")
                    Box = Split(Sess.ActiveClientValue, "|")
                    Sess.ActiveClientIDOnly = Box(0)
                    Sess.DateRange = Request.Form("hdDateRange")
                    'cFilterOnResponse = Request.Form("hdFilterOnResponse")
            End Select

        Catch ex As Exception
            Throw New Exception("Error #103: LogWorklist LoadVariables. " & ex.Message)
        End Try
    End Sub

    Private Function DefineDataGrid(ByVal Entity As String) As DG
        Dim DG As DG

        Try

            DG = New DG("FileID", Rights, True, "EmbeddedTableDef", "FileSendReceiveDate", "D")
            'DG.AddDataBoundColumn("FileSendReceiveDate", "FileSendReceiveDate", "Send/Recd<br>Date", "FileSendReceiveDate", True, Nothing, Nothing, "align='left'")
            DG.AddDataBoundColumn("FileSendReceiveDate", "FileSendReceiveDate", "Send/Recd<br>Date", "FileSendReceiveDate", True, "MMM-dd-yy HH:mm", Nothing, "align='left'")
            DG.AddDataBoundColumn("FileID", "FileID", "File<br>ID", "FileID", True, Nothing, Nothing, "align='left'")
            If Sess.ActiveProcessTypeID = "Export" Then
                DG.AddDataBoundColumn("DestinationCode", "DestinationCode", "Destination<br>Code", "DestinationCode", True, Nothing, Nothing, "align='left'")
            Else
                DG.AddDataBoundColumn("DestinationCode", "DestinationCode", "Destination<br>Code", "DestinationCode", False, Nothing, Nothing, "align='left'")
            End If

            'DG.AddFreeFormColumn("Spacer0", "       ", Nothing, Nothing, True, Nothing)

            'If Sess.ActiveProcessTypeID = "Export" Then
            '    DG.AddDataBoundColumn("ProcessName", "ProcessName", "Process<br>Name", "ProcessName", True, Nothing, Nothing, "align='left'")
            'Else
            '    DG.AddDataBoundColumn("ProcessName", "ProcessName", "Process<br>Name", "ProcessName", False, Nothing, Nothing, "align='left'")
            'End If

            'DG.AddFreeFormColumn("Spacer", "       ", Nothing, Nothing, True, Nothing)

            '"MMM-dd-yy hh:mm tt"

            DG.AddDataBoundColumn("FilePostedDate", "FilePostedDate", "Posted<br>Date", "FilePostedDate", True, "MMM-dd-yy HH:mm", Nothing, "align='left'")
            'DG.AddDataBoundColumn("FilePostedDate", "FilePostedDate", "Posted<br>Date", "FilePostedDate", True, Nothing, Nothing, "align='left'")

            DG.AddDataBoundColumn("FilePostedBy", "FilePostedBy", "Posted<br>By", "FilePostedBy", True, Nothing, Nothing, "align='left'")
            DG.AddDataBoundColumn("FileReceiptReceivedDate", "FileReceiptReceivedDate", "Receipt<br>Recd<br>Date", "FileReceiptReceivedDate", True, "MMM-dd-yy", Nothing, "align='left'")
            DG.AddDataBoundColumn("FileReceiptReceivedBy", "FileReceiptReceivedBy", "Receipt<br>Recd<br>By", "FileReceiptReceivedBy", True, Nothing, Nothing, "align='left'")
            DG.AddDataBoundColumn("ErrorReportDoneDate", "ErrorReportDoneDate", "Error<br>Rpt<br>Date", "ErrorReportDoneDate", True, "MMM-dd-yy", Nothing, "align='left'")
            DG.AddDataBoundColumn("ErrorReportDoneBy", "ErrorReportDoneBy", "Error<br>Rpt<br>By", "ErrorReportDoneBy", True, Nothing, Nothing, "align='left'")

            Dim TemplateCol As New DG.TemplateColumn("Icons", Nothing, False, Nothing, True)
            TemplateCol.AddDefaultTemplateItem("View", "LogMaintain", "StandardView", "Feed Detail", Rights.LogView, "ViewLog")
            DG.AttachTemplateCol(TemplateCol)

            ' ___ Build the external filter
            Dim ExternalFilter As DG.ExternalFilter
            'ExternalFilter = DG.AttachExternalFilter(DG.FilterOperationMode.FilterSwitchable, DG.FilterInitialShowHideEnum.FilterInitialShow, DG.RecordsInitialShowHideEnum.RecordsInitialShow)
            ExternalFilter = DG.AttachExternalFilter()
            ExternalFilter.AddDropdown("ClientID", "ClientID", 2, "onchange='ClientSelect()'")
            If Sess.ActiveProcessTypeID = "Export" Then
                ExternalFilter.AddDropdown("ProcessTypeID", "ProcessTypeID", 3, "onchange='ProcessTypeIDSelect()'")
            Else
                ExternalFilter.AddDropdown("ProcessTypeID", "ProcessTypeID", 4, "onchange='ProcessTypeIDSelect()'")
            End If

            ExternalFilter.AddLinkItem("DateRange", "SendReceiveDate", 1, "GetDateRange")

            Return DG

        Catch ex As Exception
            Throw New Exception("Error #103: LogWorklist DefineDataGrid. " & ex.Message)
        End Try
    End Function

    Private Sub DisplayPage(ByVal PageMode As PageMode, ByVal DG As DG, ByVal OrderByType As DG.OrderByType, Optional ByVal OrderByField As String = Nothing)
        Dim i As Integer
        Dim dt As DataTable
        Dim Sql As String
        Dim WhereClause As New System.Text.StringBuilder
        Dim ShowFilter As Boolean
        Dim ShowRecords As Boolean
        Dim sbHiddens As New System.Text.StringBuilder
        Dim sb As New System.Text.StringBuilder
        Dim SecurityWhereClause As String

        Dim CmdAsst As CmdAsst
        Dim QueryPack As CmdAsst.QueryPack
        Dim Value As String
        Dim Client As Object
        Dim ExternalFilter As DG.ExternalFilter

        Try


            ExternalFilter = DG.GetExternalFilter

            ' ___ Client Name and Process Type dropdowns        
            CmdAsst = New CmdAsst(CommandType.StoredProcedure, "ClientRead")
            QueryPack = CmdAsst.Execute()
            dt = QueryPack.dt
            ExternalFilter("ClientID").AddDropdownItem("", "", True)
            ExternalFilter("ProcessTypeID").AddDropdownItem("", "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;", True)

            If dt.Rows.Count > 0 Then
                For i = 0 To dt.Rows.Count - 1
                    Value = Common.StrInHandler(dt.Rows(i)(0))
                    Client = Split(Value, "|")
                    ExternalFilter("ClientID").AddDropdownItem(Value, Client(0))
                Next
            End If
            If PageMode = PageMode.ReturnFromChild Or PageMode = PageMode.CalledByOther Then
                ExternalFilter.Coll("ClientID").SetOverrideValue(Sess.ClientIDSelectedFilterValue)
            End If

            ExternalFilter.Coll("DateRange").SetLinkValues("Date range", GetDateRange())

        Catch ex As Exception
            Throw New Exception("Error #104a: LogWorklist DisplayPage. " & ex.Message)
        End Try

        Try

            ' ___ Get date range
            SecurityWhereClause = GetDateRange()

            ' ___ Heading
            If Rights.HasThisRight(Rights.LogEdit) Then
                litHeading.Text = "Edit Log"
            Else
                litHeading.Text = "View Log"
            End If

            ' ___ Handle the sort
            If Sess.SortReference <> Nothing Then
                DG.UpdateSortReference(Sess.SortReference)
            End If
            DG.SetSortElements(OrderByField, OrderByType)

        Catch ex As Exception
            Throw New Exception("Error #104b: LogWorklist DisplayPage. " & ex.Message)
        End Try

        Try

            ' ___ The filters play an unusual role here. They determine the database and table
            ' from which the records are extracted as opposed to selecting records within a table.

            If Sess.ActiveClientValue = String.Empty Then

                ' ___ First time through dummy table row
                sb.Append("SELECT FileID = '', DestinationCode='', FileSendReceiveDate = '', FilePostedDate = '', FilePostedBy = '', FileReceiptReceivedDate = '', FileReceiptReceivedBy = '', ErrorReportDoneDate = '', ErrorReportDoneBy = '', ViewLog='0'")

            Else

                Select Case Sess.ActiveProcessTypeID
                    Case "Import"
                        'sb.Append("SELECT FileSendReceiveDate = FileReceiveDate, FileID, DestinationCode='', FilePostedDate, FilePostedBy, FileReceiptReceivedDate, FileReceiptReceivedBy, ErrorReportDoneDate, ErrorReportDoneBy, ViewLog = '1' FROM ")
                        sb.Append("SELECT FileSendReceiveDate = FileReceiveDate, FileID, DestinationCode='', FilePostedDate='', FilePostedBy='', FileReceiptReceivedDate='', FileReceiptReceivedBy='', ErrorReportDoneDate='', ErrorReportDoneBy='', ViewLog = '1' FROM ")
                        sb.Append(Sess.ActiveClientIDOnly & "..ImportFileHistory")
                    Case "Export"

                        If Common.DoesTableExist(Sess.ActiveClientIDOnly, "ExportDestinationCodes") Then
                            sb.Append("SELECT FileSendReceiveDate = efh.FileSendDate, efh.FileID, edc.DestinationCode, efh.FilePostedDate, efh.FilePostedBy, efh.FileReceiptReceivedDate, efh.FileReceiptReceivedBy, efh.ErrorReportDoneDate, efh.ErrorReportDoneBy, ViewLog = '1' FROM ")
                            sb.Append(Sess.ActiveClientIDOnly & "..ExportFileHistory efh ")
                            sb.Append("LEFT JOIN " & Sess.ActiveClientIDOnly & "..ExportDestinationCodes edc on efh.DestinationID = edc.DestinationID")
                        Else
                            sb.Append("SELECT FileSendReceiveDate = FileSendDate, FileID, DestinationCode = '', FilePostedDate, FilePostedBy, FileReceiptReceivedDate, FileReceiptReceivedBy, ErrorReportDoneDate, ErrorReportDoneBy, ViewLog = '1' FROM ")
                            sb.Append(Sess.ActiveClientIDOnly & "..ExportFileHistory")
                        End If

                End Select

            End If

            Sql = sb.ToString
            DG.GenerateSQL(Sql, ShowFilter, SecurityWhereClause, OrderByType, Request, Sess.FilterOnOffState, Request.Form("hdFilterShowHideToggle"), False)

        Catch ex As Exception
            Throw New Exception("Error #104c: LogWorklist DisplayPage. " & ex.Message)

        End Try

        Try

            dt = Common.GetDT(Sql)

        Catch ex As Exception
            Throw New Exception("Error #104d: LogWorklist DisplayPage. Database/table: " & Sess.ActiveClientIDOnly & ".." & Sess.ActiveProcessTypeID & "FileHistory " & vbCrLf & ex.Message)
        End Try

        Try

            ' ___ Write the datagrid to the page
            litDG.Text = DG.GetText(dt)

            ' ___ Set the FilterOnOffState
            If DG.FilterOperationMode = DG.FilterOperationModeEnum.FilterSwitchable AndAlso ShowFilter Then
                Sess.FilterOnOffState = "on"
            Else
                Sess.FilterOnOffState = "off"
            End If

            ' ___ Set the last field sorted and sort direction in the sort reference
            'Viewstate("SortReference") = DG.GetSortReference()
            'Session("LogWorklistSortReference") = DG.GetSortReference()
            Sess.SortReference = DG.GetSortReference

            sbHiddens.Append("<input type='hidden' name='hdFileID' id='hdFileID' value='" & Sess.FileId & "'>")
            sbHiddens.Append("<input type='hidden' name='hdActiveClientValue' id='hdActiveClientValue' value='" & Sess.ActiveClientValue & "'>")
            sbHiddens.Append("<input type='hidden' id='hdActiveProcessTypeID'  name='hdActiveProcessTypeID' value='" & Sess.ActiveProcessTypeID & "'>")
            'sbHiddens.Append("<input type='hidden' id='hdClientIDSelectedFilterValue' name='hdClientIDSelectedFilterValue' value='" & Sess.ClientIDSelectedFilterValue & "'>")
            sbHiddens.Append("<input type='hidden' name='hdProcessTypeIDSelectedFilterValue' id='hdProcessTypeIDSelectedFilterValue' value='" & Sess.ProcessTypeIDSelectedFilterValue & "'>")
            sbHiddens.Append("<input type='hidden' name='hdDateRange' id='hdDateRange' value='" & Sess.DateRange & "'>")
            litHiddens.Text = sbHiddens.ToString


        Catch ex As Exception
            Throw New Exception("Error #104e: LogWorklist DisplayPage. " & ex.Message)
        End Try
    End Sub

    Public Function GetDateRange() As String
        Dim WhereClause As String
        Dim FldName As String
        Dim FromMonthYear As String
        Dim ToMonthYear As String
        Dim Text As String
        Dim Dates As Object

        Try

            Select Case Sess.ActiveProcessTypeID
                Case "Import"
                    FldName = "FileReceiveDate"
                Case "Export"
                    FldName = "FileSendDate"
            End Select

            Dates = Split(Sess.DateRange, "|")

            If IsNumeric(Dates(0)) AndAlso IsNumeric(Dates(1)) Then
                FromMonthYear = FldName & " >= '" & Dates(0) + 1 & "/01/" & Dates(1) & "'"
            End If
            If IsNumeric(Dates(1)) AndAlso IsNumeric(Dates(2)) Then
                ToMonthYear = FldName & " <= '" & Dates(2) + 1 & "/" & GetLastDayOfMonth(Dates(2), Dates(3)) & "/" & Dates(3) & "'"
            End If

            If FromMonthYear = Nothing Then
                If ToMonthYear = Nothing Then
                    WhereClause = Nothing
                Else
                    WhereClause = ToMonthYear
                End If
            Else
                If ToMonthYear = Nothing Then
                    WhereClause = FromMonthYear
                Else
                    WhereClause = FromMonthYear & " AND " & ToMonthYear
                End If
            End If
            Return WhereClause

        Catch ex As Exception
            Throw New Exception("Error #105: LogWorklist GetDateRange. Sess.DateRange: " & Sess.DateRange & " " & ex.Message)
        End Try
    End Function

    Function GetLastDayOfMonth(ByVal Month As Integer, ByVal Year As Integer) As Integer
        Try

            If ((Year Mod 4 = 0) And (Year Mod 100 <> 0) Or (Year Mod 400 = 0)) Then
                Dim Days() As String = {31, 29, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31}
                Return Days(Month)
            Else
                Dim Days() As String = {31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31}
                Return Days(Month)
            End If

        Catch ex As Exception
            Throw New Exception("Error #106: LogWorklist GetLastDayOfMonth. Month: " & Month & "  Year: " & Year & " " & ex.Message)
        End Try
    End Function

End Class
