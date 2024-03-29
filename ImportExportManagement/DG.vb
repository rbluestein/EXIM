Imports System.Data.SqlClient

Public Class DG

#Region " Declarations "
    Public Event ChildDTRequest(ByRef ChildText As String, ByVal DataFldName As String, ByVal Value As String)
    Public Event NewChildDTRequest(ByRef ParmColl As Collection)

    'Public Event ChildDTRequest2(ByRef ChildText As String, ByVal DataFldName As String, ByVal Value As String, ByVal Parm2 As String)
    'Public Event ChildDTRequest3(ByRef ChildText As String, ByVal DataFldName As String, ByVal Value As String, ByVal Parm2 As String, ByVal Parm3 As String)
    'Public Event ChildDTRequest4(ByRef ChildText As String, ByVal DataFldName As String, ByVal Value As String, ByVal Parm2 As String, ByVal Parm3 As String, ByVal Parm4 As String)
    Private cChildTables As ChildTablesClass
    Private cInternalFilter As DG.Filter
    Private cExternalFilter As DG.ExternalFilter
    Private cMenu As DG.Menu
    Private cColumnColl As New Collection
    Private cSortReferenceColl As New Collection
    Private cTemplateColl As New Collection
    Private Common As Common
    Private Rights As RightsClass
    Private cKeyFieldName As String
    Private cTableDef As String
    Private cUseDefaultTableDef As Boolean
    Private cDefaultColl As New Collection
    Private cCheckboxToggleColl As Collection
    Private cHiddenColumnColl As Collection
    Private cLeftPadding As String
    Private cDefaultSortField As String
    Private cDefaultSortDirection As String
    Private cInternalFilterOperationMode As FilterOperationModeEnum = FilterOperationMode.NoFilter
    Private cInternalFilterInitialShowHide As FilterInitialShowHideEnum
    Private cRecordsInitialShowHide As RecordsInitialShowHideEnum = RecordsInitialShowHideEnum.RecordsInitialShow
    Private cShowInternalFilter As Boolean

    'Private cExternalFilterOperationMode As FilterOperationModeEnum = FilterOperationMode.NoFilter
    'Private cExternalFilterInitialShowHide As FilterInitialShowHideEnum
    'Private cExternalInitialShowHide As RecordsInitialShowHideEnum = RecordsInitialShowHideEnum.RecordsInitialShow
    Private cExternalInternalFilter As Boolean

    Private cAttachNewButton As Boolean
    Private cNewButtonRight As String
    Private cFormatAsSubTable As Boolean
    Private cNewOrderByField As String
    Private cNewSortDirection As String
#End Region

#Region " Enums "
    Public Enum Justify
        none = 0
        left = 1
        center = 2
        right = 3
    End Enum

    Public Enum ColumnType
        Databound = 1
        Template = 2
        Link = 3
        CheckboxToggle = 4
        Hidden = 5
        FreeForm = 6
        ChildTableSelect = 7
        [Boolean] = 8
        [Date] = 9
    End Enum

    Public Enum OrderByType
        Initial = 1
        Recurring = 2
        Field = 3
        ReturnToPage = 4
    End Enum

    Public Enum FilterOperationModeEnum
        NoFilter = 1
        FilterAlwaysOn = 2
        FilterAlwaysOff = 3
        FilterSwitchable = 4
    End Enum

    Public Enum FilterInitialShowHideEnum
        FilterInitialShow = 1
        FilterInitalHide = 2
    End Enum

    Public Enum RecordsInitialShowHideEnum
        RecordsInitialShow = 1
        RecordsInitialHide = 2
    End Enum
#End Region

#Region " Properties "
    Public ReadOnly Property NewOrderByField()
        Get
            Return cNewOrderByField
        End Get
    End Property
    Public ReadOnly Property NewSortDirection()
        Get
            Return cNewSortDirection
        End Get
    End Property
    Public WriteOnly Property FormatAsSubTable()
        Set(ByVal Value)
            cFormatAsSubTable = Value
        End Set
    End Property
    Public Property ChildTables()
        Get
            Return cChildTables
        End Get
        Set(ByVal Value)
            cChildTables = Value
        End Set
    End Property
    Public ReadOnly Property FilterOperationMode()
        Get
            Return cInternalFilterOperationMode
        End Get
    End Property
    Public ReadOnly Property FilterInitialShowHide()
        Get
            Return cInternalFilterInitialShowHide
        End Get
    End Property
    Public ReadOnly Property RecordsInitialShowHide()
        Get
            Return cRecordsInitialShowHide
        End Get
    End Property
#End Region

#Region " Constructors "
    Public Sub New()
        Dim dt As New DataTable
        Dim SessionObj As System.Web.SessionState.HttpSessionState = System.Web.HttpContext.Current.Session
        Common = SessionObj("Common")
        'Dim ChildDG As New DG("Dog", Rights, True, "", "")
        'ChildDG.GetChildText(dt, "KeyFldName", "KeyFldValue")
    End Sub

    Public Sub New(ByVal KeyFieldName As String, ByVal Rights As RightsClass, ByVal UseDefaultTableDef As Boolean, ByVal TableDef As String, ByVal DefaultSortField As String, ByVal DefaultSortDirection As String)
        cKeyFieldName = KeyFieldName
        Dim aRights(0) As String
        cUseDefaultTableDef = UseDefaultTableDef
        cTableDef = TableDef
        Me.Rights = Rights
        cDefaultColl.Add("<table class='DG' style='LEFT: 150px; POSITION: absolute; TOP: 214px' cellSpacing='0' cellPadding='0' width='650' border='0'>", "StandardTableDef")
        cDefaultColl.Add("<table class='DGEmbedded' cellSpacing='0' cellPadding='0' width='100%' border='0'>", "EmbeddedTableDef")
        'cLeftPadding = " style='padding-left:10px' "
        cLeftPadding = ""
        cDefaultSortField = DefaultSortField
        cDefaultSortDirection = DefaultSortDirection
        Dim SessionObj As System.Web.SessionState.HttpSessionState = System.Web.HttpContext.Current.Session
        Common = SessionObj("Common")
    End Sub
#End Region

#Region " Generate SQL "
    '   Public Sub GenerateSQL(ByRef Sql As String, ByRef ShowFilter As Boolean, ByVal SecurityWhereClause As String, ByVal OrderByType As DG.OrderByType, ByVal Request As HttpRequest, Optional ByVal OmitWhere As Boolean = False)
    Public Sub GenerateSQL(ByRef Sql As String, ByRef ShowFilter As Boolean, ByVal SecurityWhereClause As String, ByVal OrderByType As DG.OrderByType, ByRef Request As HttpRequest, ByVal FilterOnOffState As String, ByVal FilterShowHideToggle As String, Optional ByVal OmitWhere As Boolean = False)
        Dim ShowRecords As Boolean
        Dim FilterWhereClause As String
        Dim WorkingSql As String = String.Empty
        Dim i As Integer

        If cInternalFilter Is Nothing Then
            If SecurityWhereClause <> Nothing Then

                If OmitWhere Then
                    Sql &= " AND " & SecurityWhereClause
                Else
                    Sql &= " WHERE " & SecurityWhereClause
                End If

            End If
            Sql &= " ORDER BY " & Replace(cNewOrderByField, "~", "'")
            If cNewSortDirection = "A" Then
                Sql &= " ASC"
            Else
                Sql &= " DESC"
            End If
        Else

            ' ___ Show/hide the filter. Record filter values
            'ShowFilter = GetShowInternalFilter(OrderByType, Request)
            ShowFilter = GetShowInternalFilter(OrderByType, FilterOnOffState, FilterShowHideToggle)
            ShowRecords = GetShowRecords(OrderByType)
            ' RecordFilterValues(ShowFilter, Request)
            RecordFilterValues(ShowFilter, Request, FilterOnOffState)

            ' new
            If ShowRecords And FilterOperationMode <> DG.FilterOperationModeEnum.NoFilter Then
                FilterWhereClause = GetFilterWhereClause()
            End If
            If Not ShowRecords Then
                FilterWhereClause = "  0 = 1 "
            End If

            If SecurityWhereClause = Nothing Then
                SecurityWhereClause = String.Empty
            End If

            If SecurityWhereClause.Length = 0 Then
                If FilterWhereClause.Length = 0 Then
                    ' No action
                Else
                    WorkingSql = FilterWhereClause
                End If
            Else
                If FilterWhereClause.Length = 0 Then
                    WorkingSql = SecurityWhereClause
                Else
                    WorkingSql = SecurityWhereClause & " AND " & FilterWhereClause
                End If
            End If

            If WorkingSql.Length > 0 Then
                If OmitWhere Then
                    Sql &= " AND " & WorkingSql
                Else
                    Sql &= " WHERE " & WorkingSql
                End If
            End If

            Sql &= " ORDER BY " & Replace(cNewOrderByField, "~", "'")
            If cNewSortDirection = "A" Then
                Sql &= " ASC"
            Else
                Sql &= " DESC"
            End If
        End If

        'If Not cExternalFilter Is Nothing Then
        '    ' Record filter values
        '    For i = 1 To cExternalFilter.Coll.count
        '        cExternalFilter.Coll(i).SetValue(Request.Form(cExternalFilter.Coll(i).CtlName))
        '    Next
        'End If

        If Not cExternalFilter Is Nothing Then
            ' Record filter values
            For i = 1 To cExternalFilter.Coll.count
                If Not cExternalFilter.Coll(i).IsLink Then
                    If cExternalFilter.Coll(i).GetOverrideValue = Nothing Then
                        cExternalFilter.Coll(i).SetValue(Request.Form(cExternalFilter.Coll(i).CtlName))
                    Else
                        cExternalFilter.Coll(i).SetValue(cExternalFilter.Coll(i).GetOverrideValue)
                    End If
                Else
                End If
            Next
        End If

    End Sub
#End Region

#Region " Handle sorting "
    Public Sub SetSortElements(ByVal OrderByField As String, ByVal OrderByType As OrderByType)
        Dim CurSortDirection As String
        Dim NewOrderByField As String
        Dim NewSortDirection As String

        If OrderByType = OrderByType.Initial Then
            NewOrderByField = cDefaultSortField
            If cDefaultSortDirection = "A" Then
                NewSortDirection = "A"
            Else
                NewSortDirection = "D"
            End If
            ' NewSortDirection = "A"

        ElseIf OrderByType = OrderByType.Recurring Or OrderByType = OrderByType.ReturnToPage Then
                NewOrderByField = GetLastFieldSorted()
                If NewOrderByField = "" Then
                    NewOrderByField = cDefaultSortField
                    NewSortDirection = "A"
                Else
                    NewSortDirection = GetSortDirection(NewOrderByField)
                End If
            ElseIf OrderByType = OrderByType.Field Then
                NewOrderByField = OrderByField
                CurSortDirection = GetSortDirection(OrderByField)
                Select Case CurSortDirection
                    Case "A"
                        NewSortDirection = "D"
                    Case "N", "D"
                        NewSortDirection = "A"
                End Select
            End If
            SetLastFieldSorted(NewOrderByField)
            SetSortDirection(NewOrderByField, NewSortDirection)
            cNewOrderByField = NewOrderByField
            cNewSortDirection = NewSortDirection
    End Sub

    'Public Sub SetSortElements(ByVal OrderByField As String, ByVal OrderByType As OrderByType, ByRef NewOrderByField As String, ByRef NewSortDirection As String)
    '    Dim CurSortDirection As String

    '    If OrderByType = OrderByType.Initial Then
    '        NewOrderByField = cDefaultSortField
    '        NewSortDirection = "A"
    '    ElseIf OrderByType = OrderByType.Recurring Or OrderByType = OrderByType.ReturnToPage Then
    '        NewOrderByField = GetLastFieldSorted()
    '        If NewOrderByField = "" Then
    '            NewOrderByField = cDefaultSortField
    '            NewSortDirection = "A"
    '        Else
    '            NewSortDirection = GetSortDirection(NewOrderByField)
    '        End If
    '    ElseIf OrderByType = OrderByType.Field Then
    '        NewOrderByField = OrderByField
    '        CurSortDirection = GetSortDirection(OrderByField)
    '        Select Case CurSortDirection
    '            Case "A"
    '                NewSortDirection = "D"
    '            Case "N", "D"
    '                NewSortDirection = "A"
    '        End Select
    '    End If
    '    SetLastFieldSorted(NewOrderByField)
    '    SetSortDirection(NewOrderByField, NewSortDirection)
    '    cNewOrderByField = NewOrderByField
    '    cNewSortDirection = NewSortDirection
    'End Sub

    Private Sub AppendSortReference(ByVal SortExpression As String)
        Dim Position As String
        If SortExpression <> Nothing AndAlso SortExpression.Length > 0 Then
            Position = CStr(101 + cSortReferenceColl.Count).ToString
            cSortReferenceColl.Add(New SortItem(SortExpression, Position), SortExpression)
        End If
    End Sub

    Public Sub UpdateSortReference(ByVal SortReference As String)
        Dim i As Integer
        For i = 1 To cSortReferenceColl.Count
            cSortReferenceColl(i).SortDirection = SortReference.Substring((5 * (i - 1)) + 3, 1)
            cSortReferenceColl(i).LastFieldSorted = SortReference.Substring((5 * (i - 1)) + 4, 1)
        Next
    End Sub

    Public Function GetSortReference() As String
        Dim i As Integer
        Dim sb As New System.Text.StringBuilder
        For i = 1 To cSortReferenceColl.Count
            sb.Append(cSortReferenceColl(i).Position & cSortReferenceColl(i).SortDirection & cSortReferenceColl(i).LastFieldSorted)
        Next
        Return sb.ToString
    End Function

    Private Function GetSortDirection(ByVal FldName As String) As String
        If FldName = String.Empty Then
            Return String.Empty
        Else
            Return cSortReferenceColl(FldName).SortDirection
        End If
    End Function

    Private Sub SetSortDirection(ByVal FldName As String, ByVal Value As String)
        If cSortReferenceColl.Count > 0 Then
            cSortReferenceColl(FldName).SortDirection = Value
        End If
    End Sub

    Public Function GetLastFieldSorted() As String
        Dim i As Integer
        For i = 1 To cSortReferenceColl.Count
            If cSortReferenceColl(i).LastFieldSorted = "T" Then
                'Return cSortReferenceColl(i).Name
                Return cSortReferenceColl(i).SortExpression
            End If
        Next
        If i = cSortReferenceColl.Count + 1 Then
            Return String.Empty
        End If
    End Function

    Public Sub SetLastFieldSorted(ByVal FldName As String)
        Dim i As Integer
        If cSortReferenceColl.Count > 0 Then
            For i = 1 To cSortReferenceColl.Count
                cSortReferenceColl(i).LastFieldSorted = "F"
            Next
            If FldName <> String.Empty Then
                cSortReferenceColl(FldName).LastFieldSorted = "T"
            End If
        End If
    End Sub
#End Region

#Region " Handle the menu "
    Public Function AttachMenu(ByVal CellWidthPercent As Integer) As DG.Menu
        cMenu = New DG.Menu(CellWidthPercent)
        Return cMenu
    End Function
#End Region

#Region " Handle the internal filter "
    Public Function GetFilter() As DG.Filter
        Return cInternalFilter
    End Function

    Public Function AttachFilter(ByVal FilterOperationMode As FilterOperationModeEnum, ByVal FilterInitialShowHide As FilterInitialShowHideEnum, ByVal RecordsInitialShowHide As RecordsInitialShowHideEnum) As DG.Filter
        cInternalFilterOperationMode = FilterOperationMode
        cInternalFilterInitialShowHide = FilterInitialShowHide
        cRecordsInitialShowHide = RecordsInitialShowHide
        cInternalFilter = New DG.Filter
        Return cInternalFilter
    End Function

    ' Public Function GetShowInternalFilter(ByVal OrderByType As OrderByType, ByVal Request As HttpRequest) As Boolean
    Private Function GetShowInternalFilter(ByVal OrderByType As OrderByType, ByVal FilterOnOffState As String, ByVal FilterShowHideToggle As String) As Boolean
        Dim ShowFilter As Boolean

        Select Case cInternalFilterOperationMode
            Case DG.FilterOperationModeEnum.NoFilter, DG.FilterOperationModeEnum.FilterAlwaysOff
                ShowFilter = False
            Case DG.FilterOperationModeEnum.FilterAlwaysOn
                ShowFilter = True
            Case DG.FilterOperationModeEnum.FilterSwitchable
                Select Case OrderByType
                    Case DG.OrderByType.Initial
                        Select Case cInternalFilterInitialShowHide
                            Case DG.FilterInitialShowHideEnum.FilterInitialShow
                                ShowFilter = True
                            Case DG.FilterInitialShowHideEnum.FilterInitalHide
                                ShowFilter = False
                        End Select

                    Case OrderByType.ReturnToPage
                        Select Case FilterOnOffState
                            Case "on"
                                ShowFilter = True
                            Case Else
                                ShowFilter = False
                        End Select

                    Case Else

                        Select Case FilterOnOffState
                            Case "on"
                                Select Case FilterShowHideToggle
                                    Case "0"
                                        ShowFilter = True
                                    Case "1"
                                        ShowFilter = False
                                End Select
                            Case "off"
                                Select Case FilterShowHideToggle
                                    Case "0"
                                        ShowFilter = False
                                    Case "1"
                                        ShowFilter = True
                                End Select
                        End Select
                End Select
        End Select
        cShowInternalFilter = ShowFilter
        Return ShowFilter
    End Function

    Public Function GetShowRecords(ByVal OrderByType As DG.OrderByType) As Boolean
        Dim ShowRecords As Boolean
        Select Case OrderByType
            Case DG.OrderByType.Initial
                Select Case cRecordsInitialShowHide
                    Case DG.FilterInitialShowHideEnum.FilterInitialShow
                        ShowRecords = True
                    Case DG.FilterInitialShowHideEnum.FilterInitalHide
                        ShowRecords = False
                End Select
            Case Else
                ShowRecords = True
        End Select
        Return ShowRecords
    End Function

    Public Sub RecordFilterValues(ByVal ShowFilter As Boolean, ByRef Request As HttpRequest, ByVal FilterOn As String)
        Dim i As Integer

        ' ___ Internal filter
        Select Case FilterOn
            Case "on"
                Select Case ShowFilter
                    Case True
                        ' Write the values
                        For i = 1 To cInternalFilter.Coll.count
                            If cInternalFilter.Coll(i).GetOverrideValue = Nothing Then
                                cInternalFilter.Coll(i).SetValue(Request.Form(cInternalFilter.Coll(i).CtlName))
                            Else
                                cInternalFilter.Coll(i).SetValue(cInternalFilter.Coll(i).GetOverrideValue)
                            End If
                        Next
                    Case False
                        ' Clear it out
                        For i = 1 To cInternalFilter.Coll.count
                            cInternalFilter.Coll(i).SetValue("")
                        Next
                End Select
            Case "off"
                ' No action
        End Select

    End Sub

    Public Function GetFilterWhereClause() As String
        Dim Coll As New Collection
        Dim i As Integer
        Dim FilterWhereClause As New System.Text.StringBuilder
        Dim SelectedValue As String

        For i = 1 To cInternalFilter.Coll.count
            If cInternalFilter.Coll(i).GetValue <> String.Empty Then
                If cInternalFilter.Coll(i).IsTextbox Then
                    If cInternalFilter.Coll(i).FilterField = Nothing Then
                        'Coll.Add(Common.StrInHandler(cInternalFilter.Coll(i).DataFldName) & " LIKE  '" & cInternalFilter.Coll(i).GetValue & "%'")
                        Coll.Add(Common.StrInHandler(cInternalFilter.Coll(i).DataFldName) & " LIKE  '" & Common.StrOutHandler(cInternalFilter.Coll(i).GetValue, False, False) & "%'")
                    Else
                        '  Coll.Add(Common.StrInHandler(cInternalFilter.Coll(i).FilterField) & " LIKE  '" & cInternalFilter.Coll(i).GetValue & "%'")
                        Coll.Add(Common.StrInHandler(cInternalFilter.Coll(i).FilterField) & " LIKE  '" & Common.StrOutHandler(cInternalFilter.Coll(i).GetValue, False, False) & "%'")
                    End If
                Else
                    If cInternalFilter.Coll(i).IsStandard Then
                        Coll.Add(cInternalFilter.Coll(i).DataFldName & " = '" & cInternalFilter.Coll(i).GetValue & "'")
                    ElseIf cInternalFilter.Coll(i).IsExtended Then
                        SelectedValue = cInternalFilter.Coll(i).GetValue
                        Coll.Add(cInternalFilter.Coll(i).Coll(SelectedValue).Sql)
                    End If
                End If
            End If
        Next

        For i = 1 To Coll.Count
            If i = 1 Then
                FilterWhereClause.Append(Coll(i))
            Else
                FilterWhereClause.Append(" and " & Coll(i))
            End If
        Next
        Return FilterWhereClause.ToString
    End Function


#End Region

#Region " Handle the external filter "
    Public Function GetExternalFilter() As DG.ExternalFilter
        Return cExternalFilter
    End Function

    Public Function AttachExternalFilter() As DG.ExternalFilter
        'Public Function AttachExternalFilter(ByVal FilterOperationMode As FilterOperationModeEnum, ByVal FilterInitialShowHide As FilterInitialShowHideEnum, ByVal RecordsInitialShowHide As RecordsInitialShowHideEnum) As DG.ExternalFilter
        'cExternalFilterOperationMode = FilterOperationMode
        'cExternalFilterInitialShowHide = FilterInitialShowHide
        'cExternalInitialShowHide = RecordsInitialShowHide
        cExternalFilter = New DG.ExternalFilter
        Return cExternalFilter
    End Function
#End Region

#Region " Attach components "
    Public Sub AddNewButton(ByVal Right As String)
        cAttachNewButton = True
        cNewButtonRight = Right
    End Sub

    Public Sub AddDataBoundColumn(ByVal ItemName As String, ByVal DataFldName As String, ByVal HeaderText As String, ByVal SortExpression As String, ByVal Visible As Boolean, ByVal DataFormatString As String, ByVal TitleFldName As String, ByVal Attributes As String)
        cColumnColl.Add(New DataBoundColumnItems(ColumnType.Databound, ItemName, DataFldName, HeaderText, SortExpression, Visible, DataFormatString, TitleFldName, Attributes), ItemName)
        AppendSortReference(SortExpression)
    End Sub

    Public Sub AddDateColumn(ByVal ItemName As String, ByVal DataFldName As String, ByVal HeaderText As String, ByVal SortExpression As String, ByVal Visible As Boolean, ByVal DataFormatString As String, ByVal TitleFldName As String, ByVal Attributes As String)
        cColumnColl.Add(New DateColumn(ColumnType.Date, ItemName, DataFldName, HeaderText, SortExpression, Visible, DataFormatString, TitleFldName, Attributes), ItemName)
        AppendSortReference(SortExpression)
    End Sub

    'Public Sub AddFreeFormColumn(ByVal ItemName As String, ByVal DataFldName As String, ByVal HeaderText As String, ByVal SortExpression As String, ByVal Visible As Boolean, ByVal DataFormatString As String, ByVal TitleFldName As String, ByVal Attributes As String, ByVal Text As String)
    '    cColumnColl.Add(New FreeFormColumn(ColumnType.FreeForm, ItemName, DataFldName, HeaderText, SortExpression, Visible, DataFormatString, TitleFldName, Attributes, Text), ItemName)
    '    AppendSortReference(SortExpression)
    'End Sub

    Public Sub AddChildTableSelectColumn(ByVal ItemName As String, ByVal DataFldName As String, ByVal EnableFldName As String, ByVal Title As String, ByVal Attributes As String)
        Dim ChildTableSelectColumn As New ChildTableSelectColumn(cChildTables, ColumnType.ChildTableSelect, ItemName, DataFldName, Nothing, Nothing, True, EnableFldName, Nothing, Title, Attributes, Nothing, Nothing, Nothing)
        cColumnColl.Add(ChildTableSelectColumn, ItemName, 1)
        cChildTables.ChildTableSelectColumn = ChildTableSelectColumn
        ' cColumnColl.Add(New ChildTableSelectColumn(cChildTables, ColumnType.ChildTableSelect, ItemName, DataFldName, Nothing, Nothing, True, EnableFldName, Nothing, Title, Attributes, Nothing, Nothing, Nothing), ItemName, 1)
    End Sub

    Public Sub AddChildTableSelectColumn(ByVal ItemName As String, ByVal DataFldName As String, ByVal EnableFldName As String, ByVal Title As String, ByVal Attributes As String, ByVal Parm2 As String)
        Dim ChildTableSelectColumn As New ChildTableSelectColumn(cChildTables, ColumnType.ChildTableSelect, ItemName, DataFldName, Nothing, Nothing, True, EnableFldName, Nothing, Title, Attributes, Parm2, Nothing, Nothing)
        cColumnColl.Add(ChildTableSelectColumn, ItemName, 1)
        cChildTables.ChildTableSelectColumn = ChildTableSelectColumn
        'cColumnColl.Add(New ChildTableSelectColumn(cChildTables, ColumnType.ChildTableSelect, ItemName, DataFldName, Nothing, Nothing, True, EnableFldName, Nothing, Title, Attributes, Parm2, Nothing, Nothing), ItemName, 1)
    End Sub

    Public Sub AddChildTableSelectColumn(ByVal ItemName As String, ByVal DataFldName As String, ByVal EnableFldName As String, ByVal Title As String, ByVal Attributes As String, ByVal Parm2 As String, ByVal Parm3 As String)
        Dim ChildTableSelectColumn As New ChildTableSelectColumn(cChildTables, ColumnType.ChildTableSelect, ItemName, DataFldName, Nothing, Nothing, True, EnableFldName, Nothing, Title, Attributes, Parm2, Parm3, Nothing)
        cColumnColl.Add(ChildTableSelectColumn, ItemName, 1)
        cChildTables.ChildTableSelectColumn = ChildTableSelectColumn
        ' cColumnColl.Add(New ChildTableSelectColumn(cChildTables, ColumnType.ChildTableSelect, ItemName, DataFldName, Nothing, Nothing, True, EnableFldName, Nothing, Title, Attributes, Parm2, Parm3, Nothing), ItemName, 1)
    End Sub

    Public Sub AddChildTableSelectColumn(ByVal ItemName As String, ByVal DataFldName As String, ByVal EnableFldName As String, ByVal Title As String, ByVal Attributes As String, ByVal Parm2 As String, ByVal Parm3 As String, ByVal Parm4 As String)
        Dim ChildTableSelectColumn As New ChildTableSelectColumn(cChildTables, ColumnType.ChildTableSelect, ItemName, DataFldName, Nothing, Nothing, True, EnableFldName, Nothing, Title, Attributes, Parm2, Parm3, Parm4)
        cColumnColl.Add(ChildTableSelectColumn, ItemName, 1)
        cChildTables.ChildTableSelectColumn = ChildTableSelectColumn
        ' cColumnColl.Add(New ChildTableSelectColumn(cChildTables, ColumnType.ChildTableSelect, ItemName, DataFldName, Nothing, Nothing, True, EnableFldName, Nothing, Title, Attributes, Parm2, Parm3, Parm4), ItemName, 1)
    End Sub

    Public Sub AddBooleanColumn(ByVal ItemName As String, ByVal DataFldName As String, ByVal HeaderText As String, ByVal SortExpression As String, ByVal Visible As Boolean, ByVal TrueValue As String, ByVal TrueText As String, ByVal FalseText As String, ByVal Title As String, ByVal Attributes As String)
        cColumnColl.Add(New BooleanColumn(ColumnType.Boolean, ItemName, DataFldName, HeaderText, SortExpression, Visible, TrueValue, TrueText, FalseText, Title, Attributes), ItemName)
        AppendSortReference(SortExpression)
    End Sub

    Public Sub AddFreeFormColumn(ByVal ItemName As String, ByVal CellText As String, ByVal Header As String, ByVal Title As String, ByVal Visible As Boolean, ByVal Attributes As String)
        cColumnColl.Add(New FreeFormColumn(ColumnType.FreeForm, ItemName, CellText, Header, Title, Visible, Attributes), ItemName)
    End Sub

    Public Sub AddHiddenColumn(ByVal ItemName As String, ByVal DataFldName As String)
        cColumnColl.Add(New HiddenItems(ColumnType.Hidden, ItemName, DataFldName), ItemName)
        If cHiddenColumnColl Is Nothing Then
            cHiddenColumnColl = New Collection
        End If
        cHiddenColumnColl.Add(New Collection, ItemName)
    End Sub

    Public Sub AddLinkColumn(ByVal ItemName As String, ByVal DataFldName As String, ByVal HRef As String, ByVal HeaderText As String, ByVal SortExpression As String, ByVal Visible As Boolean, ByVal DataFormatString As String, ByVal TitleFldName As String, ByVal Attributes As String, ByVal AddlParm As String)
        cColumnColl.Add(New LinkColumnItems(ColumnType.Link, HRef, ItemName, DataFldName, HeaderText, SortExpression, Visible, DataFormatString, TitleFldName, Attributes, AddlParm), ItemName)
        AppendSortReference(SortExpression)
    End Sub

    Public Sub AddCheckboxToggleColumn(ByVal ItemName As String, ByVal DataFldName As String, ByVal HeaderText As String, ByVal SortExpression As String, ByVal Visible As Boolean, ByVal Right As String, ByVal TestFld As String, ByVal TrueText As String, ByVal FalseText As String, ByVal TitleFldName As String, ByVal Attributes As String)
        If Rights.HasThisRight(Right) Then
            cColumnColl.Add(New CheckboxToggleColumnItems(ColumnType.CheckboxToggle, ItemName, DataFldName, HeaderText, SortExpression, Visible, TitleFldName, Attributes, TestFld, TrueText, FalseText), ItemName)
            If cCheckboxToggleColl Is Nothing Then
                cCheckboxToggleColl = New Collection
            End If
            cCheckboxToggleColl.Add(New System.Text.StringBuilder, ItemName)
            AppendSortReference(SortExpression)
        End If
    End Sub

    'Public Function GetTemplateColumn(ByVal ItemName As String, ByVal HeaderText As String, ByVal Wrap As Boolean, ByVal Visible As Boolean) As TemplateColumn
    '    cColumnColl.Add(New TemplateColumn(ItemName, HeaderText, Wrap, Visible), ItemName)
    '    Return cColumnColl(cColumnColl.Count)
    'End Function

    Public Sub AttachTemplateCol(ByRef TemplateCol As TemplateColumn)
        cColumnColl.Add(TemplateCol, TemplateCol.ItemName)
    End Sub

    Public Function GetCheckboxToggleColl(ByVal ItemName As String) As String
        Dim Value As String
        Value = cCheckboxToggleColl(ItemName).ToString
        Value = Value.Substring(0, Value.Length - 1)
        Return Value
    End Function

    Public Function GetColumnColl(ByVal ItemName) As String
        Dim i As Integer
        Dim Coll As Collection
        Coll = cHiddenColumnColl(ItemName)
        Dim Value As String

        For i = 1 To Coll.Count
            Value &= Coll(i) & "|"
        Next
        Return Value
        Value = Value.Substring(0, Value.Length - 1)
        Return Value
    End Function

    Public Function AttachChildTables(ByVal ItemName As String, ByVal DataFldName As String, ByVal PermissionFldName As String) As DG.ChildTablesClass
        cChildTables = New ChildTablesClass(ItemName, DataFldName, PermissionFldName)
        Return cChildTables
    End Function
#End Region

#Region " GetText et al "
    'Public Function OldGetText(ByRef dt As Data.DataTable) As String
    '    Dim sb As New System.Text.StringBuilder
    '    Dim OddRow As Boolean = True
    '    Dim RowNum As Integer
    '    Dim ChildText As String

    '    If cUseDefaultTableDef Then
    '        sb.Append(cDefaultColl(cTableDef))
    '    Else
    '        sb.Append(cTableDef)
    '    End If

    '    AddMenu(sb)

    '    If Not cExternalFilter Is Nothing Then
    '        AddExternalFilter(sb)
    '    End If

    '    AddHeaderRow(sb)

    '    If cInternalFilterOperationMode <> FilterOperationModeEnum.NoFilter Then
    '        AddFilter(sb, "filter")
    '    End If

    '    '  For RowNum = 0 To dt.Rows.Count - 1
    '    '  AddDataRow(dt, RowNum, OddRow, sb)
    '    '  OddRow = Not OddRow
    '    '  Next

    '    'For RowNum = 0 To dt.Rows.Count - 1
    '    '    AddDataRow(dt, RowNum, OddRow, sb)
    '    '    If Not cChildTables Is Nothing AndAlso dt.Rows(RowNum)(cColumnColl(1).DataFldName) = cChildTables.Value AndAlso dt.Rows(RowNum)(cColumnColl(1).EnableFldName) = 1 Then
    '    '        RaiseEvent ChildDTRequest(ChildText, cChildTables.DataFldName, cChildTables.Value)
    '    '        sb.Append("<tr><td width='20px'>&nbsp;</td><td colspan='" & cColumnColl.Count - 1 & "'>" & ChildText & "</td></tr>")
    '    '        sb.Append("<td><td>&nbsp;</td></tr>")
    '    '    Else
    '    '        If Not cFormatAsSubTable Then
    '    '            OddRow = Not OddRow
    '    '        End If
    '    '    End If
    '    'Next

    '    For RowNum = 0 To dt.Rows.Count - 1
    '        AddDataRow(dt, RowNum, OddRow, sb)
    '        If Not cChildTables Is Nothing AndAlso dt.Rows(RowNum)(cColumnColl(1).DataFldName) = cChildTables.ChildTableSelectColumn.ParmColl("DataFldName").Value AndAlso dt.Rows(RowNum)(cColumnColl(1).EnableFldName) = 1 Then
    '            ' Parms.Add(cChildTables.ChildTableSelectColumn.ParmColl("DataFldName").Value, "DataFldName")

    '            Dim i As Integer
    '            Dim EventQualify As Boolean = True

    '            If cChildTables Is Nothing Then
    '                EventQualify = False
    '            End If

    '            If EventQualify AndAlso (Not dt.Rows(RowNum)(cColumnColl(1).DataFldName) = cChildTables.ChildTableSelectColumn.ParmColl("DataFldName").Value) Then
    '                EventQualify = False
    '            End If

    '            If EventQualify AndAlso (Not dt.Rows(RowNum)(cColumnColl(1).EnableFldName) = 1) Then
    '                EventQualify = False
    '            End If

    '            Dim FldName As String
    '            If EventQualify Then
    '                For i = 1 To cChildTables.ChildTableSelectColumn.ParmColl.Count
    '                    FldName = cChildTables.ChildTableSelectColumn.ParmColl(i).FldName
    '                    If (Not dt.Rows(RowNum)(FldName) = cChildTables.ChildTableSelectColumn.ParmColl(i).Value) Then
    '                        EventQualify = False
    '                        Exit For
    '                    End If
    '                Next
    '            End If

    '            If EventQualify Then
    '                RaiseEvent ChildDTRequest(ChildText, cChildTables.DataFldName, cChildTables.ChildTableSelectColumn.ParmColl("DataFldName").Value)
    '            End If

    '            'RaiseEvent NewChildDTRequest(cChildTables.ChildTableSelectColumn.ParmColl)


    '            'RaiseEvent ChildDTRequest(ChildText, cChildTables.DataFldName, cChildTables.ChildTableSelectColumn.ParmColl("DataFldName").Value)
    '            'Select Case cChildTables.ChildTableSelectColumn.ParmColl.Count
    '            '    Case 1
    '            '        RaiseEvent ChildDTRequest(ChildText, cChildTables.DataFldName, cChildTables.ChildTableSelectColumn.ParmColl("DataFldName").Value)
    '            '    Case 2
    '            '        RaiseEvent ChildDTRequest2(ChildText, cChildTables.DataFldName, cChildTables.ChildTableSelectColumn.ParmColl("DataFldName").Value, cChildTables.ChildTableSelectColumn.ParmColl("Parm2").Value)
    '            '    Case 3
    '            '        RaiseEvent ChildDTRequest3(ChildText, cChildTables.DataFldName, cChildTables.ChildTableSelectColumn.ParmColl("DataFldName").Value, cChildTables.ChildTableSelectColumn.ParmColl("Parm2").Value, cChildTables.ChildTableSelectColumn.ParmColl("Parm3").Value)
    '            '    Case 4
    '            '        RaiseEvent ChildDTRequest4(ChildText, cChildTables.DataFldName, cChildTables.ChildTableSelectColumn.ParmColl("DataFldName").Value, cChildTables.ChildTableSelectColumn.ParmColl("Parm2").Value, cChildTables.ChildTableSelectColumn.ParmColl("Parm3").Value, cChildTables.ChildTableSelectColumn.ParmColl("Parm4").Value)
    '            'End Select
    '            sb.Append("<tr><td width='20px'>&nbsp;</td><td colspan='" & cColumnColl.Count - 1 & "'>" & ChildText & "</td></tr>")
    '            sb.Append("<td><td>&nbsp;</td></tr>")
    '        Else
    '            If Not cFormatAsSubTable Then
    '                OddRow = Not OddRow
    '            End If
    '        End If
    '    Next


    '    sb.Append("</table>")
    '    Return sb.ToString
    'End Function

    Public Function GetChildTableText(ByRef dt As Data.DataTable) As String
        Dim i As Integer
        Dim sb As New System.Text.StringBuilder
        Dim OddRow As Boolean = True

        sb.Append("<table  cellSpacing='0' cellPadding='0' width='100%' border='0'>")
        sb.Append("<tr><td>&nbsp;</td></tr>")
        sb.Append(AffixHeaderRow)
        For i = 0 To dt.Rows.Count - 1
            AffixDataRow(dt, i, OddRow, sb)
            OddRow = Not OddRow
        Next
        sb.Append("</table>")
        Return sb.ToString
    End Function

    Public Function GetText(ByRef dt As Data.DataTable) As String
        Dim sb As New System.Text.StringBuilder
        Dim OddRow As Boolean = True
        Dim RowNum As Integer
        Dim ChildText As String
        Dim ShowChildTable As Boolean

        ' ___ Level 1 table: Defined in page

        ' ___ Level 2 table
        If cUseDefaultTableDef Then
            sb.Append(cDefaultColl(cTableDef))
        Else
            sb.Append(cTableDef)
        End If

        sb.Append(AffixMenuBand)

        sb.Append(AffixExternalFilterBand)

        ' ___ Level 3 table
        sb.Append("<tr class=""dgh""><td><table  cellSpacing='0' cellPadding='0' width='100%' border='0'>")

        sb.Append(AffixHeaderRow)

        sb.Append(AffixInternalFilter)

        For RowNum = 0 To dt.Rows.Count - 1
            ShowChildTable = AffixDataRow(dt, RowNum, OddRow, sb)
            If ShowChildTable Then
                RaiseEvent ChildDTRequest(ChildText, cChildTables.DataFldName, cChildTables.ChildTableSelectColumn.ParmColl("DataFldName").Value)
                sb.Append("<tr><td width='20px'>&nbsp;</td><td colspan='" & cColumnColl.Count - 1 & "'>" & ChildText & "</td></tr>")
                sb.Append("<td>&nbsp;</td></tr>")
            Else
                If Not cFormatAsSubTable Then
                    OddRow = Not OddRow
                End If
            End If
        Next

        ' ___ Close level 3 table
        sb.Append("</table>")

        ' ___ Close level 2 table
        sb.Append("</table>")

        Return sb.ToString
    End Function

    'Private Function EventQualify(ByRef dt As DataTable, ByVal RowNum As Integer) As Boolean
    '    Dim i As Integer
    '    Dim RunningQualify As Boolean
    '    Dim FldName As String

    '    If (Not cChildTables Is Nothing) AndAlso (dt.Rows(RowNum)(cColumnColl(1).EnableFldName) = 1) Then
    '        RunningQualify = True
    '        If RunningQualify Then
    '            For i = 1 To cChildTables.ChildTableSelectColumn.ParmColl.Count
    '                FldName = cChildTables.ChildTableSelectColumn.ParmColl(i).FldName
    '                If (Not dt.Rows(RowNum)(FldName) = cChildTables.ChildTableSelectColumn.ParmColl(i).Value) Then
    '                    RunningQualify = False
    '                    Exit For
    '                End If
    '            Next
    '        End If
    '    End If
    '    Return RunningQualify
    'End Function


    'Private Sub SAFE2AddMenu(ByRef sb As System.Text.StringBuilder)
    '    Dim i As Integer
    '    Dim ShowHideFilter As String
    '    Dim DisplayNewButton As Boolean
    '    Dim sbMenu As New System.Text.StringBuilder
    '    Dim InternalFilterExists As Boolean
    '    Dim ExternalFilterExists As Boolean
    '    Dim MenuExists As Boolean
    '    Dim ItemAdded As Boolean
    '    Dim ColNum As Integer
    '    Dim ColCount As Integer

    '    ' ___ How this works
    '    ' If a filter exists, the menu displays the Go link, then the Show/Hide filter link.
    '    ' If a shortcut new button is present, it appears next.
    '    ' All items listed in the menu object appear next.
    '    ' Whether or not any menu items appear, this methods adds a 30px row.

    '    ' ___ Get the column count
    '    For ColNum = 1 To cColumnColl.Count
    '        If cColumnColl(ColNum).ColumnType <> DG.ColumnType.Hidden Then
    '            If cColumnColl(ColNum).Visible Then
    '                If cColumnColl(ColNum).ColumnType <> DG.ColumnType.Template Then
    '                    ColCount += 1
    '                End If
    '            End If
    '        End If
    '    Next

    '    If Not cInternalFilter Is Nothing Then
    '        InternalFilterExists = True
    '    End If

    '    If Not cExternalFilter Is Nothing Then
    '        ExternalFilterExists = True
    '    End If

    '    If Not cMenu Is Nothing Then
    '        MenuExists = True
    '    End If

    '    If InternalFilterExists Then
    '        If cShowInternalFilter Then
    '            ShowHideFilter = "Hide Filter"
    '        Else
    '            ShowHideFilter = "Show Filter"
    '        End If
    '    End If

    '    If cAttachNewButton Then
    '        If cNewButtonRight = Nothing Then
    '            DisplayNewButton = True
    '        Else
    '            If Rights.HasThisRight(cNewButtonRight) Then
    '                DisplayNewButton = True
    '            End If
    '        End If
    '    End If

    '    If InternalFilterExists Then
    '        sbMenu.Append("<a href=""javascript:ApplyFilter()"">Go</a>&nbsp;&nbsp;&nbsp;<a href=""javascript:ToggleShowFilter()"">" & ShowHideFilter & "</a>")
    '    End If

    '    If DisplayNewButton Then
    '        If InternalFilterExists Then
    '            sbMenu.Append("&nbsp;&nbsp;&nbsp;")
    '        End If
    '        sbMenu.Append("<a href='javascript:NewRecord()'>New</a>")
    '    End If

    '    If MenuExists Then
    '        If InternalFilterExists Or DisplayNewButton Then
    '            sbMenu.Append("&nbsp;&nbsp;&nbsp;")
    '        End If

    '        For i = 1 To cMenu.Coll.count
    '            If cMenu.Coll(i).IsVisible And Rights.HasThisRight(cMenu.Coll(i).Right) Then
    '                If ItemAdded Then
    '                    sbMenu.Append("&nbsp;&nbsp;&nbsp;")
    '                End If
    '                If cMenu.Coll(i).IsLink Then
    '                    sbMenu.Append("<a href='javascript:" & cMenu.Coll(i).OnClickMethod & "()'>" & cMenu.Coll(i).Text & "</a>")
    '                ElseIf cMenu.Coll(i).IsButton Then
    '                    sbMenu.Append("<input onclick='" & cMenu.Coll(i).OnClickMethod & "()' type='button' value='" & cMenu.Coll(i).Text & "'>")
    '                End If
    '                ItemAdded = True
    '            End If
    '        Next
    '    End If


    '    'sb.Append("<tr height='30px' class='dgh'>")
    '    ''   sb.Append("<tr class='dgh'>")
    '    'If FilterExists Or MenuExists Or DisplayNewButton Then
    '    '    sb.Append("<td align='left' colspan='" & ColCount - 1 & "'>" & sbMenu.ToString & "</td>")
    '    'Else
    '    '    sb.Append("<td align='left' colspan='" & ColCount - 1 & "'>&nbsp;</td>")
    '    'End If
    '    'sb.Append("</tr>")

    '    sb.Append("<tr height='20px' class='dgh'>")
    '    If InternalFilterExists Or MenuExists Or DisplayNewButton Then
    '        sb.Append("<td align='left' colspan='" & ColCount - 1 & "'>" & sbMenu.ToString & "</td>")
    '    Else
    '        sb.Append("<td align='left' colspan='" & ColCount - 1 & "'>&nbsp;</td>")
    '    End If
    '    sb.Append("</tr>")

    'End Sub

    Private Function AffixMenuBand() As String
        Dim i As Integer
        Dim ShowHideFilter As String
        Dim DisplayNewButton As Boolean
        Dim sbMenu As New System.Text.StringBuilder
        Dim InternalFilterExists As Boolean
        Dim ExternalFilterExists As Boolean
        Dim MenuExists As Boolean
        ' Dim ItemAdded As Boolean
        Dim ColNum As Integer
        Dim ColCount As Integer
        Dim sb As New System.Text.StringBuilder
        Dim Width As Integer = 10

        ' ___ How this works
        ' If a filter exists, the menu displays the Go link, then the Show/Hide filter link.
        ' If a shortcut new button is present, it appears next.
        ' All items listed in the menu object appear next.
        ' Whether or not any menu items appear, this methods adds a 30px row.

        If Not cInternalFilter Is Nothing Then
            InternalFilterExists = True
        End If

        If Not cExternalFilter Is Nothing Then
            ExternalFilterExists = True
        End If

        If Not cMenu Is Nothing Then
            MenuExists = True
            Width = cMenu.CellWidthPercent
        End If

        If InternalFilterExists And Not ExternalFilterExists Then
            If cShowInternalFilter Then
                ShowHideFilter = "Hide Filter"
            Else
                ShowHideFilter = "Show Filter"
            End If
        End If

        If cAttachNewButton Then
            If cNewButtonRight = Nothing Then
                DisplayNewButton = True
            Else
                If Rights.HasThisRight(cNewButtonRight) Then
                    DisplayNewButton = True
                End If
            End If
        End If


        If InternalFilterExists And Not ExternalFilterExists Then
            sbMenu.Append("<td  align='center' width='" & Width & "%'  class=""DGMenuItem""><a href=""javascript:ApplyFilter()"">Go</a></td><td  align='center' width='" & Width & "%'  class=""DGMenuItem""><a href=""javascript:ToggleShowFilter()"">" & ShowHideFilter & "</a></td>")
        ElseIf InternalFilterExists And ExternalFilterExists Then
            sbMenu.Append("<td  align='center' width='" & Width & "%'  class=""DGMenuItem""><a href=""javascript:ApplyFilter()"">Go</a></td>")
        ElseIf Not InternalFilterExists And Not ExternalFilterExists Then
            ' No action
        ElseIf Not InternalFilterExists And ExternalFilterExists Then
            sbMenu.Append("<td  align='center' width='" & Width & "%'  class=""DGMenuItem""><a href=""javascript:ApplyFilter()"">Go</a></td>")
        End If

        If DisplayNewButton Then
            sbMenu.Append("<td  align='center' width='" & Width & "%'  class=""DGMenuItem""><a href='javascript:NewRecord()'>New</a></td>")
        End If

        If MenuExists Then

            For i = 1 To cMenu.Coll.count
                If cMenu.Coll(i).IsVisible And Rights.HasThisRight(cMenu.Coll(i).Right) Then
                    If cMenu.Coll(i).IsLink Then
                        sbMenu.Append("<td  align='center' width='" & Width & "%'  class=""DGMenuItem""><a href='javascript:" & cMenu.Coll(i).OnClickMethod & "()'>" & cMenu.Coll(i).Text & "</a></td>")
                    ElseIf cMenu.Coll(i).IsButton Then
                        sbMenu.Append("<td align='center' width='" & Width & "%'  ><input onclick='" & cMenu.Coll(i).OnClickMethod & "()' type='button' value=""" & cMenu.Coll(i).Text & """></td>")
                    End If
                End If
            Next
        End If

        sbMenu.Append("<td>&nbsp;</td>")

        'sbMenu.Length = 0
        'sbMenu.Append("<table cellspacing=""3"" cellpadding=""0"" padding=""0""><tr align=""left"">")
        'sbMenu.Append("<td width=""10%"" bgcolor=#b0e0e6 align='center' style='border-collapse:collapse;border:1px solid blue;font: 8pt Arial, Helvetica, sans-serif;' ><a style='text-decoration:none;color:black;font-family:arial;' href=""javascript:ApplyFilter()"">Go</a></td>")
        'sbMenu.Append("<td width=""10%""  bgcolor=#b0e0e6 align=""center"" style=""border-collapse:collapse;border:1px solid blue;font: 8pt Arial, Helvetica, sans-serif;"" ><a style=""text-decoration:none;color:black;font-family:arial;"" href=""javascript:ToggleShowFilter()"">Hide Filter</a></td>")
        'sbMenu.Append("<td width=""10%""  bgcolor=#b0e0e6 align=""center"" style=""border-collapse:collapse;border:1px solid blue;font: 8pt Arial, Helvetica, sans-serif;"" ><a style=""text-decoration:none;color:black;font-family:arial;"" href=""javascript:NewFeed()"">New Feed</a></td><td>&nbsp;</td>")
        'sbMenu.Append("<td width=""70%"">&nbsp;</td>")
        'sbMenu.Append("</tr></table>")

        'sbMenu.Append("<table cellspacing=""3"" cellpadding=""0"" border=""0"" padding=""0""><tr align=""left"">")
        'sbMenu.Append("<td width=""10%"" align=""center"" class=""DGMenuItem"" ><a href=""javascript:ApplyFilter()"">Go</a></td>")
        'sbMenu.Append("<td width=""10%""  bgcolor=#b0e0e6 align=""center"" style=""border-collapse:collapse;border:1px solid blue;font: 8pt Arial, Helvetica, sans-serif;"" ><a style=""text-decoration:none;color:black;font-family:arial;"" href=""javascript:ToggleShowFilter()"">Hide Filter</a></td>")
        'sbMenu.Append("<td width=""10%""  bgcolor=#b0e0e6 align=""center"" style=""border-collapse:collapse;border:1px solid blue;font: 8pt Arial, Helvetica, sans-serif;"" ><a style=""text-decoration:none;color:black;font-family:arial;"" href=""javascript:NewFeed()"">New Feed</a></td><td>&nbsp;</td>")
        'sbMenu.Append("<td width=""70%"">&nbsp;</td>")
        'sbMenu.Append("</tr></table>")

        ' sb.Append("<tr height='20px' class='dgh'><td>")
        sb.Append("<tr height='20px' align=""left""><td>")
        If InternalFilterExists Or ExternalFilterExists Or MenuExists Or DisplayNewButton Then
            sb.Append("<table cellspacing=""3"" cellpadding=""0"" padding=""0"" width=""100%"">")
            sb.Append("<tr align=""left"">" & sbMenu.ToString & "</tr></table>")
        Else
            sb.Append("<table  cellSpacing='0' cellPadding='0' width='100%' border='0'><tr><td>&nbsp;</td></tr></table>")
        End If
        sb.Append("</td></tr>")

        Return sb.ToString
    End Function

    Private Sub FriAddMenu(ByRef sb As System.Text.StringBuilder)
        Dim i As Integer
        Dim ShowHideFilter As String
        Dim DisplayNewButton As Boolean
        Dim sbMenu As New System.Text.StringBuilder
        Dim InternalFilterExists As Boolean
        Dim ExternalFilterExists As Boolean
        Dim MenuExists As Boolean
        Dim ItemAdded As Boolean
        Dim ColNum As Integer
        Dim ColCount As Integer

        Dim Style As String
        Dim Attributes As String
        Dim Style2 As String
        Dim Attribute2 As String

        Style = " style=""text-decoration:none;color:black;font-family:arial;"" "
        Attributes = " width=""10%"" bgcolor=#b0e0e6 align=""center"" style=""border-collapse:collapse;border:1px solid blue;font: 9pt Arial, Helvetica, sans-serif;"" "

        ' ___ How this works
        ' If a filter exists, the menu displays the Go link, then the Show/Hide filter link.
        ' If a shortcut new button is present, it appears next.
        ' All items listed in the menu object appear next.
        ' Whether or not any menu items appear, this methods adds a 30px row.

        ' ___ Get the column count
        For ColNum = 1 To cColumnColl.Count
            If cColumnColl(ColNum).ColumnType <> DG.ColumnType.Hidden Then
                If cColumnColl(ColNum).Visible Then
                    If cColumnColl(ColNum).ColumnType <> DG.ColumnType.Template Then
                        ColCount += 1
                    End If
                End If
            End If
        Next

        If Not cInternalFilter Is Nothing Then
            InternalFilterExists = True
        End If

        If Not cExternalFilter Is Nothing Then
            ExternalFilterExists = True
        End If

        If Not cMenu Is Nothing Then
            MenuExists = True
        End If

        If InternalFilterExists And Not ExternalFilterExists Then
            If cShowInternalFilter Then
                ShowHideFilter = "Hide Filter"
            Else
                ShowHideFilter = "Show Filter"
            End If
        End If

        If cAttachNewButton Then
            If cNewButtonRight = Nothing Then
                DisplayNewButton = True
            Else
                If Rights.HasThisRight(cNewButtonRight) Then
                    DisplayNewButton = True
                End If
            End If
        End If


        If InternalFilterExists And Not ExternalFilterExists Then
            sbMenu.Append("<a" & Style & "href=""javascript:ApplyFilter()"">Go</a><a" & Style & "href=""javascript:ToggleShowFilter()"">" & ShowHideFilter & "</a>")
        ElseIf InternalFilterExists And ExternalFilterExists Then
            sbMenu.Append("<a" & Style & "href=""javascript:ApplyFilter()"">Go</a>")
        ElseIf Not InternalFilterExists And Not ExternalFilterExists Then
            ' No action
        ElseIf Not InternalFilterExists And ExternalFilterExists Then
            sbMenu.Append("<a" & Style & "href=""javascript:ApplyFilter()"">Go</a>")
        End If

        'If InternalFilterExists Then
        '    If ExternalFilterExists Then
        '        sbMenu.Append("<a href=""javascript:ApplyFilter()"">Go</a>")
        '    Else
        '        sbMenu.Append("<a href=""javascript:ApplyFilter()"">Go</a>&nbsp;&nbsp;&nbsp;<a href=""javascript:ToggleShowFilter()"">" & ShowHideFilter & "</a>")
        '    End If
        'End If

        If DisplayNewButton Then
            If InternalFilterExists Then
                sbMenu.Append("&nbsp;&nbsp;&nbsp;")
            End If
            sbMenu.Append("<a" & Style & "href='javascript:NewRecord()'>New</a>")
        End If

        If MenuExists Then
            If InternalFilterExists Or DisplayNewButton Then
                sbMenu.Append("&nbsp;&nbsp;&nbsp;")
            End If

            For i = 1 To cMenu.Coll.count
                If cMenu.Coll(i).IsVisible And Rights.HasThisRight(cMenu.Coll(i).Right) Then
                    If ItemAdded Then
                        sbMenu.Append("&nbsp;&nbsp;&nbsp;")
                    End If
                    If cMenu.Coll(i).IsLink Then
                        sbMenu.Append("<a" & Style & "href='javascript:" & cMenu.Coll(i).OnClickMethod & "()'>" & cMenu.Coll(i).Text & "</a>")
                    ElseIf cMenu.Coll(i).IsButton Then
                        sbMenu.Append("<input onclick='" & cMenu.Coll(i).OnClickMethod & "()' type='button' value=""" & cMenu.Coll(i).Text & """>")
                    End If
                    ItemAdded = True
                End If
            Next
        End If


        'sb.Append("<tr height='30px' class='dgh'>")
        ''   sb.Append("<tr class='dgh'>")
        'If FilterExists Or MenuExists Or DisplayNewButton Then
        '    sb.Append("<td align='left' colspan='" & ColCount - 1 & "'>" & sbMenu.ToString & "</td>")
        'Else
        '    sb.Append("<td align='left' colspan='" & ColCount - 1 & "'>&nbsp;</td>")
        'End If
        'sb.Append("</tr>")

        sb.Append("<tr height='20px' class='dgh'>")
        If InternalFilterExists Or ExternalFilterExists Or MenuExists Or DisplayNewButton Then
            sb.Append("<td align='left' colspan='" & ColCount - 1 & "'>" & sbMenu.ToString & "</td>")
        Else
            sb.Append("<tr height='20px' class='dgh'>td align='left' colspan='" & ColCount - 1 & "'>&nbsp;</td><tr>")
        End If
        sb.Append("</tr>")

    End Sub

    Private Sub TrashedAddMenu(ByRef sb As System.Text.StringBuilder)
        Dim i As Integer
        Dim ShowHideFilter As String
        Dim DisplayNewButton As Boolean
        Dim sbMenu As New System.Text.StringBuilder
        Dim InternalFilterExists As Boolean
        Dim ExternalFilterExists As Boolean
        Dim MenuExists As Boolean
        Dim ItemAdded As Boolean
        Dim ColNum As Integer
        Dim ColCount As Integer

        ' ___ How this works
        ' If a filter exists, the menu displays the Go link, then the Show/Hide filter link.
        ' If a shortcut new button is present, it appears next.
        ' All items listed in the menu object appear next.
        ' Whether or not any menu items appear, this methods adds a 30px row.

        ' ___ Get the column count
        For ColNum = 1 To cColumnColl.Count
            If cColumnColl(ColNum).ColumnType <> DG.ColumnType.Hidden Then
                If cColumnColl(ColNum).Visible Then
                    If cColumnColl(ColNum).ColumnType <> DG.ColumnType.Template Then
                        ColCount += 1
                    End If
                End If
            End If
        Next

        If Not cInternalFilter Is Nothing Then
            InternalFilterExists = True
        End If

        If Not cExternalFilter Is Nothing Then
            ExternalFilterExists = True
        End If

        If Not cMenu Is Nothing Then
            MenuExists = True
        End If

        If InternalFilterExists And Not ExternalFilterExists Then
            If cShowInternalFilter Then
                ShowHideFilter = "Hide Filter"
            Else
                ShowHideFilter = "Show Filter"
            End If
        End If

        If cAttachNewButton Then
            If cNewButtonRight = Nothing Then
                DisplayNewButton = True
            Else
                If Rights.HasThisRight(cNewButtonRight) Then
                    DisplayNewButton = True
                End If
            End If
        End If

        Dim Style As String
        Dim Attributes As String
        Dim Style2 As String
        Dim Attribute2 As String

        Style = " style=""text-decoration:none;color:black;font-family:arial;"" "
        Attributes = "bgcolor=#b0e0e6 align=""center"" style=""border-collapse:collapse;border:1px solid blue;font: 9pt Arial, Helvetica, sans-serif;"" "
        '<td  bgcolor=#B0E0E6 align="center" style="border-collapse:collapse;border:1px solid blue;FONT: 9pt Arial, Helvetica, sans-serif;"><a  style="text-decoration:none;color:black;font-family:arial;" href="http://www.google.com">Go</a></td>

        Style2 = ""
        Attribute2 = ""


        If InternalFilterExists And Not ExternalFilterExists Then
            sbMenu.Append("<a" & Style & "href=""javascript:ApplyFilter()"">Go</a>&nbsp;&nbsp;&nbsp;<a" & Style & "href=""javascript:ToggleShowFilter()"">" & ShowHideFilter & "</a>")
        ElseIf InternalFilterExists And ExternalFilterExists Then
            sbMenu.Append("<a" & Style & "href=""javascript:ApplyFilter()"">Go</a>")
        ElseIf Not InternalFilterExists And Not ExternalFilterExists Then
            ' No action
        ElseIf Not InternalFilterExists And ExternalFilterExists Then
            sbMenu.Append("<a" & Style & "href=""javascript:ApplyFilter()"">Go</a>")
        End If

        'If InternalFilterExists Then
        '    If ExternalFilterExists Then
        '        sbMenu.Append("<a href=""javascript:ApplyFilter()"">Go</a>")
        '    Else
        '        sbMenu.Append("<a href=""javascript:ApplyFilter()"">Go</a>&nbsp;&nbsp;&nbsp;<a href=""javascript:ToggleShowFilter()"">" & ShowHideFilter & "</a>")
        '    End If
        'End If

        If DisplayNewButton Then
            If InternalFilterExists Then
                sbMenu.Append("&nbsp;&nbsp;&nbsp;")
            End If
            sbMenu.Append("<a" & Style & "href='javascript:NewRecord()'>New</a>")
        End If

        If MenuExists Then
            If InternalFilterExists Or DisplayNewButton Then
                sbMenu.Append("&nbsp;&nbsp;&nbsp;")
            End If

            For i = 1 To cMenu.Coll.count
                If cMenu.Coll(i).IsVisible And Rights.HasThisRight(cMenu.Coll(i).Right) Then
                    If ItemAdded Then
                        sbMenu.Append("&nbsp;&nbsp;&nbsp;")
                    End If
                    If cMenu.Coll(i).IsLink Then
                        sbMenu.Append("<a" & Style & "href='javascript:" & cMenu.Coll(i).OnClickMethod & "()'>" & cMenu.Coll(i).Text & "</a>")
                    ElseIf cMenu.Coll(i).IsButton Then
                        sbMenu.Append("<input onclick='" & cMenu.Coll(i).OnClickMethod & "()' type='button' value=""" & cMenu.Coll(i).Text & """>")
                    End If
                    ItemAdded = True
                End If
            Next
        End If


        'sb.Append("<tr height='30px' class='dgh'>")
        ''   sb.Append("<tr class='dgh'>")
        'If FilterExists Or MenuExists Or DisplayNewButton Then
        '    sb.Append("<td align='left' colspan='" & ColCount - 1 & "'>" & sbMenu.ToString & "</td>")
        'Else
        '    sb.Append("<td align='left' colspan='" & ColCount - 1 & "'>&nbsp;</td>")
        'End If
        'sb.Append("</tr>")

        sb.Append("<tr height='20px' class='dgh'>")
        If InternalFilterExists Or ExternalFilterExists Or MenuExists Or DisplayNewButton Then
            sb.Append("<td align='left'" & Attributes & "colspan='" & ColCount - 1 & "'>" & sbMenu.ToString & "</td>")
        Else
            sb.Append("<td align='left' colspan='" & ColCount - 1 & "'>&nbsp;</td>")
        End If
        sb.Append("</tr>")

    End Sub


    Private Function AffixHeaderRow() As String
        Dim ColNum As Integer
        Dim DataFldName As String
        Dim HeaderText As String = String.Empty
        Dim Width As String
        'Dim LeftMost As Boolean
        Dim sb As New System.Text.StringBuilder

        'sb.Append("<tr class=""dgh""><td><table  cellSpacing='0' cellPadding='0' width='100%' border='0'>")

        'LeftMost = True
        sb.Append("<tr class=""dgh"">")

        For ColNum = 1 To cColumnColl.Count

            If cColumnColl(ColNum).ColumnType <> DG.ColumnType.Hidden Then

                If cColumnColl(ColNum).Visible Then
                    If cColumnColl(ColNum).ColumnType <> DG.ColumnType.Template Then
                        DataFldName = cColumnColl(ColNum).DataFldName
                        If cColumnColl(ColNum).HeaderText = Nothing Then
                            HeaderText = "&nbsp;"
                        Else
                            HeaderText = cColumnColl(ColNum).HeaderText
                        End If

                        '' ___ td start tag
                        'sb.Append("<td " & cColumnColl(ColNum).Attributes)
                        ''If LeftMost Then
                        ''    sb.Append(cLeftPadding)
                        ''    LeftMost = False
                        ''End If
                        'sb.Append(">")

                        '' ___ Value
                        'If cColumnColl(ColNum).SortExpression = Nothing Then
                        '    sb.Append(HeaderText)
                        'Else
                        '    sb.Append("<a href=""javascript:Sort('" & cColumnColl(ColNum).SortExpression & "')"">" & HeaderText & "</a>")
                        'End If

                        ' ___ td end tag
                        'sb.Append("</td>")

                        If cColumnColl(ColNum).SortExpression = Nothing Then
                            sb.Append("<td " & cColumnColl(ColNum).Attributes & ">" & HeaderText & "</td>")
                        Else
                            sb.Append("<td " & cColumnColl(ColNum).Attributes & "><a href=""javascript:Sort('" & cColumnColl(ColNum).SortExpression & "')"">" & HeaderText & "</a></td>")
                        End If

                    Else
                        HeaderText = cColumnColl(ColNum).HeaderText
                        sb.Append("<td>" & HeaderText & "</td>")

                        'If LeftMost Then
                        '    sb.Append("<td" & cLeftPadding & ">" & HeaderText & "</td>")
                        '    LeftMost = False
                        'Else
                        '    sb.Append("<td>" & HeaderText & "</td>")
                        'End If

                    End If
                End If

            End If

        Next

        sb.Append("</tr>")

        Return sb.ToString
    End Function

    Private Function AffixInternalFilter() As String
        Dim i, j As Integer
        Dim ColNum As Integer
        Dim ItemName As String
        Dim ColCount As Integer
        Dim ColumnHasFilter As Boolean
        Dim DropdownColl As Collection
        Dim SelectedValue As String
        Dim sb As New System.Text.StringBuilder

        ' ___ Is there an internal filter?
        If cInternalFilterOperationMode = FilterOperationModeEnum.NoFilter Then
            Exit Function
        End If

        ' ___ Are there any filter items?
        If cInternalFilter.Coll.count = 0 Then
            Exit Function
        End If

        ' ___ Get the column count
        For ColNum = 1 To cColumnColl.Count
            If cColumnColl(ColNum).ColumnType <> DG.ColumnType.Hidden Then
                If cColumnColl(ColNum).Visible Then
                    If cColumnColl(ColNum).ColumnType <> DG.ColumnType.Template Then
                        ColCount += 1
                    End If
                End If
            End If
        Next

        If cShowInternalFilter Then
            sb.Append("<tr class=""dgh"">")
            For ColNum = 1 To cColumnColl.Count
                If cColumnColl(ColNum).ColumnType <> DG.ColumnType.Hidden Then
                    If cColumnColl(ColNum).Visible Then
                        If cColumnColl(ColNum).ColumnType <> DG.ColumnType.Template Then

                            ItemName = cColumnColl(ColNum).ItemName.toupper
                            ColumnHasFilter = False
                            For i = 1 To cInternalFilter.Coll.count
                                If cInternalFilter.Coll(i).ItemName.toupper = ItemName Then
                                    ColumnHasFilter = True
                                    Exit For
                                End If
                            Next

                            If ColumnHasFilter Then
                                If cInternalFilter.Coll(ItemName).IsTextbox Then
                                    sb.Append("<td " & cColumnColl(ColNum).Attributes & "><input style='FONT: 8pt Arial, Helvetica, sans-serif;' type='text' name='" & cInternalFilter.Coll(ItemName).CtlName & "' id='" & cInternalFilter.Coll(ItemName).CtlName & "' value=""" & cInternalFilter.Coll(ItemName).GetValue() & """ maxlength='" & cInternalFilter.Coll(ItemName).MaxLength & "' onkeypress='return SubmitOnEnterKey(event)'></td>")
                                ElseIf cInternalFilter.Coll(ItemName).IsDropdown Then
                                    DropdownColl = cInternalFilter.Coll(ItemName).Coll
                                    sb.Append("<td " & cColumnColl(ColNum).Attributes & "><select style='FONT: 8pt Arial, Helvetica, sans-serif;' name='" & cInternalFilter.Coll(ItemName).CtlName & "' id='" & cInternalFilter.Coll(ItemName).CtlName & "' value=""" & cInternalFilter.Coll(ItemName).GetValue() & """>")
                                    For j = 1 To DropdownColl.Count
                                        SelectedValue = cInternalFilter.Coll(ItemName).GetValue()
                                        If DropdownColl(j).Value = SelectedValue Then
                                            sb.Append("<option selected value=""" & DropdownColl(j).Value & """>" & DropdownColl(j).Text & "</option>")
                                        Else
                                            sb.Append("<option value=""" & DropdownColl(j).Value & """>" & DropdownColl(j).Text & "</option>")
                                        End If
                                    Next
                                    sb.Append("</select>")
                                End If

                            Else
                                sb.Append("<td>&nbsp;</td>")
                            End If

                        End If
                    End If
                End If
            Next
            sb.Append("</tr>")
        End If

        sb.Append("<tr class=""dgh"">")

        Return sb.ToString
    End Function

    'Private Function AffixExternalFilterBand() As String
    '    Dim i, j As Integer
    '    Dim ColNum As Integer
    '    Dim ItemName As String
    '    Dim CtlName As String
    '    Dim Width As String
    '    Dim ColCount As Integer
    '    Dim ShowHideFilter As String
    '    Dim ColumnHasFilter As Boolean
    '    Dim DropdownColl As Collection
    '    Dim SelectedValue As String
    '    Dim DisplayNewButton As Boolean
    '    Dim HeaderLinkText As String
    '    Dim sb As New System.Text.StringBuilder

    '    ' ___ Exit if no external filter
    '    If cExternalFilter Is Nothing Then
    '        Exit Function
    '    End If

    '    ' ___ Exit if there are not any filter items
    '    If cExternalFilter.Coll.count = 0 Then
    '        Exit Function
    '    End If

    '    ' ___ Get the column count
    '    For ColNum = 1 To cColumnColl.Count
    '        If cColumnColl(ColNum).ColumnType <> DG.ColumnType.Hidden Then
    '            If cColumnColl(ColNum).Visible Then
    '                If cColumnColl(ColNum).ColumnType <> DG.ColumnType.Template Then
    '                    ColCount += 1
    '                End If
    '            End If
    '        End If
    '    Next

    '    sb.Append("<table  cellSpacing='0' cellPadding='0' border='0'>")
    '    sb.Append("<tr class=""dgh"" height=""30px"" align=""bottom""><td align=""left"">")

    '    For ColNum = 1 To cColumnColl.Count
    '        If cColumnColl(ColNum).ColumnType <> DG.ColumnType.Hidden Then
    '            If cColumnColl(ColNum).Visible Then
    '                If cColumnColl(ColNum).ColumnType <> DG.ColumnType.Template Then

    '                    ColumnHasFilter = False
    '                    For i = 1 To cExternalFilter.Coll.count
    '                        If cExternalFilter.Coll(i).Position = ColNum Then
    '                            ColumnHasFilter = True
    '                            ItemName = cExternalFilter.Coll(i).ItemName
    '                            Exit For
    '                        End If
    '                    Next

    '                    If ColumnHasFilter Then
    '                        If cExternalFilter.Coll(ItemName).IsTextbox Then
    '                            sb.Append("<td " & cColumnColl(ColNum).Attributes & "><input style='FONT: 8pt Arial, Helvetica, sans-serif;' type='text' name='" & cExternalFilter.Coll(ItemName).CtlName & "' id='" & cExternalFilter.Coll(ItemName).CtlName & "' value='" & cExternalFilter.Coll(ItemName).GetValue() & "' maxlength='" & cExternalFilter.Coll(ItemName).MaxLength & "' onkeypress='return SubmitOnEnterKey(event)'></td>")


    '                        ElseIf cExternalFilter.Coll(ItemName).IsDropdown Then
    '                            DropdownColl = cExternalFilter.Coll(ItemName).Coll
    '                            sb.Append("<td " & cColumnColl(ColNum).Attributes & "><select style='FONT: 8pt Arial, Helvetica, sans-serif;' name='" & cExternalFilter.Coll(ItemName).CtlName & "' id='" & cExternalFilter.Coll(ItemName).CtlName & "'")
    '                            If cExternalFilter.Coll(ItemName).EventString = Nothing Then
    '                                sb.Append(">")
    '                            Else
    '                                sb.Append(" " & cExternalFilter.Coll(ItemName).EventString & ">")
    '                            End If

    '                            For j = 1 To DropdownColl.Count
    '                                SelectedValue = cExternalFilter.Coll(ItemName).GetValue()
    '                                If DropdownColl(j).Value = SelectedValue Then
    '                                    sb.Append("<option selected value='" & DropdownColl(j).Value & "'>" & DropdownColl(j).Text & "</option>")
    '                                Else
    '                                    sb.Append("<option value='" & DropdownColl(j).Value & "'>" & DropdownColl(j).Text & "</option>")
    '                                End If
    '                            Next
    '                            sb.Append("</select>")

    '                        ElseIf cExternalFilter.Coll(ItemName).IsLink Then
    '                            sb.Append("<td><a href=""javascript:" & cExternalFilter.Coll(ItemName).EventString & "()"">" & cExternalFilter.Coll(ItemName).Text & "</a></td>")
    '                        End If

    '                    Else
    '                        sb.Append("&nbsp;")
    '                    End If

    '                End If
    '            End If
    '        End If
    '    Next
    '    sb.Append("</td></tr></table>")

    '    ' sb.Append("<tr class=""dgh"">")
    '    'sb.Append("</table>")

    '    Return sb.ToString
    'End Function

    Private Function AffixExternalFilterBand() As String
        Dim i, j As Integer
        Dim ColNum As Integer
        Dim ItemName As String
        Dim CtlName As String
        Dim Width As String
        Dim ColCount As Integer
        Dim ShowHideFilter As String
        Dim ColumnHasFilter As Boolean
        Dim DropdownColl As Collection
        Dim SelectedValue As String
        Dim DisplayNewButton As Boolean
        Dim HeaderLinkText As String
        Dim sb As New System.Text.StringBuilder

        ' ___ Exit if no external filter
        If cExternalFilter Is Nothing Then
            Exit Function
        End If

        ' ___ Exit if there are not any filter items
        If cExternalFilter.Coll.count = 0 Then
            Exit Function
        End If

        ' ___ Get the column count
        For ColNum = 1 To cColumnColl.Count
            If cColumnColl(ColNum).ColumnType <> DG.ColumnType.Hidden Then
                If cColumnColl(ColNum).Visible Then
                    If cColumnColl(ColNum).ColumnType <> DG.ColumnType.Template Then
                        ColCount += 1
                    End If
                End If
            End If
        Next

        sb.Append("<table  cellSpacing='0' cellPadding='0' width='100%' border='0'>")
        sb.Append("<tr class=""dgh"" height=""30px"" align=""bottom""><td align=""left"">")
        'sb.Append("<tr class=""dgh""><td>")

        For ColNum = 1 To cColumnColl.Count
            If cColumnColl(ColNum).ColumnType <> DG.ColumnType.Hidden Then
                If cColumnColl(ColNum).Visible Then
                    If cColumnColl(ColNum).ColumnType <> DG.ColumnType.Template Then

                        'ItemName = cColumnColl(ColNum).ItemName.toupper
                        ColumnHasFilter = False
                        For i = 1 To cExternalFilter.Coll.count
                            If cExternalFilter.Coll(i).Position = ColNum Then
                                ColumnHasFilter = True
                                ItemName = cExternalFilter.Coll(i).ItemName
                                Exit For
                            End If
                        Next

                        If ColumnHasFilter Then
                            If cExternalFilter.Coll(ItemName).IsTextbox Then
                                'sb.Append("<td " & cColumnColl(ColNum).Attributes & "><input style='FONT: 8pt Arial, Helvetica, sans-serif;' type='text' name='" & cExternalFilter.Coll(ItemName).CtlName & "' id='" & cExternalFilter.Coll(ItemName).CtlName & "' value='" & cExternalFilter.Coll(ItemName).GetValue() & "' maxlength='" & cExternalFilter.Coll(ItemName).MaxLength & "' onkeypress='return SubmitOnEnterKey(event)'></td>")
                                sb.Append("<input style='FONT: 8pt Arial, Helvetica, sans-serif;' type='text' name='" & cExternalFilter.Coll(ItemName).CtlName & "' id='" & cExternalFilter.Coll(ItemName).CtlName & "' value=""" & cExternalFilter.Coll(ItemName).GetValue() & """ maxlength='" & cExternalFilter.Coll(ItemName).MaxLength & "' onkeypress='return SubmitOnEnterKey(event)'>&nbsp;&nbsp;&nbsp;&nbsp;")


                            ElseIf cExternalFilter.Coll(ItemName).IsDropdown Then
                                DropdownColl = cExternalFilter.Coll(ItemName).Coll
                                'sb.Append("<td " & cColumnColl(ColNum).Attributes & "><select style='FONT: 8pt Arial, Helvetica, sans-serif;' name='" & cExternalFilter.Coll(ItemName).CtlName & "' id='" & cExternalFilter.Coll(ItemName).CtlName & "' value='" & cExternalFilter.Coll(ItemName).GetValue() & "'>")
                                'sb.Append("<td " & cColumnColl(ColNum).Attributes & "><select style='FONT: 8pt Arial, Helvetica, sans-serif;' name='" & cExternalFilter.Coll(ItemName).CtlName & "' id='" & cExternalFilter.Coll(ItemName).CtlName & "'")
                                sb.Append("<select style='FONT: 8pt Arial, Helvetica, sans-serif;' name='" & cExternalFilter.Coll(ItemName).CtlName & "' id='" & cExternalFilter.Coll(ItemName).CtlName & "'")
                                If cExternalFilter.Coll(ItemName).EventString = Nothing Then
                                    sb.Append(">")
                                Else
                                    sb.Append(" " & cExternalFilter.Coll(ItemName).EventString & ">")
                                End If

                                For j = 1 To DropdownColl.Count
                                    SelectedValue = cExternalFilter.Coll(ItemName).GetValue()
                                    If DropdownColl(j).Value = SelectedValue Then
                                        sb.Append("<option selected value=""" & DropdownColl(j).Value & """>" & DropdownColl(j).Text & "</option>")
                                    Else
                                        sb.Append("<option value=""" & DropdownColl(j).Value & """>" & DropdownColl(j).Text & "</option>")
                                    End If
                                Next
                                sb.Append("</select>&nbsp;&nbsp;&nbsp;&nbsp;")

                            ElseIf cExternalFilter.Coll(ItemName).IsLink Then
                                '  sb.Append("<td><a href=""javascript:ApplyFilter()"">" & cExternalFilter.Coll(ItemName).Text & "</a></td>")
                                'sb.Append("<td><a href=""javascript:" & cExternalFilter.Coll(ItemName).EventString & "()"">" & cExternalFilter.Coll(ItemName).Text & "</a></td>")
                                sb.Append("<a href=""javascript:" & cExternalFilter.Coll(ItemName).EventString & "()"">" & cExternalFilter.Coll(ItemName).Text & "</a>&nbsp;&nbsp;&nbsp;&nbsp;")
                            End If

                        Else
                            sb.Append("&nbsp;")
                        End If

                    End If
                End If
            End If
        Next
        sb.Append("</td></tr></table>")

        ' sb.Append("<tr class=""dgh"">")
        'sb.Append("</table>")

        Return sb.ToString
    End Function

    'Private Sub AddDataRow(ByRef dt As Data.DataTable, ByVal RowNum As Integer, ByVal OddRow As Boolean, ByRef sb As System.Text.StringBuilder)
    '    Dim ColNum As Integer

    '    If OddRow Then
    '        sb.Append("<tr class=""DGOdd"">" & vbCrLf)
    '    Else
    '        sb.Append("<tr class=""DGEven"">" & vbCrLf)
    '    End If

    '    For ColNum = 1 To cColumnColl.Count

    '        If cColumnColl(ColNum).ColumnType = DG.ColumnType.Hidden Then
    '            HandleNonTemplateColumns(ColNum, dt, RowNum, sb)
    '        Else
    '            If cColumnColl(ColNum).Visible Then
    '                If cColumnColl(ColNum).ColumnType <> DG.ColumnType.Template Then
    '                    HandleNonTemplateColumns(ColNum, dt, RowNum, sb)
    '                Else
    '                    HandleTemplateColumn(ColNum, dt, RowNum, sb)
    '                End If
    '            End If
    '        End If

    '    Next
    '    sb.Append("</tr>")
    'End Sub

    Private Function AffixDataRow(ByRef dt As Data.DataTable, ByVal RowNum As Integer, ByVal OddRow As Boolean, ByRef sb As System.Text.StringBuilder) As Boolean
        Dim ColNum As Integer
        Dim ShowChildTable As Boolean

        If OddRow Then
            sb.Append("<tr class=""DGOdd"">" & vbCrLf)
        Else
            sb.Append("<tr class=""DGEven"">" & vbCrLf)
        End If

        For ColNum = 1 To cColumnColl.Count

            If cColumnColl(ColNum).ColumnType = DG.ColumnType.Hidden Then
                HandleHiddenColumn(ColNum, dt, RowNum, sb)
            Else
                If cColumnColl(ColNum).Visible Then
                    Select Case cColumnColl(ColNum).ColumnType
                        Case DG.ColumnType.Databound
                            HandleDataboundColumn(ColNum, dt, RowNum, sb)
                        Case DG.ColumnType.Link
                            HandleLinkColumn(ColNum, dt, RowNum, sb)
                        Case DG.ColumnType.CheckboxToggle
                            HandleCheckboxToggleColumn(ColNum, dt, RowNum, sb)
                        Case DG.ColumnType.Template
                            HandleTemplateColumn(ColNum, dt, RowNum, sb)
                        Case DG.ColumnType.ChildTableSelect
                            ShowChildTable = HandleChildTableSelectColumn(ColNum, dt, RowNum, sb)
                        Case DG.ColumnType.Boolean
                            HandleBooleanColumn(ColNum, dt, RowNum, sb)
                        Case DG.ColumnType.Date
                            HandleDateColumn(ColNum, dt, RowNum, sb)
                        Case DG.ColumnType.FreeForm
                            HandleFreeFormColumn(ColNum, dt, RowNum, sb)
                    End Select
                End If
            End If

        Next
        sb.Append("</tr>")

        Return ShowChildTable
    End Function

    Private Sub HandleCheckboxToggleColumn(ByVal ColNum As Integer, ByRef dt As Data.DataTable, ByVal RowNum As Integer, ByRef sb As System.Text.StringBuilder)
        Dim Value As String
        Dim ColumnAttributes As String
        Dim CheckboxLabel As String

        ' __ Handle column attributes and tooltip
        ColumnAttributes = cColumnColl(ColNum).Attributes
        If cColumnColl(ColNum).TitleFldName <> Nothing Then
            ' ColumnAttributes &= " title='" & Common.StrInHandler(objDataReader(cColumnColl(i).TitleFldName)) & "' "
            ColumnAttributes &= " title='" & Common.StrInHandler(dt.Rows(RowNum)(cColumnColl(ColNum).TitleFldName)) & "' "
        End If

        If dt.Rows(RowNum)(cColumnColl(ColNum).TestFld) = "1" Then
            CheckboxLabel = cColumnColl(ColNum).TrueText
        Else
            CheckboxLabel = cColumnColl(ColNum).FalseText
        End If

        Value = "<input type='checkbox' name='" & cColumnColl(ColNum).ItemName & "|" & dt.Rows(RowNum)(cKeyFieldName) & "' value='on'>&nbsp;" & CheckboxLabel
        'Value &= "<input type='hidden' name='hd" & cColumnColl(i).ItemName & "|" & dt.Rows(RowNum)(cKeyFieldName) & " value='" & dt.Rows(RowNum)(cColumnColl(i).DataFldName) & "'>"

        ' ___ This is provided as an alternative way of iterating through the checkbox items.
        cCheckboxToggleColl(cColumnColl(ColNum).ItemName).append(dt.Rows(RowNum)(cKeyFieldName) & "~" & dt.Rows(RowNum)(cColumnColl(ColNum).DataFldName) & "|")
        sb.Append("<td " & ColumnAttributes & ">" & Value & "</td>")
    End Sub

    Private Sub HandleLinkColumn(ByVal ColNum As Integer, ByRef dt As Data.DataTable, ByVal RowNum As Integer, ByRef sb As System.Text.StringBuilder)
        Dim Value As String
        Dim ColumnAttributes As String
        Dim Box As Object
        Dim DisplayLinkAsLink As Boolean
        Dim DisplayLinkAsText As Boolean
        Dim DataFldName As String

        ' __ Handle column attributes and tooltip
        ColumnAttributes = cColumnColl(ColNum).Attributes
        If cColumnColl(ColNum).TitleFldName <> Nothing Then
            ColumnAttributes &= " title='" & Common.StrInHandler(dt.Rows(RowNum)(cColumnColl(ColNum).TitleFldName)) & "' "
        End If

        ' HRef = "javascript:fnToggle"
        ' DataFldName "ReadyForPrint|1"       Display as link
        ' DataFldName "ReadyForPrint|0"       Display as text
        ' DataFldName "ReadyForPrint"          Display as link

        If InStr(dt.Rows(RowNum)(cColumnColl(ColNum).DataFldName), "|") = 0 Then
            DisplayLinkAsText = False
            DisplayLinkAsLink = True
            DataFldName = cColumnColl(ColNum).DataFldName
        Else
            Box = Split(dt.Rows(RowNum)(cColumnColl(ColNum).DataFldName), "|")
            If Box(1) = "0" Then
                DisplayLinkAsText = True
                DisplayLinkAsLink = False
                DataFldName = Box(0)
            Else
                DisplayLinkAsText = False
                DisplayLinkAsLink = True
                DataFldName = Box(0)
            End If
        End If

        If DisplayLinkAsLink Then
            Value = cColumnColl(ColNum).Href
            If cColumnColl(ColNum).AddlParm = Nothing Then
                Value = "<a href=""javascript:" & Value & "('" & dt.Rows(RowNum)(cKeyFieldName) & "')"">" & Common.StrInHandler(dt.Rows(RowNum)(DataFldName)) & "</a>"
            Else
                Value = "<a href=""javascript:" & Value & "('" & dt.Rows(RowNum)(cKeyFieldName) & "', '" & dt.Rows(RowNum)(cColumnColl(ColNum).AddlParm) & "')"">" & Common.StrInHandler(dt.Rows(RowNum)(DataFldName)) & "</a>"
            End If
        ElseIf DisplayLinkAsText Then
            'Value = Common.StrInHandler(objDataReader(DataFldName))
            Value = Common.StrInHandler(dt.Rows(RowNum)(DataFldName))
            If Value.Length = 0 Then
                Value = "&nbsp;"
            End If
        End If
        sb.Append("<td " & ColumnAttributes & ">" & Value & "</td>")
    End Sub

    Private Sub HandleBooleanColumn(ByVal ColNum As Integer, ByRef dt As Data.DataTable, ByVal RowNum As Integer, ByRef sb As System.Text.StringBuilder)
        Dim Value As String
        Dim ColumnAttributes As String

        ' __ Handle column attributes and tooltip
        ColumnAttributes = cColumnColl(ColNum).Attributes
        If cColumnColl(ColNum).Title <> Nothing Then
            ColumnAttributes &= " title='" & Common.StrInHandler(cColumnColl(ColNum).Title) & "' "
        End If

        ' ___ Determine the value
        ' If Common.StrInHandler(dt.Rows(RowNum)(cColumnColl(ColNum).DataFldName)) = cColumnColl(ColNum).TrueValue Then
        If dt.Rows(RowNum)(cColumnColl(ColNum).DataFldName) = cColumnColl(ColNum).TrueValue Then
            Value = cColumnColl(ColNum).TrueText
        Else
            Value = cColumnColl(ColNum).FalseText
        End If

        sb.Append("<td " & ColumnAttributes & ">" & Value & "</td>")
    End Sub

    Private Sub HandleDateColumn(ByVal ColNum As Integer, ByRef dt As Data.DataTable, ByVal RowNum As Integer, ByRef sb As System.Text.StringBuilder)
        Dim OrigValue As DateTime
        Dim Value As String
        Dim ColumnAttributes As String

        ' __ Handle column attributes and tooltip
        ColumnAttributes = cColumnColl(ColNum).Attributes
        If cColumnColl(ColNum).Title <> Nothing Then
            ColumnAttributes &= " title='" & Common.StrInHandler(cColumnColl(ColNum).Title) & "' "
        End If

        ' ___ Get and format value
        Value = Common.DateInHandler(dt.Rows(RowNum)(cColumnColl(ColNum).DataFldName))
        If Value = String.Empty Then
            Value = "&nbsp;"
        Else
            OrigValue = dt.Rows(RowNum)(cColumnColl(ColNum).DataFldName)
            If cColumnColl(ColNum).DataFormatString = Nothing Then
                Value = OrigValue.ToString
            Else
                Value = OrigValue.ToString(cColumnColl(ColNum).DataFormatString)
            End If
        End If


        'If IsDBNull(dt.Rows(RowNum)(cColumnColl(ColNum).DataFldName)) Then
        '    Value = "&nbsp;"
        'Else
        '    OrigValue = dt.Rows(RowNum)(cColumnColl(ColNum).DataFldName)
        '    If cColumnColl(ColNum).DataFormatString = Nothing Then
        '        Value = OrigValue.ToString
        '    Else
        '        Value = OrigValue.ToString(cColumnColl(ColNum).DataFormatString)
        '    End If
        'End If

        sb.Append("<td " & ColumnAttributes & ">" & Value & "</td>")
    End Sub


    Private Sub HandleFreeFormColumn(ByVal ColNum As Integer, ByRef dt As Data.DataTable, ByVal RowNum As Integer, ByRef sb As System.Text.StringBuilder)
        Dim Value As String
        Dim ColumnAttributes As String

        ' __ Handle column attributes and tooltip
        ColumnAttributes = cColumnColl(ColNum).Attributes
        If cColumnColl(ColNum).Title <> Nothing Then
            ColumnAttributes &= " title='" & Common.StrInHandler(dt.Rows(RowNum)(cColumnColl(ColNum).Title)) & "' "
        End If

        ' ___ Get the value
        Value = Common.StrInHandler(cColumnColl(ColNum).CellText)

        ' ___ Format value
        If Value = Nothing Then
            Value = "&nbsp;"
        End If

        sb.Append("<td " & ColumnAttributes & ">" & Value & "</td>")
    End Sub

    Private Sub HandleDataboundColumn(ByVal ColNum As Integer, ByRef dt As Data.DataTable, ByVal RowNum As Integer, ByRef sb As System.Text.StringBuilder)
        Dim Value As String
        Dim ValueWorking As Object
        Dim ColumnAttributes As String

        ' __ Handle column attributes and tooltip
        ColumnAttributes = cColumnColl(ColNum).Attributes
        If cColumnColl(ColNum).TitleFldName <> Nothing Then
            ColumnAttributes &= " title='" & Common.StrInHandler(dt.Rows(RowNum)(cColumnColl(ColNum).TitleFldName)) & "' "
        End If

        ' ___ Get the value
        ValueWorking = Common.StrInHandler(dt.Rows(RowNum)(cColumnColl(ColNum).DataFldName))
        'Value = Common.StrInHandler(dt.Rows(RowNum)(cColumnColl(ColNum).DataFldName))

        '' ___ Handle boolean
        'If dt.Columns(cColumnColl(ColNum).DataFldName).datatype.ToString.ToLower = "system.boolean" Then
        '    If IsDBNull(dt.Rows(RowNum)(cColumnColl(ColNum).DataFldName)) Then
        '        Value = "No"
        '    Else
        '        If dt.Rows(RowNum)(cColumnColl(ColNum).DataFldName) Then
        '            Value = "Yes"
        '        Else
        '            Value = "No"
        '        End If
        '    End If
        'End If

        ' ___ Format value
        If ValueWorking.Length = 0 Then
            ValueWorking = "&nbsp;"
        Else

            Dim DataType As String

            If IsNumeric(ValueWorking) Then
                DataType = "Numeric"
            ElseIf IsDate(ValueWorking) Then
                DataType = "Date"
            End If

            If cColumnColl(ColNum).DataFormatString = Nothing Then
                Value = ValueWorking
            Else

                'Value = CDbl(Value).ToString(cColumnColl(ColNum).DataFormatString)

                If DataType = "Numeric" Then
                    Value = CDbl(ValueWorking).ToString(cColumnColl(ColNum).DataFormatString)
                ElseIf DataType = "Date" Then
                    Value = CType(ValueWorking, System.DateTime).ToString(cColumnColl(ColNum).DataFormatString)
                Else
                    Value = ValueWorking
                End If

            End If
        End If

        sb.Append("<td " & ColumnAttributes & ">" & Value & "</td>")
    End Sub

    Private Sub HandleHiddenColumn(ByVal ColNum As Integer, ByRef dt As Data.DataTable, ByVal RowNum As Integer, ByRef sb As System.Text.StringBuilder)
        Dim Value As String

        Value = "<input type='hidden' id='" & cColumnColl(ColNum).ItemName & "|" & dt.Rows(RowNum)(cKeyFieldName) & "' name='" & cColumnColl(ColNum).ItemName & "|" & dt.Rows(RowNum)(cKeyFieldName) & "' value=""" & dt.Rows(RowNum)(cColumnColl(ColNum).DataFldName) & """>"
        cHiddenColumnColl(cColumnColl(ColNum).ItemName).add(dt.Rows(RowNum)(cKeyFieldName) & "~" & dt.Rows(RowNum)(cColumnColl(ColNum).DataFldName))
        sb.Append(Value)
    End Sub


    Private Function HandleChildTableSelectColumn(ByVal ColNum As Integer, ByRef dt As Data.DataTable, ByVal RowNum As Integer, ByRef sb As System.Text.StringBuilder) As Boolean
        Dim Value As String
        Dim ColumnAttributes As String
        Dim i As Integer
        Dim RunningQualify As Boolean
        Dim FldName As String

        ' __ Handle column attributes and tooltip
        ColumnAttributes = "width= '20px' "
        ColumnAttributes &= cColumnColl(ColNum).Attributes

        If dt.Rows(RowNum)(cColumnColl(ColNum).EnableFldName) = 0 Then
            Value = "&nbsp;"
        Else
            If cColumnColl(ColNum).Title <> Nothing Then
                ColumnAttributes &= " title='" & cColumnColl(ColNum).Title & "' "
            End If


            ' ___ Show subtable for this parent row?
            ' copied from eventquality


            If (Not cChildTables Is Nothing) AndAlso (dt.Rows(RowNum)(cColumnColl(1).EnableFldName) = 1) Then
                RunningQualify = True
                If RunningQualify Then
                    For i = 1 To cChildTables.ChildTableSelectColumn.ParmColl.Count
                        FldName = cChildTables.ChildTableSelectColumn.ParmColl(i).FldName
                        If (Not dt.Rows(RowNum)(FldName) = cChildTables.ChildTableSelectColumn.ParmColl(i).Value) Then
                            RunningQualify = False
                            Exit For
                        End If
                    Next
                End If
            End If






            If RunningQualify Then
                'If dt.Rows(RowNum)(cColumnColl(ColNum).DataFldName) = cColumnColl(ColNum).ParmColl("DataFldName").Value Then
                Value = "<a href=""javascript:ShowHideSubTable('0', '')""><strong>&nbsp;&nbsp;-&nbsp;&nbsp;</strong></a>"
            Else
                Select Case cColumnColl(ColNum).ParmColl.Count
                    Case 1
                        Value = "<a href=""javascript:ShowHideSubTable('1', '" & dt.Rows(RowNum)(cColumnColl(ColNum).DataFldName) & "')""><strong>&nbsp;&nbsp;+&nbsp;&nbsp;</strong></a>"
                    Case 2
                        Value = "<a href=""javascript:ShowHideSubTable('1', '" & dt.Rows(RowNum)(cColumnColl(ColNum).DataFldName) & "', '" & dt.Rows(RowNum)(cColumnColl(ColNum).ParmColl("Parm2").FldName) & "')""><strong>&nbsp;&nbsp;+&nbsp;&nbsp;</strong></a>"
                    Case 3
                        Value = "<a href=""javascript:ShowHideSubTable('1', '" & dt.Rows(RowNum)(cColumnColl(ColNum).DataFldName) & "', '" & dt.Rows(RowNum)(cColumnColl(ColNum).ParmColl("Parm2").FldName) & "', '" & dt.Rows(RowNum)(cColumnColl(ColNum).ParmColl("Parm3").FldName) & "')""><strong>&nbsp;&nbsp;+&nbsp;&nbsp;</strong></a>"
                    Case 4
                        Value = "<a href=""javascript:ShowHideSubTable('1', '" & dt.Rows(RowNum)(cColumnColl(ColNum).DataFldName) & "', '" & dt.Rows(RowNum)(cColumnColl(ColNum).ParmColl("Parm2").FldName) & "', '" & dt.Rows(RowNum)(cColumnColl(ColNum).ParmColl("Parm3").FldName) & "', '" & dt.Rows(RowNum)(cColumnColl(ColNum).ParmColl("Parm4").FldName) & "')""><strong>&nbsp;&nbsp;+&nbsp;&nbsp;</strong></a>"
                End Select
            End If
        End If

        sb.Append("<td " & ColumnAttributes & ">" & Value & "</td>")

        Return RunningQualify
    End Function

    Private Sub oldHandleChildTableSelectColumn(ByVal ColNum As Integer, ByRef dt As Data.DataTable, ByVal RowNum As Integer, ByRef sb As System.Text.StringBuilder)
        Dim Value As String
        Dim ColumnAttributes As String

        ' __ Handle column attributes and tooltip
        ColumnAttributes = "width= '20px' "
        ColumnAttributes &= cColumnColl(ColNum).Attributes

        If dt.Rows(RowNum)(cColumnColl(ColNum).EnableFldName) = 0 Then
            Value = "&nbsp;"
        Else
            If cColumnColl(ColNum).Title <> Nothing Then
                ColumnAttributes &= " title='" & cColumnColl(ColNum).Title & "' "
            End If
            'If dt.Rows(RowNum)(cColumnColl(ColNum).DataFldName) = cChildTables.Value Then
            '    ' Value = "<a href=""javascript:ShowHideSubTable('" & cColumnColl(ColNum).ParmColl("DataFldName") & "', '')""><strong>-</strong></a>"
            '    Value = "<a href=""javascript:ShowHideSubTable('" & cColumnColl(ColNum).DataFldName & "', '')""><strong>-</strong></a>"
            'Else
            '    'Value = "<a href=""javascript:ShowHideSubTable('" & cColumnColl(ColNum).DataFldName & "', '" & dt.Rows(RowNum)(cColumnColl(ColNum).DataFldName) & "')""><strong>+</strong></a>"
            '    Value = "<a href=""javascript:ShowHideSubTable('" & cColumnColl(ColNum).ParmColl("DataFldName") & "', '" & dt.Rows(RowNum)(cColumnColl(ColNum).DataFldName) & "')""><strong>+</strong></a>"
            'End If
        End If
        sb.Append("<td " & ColumnAttributes & ">" & Value & "</td>")
    End Sub

    'Private Sub HandleNonTemplateColumns(ByVal ColNum As Integer, ByRef dt As Data.DataTable, ByVal RowNum As Integer, ByRef sb As System.Text.StringBuilder)
    '    Dim Value As String
    '    Dim ColumnAttributes As String
    '    Dim Box As Object
    '    Dim DisplayLinkAsLink As Boolean
    '    Dim DisplayLinkAsText As Boolean
    '    Dim DataFldName As String
    '    Dim CheckboxLabel As String

    '    ' __ Handle column attributes and tooltip
    '    ColumnAttributes = cColumnColl(ColNum).Attributes
    '    If cColumnColl(ColNum).TitleFldName <> Nothing Then
    '        ' ColumnAttributes &= " title='" & Common.StrInHandler(objDataReader(cColumnColl(i).TitleFldName)) & "' "
    '        ColumnAttributes &= " title='" & Common.StrInHandler(dt.Rows(RowNum)(cColumnColl(ColNum).TitleFldName)) & "' "
    '    End If

    '    If cColumnColl(ColNum).ColumnType = DG.ColumnType.Databound Then

    '        'Value = Common.StrInHandler(objDataReader(cColumnColl(i).DataFldName))
    '        Value = Common.StrInHandler(dt.Rows(RowNum)(cColumnColl(ColNum).DataFldName))

    '        ' ___ Handle boolean
    '        If dt.Columns(cColumnColl(ColNum).DataFldName).datatype.ToString.ToLower = "system.boolean" Then
    '            If IsDBNull(dt.Rows(RowNum)(cColumnColl(ColNum).DataFldName)) Then
    '                Value = "No"
    '            Else
    '                If dt.Rows(RowNum)(cColumnColl(ColNum).DataFldName) Then
    '                    Value = "Yes"
    '                Else
    '                    Value = "No"
    '                End If
    '            End If
    '        End If

    '        ' ___ Format value
    '        If Value.Length = 0 Then
    '            Value = "&nbsp;"
    '        Else
    '            If Not cColumnColl(ColNum).DataFormatString = Nothing Then
    '                Value = CDbl(Value).ToString(cColumnColl(ColNum).DataFormatString)
    '            End If
    '        End If

    '    ElseIf cColumnColl(ColNum).columntype = DG.ColumnType.CheckboxToggle Then
    '        If dt.Rows(RowNum)(cColumnColl(ColNum).TestFld) = "1" Then
    '            CheckboxLabel = cColumnColl(ColNum).TrueText
    '        Else
    '            CheckboxLabel = cColumnColl(ColNum).FalseText
    '        End If

    '        Value = "<input type='checkbox' name='" & cColumnColl(ColNum).ItemName & "|" & dt.Rows(RowNum)(cKeyFieldName) & "' value='on'>&nbsp;" & CheckboxLabel
    '        'Value &= "<input type='hidden' name='hd" & cColumnColl(i).ItemName & "|" & dt.Rows(RowNum)(cKeyFieldName) & " value='" & dt.Rows(RowNum)(cColumnColl(i).DataFldName) & "'>"

    '        ' ___ This is provided as an alternative way of iterating through the checkbox items.
    '        cCheckboxToggleColl(cColumnColl(ColNum).ItemName).append(dt.Rows(RowNum)(cKeyFieldName) & "~" & dt.Rows(RowNum)(cColumnColl(ColNum).DataFldName) & "|")

    '    ElseIf cColumnColl(ColNum).columntype = DG.ColumnType.Hidden Then
    '        Value = "<input type='hidden' id='" & cColumnColl(ColNum).ItemName & "|" & dt.Rows(RowNum)(cKeyFieldName) & "' name='" & cColumnColl(ColNum).ItemName & "|" & dt.Rows(RowNum)(cKeyFieldName) & "' value='" & dt.Rows(RowNum)(cColumnColl(ColNum).DataFldName) & "'>"
    '        cHiddenColumnColl(cColumnColl(ColNum).ItemName).add(dt.Rows(RowNum)(cKeyFieldName) & "~" & dt.Rows(RowNum)(cColumnColl(ColNum).DataFldName))

    '    ElseIf cColumnColl(ColNum).columntype = DG.ColumnType.Link Then

    '        ' ___ Link
    '        ' HRef = "javascript:fnToggle"
    '        ' DataFldName "ReadyForPrint|1"       Display as link
    '        ' DataFldName "ReadyForPrint|0"       Display as text
    '        ' DataFldName "ReadyForPrint"          Display as link

    '        'If InStr(objDataReader(cColumnColl(i).DataFldName), "|") = 0 Then
    '        If InStr(dt.Rows(RowNum)(cColumnColl(ColNum).DataFldName), "|") = 0 Then
    '            DisplayLinkAsText = True
    '            DisplayLinkAsLink = False
    '            DataFldName = cColumnColl(ColNum).DataFldName
    '        Else
    '            'Box = Split(objDataReader(cColumnColl(i).DataFldName), "|")
    '            Box = Split(dt.Rows(RowNum)(cColumnColl(ColNum).DataFldName), "|")
    '            If Box(1) = "0" Then
    '                DisplayLinkAsText = True
    '                DisplayLinkAsLink = False
    '                DataFldName = Box(0)
    '            Else
    '                DisplayLinkAsText = False
    '                DisplayLinkAsLink = True
    '                DataFldName = Box(0)
    '            End If
    '        End If

    '        If DisplayLinkAsLink Then
    '            Value = cColumnColl(ColNum).Href
    '            ' Value = "<a href=""""" & Value & "('" & objDataReader(cKeyFieldName) & "')"""">" & Common.StrInHandler(objDataReader(DataFldName)) & "</a>"
    '            Value = "<a href=""""" & Value & "('" & dt.Rows(RowNum)(cKeyFieldName) & "')"""">" & Common.StrInHandler(dt.Rows(RowNum)(DataFldName)) & "</a>"
    '        ElseIf DisplayLinkAsText Then
    '            'Value = Common.StrInHandler(objDataReader(DataFldName))
    '            Value = Common.StrInHandler(dt.Rows(RowNum)(DataFldName))
    '            If Value.Length = 0 Then
    '                Value = "&nbsp;"
    '            End If
    '        End If

    '    End If

    '    If cColumnColl(ColNum).columntype = DG.ColumnType.Hidden Then
    '        sb.Append(Value)
    '    Else
    '        sb.Append("<td " & ColumnAttributes & ">" & Value & "</td>")
    '        'If LeftMost Then
    '        '    sb.Append("<td " & cLeftPadding & ColumnAttributes & ">" & Value & "</td>")
    '        '    LeftMost = False
    '        'Else
    '        '    sb.Append("<td " & ColumnAttributes & ">" & Value & "</td>")
    '        'End If
    '    End If
    'End Sub

    Private Sub HandleTemplateColumn(ByVal ColNum As Integer, ByRef dt As Data.DataTable, ByVal RowNum As Integer, ByRef sb As System.Text.StringBuilder)
        Dim TemplateCol As DG.TemplateColumn
        Dim IsVisible As Boolean
        Dim j As Integer
        Dim PassPermissionFldTest As Boolean
        Dim PassRightTest As Boolean

        TemplateCol = cColumnColl(ColNum)

        ' __ Handle column attributes and tooltip
        sb.Append("<td ")
        If cColumnColl(ColNum).Attributes <> Nothing Then
            sb.Append(cColumnColl(ColNum).Attributes)
        End If
        If Not TemplateCol.Wrap Then
            sb.Append(" nowrap ")
        End If
        sb.Append(">")

        For j = 1 To TemplateCol.Count

            IsVisible = False
            PassPermissionFldTest = False
            PassRightTest = False

            ' ___ Permission field test
            If TemplateCol(j).PermissionFldName = Nothing Then
                PassPermissionFldTest = True
            Else
                If (Not IsDBNull(dt.Rows(RowNum)(TemplateCol(j).PermissionFldName))) AndAlso (dt.Rows(RowNum)(TemplateCol(j).PermissionFldName) = "1") Then
                    PassPermissionFldTest = True
                End If
            End If

            ' ___ Rights test
            If TemplateCol(j).Right = Nothing Then
                PassRightTest = True
            Else
                If Rights.HasThisRight(TemplateCol(j).Right) Then
                    PassRightTest = True
                End If
            End If

            If PassPermissionFldTest AndAlso PassRightTest Then
                IsVisible = True
            End If

            If IsVisible Then
                sb.Append("<a ")
                sb.Append("id=""" & TemplateCol(j).DataFldName & """ ")

                If TemplateCol(j).Parm2 = Nothing Then
                    sb.Append("onclick=""" & TemplateCol(j).OnclickMethod & "('" & dt.Rows(RowNum)(cKeyFieldName) & "')"">")
                ElseIf TemplateCol(j).Parm3 = Nothing Then
                    sb.Append("onclick=""" & TemplateCol(j).OnclickMethod & "('" & dt.Rows(RowNum)(cKeyFieldName) & "', '" & Common.StrInHandler(dt.Rows(RowNum)(TemplateCol(j).Parm2)) & "')"">")
                ElseIf TemplateCol(j).Parm4 = Nothing Then
                    sb.Append("onclick=""" & TemplateCol(j).OnclickMethod & "('" & dt.Rows(RowNum)(cKeyFieldName) & "', '" & Common.StrInHandler(dt.Rows(RowNum)(TemplateCol(j).Parm2)) & "', '" & Common.StrInHandler(dt.Rows(RowNum)(TemplateCol(j).Parm3)) & "')"">")
                Else
                    sb.Append("onclick=""" & TemplateCol(j).OnclickMethod & "('" & dt.Rows(RowNum)(cKeyFieldName) & "', '" & Common.StrInHandler(dt.Rows(RowNum)(TemplateCol(j).Parm2)) & "', '" & Common.StrInHandler(dt.Rows(RowNum)(TemplateCol(j).Parm3)) & "', '" & Common.StrInHandler(dt.Rows(RowNum)(TemplateCol(j).Parm4)) & "')"">")
                End If

                If TemplateCol(j).UseDefaultImage Then
                    sb.Append(cColumnColl(ColNum).DefaultImageColl(TemplateCol(j).DefaultImage))
                Else
                    sb.Append("<img src=""" & TemplateCol(j).ImagePath & """ ")
                    sb.Append(TemplateCol(j).ImageAttributes)
                End If

                sb.Append("title=""" & TemplateCol(j).Title & """> ")
                sb.Append("</a>")
            End If
        Next
        sb.Append("</td>")
    End Sub

    'Private Sub HandleTemplateColumn(ByVal ColNum As Integer, ByRef dt As Data.DataTable, ByVal RowNum As Integer, ByRef sb As System.Text.StringBuilder)
    '    Dim TemplateCol As DG.TemplateColumn
    '    Dim IsVisible As Boolean
    '    Dim j As Integer
    '    Dim PassPermissionFldTest As Boolean
    '    Dim PassRightTest As Boolean
    '    Dim ColumnAttributes As String = String.Empty

    '    '            cDefaultImageColl.Add("class='MainMenuTbl><a href='AboutUs.aspx'>+</a>", "PlusSignView")
    '    '		<td class="MainMenuTbl"><a href="AboutUs.aspx">+ and a dog</a></td>
    '    '		<td class="MainMenuTbl"><a href="javascript:fnTestie()">Testie</a></td>

    '    TemplateCol = cColumnColl(ColNum)

    '    'If TemplateCol.Wrap Then
    '    '    sb.Append("<td>")
    '    'Else
    '    '    sb.Append("<td nowrap>")
    '    'End If

    '    ' __ Handle column attributes and tooltip
    '    ColumnAttributes = "<td "
    '    If cColumnColl(ColNum).Attributes <> Nothing Then
    '        ColumnAttributes &= cColumnColl(ColNum).Attributes
    '    End If
    '    'If TemplateCol.Ti <> Nothing Then
    '    '    ' ColumnAttributes &= " title='" & Common.StrInHandler(objDataReader(cColumnColl(i).TitleFldName)) & "' "
    '    '    ColumnAttributes &= " title='" & Common.StrInHandler(dt.Rows(RowNum)(cColumnColl(ColNum).TitleFldName)) & "' "
    '    'End If
    '    If Not TemplateCol.Wrap Then
    '        ColumnAttributes &= " nowrap "
    '    End If
    '    ColumnAttributes &= ">"


    '    'If LeftMost Then
    '    '    Attributes = cLeftPadding
    '    '    LeftMost = False
    '    'End If
    '    'If Not TemplateCol.Wrap Then
    '    '    Attributes &= " nowrap "
    '    'End If
    '    'sb.Append("<td" & Attributes & ">")


    '    For j = 1 To TemplateCol.Count

    '        IsVisible = False
    '        PassPermissionFldTest = False
    '        PassRightTest = False

    '        ' ___ Permission field test
    '        If TemplateCol(j).PermissionFldName = Nothing Then
    '            PassPermissionFldTest = True
    '        Else
    '            If (Not IsDBNull(dt.Rows(RowNum)(TemplateCol(j).PermissionFldName))) AndAlso (dt.Rows(RowNum)(TemplateCol(j).PermissionFldName) = "1") Then
    '                PassPermissionFldTest = True
    '            End If
    '        End If

    '        ' ___ Rights test
    '        If TemplateCol(j).Right = Nothing Then
    '            PassRightTest = True
    '        Else
    '            If Rights.HasThisRight(TemplateCol(j).Right) Then
    '                PassRightTest = True
    '            End If
    '        End If

    '        If PassPermissionFldTest AndAlso PassRightTest Then
    '            IsVisible = True
    '        End If

    '        If IsVisible Then

    '            sb.Append("<a ")
    '            '  sb.Append("id=""" & TemplateCol(j).DataFldName & """ ")
    '            sb.Append("id='" & TemplateCol(j).DataFldName & "' ")

    '            If cColumnColl(ColNum).ColumnUsage = TemplateColumn.ColumnUsageEnum.Icon Then
    '                If TemplateCol(j).Parm2 = Nothing Then
    '                    sb.Append("onclick=""" & TemplateCol(j).OnclickMethod & "('" & dt.Rows(RowNum)(cKeyFieldName) & "')"">")
    '                Else
    '                    sb.Append("onclick=""" & TemplateCol(j).OnclickMethod & "('" & dt.Rows(RowNum)(cKeyFieldName) & "', '" & Common.StrInHandler(dt.Rows(RowNum)(TemplateCol(j).Parm2)) & "')"">")
    '                End If
    '                If TemplateCol(j).UseDefaultImage Then
    '                    sb.Append(cColumnColl(ColNum).DefaultImageColl(TemplateCol(j).DefaultImage))
    '                Else
    '                    sb.Append("<img src=""" & TemplateCol(j).ImagePath & """ ")
    '                    sb.Append(TemplateCol(j).ImageAttributes)
    '                End If

    '            ElseIf cColumnColl(ColNum).ColumnUsage = TemplateColumn.ColumnUsageEnum.CellAsLink Then
    '                If TemplateCol(j).Parm2 = Nothing Then
    '                    sb.Append("href=""javascript:" & TemplateCol(j).OnClickMethod & "('" & dt.Rows(RowNum)(cKeyFieldName) & "')"">")
    '                Else
    '                    sb.Append("href=""javascript:" & TemplateCol(j).OnClickMethod & "('" & dt.Rows(RowNum)(cKeyFieldName) & "', '" & Common.StrInHandler(dt.Rows(RowNum)(TemplateCol(j).Parm2)) & "')"">")
    '                End If
    '            End If

    '            sb.Append("title=""" & TemplateCol(j).Title & """> ")
    '            sb.Append("</a>")
    '        End If
    '    Next
    '    sb.Append("</td>")
    'End Sub
#End Region

#Region " Column classes "
    Public Class FreeFormColumn
        Inherits DataBoundColumnItems
        Private cCellText As String
        Private cTitle As String
        Public ReadOnly Property Title()
            Get
                Return cTitle
            End Get
        End Property

        Public ReadOnly Property CellText()
            Get
                Return cCellText
            End Get
        End Property
        Public Sub New(ByVal ColumnType As DG.ColumnType, ByVal ItemName As String, ByVal CellText As String, ByVal HeaderText As String, ByVal Title As String, ByVal Visible As Boolean, ByVal Attributes As String)
            MyBase.New(ColumnType, ItemName, Nothing, HeaderText, Nothing, Visible, Nothing, Nothing, Attributes)
            cCellText = CellText
            cTitle = Title
        End Sub
    End Class

    Public Class ChildTableSelectColumn
        Inherits DataBoundColumnItems
        Private cTitle As String
        Private cEnableFldName As String
        Private cParmColl As Collection
        Private cChildTables As DG.ChildTablesClass

        Public ReadOnly Property ParmColl()
            Get
                Return cParmColl
            End Get
        End Property
        Public ReadOnly Property EnableFldName()
            Get
                Return cEnableFldName
            End Get
        End Property
        Public ReadOnly Property Title()
            Get
                Return cTitle
            End Get
        End Property
        Public Sub New(ByRef ChildTables As DG.ChildTablesClass, ByVal ColumnType As DG.ColumnType, ByVal ItemName As String, ByVal DataFldName As String, ByVal HeaderText As String, ByVal SortExpression As String, ByVal Visible As Boolean, ByVal EnableFldName As String, ByVal DataFormatString As String, ByVal Title As String, ByVal Attributes As String, ByVal Parm2 As String, ByVal Parm3 As String, ByVal Parm4 As String)
            MyBase.New(ColumnType, ItemName, DataFldName, HeaderText, SortExpression, Visible, DataFormatString, Nothing, Attributes)
            cEnableFldName = EnableFldName
            cTitle = Title
            cChildTables = ChildTables
            cParmColl = New Collection
            cParmColl.Add(New Items("DataFldName", DataFldName), "DataFldName")
            If Not Parm2 = Nothing Then
                cParmColl.Add(New Items("Parm2", Parm2), "Parm2")
            End If
            If Not Parm3 = Nothing Then
                cParmColl.Add(New Items("Parm3", Parm3), "Parm3")
            End If
            If Not Parm4 = Nothing Then
                cParmColl.Add(New Items("Parm4", Parm4), "Parm4")
            End If
        End Sub

        Public Class Items
            Private cItemName As String
            Private cFldName As String
            Private cValue As String
            Public ReadOnly Property ItemName()
                Get
                    Return cItemName
                End Get
            End Property
            Public ReadOnly Property FldName()
                Get
                    Return cFldName
                End Get
            End Property
            Public Property Value()
                Get
                    Return cValue
                End Get
                Set(ByVal Value)
                    cValue = Value
                End Set
            End Property

            Public Sub New(ByVal ItemName As String, ByVal FldName As String)
                cItemName = ItemName
                cFldName = FldName
            End Sub
        End Class
    End Class

    Public Class TemplateColumn
        Private cColl As New Collection
        Private cHeaderText As String
        Private cWrap As Boolean
        Private cVisible As Boolean
        Private cDefaultImageColl As New Collection
        Private cItemName As String
        Private cAttributes As String

        Default Public ReadOnly Property Coll(ByVal Idx As Integer)
            Get
                Return cColl(Idx)
            End Get
        End Property
        Public ReadOnly Property Attributes() As String
            Get
                Return cAttributes
            End Get
        End Property
        Public ReadOnly Property ItemName()
            Get
                Return cItemName
            End Get
        End Property
        Public ReadOnly Property DefaultImageColl()
            Get
                Return cDefaultImageColl
            End Get
        End Property
        Public ReadOnly Property ColumnType()
            Get
                Return DG.ColumnType.Template
            End Get
        End Property
        Public ReadOnly Property HeaderText() As String
            Get
                Return cHeaderText
            End Get
        End Property
        Public ReadOnly Property Wrap()
            Get
                Return cWrap
            End Get
        End Property
        Public ReadOnly Property Visible()
            Get
                Return cVisible
            End Get
        End Property
        Public ReadOnly Property Count()
            Get
                Return cColl.Count
            End Get
        End Property

        Public Sub New(ByVal ItemName As String, ByVal HeaderText As String, ByVal Wrap As Boolean, ByVal Attributes As String, ByVal Visible As Boolean)
            cItemName = ItemName
            cHeaderText = HeaderText
            cWrap = Wrap
            cVisible = Visible
            cAttributes = Attributes
            ' cDefaultImageColl.Add("<img src='img/edit.ico'  border='0' width='14' height='14' vspace='0' hspace='0'", "StandardView")
            cDefaultImageColl.Add("<img src='img/view.gif'  border='0' width='14' height='14' vspace='0' hspace='0'", "StandardView")
            cDefaultImageColl.Add("<img src='img/delete.gif'  border='0' width='14' height='14' vspace='0' hspace='0'", "StandardDelete")
            cDefaultImageColl.Add("<img src='img/Clip.ico'  border='0' width='16' height='16' vspace='0' hspace='0'", "StandardClip")
            cDefaultImageColl.Add("<img src='img/BriefCse.ico'  border='0' width='16' height='16' vspace='0' hspace='0'", "StandardBriefcase")
            cDefaultImageColl.Add("<img src='img/Cardfil1.ico'  border='0' width='16' height='16' vspace='0' hspace='0'", "StandardCardfile")
            cDefaultImageColl.Add("<img src='img/23_6.png'  border='0' width='16' height='16' vspace='0' hspace='0'", "Standard23_6")
            cDefaultImageColl.Add("<img src='img/133.png'  border='0' width='16' height='16' vspace='0' hspace='0'", "Standard133")
            cDefaultImageColl.Add("<img src='img/Notes.gif'  border='0' width='16' height='16' vspace='0' hspace='0'", "StandardNotes")
            ' cDefaultImageColl.Add("<img src='genicon.gif'  border='0' width='16' height='16' vspace='0' hspace='0'", "StandardMisc")
        End Sub

        Public Sub AddTemplateItem(ByVal ItemName As String, ByVal OnClickMethod As String, ByVal ImagePath As String, ByVal ImageAttributes As String, ByVal Title As String, ByVal Right As String, ByVal PermissionFldName As String)
            cColl.Add(New TemplateColumnItems(ItemName, OnClickMethod, ImagePath, ImageAttributes, Title, Right, PermissionFldName, Nothing, Nothing, Nothing), ItemName)
        End Sub

        Public Sub AddTemplateItem(ByVal ItemName As String, ByVal OnClickMethod As String, ByVal ImagePath As String, ByVal ImageAttributes As String, ByVal Title As String, ByVal Right As String, ByVal PermissionFldName As String, ByVal Parm2 As String)
            cColl.Add(New TemplateColumnItems(ItemName, OnClickMethod, ImagePath, ImageAttributes, Title, Right, PermissionFldName, Parm2, Nothing, Nothing), ItemName)
        End Sub

        Public Sub AddTemplateItem(ByVal ItemName As String, ByVal OnClickMethod As String, ByVal ImagePath As String, ByVal ImageAttributes As String, ByVal Title As String, ByVal Right As String, ByVal PermissionFldName As String, ByVal Parm2 As String, ByVal Parm3 As String)
            cColl.Add(New TemplateColumnItems(ItemName, OnClickMethod, ImagePath, ImageAttributes, Title, Right, PermissionFldName, Parm2, Parm3, Nothing), ItemName)
        End Sub

        Public Sub AddTemplateItem(ByVal ItemName As String, ByVal OnClickMethod As String, ByVal ImagePath As String, ByVal ImageAttributes As String, ByVal Title As String, ByVal Right As String, ByVal PermissionFldName As String, ByVal Parm2 As String, ByVal Parm3 As String, ByVal Parm4 As String)
            cColl.Add(New TemplateColumnItems(ItemName, OnClickMethod, ImagePath, ImageAttributes, Title, Right, PermissionFldName, Parm2, Parm3, Parm4), ItemName)
        End Sub

        Public Sub AddDefaultTemplateItem(ByVal ItemName As String, ByVal OnClickMethod As String, ByVal DefaultImage As String, ByVal Title As String, ByVal Right As String, ByVal PermissionFldName As String)
            cColl.Add(New TemplateColumnItems(ItemName, OnClickMethod, DefaultImage, Title, Right, PermissionFldName, Nothing, Nothing, Nothing), ItemName)
        End Sub

        Public Sub AddDefaultTemplateItem(ByVal ItemName As String, ByVal OnClickMethod As String, ByVal DefaultImage As String, ByVal Title As String, ByVal Right As String, ByVal PermissionFldName As String, ByVal Parm2 As String)
            cColl.Add(New TemplateColumnItems(ItemName, OnClickMethod, DefaultImage, Title, Right, PermissionFldName, Parm2, Nothing, Nothing), ItemName)
        End Sub

        Public Sub AddDefaultTemplateItem(ByVal ItemName As String, ByVal OnClickMethod As String, ByVal DefaultImage As String, ByVal Title As String, ByVal Right As String, ByVal PermissionFldName As String, ByVal Parm2 As String, ByVal Parm3 As String)
            cColl.Add(New TemplateColumnItems(ItemName, OnClickMethod, DefaultImage, Title, Right, PermissionFldName, Parm2, Parm3, Nothing), ItemName)
        End Sub

        Public Sub AddDefaultTemplateItem(ByVal ItemName As String, ByVal OnClickMethod As String, ByVal DefaultImage As String, ByVal Title As String, ByVal Right As String, ByVal PermissionFldName As String, ByVal Parm2 As String, ByVal Parm3 As String, ByVal Parm4 As String)
            cColl.Add(New TemplateColumnItems(ItemName, OnClickMethod, DefaultImage, Title, Right, PermissionFldName, Parm2, Parm3, Parm4), ItemName)
        End Sub

        Public Class TemplateColumnItems
            Inherits DataBoundColumnItems
            Private cOnClickMethod As String
            Private cImagePath As String
            Private cImageAttributes As String
            Private cTitle As String
            Private cRight As String
            Private cPermissionFldName As String
            Private cParm2 As String
            Private cParm3 As String
            Private cParm4 As String
            Private cDefaultImage
            Private cUseDefaultImage As Boolean
            Private cIsImageType As Boolean
            Private cIsFreeForm As Boolean
            Private cCellText As String

            Public ReadOnly Property IsImageType()
                Get
                    Return cIsImageType
                End Get
            End Property
            Public ReadOnly Property IsFreeForm()
                Get
                    Return cIsFreeForm
                End Get
            End Property
            Public ReadOnly Property CellText()
                Get
                    Return cCellText
                End Get
            End Property
            Public ReadOnly Property DefaultImage()
                Get
                    Return cDefaultImage
                End Get
            End Property
            Public ReadOnly Property ImageAttributes()
                Get
                    Return cImageAttributes
                End Get
            End Property
            Public ReadOnly Property UseDefaultImage()
                Get
                    Return cUseDefaultImage
                End Get
            End Property
            Public ReadOnly Property OnClickMethod()
                Get
                    Return cOnClickMethod
                End Get
            End Property
            Public ReadOnly Property ImagePath()
                Get
                    Return cImagePath
                End Get
            End Property
            Public ReadOnly Property Title()
                Get
                    Return cTitle
                End Get
            End Property
            Public ReadOnly Property Right()
                Get
                    Return cRight
                End Get
            End Property
            Public ReadOnly Property PermissionFldName()
                Get
                    Return cPermissionFldName
                End Get
            End Property
            Public ReadOnly Property Parm2()
                Get
                    Return cParm2
                End Get
            End Property
            Public ReadOnly Property Parm3()
                Get
                    Return cParm3
                End Get
            End Property
            Public ReadOnly Property Parm4()
                Get
                    Return cParm4
                End Get
            End Property

            Public Sub New(ByVal ItemName As String, ByVal OnClickMethod As String, ByVal ImagePath As String, ByVal ImageAttributes As String, ByVal Title As String, ByVal Right As String, ByVal PermissionFldName As String, ByVal Parm2 As String, ByVal Parm3 As String, ByVal Parm4 As String)
                MyBase.New(DG.ColumnType.Template, ItemName, Nothing)
                cOnClickMethod = OnClickMethod
                cImagePath = ImagePath
                cImageAttributes = ImageAttributes
                cTitle = Title
                cRight = Right
                cPermissionFldName = PermissionFldName
                cParm2 = Parm2
                cParm3 = Parm3
                cParm4 = Parm4
                cUseDefaultImage = False
                cIsImageType = True
            End Sub

            Public Sub New(ByVal ItemName As String, ByVal OnClickMethod As String, ByVal DefaultImage As String, ByVal Title As String, ByVal Right As String, ByVal PermissionFldName As String, ByVal Parm2 As String, ByVal Parm3 As String, ByVal Parm4 As String)
                MyBase.New(DG.ColumnType.Template, ItemName, Nothing)
                cOnClickMethod = OnClickMethod
                cDefaultImage = DefaultImage
                cTitle = Title
                cRight = Right
                cPermissionFldName = PermissionFldName
                cParm2 = Parm2
                cParm3 = Parm3
                cParm4 = Parm4
                cUseDefaultImage = True
                cIsImageType = True
            End Sub

            Public Sub New(ByVal ItemName As String, ByVal OnClickMethod As String, ByVal CellText As String, ByVal Title As String, ByVal Right As String, ByVal PermissionFldName As String, ByVal Parm2 As String, ByVal Parm3 As String, ByVal Parm4 As String, ByVal IDoNothing1 As String, ByVal IDoNothing2 As String)
                MyBase.New(DG.ColumnType.Template, ItemName, Nothing)
                cOnClickMethod = OnClickMethod
                cDefaultImage = DefaultImage
                cTitle = Title
                cRight = Right
                cPermissionFldName = PermissionFldName
                cParm2 = Parm2
                cParm3 = Parm3
                cParm4 = Parm4
                cUseDefaultImage = True
                cIsFreeForm = True
                cCellText = CellText
            End Sub
        End Class

        Protected Overrides Sub Finalize()
            MyBase.Finalize()
        End Sub
    End Class

    Public Class HiddenItems
        Inherits DataBoundColumnItems

        Public Sub New(ByVal ColumnType As DG.ColumnType, ByVal ItemName As String, ByVal DataFldName As String)
            MyBase.New(ColumnType, ItemName, DataFldName)
        End Sub
    End Class

    Public Class CheckboxToggleColumnItems
        Inherits DataBoundColumnItems
        Private cTrueText As String
        Private cFalseText As String
        Private cTestFld As String

        Public ReadOnly Property TestFld()
            Get
                Return cTestFld
            End Get
        End Property

        Public ReadOnly Property TrueText()
            Get
                Return cTrueText
            End Get
        End Property
        Public ReadOnly Property FalseText()
            Get
                Return cFalseText
            End Get
        End Property
        Public Sub New(ByVal ColumnType As DG.ColumnType, ByVal ItemName As String, ByVal DataFldName As String, ByVal HeaderText As String, ByVal SortExpression As String, ByVal Visible As Boolean, ByVal TitleFldName As String, ByVal Attributes As String, ByVal TestFld As String, ByVal TrueText As String, ByVal FalseText As String)
            MyBase.New(ColumnType, ItemName, DataFldName, HeaderText, SortExpression, Visible, Nothing, TitleFldName, Attributes)
            cTrueText = TrueText
            cFalseText = FalseText
            cTestFld = TestFld
        End Sub
    End Class

    Public Class LinkColumnItems
        Inherits DataBoundColumnItems
        Private cHRef As String
        Private cAddlParm As String
        Public ReadOnly Property AddlParm()
            Get
                Return cAddlParm
            End Get
        End Property
        Public ReadOnly Property HRef()
            Get
                Return cHRef
            End Get
        End Property
        Public Sub New(ByVal ColumnType As DG.ColumnType, ByVal HRef As String, ByVal ItemName As String, ByVal DataFldName As String, ByVal HeaderText As String, ByVal SortExpression As String, ByVal Visible As Boolean, ByVal DataFormatString As String, ByVal TitleFldName As String, ByVal Attributes As String, ByVal AddlParm As String)
            MyBase.New(ColumnType, ItemName, DataFldName, HeaderText, SortExpression, Visible, DataFormatString, TitleFldName, Attributes)
            cHRef = HRef
            cAddlParm = AddlParm
        End Sub
    End Class

    Public Class DateColumn
        Inherits DataBoundColumnItems
        Private cTitle As String

        Public ReadOnly Property Title()
            Get
                Return cTitle
            End Get
        End Property

        Public Sub New(ByVal ColumnType As DG.ColumnType, ByVal ItemName As String, ByVal DataFldName As String, ByVal HeaderText As String, ByVal SortExpression As String, ByVal Visible As Boolean, ByVal DataFormatString As String, ByVal Title As String, ByVal Attributes As String)
            MyBase.New(ColumnType, ItemName, DataFldName, HeaderText, SortExpression, Visible, DataFormatString, Nothing, Attributes)
            cTitle = Title
        End Sub
    End Class

    Public Class BooleanColumn
        Inherits DataBoundColumnItems
        Private cTrueValue As String
        Private cTrueText As String
        Private cFalseText As String
        Private cTitle As String
        Public ReadOnly Property TrueText()
            Get
                Return cTrueText
            End Get
        End Property
        Public ReadOnly Property FalseText()
            Get
                Return cFalseText
            End Get
        End Property
        Public ReadOnly Property Truevalue()
            Get
                Return cTrueValue
            End Get
        End Property
        Public ReadOnly Property Title()
            Get
                Return cTitle
            End Get
        End Property
        Public Sub New(ByVal ColumnType As DG.ColumnType, ByVal ItemName As String, ByVal DataFldName As String, ByVal HeaderText As String, ByVal SortExpression As String, ByVal Visible As Boolean, ByVal TrueValue As String, ByVal TrueText As String, ByVal FalseText As String, ByVal Title As String, ByVal Attributes As String)
            MyBase.New(ColumnType, ItemName, DataFldName, HeaderText, SortExpression, Visible, Nothing, Nothing, Attributes)
            cTrueValue = TrueValue
            cTrueText = TrueText
            cFalseText = FalseText
            cTitle = Title
        End Sub
    End Class

    Public Class DataBoundColumnItems
        Private cColumnType As DG.ColumnType
        Private cItemName As String
        Private cDataFldName As String
        Private cHeaderText As String
        Private cSortExpression As String
        Private cVisible As Boolean
        Private cDataFormatString As String
        Private cTitleFldName As String
        Private cAttributes As String

        Public ReadOnly Property ColumnType()
            Get
                Return cColumnType
            End Get
        End Property
        Public ReadOnly Property TitleFldName()
            Get
                Return cTitleFldName
            End Get
        End Property
        Public ReadOnly Property ItemName()
            Get
                Return cItemName
            End Get
        End Property
        Public ReadOnly Property DataFldName()
            Get
                Return cDataFldName
            End Get
        End Property
        Public ReadOnly Property HeaderText()
            Get
                Return cHeaderText
            End Get
        End Property
        Public ReadOnly Property SortExpression()
            Get
                Return cSortExpression
            End Get
        End Property
        Public ReadOnly Property Visible()
            Get
                Return cVisible
            End Get
        End Property
        Public ReadOnly Property DataFormatString()
            Get
                Return cDataFormatString
            End Get
        End Property
        Public ReadOnly Property Attributes()
            Get
                Return cAttributes
            End Get
        End Property

        Public Sub New(ByVal ColumnType As DG.ColumnType, ByVal ItemName As String, ByVal DataFldName As String)
            cColumnType = ColumnType
            cItemName = ItemName
            cDataFldName = DataFldName
        End Sub

        Public Sub New(ByVal ColumnType As DG.ColumnType, ByVal ItemName As String, ByVal DataFldName As String, ByVal HeaderText As String, ByVal SortExpression As String, ByVal Visible As Boolean, ByVal DataFormatString As String, ByVal TitleFldName As String, ByVal Attributes As String)
            cColumnType = ColumnType
            cItemName = ItemName
            cDataFldName = DataFldName
            cHeaderText = HeaderText
            cSortExpression = SortExpression
            cVisible = Visible
            cDataFormatString = DataFormatString
            cTitleFldName = TitleFldName
            If Attributes = Nothing Then
                cAttributes = String.Empty
            Else
                cAttributes = Attributes
            End If
        End Sub
    End Class
#End Region

#Region " SortItem "
    Public Class SortItem
        Private cSortExpression As String
        Private cPosition As String
        Private cSortDirection As String
        Private cLastFieldSorted As String
        Public ReadOnly Property SortExpression()
            Get
                Return cSortExpression
            End Get
        End Property
        Public ReadOnly Property Position()
            Get
                Return cPosition
            End Get
        End Property
        Public Property SortDirection()
            Get
                Return cSortDirection
            End Get
            Set(ByVal Value)
                cSortDirection = Value
            End Set
        End Property
        Public Property LastFieldSorted()
            Get
                Return cLastFieldSorted
            End Get
            Set(ByVal Value)
                cLastFieldSorted = Value
            End Set
        End Property
        Public Sub New(ByVal SortExpression As String, ByVal Position As String)
            cSortExpression = SortExpression
            cPosition = Position
            cLastFieldSorted = "F"
            cSortDirection = "N"
        End Sub
    End Class
#End Region

#Region " Filter "
    Public Class Filter
        Private cColl As New Collection

        Default Public ReadOnly Property Item(ByVal ItemName As String) As Filter.FilterItem
            Get
                Return cColl(ItemName)
            End Get
        End Property

        Public ReadOnly Property Coll()
            Get
                Return cColl
            End Get
        End Property

        Public Sub AddTextbox(ByVal ItemName As String, ByVal DataFldName As String, ByVal MaxLength As Integer)
            cColl.Add(New TextboxCtl(ItemName, DataFldName, MaxLength, Nothing, Nothing), ItemName)
        End Sub

        Public Sub AddTextbox(ByVal ItemName As String, ByVal DataFldName As String, ByVal MaxLength As Integer, ByVal FilterField As String, ByVal DefaultValue As String)
            cColl.Add(New TextboxCtl(ItemName, DataFldName, MaxLength, FilterField, DefaultValue), ItemName)
        End Sub

        Public Sub AddDropdown(ByVal ItemName As String, ByVal DataFldName As String)
            cColl.Add(New DropdownCtl(ItemName, True, False, DataFldName), ItemName)
        End Sub

        Public Sub AddExtendedDropdown(ByVal ItemName As String, ByVal DataFldName As String)
            cColl.Add(New DropdownCtl(ItemName, False, True, DataFldName), ItemName)
        End Sub

        Public Class DropdownCtl
            Inherits FilterItem
            Private cColl As New Collection
            Private cSelectedValue As String = String.Empty
            Private cIsStandard As Boolean
            Private cIsExtended As Boolean

            Public ReadOnly Property IsStandard()
                Get
                    Return cIsStandard
                End Get
            End Property

            Public ReadOnly Property IsExtended()
                Get
                    Return cIsExtended
                End Get
            End Property

            Public ReadOnly Property Coll()
                Get
                    Return cColl
                End Get
            End Property

            Public Overrides Sub SetValue(ByVal Value As String)
                cSelectedValue = Value
            End Sub

            Public Overrides Function GetValue() As String
                Return cSelectedValue
                'Dim i As Integer
                'For i = 1 To cColl.Count
                '    If cColl(i).Selected = True Then
                '        Return cColl(i).Value
                '    End If
                'Next
                'Return String.Empty
            End Function

            Public Sub New(ByVal ItemName As String, ByVal IsStandard As Boolean, ByVal IsExtended As Boolean, ByVal DataFldName As String)
                MyBase.new(ItemName, DataFldName, False, True)
                cIsStandard = IsStandard
                cIsExtended = IsExtended
            End Sub

            Public Overrides Sub AddDropdownItem(ByVal Value As String, ByVal Text As String, Optional ByVal Selected As Boolean = False)
                If Selected Then
                    cSelectedValue = Value
                End If
                cColl.Add(New DropdownItem(Value, Text), Value)
            End Sub

            Public Overrides Sub AddExtendedDropdownItem(ByVal Value As String, ByVal Text As String, ByVal Sql As String, Optional ByVal Selected As Boolean = False)
                If Selected Then
                    cSelectedValue = Value
                End If
                cColl.Add(New DropdownItem(Value, Text, Sql), Value)
            End Sub

            Public Class DropdownItem
                Private cValue As String
                Private cText As String
                Private cSql As String

                Public ReadOnly Property Sql()
                    Get
                        Return cSql
                    End Get
                End Property
                Public ReadOnly Property Value()
                    Get
                        Return cValue
                    End Get
                End Property
                Public ReadOnly Property Text()
                    Get
                        Return cText
                    End Get
                End Property

                Public Sub New(ByVal Value As String, ByVal Text As String)
                    cValue = Value
                    cText = Text
                End Sub

                Public Sub New(ByVal Value As String, ByVal Text As String, ByVal Sql As String)
                    cValue = Value
                    cText = Text
                    cSql = Sql
                End Sub

            End Class

        End Class

        Public Class TextboxCtl
            Inherits FilterItem
            Private cMaxLength As Integer
            Private cInternalFilterField As String
            Private cText As String

            Public Overrides Function GetValue() As String
                Return cText
            End Function
            Public Overrides Sub SetValue(ByVal Value As String)
                cText = Value
            End Sub

            Public ReadOnly Property MaxLength()
                Get
                    Return cMaxLength
                End Get
            End Property

            Public ReadOnly Property FilterField()
                Get
                    Return cInternalFilterField
                End Get
            End Property

            Public Sub New(ByVal ItemName As String, ByVal DataFldName As String, ByVal MaxLength As Integer)
                MyBase.New(ItemName, DataFldName, True, False)
                cMaxLength = MaxLength
            End Sub

            Public Sub New(ByVal ItemName As String, ByVal DataFldName As String, ByVal MaxLength As Integer, ByVal FilterField As String, ByVal Text As String)
                MyBase.New(ItemName, DataFldName, True, False)
                cMaxLength = MaxLength
                cInternalFilterField = FilterField
                cText = Text
            End Sub
        End Class

        Public Class FilterItem
            Private cItemName As String
            Private cDataFldName As String
            Private cIsTextBox As Boolean
            Private cIsDropdown As Boolean
            Private cOverrideValue As String

            Public ReadOnly Property CtlName()
                Get
                    If cIsTextBox Then
                        Return "txt" & cDataFldName
                    ElseIf cIsDropdown Then
                        Return "dd" & cDataFldName
                    End If
                End Get
            End Property
            Public ReadOnly Property ItemName()
                Get
                    Return cItemName
                End Get
            End Property
            Public ReadOnly Property DataFldName()
                Get
                    Return cDataFldName
                End Get
            End Property
            Public ReadOnly Property IsTextBox()
                Get
                    Return cIsTextBox
                End Get
            End Property
            Public ReadOnly Property IsDropdown()
                Get
                    Return cIsDropdown
                End Get
            End Property

            Public Sub SetOverrideValue(ByVal Value As String)
                cOverrideValue = Value
            End Sub

            Public Function GetOverrideValue() As String
                Return cOverrideValue
            End Function

            Public Sub New(ByVal ItemName As String, ByVal DataFldName As String, ByVal IsTextBox As Boolean, ByVal IsDropdown As Boolean)
                cItemName = ItemName
                cDataFldName = DataFldName
                cIsTextBox = IsTextBox
                cIsDropdown = IsDropdown
            End Sub

            Public Overridable Sub AddDropdownItem(ByVal Value As String, ByVal Text As String, Optional ByVal Selected As Boolean = False)
                'cColl.Add(New DropdownItem(Value, Text, Selected), Value)
            End Sub

            Public Overridable Sub AddExtendedDropdownItem(ByVal Value As String, ByVal Text As String, ByVal Sql As String, Optional ByVal Selected As Boolean = False)
            End Sub

            Public Overridable Function GetValue() As String
            End Function

            Public Overridable Sub SetValue(ByVal Value As String)
            End Sub

        End Class

    End Class
#End Region


#Region " ExternalFilter "
    Public Class ExternalFilter
        Private cColl As New Collection

        Default Public ReadOnly Property Item(ByVal ItemName As String) As ExternalFilter.FilterItem
            Get
                Return cColl(ItemName)
            End Get
        End Property

        Public ReadOnly Property Coll()
            Get
                Return cColl
            End Get
        End Property

        Public Sub AddTextbox(ByVal ItemName As String, ByVal DataFldName As String, ByVal MaxLength As Integer, ByVal Position As Integer)
            cColl.Add(New TextboxCtl(ItemName, DataFldName, MaxLength, Nothing, Nothing, Position), ItemName)
        End Sub

        Public Sub AddTextbox(ByVal ItemName As String, ByVal DataFldName As String, ByVal MaxLength As Integer, ByVal FilterField As String, ByVal DefaultValue As String, ByVal Position As Integer)
            cColl.Add(New TextboxCtl(ItemName, DataFldName, MaxLength, FilterField, DefaultValue, Position), ItemName)
        End Sub

        Public Sub AddDropdown(ByVal ItemName As String, ByVal DataFldName As String, ByVal Position As Integer, Optional ByVal EventString As String = Nothing)
            cColl.Add(New DropdownCtl(ItemName, True, False, DataFldName, Position, EventString), ItemName)
        End Sub

        Public Sub AddExtendedDropdown(ByVal ItemName As String, ByVal DataFldName As String, ByVal Position As Integer, Optional ByVal EventString As String = Nothing)
            cColl.Add(New DropdownCtl(ItemName, False, True, DataFldName, Position, EventString), ItemName)
        End Sub

        Public Sub AddLinkItem(ByVal ItemName As String, ByVal DataFldName As String, ByVal Position As Integer, ByVal EventString As String)
            cColl.Add(New LinkFilterCtl(ItemName, DataFldName, Position, EventString), ItemName)
        End Sub

        Public Class DropdownCtl
            Inherits FilterItem
            Private cColl As New Collection
            Private cSelectedValue As String = String.Empty
            Private cIsStandard As Boolean
            Private cIsExtended As Boolean
            Private cEventString As String

            Public ReadOnly Property EventString()
                Get
                    Return cEventString
                End Get
            End Property

            Public ReadOnly Property IsStandard()
                Get
                    Return cIsStandard
                End Get
            End Property

            Public ReadOnly Property IsExtended()
                Get
                    Return cIsExtended
                End Get
            End Property

            Public ReadOnly Property Coll()
                Get
                    Return cColl
                End Get
            End Property

            Public Overrides Sub SetValue(ByVal Value As String)
                cSelectedValue = Value
            End Sub

            Public Overrides Function GetValue() As String
                Return cSelectedValue
                'Dim i As Integer
                'For i = 1 To cColl.Count
                '    If cColl(i).Selected = True Then
                '        Return cColl(i).Value
                '    End If
                'Next
                'Return String.Empty
            End Function

            Public Sub New(ByVal ItemName As String, ByVal IsStandard As Boolean, ByVal IsExtended As Boolean, ByVal DataFldName As String, ByVal Position As Integer, ByVal EventString As String)
                MyBase.new(ItemName, DataFldName, False, True, False, Position)
                cIsStandard = IsStandard
                cIsExtended = IsExtended
                cEventString = EventString
            End Sub

            Public Overrides Sub AddDropdownItem(ByVal Value As String, ByVal Text As String, Optional ByVal Selected As Boolean = False)
                If Selected Then
                    cSelectedValue = Value
                End If
                cColl.Add(New DropdownItem(Value, Text), Value)
            End Sub

            Public Overrides Sub AddExtendedDropdownItem(ByVal Value As String, ByVal Text As String, ByVal Sql As String, Optional ByVal Selected As Boolean = False)
                If Selected Then
                    cSelectedValue = Value
                End If
                cColl.Add(New DropdownItem(Value, Text, Sql), Value)
            End Sub



            Public Class DropdownItem
                Private cValue As String
                Private cText As String
                Private cSql As String

                Public ReadOnly Property Sql()
                    Get
                        Return cSql
                    End Get
                End Property
                Public ReadOnly Property Value()
                    Get
                        Return cValue
                    End Get
                End Property
                Public ReadOnly Property Text()
                    Get
                        Return cText
                    End Get
                End Property

                Public Sub New(ByVal Value As String, ByVal Text As String)
                    cValue = Value
                    cText = Text
                End Sub

                Public Sub New(ByVal Value As String, ByVal Text As String, ByVal Sql As String)
                    cValue = Value
                    cText = Text
                    cSql = Sql
                End Sub

            End Class

        End Class

        Public Class LinkFilterCtl
            Inherits FilterItem

            Private cText As String
            Private cFilterExpression As String
            Private cEventString As String


            Public ReadOnly Property EventString()
                Get
                    Return cEventString
                End Get
            End Property
            Public Property Text()
                Get
                    Return cText
                End Get
                Set(ByVal Value)
                    cText = Value
                End Set
            End Property
            Public Property FilterExpression()
                Get
                    Return cFilterExpression
                End Get
                Set(ByVal Value)
                    cFilterExpression = Value
                End Set
            End Property

            Public Sub New(ByVal ItemName As String, ByVal DataFldName As String, ByVal Position As Integer, ByVal EventString As String)
                MyBase.New(ItemName, DataFldName, False, False, True, Position)

                cEventString = EventString
            End Sub

            Public Sub SetLinkValues(ByVal Text As String, ByVal FilterExpression As String)
                cText = Text
                cFilterExpression = FilterExpression
            End Sub

        End Class

        Public Class TextboxCtl
            Inherits FilterItem
            Private cMaxLength As Integer
            Private cInternalFilterField As String
            Private cText As String

            Public Overrides Function GetValue() As String
                Return cText
            End Function
            Public Overrides Sub SetValue(ByVal Value As String)
                cText = Value
            End Sub

            Public ReadOnly Property MaxLength()
                Get
                    Return cMaxLength
                End Get
            End Property

            Public ReadOnly Property FilterField()
                Get
                    Return cInternalFilterField
                End Get
            End Property

            Public Sub New(ByVal ItemName As String, ByVal DataFldName As String, ByVal MaxLength As Integer, ByVal Position As Integer)
                MyBase.New(ItemName, DataFldName, True, False, False, Position)
                cMaxLength = MaxLength
            End Sub

            Public Sub New(ByVal ItemName As String, ByVal DataFldName As String, ByVal MaxLength As Integer, ByVal FilterField As String, ByVal Text As String, ByVal Position As Integer)
                MyBase.New(ItemName, DataFldName, True, False, False, Position)
                cMaxLength = MaxLength
                cInternalFilterField = FilterField
                cText = Text
            End Sub
        End Class

        Public Class FilterItem
            Private cItemName As String
            Private cDataFldName As String
            Private cIsTextBox As Boolean
            Private cIsDropdown As Boolean
            Private cIsLink As Boolean
            Private cPosition As Integer
            Private cOverrideValue As String
            Private cControlType As ControlTypeEnum

            Public Enum ControlTypeEnum
                Textbox = 1
                StandardDropdown = 2
                ExtendedDropdown = 3
                Link = 4
            End Enum

            Public Sub SetOverrideValue(ByVal Value As String)
                cOverrideValue = Value
            End Sub

            Public Function GetOverrideValue() As String
                Return cOverrideValue
            End Function

            Public ReadOnly Property ControlType()
                Get
                    Return cControlType
                End Get
            End Property

            Public ReadOnly Property IsLink()
                Get
                    Return cIsLink
                End Get
            End Property
            Public ReadOnly Property Position()
                Get
                    Return cPosition
                End Get
            End Property

            Public ReadOnly Property CtlName()
                Get
                    If cIsTextBox Then
                        Return "txt" & cDataFldName
                    ElseIf cIsDropdown Then
                        Return "dd" & cDataFldName
                    ElseIf cIsLink Then
                        Return "lnk" & cDataFldName
                    End If
                End Get
            End Property
            Public ReadOnly Property ItemName()
                Get
                    Return cItemName
                End Get
            End Property
            Public ReadOnly Property DataFldName()
                Get
                    Return cDataFldName
                End Get
            End Property
            Public ReadOnly Property IsTextBox()
                Get
                    Return cIsTextBox
                End Get
            End Property
            Public ReadOnly Property IsDropdown()
                Get
                    Return cIsDropdown
                End Get
            End Property

            Public Sub New(ByVal ItemName As String, ByVal DataFldName As String, ByVal IsTextBox As Boolean, ByVal IsDropdown As Boolean, ByVal IsLink As Boolean, ByVal Position As Integer)
                cItemName = ItemName
                cDataFldName = DataFldName
                cIsTextBox = IsTextBox
                cIsDropdown = IsDropdown
                cIsLink = IsLink
                cPosition = Position
                If IsTextBox Then
                    cControlType = ControlTypeEnum.Textbox
                ElseIf IsDropdown Then
                    cControlType = ControlTypeEnum.StandardDropdown
                ElseIf IsLink Then
                    cControlType = ControlTypeEnum.Link
                End If
            End Sub

            Public Overridable Sub AddDropdownItem(ByVal Value As String, ByVal Text As String, Optional ByVal Selected As Boolean = False)
                'cColl.Add(New DropdownItem(Value, Text, Selected), Value)
            End Sub

            Public Overridable Sub AddLink(ByVal Value As String, ByVal Text As String, ByVal Sql As String, Optional ByVal Selected As Boolean = False)
            End Sub

            Public Overridable Sub AddExtendedDropdownItem(ByVal Value As String, ByVal Text As String, ByVal Sql As String, Optional ByVal Selected As Boolean = False)
            End Sub

            Public Overridable Function GetValue() As String
            End Function

            Public Overridable Sub SetValue(ByVal Value As String)
            End Sub

        End Class

    End Class
#End Region

#Region " ChildTables "
    Public Class ChildTablesClass
        Private cItemName As String
        Private cDataFldName As String
        ' Private cValue As String
        Private cPermissionFldName As String
        Private cChildTableSelectColumn As ChildTableSelectColumn

        Public Property ChildTableSelectColumn()
            Get
                Return cChildTableSelectColumn
            End Get
            Set(ByVal Value)
                cChildTableSelectColumn = Value
            End Set
        End Property

        Public ReadOnly Property ItemName()
            Get
                Return cItemName
            End Get
        End Property
        Public ReadOnly Property DataFldName()
            Get
                Return cDataFldName
            End Get
        End Property
        'Public Property Value()
        '    Get
        '        Return cValue
        '    End Get
        '    Set(ByVal Value)
        '        cValue = Value
        '    End Set
        'End Property
        Public ReadOnly Property PermissionFldName()
            Get
                Return cPermissionFldName
            End Get
        End Property

        Public Sub New(ByVal ItemName As String, ByVal DataFldName As String, ByVal PermissionFldName As String)
            cItemName = ItemName
            cDataFldName = DataFldName
            cPermissionFldName = PermissionFldName
        End Sub
    End Class
#End Region

#Region " Menu "
    Public Class Menu
        Private cColl As Collection
        Private cCellWidthPercent As Integer

        Public Enum ObjectTypeEnum
            IsLink = 1
            IsButton = 2
        End Enum

        Public ReadOnly Property CellWidthPercent()
            Get
                Return cCellWidthPercent
            End Get
        End Property

        Public ReadOnly Property Coll()
            Get
                Return cColl
            End Get
        End Property

        Public Sub New(ByVal CellWidthPercent As Integer)
            cColl = New Collection
            cCellWidthPercent = CellWidthPercent
        End Sub

        Public Sub AddItem(ByVal ObjectType As ObjectTypeEnum, ByVal OnClickMethod As String, ByVal Text As String, ByVal Right As String)
            cColl.Add(New MenuItem(ObjectType, OnClickMethod, Text, Right))
        End Sub

        Public Class MenuItem
            Private cIsLink As Boolean
            Private cIsButton As Boolean
            Private cOnClickMethod As String
            Private cText As String
            Private cIsVisible As Boolean = True
            Private cRight As String

            Public ReadOnly Property Right()
                Get
                    Return cRight
                End Get
            End Property

            Public Property IsVisible()
                Get
                    Return cIsVisible
                End Get
                Set(ByVal Value)
                    cIsVisible = Value
                End Set
            End Property

            Public ReadOnly Property Text()
                Get
                    Return cText
                End Get
            End Property
            Public ReadOnly Property IsLink()
                Get
                    Return cIsLink
                End Get
            End Property
            Public ReadOnly Property IsButton()
                Get
                    Return cIsButton
                End Get
            End Property
            Public ReadOnly Property OnClickMethod()
                Get
                    Return cOnClickMethod
                End Get
            End Property

            Public Sub New(ByVal ObjectType As ObjectTypeEnum, ByVal OnClickMethod As String, ByVal Text As String, ByVal Right As String)
                If ObjectType = ObjectTypeEnum.IsButton Then
                    cIsButton = True
                Else
                    cIsLink = True
                End If
                cOnClickMethod = OnClickMethod
                cText = Text
                cRight = Right
            End Sub
        End Class
    End Class
#End Region

End Class