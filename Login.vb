Imports MySql.Data.MySqlClient

Public Class Login

    Dim cm As New MySqlCommand()
    Dim dbcon As New dbConnect()
    Dim dr As MySqlDataReader
    Dim title As String = "Car Wash Management System"


    Dim company As String = ""
    Dim address As String = ""


    Public Sub New()
        InitializeComponent()
        LoadCompany()
    End Sub


    Private Sub Login_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        txtPassword.PasswordChar = "●"c
    End Sub


    Public Class dbConnect
        Private conn As New MySqlConnection("server=localhost;user id=root;password=;database=carwash_management_system")

        Public Function connect() As MySqlConnection
            Return conn
        End Function

        Public Sub open()
            If conn.State = ConnectionState.Closed Then
                conn.Open()
            End If
        End Sub

        Public Sub close()
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Sub
    End Class


    Public Sub LoadCompany()
        Try
            cm = New MySqlCommand("SELECT * FROM tb_company", dbcon.connect())
            dbcon.open()
            dr = cm.ExecuteReader()

            If dr.Read() Then
                ' Optional - Uncomment if you have labels on the form
                ' lblCompany.Text = dr("name").ToString()
                ' lblAddress.Text = dr("address").ToString()
                company = dr("name").ToString()
                address = dr("address").ToString()
            End If

            dr.Close()
            dbcon.close()
        Catch ex As Exception
            MessageBox.Show("Error loading company info: " & ex.Message, "Error")
        End Try
    End Sub


    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        txtName.Clear()
        txtPassword.Clear()
        txtName.Focus()
    End Sub



    Private Sub btnLogin_Click(sender As Object, e As EventArgs) Handles btnLogin.Click
        Try
            cm = New MySqlCommand("SELECT name FROM tb_employer WHERE BINARY name = @name AND BINARY password = @password", dbcon.connect())
            cm.Parameters.AddWithValue("@name", txtName.Text.Trim())
            cm.Parameters.AddWithValue("@password", txtPassword.Text.Trim())

            dbcon.open()
            dr = cm.ExecuteReader()

            If dr.Read() Then
                MessageBox.Show("WELCOME " & dr("name").ToString() & " | ", "ACCESS GRANTED", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Me.Hide()
                Dim main As New MainForm()
                main.ShowDialog()
            Else
                MessageBox.Show("Invalid username or password", "ACCESS DENIED", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If

            dr.Close()
            dbcon.close()
        Catch ex As Exception
            MessageBox.Show("Login Error: " & ex.Message, title)
        End Try
    End Sub


    Private Sub LoginForm_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Enter Then
            btnLogin.PerformClick()
        End If
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        If CheckBox1.Checked Then
            ' Show password characters
            txtPassword.PasswordChar = ControlChars.NullChar
        Else

            txtPassword.PasswordChar = "●"c
        End If
    End Sub

    Private Sub Panel1_Paint(sender As Object, e As PaintEventArgs) Handles Panel1.Paint

    End Sub

    Private Sub txtPassword_KeyDown(sender As Object, e As KeyEventArgs) Handles txtPassword.KeyDown
        If e.KeyCode = Keys.Enter Then
            btnLogin.PerformClick()
        End If
    End Sub

End Class
