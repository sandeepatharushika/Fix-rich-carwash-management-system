
Imports MySql.Data.MySqlClient

Public Class Report
    Dim cm As MySqlCommand
    Dim dr As MySqlDataReader
    Dim title As String = "Car Wash Management System"

    Public Sub New()
        InitializeComponent()
        LoadTopSelling()
        LoadRevenus()
        LoadCostofGood()
        LoadGrossProfit()
    End Sub


    Private Sub dtFromTopSelling_ValueChanged(sender As Object, e As EventArgs) Handles dtFromTopSelling.ValueChanged
        LoadTopSelling()
    End Sub

    Private Sub dtToTopSelling_ValueChanged(sender As Object, e As EventArgs) Handles dtToTopSelling.ValueChanged
        LoadTopSelling()
    End Sub

    Private Sub dgvTopSelling_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvTopSelling.CellContentClick

    End Sub

    Public Sub LoadTopSelling()
        Try
            Dim dbcon As New MySqlConnection("server=localhost;user id=root;password=;database=carwash_management_system;")
            Dim cm As New MySqlCommand()
            Dim dr As MySqlDataReader
            Dim i As Integer = 0

            dgvTopSelling.Rows.Clear()


            Dim query As String = "
            SELECT 
                se.name, 
                COUNT(ca.sid) AS qty,
                se.price AS unit_price,
                (COUNT(ca.sid) * se.price) AS total
            FROM tb_service AS se
            JOIN tb_cash AS ca ON se.id = ca.sid
            WHERE ca.date BETWEEN @fromDate AND @toDate
            AND ca.status = 'pending'
            GROUP BY se.id, se.name, se.price
            ORDER BY qty DESC
            LIMIT 10
        "

            cm = New MySqlCommand(query, dbcon)
            cm.Parameters.AddWithValue("@fromDate", dtFromTopSelling.Value.Date)
            cm.Parameters.AddWithValue("@toDate", dtToTopSelling.Value.Date)

            dbcon.Open()
            dr = cm.ExecuteReader()

            While dr.Read()
                i += 1
                Dim name As String = dr("name").ToString()
                Dim qty As Integer = Convert.ToInt32(dr("qty"))
                Dim unitPrice As Decimal = Convert.ToDecimal(dr("unit_price"))
                Dim totalPrice As Decimal = Convert.ToDecimal(dr("total"))

                dgvTopSelling.Rows.Add(i, name, qty, unitPrice.ToString("#,##0.00"), totalPrice.ToString("#,##0.00"))
            End While

            dr.Close()
            dbcon.Close()

        Catch ex As Exception
            MessageBox.Show("Error loading top selling: " & ex.Message, "Error")
        End Try
    End Sub

    Private Sub dtFromRevenus_ValueChanged(sender As Object, e As EventArgs) Handles dtFromRevenus.ValueChanged
        LoadRevenus()
    End Sub

    Private Sub dtToRevenus_ValueChanged(sender As Object, e As EventArgs) Handles dtToRevenus.ValueChanged
        LoadRevenus()
    End Sub

    Public Sub LoadRevenus()
        Try

            Dim i As Integer = 0
            Dim total As Double = 0
            dgvRevenus.Rows.Clear()


            Dim fromDate As Date = dtFromTopSelling.Value.Date
            Dim toDate As Date = dtToTopSelling.Value.Date

            Dim query As String = "
            SELECT 
                DATE(ca.date) AS trans_date, 
                SUM(IFNULL(se.price, 0)) AS total
            FROM tb_cash ca
            INNER JOIN tb_service se ON ca.sid = se.id
            WHERE DATE(ca.date) BETWEEN @fromDate AND @toDate
            GROUP BY DATE(ca.date)
        "

            Dim revenueByDate As New Dictionary(Of String, Double)

            Using dbcon As New MySqlConnection("server=localhost;user id=root;password=;database=carwash_management_system"),
              cm As New MySqlCommand(query, dbcon)

                cm.Parameters.AddWithValue("@fromDate", fromDate.ToString("yyyy-MM-dd"))
                cm.Parameters.AddWithValue("@toDate", toDate.ToString("yyyy-MM-dd"))

                dbcon.Open()
                Using dr As MySqlDataReader = cm.ExecuteReader()
                    While dr.Read()
                        If Not IsDBNull(dr("trans_date")) AndAlso Not IsDBNull(dr("total")) Then
                            Dim dateStr As String = Convert.ToDateTime(dr("trans_date")).ToString("yyyy-MM-dd")
                            Dim amount As Double = Convert.ToDouble(dr("total"))
                            revenueByDate(dateStr) = amount
                        End If
                    End While
                End Using
            End Using


            Dim currentDate As Date = fromDate
            While currentDate <= toDate
                i += 1
                Dim dateKey As String = currentDate.ToString("yyyy-MM-dd")
                Dim revenue As Double = If(revenueByDate.ContainsKey(dateKey), revenueByDate(dateKey), 0)

                dgvRevenus.Rows.Add(i, currentDate.ToString("dd/MM/yyyy"), revenue.ToString("#,##0.00"))
                total += revenue
                currentDate = currentDate.AddDays(1)
            End While

            ' Step 6: Show total
            lblRevenus.Text = total.ToString("#,##0.00")

        Catch ex As Exception
            MessageBox.Show("Error loading revenue: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub








    Private Sub dtFromCoG_ValueChanged(sender As Object, e As EventArgs) Handles dtFromCoG.ValueChanged
        LoadCostofGood()
    End Sub

    Private Sub dtToCoG_ValueChanged(sender As Object, e As EventArgs) Handles dtToCoG.ValueChanged
        LoadCostofGood()
    End Sub

    Public Sub LoadCostofGood()
        Try
            Dim i As Integer = 0
            Dim total As Double = 0
            dgvCostofGood.Rows.Clear()


            Dim query As String = "SELECT cost_name, cost, date FROM tb_costofgood " &
                              "WHERE date BETWEEN @fromDate AND @toDate"

            Using dbcon As New MySqlConnection("server=localhost;user id=root;password=;database=carwash_management_system;"),
              cm As New MySqlCommand(query, dbcon)


                cm.Parameters.AddWithValue("@fromDate", dtFromCoG.Value.Date)
                cm.Parameters.AddWithValue("@toDate", dtToCoG.Value.Date.AddDays(1).AddTicks(-1))

                dbcon.Open()
                Using dr As MySqlDataReader = cm.ExecuteReader()
                    While dr.Read()
                        i += 1
                        dgvCostofGood.Rows.Add(i,
                                           dr("cost_name").ToString(),
                                           Convert.ToDouble(dr("cost")).ToString("#,##0.00"),
                                           DateTime.Parse(dr("date").ToString()).ToShortDateString())
                        total += Convert.ToDouble(dr("cost"))
                    End While
                End Using
            End Using


            lblCOGS.Text = total.ToString("#,##0.00")

        Catch ex As Exception
            MessageBox.Show("Error loading cost of goods: " & ex.Message, "Error")
        End Try
    End Sub


    Private Sub dtFromGp_ValueChanged(sender As Object, e As EventArgs) Handles dtFromGp.ValueChanged
        LoadGrossProfit()
    End Sub

    Private Sub dtToGp_ValueChanged(sender As Object, e As EventArgs) Handles dtToGp.ValueChanged
        LoadGrossProfit()
    End Sub
    Public Sub LoadGrossProfit()
        Try

            Dim revenueQuery As String = "SELECT IFNULL(SUM(price), 0) AS total FROM tb_cash " &
                                     "WHERE DATE(date) BETWEEN @fromDate AND @toDate"

            Dim costQuery As String = "SELECT IFNULL(SUM(cost), 0) AS total FROM tb_costofgood " &
                                  "WHERE DATE(date) BETWEEN @fromDate AND @toDate"

            Dim fromDate As String = dtFromGp.Value.Date.ToString("yyyy-MM-dd")
            Dim toDate As String = dtToGp.Value.Date.ToString("yyyy-MM-dd")

            Dim revenue As Double = ExtractData(revenueQuery, fromDate, toDate)
            Dim cost As Double = ExtractData(costQuery, fromDate, toDate)


            txtRevenus.Text = revenue.ToString("#,##0.00")
            txtCoG.Text = cost.ToString("#,##0.00")
            txtGrossProfit.Text = (revenue - cost).ToString("#,##0.00")

        Catch ex As Exception
            MessageBox.Show("Error loading gross profit: " & ex.Message, "Error")
        End Try
    End Sub


    Public Function ExtractData(query As String, fromDate As Date, toDate As Date) As Double
        Dim result As Double = 0

        Try
            Using dbcon As New MySqlConnection("server=localhost;user id=root;password=;database=carwash_management_system;"),
              cm As New MySqlCommand(query, dbcon)

                cm.Parameters.AddWithValue("@fromDate", fromDate)
                cm.Parameters.AddWithValue("@toDate", toDate)

                dbcon.Open()
                Dim obj = cm.ExecuteScalar()
                If obj IsNot Nothing AndAlso Not IsDBNull(obj) Then
                    result = Convert.ToDouble(obj)
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show("Error extracting data: " & ex.Message, "Error")
        End Try

        Return result
    End Function

    Private Sub dgvRevenus_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvRevenus.CellContentClick

    End Sub
End Class