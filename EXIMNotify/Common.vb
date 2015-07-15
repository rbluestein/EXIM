Imports System.Data.SqlClient

#Region " Results class "
Public Class Results
    Private cSuccess As Boolean
    Private cMessage As String
    Private cUpdateRequired As Boolean
    Private cReturnStr As String

    Public Property UpdateRequired() As Boolean
        Get
            Return cUpdateRequired
        End Get
        Set(ByVal Value As Boolean)
            cUpdateRequired = Value
        End Set
    End Property
    Public Property Success() As Boolean
        Get
            Return cSuccess
        End Get
        Set(ByVal Value As Boolean)
            cSuccess = Value
        End Set
    End Property
    Public Property Message() As String
        Get
            Return cMessage
        End Get
        Set(ByVal Value As String)
            cMessage = Value
        End Set
    End Property
    Public Property ReturnStr() As String
        Get
            Return cReturnStr
        End Get
        Set(ByVal Value As String)
            cReturnStr = Value
        End Set
    End Property

End Class
#End Region

Public Class Common
    Public Event ExitApplication()
    Public Const VersionNumber As String = "1.2.064"

    Public Function GetServerDateTime() As DateTime
        Return Date.Now.ToUniversalTime.AddHours(-5)
    End Function

    Public ReadOnly Property ConnectionString()
        Get
            ' Return "Provider=SQLOLEDB.1;User ID=BVI_SQL_SERVER;Password=noisivtifeneb;Persist Security Info=True;Initial Catalog=BVI;Data Source=192.168.1.10"
            'Return "Provider=SQLOLEDB.1;User ID=BVI_SQL_SERVER;Password=noisivtifeneb;Persist Security Info=True;ConnectionTimeout=30;Initial Catalog=BVI;Data Source=HBG-SQL"
            Return "User ID=BVI_SQL_SERVER;Password=noisivtifeneb;Persist Security Info=True;Initial Catalog=BVI;Data Source=HBG-SQL"
        End Get
    End Property

    Public Enum ReportTypeEnum
        Information = 1
        [Error] = 2
        ErrorNoShutdown = 3
    End Enum

    Public Sub Report(ByVal ReportType As ReportTypeEnum, ByVal Message As String)
        Dim SendEmailResults As Results

        Select Case ReportType
            Case ReportTypeEnum.Error, ReportTypeEnum.ErrorNoShutdown
                SendEmailResults = SendEmail("rbluestein@benefitvision.com", "rbluestein@benefitvision.com", "", "EXIMNotify error", "An error occurred in the execution of EXIMNotify at " & GetTime())
                If ReportType = ReportTypeEnum.Error Then
                    RaiseEvent ExitApplication()
                End If
        End Select
    End Sub

    Public Function GetTime() As String
        Return Date.Now.ToUniversalTime.AddHours(-5).ToString
    End Function

    Public Function SendEmail(ByVal SendTo As String, ByVal From As String, ByVal cc As String, ByVal Subject As String, ByVal HTMLBody As String) As Results
        Dim schema As String
        Dim MyResults As New Results
        Dim i As Integer

        Try

            Dim CDOConfig As New CDO.Configuration
            schema = "http://schemas.microsoft.com/cdo/configuration/"
            CDOConfig.Fields("http://schemas.microsoft.com/cdo/configuration/sendusing").Value = 2
            ' CDOConfig.Fields("http://schemas.microsoft.com/cdo/configuration/smtpserverport").Value = 25
            CDOConfig.Fields("http://schemas.microsoft.com/cdo/configuration/smtpserver").Value = "mail.benefitvision.com"
            CDOConfig.Fields.Update()

            Dim iMsg As New CDO.Message
            iMsg.To = SendTo
            'iMsg.To = "rbluestein@benefitvision.com"
            iMsg.From = From
            iMsg.CC = cc
            iMsg.Subject = Subject

            iMsg.Configuration = CDOConfig
            iMsg.HTMLBody = HTMLBody

            iMsg.Send()

            ' ___ Clean up
            CDOConfig = Nothing
            iMsg = Nothing

            MyResults.Success = True
            Return MyResults

        Catch ex As Exception
            MyResults.Success = False
            MyResults.Message = "Error #2104: " & ex.Message
            Return MyResults
        End Try
    End Function

    Public Function GetDTWithQueryPack(ByVal Sql As String, Optional ByVal ExtendedTbl As Boolean = False) As QueryPack
        Dim DataAdapter As SqlDataAdapter
        Dim dt As New DataTable
        Dim QueryPack As New QueryPack

        Dim SqlCmd As New SqlCommand(Sql)
        SqlCmd.CommandType = CommandType.Text
        SqlCmd.Connection = New SqlConnection(ConnectionString())
        DataAdapter = New SqlDataAdapter(SqlCmd)
        Try
            DataAdapter.Fill(dt)
            QueryPack.Success = True
            QueryPack.dt = dt
        Catch ex As Exception
            QueryPack.Success = False
            QueryPack.TechErrMsg = ex.Message
        End Try

        DataAdapter.Dispose()
        SqlCmd.Dispose()
        Return QueryPack
    End Function


End Class

Public Class QueryPack
    Private cReturnDataTable As Boolean
    Private cReturnDataSet As Boolean
    Private cSuccess As Boolean
    Private cGenErrMsg As String
    Private cTechErrMsg As String
    Private cdt As DataTable
    Private cds As DataSet

    Public Property Success()
        Get
            Return cSuccess
        End Get
        Set(ByVal Value)
            cSuccess = Value
        End Set
    End Property

    Public ReadOnly Property GenErrMsg()
        Get
            Return GenErrMsg
        End Get
    End Property
    Public Property TechErrMsg()
        Get
            Return cTechErrMsg
        End Get
        Set(ByVal Value)
            cTechErrMsg = Value
        End Set
    End Property
    Public Property dt()
        Get
            Return cdt
        End Get
        Set(ByVal Value)
            cdt = Value
        End Set
    End Property
    Public Property ds()
        Get
            Return cds
        End Get
        Set(ByVal Value)
            cds = Value
        End Set
    End Property
End Class






