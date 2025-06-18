using SV21T1020200.DataLayers;
using SV21T1020200.DomainModels;

namespace SV21T1020200.BussinessLayers
{
    public static class UserAccountService
    {
        private static readonly IUserAccountDAL employeeAccountDB;
        private static readonly IUserAccountDAL customerAccountDB;

        static UserAccountService()
        {
            string connectionString = Configuration.ConnectionString;

            employeeAccountDB = new DataLayers.SQLServer.EmployeeAccountDAL(connectionString);
            customerAccountDB = new DataLayers.SQLServer.CustomerAccountDAL(connectionString);
        }

        public static UserAccount? Authorize(UserTypes userType, string username, string password)
        {
            if (userType == UserTypes.Employee)
            {
                return employeeAccountDB.Authorize(username, password);
            }
            else
            {
                return customerAccountDB.Authorize(username, password);
            }
        }
        public static bool ChangePassword(UserTypes userType, string username, string password)
        {
            if (userType == UserTypes.Employee)
            {
                return employeeAccountDB.ChangePassword(username, password);
            }
            else
            {
                return customerAccountDB.ChangePassword(username, password);
            }
        }
    }
    /// <summary>
    /// Loại tài khoản
    /// </summary>
    public enum UserTypes
    {
        Employee,
        Customer
    }
}
