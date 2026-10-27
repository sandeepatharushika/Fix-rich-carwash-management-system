Imports System.Data.SqlClient
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox
Imports MySql.Data.MySqlClient



Public Class Cash

    Dim cm As MySqlCommand
    Dim dr As MySqlDataReader
    Dim title As String = "Car Wash Management System"
    Public customerId As Integer = 0
    Public vehicleId As Integer = 0

    Public carmodel As String
    Public carno As String

    Private Sub Cash_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        getTransno()
        loadCash()
    End Sub

    Private Sub btnAddCustomer_Click(sender As Object, e As EventArgs) Handles btnAddCustomer.Click
        OpenChildForm(New CashCustomer(Me))
        btnAddService.Enabled = True
    End Sub

    Private Sub btnAddService_Click(sender As Object, e As EventArgs) Handles btnAddService.Click
        OpenChildForm(New cashService(Me))
        btnAddCustomer.Enabled = False
    End Sub

    Private Sub btnCash_Click(sender As Object, e As EventArgs) Handles btnCash.Click
        Dim settleForm As New Calculater(Me)
        settleForm.txtSale.Text = lblTotal.Text
        settleForm.ShowDialog()
    End Sub


    Public Sub loadCash()
        Try
            Dim i As Integer = 0
            Dim total As Double = 0
            dgvCash.Rows.Clear()

            Console.WriteLine($"Loading cash for transaction: {lbl1.Text}")

            Using con As MySqlConnection = DBConnection.GetConnection()


                Using cm As New MySqlCommand(
            "SELECT ca.id, ca.trans_no AS transno, cu.name AS customer_name, cu.car_no AS car_no, cu.car_model AS car_model, " &
            "v.name AS vehicle_name, v.class, s.name AS service_name, ca.price, ca.date " &
            "FROM tb_cash AS ca " &
            "LEFT JOIN tb_customer AS cu ON ca.cid = cu.id " &
            "LEFT JOIN tb_service AS s ON ca.sid = s.id " &
            "LEFT JOIN tb_vehicle AS v ON ca.vid = v.id " &
            "WHERE ca.status = @status AND ca.trans_no = @trans_no", con)

                    cm.Parameters.AddWithValue("@status", "pending")
                    cm.Parameters.AddWithValue("@trans_no", lbl1.Text)

                    Using dr As MySqlDataReader = cm.ExecuteReader()
                        While dr.Read()
                            i += 1
                            Dim price As Double = 0

                            If Not IsDBNull(dr("price")) Then
                                Double.TryParse(dr("price").ToString(), price)
                            End If

                            dgvCash.Rows.Add(i,
                          If(IsDBNull(dr("id")), "", dr("id").ToString()),
                          If(IsDBNull(dr("transno")), "", dr("transno").ToString()),
                          If(IsDBNull(dr("customer_name")), "", dr("customer_name").ToString()),
                          If(IsDBNull(dr("car_no")), "", dr("car_no").ToString()),
                          If(IsDBNull(dr("car_model")), "", dr("car_model").ToString()),
                          If(IsDBNull(dr("vehicle_name")), "", dr("vehicle_name").ToString()),
                          If(IsDBNull(dr("class")), "", dr("class").ToString()),
                          If(IsDBNull(dr("service_name")), "", dr("service_name").ToString()),
                          price.ToString("N2"),
                          If(IsDBNull(dr("date")), "", dr("date").ToString()))

                            total += price
                            carno = dr(3).ToString()
                            carmodel = dr(4).ToString()

                        End While

                        lblTotal.Text = total.ToString("N2")

                        Console.WriteLine($"Found {i} pending records for transaction {lbl1.Text}")
                        ' If i = 0 Then
                        'ageBox.Show($"No pending records found for transaction: {lbl1.Text}",
                        ' "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        ' End If
                    End Using
                End Using
            End Using

        Catch ex As Exception
            MessageBox.Show("Error in loadCash: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub





    Public Sub getTransno()
        Try
            Dim sdate As String = DateTime.Now.ToString("yyyyMMdd")
            Dim count As Integer = 0
            Dim transno As String = ""

            Dim con As New MySqlConnection("server=localhost;user id=root;password=;database=carwash_management_system")
            con.Open()

            Dim query As String = "SELECT trans_no FROM tb_cash WHERE trans_no LIKE @sdate ORDER BY trans_no DESC LIMIT 1"
            Dim cmd As New MySqlCommand(query, con)
            cmd.Parameters.AddWithValue("@sdate", sdate & "%")

            dr = cmd.ExecuteReader()

            If dr.Read() Then
                transno = dr("trans_no").ToString()
                count = Integer.Parse(transno.Substring(8, 4))
                count += 1
            Else
                count = 1001
            End If

            dr.Close()
            con.Close()

            lbl1.Text = sdate & count.ToString("D4")

        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, title)
        End Try
    End Sub

    Private currentChildForm As Form = Nothing

    Public Sub OpenChildForm(childForm As Form)
        If currentChildForm IsNot Nothing Then
            currentChildForm.Close()
        End If

        currentChildForm = childForm
        childForm.TopLevel = False
        childForm.FormBorderStyle = FormBorderStyle.None
        childForm.Dock = DockStyle.Fill
        panelCash.Height = 200
        panelCash.Controls.Add(childForm)
        panelCash.Tag = childForm

        childForm.BringToFront()
        childForm.Show()
    End Sub


    Private Sub dgvCash_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvCash.CellContentClick
        Dim colName As String = dgvCash.Columns(e.ColumnIndex).Name

        If colName = "Delete" Then
            Try
                If MessageBox.Show("Are you sure you want to cancel this service?", "Cancel Services", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then


                    Using con As MySqlConnection = DBConnection.GetConnection()
                        'con.Open()

                        ' Prepare the DELETE command using MySQL
                        Dim query As String = "DELETE FROM tb_cash WHERE id = @id"
                        Using cm As New MySqlCommand(query, con)
                            cm.Parameters.AddWithValue("@id", dgvCash.Rows(e.RowIndex).Cells(1).Value.ToString())
                            cm.ExecuteNonQuery()
                        End Using
                    End Using

                    MessageBox.Show("Service has been successfully Canceled!", title, MessageBoxButtons.OK, MessageBoxIcon.Information)


                    loadCash()
                End If

            Catch ex As Exception
                MessageBox.Show(ex.Message, title)
            End Try
        End If
    End Sub




End Class
