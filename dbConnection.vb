Imports MySql.Data.MySqlClient

Module DBConnection


    Public ReadOnly connString As String = "Server=localhost;Database=carwash_management_system;User Id=root;Password=;"


    Public Function GetConnection() As MySqlConnection
        Dim conn As New MySqlConnection(connString)
        Try
            conn.Open()
            'MessageBox.Show("Connectiion Successfull!")
        Catch ex As Exception
            MessageBox.Show("Database Connection Failed: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
        Return conn
    End Function

    Public Function TestConnection() As Boolean
        Using conn As MySqlConnection = GetConnection()
            Try
                Return True
            Catch ex As Exception
                Return False
            End Try
        End Using
    End Function

    Public Class dbConnect
        Public con As New MySqlConnection("server=localhost;user id=root;password=;database=carwash_management_system")
        Public cm As MySqlCommand

        Public Sub executeQuery(ByVal sql As String)
            Try
                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If

                cm = New MySqlCommand(sql, con)
                cm.ExecuteNonQuery()
                con.Close()

            Catch ex As Exception
                MessageBox.Show(ex.Message, "Car Wash Management System", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Finally
                If con.State = ConnectionState.Open Then
                    con.Close()
                End If
            End Try
        End Sub
    End Class


End Module