Public Class RightsClass
    Private cRightsColl As New Collection

#Region " Constants "
    Public Const FeedlistView = "FLV"
    Public Const FeedlistEdit = "FLE"
    Public Const ScheduleView = "SCV"
    Public Const ScheduleEdit = "SCE"
    Public Const LogView = "LOV"
    Public Const LogEdit = "LOE"
    Public Const HistoryView = "HV"
    Public Const QAFieldView = "QAV"
    Public Const QAFieldEdit = "QAE"
    Public Const BoardRecords = "BR"
#End Region

    'Public Enum AccessLevelEnum
    '    AllAccess = 1
    '    SupervisorAccess = 2
    '    EnrollerAccess = 3
    'End Enum

    Public ReadOnly Property RightsColl()
        Get
            Return cRightsColl
        End Get
    End Property

    Public Sub New(ByVal LoggedInUserID As String, ByVal CurPage As Page)
        Dim Common As Common
        Dim dt As DataTable
        Dim i As Integer
        Dim AllRights(8) As String

        Dim SessionObj As System.Web.SessionState.HttpSessionState = System.Web.HttpContext.Current.Session
        Common = SessionObj("Common")

        dt = Common.GetDT("SELECT Role, LocationID FROM UserManagement..Users WHERE UserID ='" & LoggedInUserID & "'")
        If dt.Rows.Count = 0 Then
            CurPage.Response.Redirect("InsufficientRights.htm")
        End If

        AllRights(0) = "FLV"
        AllRights(1) = "FLE"
        AllRights(2) = "SCV"
        AllRights(3) = "SCE"
        AllRights(4) = "LOV"
        AllRights(5) = "LOE"
        AllRights(6) = "HV"
        AllRights(7) = "QAV"
        AllRights(8) = "QAE"

        Select Case dt.Rows(0)("Role")
            Case "IT"
                For i = 0 To AllRights.GetUpperBound(0)
                    cRightsColl.Add(AllRights(i))
                Next
                'For i = 0 To AllRights.GetUpperBound(0) - 2
                '    cRightsColl.Add(AllRights(i))
                'Next


            Case "ADMIN", "ADMIN LIC", "SUPERVISOR"
                cRightsColl.Add("FLV")
                cRightsColl.Add("SCV")
                cRightsColl.Add("LOV")
                cRightsColl.Add("HV")
                cRightsColl.Add("QAV")
                cRightsColl.Add("QAE")

            Case "CLIENT"
                cRightsColl.Add("LOV")
                cRightsColl.Add("HV")

            Case "ENROLLER"
                ' ___ None
        End Select

        If LoggedInUserID = "rbluestein" Or LoggedInUserID = "jkleiman" Then
            cRightsColl.Add("BR")
        End If
    End Sub

    'Public Sub GetSecurityFlds(ByVal LoggedInUserID As String, ByRef AccessLevel As AccessLevelEnum, ByRef Role As String, ByRef LocationID As String)
    '    Dim Common As New Common
    '    Dim dt As DataTable
    '    dt = Common.GetDT("SELECT Role, LocationID FROM Users WHERE UserID ='" & LoggedInUserID & "'")
    '    Select Case dt.Rows(0)("Role")
    '        Case "ADMIN"
    '            AccessLevel = AccessLevelEnum.AllAccess
    '        Case "SUPERVISOR"
    '            AccessLevel = AccessLevelEnum.SupervisorAccess
    '        Case "ENROLLER"
    '            AccessLevel = AccessLevelEnum.EnrollerAccess
    '    End Select
    '    LocationID = dt.Rows(0)("LocationID")
    'End Sub


    Public Function HasSufficientRights(ByRef RightsRqd As String(), ByVal RedirectOnError As Boolean, ByRef CurPage As System.Web.UI.Page) As Boolean
        Dim i, j As Integer
        Dim Passed As Boolean

        For i = 0 To RightsRqd.GetUpperBound(0)
            For j = 1 To cRightsColl.Count
                If cRightsColl(j) = RightsRqd(i) Then
                    Passed = True
                    Exit For
                End If
            Next
            If Passed Then
                Exit For
            End If
        Next

        If Passed Then
            Return True
        Else
            If RedirectOnError Then
                CurPage.Response.Redirect("InsufficientRights.htm")
            Else
                Return False
            End If
        End If

    End Function

    Public Function HasThisRight(ByVal RightCd As String) As Boolean
        Dim i As Integer
        For i = 1 To cRightsColl.Count
            If cRightsColl(i) = RightCd Then
                Return True
            End If
        Next
        Return False
    End Function

End Class

