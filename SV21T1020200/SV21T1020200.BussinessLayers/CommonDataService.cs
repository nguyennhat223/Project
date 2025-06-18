using SV21T1020200.DataLayers;
using SV21T1020200.DataLayers.SQLServer;
using SV21T1020200.DomainModels;

namespace SV21T1020200.BussinessLayers
{
    public class CommonDataService
    {
        private static readonly ICommonDAL<Customer> customerDB;
        private static readonly ICommonDAL<Shipper> shipperDB;
        private static readonly ICommonDAL<Supplier> supplierDB;
        private static readonly ICommonDAL<Employee> employeeDB;
        private static readonly ICommonDAL<Category> categoryDB;
        private static readonly ISimpleQueryDAL<Province> provinceDB;

        /// <summary>
        /// Ctor
        /// </summary>
        static CommonDataService()
        {
            //string connectionString = "server=DESKTOP-9BUVSMC,1433;user id=sa;password=123;database=LiteCommerceDB;TrustServerCertificate=true";
            //provinceDB = new DataLayers.SQLServer.ProvinceDAL(connectionString);
            //customerDB = new DataLayers.SQLServer.CustomerDAL(connectionString);
            //shipperDB = new DataLayers.SQLServer.ShipperDAL(connectionString);
            //supplierDB = new DataLayers.SQLServer.SupplierDAL(connectionString);
            //employeeDB = new DataLayers.SQLServer.EmployeeDAL(connectionString);
            //categoryDB = new DataLayers.SQLServer.CategoryDAL(connectionString);
            string connectionString = Configuration.ConnectionString;
            provinceDB = new ProvinceDAL(connectionString);
            customerDB = new CustomerDAL(connectionString);
            shipperDB = new ShipperDAL(connectionString);
            supplierDB = new SupplierDAL(connectionString);
            employeeDB = new EmployeeDAL(connectionString);
            categoryDB = new CategoryDAL(connectionString);
        }
        /// <summary>
        /// Tìm kiếm và lấy danh sách khách hàng dưới dạng phân trang
        /// </summary>
        /// <param name="rowCount">Tham số đầu ra cho biết số dòng tìm được</param>
        /// <param name="page">Trang cần hiển thị</param>
        /// <param name="pageSize">Số dòng hiển thị trên mỗi trang</param>
        /// <param name="searchValue">Tên khách hàng hoặc tên giao dịch cần tìm</param>
        /// <returns></returns>
        public static List<Employee> ListOfEmployees(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = employeeDB.Count(searchValue);
            return employeeDB.List(page, pageSize, searchValue);
        }
        public static Employee? GetEmployee(int id)
        {
            return employeeDB.Get(id);
        }
        public static int AddEmployee(Employee employee)
        {
            return employeeDB.Add(employee);
        }
        public static bool DeleteEmployee(int id)
        {
            return employeeDB.Delete(id);
        }
        public static bool UpdateEmployee(Employee employee)
        {
            return employeeDB.Update(employee);
        }
        public static bool InUsedEmployee(int id)
        {
            return employeeDB.InUsed(id);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Shipper> ListOfShippers(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = shipperDB.Count(searchValue);
            return shipperDB.List(page, pageSize, searchValue);
        }
        public static Shipper? GetShipper(int id)
        {
            return shipperDB.Get(id);
        }
        public static int AddShipper(Shipper shipper)
        {
            return shipperDB.Add(shipper);
        }
        public static bool UpdateShipper(Shipper shipper)
        {
            return shipperDB.Update(shipper);
        }
        public static bool DeleteShipper(int id)
        {
            return shipperDB.Delete(id);
        }
        public static bool InUsedShipper(int id)
        {
            return shipperDB.InUsed(id);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Supplier> ListOfSuppliers(out int rowCount, int page, int pageSize = 0, string searchValue = "")
        {
            rowCount = supplierDB.Count(searchValue);
            return supplierDB.List(page, pageSize, searchValue);
        }
        /// <summary>
        /// Lấy thông tin nhà cung cấp dựa vào mã
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Supplier? GetSupplier(int id)
        {
            return supplierDB.Get(id);
        }
        public static int AddSupplier (Supplier data)
        {
            return supplierDB.Add(data);
        }
        public static bool UpdateSupplier(Supplier data)
        {
            return supplierDB.Update(data);
        }
        public static bool DeleteSupplier(int id)
        {
            return supplierDB.Delete(id);
        }
        public static bool InUsedSupplier(int id)
        {
            return supplierDB.InUsed(id);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<Province> ListOfProvinces()
        {
            return provinceDB.List();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Customer> ListOfCustomers(out int rowCount, int page, int pageSize = 0, string searchValue = "")
        {
            rowCount = customerDB.Count(searchValue);
            return customerDB.List(page, pageSize, searchValue);
        }
        /// <summary>
        /// Lấy thông tin của 1 khách hàng dựa vào mã
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Customer? GetCustomer(int id)
        {
            return customerDB.Get(id);
        }
        /// <summary>
        /// Bố sung 1 khách hàng mới. Hàm trả về mã của khách hàng được bổ sung
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddCustomer(Customer data)
        {
            return customerDB.Add(data);
        }
        /// <summary>
        /// Cập nhật thông tin của 1 khách hàng
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public static bool UpdateCustomer(Customer customer)
        {
            return customerDB.Update(customer);
        }
        /// <summary>
        /// Xóa 1 khách hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteCustomer(int id)
        {
            if (customerDB.InUsed(id))
            {
                return false;
            }
            return customerDB.Delete(id);
        }
        /// <summary>
        /// Kiểm tra xem một khách hàng hiện đang có đơn hàng hay không ?
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool InUsedCustomer(int id)
        {
            return customerDB.InUsed(id);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Category> ListOfCategories(out int rowCount, int page, int pageSize = 0, string searchValue = "")
        {
            rowCount = categoryDB.Count(searchValue);
            return categoryDB.List(page, pageSize, searchValue);
        }
        public static Category? GetCategory(int id)
        {
            return categoryDB.Get(id);
        }
        /// <summary>
        /// Thêm loại hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddCategory(Category data)
        {
            return categoryDB.Add(data);
        }
        /// <summary>
        /// Cập nhật loại hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateCategory(Category data)
        {
            return categoryDB.Update(data);
        }
        /// <summary>
        /// Xóa loại hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteCategory(int id) 
        {
            return categoryDB.Delete(id); 
        }
        /// <summary>
        /// Kiểm tra loại hàng còn trong mặt hàng không
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool InUsedCategory (int id)
        {
            return categoryDB.InUsed(id);
        }
    }
}
