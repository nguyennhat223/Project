﻿using Dapper;
using SV21T1020200.DomainModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SV21T1020200.DataLayers.SQLServer
{
    public class CustomerDAL: BaseDAL, ICommonDAL<Customer>
    {
        public CustomerDAL(string connectionString): base(connectionString)
        {

        }

        public int Add(Customer data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists (select * from Customers where Email = @Email)
                                select -1
                            else
                                begin
                                    insert into Customers(CustomerName, ContactName, Province, Address, Phone, Email, IsLocked, Password)
                                    values(@CustomerName, @ContactName, @Province, @Address, @Phone, @Email, @Islocked, @Password)
                                    select scope_identity();
                                end
                            ";
                var parameters = new
                {
                    CustomerName = data.CustomerName ?? "",
                    ContactName = data.ContactName ?? "",
                    Province = data.Province ?? "",
                    Address = data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? "",
                    IsLocked = data.IsLocked,
                    Password = data.Password ?? ""
                };
                id=connection.ExecuteScalar<int>(sql: sql,param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public int Count(string searchValue = "")
        {
            int count = 0;
            searchValue = $"%{searchValue}%";
            using (var connection = OpenConnection())
            {
                var sql = @"
                            select  count(*)
                            from    Customers
                            where   (CustomerName like @searchValue) or (ContactName like @searchValue)
                       ";
                var parameters = new
                {
                    searchValue
                };
                count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return count;
        }

        public bool Delete(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from Customers where CustomerId = @CustomerId";
                var parameters = new
                {
                    CustomerId = id
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public Customer? Get(int id)
        {
            Customer? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"
                             select * from Customers where CustomerId = @CustomerId                  
                            ";
                var parameters = new
                {
                    CustomerId = id
                };
                data = connection.QueryFirstOrDefault<Customer>(sql: sql,  param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close ();
            }
            return data;
        }
        public bool InUsed(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from Orders where CustomerId = @CustomerId)
                                    select 1;
                            else
                                    select 0;";
                var parameters = new
                {
                    CustomerId = id
                };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return result;
        }
        public List<Customer> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Customer> data = new List<Customer>();
            searchValue = $"%{searchValue}%";
            using (var connection = OpenConnection())
            {
                var sql = @"select *
                            from (
		                            select *,
				                    ROW_NUMBER() over(order by CustomerName) as RowNumber
		                            from Customers
		                            where (CustomerName like @SearchValue) or (ContactName like @searchValue)
	                              ) as t
                            where	(@pageSize = 0)
	                                or	(t.RowNumber between (@page -1) * @pageSize +1 and @page * @pageSize)
                            order by RowNumber
                        ";
                var parameters = new
                {
                    page = page,                // Bên trái: tên của tham số trong câu lẹnh của SQL, Bên phải: giá trị truyền cho tham số
                    pageSize = pageSize,
                    searchValue = searchValue
                };
                data = connection.Query<Customer>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }

        public bool Update(Customer data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"
                            if not exists (select * from Customers where CustomerId <> @CustomerId and Email = @Email)
                            begin
                            update Customers
                            set CustomerName = @CustomerName, ContactName = @ContactName,
                                Province = @Province, Address= @Address, Phone= @Phone,
                                Email= @Email, IsLocked = @IsLocked
                            where CustomerId= @CustomerId
                            end
                           ";
                var parameters = new
                {
                    CustomerId = data.CustomerID,
                    CustomerName = data.CustomerName ?? "",
                    ContactName = data.ContactName ?? "",
                    Province = data.Province ?? "",
                    Address = data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? "",
                    IsLocked = data.IsLocked
                };
                result= connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text)>0;
                connection.Close();
            }
            return result;
        }
    }
}
