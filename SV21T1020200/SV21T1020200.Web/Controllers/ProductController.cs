using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020200.BussinessLayers;
using SV21T1020200.DomainModels;
using SV21T1020200.Web.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SV21T1020200.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.EMPLOYEE}, {WebUserRoles.ADMINISTRATOR}")]
    public class ProductController : Controller
    {
        private const int PAGE_SIZE = 20;
        private const string PRODUCT_SEARCH_CONDITION = "ProductSearchCondition";
        public IActionResult Index()
        {
            ProductSearchInput? condition = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH_CONDITION);
            if (condition == null)
            {
                condition = new ProductSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = "",
                    CategoryID = 0,
                    SupplierID = 0,
                    MinPrice = 0,
                    MaxPrice = 0
                };
            }
            return View(condition);
        }
        public IActionResult Search(ProductSearchInput condition)
        {
            int rowCount;
            var data = ProductDataService.ListProducts(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "", 
                                                        condition.CategoryID, condition.SupplierID, condition.MinPrice, condition.MaxPrice);
            ProductSearchResult model = new ProductSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                CategoryID = condition.CategoryID,
                SupplierID = condition.SupplierID,
                MinPrice = condition.MinPrice,
                MaxPrice = condition.MaxPrice,
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(PRODUCT_SEARCH_CONDITION, condition);
            return View(model);
        }
        public IActionResult Create()
        {
            var data = new Product()
            {
                ProductID = 0,
                IsSelling = false
            };
            ViewBag.Title ="Bổ sung sản phẩm mới";
            return View("Edit", data);
        }
        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                ProductDataService.DeleteProduct(id);
                return RedirectToAction("Index");
            }
            var data = ProductDataService.GetProduct(id);
            if (data == null)
            {
                return RedirectToAction("Index");
            }
            return View(data);
        }
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin của mặt hàng";
            var data = ProductDataService.GetProduct(id);
            if (data == null)
            {
                return RedirectToAction("Index");
            }
            return View(data);
        }
        public IActionResult Photo(int id = 0, string method = "", int photoId = 0)
        {
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung ảnh cho mặt hàng";
                    var newProductPhoto = new ProductPhoto
                    {
                        PhotoID = 0,
                        ProductID = id,
                        IsHidden = false

                    };
                    return View("Photo", newProductPhoto);
                case "edit":
                    ViewBag.Title = "Thay đổi ảnh của mặt hàng";
                    var dataPhoto = ProductDataService.GetPhoto(photoId);

                    if (dataPhoto == null)
                    {
                        return RedirectToAction("Index");
                    }

                    return View(dataPhoto);
                case "delete":
                    ProductDataService.DeletePhoto(photoId);
                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Index");
            }
        }
        public IActionResult Attribute(int id = 0, string method = "", int attributeId = 0)
        {
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung thuộc tính của mặt hàng";
                    var data = new ProductAttribute()
                    {
                        AttributeID = 0,
                        DisplayOrder = 0,
                        ProductID = id
                    };
                    return View(data);
                case "edit":
                    ViewBag.Title = "Cập nhật thuộc tính của mặt hàng";
                    var md = ProductDataService.GetAttribute(attributeId);
                    if (md == null)
                        return RedirectToAction("Edit");
                    return View(md);
                case "delete":
                    ProductDataService.DeleteAttribute(attributeId);
                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public IActionResult Save(Product data, IFormFile? _Photo)
        {
            ViewBag.Title = data.ProductID == 0 ? "Bổ sung mặt hàng mới" : "Cập nhật thông tin mặt hàng";

            if (string.IsNullOrWhiteSpace(data.ProductName))
                ModelState.AddModelError(nameof(data.ProductName), "Tên không được để trống");
            if (data.CategoryID == 0)
                ModelState.AddModelError(nameof(data.CategoryID), "Vui lòng chọn loại hàng");
            if (data.SupplierID == 0)
                ModelState.AddModelError(nameof(data.SupplierID), "Vui lòng chọn nhà cung cấp");
            if (string.IsNullOrWhiteSpace(data.Unit))
                ModelState.AddModelError(nameof(data.Unit), "Đơn vị tính không được để trống");
            if (data.Price <= 0)
                ModelState.AddModelError(nameof(data.Price), "Vui lòng nhập giá");


            if (!ModelState.IsValid) // NẾu trường hợp Modelstate không hợp lệ
            {

                return View("Edit", data);
            }
            if (_Photo != null)
            {
                //Tên file sẽ lưu trên server
                string fileName = $"{DateTime.Now.Ticks}_{_Photo.FileName}";
                //Đường dẫn đến file sẽ lưu trên server (vd: D:\MyWeb\wwwroot\images\employees\photo.png)
                string filePath = Path.Combine(ApplicationContext.WebRootPath, @"images\products", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    _Photo.CopyTo(stream);
                }
                data.Photo = fileName;
            }
            try
            {
                if (data.ProductID == 0)
                {
                    int id = ProductDataService.AddProduct(data);
                    if (id <= 0)
                    {
                        ModelState.AddModelError("Error", "Không thêm được mặt hàng.");
                        return View("Edit", data);
                    }
                }
                else
                {
                    bool result = ProductDataService.UpdateProduct(data);
                    if (!result)
                    {
                        ModelState.AddModelError("Error", "Không cập nhật được mặt hàng.");
                        return View("Edit", data);
                    }
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return View("Edit", data);
            }


        }
        [HttpPost]
        public IActionResult SavePhoto(ProductPhoto dataProductPhoto, IFormFile? _Photo)
        {
            ViewBag.Title = dataProductPhoto.PhotoID == 0 ? "Bổ sung ảnh của mặt hàng" : "Thay đổi ảnh của mặt hàng";

            if (string.IsNullOrWhiteSpace(dataProductPhoto.Description))
                ModelState.AddModelError(nameof(dataProductPhoto.Description), "Mô tả không được để trống.");
            if (dataProductPhoto.DisplayOrder == 0)
                ModelState.AddModelError(nameof(dataProductPhoto.DisplayOrder), "Vui lòng nhập thứ tự hiển thị của mặt hàng.");
            //if (_Photo == null)
            //    ModelState.AddModelError(nameof(dataProductPhoto.Photo), "Vui lòng chọn ảnh cho mặt hàng");

            //Dựa vào ModelState để biết có tồn tại trường hợp lỗi nào không? Sử dụng thuộc tính ModelState.IsValid
            if (ModelState.IsValid == false)
            {
                return View("Photo", dataProductPhoto);
            }

            // Xử lý ảnh
            try
            {
                if (_Photo != null)
                {
                    string fileName = $"{DateTime.Now.Ticks}-{_Photo.FileName}";
                    string filePath = Path.Combine(ApplicationContext.WebRootPath, @"images\products", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        _Photo.CopyTo(stream);
                    }
                    dataProductPhoto.Photo = fileName;
                }


                if (dataProductPhoto.PhotoID == 0)
                {
                    long photoID = ProductDataService.AddPhoto(dataProductPhoto);
                    if (photoID <= 0)
                    {
                        ModelState.AddModelError(nameof(dataProductPhoto.DisplayOrder), "Thứ tự hiển thị đã tồn tại.");
                        return View("Photo", dataProductPhoto);
                    }
                }
                else
                {
                    bool result = ProductDataService.UpdatePhoto(dataProductPhoto);
                    if (!result)
                    {
                        ModelState.AddModelError(nameof(dataProductPhoto.DisplayOrder), "Thứ tự hiển thị đã tồn tại.");
                        return View("Photo", dataProductPhoto);
                    }
                }
            }
            catch
            {
                ModelState.AddModelError("Error", "Hệ Thống Tạm Thời Bị Gián Đoạn.");
                return View("Edit", dataProductPhoto);
            }

            return RedirectToAction("Edit", new { id = dataProductPhoto.ProductID });

        }
        [HttpPost]
        public IActionResult SaveAttribute(ProductAttribute dataProductAttribute)
        {
            ViewBag.Title = dataProductAttribute.AttributeID == 0 ? "Bổ sung thuộc tính của mặt hàng" : "Thay đổi thuộc tính của mặt hàng";

            if (string.IsNullOrWhiteSpace(dataProductAttribute.AttributeName))
                ModelState.AddModelError(nameof(dataProductAttribute.AttributeName), "Tên thuộc tính không được để trống.");
            if (string.IsNullOrWhiteSpace(dataProductAttribute.AttributeValue))
                ModelState.AddModelError(nameof(dataProductAttribute.AttributeValue), "Giá trị không được để trống.");
            if (dataProductAttribute.DisplayOrder == 0)
                ModelState.AddModelError(nameof(dataProductAttribute.DisplayOrder), "Vui lòng nhập thứ tự hiển thị của mặt hàng.");

            //Dựa vào ModelState để biết có tồn tại trường hợp lỗi nào không? Sử dụng thuộc tính ModelState.IsValid
            if (ModelState.IsValid == false)
            {
                return View("Attribute", dataProductAttribute);
            }


            if (dataProductAttribute.AttributeID == 0)
            {
                long attributeID = ProductDataService.AddAttribute(dataProductAttribute);
                if (attributeID <= 0)
                {
                    ModelState.AddModelError(nameof(dataProductAttribute.DisplayOrder), "Thứ tự hiển thị đã tồn tại.");
                    return View("Attribute", dataProductAttribute);
                }
            }
            else
            {
                bool result = ProductDataService.UpdateAttribute(dataProductAttribute);
                if (!result)
                {
                    ModelState.AddModelError(nameof(dataProductAttribute.DisplayOrder), "Thứ tự hiển thị đã tồn tại.");
                    return View("Attribute", dataProductAttribute);
                }
            }
            return RedirectToAction("Edit", new { id = dataProductAttribute.ProductID });
        }
    }
}
