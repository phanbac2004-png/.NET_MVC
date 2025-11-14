using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PhanXuanBac_22103100089.Data;
using PhanXuanBac_22103100089.Helpers;
using PhanXuanBac_22103100089.ViewModels;

namespace PhanXuanBac_22103100089.Controllers
{
    public class KhachHangController : Controller
    {
        private readonly Hshop2023Context db;
        private readonly IMapper _mapper;

        public KhachHangController(Hshop2023Context context, IMapper mapper)
        {
            db=context;
            _mapper = mapper;
        }
        [HttpGet]
        public IActionResult DangKy()
        {
            return View();
        }
        [HttpPost]
        public IActionResult DangKy(RegisterVM model, IFormFile Hinh)
        {
            if (ModelState.IsValid)
            {
                var khachHang = _mapper.Map<KhachHang>(model);
                khachHang.RandomKey = MyUtil.GenerateRandomKey();
                khachHang.MatKhau = model.MatKhau.ToMd5Hash(khachHang.RandomKey);
                khachHang.HieuLuc = true;
                khachHang.VaiTro = 0;

                if(Hinh != null)
                {
                    khachHang.Hinh = MyUtil.UploadHinh(Hinh, "KhachHang");
                }
                db.Add(khachHang);
                db.SaveChanges();
                return RedirectToAction("Index", "HangHoa");
            }
            return View();
        }
    }
}
