﻿Imports MySql.Data.MySqlClient
Public Class InvoiceDB
    Public Shared Function GetAllInvoices(AccountId As Integer, StoreId As Integer) As DataTable
        ' Se usa en Main para obtener el listado de ventas
        Dim dt = New DataTable()
        Dim Connection As MySqlConnection = MySqlDataBase.GetConnection
        Dim Sql = "SELECT * FROM sales_view WHERE account_id = @account AND store_id= @store"

        Dim dbcommand As New MySqlCommand(Sql, Connection)
        dbcommand.Parameters.AddWithValue("@account", AccountId)
        dbcommand.Parameters.AddWithValue("@store", StoreId)

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
    Public Shared Function GetInvoicesFromSession(SessionId As Integer) As DataTable
        ' Se usa en Main para obtener el listado de ventas
        Dim dt = New DataTable()
        Dim Connection As MySqlConnection = MySqlDataBase.GetConnection
        Dim Sql = "SELECT * FROM sales_view WHERE session_id = @session"

        Dim dbcommand As New MySqlCommand(Sql, Connection)
        dbcommand.Parameters.AddWithValue("@session", SessionId)

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
    Public Shared Function GetTotalFromSession(SessionId As Integer) As Double
        ' Se usa en Main para obtener el listado de ventas
        Dim Total As Double
        Dim Connection As MySqlConnection = MySqlDataBase.GetConnection
        Dim Sql = "SELECT SUM(sale_total) as session_total FROM albaco.sales_view where session_id = @session AND status='Pagada';"

        Dim dbcommand As New MySqlCommand(Sql, Connection)
        dbcommand.Parameters.AddWithValue("@session", SessionId)

        Try
            Connection.Open()

            Dim reader As MySqlDataReader = dbcommand.ExecuteReader(CommandBehavior.SingleRow)

            If reader.Read Then
                Total = reader("session_total").ToString
            Else
                Total = Nothing
            End If
            reader.Close()
        Catch ex As Exception
            Throw ex
        Finally
            Connection.Close()
        End Try

        Return Total

    End Function
    Public Shared Function GetReportFromSession(SessionId As Integer) As DataTable
        ' Se usa en Main para obtener el listado de ventas
        Dim dt = New DataTable()
        Dim Connection As MySqlConnection = MySqlDataBase.GetConnection
        Dim Sql = "SELECT 
                        t1.sale_id,
                        t3.sale_timestamp,
                        t2.product_name,
                        t1.sale_quantity,
                        t1.sale_price,
                        t1.sale_tax,
                        (t1.sale_quantity * t1.sale_price * CONCAT('1.', t1.sale_tax)) AS sale_total
                    FROM
                        products_has_sales t1
                            INNER JOIN
                        products t2 ON t1.product_id = t2.product_id
                            INNER JOIN
                        sales t3 ON t1.sale_id = t3.sale_id
                    WHERE
                        t3.status_id = 1 AND t3.session_id = @session"

        Dim dbcommand As New MySqlCommand(Sql, Connection)
        dbcommand.Parameters.AddWithValue("@session", SessionId)

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
    Public Shared Function GetInvoice(InvoiceId As Integer) As Invoice
        Dim invoice As New Invoice
        Dim Connection As MySqlConnection = MySqlDataBase.GetConnection
        Dim Sql As String = "SELECT t1.sale_id,t1.account_id,t1.customer_id,t1.profile_id,t1.status_id,t1.sale_note,t1.sale_timestamp," &
            "SUM(t2.sale_price * t2.sale_quantity) AS sale_subtotal, " &
            "SUM((t2.sale_price * t2.sale_quantity) * (t2.sale_tax / 100)) AS sale_tax, " &
            "SUM((t2.sale_price * t2.sale_quantity) + ((t2.sale_price * t2.sale_quantity) * (t2.sale_tax / 100))) AS sale_total " &
            "FROM sales t1 LEFT JOIN products_has_sales t2 " &
            "ON t1.sale_id = t2.sale_id " &
            "WHERE t1.sale_id = @id " &
            "GROUP BY t1.sale_id"
        Dim dbcommand As New MySqlCommand(Sql, Connection)

        dbcommand.Parameters.AddWithValue("@id", InvoiceId)

        Try
            Connection.Open()

            Dim reader As MySqlDataReader = dbcommand.ExecuteReader(CommandBehavior.SingleRow)

            If reader.Read Then
                With invoice
                    .Id = reader("sale_id")
                    .Account = reader("account_id")
                    .Customer = reader("customer_id")
                    .Profile = reader("profile_id")
                    .Status = reader("status_id")
                    .Note = reader("sale_note").ToString
                    .Timestamp = reader("sale_timestamp")
                    If IsDBNull(reader("sale_subtotal")) Then
                        .Subtotal = 0
                    Else
                        .Subtotal = reader("sale_subtotal")
                    End If
                    If IsDBNull(reader("sale_tax")) Then
                        .Tax = 0
                    Else
                        .Tax = reader("sale_tax")
                    End If
                    If IsDBNull(reader("sale_total")) Then
                        .Total = 0
                    Else
                        .Total = reader("sale_total")
                    End If

                End With
            Else
                invoice = Nothing
            End If
            reader.Close()
        Catch ex As Exception
            ' MessageBox.Show("Ocurrio un error; " & ex.ToString, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Throw ex
        Finally
            Connection.Close()
        End Try

        Return invoice
    End Function
    Public Shared Function AddNewSale(sale As Invoice) As Integer
        Dim Connection As MySqlConnection = MySqlDataBase.GetConnection
        Dim Id As Integer = 0
        Dim Sql = "INSERT INTO sales (account_id,customer_id,profile_id,store_id,session_id) VALUES (@account,@customer,@profile,@store,@session)"

        Dim dbcommand As New MySqlCommand(sql, Connection)

        dbcommand.Parameters.AddWithValue("@account", sale.Account)
        dbcommand.Parameters.AddWithValue("@customer", sale.Customer)
        dbcommand.Parameters.AddWithValue("@profile", sale.Profile)
        dbcommand.Parameters.AddWithValue("@store", sale.Store)
        dbcommand.Parameters.AddWithValue("@session", sale.Session)

        Try
            Connection.Open()
            dbcommand.ExecuteNonQuery()
            Id = dbcommand.LastInsertedId
        Catch ex As Exception
            Throw ex
        Finally
            Connection.Close()
        End Try

        Return Id
    End Function
    Public Shared Function UpdateInvoice(invoice As Invoice) As Boolean
        Dim Connection As MySqlConnection = MySqlDataBase.GetConnection
        Dim Sql As String = "UPDATE sales " &
            "SET customer_id=@customer,status_id=@status,sale_note=@note " &
            "WHERE sale_id=@id"
        Dim dbcommand As New MySqlCommand(Sql, Connection)

        dbcommand.Parameters.AddWithValue("@id", invoice.Id)
        dbcommand.Parameters.AddWithValue("@customer", invoice.Customer)
        dbcommand.Parameters.AddWithValue("@profile", invoice.Profile)
        dbcommand.Parameters.AddWithValue("@status", invoice.Status)
        dbcommand.Parameters.AddWithValue("@note", invoice.Note)

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
    Public Shared Function GetSaleType() As DataTable
        ' Se usa en Main para obtener el listado de ventas
        Dim dt = New DataTable()
        Dim Connection As MySqlConnection = MySqlDataBase.GetConnection
        Dim Sql = "SELECT * FROM status"

        Dim dbcommand As New MySqlCommand(Sql, Connection)

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
End Class
