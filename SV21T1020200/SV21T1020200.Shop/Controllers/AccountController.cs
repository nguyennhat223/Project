using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020200.BussinessLayers;
using SV21T1020200.DomainModels;

namespace SV21T1020200.Shop.Controllers
{
    public class AccountController : Controller
    {
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginAsync(string username, string password)
        {
            ViewBag.Username = username;

            // Kiểm tra thông tin đầu vào
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("Error", "Nhập tên và mật khẩu!");
            }

            var userAccount = UserAccountService.Authorize(UserTypes.Customer, username, password);
            if (userAccount == null)
            {
                ModelState.AddModelError("Error", "Đăng nhập thất bại");
                return View();
            }

            // Đăng nhập thành công: ghi nhận trạng thái đăng nhập
            //1.Tạo thông tin của người dùng
            var userData = new WebUserData()
            {
                UserId = userAccount.UserId,
                UserName = userAccount.UserName,
                DisplayName = userAccount.DisplayName,
                Photo = userAccount.Photo,
                Roles = userAccount.RoleNames.Split(',').ToList()
            };
            //2. Ghi nhận trạng thái đăng nhập
            await HttpContext.SignInAsync(userData.CreatePrincipal());

            //3. Quay về trang chủ
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Remove("userId");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
        public IActionResult AccessDenined()
        {
            return Content("Tài khoản không được phép sử dụng chức năng này");
        }
        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SavePass(string userName, string oldPassword, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrEmpty(oldPassword))
            {
                ModelState.AddModelError("oldPassword", "Vui lòng nhập mật khẩu cũ");
            }
            if (string.IsNullOrEmpty(newPassword))
            {
                ModelState.AddModelError("newPassword", "Vui lòng nhập mật khẩu mới");
            }
            if (string.IsNullOrEmpty(confirmPassword))
            {
                ModelState.AddModelError("confirmPassword", "Vui lòng nhập xác nhận mật khẩu");
            }
            var check = UserAccountService.Authorize(UserTypes.Customer, userName, oldPassword);
            if (check == null)
            {
                ModelState.AddModelError("oldPassword", "Mật khẩu sai. Vui lòng nhập lại.");
            }
            if (newPassword == oldPassword)
            {
                ModelState.AddModelError("newPassword", "Mật khẩu mới chưa trùng khớp");
            }
            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("confirmPassword", "Mật khẩu xác nhận chưa trùng khớp");
            }
            if (ModelState.IsValid == false)
            {
                ModelState.AddModelError("Error", "Hệ thống tạm thời gián đoạn");
                return View("ChangePassword");
            }
            if (oldPassword != null && newPassword != null)
            {
                UserAccountService.ChangePassword(UserTypes.Customer, userName, newPassword);
                ModelState.AddModelError("Accept", "Đổi mật khẩu thành công");
                return View("ChangePassword");
            }
            return Json("Không đổi được mật khẩu");
        }
        [HttpGet]
        public IActionResult ChangeInformation(int id)
        {
            ViewBag.Title = "Cập nhật thông tin của khách hàng";
            var data = CommonDataService.GetCustomer(id);
            if (data == null)
            {
                return RedirectToAction("Index");
            }
            return View(data);
        }
        [HttpPost]
        public IActionResult Save(Customer data)
        {
            //TODO: Kiểm soát dữ liệu đầu vào
            ViewBag.Title = "Cập nhật thông tin khách hàng";
            // Kiểm tra nếu dữ liệu đầu vào không hợp lệ thì tạo ra một thông báo lỗi và lưu trữ vào ModelState
            if (string.IsNullOrWhiteSpace(data.ContactName))
            {
                ModelState.AddModelError(nameof(data.ContactName), "Tên giao dịch không được để trống");
            }
            if (string.IsNullOrWhiteSpace(data.Phone))
            {
                ModelState.AddModelError(nameof(data.Phone), "Vui lòng nhập số điện thoại");
            }
            if (string.IsNullOrWhiteSpace(data.Province))
            {
                ModelState.AddModelError(nameof(data.Province), "Hãy chọn tỉnh/thành");
            }

            //Dựa vào thuộc tính IsValid của ModelState để biết có tồn tại lỗi hay không?
            if (ModelState.IsValid == false)
            {
                return View("ChangeInformation", data);
            }
            try
            {
                //Cập nhật
                bool result = CommonDataService.UpdateCustomer(data);
                if (result == false)
                {
                    ModelState.AddModelError("Error", "Hệ thống tạm thời gián đoạn");
                    return View("ChangeInformation", data);
                }
                ModelState.AddModelError("Accept", "Đổi thông tin thành công");
                return RedirectToAction("Login");
            }
            catch
            {
                ModelState.AddModelError("Error", "Hệ thống tạm thời gián đoạn");
                return View("ChangeInformation", data);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            var data = new Customer()
            {
                CustomerID = 0,
                IsLocked = false
            };
            ViewBag.Title = "Đăng ký";
            return View(data);
        }
        [HttpPost]
        public IActionResult SaveRegister(Customer data)
        {
            //TODO: Kiểm soát dữ liệu đầu vào
            ViewBag.Title = "Đăng ký tài khoản mới";
            // Kiểm tra nếu dữ liệu đầu vào không hợp lệ thì tạo ra một thông báo lỗi và lưu trữ vào ModelState
            if (string.IsNullOrWhiteSpace(data.CustomerName))
            {
                ModelState.AddModelError(nameof(data.CustomerName), "Tên khách hàng không được để trống");
            }
            if (string.IsNullOrWhiteSpace(data.ContactName))
            {
                ModelState.AddModelError(nameof(data.ContactName), "Tên giao dịch không được để trống");
            }
            if (string.IsNullOrWhiteSpace(data.Phone))
            {
                ModelState.AddModelError(nameof(data.Phone), "Vui lòng nhập số điện thoại");
            }
            if (string.IsNullOrWhiteSpace(data.Email))
            {
                ModelState.AddModelError(nameof(data.Email), "Vui lòng nhập Email");
            }
            if (string.IsNullOrWhiteSpace(data.Password))
            {
                ModelState.AddModelError(nameof(data.Password), "Vui lòng nhập Password");
            }
            if (string.IsNullOrWhiteSpace(data.Address))
            {
                ModelState.AddModelError(nameof(data.Address), "Vui lòng nhập địa chỉ của khách hàng");
            }
            if (string.IsNullOrWhiteSpace(data.Province))
            {
                ModelState.AddModelError(nameof(data.Province), "Hãy chọn tỉnh/thành cho khách hàng");
            }

            //Dựa vào thuộc tính IsValid của ModelState để biết có tồn tại lỗi hay không?
            if (ModelState.IsValid == false)
            {
                return View("Register", data);
            }
            try
            {
                if (data.CustomerID == 0)
                {
                    //Thêm
                    int id = CommonDataService.AddCustomer(data);
                    if (id <= 0)
                    {
                        ModelState.AddModelError(nameof(data.Email), "Email đã được sử dụng");
                        return View("Register", data);
                    }
                }
                return RedirectToAction("Login");
            }
            catch
            {
                ModelState.AddModelError("Error", "Hệ thống tạm thời gián đoạn");
                return View("Register", data);
            }
        }
    }
}
