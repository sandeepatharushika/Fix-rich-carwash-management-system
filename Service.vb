Imports MySql.Data.MySqlClient

Public Class Service
    Dim cm As MySqlCommand
    Dim dr As MySqlDataReader
    Dim title As String = "Car Wash Management System"


    Public Sub New()
        InitializeComponent()

        loadService()
    End Sub

    Private Sub dgvService_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvService.CellContentClick

        If e.RowIndex >= 0 Then
            Dim colName As String = dgvService.Columns(e.ColumnIndex).Name

            If colName = "Edit" Then

                Dim moduleForm As New ServiceModule(Me)
                moduleForm.lblSid.Text = dgvService.Rows(e.RowIndex).Cells(1).Value.ToString()
                moduleForm.txtFullName.Text = dgvService.Rows(e.RowIndex).Cells(2).Value.ToString()
                moduleForm.txtPrice.Text = dgvService.Rows(e.RowIndex).Cells(3).Value.ToString()

                moduleForm.btnSave.Enabled = False
                moduleForm.ShowDialog()

            ElseIf colName = "Delete" Then
                Try
                    If MessageBox.Show("Are you sure you want to delete this record?", "Delete Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                        Dim deleteId As String = dgvService.Rows(e.RowIndex).Cells(1).Value.ToString()


                        cm = New MySqlCommand("DELETE FROM tb_service WHERE id = @id")
                        cm.Parameters.AddWithValue("@id", deleteId)

                        cm.Connection = GetConnection()

                        cm.ExecuteNonQuery()

                        GetConnection.Close()
                        MessageBox.Show("Service  has been successfully deleted!", title, MessageBoxButtons.OK, MessageBoxIcon.Information)


                        loadService()
                    End If
                Catch ex As Exception
                    MessageBox.Show("Error deleting record: " & ex.Message, title)
                End Try
            End If
        End If
    End Sub


    Public Sub loadService()
        Try
            Dim i As Integer = 0
            dgvService.Rows.Clear()
            cm = New MySqlCommand("SELECT * FROM tb_service WHERE name LIKE '%" & TxtSearch.Text & "%'", GetConnection)
            dr = cm.ExecuteReader()
            While dr.Read()
                i = i + 1

                dgvService.Rows.Add(i, dr(0).ToString(), dr(1).ToString(), dr(2).ToString())
            End While
            GetConnection.Close()
        Catch ex As Exception
            MessageBox.Show(ex.Message, title)
        End Try
    End Sub

    Private Sub TxtSearch_TextChanged(sender As Object, e As EventArgs) Handles TxtSearch.TextChanged
        loadService()
    End Sub

    Private Sub btnAddService_Click(sender As Object, e As EventArgs) Handles btnAddService.Click
        Dim serviceModule As New ServiceModule(Me)
        serviceModule.btnUpdate.Enabled = True
        serviceModule.ShowDialog()
    End Sub

End Class