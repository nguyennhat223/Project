using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020200.BussinessLayers;

namespace SV21T1020200.Web.Controllers
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
        public async Task<IActionResult> LoginAsync (string username, string password)
        {
            ViewBag.Username = username;

            // Kiểm tra thông tin đầu vào
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("Error", "Nhập tên và mật khẩu!");
            }

            //TODO: Kiểm tra xem username và password có đúng hay không?
            //if (username != "admin")
            //{
            //    ModelState.AddModelError("Error", "Đăng nhập thất bại");
            //    return View();
            //}

            var userAccount = UserAccountService.Authorize(UserTypes.Employee, username, password);
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
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
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
            var check = UserAccountService.Authorize(UserTypes.Employee ,userName, oldPassword);
            if(check==null)
            {
                ModelState.AddModelError("oldPassword", "Mật khẩu sai. Vui lòng nhập lại.");
            }    
            if (newPassword == oldPassword)
            {
                ModelState.AddModelError("newPassword", "Mật khẩu mới chưa trùng khớp");
            }
            if(newPassword != confirmPassword)
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
                UserAccountService.ChangePassword(UserTypes.Employee, userName, newPassword);
                ModelState.AddModelError("Error", "Đổi mật khẩu thành công");
                return View("ChangePassword");
            }
            return Json("Không đổi được mật khẩu");
        }
    }
}
