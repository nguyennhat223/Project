using SV21T1020200.DomainModels;

namespace SV21T1020200.Web.Models
{
    public class CustomerSearchResult : PaginationSearchResult
    {
        public required List<Customer> Data { get; set; }
    }
}
