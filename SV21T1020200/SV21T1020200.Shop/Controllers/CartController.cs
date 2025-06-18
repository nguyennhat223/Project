using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020200.BussinessLayers;
using SV21T1020200.DomainModels;
using SV21T1020200.Shop.Models;
using System.Globalization;

namespace SV21T1020200.Shop.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        public const string ORDER_SEARCH_CONDITION = "OrderSearchCondition";
        private const string PRODUCT_SEARCH_CONDITION = "ProductSearchSale";
        private const int PRODUCT_PAGE_SIZE = 5;
        private const string SHOPPING_CART = "ShoppingCart";
        public const int PAGE_SIZE = 20;
        public IActionResult Index()
        {
            var condition = ApplicationContext.GetSessionData<OrderSearchInput>(ORDER_SEARCH_CONDITION);
            if (condition == null)
            {
                var cultureInfo = new CultureInfo("en-GB");
                condition = new OrderSearchInput
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    Status = 0,
                    TimeRange = $"{DateTime.Today.AddDays(-7).ToString("dd/MM/yyyy", cultureInfo)} - {DateTime.Today.ToString("dd/MM/yyyy", cultureInfo)}"
                };
            }
            return View(condition);
        }
        public IActionResult Create()
        {
            return View();
        }
        private List<CartItem> GetShoppingCart()
        {
            var shoppingCart = ApplicationContext.GetSessionData<List<CartItem>>(SHOPPING_CART);
            if (shoppingCart == null)
            {
                shoppingCart = new List<CartItem>();
                ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            }
            return shoppingCart;
        }
        public IActionResult ShoppingCart()
        {
            return View(GetShoppingCart());
        }
        public IActionResult AddToCart(CartItem item)
        {
            var shoppingCart = GetShoppingCart();
            var existsProduct = shoppingCart.FirstOrDefault(m => m.ProductID == item.ProductID);
            if (existsProduct == null)
            {
                shoppingCart.Add(item);
            }
            else
            {
                existsProduct.Quantity += item.Quantity;
                existsProduct.SalePrice = item.SalePrice;
            }
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }
        public IActionResult RemoveFromCart(int id = 0)
        {
            var shoppingCart = GetShoppingCart();
            int index = shoppingCart.FindIndex(m => m.ProductID == id);
            if (index >= 0)
            {
                shoppingCart.RemoveAt(index);
            }
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return RedirectToAction("Create");
        }
        public IActionResult ClearCart()
        {
            var shoppingCart = GetShoppingCart();
            shoppingCart.Clear();
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return RedirectToAction("Create");
        }
        public IActionResult Init(string deliveryProvince = "", string deliveryAddress = "")
        {
            var shoppingCart = GetShoppingCart();
            if (shoppingCart.Count == 0)
            {
                return Json("Giỏ hàng trống. Vui lòng chọn mặt hàng cần bán");
            }
            if (string.IsNullOrWhiteSpace(deliveryProvince) || string.IsNullOrWhiteSpace(deliveryAddress))
            {
                return Json("Vui lòng nhập đầy đủ thông tin nơi giao hàng");
            }
            var user = User.GetUserData();
            int customerID = int.Parse(user.UserId); //TODO: Thay bởi ID của nhân viên đang login vào hệ thống

            List<OrderDetail> orderDetails = new List<OrderDetail>();
            foreach (var item in shoppingCart)
            {
                orderDetails.Add(new OrderDetail
                {
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    SalePrice = item.SalePrice
                });
            }
            // Gắn EmployeeID cho 1 nhân viên cố định
            int orderID = OrderDataService.InitOrder(48, customerID, deliveryProvince, deliveryAddress, orderDetails);
            ClearCart();
            return Json(orderID);
        }
        public IActionResult HistoryOrder(OrderSearchInput condition)
        {
            int rowCount;
            var data = OrderDataService.ListOrders(out rowCount, condition.Page, condition.PageSize,
                                                    condition.Status, condition.FromTime, condition.ToTime, condition.SearchValue ?? "");
            var model = new OrderSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                Status = condition.Status,
                TimeRange = condition.TimeRange,
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(ORDER_SEARCH_CONDITION, condition);
            return View(model);
        }
        public IActionResult Cancel(int id = 0)
        {
            if (OrderDataService.CancelOrder(id))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Details");
            }
        }
        public IActionResult Finish(int id = 0)
        {
            if (!OrderDataService.FinishOrder(id))
            {
                return RedirectToAction("HistoryOrder");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        public IActionResult Order ()
        {
            var user = User.GetUserData();
            var order = new CustomerInput()
            {
                customer = CommonDataService.GetCustomer(int.Parse(user.UserId)),
                cartItems = GetShoppingCart()
            };
            return View(order);
        }
        public IActionResult Details(int id)
        {
            var order = OrderDataService.GetOrder(id);
            if (order == null)
                return RedirectToAction("Index");
            var details = OrderDataService.ListOrderDetails(id);
            var model = new OrderDetailModel()
            {
                Order = order,
                Details = details
            };
            return View(model);
        }
    }
}
