using SV21T1020200.DomainModels;

namespace SV21T1020200.Shop.Models
{
    public class CustomerInput
    {
        public Customer customer { get; set; }
        public List<CartItem> cartItems { get; set; }
    }
}
