Imports MySql.Data.MySqlClient

Public Class Splash

    Dim start As Integer = 0

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        start += 2
        ProgressBar1.Value = start

        If ProgressBar1.Value >= 100 Then
            Timer1.Stop()
            ProgressBar1.Value = 0

            Dim login As New Login()
            Me.Hide()
            login.ShowDialog()
            Me.Close()
        End If
    End Sub

    Private Sub Splash_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ProgressBar1.Value = 0
        Timer1.Start()
    End Sub

End Class
