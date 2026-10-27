'Imports Microsoft.Reporting.WinForms
'Imports MySql.Data.MySqlClient
'Imports Org.BouncyCastle.Asn1.CryptoPro
'Imports System.IO

'Public Class receipt

'    Dim title As String = "Car Wash Management System"
'    Dim company As String
'    Dim address As String

'    ' ✅ Hold reference to the Cash form
'    Private cashForm As Cash

'    ' ✅ Constructor to accept the Cash form
'    Public Sub New(cashform As Cash)
'        ' InitializeComponent()
'        'Me.cashForm = cashform
'        loadCompany()
'    End Sub

'    Private Sub receipt_Load(sender As Object, e As EventArgs) Handles MyBase.Load
'        Me.ReportViewer1.RefreshReport()
'    End Sub

'    ' ✅ Load company data from the database
'    Public Sub loadCompany()
'        Try
'            Using con As MySqlConnection = DBConnection.GetConnection()

'                Dim query As String = "SELECT * FROM tb_company"
'                Using cm As New MySqlCommand(query, con)
'                    Using dr As MySqlDataReader = cm.ExecuteReader()
'                        If dr.Read() Then
'                            company = dr("name").ToString()
'                            address = dr("address").ToString()
'                        End If
'                    End Using
'                End Using

'            End Using
'        Catch ex As Exception
'            MessageBox.Show("Error loading company data: " & ex.Message, title, MessageBoxButtons.OK, MessageBoxIcon.Error)
'        End Try
'    End Sub

'    Private Sub ReportViewer1_Load(sender As Object, e As EventArgs) Handles ReportViewer1.Load
'        ' Optional: You can use this if you want to set up something during ReportViewer load
'    End Sub

'    ' ✅ Load receipt data and display in ReportViewer
'    Public Sub LoadReceipt(pcash As String, pchange As String)
'        Try
'            ' Check if transaction number is available
'            If String.IsNullOrWhiteSpace(cashForm.lbl1.Text) Then
'                MessageBox.Show("Transaction number is missing!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
'                Return
'            End If

'            ' Check for the RDLC file
'            Dim reportPath As String = Path.Combine(Application.StartupPath, "Report\rptReceipt.rdlc")
'            If Not File.Exists(reportPath) Then
'                Throw New FileNotFoundException("RDLC file not found: " & reportPath)
'            End If

'            ' Setup ReportViewer
'            Me.ReportViewer1.LocalReport.ReportPath = reportPath
'            Me.ReportViewer1.LocalReport.DataSources.Clear()

'            ' Load receipt data into dataset
'            Dim ds As New DataSet1
'            Dim transactionNo As String = cashForm.lbl1.Text.Trim()

'            Using con As MySqlConnection = DBConnection.GetConnection()

'                Dim query As String = "SELECT s.name, c.price FROM tb_cash AS c INNER JOIN tb_service AS s ON c.sid = s.id WHERE c.trans_no = @transno"
'                Using cmd As New MySqlCommand(query, con)
'                    cmd.Parameters.AddWithValue("@transno", transactionNo)
'                    Using da As New MySqlDataAdapter(cmd)
'                        da.Fill(ds.Tables("dtReceipt"))
'                    End Using
'                End Using

'            End Using

'            '"SELECT s.name, c.price FROM tb_cash AS c INNER JOIN tb_service AS s ON c.sid = s.id WHERE c.trans_no = @transno"

'            ' Prepare report parameters
'            Dim totalAmount As String = If(String.IsNullOrWhiteSpace(cashForm.lblTotal.Text), "0", cashForm.lblTotal.Text.Trim())

'            Dim rptDataSource As New ReportDataSource("DataSet1", ds.Tables("dtReceipt"))
'            Me.ReportViewer1.LocalReport.DataSources.Add(rptDataSource)

'            Dim parameters As ReportParameter() = {
'                New ReportParameter("pCompany", company),
'                New ReportParameter("pAddress", address),
'                New ReportParameter("p1Change", pchange),
'                New ReportParameter("pCash", pcash),
'                New ReportParameter("pTotal", totalAmount),
'                New ReportParameter("pCarno", cashForm.carno),
'                New ReportParameter("pCarModel", cashForm.carmodel)
'            }



'            Me.ReportViewer1.LocalReport.DataSources.Clear()
'            Me.ReportViewer1.LocalReport.SetParameters(parameters)
'            Me.ReportViewer1.SetDisplayMode(DisplayMode.PrintLayout)
'            Me.ReportViewer1.ZoomMode = ZoomMode.FullPage
'            Me.ReportViewer1.RefreshReport()

'        Catch ex As Exception
'            MessageBox.Show("Load Receipt Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
'        End Try
'    End Sub

'End Class
