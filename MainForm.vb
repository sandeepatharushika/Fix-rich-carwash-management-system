Imports System.ComponentModel
Imports System.Data.SqlClient
Imports MySql.Data.MySqlClient

Public Class MainForm


    Private currentForm As Form = Nothing



    Private Sub OpenChildForm(childForm As Form)

        If currentForm IsNot Nothing Then
            currentForm.Close()
            currentForm.Dispose()
        End If

        currentForm = childForm
        childForm.TopLevel = False
        childForm.FormBorderStyle = FormBorderStyle.None
        childForm.Dock = DockStyle.Fill
        childForm.Padding = New Padding(0)

        panel_Child.Controls.Clear()
        panel_Child.Controls.Add(childForm)
        childForm.BringToFront()
        childForm.Show()
    End Sub

    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        LoadGrossProfit()
    End Sub

    Private Sub btnDashboard_Click(sender As Object, e As EventArgs) Handles btnDashboard.Click
        Panel_Slide.Height = btnDashboard.Height
        Panel_Slide.Top = btnDashboard.Top

    End Sub

    Private Sub btnEmployer_Click(sender As Object, e As EventArgs) Handles btnEmployer.Click
        Panel_Slide.Height = btnEmployer.Height
        Panel_Slide.Top = btnEmployer.Top
        OpenChildForm(New Employer()) ' Show inside panel_Child
    End Sub

    Private Sub btnCustomer_Click(sender As Object, e As EventArgs) Handles btnCustomer.Click
        Panel_Slide.Height = btnCustomer.Height
        Panel_Slide.Top = btnCustomer.Top
        OpenChildForm(New Customer()) ' Show inside panel_Child
    End Sub

    Private Sub btnService_Click(sender As Object, e As EventArgs) Handles btnService.Click
        Panel_Slide.Height = btnService.Height
        Panel_Slide.Top = btnService.Top
        OpenChildForm(New Service()) ' Show inside panel_Child
    End Sub

    Private Sub btnReport_Click(sender As Object, e As EventArgs) Handles btnReport.Click
        Panel_Slide.Height = btnReport.Height
        Panel_Slide.Top = btnReport.Top
        OpenChildForm(New Report()) ' Show inside panel_Child
    End Sub

    Private Sub btnSetting_Click(sender As Object, e As EventArgs) Handles btnSetting.Click
        Panel_Slide.Height = btnSetting.Height
        Panel_Slide.Top = btnSetting.Top
        OpenChildForm(New setting())
    End Sub

    Private Sub btnCash_Click(sender As Object, e As EventArgs) Handles btnCash.Click
        Panel_Slide.Height = btnCash.Height
        Panel_Slide.Top = btnCash.Top
        OpenChildForm(New Cash())
    End Sub

    Private Sub btnLogout_Click(sender As Object, e As EventArgs) Handles btnLogout.Click
        Panel_Slide.Height = btnLogout.Height
        Panel_Slide.Top = btnLogout.Top


        Dim result = MessageBox.Show("Are you sure you want to logout?", "Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If result = DialogResult.Yes Then
            Me.Close()
        End If
    End Sub
    Private connectionString As String = "server=localhost;user id=root;password=;database=carwash_management_system"
    Public Sub LoadGrossProfit()
        Try

            Dim fromDate As String = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd")
            Dim toDate As String = DateTime.Now.ToString("yyyy-MM-dd")


            Dim revenueQuery As String = "SELECT IFNULL(SUM(se.price), 0) AS total " &
                                       "FROM tb_cash ca " &
                                       "JOIN tb_service se ON ca.sid = se.id " &
                                       "WHERE DATE(ca.date) BETWEEN @fromDate AND @toDate " &
                                       "AND ca.status = 'pending'"


            Dim costQuery As String = "SELECT IFNULL(SUM(cost), 0) AS total " &
                                    "FROM tb_costofgood " &
                                    "WHERE DATE(date) BETWEEN @fromDate AND @toDate"


            Dim revenue As Double = ExtractData(revenueQuery, fromDate, toDate)
            Dim cost As Double = ExtractData(costQuery, fromDate, toDate)
            Dim grossProfit As Double = revenue - cost


            If Me.InvokeRequired Then
                Me.Invoke(New Action(Sub()
                                         UpdateLabels(revenue, cost, grossProfit)
                                     End Sub))
            Else
                UpdateLabels(revenue, cost, grossProfit)
            End If

        Catch ex As Exception
            MessageBox.Show("Error loading gross profit: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    Private Sub UpdateLabels(revenue As Double, cost As Double, grossProfit As Double)
        Try
            lblRevenus.Text = revenue.ToString("#,##0.00")
            lblCostSold.Text = cost.ToString("#,##0.00")
            lblGP.Text = grossProfit.ToString("#,##0.00")
        Catch ex As Exception

            MessageBox.Show("Error updating labels: " & ex.Message & vbCrLf & "Please ensure lblRevenus, lblCostSold, and lblGP labels exist on your form.", "Label Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End Try
    End Sub


    Private Function ExtractData(query As String, fromDate As String, toDate As String) As Double
        Dim result As Double = 0
        Try
            Using con As New MySqlConnection(connectionString)
                con.Open()
                Using cmd As New MySqlCommand(query, con)
                    cmd.Parameters.AddWithValue("@fromDate", fromDate)
                    cmd.Parameters.AddWithValue("@toDate", toDate)
                    Dim value = cmd.ExecuteScalar()
                    If value IsNot Nothing AndAlso value IsNot DBNull.Value Then
                        result = Convert.ToDouble(value)
                    End If
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error extracting data: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
        Return result
    End Function

    ' Timer for real-time updates - Add this to your form
    Private WithEvents updateTimer As New Timer()


    Private Sub InitializeRealTimeUpdates()
        updateTimer.Interval = 30000
        updateTimer.Enabled = True
        AddHandler updateTimer.Tick, AddressOf UpdateTimer_Tick
    End Sub


    Private Sub UpdateTimer_Tick(sender As Object, e As EventArgs)

        Dim bgWorker As New System.ComponentModel.BackgroundWorker()
        AddHandler bgWorker.DoWork, AddressOf BgWorker_DoWork
        bgWorker.RunWorkerAsync()
    End Sub


    Private Sub BgWorker_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs)
        LoadGrossProfit()
    End Sub


    Public Sub RefreshData()
        LoadGrossProfit()
    End Sub

    ' Call this method in your Form_Load event
    Private Sub Form_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Initial load
        LoadGrossProfit()
        ' Start real-time updates
        InitializeRealTimeUpdates()
    End Sub

    Private Sub Form_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If updateTimer IsNot Nothing Then
            updateTimer.Enabled = False
            updateTimer.Dispose()
        End If
    End Sub

    Private Sub panel_Child_Paint(sender As Object, e As PaintEventArgs) Handles panel_Child.Paint

    End Sub
End Class


