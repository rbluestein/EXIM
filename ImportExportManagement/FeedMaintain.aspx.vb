Public Class FeedMaintain
    Inherits System.Web.UI.Page

#Region " Declarations "
    Protected PageCaption As HtmlGenericControl
    Protected WithEvents litMsg As System.Web.UI.WebControls.Literal
    Protected WithEvents litHeading As System.Web.UI.WebControls.Literal
    Protected WithEvents litResponseAction As System.Web.UI.WebControls.Literal
    Protected WithEvents txtAltContactNumber As System.Web.UI.WebControls.TextBox
    Protected WithEvents chkBVI As System.Web.UI.WebControls.CheckBox
    Protected WithEvents chkOther As System.Web.UI.WebControls.CheckBox
    Protected WithEvents lblCurrentRights As System.Web.UI.WebControls.Label
    Private Common As Common
    Private AppSession As New AppSession
    Private Rights As RightsClass
    Private cLoggedInUserID As String
    Protected WithEvents lblUserID As System.Web.UI.WebControls.Label
    Protected WithEvents txtLocationID As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtRole As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtCompanyID As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtWorkState As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtErrorReportSource As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtBVI As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtOther As System.Web.UI.WebControls.TextBox
    Protected WithEvents litHiddens As System.Web.UI.WebControls.Literal
    Private cSubjUserID As String
    Private cPageLoad As PageMode

    Protected WithEvents txtProcessName As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtDestinationID As System.Web.UI.WebControls.TextBox
    Protected WithEvents ddDestinationID As System.Web.UI.WebControls.DropDownList
    Protected WithEvents ddCarrierID As System.Web.UI.WebControls.DropDownList
    Protected WithEvents txtCarrierID As System.Web.UI.WebControls.TextBox
    Protected WithEvents rbActiveInd As System.Web.UI.WebControls.RadioButtonList
    Protected WithEvents txtActiveInd As System.Web.UI.WebControls.TextBox
    Protected WithEvents rbScheduledInd As System.Web.UI.WebControls.RadioButtonList
    Protected WithEvents txtScheduledInd As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtFileName As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtFileLocation As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtFileDestLocation As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtFTPUserName As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtFTPPassword As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtEmailTo As System.Web.UI.WebControls.TextBox
    Protected WithEvents rbEmailAttachmentInd As System.Web.UI.WebControls.RadioButtonList
    Protected WithEvents txtEmailAttachmentInd As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtPGPKey As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtFrequencyID As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtNotes As System.Web.UI.WebControls.TextBox
    Protected WithEvents ddDeveloper As System.Web.UI.WebControls.DropDownList
    Protected WithEvents txtDeveloper As System.Web.UI.WebControls.TextBox
    Protected WithEvents litUpdate As System.Web.UI.WebControls.Literal
    Protected WithEvents ddClientID As System.Web.UI.WebControls.DropDownList
    Protected WithEvents txtClientID As System.Web.UI.WebControls.TextBox
    Protected WithEvents rbBVIOrCarrierSendsErrorReportInd As System.Web.UI.WebControls.RadioButtonList
    Protected WithEvents txtBVIOrCarrierSendsErrorReportInd As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtErrorReportHandler As System.Web.UI.WebControls.TextBox
    Protected WithEvents ddErrorReportHandler As System.Web.UI.WebControls.DropDownList
    Protected WithEvents ddFrequencyID As System.Web.UI.WebControls.DropDownList
    Protected WithEvents phQASection As System.Web.UI.WebControls.PlaceHolder
    Protected WithEvents ddQAPOC As System.Web.UI.WebControls.DropDownList
    Protected WithEvents txtQAPOC As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtQAStatus As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtQANotes As System.Web.UI.WebControls.TextBox
    Protected WithEvents rbQADCScheduledInd As System.Web.UI.WebControls.RadioButtonList
    Protected WithEvents txtQADCScheduledInd As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtQADCLastModDate As System.Web.UI.WebControls.TextBox
    Protected WithEvents lblEndDateLink As System.Web.UI.WebControls.Label
    Protected WithEvents lblQADCLastModDateLink As System.Web.UI.WebControls.Label
    Protected WithEvents litCalendar As System.Web.UI.WebControls.Literal
    Private Sess As FeedlistSession
    Protected WithEvents phExport As System.Web.UI.WebControls.PlaceHolder
    Protected WithEvents rbReceiptInd As System.Web.UI.WebControls.RadioButtonList
    Protected WithEvents txtReceiptInd As System.Web.UI.WebControls.TextBox
    Protected WithEvents litEnviro As System.Web.UI.WebControls.Literal
    Protected WithEvents lblBVIOrCarrierSendsErrorReport As System.Web.UI.WebControls.Label
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
            RightsRqd.SetValue(Rights.FeedlistView, 0)
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
            Throw New Exception("Error #402: Feedlist Page_Load " & ex.Message)
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
            Throw New Exception("Error #403: Feedlist ExecuteRequestAction " & ex.Message)
        End Try
    End Function

    Private Function IsDataValid(ByVal RequestAction As RequestAction) As Results
        Dim MyResults As New Results
        Dim ErrColl As New Collection
        Dim dt As DataTable
        Dim OKSoFar As Boolean = True

        Try

            ' ___ Trim the textbox input
            txtProcessName.Text = Trim(txtProcessName.Text)
            txtFrequencyID.Text = Trim(txtFrequencyID.Text)
            txtFileName.Text = Trim(txtFileName.Text)
            txtFileLocation.Text = Trim(txtFileLocation.Text)
            txtFileDestLocation.Text = Trim(txtFileDestLocation.Text)
            txtEmailTo.Text = Trim(txtEmailTo.Text)
            txtFTPUserName.Text = Trim(txtFTPUserName.Text)
            txtFTPPassword.Text = Trim(txtFTPPassword.Text)
            txtPGPKey.Text = Trim(txtPGPKey.Text)
            txtErrorReportHandler.Text = Trim(txtErrorReportHandler.Text)
            txtErrorReportSource.Text = Trim(txtErrorReportSource.Text)

            ' ___ Check for key violation
            If IsNumeric(txtDestinationID.Text) Then
                If RequestAction = RequestAction.SaveNew Then
                    dt = Common.GetDT("SELECT Count (*) FROM Feedlist WHERE ClientID = '" & ddClientID.SelectedItem.Value & "' AND ProcessName = '" & txtProcessName.Text & "' AND DestinationID = " & txtDestinationID.Text)
                    If dt.Rows(0)(0) > 0 Then
                        ' If cProcessTypeID = "Import" Then
                        If Sess.ProcessTypeID = "Import" Then
                            Common.ValidateErrorOnly(ErrColl, "record having this client name and process name is already in use")
                        Else
                            Common.ValidateErrorOnly(ErrColl, "record having this client name, process name, and destination id is already in use")
                        End If
                        OKSoFar = False
                    End If

                ElseIf RequestAction = RequestAction.SaveExisting Then

                    ' ___ Are the proposed key fields the same as the current key fields?
                    If ddClientID.SelectedItem.Value.ToLower = Sess.ActiveClientID.ToLower AndAlso txtProcessName.Text = Sess.ProcessName AndAlso txtDestinationID.Text = Sess.DestinationID Then
                        ' This change does not affect the record key.
                    Else
                        ' Key fields changed. Will the proposed change result in a duplicate key?
                        dt = Common.GetDT("SELECT Count (*) FROM Feedlist WHERE ClientID = '" & ddClientID.SelectedItem.Value & "' AND ProcessName = '" & txtProcessName.Text & "' AND DestinationID = " & txtDestinationID.Text)
                        If dt.Rows(0)(0) > 0 Then
                            If Sess.ProcessTypeID = "Import" Then
                                Common.ValidateErrorOnly(ErrColl, "record having this client name and process name is already  in use")
                            Else
                                Common.ValidateErrorOnly(ErrColl, "record having this client name, process name, and destination id is already  in use")
                            End If
                            OKSoFar = False
                        End If
                    End If

                End If
            End If

            If OKSoFar Then
                Common.ValidateDropDown(ErrColl, ddClientID, 1, "client not provided")
                Common.ValidateStringField(ErrColl, txtProcessName.Text, 1, "process name not provided")
                Common.ValidateDropDown(ErrColl, ddFrequencyID, 1, "frequency not provided")
                Common.ValidateRadio(ErrColl, rbActiveInd.SelectedIndex, False, "status not provided")
                Common.ValidateRadio(ErrColl, rbScheduledInd.SelectedIndex, False, "scheduled not provided")
                Common.ValidateRadio(ErrColl, rbReceiptInd.SelectedIndex, False, "receipt not provided")
                Common.ValidateStringField(ErrColl, txtFileName.Text, 1, "file name not provided")
                Common.ValidateStringField(ErrColl, txtFileLocation.Text, 1, "file location not provided")

                ' ___ Export only rules
                If Sess.ProcessTypeID = "Export" Then

                    ' ___ DestinationID required
                    If ddDestinationID.Visible Then
                        Common.ValidateDropDown(ErrColl, ddDestinationID, 1, "destination not provided")
                    End If

                    ' ___ Carrier required
                    Common.ValidateDropDown(ErrColl, ddCarrierID, 1, "carrier not provided")

                    ' ___ Email required
                    If txtEmailTo.Text.Length = 0 Then
                        Common.ValidateErrorOnly(ErrColl, "email address not provided")
                    Else
                        Common.ValidateEmailAddress(ErrColl, txtEmailTo.Text, "valid email address not provided")
                    End If
                End If

                Common.ValidateRadio(ErrColl, rbEmailAttachmentInd.SelectedIndex, False, "email attachment not provided")
                Common.ValidateDropDown(ErrColl, ddDeveloper, 1, "developer not provided")

                'If rbCarrierSendsErrorReportInd.SelectedIndex <> 0 AndAlso ddErrorReportHandler.SelectedIndex > 0 Then
                '    Common.ValidateErrorOnly(ErrColl, "error report handler designated but carrier does not send error report")
                'ElseIf rbCarrierSendsErrorReportInd.SelectedIndex = 0 AndAlso ddErrorReportHandler.SelectedIndex < 1 Then
                '    Common.ValidateErrorOnly(ErrColl, "no error report handler designated")
                'End If
            End If

            If ErrColl.Count = 0 Then
                MyResults.Success = True
            Else
                MyResults.Success = False
                MyResults.Msg = Common.ErrCollToString(ErrColl, "Not saved. Please correct the following:")
            End If
            Return MyResults

        Catch ex As Exception
            Throw New Exception("Error #404: Feedlist IsDataValid " & ex.Message)
        End Try
    End Function

    Private Function PerformSave(ByVal RequestAction As RequestAction) As Results
        Dim MyResults As New Results
        Dim CmdAsst As CmdAsst
        Dim QueryPack As CmdAsst.QueryPack

        Try

            Dim StrOutNull As New StringOut(StringOut.SourceNull.ToNull, StringOut.SourceNothing.ToNull, StringOut.SourceEmptyString.ToEmptyString, StringOut.OutputType.ToParameter)
            Dim StrOutAllNull As New StringOut(StringOut.SourceNull.ToNull, StringOut.SourceNothing.ToNull, StringOut.SourceEmptyString.ToNull, StringOut.OutputType.ToParameter)

            If RequestAction = RequestAction.SaveNew Then
                CmdAsst = New CmdAsst(CommandType.StoredProcedure, "FeedInsert")
            Else
                CmdAsst = New CmdAsst(CommandType.StoredProcedure, "FeedUpd")
                CmdAsst.AddVarChar("OldClientID", Sess.ActiveClientID, 50, False)
                CmdAsst.AddVarChar("OldProcessName", Sess.ProcessName, 50, False)
                CmdAsst.AddInt("OldDestinationID", Sess.DestinationID, False)
            End If

            CmdAsst.AddVarChar("ClientID", ddClientID.SelectedItem.Text, 50, False)
            CmdAsst.AddVarChar("ProcessName", txtProcessName.Text, 50, False)
            CmdAsst.AddVarChar("CarrierID", ddCarrierID.SelectedItem.Text, 50, False)
            CmdAsst.AddVarChar("ProcessTypeID", Sess.ProcessTypeID, 10, False)
            CmdAsst.AddVarChar("Developer", ddDeveloper.SelectedItem.Value, 50, False)

            CmdAsst.AddVarChar("FrequencyID", ddFrequencyID.SelectedItem.Text, 50, False)
            CmdAsst.AddVarChar("FileName", txtFileName.Text, 100, False)
            CmdAsst.AddVarChar("FileLocation", txtFileLocation.Text, 100, False)
            CmdAsst.AddVarChar("EMailTo", txtEmailTo.Text, 150, False)
            CmdAsst.AddBit("EmailAttachmentInd", rbEmailAttachmentInd.SelectedIndex, 0, True)
            CmdAsst.AddVarChar("FileDestLocation", txtFileDestLocation.Text, 150, True)
            CmdAsst.AddVarChar("FTPUserName", txtFTPUserName.Text, 50, True)
            CmdAsst.AddVarChar("FTPPassword", txtFTPPassword.Text, 50, True)
            CmdAsst.AddVarChar("PGPKey", txtPGPKey.Text, 50, True)
            CmdAsst.AddBit("ScheduledInd", rbScheduledInd.SelectedIndex, 0, False)
            CmdAsst.AddBit("ReceiptInd", rbReceiptInd.SelectedIndex, 0, False)
            CmdAsst.AddBit("ActiveInd", rbActiveInd.SelectedIndex, 0, False)

            CmdAsst.AddVarChar("Notes", txtNotes.Text, 100, True)
            CmdAsst.AddBit("BVIOrCarrierSendsErrorReportInd", rbBVIOrCarrierSendsErrorReportInd.SelectedIndex, 0, True)
            CmdAsst.AddVarChar("ErrorReportHandler", ddErrorReportHandler.SelectedItem.Value, 50, True)
            CmdAsst.AddVarChar("ErrorReportSource", txtErrorReportSource.Text, 100, True)

            If Sess.ProcessTypeID = "Import" Then
                CmdAsst.AddInt("DestinationID", 0, False)
            Else
                If ddDestinationID.Visible Then
                    CmdAsst.AddInt("DestinationID", ddDestinationID.SelectedItem.Value, False)
                Else
                    CmdAsst.AddInt("DestinationID", 0, False)
                End If
            End If

            If Rights.HasThisRight(Rights.QAFieldEdit) Then
                CmdAsst.AddVarChar("QAPOC", ddQAPOC.SelectedItem.Value, 50, True)
                CmdAsst.AddVarChar("QAStatus", txtQAStatus.Text, 100, True)
                CmdAsst.AddVarChar("QANotes", txtQANotes.Text, 100, True)
                CmdAsst.AddBit("QADCScheduledInd", rbQADCScheduledInd.SelectedIndex, 0, True)
                CmdAsst.AddDateTime("QADCLastModDate", txtQADCLastModDate.Text, True)
            End If

            QueryPack = CmdAsst.Execute

            MyResults.Success = QueryPack.Success
            If QueryPack.Success Then
                Sess.ActiveClientID = ddClientID.SelectedItem.Text
                Sess.ProcessName = txtProcessName.Text
                If Sess.ProcessTypeID = "Import" Then
                    Sess.DestinationID = 0
                Else
                    If ddDestinationID.Visible Then
                        Sess.DestinationID = ddDestinationID.SelectedItem.Value
                    Else
                        Sess.DestinationID = 0
                    End If
                End If
                MyResults.Msg = "Update complete."
            Else
                MyResults.Msg = "Unable to update record."
            End If
            Return MyResults

        Catch ex As Exception
            Throw New Exception("Error #405: Feedlist PerformSave " & ex.Message)
        End Try
    End Function

    Private Sub DisplayPage(ByVal ResponseAction As ResponseAction)
        Dim i As Integer
        Dim dt As DataTable
        Dim Sql As String
        Dim ClientValue As String
        Dim DestinationID As String
        Dim Working As String

        Try

            ' ___ Set the attributes for controls
            txtNotes.Attributes.Add("onkeyup", "return legallength(this, 100)")
            ddClientID.Attributes.Add("onchange", "ClientSelectionChanged()")

            Select Case ResponseAction
                Case ResponseAction.DisplayBlank, ResponseAction.DisplayUserInputNew
                    If Sess.ProcessTypeID = "Import" Then
                        litHeading.Text = "New Import"
                    Else
                        litHeading.Text = "New Export"
                    End If
                Case ResponseAction.DisplayExisting, ResponseAction.DisplayUserInputExisting
                    If Rights.HasThisRight(Rights.FeedlistEdit) Then
                        If Sess.ProcessTypeID = "Import" Then
                            litHeading.Text = "Edit Import"
                        Else
                            litHeading.Text = "Edit Export"
                        End If
                    Else
                        If Sess.ProcessTypeID = "Import" Then
                            litHeading.Text = "View Import"
                        Else
                            litHeading.Text = "View Export"
                        End If
                    End If
            End Select

        Catch ex As Exception
            Throw New Exception("Error #406a: Feedlist DisplayPage. " & ex.Message)
        End Try

        Try

            ' ___ Destination ID/Code
            If Sess.ProcessTypeID = "Export" Then
                If ddDestinationID.Items.Count > 0 Then
                    DestinationID = ddDestinationID.SelectedItem.Value
                    ddDestinationID.ClearSelection()
                    ddDestinationID.Items.Clear()
                End If

                If Not Page.IsPostBack Then
                    If Request.QueryString("CallType") = "Existing" Then
                        ClientValue = Sess.ActiveClientID
                    End If
                Else
                    If ddClientID.SelectedIndex > 0 Then
                        ClientValue = ddClientID.SelectedItem.Value
                    End If
                End If
            End If

        Catch ex As Exception
            Throw New Exception("Error #406b: Feedlist DisplayPage. " & ex.Message)
        End Try


        Try

            If ClientValue <> Nothing Then

                If Sess.ProcessTypeID = "Export" Then
                    If Common.DoesDatabaseAndTableExist(ClientValue, "ExportDestinationCodes") Then
                        'ddDestinationID.ClearSelection()
                        'ddDestinationID.Items.Clear()
                        Sql = "SELECT DestinationID, DestinationCode FROM " & ClientValue & "..ExportDestinationCodes ORDER BY DestinationCode"
                        dt = Common.GetDT(Sql)
                        ddDestinationID.Items.Add(New ListItem("", ""))
                        For i = 0 To dt.Rows.Count - 1
                            ddDestinationID.Items.Add(New ListItem(dt.Rows(i)("DestinationCode"), dt.Rows(i)("DestinationID")))
                            If ddDestinationID.Items(i).Value = DestinationID Then
                                ddDestinationID.SelectedIndex = i
                            End If
                        Next
                    End If
                End If
            End If

        Catch ex As Exception
            Throw New Exception("Error #406c: Feedlist DisplayPage. " & ex.Message)
        End Try

        Try

            If Not Page.IsPostBack Then

                ' ___ Client
                dt = Common.GetDT("SELECT ClientID FROM UserManagement..Codes_ClientID WHERE LogicalDelete = 0 ORDER BY ClientID")
                ddClientID.Items.Add(New ListItem("", ""))
                For i = 0 To dt.Rows.Count - 1
                    ddClientID.Items.Add(New ListItem(dt.Rows(i)(0), dt.Rows(i)(0)))
                Next

                ' ___ Carrier ID
                'dt = Common.GetDT("SELECT DISTINCT CarrierID FROM UserManagement..Codes_CarrierID ORDER BY CarrierID")
                dt = Common.GetDT("SELECT CarrierID FROM EXIM..Codes_CarrierID_Auxiliary UNION SELECT CarrierID FROM UserManagement..Codes_CarrierID ORDER BY CarrierID")
                ddCarrierID.Items.Add(New ListItem("", ""))
                For i = 0 To dt.Rows.Count - 1
                    ddCarrierID.Items.Add(New ListItem(dt.Rows(i)(0), dt.Rows(i)(0)))
                Next

                ' ___ Developer / Error Report Handler/QAPOC
                ' dt = Common.GetDT("SELECT LastName + ', ' + FirstName FROM UserManagement..Users WHERE Role = 'IT' ORDER BY LastName, FirstName")
                dt = Common.GetDT("SELECT UserID, LastName + ', ' + FirstName FROM UserManagement..Users WHERE Role = 'IT' ORDER BY LastName, FirstName")
                ddDeveloper.Items.Add(New ListItem("", ""))
                ddErrorReportHandler.Items.Add(New ListItem("", ""))
                ddQAPOC.Items.Add(New ListItem("", ""))
                For i = 0 To dt.Rows.Count - 1
                    ddDeveloper.Items.Add(New ListItem(dt.Rows(i)(1), dt.Rows(i)(0)))
                    ddErrorReportHandler.Items.Add(New ListItem(dt.Rows(i)(1), dt.Rows(i)(0)))
                    ddQAPOC.Items.Add(New ListItem(dt.Rows(i)(1), dt.Rows(i)(0)))
                Next

                ' ___ Frequency ID
                dt = Common.GetDT("SELECT FrequencyID FROM Codes_FrequencyID ORDER BY SortOrder")
                ddFrequencyID.Items.Add(New ListItem("", ""))
                For i = 0 To dt.Rows.Count - 1
                    ddFrequencyID.Items.Add(New ListItem(dt.Rows(i)(0), dt.Rows(i)(0)))
                Next

            End If

        Catch ex As Exception
            Throw New Exception("Error #406d: Feedlist DisplayPage. " & ex.Message)
        End Try

        Try


            ' ___ Format controls
            FormatControls()

            ' ___ Display record
            If ResponseAction = ResponseAction.DisplayExisting Then

                ' ___ Get the data
                dt = Common.GetDT("SELECT * FROM Feedlist WHERE ClientID = '" & Sess.ActiveClientID & "' AND ProcessName = '" & Sess.ProcessName & "' AND DestinationID = " & IIf(Sess.DestinationID = String.Empty, 0, Sess.DestinationID))

                ' ___ Clear the dropdown/radio list selections
                ddClientID.ClearSelection()
                ddCarrierID.ClearSelection()
                ddFrequencyID.ClearSelection()
                rbActiveInd.ClearSelection()
                rbScheduledInd.ClearSelection()
                rbReceiptInd.ClearSelection()
                rbBVIOrCarrierSendsErrorReportInd.ClearSelection()
                rbEmailAttachmentInd.ClearSelection()
                ddDeveloper.ClearSelection()
                ddErrorReportHandler.ClearSelection()
                ddQAPOC.ClearSelection()
                If Sess.ProcessTypeID = "Export" Then
                    ddDestinationID.ClearSelection()
                End If

                Try

                    ' ___ Version 1.2: make case insensitive
                    'ddClientID.Items.FindByValue(dt.Rows(0)("ClientID")).Selected = True
                    Working = dt.Rows(0)("ClientID").ToLower
                    For i = 0 To ddClientID.Items.Count - 1
                        If ddClientID.Items(i).Value.ToLower = Working Then
                            ddClientID.Items(i).Selected = True
                            Exit For
                        End If
                    Next

                Catch ex As Exception
                    Throw New Exception("Error #406e: Feedlist DisplayPage. " & ex.Message)
                End Try


                txtClientID.Text = Common.StrInHandler(dt.Rows(0)("ClientID"))

                txtProcessName.Text = Common.StrInHandler(dt.Rows(0)("ProcessName"))

                If Sess.ProcessTypeID = "Export" And ddDestinationID.Items.Count > 1 Then
                    Try
                        ddDestinationID.Items.FindByValue(dt.Rows(0)("DestinationID")).Selected = True
                    Catch ex As Exception
                        Throw New Exception("Error #406f: Feedlist DisplayPage. " & ex.Message)
                    End Try
                    txtDestinationID.Text = Common.NumInHandler(dt.Rows(0)("DestinationID"), False)
                Else
                    txtDestinationID.Text = "0"
                End If

                Try
                    ddCarrierID.Items.FindByValue(dt.Rows(0)("CarrierID")).Selected = True
                Catch ex As Exception
                    Throw New Exception("Error #406g: Feedlist DisplayPage. " & ex.Message)
                End Try
                txtCarrierID.Text = Common.StrInHandler(dt.Rows(0)("CarrierID"))

                Try
                    ddFrequencyID.Items.FindByValue(dt.Rows(0)("FrequencyID")).Selected = True
                Catch ex As Exception
                    Throw New Exception("Error #406h: Feedlist DisplayPage. " & ex.Message)
                End Try
                txtFrequencyID.Text = Common.StrInHandler(dt.Rows(0)("FrequencyID"))

                rbActiveInd.SelectedIndex = Common.BitToRadio(dt.Rows(0)("ActiveInd"), 0, True)
                txtActiveInd.Text = Common.BitToString(dt.Rows(0)("ActiveInd"), "Active", "Inactive", False)

                rbScheduledInd.SelectedIndex = Common.BitToRadio(dt.Rows(0)("ScheduledInd"), 0, True)
                txtScheduledInd.Text = Common.BitToString(dt.Rows(0)("ScheduledInd"), "Yes", "No", False)

                rbReceiptInd.SelectedIndex = Common.BitToRadio(dt.Rows(0)("ReceiptInd"), 0, True)
                txtReceiptInd.Text = Common.BitToString(dt.Rows(0)("ReceiptInd"), "Yes", "No", False)

                rbBVIOrCarrierSendsErrorReportInd.SelectedIndex = Common.BitToRadio(dt.Rows(0)("BVIOrCarrierSendsErrorReportInd"), 0, True)
                txtBVIOrCarrierSendsErrorReportInd.Text = Common.BitToString(dt.Rows(0)("BVIOrCarrierSendsErrorReportInd"), "Yes", "No", False)

                Try
                    ddErrorReportHandler.Items.FindByValue(Common.StrInHandler(dt.Rows(0)("ErrorReportHandler"))).Selected = True
                Catch ex As Exception
                    Throw New Exception("Error #406i: Feedlist DisplayPage. " & ex.Message)
                End Try
                txtErrorReportHandler.Text = Common.StrInHandler(dt.Rows(0)("ErrorReportHandler"))
                txtErrorReportSource.Text = Common.StrInHandler(dt.Rows(0)("ErrorReportSource"))

                txtFileName.Text = Common.StrInHandler(dt.Rows(0)("FileName"))
                txtFileLocation.Text = Common.StrInHandler(dt.Rows(0)("FileLocation"))
                txtFileDestLocation.Text = Common.StrInHandler(dt.Rows(0)("FileDestLocation"))

                txtEmailTo.Text = Common.StrInHandler(dt.Rows(0)("EmailTo"))

                rbEmailAttachmentInd.SelectedIndex = Common.BitToRadio(dt.Rows(0)("EmailAttachmentInd"), 0, True)
                txtEmailAttachmentInd.Text = Common.BitToString(dt.Rows(0)("EmailAttachmentInd"), "Yes", "No", False)

                txtFTPUserName.Text = Common.StrInHandler(dt.Rows(0)("FTPUserName"))
                txtFTPPassword.Text = Common.StrInHandler(dt.Rows(0)("FTPPassword"))

                txtPGPKey.Text = Common.StrInHandler(dt.Rows(0)("PGPKey"))
                txtNotes.Text = Common.StrInHandler(dt.Rows(0)("Notes"))

                Try
                    ddDeveloper.Items.FindByValue(dt.Rows(0)("Developer")).Selected = True
                Catch ex As Exception
                    Throw New Exception("Error #406j: Feedlist DisplayPage. " & ex.Message)
                End Try
                txtDeveloper.Text = Common.StrInHandler(dt.Rows(0)("Developer"))

                If Rights.HasThisRight(Rights.QAFieldView) Then
                    ddQAPOC.Items.FindByValue(Common.StrInHandler(dt.Rows(0)("QAPOC"))).Selected = True
                    txtQAPOC.Text = Common.StrInHandler(dt.Rows(0)("QAPOC"))
                    txtQAStatus.Text = Common.StrInHandler(dt.Rows(0)("QAStatus"))
                    txtQANotes.Text = Common.StrInHandler(dt.Rows(0)("QANotes"))
                    rbQADCScheduledInd.SelectedIndex = Common.BitToRadio(dt.Rows(0)("QADCScheduledInd"), 0, True)
                    txtQADCScheduledInd.Text = Common.BitToString(dt.Rows(0)("QADCScheduledInd"), "Yes", "No", True)
                    txtQADCLastModDate.Text = Common.DateInHandler(dt.Rows(0)("QADCLastModDate"))
                End If

            End If

            If phQASection.Visible AndAlso lblQADCLastModDateLink.Visible Then
                litCalendar.Text = "<script language='javascript'>var QADCLastModDate = new calendar2(document.forms['form1'].elements['txtQADCLastModDate']);QADCLastModDate.year_scroll = true; QADCLastModDate.time_comp = false;</script>"
            End If

            'sbHiddens.Append("<input type='hidden' id='hdProcessTypeID' name='hdProcessTypeID' value='" & cProcessTypeID & "'>")
            'litHiddens.Text = sbHiddens.ToString

        Catch ex As Exception
            Throw New Exception("Error #406k: Feedlist DisplayPage. " & ex.Message)
        End Try

    End Sub

    Private Sub PopulateDestinationCodes()
        Dim i As Integer
        Dim dt As DataTable
        Dim Sql As String

        If Common.DoesTableExist(ddClientID.SelectedItem.Value, "ExportDestinationCodes") Then
            ddDestinationID.Items.Clear()
            Sql = "SELECT DestinationID, DestinationCode FROM " & ddClientID.SelectedItem.Value & "..ExportDestinationCodes ORDER BY DestinationCode"
            dt = Common.GetDT(Sql)
            ddDestinationID.Items.Add(New ListItem("", ""))
            For i = 0 To dt.Rows.Count - 1
                ddDestinationID.Items.Add(New ListItem(dt.Rows(i)("DestinationCode"), dt.Rows(i)("DestinationID")))
            Next
        End If
    End Sub

    Private Sub FormatControls()

        ' ___ Show/hide QA section
        If Rights.HasThisRight(Rights.QAFieldView) Then
            phQASection.Visible = True
            '   litCalendar.Text = "<script language='javascript'>var QADCLastModDate = new calendar2(document.forms['form1'].elements['txtQADCLastModDate']);QADCLastModDate.year_scroll = true; QADCLastModDate.time_comp = false;</script>"
        Else
            phQASection.Visible = False
        End If

        ' ___ Show/hide DestinationID
        'If Sess.ProcessTypeID = "Export" Then
        '    'If ddClientID.SelectedIndex > 0 AndAlso Common.DoesDatabaseAndTableExist(ddClientID.SelectedValue, "ExportDestinationCodes") Then
        '    If Common.DoesDatabaseAndTableExist(ddClientID.SelectedValue, "ExportDestinationCodes") Then
        '        phExport.Visible = True
        '    Else
        '        phExport.Visible = False
        '    End If
        'Else
        '    phExport.Visible = False
        'End If

        ' ___  lblBVIOrCarrierSendsErrorReport
        If Sess.ProcessTypeID = "Export" Then
            lblBVIOrCarrierSendsErrorReport.Text = "Carrier Sends Error Report:"
        Else
            lblBVIOrCarrierSendsErrorReport.Text = "BVI Sends Error Report"
        End If

        If Rights.HasThisRight(Rights.FeedlistEdit) Then
            litUpdate.Text = "<input onclick='Update()' type='button' value='Update'>"

            ddClientID.Visible = True
            Style.AddStyle(txtClientID, Style.StyleType.NotVisible, 300)

            Style.AddStyle(txtProcessName, Style.StyleType.NormalEditable, 300)
            Style.AddStyle(txtDestinationID, Style.StyleType.NormalEditable, 300)

            ddCarrierID.Visible = True
            Style.AddStyle(txtCarrierID, Style.StyleType.NotVisible, 300)

            ddFrequencyID.Visible = True
            Style.AddStyle(txtFrequencyID, Style.StyleType.NotVisible, 300)

            rbActiveInd.Visible = True
            Style.AddStyle(txtActiveInd, Style.StyleType.NotVisible, 300)

            rbScheduledInd.Visible = True
            Style.AddStyle(txtScheduledInd, Style.StyleType.NotVisible, 300)

            rbReceiptInd.Visible = True
            Style.AddStyle(txtReceiptInd, Style.StyleType.NotVisible, 300)

            rbBVIOrCarrierSendsErrorReportInd.Visible = True
            Style.AddStyle(txtBVIOrCarrierSendsErrorReportInd, Style.StyleType.NotVisible, 300)

            Style.AddStyle(txtFileName, Style.StyleType.NormalEditable, 300)
            Style.AddStyle(txtFileLocation, Style.StyleType.NormalEditable, 300)
            Style.AddStyle(txtFileDestLocation, Style.StyleType.NormalEditable, 300)
            Style.AddStyle(txtEmailTo, Style.StyleType.NormalEditable, 300)

            rbEmailAttachmentInd.Visible = True
            Style.AddStyle(txtEmailAttachmentInd, Style.StyleType.NotVisible, 300)

            Style.AddStyle(txtFTPUserName, Style.StyleType.NormalEditable, 300)
            Style.AddStyle(txtFTPPassword, Style.StyleType.NormalEditable, 300)
            Style.AddStyle(txtPGPKey, Style.StyleType.NormalEditable, 300)
            Style.AddStyle(txtNotes, Style.StyleType.NormalEditable, 300, True)

            ddDeveloper.Visible = True
            txtDeveloper.Visible = False

            ddErrorReportHandler.Visible = True
            txtErrorReportHandler.Visible = False

            Style.AddStyle(txtErrorReportSource, Style.StyleType.NormalEditable, 300)

            If Sess.ProcessTypeID = "Export" Then
                phExport.Visible = True
                ddDestinationID.Visible = True
                txtDestinationID.Visible = False
            Else
                phExport.Visible = False
            End If

            'If Sess.ProcessTypeID = "Export" Then
            '    ddDestinationID.Visible = True
            '    txtDestinationID.Visible = False
            'Else
            '    ddDestinationID.Visible = False
            '    txtDestinationID.Visible = False
            'End If

            If Rights.HasThisRight(Rights.QAFieldEdit) Then
                ddQAPOC.Visible = True
                txtQAPOC.Visible = False
                Style.AddStyle(txtQAStatus, Style.StyleType.NormalEditable, 300)
                Style.AddStyle(txtQANotes, Style.StyleType.NormalEditable, 300)
                rbQADCScheduledInd.Visible = True
                txtQADCScheduledInd.Visible = False
                Style.AddStyle(txtQADCLastModDate, Style.StyleType.NoneditableWhite, 300, True)
                lblQADCLastModDateLink.Visible = True
            End If

        Else

            ddClientID.Visible = False
            Style.AddStyle(txtClientID, Style.StyleType.NoneditableGrayed, 300)

            Style.AddStyle(txtProcessName, Style.StyleType.NoneditableGrayed, 300)
            Style.AddStyle(txtDestinationID, Style.StyleType.NoneditableGrayed, 300)

            ddCarrierID.Visible = False
            Style.AddStyle(txtCarrierID, Style.StyleType.NoneditableGrayed, 300)

            ddFrequencyID.Visible = False
            Style.AddStyle(txtFrequencyID, Style.StyleType.NoneditableGrayed, 300)

            rbActiveInd.Visible = False
            Style.AddStyle(txtActiveInd, Style.StyleType.NoneditableGrayed, 300)

            rbScheduledInd.Visible = False
            Style.AddStyle(txtScheduledInd, Style.StyleType.NoneditableGrayed, 300)

            rbReceiptInd.Visible = False
            Style.AddStyle(txtReceiptInd, Style.StyleType.NoneditableGrayed, 300)

            rbBVIOrCarrierSendsErrorReportInd.Visible = False
            Style.AddStyle(txtBVIOrCarrierSendsErrorReportInd, Style.StyleType.NoneditableGrayed, 300)

            Style.AddStyle(txtFileName, Style.StyleType.NoneditableGrayed, 300)
            Style.AddStyle(txtFileLocation, Style.StyleType.NoneditableGrayed, 300)
            Style.AddStyle(txtFileDestLocation, Style.StyleType.NoneditableGrayed, 300)
            Style.AddStyle(txtEmailTo, Style.StyleType.NoneditableGrayed, 300)
            rbEmailAttachmentInd.Visible = False

            Style.AddStyle(txtEmailAttachmentInd, Style.StyleType.NoneditableGrayed, 300)
            Style.AddStyle(txtFTPUserName, Style.StyleType.NoneditableGrayed, 300)
            Style.AddStyle(txtFTPPassword, Style.StyleType.NoneditableGrayed, 300)
            Style.AddStyle(txtPGPKey, Style.StyleType.NoneditableGrayed, 300)
            Style.AddStyle(txtNotes, Style.StyleType.NoneditableGrayed, 300, True)

            ddDeveloper.Visible = False
            Style.AddStyle(txtDeveloper, Style.StyleType.NoneditableGrayed, 300, True)

            ddErrorReportHandler.Visible = False
            Style.AddStyle(txtErrorReportHandler, Style.StyleType.NoneditableGrayed, 300, True)
            Style.AddStyle(txtErrorReportSource, Style.StyleType.NoneditableGrayed, 300)

            If Sess.ProcessTypeID = "Export" Then
                ddDestinationID.Visible = False
                Style.AddStyle(txtDestinationID, Style.StyleType.NoneditableGrayed, 300)
            Else
                ddDestinationID.Visible = False
                txtDestinationID.Visible = False
            End If

            If Rights.HasThisRight(Rights.QAFieldEdit) Then
                ddQAPOC.Visible = False
                Style.AddStyle(txtQAPOC, Style.StyleType.NoneditableGrayed, 300, True)
                Style.AddStyle(txtQAStatus, Style.StyleType.NoneditableGrayed, 300)
                Style.AddStyle(txtQANotes, Style.StyleType.NoneditableGrayed, 300)
                rbQADCScheduledInd.Visible = False
                Style.AddStyle(txtQADCScheduledInd, Style.StyleType.NoneditableGrayed, 300)
                Style.AddStyle(txtQADCLastModDate, Style.StyleType.NoneditableGrayed, 300, True)
                lblQADCLastModDateLink.Visible = False
            End If

        End If

    End Sub
End Class
