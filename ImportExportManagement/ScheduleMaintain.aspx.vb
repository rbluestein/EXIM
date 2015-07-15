Public Class ScheduleMaintain
    Inherits System.Web.UI.Page

#Region " Declarations "
    Protected PageCaption As HtmlGenericControl
    Protected WithEvents litMsg As System.Web.UI.WebControls.Literal
    Protected WithEvents litResponseAction As System.Web.UI.WebControls.Literal
    Protected WithEvents txtAltContactNumber As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtEmail As System.Web.UI.WebControls.TextBox
    Protected WithEvents chkBVI As System.Web.UI.WebControls.CheckBox
    Protected WithEvents chkOther As System.Web.UI.WebControls.CheckBox
    Protected WithEvents lblCurrentRights As System.Web.UI.WebControls.Label
    Private Common As Common
    Private AppSession As New AppSession
    Private Rights As RightsClass
    Private cLoggedInUserID As String
    Protected WithEvents lblUserID As System.Web.UI.WebControls.Label
    Protected WithEvents txtLocationID As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtDestinationID As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtRole As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtCompanyID As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtWorkState As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtBVI As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtOther As System.Web.UI.WebControls.TextBox
    Protected WithEvents litHiddens As System.Web.UI.WebControls.Literal

    'Private cActiveClientID As String
    'Private cProcessName As String
    'Private cDestinationID As Integer
    Private cStartDate As String

    Protected WithEvents rbActiveInd As System.Web.UI.WebControls.RadioButtonList
    Protected WithEvents txtActiveInd As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtLicenseNumber As System.Web.UI.WebControls.TextBox
    Protected WithEvents litHeading As System.Web.UI.WebControls.Literal
    Protected WithEvents txtClientID As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtProcessName As System.Web.UI.WebControls.TextBox
    Protected WithEvents litUpdate As System.Web.UI.WebControls.Literal
    Protected WithEvents txtStartDate As System.Web.UI.WebControls.TextBox
    Protected WithEvents lblStartDateLink As System.Web.UI.WebControls.Label
    Protected WithEvents txtEndDate As System.Web.UI.WebControls.TextBox
    Protected WithEvents lblEndDateLink As System.Web.UI.WebControls.Label
    Protected WithEvents txtDayOfWeek As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtDayOfMonth As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtDateOfMonth As System.Web.UI.WebControls.TextBox
    Protected WithEvents lblDayOfWeekLink As System.Web.UI.WebControls.Label
    Protected WithEvents ddDateOfMonth As System.Web.UI.WebControls.DropDownList
    Protected WithEvents phQASection As System.Web.UI.WebControls.PlaceHolder
    Protected WithEvents phExport As System.Web.UI.WebControls.PlaceHolder
    Protected WithEvents txtTimeOfDay As System.Web.UI.WebControls.TextBox
    Protected WithEvents lblTimeOfDayLink As System.Web.UI.WebControls.Label
    Protected WithEvents plDayOfMonth As System.Web.UI.WebControls.PlaceHolder
    Protected WithEvents plTimeOfDay As System.Web.UI.WebControls.PlaceHolder
    Protected WithEvents litEnviro As System.Web.UI.WebControls.Literal
    Protected WithEvents NumDaysForReceipt As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtNumDaysForReceipt As System.Web.UI.WebControls.TextBox
    Private Sess As FeedlistSession
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

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim RequestAction As RequestAction
        Dim ResponseAction As ResponseAction
        Dim Results As New Results

        Try

            Common = Session("Common")

            ' ___ Restore  session
            cLoggedInUserID = AppSession.RestoreSession(Page)

            ' ___ Right Check
            Dim RightsRqd(0) As String
            Rights = New RightsClass(cLoggedInUserID, Page)
            RightsRqd.SetValue(Rights.ScheduleView, 0)
            Rights.HasSufficientRights(RightsRqd, True, Page)
            lblCurrentRights.Text = Common.GetCurRightsHidden(Rights.RightsColl)

            ' ___ Get the session object
            Sess = Session("FeedlistSession")

            ' ___ Get RequestAction
            RequestAction = Common.GetRequestAction(Page)

            ' ___ Execute the RequestAction
            Results = ExecuteRequestAction(RequestAction)
            ResponseAction = Results.ResponseAction

            ' ___ Execute the ResponseAction
            If ResponseAction = ResponseAction.ReturnToCallingPage Then
                Sess.PageReturnOnLoadMessage = Results.Msg
                Response.Redirect("Feedlist.aspx?CalledBy=Child")
            Else
                DisplayPage(ResponseAction)
                If Not Results.Msg = Nothing Then
                    litMsg.Text = "<script language='javascript'>alert('" & Results.Msg & "')</script>"
                End If
                litResponseAction.Text = "<input type='hidden' name='hdResponseAction' value = '" & ResponseAction.ToString & "'>"
            End If

            ' ___ Display enviroment
            PageCaption.InnerHtml = Common.GetPageCaption
            litEnviro.Text = "<input type='hidden' name='hdLoggedInUserID' value='" & Common.LoggedInUserID & "'><input type='hidden' name='hdDBHost'  value='hbg-sql'>"

        Catch ex As Exception
            Throw New Exception("Error #502: ScheduleMaintain Page_Load " & ex.Message)
        End Try
    End Sub

    Private Function ExecuteRequestAction(ByVal RequestAction As RequestAction) As Results
        Dim ValidationResults As New Results
        Dim SaveResults As New Results
        Dim MyResults As New Results

        Try

            Select Case RequestAction
                Case RequestAction.ReturnToParent
                    MyResults.ResponseAction = ResponseAction.ReturnToCallingPage
                    MyResults.Success = True

                Case RequestAction.CreateNew
                    MyResults.ResponseAction = ResponseAction.DisplayBlank

                Case RequestAction.SaveNew
                    ValidationResults = IsDataValid(RequestAction)
                    If ValidationResults.Success Then
                        SaveResults = PerformSave(RequestAction)
                        If SaveResults.Success Then
                            MyResults.ResponseAction = ResponseAction.ReturnToCallingPage
                            MyResults.Msg = SaveResults.Msg
                        Else
                            MyResults.ResponseAction = ResponseAction.DisplayUserInputNew
                            MyResults.Msg = SaveResults.Msg
                        End If
                    ElseIf ValidationResults.ObtainConfirm Then
                        MyResults.ResponseAction = ResponseAction.DisplayUserInputNew
                        MyResults.Msg = ValidationResults.Msg
                    Else
                        MyResults.ResponseAction = ResponseAction.DisplayUserInputNew
                        MyResults.Msg = ValidationResults.Msg
                    End If

                Case RequestAction.LoadExisting
                    MyResults.ResponseAction = ResponseAction.DisplayExisting

                Case RequestAction.SaveExisting
                    ValidationResults = IsDataValid(RequestAction)
                    If ValidationResults.Success Then
                        SaveResults = PerformSave(RequestAction)
                        If SaveResults.Success Then
                            MyResults.ResponseAction = ResponseAction.ReturnToCallingPage
                            MyResults.Msg = SaveResults.Msg
                        Else
                            MyResults.ResponseAction = ResponseAction.DisplayUserInputExisting
                            MyResults.Msg = SaveResults.Msg
                        End If
                    ElseIf ValidationResults.ObtainConfirm Then
                        MyResults.ResponseAction = ResponseAction.DisplayUserInputExisting
                        MyResults.Msg = ValidationResults.Msg
                    Else
                        MyResults.ResponseAction = ResponseAction.DisplayExisting
                        MyResults.Msg = ValidationResults.Msg
                    End If

                Case RequestAction.NoSaveNew
                    MyResults.ResponseAction = ResponseAction.DisplayUserInputNew

                Case RequestAction.NoSaveExisting
                    MyResults.ResponseAction = ResponseAction.DisplayUserInputExisting

                Case RequestAction.Other
                    MyResults.ResponseAction = Common.GetResponseActionFromRequestActionOther(Page)
            End Select

            Return MyResults

        Catch ex As Exception
            Throw New Exception("Error #503: ScheduleMaintain ExecuteRequestAction " & ex.Message)
        End Try
    End Function

    Private Function IsDataValid(ByVal RequestAction As RequestAction) As Results
        Dim MyResults As New Results
        Dim dt As DataTable
        Dim ErrColl As New Collection
        Dim OKSoFar As Boolean = True

        Try

            ' ___ Trim the textbox input
            txtStartDate.Text = Trim(txtStartDate.Text)
            txtEndDate.Text = Trim(txtEndDate.Text)
            txtDayOfWeek.Text = Trim(txtDayOfWeek.Text)
            txtDayOfMonth.Text = Trim(txtDayOfMonth.Text)
            txtNumDaysForReceipt.Text = Trim(txtNumDaysForReceipt.Text)
            'txtDateOfMonth.Text = Trim(txtDateOfMonth.Text)

            ' ___ Check for key violation
            If RequestAction = RequestAction.SaveNew Then
                If IsDate(txtStartDate.Text) Then
                    dt = Common.GetDT("SELECT Count (*) FROM FeedSchedule WHERE ClientID = '" & Sess.ActiveClientID & "' AND ProcessName = '" & Sess.ProcessName & "' AND DestinationID = '" & Sess.DestinationID & "' AND StartDate = '" & txtStartDate.Text & "'")
                    If dt.Rows(0)(0) > 0 Then
                        Common.ValidateErrorOnly(ErrColl, "record having this client name, process name, destination id and start date is already in use")
                        OKSoFar = False
                    End If
                End If

            ElseIf RequestAction = RequestAction.SaveExisting Then

                ' ___ Are the proposed key fields the same as the current key fields?
                If txtStartDate.Text = Sess.StartDate Then
                    ' This change does not affect the record key.
                Else
                    ' Key fields changed. Will the proposed change result in a duplicate key?
                    dt = Common.GetDT("SELECT Count (*) FROM FeedSchedule WHERE ClientID = '" & Sess.ActiveClientID & "' AND ProcessName = '" & Sess.ProcessName & "' AND DestinationID = '" & Sess.DestinationID & "' AND StartDate = '" & txtStartDate.Text & "'")
                    If dt.Rows(0)(0) > 0 Then
                        Common.ValidateErrorOnly(ErrColl, "record having this client name, process name, destination id and start date is already  in use")
                        OKSoFar = False
                    End If
                End If
            End If

            If OKSoFar Then
                Common.ValidateDateField(ErrColl, txtStartDate.Text, False, "start date not provided")
                Common.ValidateDateField(ErrColl, txtEndDate.Text, True, "end date not provided")
                Common.ValidateRadio(ErrColl, rbActiveInd.SelectedIndex, False, "status not provided")

                ' ___ v 1.21
                If txtDayOfWeek.Text.Length = 0 AndAlso ddDateOfMonth.SelectedValue.Length = 0 Then
                    Common.ValidateErrorOnly(ErrColl, "day of week or date of month required")
                End If

                If ErrColl.Count > 0 Then
                    OKSoFar = False
                End If
            End If

            If OKSoFar AndAlso IsDate(txtEndDate.Text) Then
                If CType(txtStartDate.Text, Date) > CType(txtEndDate.Text, Date) Then
                    Common.ValidateErrorOnly(ErrColl, "the start date falls after the end date")
                End If
            End If


            If ErrColl.Count = 0 Then
                MyResults.Success = True
            Else
                MyResults.Success = False
                MyResults.Msg = Common.ErrCollToString(ErrColl, "Not saved. Please correct the following:")
            End If
            Return MyResults

        Catch ex As Exception
            Throw New Exception("Error #504: ScheduleMaintain IsDataValid " & ex.Message)
        End Try
    End Function

    Private Function PerformSave(ByVal RequestAction As RequestAction) As Results
        Dim MyResults As New Results
        'Dim CmdAsst As CmdAsst
        Dim QueryPack As CmdAsst.QueryPack
        Dim TimeOfDay As Object
        Dim sb As New System.Text.StringBuilder
        Dim SPName As String


        Try

            If RequestAction = RequestAction.SaveNew Then
                'CmdAsst = New CmdAsst(CommandType.StoredProcedure, "ScheduleInsert")
                SPName = "ScheduleInsert"
            Else
                'CmdAsst = New CmdAsst(CommandType.StoredProcedure, "ScheduleUpd")
                'CmdAsst.AddDateTime("OldStartDate", Sess.StartDate, False)
                SPName = "ScheduleUpd"
            End If

            sb.Append("exec EXIM.." & SPName & " ")

            If SPName = "ScheduleUpd" Then
                sb.Append("@OldStartDate='" & Sess.StartDate & "', ")
            End If

            sb.Append("@ClientID = '" & Sess.ActiveClientID & "', ")
            sb.Append("@ProcessName = '" & Sess.ProcessName & "', ")
            sb.Append("@DestinationID = " & Sess.DestinationID & ", ")
            sb.Append("@StartDate = '" & txtStartDate.Text & "', ")

            If txtEndDate.Text.Length > 0 Then
                sb.Append("@EndDate = '" & txtEndDate.Text & "', ")
            Else
                sb.Append("@EndDate=null, ")
            End If

            If txtDayOfWeek.Text.Length > 0 Then
                sb.Append("@DayOfWeek = '" & txtDayOfWeek.Text & "', ")
            Else
                sb.Append("@DayOfWeek=null, ")
            End If

            If txtDayOfMonth.Text.Length > 0 Then
                sb.Append("@DayOfMonth = '" & txtDayOfMonth.Text & "', ")
            Else
                sb.Append("@DayOfMonth=null, ")
            End If

            If ddDateOfMonth.SelectedItem.Text.Length > 0 Then
                sb.Append("@DateOfMonth = '" & ddDateOfMonth.SelectedItem.Text & "', ")
            Else
                sb.Append("@DateOfMonth=null, ")
            End If

            sb.Append("@TimeOfDay=12, ")

            If rbActiveInd.SelectedIndex = 0 Then
                sb.Append("@ActiveInd=1, ")
            Else
                sb.Append("@ActiveInd=0, ")
            End If

            If txtNumDaysForReceipt.Text.Length > 0 Then
                sb.Append("@NumDaysForReceipt=" & txtNumDaysForReceipt.Text)
            Else
                sb.Append("@NumDaysForReceipt=null")
            End If


            'CmdAsst.AddVarChar("ClientID", Sess.ActiveClientID, 50, False)
            'CmdAsst.AddVarChar("ProcessName", Sess.ProcessName, 50, False)
            'CmdAsst.AddInt("DestinationID", Sess.DestinationID, False)
            'CmdAsst.AddDateTime("StartDate", txtStartDate.Text, False)
            'CmdAsst.AddDateTime("EndDate", txtEndDate.Text, True)
            'CmdAsst.AddVarChar("DayOfWeek", txtDayOfWeek.Text, 50, True)
            'CmdAsst.AddVarChar("DayOfMonth", txtDayOfMonth.Text, 50, True)
            'CmdAsst.AddVarChar("DateOfMonth", ddDateOfMonth.SelectedItem.Text, 50, True)

            'TimeOfDay = 12
            'CmdAsst.AddInt("TimeOfDay", TimeOfDay, True)

            'CmdAsst.AddBit("ActiveInd", rbActiveInd.SelectedIndex, 0, False)


            QueryPack = Common.ExecuteNonQueryWithQueryPack(sb.ToString)


            'QueryPack = CmdAsst.Execute

            MyResults.Success = QueryPack.Success
            If QueryPack.Success Then
                MyResults.Msg = "Update complete."
                Sess.StartDate = txtStartDate.Text
            Else
                MyResults.Msg = "Unable to update record."
            End If
            Return MyResults

        Catch ex As Exception
            Throw New Exception("Error #505: ScheduleMaintain PerformSave " & ex.Message)
        End Try
    End Function

    Private Sub DisplayPage(ByVal ResponseAction As ResponseAction)
        Dim i As Integer
        Dim dt As DataTable
        Dim Hiddens As New System.Text.StringBuilder
        Dim Sql As String
        Dim TimeOfDay As Integer

        Try

            ' ____ Control attributes
            txtNumDaysForReceipt.Attributes.Add("onkeyup", "AllowNumericOnly(this)")

            ' ___ Heading
            Select Case ResponseAction
                Case ResponseAction.DisplayBlank
                    litHeading.Text = "New Schedule"
                Case ResponseAction.DisplayUserInputNew
                    litHeading.Text = "New Schedule"
                Case ResponseAction.DisplayExisting
                    If Rights.HasThisRight(Rights.ScheduleEdit) Then
                        litHeading.Text = "Edit Schedule"
                    Else
                        litHeading.Text = "View Schedule"
                    End If
            End Select

            If Not Page.IsPostBack Then

                ' ___ Date of month
                ddDateOfMonth.Items.Add(New ListItem("", ""))
                For i = 1 To 31
                    ddDateOfMonth.Items.Add(New ListItem(i, i))
                Next
            End If

            ' ___ Format the controls
            FormatControls()

            txtClientID.Text = Sess.ActiveClientID
            txtProcessName.Text = Sess.ProcessName

        Catch ex As Exception
            Throw New Exception("Error #506a: ScheduleMaintain DisplayPage " & ex.Message)
        End Try

        Try

            If Sess.ProcessTypeID = "Export" Then
                If Common.DoesDatabaseAndTableExist(Sess.ActiveClientID, "ExportDestinationCodes") Then
                    Sql = "SELECT edc.DestinationCode FROM FeedList fl INNER JOIN " & Sess.ActiveClientID & "..ExportDestinationCodes edc on fl.DestinationID = edc.DestinationID "
                    Sql &= "WHERE  fl.ClientID = '" & Sess.ActiveClientID & "' AND fl.ProcessName = '" & Sess.ProcessName & "' AND fl.DestinationID = " & Sess.DestinationID

                    dt = Common.GetDT(Sql)
                    If dt.Rows.Count > 0 Then
                        txtDestinationID.Text = dt.Rows(0)(0)
                    Else
                        txtDestinationID.Text = Sess.DestinationID
                    End If
                    dt = Nothing
                Else
                    txtDestinationID.Text = Sess.DestinationID
                End If
            End If

        Catch ex As Exception
            Throw New Exception("Error #506b: ScheduleMaintain DisplayPage " & ex.Message)
        End Try

        Try

            If ResponseAction = ResponseAction.DisplayExisting Then

                ' ___ Get the data
                dt = Common.GetDT("SELECT * FROM FeedSchedule WHERE ClientID = '" & Sess.ActiveClientID & "' AND ProcessName = '" & Sess.ProcessName & "'  AND DestinationID = '" & Sess.DestinationID & "' AND StartDate = '" & Sess.StartDate & "'")

                ' ___ Clear the dropdown/radio list selections
                rbActiveInd.ClearSelection()
                ddDateOfMonth.ClearSelection()

                txtStartDate.Text = Common.StrInHandler(dt.Rows(0)("StartDate"))
                txtEndDate.Text = Common.StrInHandler(dt.Rows(0)("EndDate"))
                txtDayOfWeek.Text = Common.StrInHandler(dt.Rows(0)("DayOfWeek"))
                txtDayOfMonth.Text = Common.StrInHandler(dt.Rows(0)("DayOfMonth"))
                txtNumDaysForReceipt.Text = Common.StrInHandler(dt.Rows(0)("NumDaysForReceipt"))

                Try
                    ddDateOfMonth.Items.FindByValue(Common.StrInHandler(dt.Rows(0)("DateOfMonth"))).Selected = True
                Catch ex As Exception
                    Throw New Exception("Error #506c: ScheduleMaintain DisplayPage " & ex.Message)
                End Try

                txtDateOfMonth.Text = Common.StrInHandler(dt.Rows(0)("DateOfMonth"))
                rbActiveInd.SelectedIndex = Common.BitToRadio(dt.Rows(0)("ActiveInd"), 0, True)
                txtActiveInd.Text = Common.BitToString(dt.Rows(0)("ActiveInd"), "Active", "Inactive", False)

            End If

        Catch ex As Exception
            Throw New Exception("Error #506d: ScheduleMaintain DisplayPage " & ex.Message)
        End Try
    End Sub

    Private Sub FormatControls()
        Dim dt As DataTable

        Try

            If Sess.ProcessTypeID = "Export" Then
                If Common.DoesDatabaseAndTableExist(Sess.ActiveClientID, "ExportDestinationCodes") Then
                    phExport.Visible = True
                Else
                    phExport.Visible = False
                End If
            Else
                phExport.Visible = False
            End If

            If Rights.HasThisRight(Rights.FeedlistEdit) Then
                litUpdate.Text = "<input onclick='Update()' type='button' value='Update'>"

                Style.AddStyle(txtClientID, Style.StyleType.NoneditableGrayed, 300)
                Style.AddStyle(txtProcessName, Style.StyleType.NoneditableGrayed, 300)
                Style.AddStyle(txtDestinationID, Style.StyleType.NoneditableGrayed, 300)

                rbActiveInd.Visible = True
                Style.AddStyle(txtActiveInd, Style.StyleType.NotVisible, 112)


                'Calendar1.Visible = False
                'lblYearUpDown.Visible = False
                lblStartDateLink.Visible = True
                lblEndDateLink.Visible = True
                lblDayOfWeekLink.Visible = True

                Style.AddStyle(txtStartDate, Style.StyleType.NormalEditable, 112)
                Style.AddStyle(txtEndDate, Style.StyleType.NoneditableWhite, 112)
                Style.AddStyle(txtDayOfWeek, Style.StyleType.NoneditableWhite, 300)
                Style.AddStyle(txtDayOfMonth, Style.StyleType.NormalEditable, 300)
                Style.AddStyle(txtTimeOfDay, Style.StyleType.NoneditableWhite, 300)

                ddDateOfMonth.Visible = True
                Style.AddStyle(txtDateOfMonth, Style.StyleType.NotVisible, 300)

            Else

                Style.AddStyle(txtClientID, Style.StyleType.NoneditableGrayed, 300)
                Style.AddStyle(txtProcessName, Style.StyleType.NoneditableGrayed, 300)
                Style.AddStyle(txtDestinationID, Style.StyleType.NoneditableGrayed, 300)

                rbActiveInd.Visible = False
                Style.AddStyle(txtActiveInd, Style.StyleType.NoneditableGrayed, 112)

                'Calendar1.Visible = False
                'lblYearUpDown.Visible = False
                lblStartDateLink.Visible = False
                lblEndDateLink.Visible = False
                lblDayOfWeekLink.Visible = False

                Style.AddStyle(txtStartDate, Style.StyleType.NoneditableGrayed, 112)
                Style.AddStyle(txtEndDate, Style.StyleType.NoneditableGrayed, 112)
                Style.AddStyle(txtDayOfWeek, Style.StyleType.NoneditableGrayed, 300)
                Style.AddStyle(txtDayOfMonth, Style.StyleType.NoneditableGrayed, 300)
                Style.AddStyle(txtTimeOfDay, Style.StyleType.NoneditableGrayed, 300)
                ddDateOfMonth.Visible = False
                Style.AddStyle(txtDateOfMonth, Style.StyleType.NoneditableGrayed, 300)
        End If
        Catch ex As Exception
            Throw New Exception("Error #507: ScheduleMaintain FormatControls " & ex.Message)
        End Try
    End Sub

End Class

