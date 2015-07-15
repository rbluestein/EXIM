Imports System.IO

Public Enum WarningEnum
    Warning = 1
    Alert = 2
    ReceiptOverdue = 3
End Enum

Public Enum ScheduleTypeEnum
    DayOfWeek = 1
    DayOfMonth = 2
    DateOfMonth = 3
End Enum

Public Module Main
    Public cCommon As New Common

    Public Sub Main()
        Dim i As Integer
        Dim QueryPack As QueryPack
        AddHandler cCommon.ExitApplication, AddressOf ExitApplication
        Dim dtFeedAdmin As DataTable
        Dim FeedAdminRow As DataRow

        Try

            ' ___ Get the feed administration table
            QueryPack = GetFeedAdminTable()
            If Not QueryPack.Success Then
                cCommon.Report(Common.ReportTypeEnum.Error, "Error #100: " & QueryPack.TechErrMsg)
            End If

            dtFeedAdmin = QueryPack.dt

            For i = 0 To dtFeedAdmin.Rows.Count - 1

                If dtFeedAdmin.Rows(i)("StartDate") <= cCommon.GetServerDateTime Then
                    FeedAdminRow = dtFeedAdmin.Rows(i)
                    AuditThisFeed(i, FeedAdminRow)
                End If

            Next

            ExitApplication()

        Catch ex As Exception
            cCommon.Report(Common.ReportTypeEnum.Error, "Error #2130a: " & ex.Message)
        End Try
    End Sub

    Private Sub AuditThisFeed(ByVal FeedAdminRowNum As Integer, ByRef FeedAdminRow As DataRow)
        Dim Sql As New System.Text.StringBuilder
        Dim QueryPack As QueryPack
        Dim FeedOnTime As Boolean
        Dim FeedIsDue As Boolean
        Dim FeedIsOverdue As Boolean
        Dim FeedIsMoreThan2HoursOverdue As Boolean
        Dim FeedOnTimeNoFileReceipt As Boolean
        Dim HoursInArrears As Double
        Dim Subject As String
        Dim HTMLBody As New System.Text.StringBuilder
        Dim SendTo As String
        Dim SentFrom As String
        Dim cc As String
        Dim dtFeedLogForThisAdminMinus24 As DataTable
        Dim FeedAdminDataPack As FeedAdminDataPack
        Dim FeedScheduledDateTime As DateTime

        ' ___ Email addresses
        'SendTo = FeedAdminRow("Developer") & "@benefitvision.com, jkleiman@benefitvision.com, myacht@benefitvision.com"
        SendTo = "dataexchange@benefitvision.com"
        SentFrom = "automail@benefitvision.com"
        'cc = "rbluestein@benefitvision.com"

        ' ___ When was the most recently scheduled feed?
        FeedAdminDataPack = GetFeedAdminDataPack(FeedAdminRow)

        ' ___ Get a list of all of the feeds run against this admin item beginning 24 hours prior to the actual due datetime.
        QueryPack = GetFeedLog(FeedAdminRowNum, FeedAdminRow, FeedAdminDataPack.SearchStartDateTime)
        If Not QueryPack.Success Then
            cCommon.Report(Common.ReportTypeEnum.Error, "Error #200: " & QueryPack.TechErrMsg)
        End If

        dtFeedLogForThisAdminMinus24 = QueryPack.dt

        FeedScheduledDateTime = FeedAdminDataPack.SearchStartDateTime.AddDays(1).ToString

        GetFeedStatus(dtFeedLogForThisAdminMinus24, FeedAdminDataPack, FeedOnTime, FeedIsDue, FeedIsOverdue, FeedIsMoreThan2HoursOverdue, FeedOnTimeNoFileReceipt)

        ' ___ 11/07/2008: Turned off feed is due notice. Most feeds are automated. Restore after adding an indicator to prevent sending notice for automated feeds.
        FeedIsDue = False

        If FeedOnTimeNoFileReceipt Then
            Subject = "Feed file receipt not yet received"
            HTMLBody.Append("<table>")
            HTMLBody.Append("<tr><td>Message from EXIM Notify</td></tr>")
            HTMLBody.Append("<tr><td>Feed file receipt not yet received notice</td></tr>")
            HTMLBody.Append("<tr><td>&nbsp;</td></tr>")
            HTMLBody.Append("<tr><td>Client: " & FeedAdminRow("ClientID") & "</td></tr>")
            HTMLBody.Append("<tr><td>Process name: " & FeedAdminRow("ProcessName") & "</td></tr>")
            HTMLBody.Append("<tr><td>Last scheduled feed: " & FeedScheduledDateTime.ToString & "</td></tr>")
            HTMLBody.Append("</table>")

        ElseIf FeedIsDue Then
            Subject = "Warning - Feed is due"
            HTMLBody.Append("<table>")
            HTMLBody.Append("<tr><td>Message from EXIM Notify</td></tr>")
            HTMLBody.Append("<tr><td>Warning -- Feed is due notice</td></tr>")
            HTMLBody.Append("<tr><td>&nbsp;</td></tr>")
            HTMLBody.Append("<tr><td>Client: " & FeedAdminRow("ClientID") & "</td></tr>")
            HTMLBody.Append("<tr><td>Process name: " & FeedAdminRow("ProcessName") & "</td></tr>")
            HTMLBody.Append("<tr><td>Next scheduled feed: " & FeedScheduledDateTime.ToString & "</td></tr>")
            HTMLBody.Append("</table>")

        ElseIf FeedIsOverdue Then
            Subject = "Warning - Feed is overdue"
            HTMLBody.Append("<table>")
            HTMLBody.Append("<tr><td>Message from EXIM Notify</td></tr>")
            HTMLBody.Append("<tr><td>Warning -- Feed is overdue notice</td></tr>")
            HTMLBody.Append("<tr><td>&nbsp;</td></tr>")
            HTMLBody.Append("<tr><td>Client: " & FeedAdminRow("ClientID") & "</td></tr>")
            HTMLBody.Append("<tr><td>Process name: " & FeedAdminRow("ProcessName") & "</td></tr>")
            HTMLBody.Append("<tr><td>Last scheduled feed: " & FeedScheduledDateTime.ToString & "</td></tr>")
            HTMLBody.Append("</table>")

        ElseIf FeedIsMoreThan2HoursOverdue Then
            Subject = "Alert - Feed more than 2 hours overdue"
            HTMLBody.Append("<table>")
            HTMLBody.Append("<tr><td>Message from EXIM Notify</td></tr>")
            HTMLBody.Append("<tr><td>Alert -- Feed more than 2 hours overdue notice</td></tr>")
            HTMLBody.Append("<tr><td>&nbsp;</td></tr>")
            HTMLBody.Append("<tr><td>Client: " & FeedAdminRow("ClientID") & "</td></tr>")
            HTMLBody.Append("<tr><td>Process name: " & FeedAdminRow("ProcessName") & "</td></tr>")
            HTMLBody.Append("<tr><td>Last scheduled feed: " & FeedScheduledDateTime.ToString & "</td></tr>")
            HTMLBody.Append("</table>")
        End If

        If FeedOnTimeNoFileReceipt Or FeedIsDue Or FeedIsOverdue Or FeedIsMoreThan2HoursOverdue Then
            cCommon.SendEmail(SendTo, SentFrom, cc, Subject, HTMLBody.ToString)
        End If

    End Sub

    Private Sub GetFeedStatus(ByRef dtFeedLogForThisAdminMinus24 As DataTable, ByRef FeedAdminDataPack As FeedAdminDataPack, ByRef FeedOnTime As Boolean, ByRef FeedIsDue As Boolean, ByRef FeedIsOverdue As Boolean, ByRef FeedIsMoreThan2HoursOverdue As Boolean, ByRef FeedOnTimeNoFileReceipt As Boolean)
        Dim Compare As Integer
        Dim HoursInArrears As Integer
        Dim HoursAfterSearchStartDateTime As Integer


        ' ___ The number of hours right now after the search start time
        HoursAfterSearchStartDateTime = Int(cCommon.GetServerDateTime.Subtract(FeedAdminDataPack.SearchStartDateTime).TotalHours)

        If dtFeedLogForThisAdminMinus24.Rows.Count > 0 Then
            FeedOnTime = True
            If FeedAdminDataPack.ReceiptInd AndAlso IsDBNull(dtFeedLogForThisAdminMinus24.Rows(0)("FileReceiptReceivedDate")) Then
                If dtFeedLogForThisAdminMinus24.Rows(0)("FilePostedDate").AddDays(FeedAdminDataPack.NumDaysForReceipt) < cCommon.GetServerDateTime Then
                    FeedOnTimeNoFileReceipt = True
                End If
            End If
        Else

            'If HoursAfterSearchStartDateTime < 26 Then
            '    FeedIsDue = True
            'Else
            '    FeedIsMoreThan2HoursOverdue = True
            'End If

            If HoursAfterSearchStartDateTime = 12 Or HoursAfterSearchStartDateTime = 23 Then
                FeedIsDue = True
            ElseIf HoursAfterSearchStartDateTime = 24 Or HoursAfterSearchStartDateTime = 25 Then
                FeedIsOverdue = True
            ElseIf HoursAfterSearchStartDateTime > 25 Then
                FeedIsMoreThan2HoursOverdue = True
            End If

        End If

        'Compare = DateTime.Compare(cCommon.GetServerDateTime, FeedAdminDataPack.SearchStartDateTime)
        '-1   t1 is earlier than t2
        '+1   t1 is later than t2

        'If Compare < 1 Then
        '    FeedIsDue = True
        'Else
        '    HoursInArrears = cCommon.GetServerDateTime.Subtract(FeedAdminDataPack.SearchStartDateTime).TotalHours
        '    If HoursInArrears < 2 Then
        '        FeedIsOverdue = True
        '    Else
        '        FeedIsMoreThan2HoursOverdue = True
        '    End If
        'End If

    End Sub

    Private Function GetFeedAdminDataPack(ByRef FeedAdminRow As DataRow) As FeedAdminDataPack
        Dim i As Integer
        Dim ScheduleType As ScheduleTypeEnum
        Dim DayArray As Integer() = {0, 0, 0, 0, 0, 0, 0}
        Dim DayText As String() = {"Sun", "Mon", "Tue", "Wed", "Thur", "Fri", "Sat"}
        Dim MostRecentlyScheduledOccurrenceOfThisFeed As DateTime
        Dim WorkingDateTime As DateTime
        Dim WorkingDateTimeStr As String
        Dim SearchDayNum As Integer
        Dim RightNowStr As String
        Dim TimeOfDay As Integer
        Dim FeedAdminDataPack As New FeedAdminDataPack

        If IsDBNull(FeedAdminRow("TimeOfDay")) Then
            TimeOfDay = 12
        Else
            TimeOfDay = FeedAdminRow("TimeOfDay")
        End If

        If Not IsDBNull(FeedAdminRow("DayOfWeek")) AndAlso FeedAdminRow("DayOfWeek").length > 0 Then
            ScheduleType = ScheduleTypeEnum.DayOfWeek
        ElseIf Not IsDBNull(FeedAdminRow("DayOfMonth")) AndAlso FeedAdminRow("DayOfMonth").length > 0 Then
            ScheduleType = ScheduleTypeEnum.DayOfMonth
        ElseIf Not IsDBNull(FeedAdminRow("DateOfMonth")) AndAlso FeedAdminRow("DateOfMonth").length > 0 Then
            ScheduleType = ScheduleTypeEnum.DateOfMonth
        End If

        If ScheduleType = ScheduleTypeEnum.DayOfWeek Then

            'Dim DayOfYear As Integer
            ' DayOfYear = CurDateTime.Subtract("1/1/2008")
            '  ? curdatetime.Subtract(#1/1/2008#).TotalDays
            '119.69227298948495

            ' ___ Populate the DayArray with the SearchStart day of week (feed schedule day minus 1)
            For i = 0 To 6
                If InStr(FeedAdminRow("DayOfWeek"), DayText(i), CompareMethod.Text) > 0 Then
                    If i = 0 Then
                        DayArray(6) = 1
                    Else
                        DayArray(i - 1) = 1
                    End If
                End If
            Next

            ' ___ Start calculating the feed search start datetime
            RightNowStr = Replace(Date.Now.ToUniversalTime.AddHours(-5).ToString("s"), "T", " ") & ".000"
            WorkingDateTime = Replace(Date.Now.ToUniversalTime.AddHours(-5).ToShortDateString, "/", "-") & " " & " " & CType(TimeOfDay, System.String).PadLeft(2, "0") & ":00:00.000"
            WorkingDateTimeStr = Replace(WorkingDateTime.ToString("s"), "T", " ") & ".000"

            ' ___ If the WorkingDateTime is later than the current datetime, set the initial most recently scheduled feed back one day. This will pick up the corresponding feed the week before.
            If WorkingDateTimeStr > RightNowStr Then
                WorkingDateTime = WorkingDateTime.AddDays(-1)
            End If

            ' SqlSearchDateTime = SearchDateTime.ToString("s")
            ' SqlSearchDateTime = Replace(SqlSearchDateTime, "T", " ") & ".000"


            For i = 1 To 7

                ' ___ Get the day number of the WorkingDateTime
                SearchDayNum = GetDayNum(WorkingDateTime.DayOfWeek.ToString)

                ' ___ Advancing forward, count the number of days to the next scheduled feed in the DayArray
                If DayArray(SearchDayNum) = 1 Then
                    Exit For
                End If

                ' ___ Decrement the WorkingDateTime once for each advance.
                WorkingDateTime = WorkingDateTime.AddDays(-1)
            Next


        ElseIf ScheduleType = ScheduleTypeEnum.DateOfMonth Then

            ' ___ Calculate the feed search start datetime
            WorkingDateTime = Replace(cCommon.GetServerDateTime.ToShortDateString, "/", "-") & " " & " " & CType(TimeOfDay, System.String).PadLeft(2, "0") & ":00:00.000"
            WorkingDateTime = WorkingDateTime.AddMonths(-1)

        End If

        FeedAdminDataPack.SearchStartDateTime = WorkingDateTime
        FeedAdminDataPack.ReceiptInd = FeedAdminRow("ReceiptInd")
        FeedAdminDataPack.BVIOrCarrierSendsErrorReportInd = FeedAdminRow("BVIOrCarrierSendsErrorReportInd")
        FeedAdminDataPack.NumDaysForReceipt = FeedAdminRow("NumDaysForReceipt")
        Return FeedAdminDataPack
    End Function

    Private Function GetDayNum(ByVal DayValue As String) As Integer
        Dim i As Integer
        Dim DayText As String() = {"Sun", "Mon", "Tue", "Wed", "Thur", "Fri", "Sat"}

        For i = 0 To 6
            If InStr(DayValue, DayText(i), CompareMethod.Text) > 0 Then
                Return i
            End If
        Next
    End Function

    Private Function GetFeedAdminTable() As QueryPack
        Dim QueryPack As QueryPack
        Dim Sql As New System.Text.StringBuilder


        Sql.Append("SELECT lower(fs.ClientId) + '_' + cast(fs.DestinationID as varchar(10)) as ScheduleKey, fs.ClientID, fs.DestinationID, ")
        Sql.Append("fs.StartDate, fs.DayOfWeek, fs.DateOfMonth, fs.DayOfMonth, fs.TimeOfDay, fs.ProcessName, ")
        Sql.Append("IsNull(fl.ReceiptInd, 0) as ReceiptInd, IsNull(fs.NumDaysForReceipt, 0) as NumDaysForReceipt, ")
        Sql.Append("IsNull(fl.BVIOrCarrierSendsErrorReportInd, 0) as BVIOrCarrierSendsErrorReportInd, fl.Developer ")
        Sql.Append("FROM EXIM..Feedlist fl ")

        Sql.Append("INNER JOIN EXIM..FeedSchedule fs ON  fs.ClientID = fl.ClientID and fs.ProcessName = fl.ProcessName and fs.DestinationID = fl.DestinationID ")
        Sql.Append("WHERE fl.DestinationID <> 0 AND fl.ActiveInd = 1 AND fs.ActiveInd = 1 ")

        'Sql.Append("SELECT lower(fs.ClientId) + '_' + cast(fs.DestinationID as varchar(10)) as ScheduleKey, fs.*, fl.Developer FROM EXIM..Feedlist fl ")
        'Sql.Append("INNER JOIN EXIM..FeedSchedule fs ON  fs.ClientID = fl.ClientID and fs.ProcessName = fl.ProcessName and fs.DestinationID = fl.DestinationID ")
        'Sql.Append("WHERE fl.DestinationID <> 0 AND fl.ActiveInd = 1 AND fs.ActiveInd = 1 ")

        Debug.WriteLine(Sql.ToString)

        QueryPack = cCommon.GetDTWithQueryPack(Sql.ToString)
        Return QueryPack
    End Function



    Private Function GetFeedLog(ByVal FeedAdminRowNum As Integer, ByRef FeedAdminRow As DataRow, ByVal SearchStartDateTime As DateTime) As QueryPack
        Dim QueryPack As QueryPack
        Dim Sql As New System.Text.StringBuilder
        Dim ClientID As String
        Dim SqlSearchDateTime As String

        'If dr(1) = "BureauVeritas" AndAlso dr("DestinationID") = 107 Then
        '    Stop
        'End If

        ClientID = FeedAdminRow("ClientID").ToString.ToLower
        'If ClientID = "harristeeter" Then
        '    ClientID = "HT"
        'End If

        '' ___ The SqlSearchDateTime value is 24 hours prior to the scheduled time for the feed.
        'MostRecentlyScheduledOccurrenceOfThisFeed = MostRecentlyScheduledOccurrenceOfThisFeed.AddDays(-1)

        SqlSearchDateTime = SearchStartDateTime.ToString("s")
        SqlSearchDateTime = Replace(SqlSearchDateTime, "T", " ") & ".000"

        Sql.Append("SELECT '" & ClientID & "_' + cast(efh.DestinationID as varchar(10)) as ScheduleKey, ")
        Sql.Append("efh.FilePostedDate, efh.FileReceiptReceivedDate ")
        Sql.Append("FROM " & ClientID & "..ExportFileHistory efh ")
        Sql.Append("WHERE efh.DestinationID = " & FeedAdminRow("DestinationID") & " AND efh.FilePostedDate >= '" & SqlSearchDateTime & "' ")
        Sql.Append("ORDER BY efh.FilePostedDate Desc")


        Debug.WriteLine(FeedAdminRowNum)
        Debug.WriteLine(Sql.ToString)
        'Debug.WriteLine(SqlSearchDateTime)

        QueryPack = cCommon.GetDTWithQueryPack(Sql.ToString)
        Return QueryPack
    End Function

    Private Sub ExitApplication()
        '  Application.Exit()
        Environment.Exit(0)
    End Sub
End Module

Public Class FeedAdminDataPack
    Private cSearchStartDateTime As DateTime
    Private cReceiptInd As Boolean
    Private cBVIOrCarrierSendsErrorReportInd As Boolean
    Private cNumDaysForReceipt As Integer

    Public Property SearchStartDateTime() As DateTime
        Get
            Return cSearchStartDateTime
        End Get
        Set(ByVal Value As DateTime)
            cSearchStartDateTime = Value
        End Set
    End Property
    Public Property ReceiptInd() As Boolean
        Get
            Return cReceiptInd
        End Get
        Set(ByVal Value As Boolean)
            cReceiptInd = Value
        End Set
    End Property
    Public Property BVIOrCarrierSendsErrorReportInd() As Boolean
        Get
            Return cBVIOrCarrierSendsErrorReportInd
        End Get
        Set(ByVal Value As Boolean)
            cBVIOrCarrierSendsErrorReportInd = Value
        End Set
    End Property
    Public Property NumDaysForReceipt() As Integer
        Get
            Return cNumDaysForReceipt
        End Get
        Set(ByVal Value As Integer)
            cNumDaysForReceipt = Value
        End Set
    End Property
End Class
