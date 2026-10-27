
Imports MySql.Data.MySqlClient
Imports MySql.Data.MySqlClient.MySqlConnection
Imports Mysqlx.XDevAPI
Imports Org.BouncyCastle.Asn1.Crmf
Imports System.Data
Imports System.Data.SqlClient
Imports System.Reflection
Imports System.Text.RegularExpressions
Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Public Class CustomerModule


    Partial Public Class CustomerModule
        Inherits Form

        Dim cm As New SqlCommand()
        Dim dbcon As New MySqlConnection()
        Dim title As String = "Car Wash Management System"
        Dim checkFlag As Boolean = False

    End Class

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Dispose()
    End Sub

    Private editMode As Boolean = False
    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        ' Trim all text fields first
        txtName.Text = txtName.Text.Trim()
        txtPhone.Text = txtPhone.Text.Trim()
        txtCarNo.Text = txtCarNo.Text.Trim()
        txtCarModel.Text = txtCarModel.Text.Trim()
        txtAddress.Text = txtAddress.Text.Trim()

        ' 1. Check for empty fields
        If String.IsNullOrWhiteSpace(txtName.Text) OrElse
       String.IsNullOrWhiteSpace(txtPhone.Text) OrElse
       String.IsNullOrWhiteSpace(txtCarNo.Text) OrElse
       String.IsNullOrWhiteSpace(txtCarModel.Text) OrElse
       String.IsNullOrWhiteSpace(cbCarType.Text) OrElse
       String.IsNullOrWhiteSpace(txtAddress.Text) Then

            MessageBox.Show("Please fill in all fields before saving.", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        ' 2. Full Name validation
        If txtName.Text.StartsWith(" ") OrElse Not Regex.IsMatch(txtName.Text, "^[A-Za-z\s]+$") Then
            MessageBox.Show("Full Name must only contain letters and must not start with a space.", "Invalid Name", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtName.Focus()
            Exit Sub
        End If

        ' 3. Phone Number validation
        If Not Regex.IsMatch(txtPhone.Text, "^\d{10}$") Then
            MessageBox.Show("Phone number must be exactly 10 digits.", "Invalid Phone", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtPhone.Focus()
            Exit Sub
        End If

        ' 4. Car Model validation
        If txtCarModel.Text.StartsWith(" ") OrElse Not Regex.IsMatch(txtCarModel.Text, "^[A-Za-z\s]+$") Then
            MessageBox.Show("Car Model must only contain letters and must not start with a space.", "Invalid Car Model", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtCarModel.Focus()
            Exit Sub
        End If

        ' 5. Address validation
        If txtAddress.Text.StartsWith(" ") Then
            MessageBox.Show("Address must not start with a space.", "Invalid Address", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtAddress.Focus()
            Exit Sub
        End If

        ' 6. Check for duplicate car number
        If CarNoExists(txtCarNo.Text) Then
            MessageBox.Show("This car number already exists. Please enter a unique one.", "Duplicate Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtCarNo.Focus()
            Exit Sub
        End If

        ' 7. Save to database
        Try
            Dim con As New MySqlConnection("server=localhost;user id=root;password=;database=carwash_management_system;")
            con.Open()

            Dim query As String
            If editMode = False Then
                query = "INSERT INTO tb_customer (name, phone, car_no, car_model, vid, address, points) " &
                    "VALUES (@name, @phone, @car_no, @car_model, @vid, @address, @points)"
            Else
                query = "UPDATE tb_customer SET name=@name, phone=@phone, car_no=@car_no, car_model=@car_model, vid=@vid, address=@address, points=@points WHERE id=@id"
            End If

            Dim cmd As New MySqlCommand(query, con)
            cmd.Parameters.AddWithValue("@name", txtName.Text)
            cmd.Parameters.AddWithValue("@phone", txtPhone.Text)
            cmd.Parameters.AddWithValue("@car_no", txtCarNo.Text)
            cmd.Parameters.AddWithValue("@car_model", txtCarModel.Text)
            cmd.Parameters.AddWithValue("@vid", cbCarType.SelectedValue)
            cmd.Parameters.AddWithValue("@address", txtAddress.Text)
            cmd.Parameters.AddWithValue("@points", udPoints.Text)

            If editMode = True Then
                cmd.Parameters.AddWithValue("@id", lblCid.Text)
            End If

            cmd.ExecuteNonQuery()
            con.Close()
            ClearFields()
            Customer.LoadCustomer()
            MessageBox.Show("Customer information saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)


        Catch ex As Exception
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub



    Private Sub ClearFields()
        txtName.Clear()
        txtPhone.Clear()
        txtCarNo.Clear()
        txtCarModel.Clear()
        cbCarType.SelectedIndex = -1
        txtAddress.Clear()
        udPoints.Text = "0"
        editMode = False
    End Sub

    Private Function ValidateInputs() As Boolean
        ' Validate Full Name
        Dim name As String = txtName.Text.Trim()
        If String.IsNullOrWhiteSpace(name) OrElse Not System.Text.RegularExpressions.Regex.IsMatch(name, "^[A-Za-z][A-Za-z\s]*$") Then
            MessageBox.Show("Full Name must not start with a space and should contain only letters.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtName.Focus()
            Return False
        End If

        ' Validate Phone Number (must be 10 digits)
        Dim phone As String = txtPhone.Text.Trim()
        If Not System.Text.RegularExpressions.Regex.IsMatch(phone, "^\d{10}$") Then
            MessageBox.Show("Phone number must be exactly 10 digits.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtPhone.Focus()
            Return False
        End If

        ' Validate Car Number (must be unique)
        Dim carNo As String = txtCarNo.Text.Trim()
        If String.IsNullOrEmpty(carNo) Then
            MessageBox.Show("Car number cannot be empty.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtCarNo.Focus()
            Return False
        End If
        If CarNoExists(carNo) Then
            MessageBox.Show("This car number already exists.", "Duplicate Car Number", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtCarNo.Focus()
            Return False
        End If

        ' Validate Address
        If String.IsNullOrWhiteSpace(txtAddress.Text.Trim()) Then
            MessageBox.Show("Address is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtAddress.Focus()
            Return False
        End If

        ' All validations passed
        Return True
    End Function


    Private Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click
        ' Trim all text fields
        txtName.Text = txtName.Text.Trim()
        txtPhone.Text = txtPhone.Text.Trim()
        txtCarNo.Text = txtCarNo.Text.Trim()
        txtCarModel.Text = txtCarModel.Text.Trim()
        txtAddress.Text = txtAddress.Text.Trim()

        ' 1. Check for empty fields
        If String.IsNullOrWhiteSpace(txtName.Text) OrElse
       String.IsNullOrWhiteSpace(txtPhone.Text) OrElse
       String.IsNullOrWhiteSpace(txtCarNo.Text) OrElse
       String.IsNullOrWhiteSpace(txtCarModel.Text) OrElse
       String.IsNullOrWhiteSpace(cbCarType.Text) OrElse
       String.IsNullOrWhiteSpace(txtAddress.Text) Then

            MessageBox.Show("Please fill in all fields before updating.", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        ' 2. Full Name validation
        If txtName.Text.StartsWith(" ") OrElse Not Regex.IsMatch(txtName.Text, "^[A-Za-z\s]+$") Then
            MessageBox.Show("Full Name must only contain letters and must not start with a space.", "Invalid Name", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtName.Focus()
            Exit Sub
        End If

        ' 3. Phone validation (10 digits)
        If Not Regex.IsMatch(txtPhone.Text, "^\d{10}$") Then
            MessageBox.Show("Phone number must be exactly 10 digits.", "Invalid Phone", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtPhone.Focus()
            Exit Sub
        End If

        ' 4. Car No validation (must be unique except for current record)
        'If CarNoExistsForOther(txtCarNo.Text, lblCid.Text) Then
        'MessageBox.Show("This car number already exists for another customer.", "Duplicate Car No.", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        'txtCarNo.Focus()
        'Exit Sub
        'End If

        ' 5. Car Model validation
        If txtCarModel.Text.StartsWith(" ") OrElse Not Regex.IsMatch(txtCarModel.Text, "^[A-Za-z\s]+$") Then
            MessageBox.Show("Car Model must only contain letters and must not start with a space.", "Invalid Car Model", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtCarModel.Focus()
            Exit Sub
        End If

        ' 6. Address validation
        If txtAddress.Text.StartsWith(" ") Then
            MessageBox.Show("Address must not start with a space.", "Invalid Address", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtAddress.Focus()
            Exit Sub
        End If

        ' 7. Perform the update
        Try
            Dim con As New MySqlConnection("server=localhost;user id=root;password=;database=carwash_management_system;")
            con.Open()

            Dim query As String = "UPDATE tb_customer SET name=@name, phone=@phone, car_no=@car_no, car_model=@car_model, vid=@vid, address=@address, points=@points WHERE id=@id"

            Dim cmd As New MySqlCommand(query, con)
            cmd.Parameters.AddWithValue("@name", txtName.Text)
            cmd.Parameters.AddWithValue("@phone", txtPhone.Text)
            cmd.Parameters.AddWithValue("@car_no", txtCarNo.Text)
            cmd.Parameters.AddWithValue("@car_model", txtCarModel.Text)
            cmd.Parameters.AddWithValue("@vid", cbCarType.SelectedValue)
            cmd.Parameters.AddWithValue("@address", txtAddress.Text)
            cmd.Parameters.AddWithValue("@points", udPoints.Text)
            cmd.Parameters.AddWithValue("@id", lblCid.Text)

            cmd.ExecuteNonQuery()
            con.Close()

            ClearFields()
            MessageBox.Show("Customer information updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Customer.LoadCustomer()

        Catch ex As Exception
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Clear()
    End Sub

    Private Sub CustomerModule_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' To add vehicle list in the combobox
        cbCarType.DataSource = vehicleType()
        cbCarType.DisplayMember = "name"
        cbCarType.ValueMember = "id"
    End Sub

    Public Function vehicleType() As DataTable
        Dim conn As New MySqlConnection("server=localhost;user id=root;password=;database=carwash_management_system;")
        Dim cmd As New MySqlCommand("SELECT * FROM tb_Vehicle", conn)
        Dim adapter As New MySqlDataAdapter()
        Dim dataTable As New DataTable()

        Try
            conn.Open()
            adapter.SelectCommand = cmd
            adapter.Fill(dataTable)
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        Finally
            conn.Close()
        End Try

        Return dataTable
    End Function

    Public Sub Clear()
        txtAddress.Clear()
        txtCarModel.Clear()
        txtCarNo.Clear()
        txtName.Clear()
        txtPhone.Clear()

        cbCarType.SelectedIndex = 0
        udPoints.Value = 0

        btnSave.Enabled = True
        btnUpdate.Enabled = False
    End Sub

    Private Sub CheckField()
        Dim check As Boolean = False ' Declare the variable

        ' Check if any required field is empty
        If txtName.Text = "" OrElse
           txtPhone.Text = "" OrElse
           txtCarNo.Text = "" OrElse
           txtCarModel.Text = "" OrElse
           txtAddress.Text = "" Then

            MessageBox.Show("Required data field!", "Warning")
            Return
        End If

        check = True ' Set to True when all fields are valid
        ' Do something with check if needed
    End Sub


    Private Sub txtPhone_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtPhone.KeyPress
        ' Allow only digits and control keys (e.g. backspace)
        If Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsDigit(e.KeyChar) Then
            e.Handled = True ' Ignore the key press
        End If
    End Sub

    Private Sub txtName_TextChanged(sender As Object, e As EventArgs) Handles txtName.TextChanged
        ' Remove leading spaces
        Dim input As String = txtName.Text.TrimStart()

        ' Remove numbers and multiple spaces
        input = System.Text.RegularExpressions.Regex.Replace(input, "[^A-Za-z\s]", "")
        input = System.Text.RegularExpressions.Regex.Replace(input, "\s{2,}", " ")

        ' Update textbox only if modified
        If txtName.Text <> input Then
            Dim cursorPos As Integer = txtName.SelectionStart
            txtName.Text = input
            txtName.SelectionStart = If(cursorPos > txtName.Text.Length, txtName.Text.Length, cursorPos)
        End If
    End Sub

    Private Sub txtPhone_TextChanged(sender As Object, e As EventArgs) Handles txtPhone.TextChanged
        ' Remove any non-digit characters
        Dim digitsOnly As String = System.Text.RegularExpressions.Regex.Replace(txtPhone.Text, "[^\d]", "")

        ' Limit to 10 digits
        If digitsOnly.Length > 10 Then
            digitsOnly = digitsOnly.Substring(0, 10)
        End If


        If txtPhone.Text <> digitsOnly Then
            Dim cursorPos As Integer = txtPhone.SelectionStart
            txtPhone.Text = digitsOnly
            txtPhone.SelectionStart = If(cursorPos > txtPhone.Text.Length, txtPhone.Text.Length, cursorPos)
        End If
    End Sub

    Private Function CarNoExists(carNo As String) As Boolean
        Dim exists As Boolean = False
        Dim query As String = "SELECT COUNT(*) FROM tb_customer WHERE car_no = @carNo"

        Using con As New MySqlConnection("server=localhost;user id=root;password=;database=carwash_management_system;")
            Using cmd As New MySqlCommand(query, con)
                cmd.Parameters.AddWithValue("@carNo", carNo)
                con.Open()
                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())
                exists = (count > 0)
            End Using
        End Using

        Return exists
    End Function

    Private Sub txtCarModel_TextChanged(sender As Object, e As EventArgs) Handles txtCarModel.TextChanged
        ' Remove leading spaces
        If txtCarModel.Text.StartsWith(" ") Then
            txtCarModel.Text = txtCarModel.Text.TrimStart()
            txtCarModel.SelectionStart = txtCarModel.Text.Length ' Set cursor to end
        End If

        ' Block numbers
        If System.Text.RegularExpressions.Regex.IsMatch(txtCarModel.Text, "\d") Then
            MessageBox.Show("Car Model cannot contain numbers.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtCarModel.Text = System.Text.RegularExpressions.Regex.Replace(txtCarModel.Text, "\d", "")
            txtCarModel.SelectionStart = txtCarModel.Text.Length ' Keep cursor at the end
        End If
    End Sub

    Private Sub txtAddress_TextChanged(sender As Object, e As EventArgs) Handles txtAddress.TextChanged
        ' Check if address starts with a space
        If txtAddress.Text.StartsWith(" ") Then
            ' Remove leading space
            txtAddress.Text = txtAddress.Text.TrimStart()
            txtAddress.SelectionStart = txtAddress.Text.Length ' Set cursor at the end
            MessageBox.Show("Address cannot start with a space.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

End Class