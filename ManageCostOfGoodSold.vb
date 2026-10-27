Imports MySql.Data.MySqlClient
Imports MySql.Data.MySqlClient.MySqlConnection
Imports Org.BouncyCastle.Asn1.Crmf
Imports System.Data
Imports System.Text.RegularExpressions
Public Class ManageCostOfGoodSold

    Private cm As New MySqlCommand
    Private dbcon As MySqlConnection = New MySqlConnection()
    Private title As String = "Car Wash Management System"
    Private setting As setting
    Private check As Boolean = False

    Public Sub New(sett As setting)
        InitializeComponent()
        setting = sett
    End Sub



    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Dispose()
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Try
            ' Validation
            If String.IsNullOrWhiteSpace(txtCostName.Text) Then
                MessageBox.Show("Please enter the Cost Name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                txtCostName.Focus()
                Exit Sub
            End If

            If String.IsNullOrWhiteSpace(txtCost.Text) Then
                MessageBox.Show("Please enter the Cost.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                txtCost.Focus()
                Exit Sub
            End If

            ' Check if Cost is a valid number
            Dim costValue As Decimal
            If Not Decimal.TryParse(txtCost.Text, costValue) Then
                MessageBox.Show("Please enter a valid number for the Cost.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                txtCost.Focus()
                Exit Sub
            End If

            ' If all validations pass, proceed to save
            If MessageBox.Show("Are you sure you want to register this cost of good sold?", "Cost of Good Sold Registration", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                cm = New MySqlCommand("INSERT INTO tb_CostofGood (cost_name, cost, date) VALUES (@cost_name, @cost, @date)", dbcon)
                cm.Parameters.AddWithValue("@cost_name", txtCostName.Text)
                cm.Parameters.AddWithValue("@cost", costValue)
                cm.Parameters.AddWithValue("@date", dtCOG.Value)
                cm.Connection = GetConnection()
                cm.ExecuteNonQuery()
                cm.Connection.Close()

                MessageBox.Show("Cost of good sold has been successfully registered!", title)

                Clear() ' Clear fields after saving
                setting.loadCostOfGoodSold()
            End If

        Catch ex As Exception
            MessageBox.Show(ex.Message, title)
        End Try
    End Sub

    Private Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click
        Try
            checkField()
            If check Then
                If MessageBox.Show("Are you sure you want to edit this cost of good sold?", "Cost of Good Sold Editing", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                    cm = New MySqlCommand("UPDATE tb_CostofGood SET cost_name=@cost_name, cost=@cost, date=@date WHERE id=@id", GetConnection())
                    cm.Parameters.AddWithValue("@id", lblCid.Text)
                    cm.Parameters.AddWithValue("@cost_name", txtCostName.Text)
                    cm.Parameters.AddWithValue("@cost", txtCost.Text)
                    cm.Parameters.AddWithValue("@date", dtCOG.Value)
                    cm.Connection = GetConnection() ' to open connection
                    cm.ExecuteNonQuery()
                    cm.Connection.Close() ' to close connection
                    MessageBox.Show("Cost of good sold has been successfully Edited!", title)
                    Clear() ' to clear data fields after data inserted into the database

                    For Each form As Form In Application.OpenForms
                        If TypeOf form Is setting Then
                            DirectCast(form, setting).loadCostOfGoodSold()
                            Exit For
                        End If
                    Next
                    Me.Dispose()

                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message, title)
        End Try
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Clear()
    End Sub

    Private Sub txtCost_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtCost.KeyPress
        ' Only allow digit numbers
        If Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsDigit(e.KeyChar) AndAlso (e.KeyChar <> "."c) Then
            e.Handled = True
        End If

        ' Only allow one decimal point
        If (e.KeyChar = "."c) AndAlso (CType(sender, TextBox).Text.IndexOf("."c) > -1) Then
            e.Handled = True
        End If
    End Sub

    Public Sub checkField()
        If txtCostName.Text = "" Or txtCost.Text = "" Then
            MessageBox.Show("Required data field!", "Warning")
            Return
        End If
        check = True
    End Sub

    Public Sub Clear()
        txtCost.Clear()
        txtCostName.Clear()
        dtCOG.Value = DateTime.Now
        btnSave.Enabled = True
        btnUpdate.Enabled = False
    End Sub

    Private Sub ManageCostOfGoodSold_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub dtCOG_ValueChanged(sender As Object, e As EventArgs) Handles dtCOG.ValueChanged

    End Sub

    Private Sub txtCostName_TextChanged(sender As Object, e As EventArgs) Handles txtCostName.TextChanged
        Dim input As String = txtCostName.Text


        If input.StartsWith(" ") Then
            MessageBox.Show("Name cannot start with a space.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            input = input.TrimStart()
        End If


        If Regex.IsMatch(input, "\d") Then
            MessageBox.Show("Name cannot contain numbers.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            input = Regex.Replace(input, "\d", "")
        End If


        If txtCostName.Text <> input Then
            txtCostName.Text = input
            txtCostName.SelectionStart = txtCostName.Text.Length
        End If
    End Sub

End Class