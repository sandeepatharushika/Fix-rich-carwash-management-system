Imports MySql.Data.MySqlClient


Public Class Calculater


    Dim cm As New MySqlCommand()
    Dim dbcon As New dbConnect()
    Dim dr As MySqlDataReader
    Dim title As String = "Car Wash Management System"
    Dim cash As Cash

    Public Sub New(cashform As Cash)
        InitializeComponent()
        cash = cashform
    End Sub


    Private Sub picClose_Click(sender As Object, e As EventArgs) Handles picClose.Click
        Me.Dispose()
    End Sub

    Private Sub btn0_Click(sender As Object, e As EventArgs) Handles btn0.Click
        txtCash.Text &= btn0.Text

    End Sub

    Private Sub btnPoint_Click(sender As Object, e As EventArgs) Handles btnPoint.Click
        txtCash.Text &= btnPoint.Text

    End Sub

    Private Sub btn1_Click(sender As Object, e As EventArgs) Handles btn1.Click
        txtCash.Text &= btn1.Text

    End Sub

    Private Sub btn2_Click(sender As Object, e As EventArgs) Handles btn2.Click
        txtCash.Text &= btn2.Text

    End Sub

    Private Sub btn3_Click(sender As Object, e As EventArgs) Handles btn3.Click
        txtCash.Text &= btn3.Text

    End Sub

    Private Sub btn4_Click(sender As Object, e As EventArgs) Handles btn4.Click
        txtCash.Text &= btn4.Text

    End Sub

    Private Sub btn5_Click(sender As Object, e As EventArgs) Handles btn5.Click
        txtCash.Text &= btn5.Text

    End Sub

    Private Sub btn6_Click(sender As Object, e As EventArgs) Handles btn6.Click
        txtCash.Text &= btn6.Text

    End Sub

    Private Sub btn7_Click(sender As Object, e As EventArgs) Handles btn7.Click
        txtCash.Text &= btn7.Text

    End Sub

    Private Sub btn8_Click(sender As Object, e As EventArgs) Handles btn8.Click
        txtCash.Text &= btn8.Text

    End Sub

    Private Sub btn9_Click(sender As Object, e As EventArgs) Handles btn9.Click
        txtCash.Text &= btn9.Text

    End Sub

    Private Sub btnClean_Click(sender As Object, e As EventArgs) Handles btnClean.Click
        txtCash.Clear()
        txtCash.Focus()
    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles btn00.Click
        txtCash.Text &= btn00.Text

    End Sub
    Private Sub txtCash_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtCash.KeyPress
        ' Allow control keys (like Backspace)
        If Char.IsControl(e.KeyChar) Then
            Return
        End If

        ' Allow only one decimal point
        If e.KeyChar = "."c AndAlso txtCash.Text.Contains(".") Then
            e.Handled = True
            Return
        End If

        ' Allow only digits and one decimal point
        If Not Char.IsDigit(e.KeyChar) AndAlso e.KeyChar <> "."c Then
            e.Handled = True ' Block invalid character
        End If
    End Sub


    Private Sub btnEnter_Click(sender As Object, e As EventArgs) Handles btnEnter.Click
        Try
            Dim totalAmount As Double
            Dim cashPaid As Double
            Dim changeAmount As Double

            ' Validate and parse
            If Not Double.TryParse(txtSale.Text, totalAmount) Then
                MessageBox.Show("Invalid total amount.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            If Not Double.TryParse(txtCash.Text, cashPaid) Then
                MessageBox.Show("Invalid cash amount.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            ' Calculate change
            changeAmount = cashPaid - totalAmount
            txtChange.Text = changeAmount.ToString("N2")

            If changeAmount < 0 Then
                MessageBox.Show("Insufficient amount, Please enter the correct amount!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If


            For i As Integer = 0 To cash.dgvCash.Rows.Count - 1
                Dim id As String = cash.dgvCash.Rows(i).Cells(1).Value.ToString()
                dbcon.executeQuery("UPDATE tb_cash SET status='pending' WHERE id='" & id & "'")
            Next


            dbcon.executeQuery("UPDATE tb_customer SET points = points + 1 WHERE id='" & cash.customerId & "'")


            ' Dim module1 As New receipt(cash)
            'module1.LoadReceipt(txtCash.Text, txtChange.Text)
            'module1.ShowDialog()

            MessageBox.Show("Payment successfully saved!", "Payment", MessageBoxButtons.OK, MessageBoxIcon.Information)
            cash.loadCash()
            Me.Dispose()
            cash.btnAddCustomer.Enabled = True
            cash.btnAddService.Enabled = False
            cash.getTransno()

        Catch ex As Exception
            MessageBox.Show(ex.Message, title)
        End Try

        cash.loadCash()

        ' ✅ Disable the cash button after successful payment
        cash.btnCash.Enabled = False

        Me.Dispose()
        cash.btnAddCustomer.Enabled = True
        cash.btnAddService.Enabled = False
        cash.getTransno()
    End Sub



    Private Sub txtCash_TextChanged(sender As Object, e As EventArgs) Handles txtCash.TextChanged
        Try
            Dim charge As Double = Double.Parse(txtSale.Text) - Double.Parse(txtCash.Text)
            txtChange.Text = charge.ToString("#,##0.00")
        Catch ex As Exception
            txtChange.Text = "0.00"
        End Try
    End Sub

    Private Sub FlowLayoutPanel1_Paint(sender As Object, e As PaintEventArgs) Handles FlowLayoutPanel1.Paint

    End Sub

    Private Sub txtChange_TextChanged(sender As Object, e As EventArgs) Handles txtChange.TextChanged

    End Sub

    Private Sub Calculater_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub Calculater_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.Enter Then
            btnEnter.PerformClick()
        ElseIf e.KeyCode = Keys.Escape Then
            Me.Dispose()
        End If
    End Sub
End Class