using Azure;
using Dapper;
using SV21T1020200.DomainModels;
using System;
using System.Buffers;
using System.ComponentModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SV21T1020200.DataLayers.SQLServer
{
    public class ProductDAL : BaseDAL, IProductDAL
    {
        public ProductDAL(string connectionString) : base(connectionString)
        {
        }
        public int Add(Product data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists (select * from Products where ProductName = @ProductName)
                                select -1
                           else
                                begin
                                    insert into Products(ProductName, ProductDescription, SupplierID, CategoryID, Unit, Price, Photo, IsSelling)
                                    values(@ProductName, @ProductDescription, @SupplierID, @CategoryID, @Unit, @Price, @Photo, @IsSelling)
                                    select scope_identity();
                                end
                            ";
                var parameters = new
                {
                    ProductName = data.ProductName ?? "",
                    ProductDescription = data.ProductDescription ?? "",
                    SupplierID = data.SupplierID,
                    CategoryID = data.CategoryID,
                    Unit = data.Unit ?? "",
                    Price = data.Price,
                    Photo = data.Photo ?? "",
                    IsSelling = data.IsSelling
                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public long AddAttribute(ProductAttribute data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists (select * from ProductAttributes where AttributeID = @AttributeID)
                                select -1
                            else
                                begin
                                    insert into ProductAttributes(ProductID ,AttributeName, AttributeValue, DisplayOrder)
                                    values(@ProductID, @AttributeName, @AttributeValue, @DisplayOrder)
                                    select scope_identity();
                                end
                            ";
                var parameters = new
                {
                    AttributeID = data.AttributeID,
                    ProductID = data.ProductID,
                    AttributeName = data.AttributeName ?? "",
                    AttributeValue = data.AttributeValue ?? "",
                    DisplayOrder = data.DisplayOrder
                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public long AddPhoto(ProductPhoto data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"
                            if exists (select * from ProductPhotos where PhotoID = @PhotoID)
                                select -1
                            else
                                begin
                                    insert into ProductPhotos(ProductID, Photo, Description, DisplayOrder, IsHidden)
                                    values(@ProductID, @Photo, @Description, @DisplayOrder, @IsHidden)
                                    select scope_identity();
                                end
                            ";
                var parameters = new
                {
                    PhotoID = data.PhotoID,
                    ProductID = data.ProductID,
                    Photo = data.Photo ?? "",
                    Description = data.Description ?? "",
                    DisplayOrder = data.DisplayOrder,
                    IsHidden = data.IsHidden
                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public int Count(string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            int count = 0;
            searchValue = $"%{searchValue}%";
            using (var connection = OpenConnection())
            {
                var sql = @"
                            select count(*)
                            from Products
                            where (ProductName like @searchValue)
                                    and (@CategoryID = 0 or CategoryID = @CategoryID)
                                    and (@SupplierID = 0 or SupplierID = @SupplierID)
                                    and (Price >= @MinPrice)
                                    and (@MaxPrice <= 0 or Price <= @MaxPrice) 
                            ";
                var parameters = new
                {
                    searchValue,
                    categoryID, 
                    supplierID, 
                    minPrice, 
                    maxPrice
                };
                count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return count;
        }

        public bool Delete(int productID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from ProductPhotos where ProductID = @productID
                            delete from ProductAttributes where ProductID = @productID
                            delete from Products where ProductID = @productID";
                var parameters = new
                {
                    productID = productID
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool DeleteAttribute(long attributeID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from ProductAttributes where AttributeID = @attributeID";
                var parameters = new
                {
                    attributeID = attributeID
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool DeletePhoto(long photoID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from ProductPhotos where PhotoID = @PhotoID";
                var parameters = new
                {
                    PhotoID = photoID
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public Product? Get(int productID)
        {
            Product? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"
                             select * from Products where ProductID = @ProductID                  
                            ";
                var parameters = new
                {
                    ProductID = productID
                };
                data = connection.QueryFirstOrDefault<Product>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public ProductAttribute? GetAttribute(long attributeID)
        {
            ProductAttribute? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"
                             select * from ProductAttributes where AttributeID = @AttributeID                  
                            ";
                var parameters = new
                {
                    AttributeID = attributeID
                };
                data = connection.QueryFirstOrDefault<ProductAttribute>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public ProductPhoto? GetPhoto(long photoID)
        {
            ProductPhoto? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"
                             select * from ProductPhotos where PhotoID = @PhotoID                  
                            ";
                var parameters = new
                {
                    PhotoID = photoID
                };
                data = connection.QueryFirstOrDefault<ProductPhoto>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public bool InUsed(int productID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from OrderDetails where ProductID = @ProductID)
                                    select 1;
                            else
                                    select 0;";
                var parameters = new
                {
                    ProductID = productID
                };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return result;
        }

        public List<Product> List(int page = 1, int PageSize = 0, string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            List<Product> data = new List<Product>();
            searchValue = $"%{searchValue}%";
            using (var connection = OpenConnection())
            {
                var sql = @"select *
                            from (
		                            select *,
				                    ROW_NUMBER() over(order by ProductName) as RowNumber
		                            from Products
		                            where (@SearchValue = N'' OR ProductName like @SearchValue)
                                      and (@CategoryID = 0 or CategoryID = @CategoryID)
                                      and (@SupplierID = 0 or SupplierID = @SupplierID)
                                      and (@MinPrice = 0 or Price >= @MinPrice)
                                      and (@MaxPrice = 0 or Price <= @MaxPrice)
	                              ) as t
                            where	(@pageSize = 0)
	                                or	(t.RowNumber between (@Page -1) * @PageSize +1 and @Page * @PageSize)
                            order by RowNumber
                        ";
                var parameters = new
                {
                    Page = page,                // Bên trái: tên của tham số trong câu lẹnh của SQL, Bên phải: giá trị truyền cho tham số
                    PageSize = PageSize,
                    SearchValue = searchValue,
                    CategoryID = categoryID,
                    SupplierID = supplierID,
                    MinPrice = minPrice,
                    MaxPrice = maxPrice
                };
                data = connection.Query<Product>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }

        public IList<ProductAttribute> ListAttributes(int productID)
        {
            List<ProductAttribute> data = new List<ProductAttribute>();
            using (var connection = OpenConnection())
            {
                var sql = @"select * from ProductAttributes where ProductID = @ProductID";
                var parameters = new
                {
                    ProductID = productID
                };
                data = connection.Query<ProductAttribute>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }

        public IList<ProductPhoto> ListPhotos(int productID)
        {
            List<ProductPhoto> data = new List<ProductPhoto>();
            using (var connection = OpenConnection())
            {
                var sql = @"select * from ProductPhotos where ProductID = @ProductID";
                var parameters = new
                {
                    ProductID = productID
                };
                data = connection.Query<ProductPhoto>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }

        public bool Update(Product data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"
                            if not exists (select * from Products where ProductID <> @ProductID and ProductName = @ProductName)
                            begin
                                update Products
                                set ProductName=@ProductName, ProductDescription=@ProductDescription, SupplierID=@SupplierID, 
                                    CategoryID=@CategoryID, Unit=@Unit, Price=@Price, Photo=@Photo, IsSelling=@IsSelling
                                where ProductID= @ProductID
                            end
                           ";
                var parameters = new
                {
                    ProductID = data.ProductID,
                    ProductName = data.ProductName ?? "",
                    ProductDescription = data.ProductDescription ?? "",
                    SupplierID = data.SupplierID,
                    CategoryID = data.CategoryID,
                    Unit = data.Unit ?? "",
                    Price = data.Price,
                    Photo = data.Photo ?? "",
                    IsSelling = data.IsSelling
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool UpdateAttribute(ProductAttribute data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"
                            if not exists (select * from ProductAttributes where AttributeID <> @AttributeID and AttributeName=@AttributeName)
                            begin
                            update ProductAttributes
                            set AttributeName=@AttributeName, AttributeValue=@AttributeValue, DisplayOrder=@DisplayOrder
                            where AttributeID= @AttributeID
                            end
                           ";
                var parameters = new
                {
                    AttributeID = data.AttributeID,
                    ProductID = data.ProductID,
                    AttributeName = data.AttributeName ?? "",
                    AttributeValue = data.AttributeValue ?? "",
                    DisplayOrder = data.DisplayOrder
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool UpdatePhoto(ProductPhoto data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"
                            if not exists (select * from ProductPhotos where PhotoID <> @PhotoID and Photo=@Photo)
                                begin
                                    update ProductPhotos
                                    set 
                                        Photo=@Photo, Description=@Description, DisplayOrder=@DisplayOrder, IsHidden=@IsHidden
                                    where PhotoID= @PhotoID
                                end
                           ";
                var parameters = new
                {
                    PhotoID = data.PhotoID,
                    ProductID = data.ProductID,
                    Photo = data.Photo ?? "",
                    Description = data.Description ?? "",
                    DisplayOrder = data.DisplayOrder,
                    IsHidden = data.IsHidden
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }
        public List<Product> GetBestSeller()
        {
            List<Product> data = new List<Product>();
            using (var connection = OpenConnection())
            {
                var sql = @"select Top(10) * from Products join 
			                (select  Products.ProductID,COUNT(OrderDetails.ProductID) as 'soban'
			                from Products join OrderDetails on Products.ProductID= OrderDetails.ProductID 
			                GROUP BY Products.ProductID) as P2
			                on Products.ProductID = P2.ProductID";
                var parameters = new
                {
                };
                data = connection.Query<Product>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }
        public int Sold(int id)
        {
            int sold = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"select COUNT(OrderDetails.ProductID) as 'soban'
			                from Products join OrderDetails on Products.ProductID= OrderDetails.ProductID 
			                where Products.ProductID = @ProductID
                            ";
                var parameters = new
                {
                    ProductID = id
                };
                sold = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return sold;
        }
    }
}
