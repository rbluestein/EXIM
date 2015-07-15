Imports System.Web
Imports System.Web.SessionState

Public Class Global
    Inherits System.Web.HttpApplication

#Region " Component Designer Generated Code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Component Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    'Required by the Component Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Component Designer
    'It can be modified using the Component Designer.
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        components = New System.ComponentModel.Container()
    End Sub

#End Region

    Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when the application is started

    End Sub

    Sub Session_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when the session is started
        Dim LogWorklistSession As New LogWorklistSession
        Session("LogWorklistSession") = LogWorklistSession
        Dim FeedlistSession As New FeedlistSession
        Session("FeedlistSession") = FeedlistSession
        Dim Common As New Common
        Session("Common") = Common
    End Sub

    Sub Application_BeginRequest(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires at the beginning of each request
    End Sub

    Sub Application_AuthenticateRequest(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires upon attempting to authenticate the use
    End Sub

    'Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
    '    ' Fires when an error occurs
    '    Dim msg As String
    '    Dim ex As Exception
    '    'Dim Request As HttpRequest

    '    ex = Server.GetLastError.InnerException
    '    'If ex.Message <> "Thread was being aborted." Then
    '    msg = ex.InnerException.Message & "~" & ex.InnerException.StackTrace
    '    msg = Replace(msg, vbCrLf, "~")
    '    Response.Redirect("ErrorPage.aspx?ErrorMsg=" & msg)
    '    'End If
    'End Sub

    Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
        Dim Querystring As String
        Dim ErrorObj As ErrorObj
        Dim ErrorMessage As String
        Dim HeaderMessage As String
        Dim ReportType As ReportTypeEnum
        Dim Common As Common

        Common = Session("Common")

        ErrorObj = New ErrorObj(Server.GetLastError, Request)
        ErrorMessage = ErrorObj.GetDisplayErrorMessage()

        ReportType = ReportTypeEnum.Error
        If ErrorObj.IsCookieError Then
            ErrorMessage = "No cookie present."
        ElseIf ErrorObj.IsTimeOutError Then
            ReportType = ReportTypeEnum.Timeout
            ErrorMessage = "Application has timed out. Please close the application and log back in."
        ElseIf ErrorObj.IsPostTimeOutError Then
            ReportType = ReportTypeEnum.Timeout
            ErrorMessage = "Application has timed out. Please close the application and log back in."
        End If

        Common.SendEmail("rbluestein@benefitvision.com", Common.LoggedInUserID & "@benefitvision.com", Nothing, "EXIM error", ErrorMessage)

        HeaderMessage = "An error has occurred requiring EXIM to shut down."
        Select Case ReportType
            Case ReportTypeEnum.Error, ReportTypeEnum.Timeout
                ErrorMessage = Replace(ErrorMessage, "#", "[sharp]")
                ErrorMessage = Replace(ErrorMessage, vbCrLf, "~")
                Response.Redirect("ErrorPage.aspx?ErrorMsg=" & ErrorMessage & "&HeaderMessage=" & HeaderMessage)
        End Select
    End Sub

    Sub Session_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when the session ends
    End Sub

    Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when the application ends
    End Sub

End Class
