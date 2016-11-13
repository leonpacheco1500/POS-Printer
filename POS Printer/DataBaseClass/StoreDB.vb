﻿Imports MySql.Data.MySqlClient
Public Class StoreDB
    Public Shared Function GetStoreName(StoreId As Integer) As String
        Dim Connection As MySqlConnection = MySqlDataBase.GetConnection
        Dim Terminal As String
        Dim Sql As String = "SELECT * FROM stores WHERE store_id = @store"
        Dim dbcommand As New MySqlCommand(Sql, Connection)

        dbcommand.Parameters.AddWithValue("@store", StoreId)

        Try
            Connection.Open()

            Dim reader As MySqlDataReader = dbcommand.ExecuteReader(CommandBehavior.SingleRow)

            If reader.Read Then
                Terminal = reader("store_name").ToString
            Else
                Terminal = Nothing
            End If
            reader.Close()
        Catch ex As Exception
            Throw ex
        Finally
            Connection.Close()
        End Try

        Return Terminal

    End Function
    Public Shared Function GetStoresList(AccountId As Integer) As DataTable
        ' Obtiene la tabla de productos
        Dim dt = New DataTable()
        Dim Connection As MySqlConnection = MySqlDataBase.GetConnection
        Dim Sql As String = "SELECT store_id,store_name " &
            "FROM stores WHERE account_id = @account ORDER BY store_name"

        Dim dbcommand = New MySqlCommand(Sql, Connection)

        dbcommand.Parameters.AddWithValue("@account", AccountId)

        Try
            Connection.Open()

            Dim reader As MySqlDataReader = dbcommand.ExecuteReader()
            If reader.HasRows Then
                dt.Load(reader)
            End If
            reader.Close()
        Catch ex As Exception
            Throw ex
        Finally
            Connection.Close()
        End Try

        Return dt
    End Function
    Public Shared Function UpdateStore(terminal As Terminal) As Boolean
        Dim Connection As MySqlConnection = MySqlDataBase.GetConnection
        Dim Sql As String = "UPDATE terminals " &
            "SET terminal_name=@name, terminal_description=@description " &
            "WHERE terminal_id=@id"
        Dim dbcommand As New MySqlCommand(Sql, Connection)

        dbcommand.Parameters.AddWithValue("@id", terminal.Id)
        dbcommand.Parameters.AddWithValue("@name", terminal.Name)
        dbcommand.Parameters.AddWithValue("@description", terminal.Description)
        dbcommand.Parameters.AddWithValue("@visible", terminal.Visible)

        Try
            Connection.Open()

            dbcommand.ExecuteNonQuery()
            Return True
        Catch ex As Exception
            Throw ex
        Finally
            Connection.Close()
        End Try
    End Function
    Public Shared Function DeleteStore(Id As Integer) As Boolean
        Dim Connection As MySqlConnection = MySqlDataBase.GetConnection
        Dim Sql As String = "DELETE FROM terminals " &
            "WHERE terminal_id=@id"
        Dim dbcommand As New MySqlCommand(Sql, Connection)

        dbcommand.Parameters.AddWithValue("@id", Id)

        Try
            Connection.Open()

            dbcommand.ExecuteNonQuery()
            Return True
        Catch ex As Exception
            Throw ex
        Finally
            Connection.Close()
        End Try
    End Function
    Public Shared Function AddStore(terminal As Terminal) As Boolean
        Dim Connection As MySqlConnection = MySqlDataBase.GetConnection
        Dim Sql As String = "INSERT INTO terminals " &
            "(account_id,terminal_name,terminal_description) " &
            "VALUES (@account,@name,@description)"
        Dim dbcommand As New MySqlCommand(Sql, Connection)

        dbcommand.Parameters.AddWithValue("@account", terminal.Account)
        dbcommand.Parameters.AddWithValue("@name", terminal.Name)
        dbcommand.Parameters.AddWithValue("@description", terminal.Description)

        Try
            Connection.Open()

            dbcommand.ExecuteNonQuery()
            Return True
        Catch ex As Exception
            Throw ex
        Finally
            Connection.Close()
        End Try
    End Function
End Class