Public Class LogMaintain
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
    Protected WithEvents litHeading As System.Web.UI.WebControls.Literal
    Protected WithEvents litUpdate As System.Web.UI.WebControls.Literal
    Protected WithEvents txtFileID As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtFilePostedBy As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtFileReceiptReceivedDate As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtFileReceiptReceivedBy As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtErrorReportDoneBy As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtrowguid As System.Web.UI.WebControls.TextBox
    Protected WithEvents lblFilePostedDateLink As System.Web.UI.WebControls.Label
    Protected WithEvents txtFilePostedDate As System.Web.UI.WebControls.TextBox
    Protected WithEvents lblFileReceiptReceivedDateLink As System.Web.UI.WebControls.Label
    Protected WithEvents lblErrorReportDoneDateLink As System.Web.UI.WebControls.Label
    Protected WithEvents ddFilePostedBy As System.Web.UI.WebControls.DropDownList
    Protected WithEvents ddFileReceiptReceivedBy As System.Web.UI.WebControls.DropDownList
    Protected WithEvents txtErrorReportDoneDate As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtExpFileSendDate As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtExpDestinationCode As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtExpFileName As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtExpFileLocation As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtExpCompleteDatetime As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtExpExtractDateFrom As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtExpExtractDateTo As System.Web.UI.WebControls.TextBox
    Protected WithEvents ExportPanel As System.Web.UI.WebControls.PlaceHolder
    Protected WithEvents txtImpFileReceiveDate As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtImpFileTypeCode As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtImpFileLocation As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtImpFileImportDate As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtClientID As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtProcessTypeID As System.Web.UI.WebControls.TextBox
    Protected WithEvents ImportPanel As System.Web.UI.WebControls.PlaceHolder
    Protected WithEvents ddErrorReportDoneBy As System.Web.UI.WebControls.DropDownList
    Protected WithEvents litServerTime As System.Web.UI.WebControls.Literal
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


    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim Results As New Results
        Dim RequestAction As RequestAction
        Dim ResponseAction As ResponseAction

        Try

            Common = Session("Common")

            ' ___ Restore  session
            cLoggedInUserID = AppSession.RestoreSession(Page)

            ' ___ Right Check
            Dim RightsRqd(0) As String
            Rights = New RightsClass(cLoggedInUserID, Page)
            RightsRqd.SetValue(Rights.LogView, 0)
            Rights.HasSufficientRights(RightsRqd, True, Page)
            lblCurrentRights.Text = Common.GetCurRightsHidden(Rights.RightsColl)

            ' ___ Get the session object
            Sess = Session("LogWorklistSession")

            ' ___ Get RequestAction
            RequestAction = Common.GetRequestAction(Page)

            ' ___ Execute the RequestAction
            Results = ExecuteRequestAction(RequestAction)
            ResponseAction = Results.ResponseAction

            ' ___ Execute the ResponseAction
            If ResponseAction = ResponseAction.ReturnToCallingPage Then
                Sess.PageReturnOnLoadMessage = Results.Msg
                Response.Redirect("LogWorklist.aspx?CalledBy=Child")
            Else
                DisplayPage(ResponseAction)
                If Not Results.Msg = Nothing Then
                    litMsg.Text = "<script language='javascript'>alert('" & Results.Msg & "')</script>"
                End If
                litResponseAction.Text = "<input type='hidden' name='hdResponseAction' value = '" & ResponseAction.ToString & "'>"
                litServerTime.Text = "<input type='hidden' name='hdServerTime' value='" & Common.GetServerDateTime & "'>"
            End If

            ' ___ Display enviroment
            PageCaption.InnerHtml = Common.GetPageCaption
            litEnviro.Text = "<input type='hidden' name='hdLoggedInUserID' value='" & cLoggedInUserID & "'><input type='hidden' name='hdDBHost'  value='hbg-sql'>"

        Catch ex As Exception
            Throw New Exception("Error #202: LogMaintain Page_Load. " & ex.Message)
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
            Throw New Exception("Error #203: LogMaintain ExecuteRequestAction. " & ex.Message)
        End Try
    End Function

    Private Function IsDataValid(ByVal RequestAction As RequestAction) As Results
        Dim MyResults As New Results
        Dim dt As DataTable
        Dim ErrColl As New Collection

        Try

            If ddFilePostedBy.SelectedIndex > 0 AndAlso txtFilePostedDate.Text.Length = 0 Then
                Common.ValidateErrorOnly(ErrColl, "file posted by requires file posted date")
            ElseIf ddFilePostedBy.SelectedIndex < 1 AndAlso txtFilePostedDate.Text.Length > 0 Then
                Common.ValidateErrorOnly(ErrColl, "file posted date requires file posted by")
            End If

            If ddFileReceiptReceivedBy.SelectedIndex > 0 AndAlso txtFileReceiptReceivedDate.Text.Length = 0 Then
                Common.ValidateErrorOnly(ErrColl, "file receipt received by requires file receipt received date")
            ElseIf ddFileReceiptReceivedBy.SelectedIndex < 1 AndAlso txtFileReceiptReceivedDate.Text.Length > 0 Then
                Common.ValidateErrorOnly(ErrColl, "file receipt received date requires file receipt received by")
            End If

            If ddErrorReportDoneBy.SelectedIndex > 0 AndAlso txtErrorReportDoneDate.Text.Length = 0 Then
                Common.ValidateErrorOnly(ErrColl, "error report done by requires error report done date")
            ElseIf ddErrorReportDoneBy.SelectedIndex < 1 AndAlso txtErrorReportDoneDate.Text.Length > 0 Then
                Common.ValidateErrorOnly(ErrColl, "error report done date requires error report done by")
            End If

            If ErrColl.Count = 0 Then
                MyResults.Success = True
            Else
                MyResults.Success = False
                MyResults.Msg = Common.ErrCollToString(ErrColl, "Not saved. Please correct the following:")
            End If
            Return MyResults

        Catch ex As Exception
            Throw New Exception("Error #204: LogMaintain IsDataValid. " & ex.Message)
        End Try
    End Function

    Private Function PerformSave(ByVal RequestAction As RequestAction) As Results
        Dim MyResults As New Results
        Dim QueryPack As CmdAsst.QueryPack
        Dim Sql As New System.Text.StringBuilder
        Dim SourceTable As String

        Try

            SourceTable = GetClientID() & ".." & Sess.ActiveProcessTypeID & "FileHistory"

            Sql.Append("UPDATE " & SourceTable & " ")
            Sql.Append("SET ")
            Sql.Append("FilePostedDate = " & Common.DateOutHandler(txtFilePostedDate.Text, True, True) & ", ")
            Sql.Append("FilePostedBy = " & Common.StrOutHandler(ddFilePostedBy.SelectedValue, True, True) & ", ")
            Sql.Append("FileReceiptReceivedDate = " & Common.DateOutHandler(txtFileReceiptReceivedDate.Text, True, True) & ", ")
            Sql.Append("FileReceiptReceivedBy = " & Common.StrOutHandler(ddFileReceiptReceivedBy.SelectedValue, True, True) & ", ")
            Sql.Append("ErrorReportDoneDate = " & Common.DateOutHandler(txtErrorReportDoneDate.Text, True, True) & ", ")
            Sql.Append("ErrorReportDoneBy = " & Common.StrOutHandler(ddErrorReportDoneBy.SelectedValue, True, True) & " ")
            Sql.Append("WHERE FileID = '" & txtFileID.Text & "' ")

            QueryPack = Common.ExecuteNonQueryWithQueryPack(Sql.ToString)
            MyResults.Success = QueryPack.Success
            If QueryPack.Success Then
                MyResults.Msg = "Update complete."
            Else
                MyResults.Msg = "Unable to update record."
            End If
            Return MyResults

        Catch ex As Exception
            Throw New Exception("Error #205: LogMaintain PerformSave. " & ex.Message)
        End Try
    End Function

    Private Sub DisplayPage(ByVal ResponseAction As ResponseAction)
        Dim dt As DataTable
        Dim Sql As String
        Dim i, j As Integer
        Dim li As ListItem
        Dim FilePostedDate As DateTime
        Dim DBName As String
        Dim Tablename As String
        Dim EDCTableExists As Boolean
        Dim Style As String

        Try

            Try

                ' ___ Heading
                Select Case ResponseAction
                    Case ResponseAction.DisplayBlank
                        litHeading.Text = "New Log"
                    Case ResponseAction.DisplayUserInputNew
                        litHeading.Text = "New Log"
                    Case ResponseAction.DisplayExisting
                        If Rights.HasThisRight(Rights.LogEdit) Then
                            litHeading.Text = "Edit Log"
                        Else
                            litHeading.Text = "View Log"
                        End If
                End Select

                ' ___ View/Edit
                Select Case Sess.ActiveProcessTypeID
                    Case "Import"
                        ImportPanel.Visible = True
                        ExportPanel.Visible = False
                    Case "Export"
                        ImportPanel.Visible = False
                        ExportPanel.Visible = True
                End Select

                ' ___ Format controls
                FormatControls()

            Catch ex As Exception
                Throw New Exception("Error #206a: LogMaintain DisplayPage. " & ex.Message)
            End Try

            Try

                ' If Not Page.IsPostBack AndAlso Rights.HasThisRight(Rights.LogEdit) Then
                If Not Page.IsPostBack Then

                    dt = Common.GetDT("SELECT UserID, Name = LastName + ', ' + FirstName FROM UserManagement..Users WHERE Role = 'ADMIN' OR Role = 'ADMIN LIC' OR Role = 'IT' ORDER BY LastName, FirstName")

                    ddFilePostedBy.Items.Add(New ListItem("", ""))
                    li = New ListItem
                    li.Value = "Automail"
                    li.Text = "Automail"
                    ddFilePostedBy.Items.Add(li)
                    For i = 0 To dt.Rows.Count - 1
                        li = New ListItem
                        li.Value = dt.Rows(i)("UserID")
                        li.Text = dt.Rows(i)("Name")
                        ddFilePostedBy.Items.Add(li)
                    Next

                    ddFileReceiptReceivedBy.Items.Add(New ListItem("", ""))
                    For i = 0 To dt.Rows.Count - 1
                        li = New ListItem
                        li.Value = dt.Rows(i)("UserID")
                        li.Text = dt.Rows(i)("Name")
                        ddFileReceiptReceivedBy.Items.Add(li)
                    Next

                    ddErrorReportDoneBy.Items.Add(New ListItem("", ""))
                    For i = 0 To dt.Rows.Count - 1
                        li = New ListItem
                        li.Value = dt.Rows(i)("UserID")
                        li.Text = dt.Rows(i)("Name")
                        ddErrorReportDoneBy.Items.Add(li)
                    Next

                End If

            Catch ex As Exception
                Throw New Exception("Error #206b: LogMaintain DisplayPage. " & ex.Message)
            End Try

            Try

                If ResponseAction = ResponseAction.DisplayExisting Then

                    ' ___ Get the data
                    DBName = GetClientID()
                    Tablename = Sess.ActiveProcessTypeID & "FileHistory"

                    If Sess.ActiveProcessTypeID = "Import" Then
                        Sql = "SELECT * FROM " & DBName & ".." & Tablename & " WHERE FileID = " & Sess.FileId
                    Else
                        If Common.DoesTableExist(DBName, Tablename) Then
                            EDCTableExists = True
                            Sql = "SELECT efh.*, edc.DestinationCode FROM " & DBName & ".." & Tablename & " efh "
                            Sql &= "INNER JOIN " & DBName & "..ExportDestinationCodes edc on efh.DestinationID = edc.DestinationID "
                            Sql &= "WHERE FileID = " & Sess.FileId
                        Else
                            Sql = "SELECT * FROM " & DBName & ".." & Tablename & " WHERE FileID = " & Sess.FileId
                        End If
                    End If

                    Try
                        dt = Common.GetDT(Sql)
                    Catch ex As Exception
                        Throw New Exception("Error #206c: LogMaintain DisplayPage. " & ex.Message)
                    End Try

                    ' ___ Clear the dropdown selections
                    ddFilePostedBy.ClearSelection()
                    ddFileReceiptReceivedBy.ClearSelection()
                    ddErrorReportDoneBy.ClearSelection()

                    txtClientID.Text = GetClientID()
                    txtProcessTypeID.Text = Sess.ActiveProcessTypeID
                    txtFileID.Text = Common.StrInHandler(dt.Rows(0)("FileID"))
                    GetText(txtrowguid, dt, "rowguid", "guid")

                    '?date.now.ToString("yyyy:MM:dd:HH:mm:ss:fff")
                    '"2008:05:09:18:57:57:848"

                    If IsDBNull(dt.Rows(0)("FilePostedDate")) Then
                        txtFilePostedDate.Text = String.Empty
                    Else
                        FilePostedDate = dt.Rows(0)("FilePostedDate")
                        txtFilePostedDate.Text = FilePostedDate.ToString("MM/dd/yyyy HH:mm:ss")
                    End If

                    txtFilePostedBy.Text = Common.StrInHandler(dt.Rows(0)("FilePostedBy"))

                    Try
                        ddFilePostedBy.Items.FindByValue(Common.StrInHandler(dt.Rows(0)("FilePostedBy"))).Selected = True
                    Catch ex As Exception
                        Throw New Exception("Error #206d: LogMaintain DisplayPage. " & ex.Message)
                    End Try


                    txtFileReceiptReceivedDate.Text = Common.StrInHandler(dt.Rows(0)("FileReceiptReceivedDate"))
                    txtFileReceiptReceivedBy.Text = Common.StrInHandler(dt.Rows(0)("FileReceiptReceivedBy"))

                    Try
                        ddFileReceiptReceivedBy.Items.FindByValue(Common.StrInHandler(dt.Rows(0)("FileReceiptReceivedBy"))).Selected = True
                    Catch ex As Exception
                        Throw New Exception("Error #206e: LogMaintain DisplayPage. " & ex.Message)
                    End Try

                    txtErrorReportDoneDate.Text = Common.StrInHandler(dt.Rows(0)("ErrorReportDoneDate"))
                    txtErrorReportDoneBy.Text = Common.StrInHandler(dt.Rows(0)("ErrorReportDoneBy"))
                    ddErrorReportDoneBy.Items.FindByValue(Common.StrInHandler(dt.Rows(0)("ErrorReportDoneBy"))).Selected = True

                    Select Case Sess.ActiveProcessTypeID
                        Case "Import"
                            GetText(txtImpFileReceiveDate, dt, "FileReceiveDate", "date")
                            GetText(txtImpFileTypeCode, dt, "FileTypeCode", "str")
                            GetText(txtImpFileLocation, dt, "FileLocation", "str")
                            GetText(txtImpFileImportDate, dt, "FileImportDate", "date")

                        Case "Export"
                            GetText(txtExpFileSendDate, dt, "FileSendDate", "date")
                            If EDCTableExists Then
                                GetText(txtExpDestinationCode, dt, "DestinationCode", "str")
                            Else
                                txtExpDestinationCode.ForeColor = Color.Red
                                Style = txtExpDestinationCode.Attributes("style")
                                Style &= "color:red;"
                                txtExpDestinationCode.Attributes("style") = Style
                                txtExpDestinationCode.Text = "This database lacks the " & Tablename & " table"

                            End If
                            GetText(txtExpFileName, dt, "FileName", "str")
                            GetText(txtExpFileLocation, dt, "FileLocation", "str")
                            GetText(txtExpCompleteDatetime, dt, "CompleteDateTime", "date")
                            GetText(txtExpExtractDateFrom, dt, "ExtractDateFrom", "date")
                            GetText(txtExpExtractDateTo, dt, "ExtractDateTo", "date")
                    End Select

                End If

            Catch ex As Exception
                Throw New Exception("Error #206f: LogMaintain DisplayPage. " & ex.Message)
            End Try

        Catch ex As Exception
            Throw New Exception("Error #206g: LogMaintain DisplayPage. " & ex.Message)
            End Try
    End Sub

    Private Sub GetText(ByRef tb As TextBox, ByRef dt As DataTable, ByVal FieldName As String, ByVal DataType As String)
        Dim Style As String

        Try
            'Dim Product As DataColumn = New DataColumn("Product")
            'Product.DataType = System.Type.GetType("System.String")

            Dim i As Integer
            For i = 0 To dt.Columns.Count - 1
                If dt.Columns(i).ColumnName.ToLower = FieldName.ToLower Then
                    Select Case DataType
                        Case "str"
                            tb.Text = Common.DateInHandler(dt.Rows(0)(FieldName))
                        Case "date"
                            tb.Text = Common.DateInHandler(dt.Rows(0)(FieldName))
                        Case "int"
                            tb.Text = Common.NumInHandler(dt.Rows(0)(FieldName), False)
                        Case "guid"
                            tb.Text = Common.GuidInHandler(dt.Rows(0)(FieldName))
                    End Select
                    Exit For
                Else
                End If
            Next
            If i = dt.Columns.Count Then
                tb.ForeColor = Color.Red
                Style = tb.Attributes("style")
                Style &= "color:red;"
                tb.Attributes("style") = Style
                tb.Text = "This table lacks " & FieldName & " field"
            End If

        Catch ex As Exception
            Throw New Exception("Error #207: LogMaintain GetText. " & ex.Message)
        End Try
    End Sub

    Private Function GetClientID() As String
        Dim Box As Object

        Try
            Box = Split(Sess.ActiveClientValue, "|")
            Return Box(0)
        Catch ex As Exception
            Throw New Exception("Error #208: LogMaintain GetClientID. " & ex.Message)
        End Try
    End Function

    Private Sub FormatControls()
        Try

            ' ___ Common
            Style.AddStyle(txtClientID, Style.StyleType.NoneditableGrayed, 250)
            Style.AddStyle(txtProcessTypeID, Style.StyleType.NoneditableGrayed, 250)
            Style.AddStyle(txtFileID, Style.StyleType.NoneditableGrayed, 250)
            Style.AddStyle(txtrowguid, Style.StyleType.NoneditableGrayed, 250)

            ' ___ Export
            Style.AddStyle(txtExpFileSendDate, Style.StyleType.NoneditableGrayed, 300)
            Style.AddStyle(txtExpDestinationCode, Style.StyleType.NoneditableGrayed, 300)
            Style.AddStyle(txtExpFileName, Style.StyleType.NoneditableGrayed, 300)
            Style.AddStyle(txtExpFileLocation, Style.StyleType.NoneditableGrayed, 450)
            Style.AddStyle(txtExpCompleteDatetime, Style.StyleType.NoneditableGrayed, 300)
            Style.AddStyle(txtExpExtractDateFrom, Style.StyleType.NoneditableGrayed, 300)
            Style.AddStyle(txtExpExtractDateTo, Style.StyleType.NoneditableGrayed, 300)

            ' ___ Import
            Style.AddStyle(txtImpFileReceiveDate, Style.StyleType.NoneditableGrayed, 300)
            Style.AddStyle(txtImpFileTypeCode, Style.StyleType.NoneditableGrayed, 300)
            Style.AddStyle(txtImpFileLocation, Style.StyleType.NoneditableGrayed, 450)
            Style.AddStyle(txtImpFileImportDate, Style.StyleType.NoneditableGrayed, 300)

            If Rights.HasThisRight(Rights.FeedlistEdit) Then
                litUpdate.Text = "<input onclick='Update()' type='button' value='Update'>"

                'Calendar1.Visible = False
                'lblYearUpDown.Visible = False
                lblFilePostedDateLink.Visible = True
                lblFileReceiptReceivedDateLink.Visible = True
                lblErrorReportDoneDateLink.Visible = True

                Style.AddStyle(txtFilePostedDate, Style.StyleType.NoneditableWhite, 148)
                ddFilePostedBy.Visible = True
                Style.AddStyle(txtFilePostedBy, Style.StyleType.NotVisible, 300)

                Style.AddStyle(txtFileReceiptReceivedDate, Style.StyleType.NoneditableWhite, 112)
                ddFileReceiptReceivedBy.Visible = True
                Style.AddStyle(txtFileReceiptReceivedBy, Style.StyleType.NotVisible, 300)

                Style.AddStyle(txtErrorReportDoneDate, Style.StyleType.NoneditableWhite, 112)
                ddErrorReportDoneBy.Visible = True
                Style.AddStyle(txtErrorReportDoneBy, Style.StyleType.NotVisible, 300)

            Else

                'Calendar1.Visible = False
                'lblYearUpDown.Visible = False
                lblFilePostedDateLink.Visible = False
                lblFileReceiptReceivedDateLink.Visible = False
                lblErrorReportDoneDateLink.Visible = False

                Style.AddStyle(txtFilePostedDate, Style.StyleType.NoneditableGrayed, 148)
                ddFilePostedBy.Visible = False
                Style.AddStyle(txtFilePostedBy, Style.StyleType.NoneditableGrayed, 300)

                Style.AddStyle(txtFileReceiptReceivedDate, Style.StyleType.NoneditableGrayed, 112)
                ddFileReceiptReceivedBy.Visible = False
                Style.AddStyle(txtFileReceiptReceivedBy, Style.StyleType.NoneditableGrayed, 300)

                Style.AddStyle(txtErrorReportDoneDate, Style.StyleType.NoneditableGrayed, 112)
                ddErrorReportDoneBy.Visible = False
                Style.AddStyle(txtErrorReportDoneBy, Style.StyleType.NoneditableGrayed, 300)

            End If

        Catch ex As Exception
            Throw New Exception("Error #209: LogMaintain FormatControls. " & ex.Message)
        End Try
    End Sub
End Class