using SV21T1020200.DomainModels;

namespace SV21T1020200.Web.Models
{
    public class EmployeeSearchResult : PaginationSearchResult
    {
        public required List<Employee> Data { get; set; }
    }
}
