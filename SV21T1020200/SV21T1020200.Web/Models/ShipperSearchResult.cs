using SV21T1020200.DomainModels;

namespace SV21T1020200.Web.Models
{
    public class ShipperSearchResult : PaginationSearchResult
    {
        public required List<Shipper> Data { get; set; }
    }
}
