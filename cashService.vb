Imports System.Data.SqlClient
Imports MySql.Data.MySqlClient

Public Class cashService
    Dim cm As New SqlCommand()
    Dim dbcon As New dbConnect()
    Dim dr As SqlDataReader
    Dim title As String = "Car Wash Management System"
    Dim cash As Cash
    Dim chk As New DataGridViewCheckBoxColumn()



    Private cashForm As Cash


    Public Sub New(ByVal cash As Cash)
        InitializeComponent()
        cashForm = cash
    End Sub


    Private Sub Form_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        loadService()
    End Sub

    Private Sub txtSearch_TextChanged(sender As Object, e As EventArgs) Handles txtSearch.TextChanged
        loadService()
    End Sub

    Private Sub dgvService_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvService.CellContentClick

    End Sub
    Private Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click
        Try
            For Each dr As DataGridViewRow In dgvService.Rows

                If dr.IsNewRow Then Continue For


                Dim chkbox As Boolean = Convert.ToBoolean(dr.Cells("Column5").Value)
                If chkbox Then

                    Using con As MySqlConnection = DBConnection.GetConnection()
                        ' con.Open() ' Make sure connection is opened

                        ' ✅ FIXED: Added status = 'pending' to the INSERT statement
                        Using cm As New MySqlCommand("INSERT INTO tb_cash (trans_no, cid, sid, vid, price, date, status) " &
                                     "SELECT @trans_no, @cid, @sid, @vid, @price, @date, @status " &
                                     "FROM DUAL " &
                                     "WHERE NOT EXISTS (SELECT * FROM tb_cash WHERE sid = @sid AND trans_no = @trans_no)", con)

                            cm.Parameters.AddWithValue("@trans_no", cashForm.lbl1.Text)
                            cm.Parameters.AddWithValue("@cid", cashForm.customerId)
                            cm.Parameters.AddWithValue("@sid", dr.Cells(1).Value.ToString())
                            cm.Parameters.AddWithValue("@vid", cashForm.vehicleId)
                            cm.Parameters.AddWithValue("@price", dr.Cells(3).Value.ToString())
                            cm.Parameters.AddWithValue("@date", DateTime.Now)
                            cm.Parameters.AddWithValue("@status", "pending") 'This was missing!

                            Dim rowsAffected As Integer = cm.ExecuteNonQuery()

                            If rowsAffected > 0 Then
                                Console.WriteLine($"Inserted service ID: {dr.Cells(1).Value} for transaction: {cashForm.lbl1.Text}")
                            End If
                        End Using
                    End Using
                    cashForm.btnCash.Enabled = True
                End If
            Next


            cashForm.loadCash()


            cashForm.panelCash.Height = 1

            Me.Dispose()

        Catch ex As Exception
            MessageBox.Show("Error in btnSubmit_Click: " & ex.Message, title, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    Public Sub loadService()
        Try
            Dim i As Integer = 0
            dgvService.Rows.Clear()


            Dim cm As New MySqlCommand("SELECT * FROM tb_service WHERE name LIKE '%" & txtSearch.Text & "%'", DBConnection.GetConnection())
            'DBConnection.GetConnection().Open()

            Dim dr As MySqlDataReader = cm.ExecuteReader()

            While dr.Read()
                i += 1


                dgvService.Rows.Add(i, dr(0).ToString(), dr(1).ToString(), dr(2).ToString())
            End While

            'DBConnection.GetConnection().Close()

        Catch ex As Exception
            MessageBox.Show(ex.Message, title)
        End Try
    End Sub


End Class