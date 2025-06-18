using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020200.BussinessLayers;
using SV21T1020200.DomainModels;
using SV21T1020200.Web.Models;

namespace SV21T1020200.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.ADMINISTRATOR}")]
    public class EmployeeController : Controller
    {
        private const int PAGE_SIZE = 10;
        private const string EMPLOYEE_SEARCH_CONDITION = "EmployeeSearchCondition";

        public IActionResult Index(int page = 1, string searchValue = "")
        {
            PaginationSearchInput? condition = ApplicationContext.GetSessionData<PaginationSearchInput>(EMPLOYEE_SEARCH_CONDITION);
            if (condition == null)
            {
                condition = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = ""
                };
            }
            return View(condition);

        }
        public IActionResult Search(PaginationSearchInput condition)
        {
            int rowCount;
            var data = CommonDataService.ListOfEmployees(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "");
            EmployeeSearchResult model = new EmployeeSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(EMPLOYEE_SEARCH_CONDITION, condition);

            return View(model);
        }
        public IActionResult Create()
        {
            var data = new Employee()
            {
                EmployeeID = 0,
                IsWorking = true,
                Photo = "nophoto.png"
            };
            ViewBag.Title = "Bổ sung nhân viên";
            return View("Edit",data);
        }
        public IActionResult Edit(int id)
        {
            ViewBag.Title = "Cập nhật thông tin của Nhân viên";
            var data = CommonDataService.GetEmployee(id);
            if (data == null)
            {
                return RedirectToAction("Index");
            }
            return View(data);
        }
        public IActionResult Delete(int id)
        {
            if (Request.Method == "POST")
            {
                CommonDataService.DeleteEmployee(id);
                return RedirectToAction("Index");
            }
            var data = CommonDataService.GetEmployee(id);
            if (data == null)
            {
                return RedirectToAction("Index");
            }
            return View(data);
        }
        [HttpPost]
        public IActionResult Save(Employee data, string _BirthDate, IFormFile? _Photo)
        {

            DateTime? d = _BirthDate.ToDateTime();
            if (d.HasValue) //(d != null)
            {
                data.BirthDate = d.Value;
            }
            //Xử lý với ảnh
            if(_Photo != null)
            {
                string fileName = $"{DateTime.Now.Ticks}-{_Photo.FileName}";
                string filePath = Path.Combine(ApplicationContext.WebRootPath, @"images\employees", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    _Photo.CopyTo(stream);
                }
                data.Photo= fileName;
            }

            //TODO: Kiểm soát dữ liệu đầu vào
            ViewBag.Title = data.EmployeeID == 0 ? "Bổ sung nhân viên mới" : "Cập nhật thông tin nhân viên";
            // Kiểm tra nếu dữ liệu đầu vào không hợp lệ thì tạo ra một thông báo lỗi và lưu trữ vào ModelState
            if (string.IsNullOrWhiteSpace(data.FullName))
            {
                ModelState.AddModelError(nameof(data.FullName), "Tên nhân viên không được để trống");
            }
            if (string.IsNullOrWhiteSpace(data.Phone))
            {
                ModelState.AddModelError(nameof(data.Phone), "Số điện thoại không được để trống");
            }
            if (string.IsNullOrWhiteSpace(data.Address))
            {
                ModelState.AddModelError(nameof(data.Address), "Địa chỉ không được để trống");
            }
            if (string.IsNullOrWhiteSpace(data.Email))
            {
                ModelState.AddModelError(nameof(data.Email), "Địa chỉ Email không được để trống");
            }
            if (string.IsNullOrWhiteSpace(data.RoleNames))
            {
                ModelState.AddModelError(nameof(data.RoleNames), "Vui lòng chọn quyền truy cập");
            }
            //Dựa vào thuộc tính IsValid của ModelState để biết có tồn tại lỗi hay không?
            if (ModelState.IsValid == false)
            {
                return View("Edit", data);
            }

            try
            {
                if (data.EmployeeID == 0)
                {
                    //Thêm
                    int id=CommonDataService.AddEmployee(data);
                    if (id <= 0)
                    {
                        ModelState.AddModelError(nameof(data.Email), "Email bị trùng");
                        return View("Edit", data);
                    }
                }
                else
                {
                    //Cập nhật
                    bool result =CommonDataService.UpdateEmployee(data);
                    if (result == false)
                    {
                        ModelState.AddModelError(nameof(data.Email), "Email bị trùng");
                        return View("Edit", data);
                    }
                }
                return RedirectToAction("Index");
            }
            catch 
            {
                ModelState.AddModelError("Error", "Hệ thống tạm thời gián đoạn");
                return View("Edit", data);
            }
        }
    }
}
