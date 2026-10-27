Imports MySql.Data.MySqlClient
Imports Mysqlx.XDevAPI
Imports System.Data
Imports System.Data.SqlClient
Imports System.Runtime.CompilerServices


Public Class setting
    ' Declare variables at class level
    Dim cm As MySqlCommand
    Dim dr As MySqlDataReader
    Dim title As String = "Car Wash Management System"
    Dim hasdetail As Boolean = False

    Private Sub btnADDCOGS_Click(sender As Object, e As EventArgs)
        Dim moduleForm As New ManageVehicleType
        moduleForm.btnUpdate.Enabled = False
        moduleForm.ShowDialog()
    End Sub


    'Private Sub dgvVehicleType_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgVehicleType.CellContentClick


    Public Sub loadVehicleType()
        Try
            Dim i As Integer = 0 ' to show number for vehicle list
            dgvVehicleType.Rows.Clear()
            cm = New MySqlCommand("SELECT * FROM tb_Vehicle WHERE CONCAT(name,class) LIKE '%" & TxtSearchVT.Text & "%'", GetConnection)
            dr = cm.ExecuteReader()
            While dr.Read()
                i = i + 1
                ' to add data to the datagridview from the database
                dgvVehicleType.Rows.Add(i, dr(0).ToString(), dr(1).ToString(), dr(2).ToString())
            End While
            GetConnection.Close()
        Catch ex As Exception
            MessageBox.Show(ex.Message, title)
        End Try
    End Sub

    Private Sub setting_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        loadVehicleType()
        loadCostOfGoodSold()
        LoadCompany()

    End Sub

    Private Sub BtnAdd_Click(sender As Object, e As EventArgs) Handles btnAddVT.Click, btnAddVT.Click
        Dim moduleForm As New ManageVehicleType
        moduleForm.btnUpdate.Enabled = False
        moduleForm.ShowDialog()
    End Sub

    Private Sub TxtSearchVT_TextChanged_2(sender As Object, e As EventArgs) Handles TxtSearchVT.TextChanged
        loadVehicleType()
    End Sub

    Private Sub dgvVehicleType_CellContentClick_1(sender As Object, e As DataGridViewCellEventArgs) Handles dgvVehicleType.CellContentClick

        If e.RowIndex >= 0 Then
            Dim colName As String = dgvVehicleType.Columns(e.ColumnIndex).Name

            If colName = "Edit" Then

                Dim moduleForm As New ManageVehicleType()
                moduleForm.lblVid.Text = dgvVehicleType.Rows(e.RowIndex).Cells(1).Value.ToString()
                moduleForm.txtFullName.Text = dgvVehicleType.Rows(e.RowIndex).Cells(2).Value.ToString()
                moduleForm.txtClass.Text = dgvVehicleType.Rows(e.RowIndex).Cells(3).Value.ToString()

                moduleForm.btnSave.Enabled = False
                moduleForm.ShowDialog()

            ElseIf colName = "Delete" Then
                Try
                    If MessageBox.Show("Are you sure you want to delete this record?", "Delete Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                        Dim deleteId As String = dgvVehicleType.Rows(e.RowIndex).Cells(1).Value.ToString()


                        cm = New MySqlCommand("DELETE FROM tb_Vehicle WHERE id = @id")
                        cm.Parameters.AddWithValue("@id", deleteId)

                        cm.Connection = GetConnection() ' Assuming GetConnection() returns a valid MySqlConnection

                        cm.ExecuteNonQuery()

                        GetConnection.Close()
                        MessageBox.Show("Vehicle type has been successfully deleted!", title, MessageBoxButtons.OK, MessageBoxIcon.Information)

                        ' Refresh DataGridView after deletion
                        loadVehicleType()
                    End If
                Catch ex As Exception
                    MessageBox.Show("Error deleting record: " & ex.Message, title)
                End Try
            End If
        End If
    End Sub





    Private Sub btnAddCOG_Click(sender As Object, e As EventArgs) Handles btnAddCOG.Click
        Dim moduleForm As New ManageCostOfGoodSold(Me)
        moduleForm.btnUpdate.Enabled = False
        moduleForm.ShowDialog()
    End Sub

    Private Sub txtSearchCOG_TextChanged(sender As Object, e As EventArgs) Handles txtSearchCOG.TextChanged
        loadCostOfGoodSold()
    End Sub


    Private Sub dgvCostOfGoodSold_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvCostOfGoodSold.CellContentClick

        If e.RowIndex >= 0 Then
            Dim colName As String = dgvCostOfGoodSold.Columns(e.ColumnIndex).Name

            If colName = "EditCog" Then

                Dim moduleForm As New ManageCostOfGoodSold(Me)
                moduleForm.lblCid.Text = dgvCostOfGoodSold.Rows(e.RowIndex).Cells(1).Value.ToString()
                moduleForm.txtCostName.Text = dgvCostOfGoodSold.Rows(e.RowIndex).Cells(2).Value.ToString()
                moduleForm.txtCost.Text = dgvCostOfGoodSold.Rows(e.RowIndex).Cells(3).Value.ToString()
                moduleForm.dtCOG.Text = dgvCostOfGoodSold.Rows(e.RowIndex).Cells(4).Value.ToString()

                moduleForm.btnSave.Enabled = False
                moduleForm.ShowDialog()

            ElseIf colName = "DeleteCoG" Then
                Try
                    If MessageBox.Show("Are you sure you want to delete this record?", "Delete Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                        Dim deleteId As String = dgvCostOfGoodSold.Rows(e.RowIndex).Cells(1).Value.ToString()


                        cm = New MySqlCommand("DELETE FROM tb_costofgood WHERE id = @id")
                        cm.Parameters.AddWithValue("@id", deleteId)

                        cm.Connection = GetConnection()



                        cm.ExecuteNonQuery()

                        GetConnection.Close()
                        MessageBox.Show("Cost of good sold data has been successfully removed!", title, MessageBoxButtons.OK, MessageBoxIcon.Information)


                        loadCostOfGoodSold()
                    End If
                Catch ex As Exception
                    MessageBox.Show("Error deleting record: " & ex.Message, title)
                End Try
            End If
        End If
    End Sub

    Public Sub loadCostOfGoodSold()
        Try
            Dim i As Integer = 0
            dgvCostOfGoodSold.Rows.Clear()


            Dim query As String = "SELECT * FROM tb_costofgood WHERE CONCAT(cost_name,cost,date) LIKE @search"

            Using conn As MySqlConnection = GetConnection()
                Using cm As New MySqlCommand(query, conn)
                    cm.Parameters.AddWithValue("@search", "%" & txtSearchCOG.Text & "%")

                    Using dr As MySqlDataReader = cm.ExecuteReader()
                        While dr.Read()
                            i += 1
                            dgvCostOfGoodSold.Rows.Add(i, dr(0).ToString(), dr(1).ToString(), dr(2).ToString(), DateTime.Parse(dr(3).ToString()).ToShortDateString())

                        End While
                    End Using
                End Using
            End Using

        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, title, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub TxtSearchVT_TextChanged_1(sender As Object, e As EventArgs)
        loadCostOfGoodSold()
    End Sub

    'company details
    Public Sub LoadCompany()
        Try
            'GetConnection.Open()
            cm = New MySqlCommand("SELECT * FROM tb_company", GetConnection)
            dr = cm.ExecuteReader()

            If dr.Read() Then
                If dr.HasRows Then
                    hasdetail = True
                    txtCName.Text = dr("name").ToString()
                    txtCAddress.Text = dr("address").ToString()
                End If
            Else
                txtCName.Clear()
                txtCAddress.Clear()
            End If

            'dr.Close()
            'GetConnection.Close()
        Catch ex As Exception
            MessageBox.Show(ex.Message, title)
        End Try
    End Sub


    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Try
            ' Validate inputs
            If String.IsNullOrWhiteSpace(txtCName.Text) Then
                MessageBox.Show("Please enter the Company Name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                txtCName.Focus()
                Exit Sub
            End If

            If String.IsNullOrWhiteSpace(txtCAddress.Text) Then
                MessageBox.Show("Please enter the Company Address.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                txtCAddress.Focus()
                Exit Sub
            End If


            Dim connectionString As String = "server=localhost;userid=root;password=;database=carwash_management_system"
            Using conn As New MySqlConnection(connectionString)
                conn.Open()

                Dim query As String


                If MessageBox.Show("Save company detail?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                    If hasdetail Then
                        query = "UPDATE tb_company SET name=@name, address=@address"
                    Else
                        query = "INSERT INTO tb_company (name, address) VALUES (@name, @address)"
                    End If

                    Using cmd As New MySqlCommand(query, conn)
                        cmd.Parameters.AddWithValue("@name", txtCName.Text)
                        cmd.Parameters.AddWithValue("@address", txtCAddress.Text)

                        cmd.ExecuteNonQuery()
                    End Using

                    MessageBox.Show("Company detail has been successfully saved!", "Save Record", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If

                conn.Close()
            End Using

        Catch ex As Exception
            MessageBox.Show(ex.Message, "Error")
        End Try
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        txtCName.Clear()
        txtCAddress.Clear()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        ManageVehicleType.ShowDialog()
    End Sub
End Class