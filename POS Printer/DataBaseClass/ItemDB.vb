﻿Imports MySql.Data.MySqlClient
Public Class ItemDB
    Public Shared Function GetAllItems(SaleId As Integer) As DataTable
        ' Se usa en Invoice para obtener el listado de productos de una venta
        Dim dt = New DataTable()
        Dim Connection As MySqlConnection = MySqlDataBase.GetConnection
        Dim Sql As String = "SELECT * FROM items_view WHERE sale_id = @id"

        Dim dbcommand = New MySqlCommand(Sql, Connection)
        dbcommand.Parameters.AddWithValue("@id", SaleId)

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
    Public Shared Function GetItem(SaleId As Integer, ProductId As Integer) As Item
        Dim item As New Item
        Dim Connection As MySqlConnection = MySqlDataBase.GetConnection
        Dim Sql = "SELECT * FROM products_has_sales WHERE sale_id=@sale AND product_id=@product"
        Dim dbcommand As New MySqlCommand(Sql, Connection)

        dbcommand.Parameters.AddWithValue("@sale", SaleId)
        dbcommand.Parameters.AddWithValue("@product", ProductId)

        Try
            Connection.Open()

            Dim reader As MySqlDataReader = dbcommand.ExecuteReader(CommandBehavior.SingleRow)

            If reader.Read Then
                With item
                    .Sale = reader("sale_id")
                    .Product = reader("product_id")
                    .Quantity = reader("sale_quantity")
                    .Price = reader("sale_price")
                    .Tax = reader("sale_tax")
                    .Formula = reader("sale_formula").ToString
                    .Note = reader("sale_note").ToString
                    .Tare = reader("sale_tare").ToString
                End With
            Else
                item = Nothing
            End If
            reader.Close()
        Catch ex As Exception
            Throw ex
        Finally
            Connection.Close()
        End Try

        Return item
    End Function
    Public Shared Function CheckItem(SaleId As Integer, ProductId As Integer) As Boolean
        Dim Connection As MySqlConnection = MySqlDataBase.GetConnection
        Dim Sql = "SELECT * FROM products_has_sales WHERE sale_id=@sale AND product_id=@product"
        Dim Result As Boolean

        Dim dbcommand As New MySqlCommand(Sql, Connection)
        dbcommand.Parameters.AddWithValue("@sale", SaleId)
        dbcommand.Parameters.AddWithValue("@product", ProductId)

        Try
            Connection.Open()

            Dim reader As MySqlDataReader = dbcommand.ExecuteReader()
            If reader.HasRows Then
                Result = True
            End If
            reader.Close()
        Catch ex As Exception
            Throw ex
        Finally
            Connection.Close()
        End Try

        Return Result
    End Function
    Public Shared Function DeleteItem(SaleId As Integer, ProductId As Integer) As Boolean
        Dim Connection As MySqlConnection = MySqlDataBase.GetConnection
        Dim Sql As String = "DELETE FROM products_has_sales WHERE sale_id=@sale AND product_id =@product"
        Dim Result As Boolean
        Dim dbcommand As New MySqlCommand(Sql, Connection)

        dbcommand.Parameters.AddWithValue("@sale", SaleId)
        dbcommand.Parameters.AddWithValue("@product", ProductId)

        Try
            Connection.Open()

            dbcommand.ExecuteNonQuery()
            Result = True
        Catch ex As Exception
            Throw ex
        Finally
            Connection.Close()
        End Try

        Return Result
    End Function
    Public Shared Function AddItem(item As Item) As Boolean
        Dim Connection As MySqlConnection = MySqlDataBase.GetConnection
        Dim Sql As String = "INSERT INTO products_has_sales " &
            "(sale_id,product_id,sale_quantity,sale_price,sale_tax,sale_formula,sale_note,sale_tare) " &
            "VALUES (@sale,@product,@quantity,@price,@tax,@formula,@note,@tare)"
        Dim Result As Boolean
        Dim dbcommand As New MySqlCommand(Sql, Connection)

        dbcommand.Parameters.AddWithValue("@sale", item.Sale)
        dbcommand.Parameters.AddWithValue("@product", item.Product)
        dbcommand.Parameters.AddWithValue("@quantity", item.Quantity)
        dbcommand.Parameters.AddWithValue("@price", item.Price)
        dbcommand.Parameters.AddWithValue("@tax", item.Tax)
        dbcommand.Parameters.AddWithValue("@formula", item.Formula)
        dbcommand.Parameters.AddWithValue("@note", item.Note)
        dbcommand.Parameters.AddWithValue("@tare", item.Tare)

        Try
            Connection.Open()

            dbcommand.ExecuteNonQuery()
            Result = True
        Catch ex As Exception
            'MessageBox.Show(ex.Message, ex.GetType.ToString)
            Throw ex
        Finally
            Connection.Close()
        End Try

        Return Result
    End Function
    Public Shared Function UpdateItem(item As Item) As Boolean
        Dim Connection As MySqlConnection = MySqlDataBase.GetConnection
        Dim Sql As String = "UPDATE products_has_sales " &
            "SET sale_quantity=@quantity, sale_price=@price, sale_tax=@tax, sale_formula=@formula, sale_note=@note, sale_tare=@tare " &
            "WHERE sale_id=@sale AND product_id=@product"
        Dim dbcommand As New MySqlCommand(Sql, Connection)

        dbcommand.Parameters.AddWithValue("@sale", item.Sale)
        dbcommand.Parameters.AddWithValue("@product", item.Product)
        dbcommand.Parameters.AddWithValue("@quantity", item.Quantity)
        dbcommand.Parameters.AddWithValue("@price", item.Price)
        dbcommand.Parameters.AddWithValue("@tax", item.Tax)
        dbcommand.Parameters.AddWithValue("@formula", item.Formula)
        dbcommand.Parameters.AddWithValue("@note", item.Note)
        dbcommand.Parameters.AddWithValue("@tare", item.Tare)

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
