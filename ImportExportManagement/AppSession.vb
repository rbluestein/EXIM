Imports System.Data.SqlClient

Public Class AppSession
    Private cActiveSqlServerConnString As String
    Private cRightsStr As String

    Public Function ExcelConnectionString() As String
        If System.Environment.MachineName.ToUpper = "LT-5ZFYRC1" Then
            Return "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Inetpub\wwwroot\ImportExportManagement\MasterList.xls;Extended Properties=Excel 8.0;"
        Else
            Return "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Internet\WEB\Test.benefitvision.com\ImportExportManagement\MasterList.xls;Extended Properties=Excel 8.0;"
        End If
    End Function

    Public Function ConnectionString(Optional ByVal db As String = "EXIM") As String
        Dim ActiveConnString As String
        If db = "EXIM" Then
            Return Replace(cActiveSqlServerConnString, "|", "EXIM")
        Else
            Return Replace(cActiveSqlServerConnString, "|", db)
        End If
    End Function

    Public Sub New()
        Dim configurationAppSettings As System.Configuration.AppSettingsReader = New System.Configuration.AppSettingsReader
        If CType(configurationAppSettings.GetValue("ActiveSqlServer", GetType(System.String)), String) = "BVITest" Then
            cActiveSqlServerConnString = CType(configurationAppSettings.GetValue("BVITestSqlServerConnString", GetType(System.String)), String)
        ElseIf CType(configurationAppSettings.GetValue("ActiveSqlServer", GetType(System.String)), String) = "BVIProd" Then
            cActiveSqlServerConnString = CType(configurationAppSettings.GetValue("BVIProdSqlServerConnString", GetType(System.String)), String)
        ElseIf CType(configurationAppSettings.GetValue("ActiveSqlServer", GetType(System.String)), String) = "Dell" Then
            cActiveSqlServerConnString = CType(configurationAppSettings.GetValue("DellSqlServerConnString", GetType(System.String)), String)
        End If
    End Sub

    Public Function Init(ByVal Page As Page) As String
        Dim LoggedInUserID As String

        If System.Environment.MachineName.ToUpper = "LT-5ZFYRC1" Then
            LoggedInUserID = "ricksupv"
            LoggedInUserID = "rgreen"
            'LoggedInUserID = "enroller"
            'LoggedInUserID = "aapostrophe"  ' admin
            LoggedInUserID = "rbluestein"
            ' LoggedInUserID = "myacht"
        Else
            LoggedInUserID = HttpContext.Current.User.Identity.Name.ToString
            LoggedInUserID = LoggedInUserID.Substring(InStr(LoggedInUserID, "\", CompareMethod.Binary))
        End If

        Dim SessionCookie As HttpCookie
        SessionCookie = New HttpCookie("BVIIEM", LoggedInUserID)
        Page.Response.Cookies.Add(SessionCookie)
        Return LoggedInUserID
    End Function


    Public Function RestoreSession(ByRef Page As System.Web.UI.Page) As String
        Dim i As Integer
        Dim SessionObj As System.Web.SessionState.HttpSessionState = System.Web.HttpContext.Current.Session
        Dim Common As Common
        Common = SessionObj("Common")
        Dim SessionId As String = String.Empty
        Dim LoggedInUserID As String

        ' ___ Attempt to retrieve the session cookie
        Try
            If IsNothing(Page.Request.Cookies.Item("BVIIEM")) Then
                Throw New Exception("No Cookie Present.")
            Else
                LoggedInUserID = Page.Request.Cookies.Item("BVIIEM").Value
            End If

            Return LoggedInUserID

        Catch ex As Exception
            Throw New Exception("Error", ex)
        End Try
    End Function
End Class

Public Class PageSession
    Private cSortReference As String
    Private cFilterOnOffState As String
    Private cPageInitiallyLoaded As Boolean
    Private cPageReturnOnLoadMessasge As String

    Public Property SortReference()
        Get
            Return cSortReference
        End Get
        Set(ByVal Value)
            cSortReference = Value
        End Set
    End Property
    Public Property PageReturnOnLoadMessage()
        Get
            Return cPageReturnOnLoadMessasge
        End Get
        Set(ByVal Value)
            cPageReturnOnLoadMessasge = Value
        End Set
    End Property
    Public Property FilterOnOffState()
        Get
            Return cFilterOnOffState
        End Get
        Set(ByVal Value)
            cFilterOnOffState = Value
        End Set
    End Property
    Public Property PageInitiallyLoaded()
        Get
            Return cPageInitiallyLoaded
        End Get
        Set(ByVal Value)
            cPageInitiallyLoaded = Value
        End Set
    End Property

End Class


Public Class FeedlistSession
    Inherits PageSession

    ' ___ Active fields
    Private cActiveClientID As String
    Private cProcessName As String
    Private cDestinationID As String
    Private cStartDate As String
    Private cProcessTypeID As String

    ' ___ Subtable selection
    Private cSubTableInd As String
    Private cSubTableClientID As String
    Private cSubTableProcessName As String
    Private cSubTableDestinationID As String

    ' ___ Filter selections
    Private cClientIDSelectedFilterValue As String
    Private cProcessTypeIDSelectedFilterValue As String
    Private cCarrierIDSelectedFilterValue As String
    Private cDeveloperSelectedFilterValue As String

    Public Property ProcessTypeID()
        Get
            Return cProcessTypeID
        End Get
        Set(ByVal Value)
            cProcessTypeID = Value
        End Set
    End Property
    Public Property ClientIDSelectedFilterValue()
        Get
            Return cClientIDSelectedFilterValue
        End Get
        Set(ByVal Value)
            cClientIDSelectedFilterValue = Value
        End Set
    End Property
    Public Property ProcessTypeIDSelectedFilterValue()
        Get
            Return cProcessTypeIDSelectedFilterValue
        End Get
        Set(ByVal Value)
            cProcessTypeIDSelectedFilterValue = Value
        End Set
    End Property
    Public Property CarrierIDSelectedFilterValue()
        Get
            Return cCarrierIDSelectedFilterValue
        End Get
        Set(ByVal Value)
            cCarrierIDSelectedFilterValue = Value
        End Set
    End Property
    Public Property DeveloperSelectedFilterValue()
        Get
            Return cDeveloperSelectedFilterValue
        End Get
        Set(ByVal Value)
            cDeveloperSelectedFilterValue = Value
        End Set
    End Property

    Public Property SubTableInd()
        Get
            Return cSubTableInd
        End Get
        Set(ByVal Value)
            cSubTableInd = Value
        End Set
    End Property
    Public Property SubTableClientID()
        Get
            Return cSubTableClientID
        End Get
        Set(ByVal Value)
            cSubTableClientID = Value
        End Set
    End Property
    Public Property SubTableProcessName()
        Get
            Return cSubTableProcessName
        End Get
        Set(ByVal Value)
            cSubTableProcessName = Value
        End Set
    End Property
    Public Property SubTableDestinationID()
        Get
            Return cSubTableDestinationID
        End Get
        Set(ByVal Value)
            cSubTableDestinationID = Value
        End Set
    End Property

    Public Property ActiveClientID()
        Get
            Return cActiveClientID
        End Get
        Set(ByVal Value)
            cActiveClientID = Value
        End Set
    End Property
    Public Property ProcessName()
        Get
            Return cProcessName
        End Get
        Set(ByVal Value)
            cProcessName = Value
        End Set
    End Property
    Public Property DestinationID()
        Get
            Return cDestinationID
        End Get
        Set(ByVal Value)
            cDestinationID = Value
        End Set
    End Property
    Public Property StartDate()
        Get
            Return cStartDate
        End Get
        Set(ByVal Value)
            cStartDate = Value
        End Set
    End Property

End Class

Public Class LogWorklistSession
    Inherits PageSession

    Private cFileID As String
    Private cActiveClientValue As String
    Private cActiveProcessTypeID As String
    Private cClientIDSelectedFilterValue As String
    Private cProcessTypeIDSelectedFilterValue As String
    Private cActiveClientIDOnly As String
    Private cDateRange As String

    Public Property DateRange()
        Get
            Return cDateRange
        End Get
        Set(ByVal Value)
            cDateRange = Value
        End Set
    End Property
    Public Property FileId()
        Get
            Return cFileID
        End Get
        Set(ByVal Value)
            cFileID = Value
        End Set
    End Property
    Public Property ActiveClientValue()
        Get
            Return cActiveClientValue
        End Get
        Set(ByVal Value)
            cActiveClientValue = Value
        End Set
    End Property

    Public Property ActiveProcessTypeID()
        Get
            Return cActiveProcessTypeID
        End Get
        Set(ByVal Value)
            cActiveProcessTypeID = Value
        End Set
    End Property
    Public Property ClientIDSelectedFilterValue()
        Get
            Return cClientIDSelectedFilterValue
        End Get
        Set(ByVal Value)
            cClientIDSelectedFilterValue = Value
        End Set
    End Property


    Public Property ProcessTypeIDSelectedFilterValue()
        Get
            Return cProcessTypeIDSelectedFilterValue
        End Get
        Set(ByVal Value)
            cProcessTypeIDSelectedFilterValue = Value
        End Set
    End Property
    Public Property ActiveClientIDOnly()
        Get
            Return cActiveClientIDOnly
        End Get
        Set(ByVal Value)
            cActiveClientIDOnly = Value
        End Set
    End Property
End Class


