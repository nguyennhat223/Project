using SV21T1020200.DomainModels;

namespace SV21T1020200.Web.Models
{
    public class SupplierSearchResult : PaginationSearchResult
    {
        public required List<Supplier> Data { get; set; }
    }
}
