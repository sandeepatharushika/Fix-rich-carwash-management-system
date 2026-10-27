Imports System.Text.RegularExpressions
Imports MySql.Data.MySqlClient



Public Class ManageVehicleType

    Dim cm As MySqlCommand
    Dim dr As MySqlDataReader
    Dim title As String = "Car Wash Management System"

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Dispose()

    End Sub
    Public Sub Clear()
        txtFullName.Clear()
        txtClass.SelectedIndex = 0

        btnSave.Enabled = True
        btnUpdate.Enabled = False
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        ' Validation
        If String.IsNullOrWhiteSpace(txtFullName.Text) Then
            MessageBox.Show("Please enter the Full Name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If

        If String.IsNullOrWhiteSpace(txtClass.Text) Then
            MessageBox.Show("Please select a Class.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If

        ' Your existing save logic here
        Try
            If MessageBox.Show("Are you sure you want to register this Vehicle type?", title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                Using conn As MySqlConnection = DBConnection.GetConnection()
                    Dim query As String = "INSERT INTO tb_vehicle(name, class) VALUES (@name, @class)"
                    Using cmd As New MySqlCommand(query, conn)
                        cmd.Parameters.AddWithValue("@name", txtFullName.Text)
                        cmd.Parameters.AddWithValue("@class", txtClass.Text)
                        cmd.ExecuteNonQuery()

                        MessageBox.Show("Vehicle type has been successfully registered!", title, MessageBoxButtons.OK, MessageBoxIcon.Information)

                        'check = False
                        Clear()

                        For Each form As Form In Application.OpenForms
                            If TypeOf form Is setting Then
                                DirectCast(form, setting).loadVehicleType()
                                Exit For
                            End If
                        Next
                    End Using
                End Using
            End If
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, title, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click
        Try
            If txtFullName.Text = "" Then
                MessageBox.Show("Required vehicle type name!", "Warning")
                Return ' Return to the data field and form
            End If

            If MessageBox.Show("Are you sure you want to edit this vehicle type?", "Vehicle Type Editing", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then

                cm = New MySqlCommand("UPDATE tb_vehicle SET name=@name, class=@class WHERE id=@id", GetConnection())
                cm.Parameters.AddWithValue("@id", lblVid.Text)
                cm.Parameters.AddWithValue("@name", txtFullName.Text)
                cm.Parameters.AddWithValue("@class", txtClass.Text)
                'etConnection.Open() ' To open connection
                cm.ExecuteNonQuery()
                'etConnection.Close() ' To close connection
                MessageBox.Show("Vehicle type has been successfully Edited!", title)
                Clear() ' To clear data field, after data inserted into the database
                For Each form As Form In Application.OpenForms
                    If TypeOf form Is setting Then
                        DirectCast(form, setting).loadVehicleType()
                        Exit For
                    End If
                Next

                Me.Dispose()
            End If

        Catch ex As Exception
            MessageBox.Show(ex.Message, title)
        End Try
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click

    End Sub

    Private Sub ManageVehicleType_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub txtFullName_TextChanged(sender As Object, e As EventArgs) Handles txtFullName.TextChanged
        Dim input As String = txtFullName.Text


        If input.StartsWith(" ") Then
            MessageBox.Show("Full name cannot start with a space.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtFullName.Text = input.TrimStart()
            txtFullName.SelectionStart = txtFullName.Text.Length
            Exit Sub
        End If


        If Regex.IsMatch(input, "\d") Then
            MessageBox.Show("Full name cannot contain numbers.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtFullName.Text = Regex.Replace(input, "\d", "")
            txtFullName.SelectionStart = txtFullName.Text.Length
        End If
    End Sub

End Class