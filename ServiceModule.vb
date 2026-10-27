Imports System.Data.SqlClient
Imports System.Drawing.Drawing2D
Imports System.Text.RegularExpressions
Imports MySql.Data.MySqlClient
Imports Mysqlx.XDevAPI

Public Class ServiceModule


    Dim cm As MySqlCommand
    Dim dr As MySqlDataReader
    Dim title As String = "Car Wash Management System"

    Private callingForm As Service

    Public Sub New(parentForm As Service)
        InitializeComponent()
        callingForm = parentForm
    End Sub



    Private Sub txtPrice_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtPrice.KeyPress
        ' Only allow digit numbers
        If Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsDigit(e.KeyChar) AndAlso (e.KeyChar <> "."c) Then
            e.Handled = True
        End If


        If (e.KeyChar = "."c) AndAlso (CType(sender, TextBox).Text.IndexOf("."c) > -1) Then
            e.Handled = True
        End If

    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Dispose()
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Try
            ' Validate required fields
            If txtFullName.Text = "" Or txtPrice.Text = "" Then
                MessageBox.Show("Required data field!", "Warning")
                Return
            End If

            ' Confirm before saving
            If MessageBox.Show("Are you sure you want to register this service?", "Service Registration", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                ' Create database connection
                Dim con As New MySqlConnection("server=localhost;user id=root;password=;database=carwash_management_system;")
                con.Open()

                ' Prepare MySQL Command
                Dim cmd As New MySqlCommand("INSERT INTO tb_service(name, price) VALUES (@name, @price)", con)
                cmd.Parameters.AddWithValue("@name", txtFullName.Text)
                cmd.Parameters.AddWithValue("@price", txtPrice.Text)

                ' Execute and close
                cmd.ExecuteNonQuery()
                con.Close()

                MessageBox.Show("Service has been successfully registered!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Clear() ' Clear input fields
            End If
            Service.loadService()

        Catch ex As Exception
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Public Sub Clear()
        txtFullName.Clear()
        txtPrice.Clear()

        btnSave.Enabled = True
        btnUpdate.Enabled = False
    End Sub


    Private Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click
        Try
            If MessageBox.Show("Are you sure you want to update this service?", title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                Using conn As MySqlConnection = DBConnection.GetConnection()
                    'conn.Open() ' Make sure the connection is open

                    Dim updateQuery As String = "UPDATE tb_service SET name=@name, price=@price WHERE id=@id"

                    Using cmd As New MySqlCommand(updateQuery, conn)
                        cmd.Parameters.AddWithValue("@id", lblSid.Text)
                        cmd.Parameters.AddWithValue("@name", txtFullName.Text)
                        cmd.Parameters.AddWithValue("@price", txtPrice.Text)

                        Dim result As Integer = cmd.ExecuteNonQuery()

                        If result > 0 Then
                            MessageBox.Show("Service record updated successfully!", title, MessageBoxButtons.OK, MessageBoxIcon.Information)
                            Clear()


                            For Each form As Form In Application.OpenForms
                                If TypeOf form Is setting Then

                                    Exit For
                                End If
                            Next

                            Me.Close()
                        Else
                            MessageBox.Show("No record was updated. Please try again.", title, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        End If
                    End Using
                End Using
            End If
            Service.loadService()

        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, title, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Clear()
        btnUpdate.Enabled = False
        btnSave.Enabled = True
    End Sub

    Private Sub ServiceModule_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub txtFullName_TextChanged(sender As Object, e As EventArgs) Handles txtFullName.TextChanged
        Dim input As String = txtFullName.Text

        ' Check if input starts with a space
        If input.StartsWith(" ") Then
            MessageBox.Show("Service name cannot start with a space.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtFullName.Text = input.TrimStart()
            txtFullName.SelectionStart = txtFullName.Text.Length
            Exit Sub
        End If


        If Regex.IsMatch(input, "\d") Then
            MessageBox.Show("Service name cannot contain numbers.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            ' Remove digits
            txtFullName.Text = Regex.Replace(input, "\d", "")
            txtFullName.SelectionStart = txtFullName.Text.Length
        End If
    End Sub
End Class