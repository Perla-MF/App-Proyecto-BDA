Imports System.Runtime.InteropServices
Imports System.Data.SqlClient

Public Class Login
    Dim cGrip As Integer = 10
    Dim query As String
    Shared cnx As New SqlConnection
    Public cmd As SqlClient.SqlCommand
    Public sqlread As SqlClient.SqlDataReader

    Protected Overrides Sub WndProc(ByRef m As Message)
        If (m.Msg = 132) Then
            Dim pos As Point = New Point((m.LParam.ToInt32 And 65535), (m.LParam.ToInt32 + 16))
            pos = Me.PointToClient(pos)
            If ((pos.X _
                        >= (Me.ClientSize.Width - cGrip)) _
                        AndAlso (pos.Y _
                        >= (Me.ClientSize.Height - cGrip))) Then
                m.Result = CType(17, IntPtr)
                Return
            End If
        End If
        MyBase.WndProc(m)
    End Sub
    Dim sizeGripRectangle As Rectangle
    Dim tolerance As Integer = 15

    Protected Overrides Sub OnSizeChanged(ByVal e As EventArgs)
        MyBase.OnSizeChanged(e)
        Dim region = New Region(New Rectangle(0, 0, Me.ClientRectangle.Width, Me.ClientRectangle.Height))
        sizeGripRectangle = New Rectangle((Me.ClientRectangle.Width - tolerance), (Me.ClientRectangle.Height - tolerance), tolerance, tolerance)
        region.Exclude(sizeGripRectangle)
        Me.PanelC.Region = region
        Me.Invalidate()
    End Sub

    Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
        Dim blueBrush As SolidBrush = New SolidBrush(Color.FromArgb(244, 244, 244))
        e.Graphics.FillRectangle(blueBrush, sizeGripRectangle)
        MyBase.OnPaint(e)
        ControlPaint.DrawSizeGrip(e.Graphics, Color.Transparent, sizeGripRectangle)
    End Sub

    <DllImport("user32.DLL", EntryPoint:="ReleaseCapture")>
    Private Shared Sub ReleaseCapture()
    End Sub
    <DllImport("user32.DLL", EntryPoint:="SendMessage")>
    Private Shared Sub SendMessage(ByVal hWnd As System.IntPtr, ByVal wMsg As Integer, ByVal wParam As Integer, ByVal lParam As Integer)
    End Sub
    Private Sub PanelTitle_MouseMove(sender As Object, e As MouseEventArgs) Handles PanelTitle.MouseMove
        ReleaseCapture()
        SendMessage(Me.Handle, &H112&, &HF012&, 0)
    End Sub

    Private Sub Label4_Click(sender As Object, e As EventArgs) Handles Label4.Click
        AddUser.Show()
    End Sub

    Private Sub Login_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        cnx.ConnectionString = "Data Source=localhost\SQLEXPRESS; Initial Catalog=Goodreads;Integrated Security=True"
    End Sub

    Private Sub btnLogin_Click(sender As Object, e As EventArgs) Handles btnLogin.Click
        cnx.Open()
        query = "SELECT id FROM [User] WHERE username = '" & txtUser.Text & "' AND [password] = '" & txtPassword.Text & "'"
        cmd = New SqlCommand(query, cnx)
        sqlread = cmd.ExecuteReader

        If sqlread.HasRows Then
            sqlread.Read()
            AppConfig.UserID = sqlread.GetInt32(0)

            FormPrincipal.Show()
            Me.Hide()
        Else
            MsgBox("Usuario o contraseña incorrectos.")
        End If
        cnx.Close()
    End Sub

End Class