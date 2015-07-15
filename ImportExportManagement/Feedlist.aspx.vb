Imports System.Data.SqlClient

Public Class Feedlist
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
    Private AppSession As New AppSession
    Private Common As Common
    Private cLoggedInUserID As String
    'Private cCarrierID
    Protected WithEvents lblEnrollerName As System.Web.UI.WebControls.Label
    'Private cSubTableInd As String = "0"

    'Private cSubTableClientID As String
    'Private cSubTableProcessName As String
    'Private cSubTableDestinationID As String

    'Private cActiveClientID As String
    'Private cProcessName As String
    'Private cDestinationID As String
    'Private cStartDate As String

    'Private cClientIDSelectedFilterValue As String
    'Private cProcessTypeIDSelectedFilterValue As String
    'Private cCarrierIDSelectedFilterValue As String
    'Private cDeveloperSelectedFilterValue As String

    'Private cFilterOnResponse As String
    'Private cFilterShowHideToggle As String

    Protected WithEvents litHiddens As System.Web.UI.WebControls.Literal

    Private DG As DG
    Private DGSchedule As DG
    Protected WithEvents litEnviro As System.Web.UI.WebControls.Literal

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

    ' ___ Event raised by the data grid.
    Public Sub HandleChildDTRequest(ByRef ChildText As String, ByVal DataFldName As String, ByVal Value As String)
        Dim dt As DataTable

        Try
            dt = Common.GetDT("SELECT * FROM FeedSchedule WHERE ClientID = '" & Sess.SubTableClientID & "' AND ProcessName = '" & Sess.SubTableProcessName & "' AND DestinationID = '" & Sess.SubTableDestinationID & "'")
            ChildText = DGSchedule.GetChildTableText(dt)
            Sess.SubTableInd = "1"
        Catch ex As Exception
            Throw New Exception("Error #308: Feedlist HandleChildDTRequest. " & ex.Message)
        End Try
    End Sub

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim PageMode As PageMode
        Dim Results As Results
        Dim Action As String

        Try

            Common = Session("Common")

            ' ___ Initialize the app session
            cLoggedInUserID = AppSession.Init(Page)

            ' ___ Right Check
            Dim RightsRqd(0) As String
            Rights = New RightsClass(cLoggedInUserID, Page)
            RightsRqd.SetValue(Rights.FeedlistView, 0)
            Rights.HasSufficientRights(RightsRqd, True, Page)
            lblCurrentRights.Text = Common.GetCurRightsHidden(Rights.RightsColl)

            ' ___ Get the page session object
            Sess = Session("FeedlistSession")

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
            DG = DefineDataGrid("Feedlist")
            AddHandler DG.ChildDTRequest, AddressOf HandleChildDTRequest
            DGSchedule = DefineDataGrid("Schedule")

            If Sess.SubTableInd = "1" Then
                DG.ChildTables.ChildTableSelectColumn.ParmColl("DataFldName").Value = Sess.SubTableClientID
                DG.ChildTables.ChildTableSelectColumn.ParmColl("Parm2").Value = Sess.SubTableProcessName
                DG.ChildTables.ChildTableSelectColumn.ParmColl("Parm3").Value = Sess.SubTableDestinationID
                DG.ChildTables.ChildTableSelectColumn.ParmColl("DataFldName").Value = Sess.SubTableClientID
                DG.ChildTables.ChildTableSelectColumn.ParmColl("Parm2").Value = Sess.SubTableProcessName
                DG.ChildTables.ChildTableSelectColumn.ParmColl("Parm3").Value = Sess.SubTableDestinationID
            End If

            ' ___ Execute action
            Select Case PageMode
                Case PageMode.Initial
                    DisplayPage(PageMode, DG, DG.OrderByType.Initial)

                Case PageMode.Postback
                    Action = Request.Form("hdAction")
                    Select Case Action
                        Case "Sort"
                            DisplayPage(PageMode, DG, DG.OrderByType.Field, Request.Form("hdSortField"))

                        Case "ApplyFilter"
                            DisplayPage(PageMode, DG, DG.OrderByType.Recurring)

                        Case "DeleteFeed"
                            Results = DeleteFeed()
                            DisplayPage(PageMode, DG, DG.OrderByType.Recurring)
                            litMsg.Text = "<script language='javascript'>alert('" & Results.Msg & "')</script>"

                        Case "DeleteSchedule"
                            Results = DeleteSchedule()
                            DisplayPage(PageMode, DG, DG.OrderByType.Recurring)
                            litMsg.Text = "<script language='javascript'>alert('" & Results.Msg & "')</script>"

                            'Case "NewImport", "NewExport", "ExistingFeed"
                            '    Response.Redirect("FeedMaintain.aspx?Action=" & Action)

                        Case "NewImport", "NewExport"
                            Response.Redirect("FeedMaintain.aspx?CallType=New")

                        Case "ExistingFeed"
                            Response.Redirect("FeedMaintain.aspx?CallType=Existing")

                            'Case "NewSchedule", "ExistingSchedule"
                            '    Response.Redirect("ScheduleMaintain.aspx?Action=" & Action)

                        Case "NewSchedule"
                            Response.Redirect("ScheduleMaintain.aspx?CallType=New")

                        Case "ExistingSchedule"
                            Response.Redirect("ScheduleMaintain.aspx?CallType=Existing")

                        Case "ShowHideSubTable"
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
            litEnviro.Text = "<input type='hidden' name='hdLoggedInUserID' value='" & Common.LoggedInUserID & "'><input type='hidden' name='hdDBHost'  value='hbg-sql'>"

        Catch ex As Exception
            Throw New Exception("Error #302: Feedlist Page_Load. " & ex.Message)
        End Try
    End Sub

    Private Sub LoadVariables(ByVal PageMode As PageMode)

        Try
            Select Case PageMode
                Case PageMode.Initial

                    ' ___ Active fields
                    'cActiveClientID = String.Empty
                    'cProcessName = String.Empty
                    'cDestinationID = String.Empty
                    'cStartDate = String.Empty

                    Sess.ActiveClientID = String.Empty
                    Sess.ProcessName = String.Empty
                    Sess.DestinationID = String.Empty
                    Sess.StartDate = String.Empty

                    ' ___ Subtable selection
                    'cSubTableInd = "0"
                    'cSubTableClientID = String.Empty
                    'cSubTableProcessName = String.Empty
                    'cSubTableDestinationID = String.Empty

                    Sess.SubTableInd = "0"
                    Sess.SubTableClientID = String.Empty
                    Sess.SubTableProcessName = String.Empty
                    Sess.SubTableDestinationID = String.Empty


                    ' ___ Filter selections
                    'cClientIDSelectedFilterValue = String.Empty
                    'cProcessTypeIDSelectedFilterValue = String.Empty
                    'cCarrierIDSelectedFilterValue = String.Empty
                    'cDeveloperSelectedFilterValue = String.Empty
                    Sess.ClientIDSelectedFilterValue = String.Empty
                    Sess.ProcessTypeIDSelectedFilterValue = String.Empty
                    Sess.CarrierIDSelectedFilterValue = String.Empty
                    Sess.DeveloperSelectedFilterValue = String.Empty



                Case PageMode.CalledByOther, PageMode.ReturnFromChild

                    ' ___ Active fields
                    'cActiveClientID = Session("FeedlistActiveClientID")
                    'cProcessName = Session("FeedlistProcessName")
                    'cDestinationID = Session("FeedlistDestinationID")

                    '' ___ Subtable selection
                    'cSubTableInd = Session("FeedlistSubTableInd")
                    'cSubTableClientID = Session("FeedlistSubTableClientID")
                    'cSubTableProcessName = Session("FeedlistSubTableProcessName")
                    'cSubTableDestinationID = Session("FeedlistSubTableDestinationID")

                    '' ___ Filter selections
                    'cClientIDSelectedFilterValue = cActiveClientID
                    'cProcessTypeIDSelectedFilterValue = Session("FeedlistProcessTypeIDSelectedFilterValue")
                    'cCarrierIDSelectedFilterValue = Session("FeedlistCarrierIDSelectedFilterValue")
                    'cDeveloperSelectedFilterValue = Session("FeedlistDeveloperSelectedFilterValue")
                    'cFilterOnResponse = Session("FeedlistFilterOnResponse")
                    'cFilterShowHideToggle = Session("FeedlistFilterShowHideToggle")

                Case PageMode.Postback

                    ' ___ Update session variables with those that the user may have changed

                    ' ___ Active client 
                    Sess.ActiveClientID = Request.Form("hdActiveClientID")
                    Sess.ProcessName = Request.Form("hdProcessName")
                    Sess.DestinationID = Request.Form("hdDestinationID")
                    Sess.StartDate = Request.Form("hdStartDate")
                    Sess.ProcessTypeID = Request.Form("hdProcessTypeID")

                    ' ___ SubTable
                    'cSubTableInd = Request.Form("hdSubTableInd")
                    'cSubTableClientID = Request.Form("hdSubTableClientID")
                    'cSubTableProcessName = Request.Form("hdSubTableProcessName")
                    'cSubTableDestinationID = Request.Form("hdSubTableDestinationID")

                    Sess.SubTableInd = Request.Form("hdSubTableInd")
                    Sess.SubTableClientID = Request.Form("hdSubTableClientID")
                    Sess.SubTableProcessName = Request.Form("hdSubTableProcessName")
                    Sess.SubTableDestinationID = Request.Form("hdSubTableDestinationID")

                    ' ___ Filter
                    'cClientIDSelectedFilterValue = Request.Form("hdClientIDSelectedFilterValue")
                    'cProcessTypeIDSelectedFilterValue = Request.Form("hdProcessTypeIDSelectedFilterValue")
                    'cCarrierIDSelectedFilterValue = Request.Form("hdCarrierIDSelectedFilterValue")
                    'cDeveloperSelectedFilterValue = Request.Form("hdDeveloperSelectedFilterValue")
                    'cFilterOnResponse = Request.Form("hdFilterOnResponse")                ' not changed in client
                    'cFilterShowHideToggle = Request.Form("hdFilterShowHideToggle")    ' changed in client keep hd

                    Sess.ClientIDSelectedFilterValue = Request.Form("ddClientID")
                    Sess.ProcessTypeIDSelectedFilterValue = Request.Form("ddProcessTypeID")
                    Sess.CarrierIDSelectedFilterValue = Request.Form("ddCarrierID")
                    Sess.DeveloperSelectedFilterValue = Request.Form("ddDeveloper")
                    ' cFilterOnResponse = Request.Form("hdFilterOnResponse")                ' not changed in client

                    ' Note:  hdFilterShowHideToggle processed elsewhere

        End Select
        Catch ex As Exception
            Throw New Exception("Error #303: Feedlist LoadVariables. " & ex.Message)
        End Try
    End Sub

    Private Function DeleteFeed() As Results
        Dim MyResults As New Results

        Try

            Dim CmdAsst As New CmdAsst(CommandType.StoredProcedure, "FeedDel")
            Dim QueryPack As CmdAsst.QueryPack
            CmdAsst.AddVarChar("ClientID", Sess.ActiveClientID, 50, False)
            CmdAsst.AddVarChar("ProcessName", Sess.ProcessName, 50, False)
            CmdAsst.AddInt("DestinationID", IIf(Sess.DestinationID = String.Empty, 0, Sess.DestinationID), False)
            Try
                QueryPack = CmdAsst.Execute
            Catch
                QueryPack.Success = False
            End Try
            MyResults.Success = QueryPack.Success
            If QueryPack.Success Then
                MyResults.Msg = "Record deleted."
            Else
                MyResults.Msg = "Unable to delete record."
            End If
            Return MyResults

        Catch ex As Exception
            Throw New Exception("Error #304: Feedlist DeleteFeed. " & ex.Message)
        End Try
    End Function

    Private Function DeleteSchedule() As Results
        Dim MyResults As New Results

        Try
            Dim CmdAsst As New CmdAsst(CommandType.StoredProcedure, "ScheduleDel")
            Dim QueryPack As CmdAsst.QueryPack
            CmdAsst.AddVarChar("ClientID", Sess.ActiveClientID, 50, False)
            CmdAsst.AddVarChar("ProcessName", Sess.ProcessName, 50, False)
            CmdAsst.AddDateTime("StartDate", Sess.StartDate, False)
            Try
                QueryPack = CmdAsst.Execute
            Catch
                QueryPack.Success = False
            End Try
            MyResults.Success = QueryPack.Success
            If QueryPack.Success Then
                MyResults.Msg = "Record deleted."
            Else
                MyResults.Msg = "Unable to delete record."
            End If
            Return MyResults

        Catch ex As Exception
            Throw New Exception("Error #305: Feedlist DeleteSchedule. " & ex.Message)
        End Try
    End Function

    Private Function DefineDataGrid(ByVal Entity As String) As DG
        Dim DG As DG

        Try

            If Entity = "Feedlist" Then
                DG = New DG("ClientID", Rights, True, "EmbeddedTableDef", "ClientID", "A")
                DG.AddDataBoundColumn("ClientID", "ClientID", "Client", "ClientID", True, Nothing, Nothing, "align='left'")
                DG.AddDataBoundColumn("ProcessName", "ProcessName", "Process<br>Name", "ProcessName", True, Nothing, Nothing, "align='left'")
                DG.AddDataBoundColumn("ProcessTypeID", "ProcessTypeID", "Process<br>Type", "ProcessTypeID", True, Nothing, Nothing, "align='left'")
                DG.AddDataBoundColumn("CarrierID", "CarrierID", "Carrier", "CarrierID", True, Nothing, Nothing, "align='left'")
                DG.AddDataBoundColumn("DeveloperFullName", "DeveloperFullName", "Developer", "us.LastName+~, ~+us.FirstName", True, Nothing, Nothing, "align='left'")
                DG.AddDataBoundColumn("NumSched", "NumSched", "Sched", Nothing, True, Nothing, Nothing, "align='left'")
                DG.AddBooleanColumn("ActiveInd", "ActiveInd", "Status", "ActiveInd", True, "1", "Active", "Inactive", Nothing, "align='left'")

                ' ___ Build the menu
                Dim Menu As DG.Menu
                Menu = DG.AttachMenu(10)
                ' Menu.AddItem(DG.Menu.ObjectTypeEnum.IsLink, "NewFeed", "New Feed", Rights.FeedlistEdit)
                Menu.AddItem(DG.Menu.ObjectTypeEnum.IsLink, "NewImport", "New Import", Rights.FeedlistEdit)
                Menu.AddItem(DG.Menu.ObjectTypeEnum.IsLink, "NewExport", "New Export", Rights.FeedlistEdit)

                Dim TemplateCol As New DG.TemplateColumn("Icons", Nothing, False, Nothing, True)

                '   templatecol.AddDefaultTemplateItem("itemname", "onclick", "img", "title", "right", "PermFld", "Parm2", "Parm3", "Parm4")
                TemplateCol.AddDefaultTemplateItem("View", "ExistingFeed", "StandardView", "Feed Detail", Rights.FeedlistView, Nothing, "ProcessName", "DestinationID", "ProcessTypeID")
                TemplateCol.AddDefaultTemplateItem("NewSchedule", "NewSchedule", "StandardClip", "New Schedule", Rights.ScheduleEdit, Nothing, "ProcessName", "DestinationID", "ProcessTypeID")
                TemplateCol.AddDefaultTemplateItem("DeleteFeed", "DeleteFeed", "StandardDelete", "Delete Feed", Rights.FeedlistEdit, Nothing, "ProcessName", "DestinationID")
                DG.AttachTemplateCol(TemplateCol)

                Dim ChildTables As DG.ChildTablesClass
                ChildTables = DG.AttachChildTables("ScheduleSubTable", "ClientID", Nothing)
                DG.AddChildTableSelectColumn("ScheduleSubTable", "ClientID", "SubTable", Nothing, Nothing, "ProcessName", "DestinationID")

                ' ___ Build the filter
                Dim Filter As DG.Filter
                Filter = DG.AttachFilter(DG.FilterOperationMode.FilterSwitchable, DG.FilterInitialShowHideEnum.FilterInitialShow, DG.RecordsInitialShowHideEnum.RecordsInitialShow)
                Filter.AddDropdown("ClientID", "ClientID")
                Filter.AddDropdown("ProcessTypeID", "ProcessTypeID")
                Filter.AddDropdown("CarrierID", "CarrierID")
                'Filter.AddDropdown("Developer", "Developer")
                'Filter.AddExtendedDropdown("Catgy")
                Filter.AddExtendedDropdown("DeveloperFullName", "DeveloperFullName")

            ElseIf Entity = "Schedule" Then
                DG = New DG("ClientID", Rights, True, "EmbeddedTableDef", "ClientID", "A")
                DG.AddDateColumn("StartDate", "StartDate", "Start Date", Nothing, True, "MM/dd/yyyy", Nothing, "align='left'")
                DG.AddDateColumn("EndDate", "EndDate", "End Date", Nothing, True, "MM/dd/yyyy", Nothing, "align='left'")
                DG.AddBooleanColumn("ActiveInd", "ActiveInd", "Status", Nothing, True, "1", "Active", "Inactive", Nothing, "align='left'")

                Dim TemplateCol As New DG.TemplateColumn("Icons", Nothing, False, Nothing, True)
                TemplateCol.AddDefaultTemplateItem("View", "ExistingSchedule", "StandardView", "Schedule Detail", Rights.ScheduleView, Nothing, "ProcessName", "DestinationID", "StartDate")
                TemplateCol.AddDefaultTemplateItem("DeleteSchedule", "DeleteSchedule", "StandardDelete", "Delete Schedule", Rights.ScheduleEdit, Nothing, "ProcessName", "DestinationID", "StartDate")
                DG.AttachTemplateCol(TemplateCol)

                'DG.AddFreeFormColumn("Spacer", Nothing, Nothing, Nothing, True, "width='80px'")
                DG.FormatAsSubTable = True
            End If

            Return DG

        Catch ex As Exception
            Throw New Exception("Error #306: Feedlist DefineDataGrid. " & ex.Message)
        End Try
    End Function

    Private Sub DisplayPage(ByVal PageMode As PageMode, ByVal DG As DG, ByVal OrderByType As DG.OrderByType, Optional ByVal OrderByField As String = Nothing)
        Dim i As Integer
        Dim dt As DataTable
        Dim sbSql As New System.Text.StringBuilder
        Dim Sql As String
        Dim WhereClause As New System.Text.StringBuilder
        Dim ShowFilter As Boolean
        Dim ShowRecords As Boolean
        Dim sbHiddens As New System.Text.StringBuilder

        Try

            ' ___ Get a filter reference
            Dim Filter As DG.Filter
            Filter = DG.GetFilter

            ' ___ Client Name
            dt = Common.GetDT("SELECT ClientID FROM UserManagement..Codes_ClientID WHERE LogicalDelete = 0 ORDER BY ClientID")
            Filter("ClientID").AddDropdownItem("", "", True)
            For i = 0 To dt.Rows.Count - 1
                Filter("ClientID").AddDropdownItem(dt.Rows(i)(0), dt.Rows(i)(0), False)
            Next
            If PageMode = PageMode.ReturnFromChild Or PageMode = PageMode.CalledByOther Then
                Filter.Coll("ClientID").SetOverrideValue(Sess.ClientIDSelectedFilterValue)
            End If

            Filter("ProcessTypeID").AddDropdownItem("", "", True)
            Filter("ProcessTypeID").AddDropdownItem("Import", "Import")
            Filter("ProcessTypeID").AddDropdownItem("Export", "Export")
            If PageMode = PageMode.ReturnFromChild Or PageMode = PageMode.CalledByOther Then
                Filter.Coll("ProcessTypeID").SetOverrideValue(Sess.ProcessTypeIDSelectedFilterValue)
            End If


            ' ___ Carrier ID
            dt = Common.GetDT("SELECT CarrierID FROM EXIM..Codes_CarrierID_Auxiliary UNION SELECT CarrierID FROM UserManagement..Codes_CarrierID ORDER BY CarrierID")
            'dt = Common.GetDT("SELECT DISTINCT CarrierID FROM UserManagement..Codes_CarrierID ORDER BY CarrierID")
            Filter("CarrierID").AddDropdownItem("", "", True)
            For i = 0 To dt.Rows.Count - 1
                Filter("CarrierID").AddDropdownItem(dt.Rows(i)(0), dt.Rows(i)(0), False)
            Next
            If PageMode = PageMode.ReturnFromChild Or PageMode = PageMode.CalledByOther Then
                Filter.Coll("CarrierID").SetOverrideValue(Sess.CarrierIDSelectedFilterValue)
            End If

            ' ___ DeveloperFullName
            ' dt = Common.GetDT("SELECT LastName + ', ' + FirstName FROM UserManagement..Users WHERE Role = 'IT' ORDER BY LastName, FirstName")
            dt = Common.GetDT("SELECT LastName, FirstName FROM UserManagement..Users WHERE Role = 'IT' ORDER BY LastName, FirstName")
            Filter("DeveloperFullName").AddDropdownItem("", "", True)
            Dim FN As String
            Dim LN As String
            For i = 0 To dt.Rows.Count - 1
                'Filter("Developer").AddDropdownItem(dt.Rows(i)(0), dt.Rows(i)(0), False)
                ' Filter("Developer").AddExtendedDropdownItem("1", "Application", " ul1.EffectiveDate = '1/1/1950' ", False)
                'DeveloperName = dt.Rows(i)(0)
                'Filter("DeveloperFullName").AddExtendedDropdownItem((i + 1).ToString, dt.Rows(i)(0), " DeveloperFullName = '" & dt.Rows(i)(0) & "'", False)
                LN = dt.Rows(i)(0)
                FN = dt.Rows(i)(1)
                Filter("DeveloperFullName").AddExtendedDropdownItem((i + 1).ToString, dt.Rows(i)(0) & ", " & dt.Rows(i)(1), "us.LastName='" & LN & "' AND us.FirstName='" & FN & "'", False)
                'Filter("D").addexetendeddropdownitem(value, text, sql, selected)


            Next
            If PageMode = PageMode.ReturnFromChild Or PageMode = PageMode.CalledByOther Then
                Filter.Coll("DeveloperFullName").SetOverrideValue(Sess.DeveloperSelectedFilterValue)
            End If

        Catch ex As Exception
            Throw New Exception("Error #307a: Feedlist DisplayPage. " & ex.Message)
        End Try

        Try

            '' ___ Catgy
            'Filter("Catgy").AddExtendedDropdownItem("", "", "", True)
            'Filter("Catgy").AddExtendedDropdownItem("1", "Application", " ul1.EffectiveDate = '1/1/1950' ", False)
            'Filter("Catgy").AddExtendedDropdownItem("2", "Effective", " ul1.EffectiveDate <> '1/1/1950' ", False)

            ' ___ Heading
            If Rights.HasThisRight(Rights.FeedlistEdit) Then
                litHeading.Text = "Edit Feed"
            Else
                litHeading.Text = "View Feed"
            End If

            ' ___ Handle the sort
            If Sess.SortReference <> Nothing Then
                DG.UpdateSortReference(Sess.SortReference)
            End If
            DG.SetSortElements(OrderByField, OrderByType)

            ' ___ Load the parameters and execute the query

            '            dt = Common.GetDT("SELECT fl.*, DeveloperFullName=us.LastName + ', ' + us.FirstName FROM Feedlist fl INNER JOIN UserManagment..Users us on fl.UserID = us.UserID WHERE fl.ClientID = '" & Sess.ActiveClientID & "' AND fl.ProcessName = '" & Sess.ProcessName & "' AND fl.DestinationID = " & IIf(Sess.DestinationID = String.Empty, 0, Sess.DestinationID))


            ' Sql = "SELECT *, SubTable='1' FROM Feedlist"
            'Select NumSch = (Select Count (*) FROM FeedSchedule  fs Where fs.ClientID = fl.ClientID) from feedlist fl
            sbSql.Append("SELECT fl.*, SubTable='1', ") 'NumSched = (Select Count (*) FROM FeedSchedule  fs Where fs.ClientID = fl.ClientID)  FROM Feedlist fl"
            sbSql.Append("DeveloperFullName=us.LastName + ', ' + us.FirstName, ")
            sbSql.Append("NumSched = (Select Count (*) FROM FeedSchedule  fs Where fs.ClientID = fl.ClientID and fs.ProcessName = fl.ProcessName and fs.DestinationID = fl.DestinationID) ")
            sbSql.Append("FROM Feedlist fl ")
            sbSql.Append("INNER JOIN UserManagement..Users us on fl.Developer = us.UserID")

            Sql = sbSql.ToString
            DG.GenerateSQL(Sql, ShowFilter, Nothing, OrderByType, Request, Sess.FilterOnOffState, Request.Form("hdFilterShowHideToggle"), False)

        Catch ex As Exception
            Throw New Exception("Error #307b Feedlist DisplayPage. " & ex.Message)
        End Try

        Try
            dt = Common.GetDT(Sql)
        Catch ex As Exception
            Throw New Exception("Error #307c: Feedlist DisplayPage " & ex.Message)
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

            ' ___ Set the last field sorted and sort direction in the sort reference string
            'Session("FeedlistSortReference") = DG.GetSortReference()
            Sess.SortReference = DG.GetSortReference

            sbHiddens.Append("<input type='hidden' id='hdActiveClientID' name='hdActiveClientID' value='" & Sess.ActiveClientID & "'>")
            sbHiddens.Append("<input type='hidden' id='hdProcessName' name='hdProcessName' value='" & Sess.ProcessName & "'>")
            sbHiddens.Append("<input type='hidden' id='hdDestinationID' name='hdDestinationID' value='" & Sess.DestinationID & "'>")
            sbHiddens.Append("<input type='hidden' id='hdStartDate' name='hdStartDate' value='" & Sess.StartDate & "'>")

            sbHiddens.Append("<input type='hidden' id='hdSubTableInd' name='hdSubTableInd' value='" & Sess.SubTableInd & "'>")
            sbHiddens.Append("<input type='hidden' id='hdSubTableClientID' name='hdSubTableClientID' value='" & Sess.SubTableClientID & "'>")
            sbHiddens.Append("<input type='hidden' id='hdSubTableProcessName' name='hdSubTableProcessName' value='" & Sess.SubTableProcessName & "'>")
            sbHiddens.Append("<input type='hidden' id='hdSubTableDestinationID' name='hdSubTableDestinationID' value='" & Sess.SubTableDestinationID & "'>")
            litHiddens.Text = sbHiddens.ToString

        Catch ex As Exception
            Throw New Exception("Error #307d: Feedlist DisplayPage " & ex.Message)
        End Try
    End Sub

End Class