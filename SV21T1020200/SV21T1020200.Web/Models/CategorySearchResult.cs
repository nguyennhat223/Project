using SV21T1020200.DomainModels;

namespace SV21T1020200.Web.Models
{
    public class CategorySearchResult : PaginationSearchResult
    {
        public required List<Category> Data { get; set; }
    }
}
