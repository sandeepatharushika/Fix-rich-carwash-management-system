Imports MySql.Data.MySqlClient

Public Class CashCustomer

    Dim cm As MySqlCommand
    Dim dr As MySqlDataReader
    Dim title As String = "Car_Wash_Management_System"
    Private cash As Cash



    Public Sub New(ByVal cashForm As Cash)
        InitializeComponent()
        cash = cashForm
        loadCustomer()
    End Sub

    Private Sub txtSearch_TextChanged(sender As Object, e As EventArgs) Handles txtSearch.TextChanged
        loadCustomer()
    End Sub

    Private Sub dgvCustomer_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvCustomer.CellContentClick
        Try
            Dim colName As String = dgvCustomer.Columns(e.ColumnIndex).Name

            If colName = "selectColumn" Then
                cash.customerId = Integer.Parse(dgvCustomer.Rows(e.RowIndex).Cells(1).Value.ToString())
                cash.vehicleId = Integer.Parse(dgvCustomer.Rows(e.RowIndex).Cells(2).Value.ToString())

                Me.Dispose()
                cash.panelCash.Height = 1
            Else
                Return
            End If

        Catch ex As Exception
            MessageBox.Show("An error occurred: " & ex.Message)
        End Try
    End Sub



    Public Sub loadCustomer()
        Try
            Dim i As Integer = 0
            dgvCustomer.Rows.Clear()

            Dim db As MySqlConnection = GetConnection()
            Dim cm As New MySqlCommand("SELECT * FROM tb_customer WHERE CONCAT(name, phone, address) LIKE '%" & txtSearch.Text & "%'", db)
            'db.Open()

            dr = cm.ExecuteReader()
            While dr.Read()
                i += 1
                dgvCustomer.Rows.Add(i, dr(0).ToString(), dr(1).ToString(), dr(2).ToString(), dr(3).ToString(), dr(4).ToString(), dr(5).ToString(), dr(6).ToString(), dr(7).ToString())
            End While
            dr.Close()
            'db.Close()
        Catch ex As Exception
            MessageBox.Show(ex.Message, title)
        End Try
    End Sub

End Class


