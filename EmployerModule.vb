
Imports MySql.Data.MySqlClient

Public Class EmployerModule

    Private Sub EmployerModule_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim title As String = "Car Wash Management System"
        Dim check As Boolean = False
        'Dim employer As Employer

        'Public Sub New(emp As Employer)
        'InitializeComponent()
        'employer = emp
        'vbRole.SelectedIndex = 3
    End Sub


    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Dim gender As String = If(RdMale.Checked, "Male", "Female")

        ' Validate Phone Number
        Dim phone As String = txtPhone.Text.Trim()


        If Not IsNumeric(phone) OrElse phone.Length <> 10 Then
            MessageBox.Show("Please enter a valid 10-digit phone number.", "Invalid Phone Number", MessageBoxButtons.OK, MessageBoxIcon.Error)
            txtPhone.Focus()
            Exit Sub
        End If



        Try
            checkField()

            If check Then
                If MessageBox.Show("Are you sure you want to register this employer?", title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                    Using conn As MySqlConnection = DBConnection.GetConnection()
                        Dim query As String = "INSERT INTO tb_employer(name, phone, address, dob, gender, role, salary, password) VALUES (@name, @phone, @address, @dob, @gender, @role, @salary, @password)"
                        Using cmd As New MySqlCommand(query, conn)
                            cmd.Parameters.AddWithValue("@name", txtName.Text)
                            cmd.Parameters.AddWithValue("@phone", txtPhone.Text)
                            cmd.Parameters.AddWithValue("@address", txtAddress.Text)
                            cmd.Parameters.AddWithValue("@dob", DtDob.Value)
                            cmd.Parameters.AddWithValue("@gender", If(RdMale.Checked, "Male", "Female"))
                            cmd.Parameters.AddWithValue("@role", vbRole.Text)
                            cmd.Parameters.AddWithValue("@salary", txtSalary.Text)
                            cmd.Parameters.AddWithValue("@password", txtPassword.Text)

                            cmd.ExecuteNonQuery()
                            MessageBox.Show("Employer has been successfully registered!", title, MessageBoxButtons.OK, MessageBoxIcon.Information)

                            check = False
                            Clear()


                        End Using
                    End Using
                End If
            End If
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, title, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try


    End Sub


    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Clear()
        btnUpdate.Enabled = False
        btnSave.Enabled = True
    End Sub


    Private Sub Clear()
        txtName.Clear()
        txtPhone.Clear()
        txtAddress.Clear()
        txtSalary.Clear()
        txtPassword.Clear()
        vbRole.SelectedIndex = -1
        RdMale.Checked = False
        RdFemale.Checked = False
        DtDob.Value = Date.Today

    End Sub


    Private Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click
        Try
            checkField()
            If check Then
                If MessageBox.Show("Are you sure you want to update this employer's details?", title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then

                    Using conn As MySqlConnection = DBConnection.GetConnection()
                        Dim updateQuery As String = "UPDATE tb_employer SET name=@name, phone=@phone, address=@address, dob=@dob, gender=@gender, role=@role, salary=@salary, password=@password WHERE employer_id=@employer_id"

                        Using cmd As New MySqlCommand(updateQuery, conn)
                            cmd.Parameters.AddWithValue("@name", txtName.Text)
                            cmd.Parameters.AddWithValue("@phone", txtPhone.Text)
                            cmd.Parameters.AddWithValue("@address", txtAddress.Text)
                            cmd.Parameters.AddWithValue("@dob", DtDob.Value)
                            cmd.Parameters.AddWithValue("@gender", If(RdMale.Checked, "Male", "Female"))
                            cmd.Parameters.AddWithValue("@role", vbRole.Text)
                            cmd.Parameters.AddWithValue("@salary", txtSalary.Text)
                            cmd.Parameters.AddWithValue("@password", txtPassword.Text)
                            cmd.Parameters.AddWithValue("@employer_id", lblEid.Text)

                            cmd.ExecuteNonQuery()
                            MessageBox.Show("Employer record updated successfully!", title, MessageBoxButtons.OK, MessageBoxIcon.Information)

                            check = False
                            Clear()
                            For Each form As Form In Application.OpenForms
                                If TypeOf form Is setting Then
                                    'DirectCast(form, setting).loadVehicleType()
                                    Exit For
                                End If
                            Next
                        End Using
                    End Using
                End If
            End If
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, title, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    ' Helper method to clear the form after successful update
    Private Sub ClearForm()
        txtName.Clear()
        txtPhone.Clear()
        txtAddress.Clear()
        DtDob.Value = DateTime.Now
        RdMale.Checked = True
        vbRole.SelectedIndex = -1
        txtSalary.Clear()
        txtPassword.Clear()
        lblEid.Text = "0" ' Reset employer ID
    End Sub

    ' Method to load employer data for updating (call this when selecting an employer from a list)
    Public Sub LoadEmployerForUpdate(employerId As Integer)
        Try
            Using conn As MySqlConnection = GetConnection()
                If conn.State = ConnectionState.Open Then
                    Dim query As String = "SELECT * FROM tb_employer WHERE employer_id = @employer_id"

                    Using cmd As New MySqlCommand(query, conn)
                        cmd.Parameters.AddWithValue("@employer_id", employerId)

                        Using reader As MySqlDataReader = cmd.ExecuteReader()
                            If reader.Read() Then

                                lblEid.Text = reader("employer_id").ToString()
                                txtName.Text = reader("name").ToString()
                                txtPhone.Text = reader("phone").ToString()
                                txtAddress.Text = reader("address").ToString()


                                If Not IsDBNull(reader("dob")) Then
                                    DtDob.Value = Convert.ToDateTime(reader("dob"))
                                End If


                                Dim gender As String = reader("gender").ToString()
                                If gender.ToLower() = "male" Then
                                    RdMale.Checked = True
                                Else
                                    RdFemale.Checked = True
                                End If

                                vbRole.Text = reader("role").ToString()
                                txtSalary.Text = reader("salary").ToString()
                                txtPassword.Text = reader("password").ToString()

                            Else
                                MessageBox.Show("Employer not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                            End If
                        End Using
                    End Using
                End If
            End Using

        Catch ex As Exception
            MessageBox.Show("Error loading employer data: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try


    End Sub



    Private check As Boolean = False
    Private title As String = "Employer Registration"

    Private Function checkAge(dob As Date) As Integer
        Dim today As Date = Date.Today
        Dim age As Integer = today.Year - dob.Year
        If dob > today.AddYears(-age) Then
            age -= 1
        End If
        Return age
    End Function

    Public Sub checkField()
        If txtAddress.Text = "" Or txtName.Text = "" Or txtPhone.Text = "" Or txtSalary.Text = "" Then
            MessageBox.Show("Required data field!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If checkAge(DtDob.Value) < 18 Then
            MessageBox.Show("Employer is under 18!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        check = True
    End Sub


    Private Sub txtSalary_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtSalary.KeyPress
        ' Only allow digit numbers
        If Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsDigit(e.KeyChar) AndAlso (e.KeyChar <> "."c) Then
            e.Handled = True
        End If

        ' Only allow one decimal point
        If (e.KeyChar = "."c) AndAlso (CType(sender, TextBox).Text.IndexOf("."c) > -1) Then
            e.Handled = True
        End If


    End Sub

    Private Sub vbRole_SelectedIndexChanged(sender As Object, e As EventArgs) Handles vbRole.SelectedIndexChanged
        If vbRole.Text = "Supervisor" OrElse vbRole.Text = "Worker" Then
            Me.Height = 453 - 26
            txtPassword.Clear()
            lblpass.Visible = False ' To hide password label and textbox
            txtPassword.Visible = False
        Else
            lblpass.Visible = True
            txtPassword.Visible = True
            Me.Height = 453
        End If

    End Sub

    Private Sub txtPhone_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtPhone.KeyPress
        ' Allow control keys (e.g., Backspace)
        If Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsDigit(e.KeyChar) Then
            e.Handled = True
        End If
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Dispose()
    End Sub


    Private Sub txtName_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtName.KeyPress
        ' Allow only letters, backspace, and space
        If Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsLetter(e.KeyChar) AndAlso Not Char.IsWhiteSpace(e.KeyChar) Then
            e.Handled = True ' Block numbers and symbols
        End If
    End Sub

    Private Sub txtPhone_TextChanged(sender As Object, e As EventArgs) Handles txtPhone.TextChanged
        If txtPhone.Text.Length > 10 Then
            MessageBox.Show("Phone number cannot exceed 10 digits.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtPhone.Text = txtPhone.Text.Substring(0, 10)
            txtPhone.SelectionStart = txtPhone.Text.Length ' Keep cursor at the end
        End If
    End Sub

End Class