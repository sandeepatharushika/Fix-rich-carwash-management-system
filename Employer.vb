
Imports MySql.Data.MySqlClient

Public Class Employer

    Inherits Form

    ' Declare MySQL database components
    Dim cm As MySqlCommand
    Dim dr As MySqlDataReader
    Dim title As String = "Car Wash Management System"

    ' Constructor
    Public Sub New()
        InitializeComponent()
        SetupDataGridView() ' Setup column headers
        loadEmployer() ' Load employer list when form starts
    End Sub

    ' Setup DataGridView columns to match database schema
    Private Sub SetupDataGridView()
        ' Clear existing columns if any
        DGVEmployer.Columns.Clear()

        ' Add columns to match your database schema
        DGVEmployer.Columns.Add("No", "No.")
        DGVEmployer.Columns.Add("employer_id", "ID")
        DGVEmployer.Columns.Add("name", "Name")
        DGVEmployer.Columns.Add("phone", "Phone")
        DGVEmployer.Columns.Add("address", "Address")
        DGVEmployer.Columns.Add("dob", "Date of Birth")
        DGVEmployer.Columns.Add("gender", "Gender")
        DGVEmployer.Columns.Add("role", "Role")
        DGVEmployer.Columns.Add("salary", "Salary")
        DGVEmployer.Columns.Add("password", "Password")

        ' Add action buttons
        Dim editBtn As New DataGridViewButtonColumn()
        editBtn.Name = "Edit"
        editBtn.HeaderText = "Edit"
        editBtn.Text = "Edit"
        editBtn.UseColumnTextForButtonValue = True
        DGVEmployer.Columns.Add(editBtn)

        Dim deleteBtn As New DataGridViewButtonColumn()
        deleteBtn.Name = "Delete"
        deleteBtn.HeaderText = "Delete"
        deleteBtn.Text = "Delete"
        deleteBtn.UseColumnTextForButtonValue = True
        DGVEmployer.Columns.Add(deleteBtn)

        ' Hide the employer_id column (but keep it for reference)
        DGVEmployer.Columns("employer_id").Visible = False

        ' Hide password column for security
        DGVEmployer.Columns("password").Visible = False
    End Sub

    ' Add button click: open employer module in save mode
    Private Sub BtnAdd_Click(sender As Object, e As EventArgs) Handles BtnAdd.Click
        Dim moduleForm As New EmployerModule
        moduleForm.btnUpdate.Enabled = False
        moduleForm.btnSave.Enabled = True
        moduleForm.ShowDialog()
        loadEmployer()
    End Sub


    Private Sub TxtSearch_TextChanged(sender As Object, e As EventArgs) Handles TxtSearch.TextChanged
        loadEmployer()
    End Sub


    Private Sub DGVEmployer_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DGVEmployer.CellContentClick
        If e.RowIndex < 0 Then Return

        Dim colName As String = DGVEmployer.Columns(e.ColumnIndex).Name

        If colName = "Edit" Then
            Dim moduleForm As New EmployerModule


            Dim employerId As String = DGVEmployer.Rows(e.RowIndex).Cells("employer_id").Value.ToString()


            With moduleForm
                'lblEid.Text = GetCellValue(e.RowIndex, "")
                .lblEid.Text = GetCellValue(e.RowIndex, "employer_id")
                .txtName.Text = GetCellValue(e.RowIndex, "name")
                .txtPhone.Text = GetCellValue(e.RowIndex, "phone")
                .txtAddress.Text = GetCellValue(e.RowIndex, "address")
                .DtDob.Text = GetCellValue(e.RowIndex, "dob")
                .RdMale.Checked = (GetCellValue(e.RowIndex, "gender") = "Male")
                .vbRole.Text = GetCellValue(e.RowIndex, "role")
                .txtSalary.Text = GetCellValue(e.RowIndex, "salary")
                .txtPassword.Text = GetCellValue(e.RowIndex, "password")
                .btnSave.Enabled = False


                .Tag = employerId
            End With

            moduleForm.ShowDialog()
            loadEmployer()


        ElseIf colName = "Delete" Then
            Try
                If MessageBox.Show("Are you sure you want to delete this record?", "Delete Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then

                    Dim employerId As String = DGVEmployer.Rows(e.RowIndex).Cells("employer_id").Value.ToString()

                    Using conn As MySqlConnection = DBConnection.GetConnection()
                        If conn.State = ConnectionState.Open Then
                            cm = New MySqlCommand("DELETE FROM tb_employer WHERE employer_id = @employer_id", conn)
                            cm.Parameters.AddWithValue("@employer_id", employerId)

                            Dim result As Integer = cm.ExecuteNonQuery()

                            If result > 0 Then
                                MessageBox.Show("Employer data has been successfully removed!", title, MessageBoxButtons.OK, MessageBoxIcon.Information)
                                loadEmployer() ' Refresh after delete
                            Else
                                MessageBox.Show("No record was deleted. Please try again.", title, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            End If
                        End If
                    End Using
                End If
            Catch ex As Exception
                MessageBox.Show("Delete error: " & ex.Message, title, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub



    Private Function GetCellValue(rowIndex As Integer, columnName As String) As String
        Try
            Dim cellValue = DGVEmployer.Rows(rowIndex).Cells(columnName).Value
            Return If(cellValue IsNot Nothing, cellValue.ToString(), "")
        Catch ex As Exception
            Return ""
        End Try
    End Function


    Public Sub loadEmployer()
        Try
            Dim i As Integer = 0
            DGVEmployer.Rows.Clear()

            Using conn As MySqlConnection = DBConnection.GetConnection()
                If conn.State = ConnectionState.Open Then

                    cm = New MySqlCommand("SELECT employer_id, name, phone, address, dob, gender, role, salary, password FROM tb_employer WHERE CONCAT(IFNULL(name,''), IFNULL(address,''), IFNULL(role,'')) LIKE @search ORDER BY employer_id", conn)
                    cm.Parameters.AddWithValue("@search", "%" & TxtSearch.Text & "%")

                    dr = cm.ExecuteReader()

                    While dr.Read()
                        i += 1


                        Dim dobValue As String = ""
                        If Not IsDBNull(dr("dob")) Then
                            Try
                                dobValue = DateTime.Parse(dr("dob").ToString()).ToShortDateString()
                            Catch
                                dobValue = dr("dob").ToString()
                            End Try
                        End If


                        DGVEmployer.Rows.Add(
                            i, ' No.
                            If(IsDBNull(dr("employer_id")), "", dr("employer_id").ToString()), ' employer_id
                            If(IsDBNull(dr("name")), "", dr("name").ToString()), ' name
                            If(IsDBNull(dr("phone")), "", dr("phone").ToString()), ' phone
                            If(IsDBNull(dr("address")), "", dr("address").ToString()), ' address
                            dobValue, ' dob
                            If(IsDBNull(dr("gender")), "", dr("gender").ToString()), ' gender
                            If(IsDBNull(dr("role")), "", dr("role").ToString()), ' role
                            If(IsDBNull(dr("salary")), "", dr("salary").ToString()), ' salary
                            If(IsDBNull(dr("password")), "", dr("password").ToString()) ' password
                        )
                    End While

                    dr.Close()
                End If

            End Using

            ' Update status
            Me.Text = title & " - " & i.ToString() & " record(s) found"

        Catch ex As Exception
            MessageBox.Show("Load error: " & ex.Message, title, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btn_employerExit_Click(sender As Object, e As EventArgs) 
        Me.Dispose()
    End Sub
End Class