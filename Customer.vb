Imports System.Data.SqlClient
Imports MySql.Data.MySqlClient
Imports Mysqlx.XDevAPI

Public Class Customer

    Dim cm As MySqlCommand
    Dim dr As MySqlDataReader
    Dim title As String = "Car Wash Management System"

    Public Sub New()
        GetConnection()
        InitializeComponent()
        'SetupDataGridView() ' Setup column headers
        LoadCustomer()
    End Sub


    Private Sub BtnAdd_Click(sender As Object, e As EventArgs) Handles BtnAdd.Click
        Dim moduleForm As New CustomerModule()
        moduleForm.btnUpdate.Enabled = False
        moduleForm.ShowDialog()
    End Sub

    Private Sub dgvCustomer_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvCustomer.CellContentClick

        Dim colName As String = dgvCustomer.Columns(e.ColumnIndex).Name


        If colName = "Edit" Then

            Dim moduleForm As New CustomerModule()
            With moduleForm
                .lblCid.Text = dgvCustomer.Rows(e.RowIndex).Cells(1).Value.ToString()
                .txtName.Text = dgvCustomer.Rows(e.RowIndex).Cells(2).Value.ToString()
                .txtPhone.Text = dgvCustomer.Rows(e.RowIndex).Cells(3).Value.ToString()
                .txtCarNo.Text = dgvCustomer.Rows(e.RowIndex).Cells(4).Value.ToString()
                .txtCarModel.Text = dgvCustomer.Rows(e.RowIndex).Cells(5).Value.ToString()
                .cbCarType.SelectedValue = (dgvCustomer.Rows(e.RowIndex).Cells(6).Value.ToString())
                .txtAddress.Text = dgvCustomer.Rows(e.RowIndex).Cells(7).Value.ToString()
                .udPoints.Text = dgvCustomer.Rows(e.RowIndex).Cells(8).Value.ToString()

                .btnSave.Enabled = False
                .udPoints.Enabled = True
                .ShowDialog()
            End With

        ElseIf colName = "Delete" Then
            Try
                If MessageBox.Show("Are you sure you want to delete this record?", "Delete Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then


                    Dim Id As String = dgvCustomer.Rows(e.RowIndex).Cells(1).Value.ToString()


                    MessageBox.Show("Deleting customer with ID: " & Id)

                    Using conn As MySqlConnection = DBConnection.GetConnection()
                        If conn.State = ConnectionState.Open Then
                            Dim cm As New MySqlCommand("DELETE FROM tb_customer WHERE id = @id", conn)
                            cm.Parameters.AddWithValue("@id", Id)

                            Dim result As Integer = cm.ExecuteNonQuery()

                            If result > 0 Then
                                MessageBox.Show("Customer data has been successfully removed!", title, MessageBoxButtons.OK, MessageBoxIcon.Information)
                                LoadCustomer()
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

    Public Sub LoadCustomer()
        Try
            Dim i As Integer = 0
            dgvCustomer.Rows.Clear()


            Dim con As New MySqlConnection("server=localhost;user id=root;password=;database=carwash_management_system;")
            con.Open()


            Dim query As String = "SELECT C.id, C.name, C.phone, C.car_no, C.car_model, V.name AS vehicle_name, C.address, C.points " &
                              "FROM tb_customer AS C " &
                              "INNER JOIN tb_vehicle AS V ON C.vid = V.id " &
                              "WHERE CONCAT(C.name, C.car_no, C.car_model, C.address) LIKE @search"

            Dim cmd As New MySqlCommand(query, con)
            cmd.Parameters.AddWithValue("@search", "%" & TxtSearch.Text & "%")

            Dim dr As MySqlDataReader = cmd.ExecuteReader()

            While dr.Read()
                i += 1
                dgvCustomer.Rows.Add(i,
                                 dr("id").ToString(),
                                 dr("name").ToString(),
                                 dr("phone").ToString(),
                                 dr("car_no").ToString(),
                                 dr("car_model").ToString(),
                                 dr("vehicle_name").ToString(),
                                 dr("address").ToString(),
                                 dr("points").ToString())
            End While

            con.Close()

        Catch ex As Exception
            MessageBox.Show(ex.Message, "Error")
        End Try
    End Sub



    Private Sub Customer_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadCustomer()
    End Sub

    Private Sub TxtSearch_TextChanged(sender As Object, e As EventArgs) Handles TxtSearch.TextChanged
        LoadCustomer()
    End Sub
End Class