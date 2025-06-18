using Microsoft.Data.SqlClient;

namespace SV21T1020200.DataLayers.SQLServer
{
    /// <summary>
    /// Lớp cơ sở (Lớp cha) của các lớp cài đặt các phép xử lý dữ liệu trên SQLServer
    /// </summary>
    public abstract class BaseDAL
    {
        /// <summary>
        /// Chuỗi tham số kết nối đến CSDL SQL Server
        /// </summary>
        protected string connectionString = "";
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="connectionString"></param>
        public BaseDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }
        /// <summary>
        /// Tạo và mở một kết nối đến CSDL (SQL Server)
        /// </summary>
        /// <returns></returns>
        protected SqlConnection OpenConnection()
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            return connection;
        }
    }
}
