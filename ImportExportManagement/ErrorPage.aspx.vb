Public Class ErrorPage
    Inherits System.Web.UI.Page

    Protected PageCaption As HtmlGenericControl

#Region " Web Form Designer Generated Code "

    'This call is required by the Web Form Designer.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub
    Protected WithEvents litError As System.Web.UI.WebControls.Literal
    Protected WithEvents litEnviro As System.Web.UI.WebControls.Literal

    'NOTE: The following placeholder declaration is required by the Web Form Designer.
    'Do not delete or move it.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: This method call is required by the Web Form Designer
        'Do not modify it using the code editor.
        InitializeComponent()
    End Sub

#End Region

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim Common As Common
        Common = Session("Common")
        Dim ErrorMsg As String = String.Empty

        ErrorMsg = Replace(Request.QueryString("ErrorMsg"), "[sharp]", "#")
        ErrorMsg = Replace(ErrorMsg, "~", "<br>")
        litError.Text = Request.QueryString("HeaderMessage") & "<br><br>" & ErrorMsg

        ' ___ Display enviroment
        PageCaption.InnerHtml = Common.GetPageCaption
        litEnviro.Text = "<input type='hidden' name='hdLoggedInUserID' value='" & Common.LoggedInUserID & "'><input type='hidden' name='hdDBHost'  value='hbg-sql'>"
    End Sub

End Class
