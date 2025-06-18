using Dapper;
using SV21T1020200.DomainModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SV21T1020200.DataLayers.SQLServer
{
    public class EmployeeAccountDAL : BaseDAL, IUserAccountDAL
    {
        public EmployeeAccountDAL(string connectionString): base (connectionString) 
        {

        }
        public UserAccount? Authorize(string username, string password)
        {
            UserAccount? data= null;
            using (var connection = OpenConnection())
            {
                var sql = @"select	EmployeeID as UserId,
		                            Email as UserName,
		                            FullName as DisplayName,
		                            Photo,
		                            RoleNames
                            from	Employees
                            where	Email = @Email and Password = @Password";
                var parameters = new
                {
                    Email = username,
                    Password = password
                };
                data = connection.QueryFirstOrDefault<UserAccount>(sql: sql,param: parameters, commandType: System.Data.CommandType.Text);
            }
            return data;
        }
        public bool ChangePassword(string username, string password)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"
			                        update Employees
			                        set Password= @Password
			                        where Email= @Email
                           ";
                var parameters = new
                {
                    Password = password,
                    Email = username
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }
    }
}
