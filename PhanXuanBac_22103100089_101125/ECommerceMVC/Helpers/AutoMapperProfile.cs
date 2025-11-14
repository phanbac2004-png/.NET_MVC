using AutoMapper;
using PhanXuanBac_22103100089.Data;
using PhanXuanBac_22103100089.ViewModels;
namespace PhanXuanBac_22103100089.Helpers
{
    public class AutoMapperProfile : Profile 
    {
        public AutoMapperProfile() {
            CreateMap<RegisterVM, KhachHang>();//.ForMember(kh => kh.HoTen, option => option.MapFrom(RegisterVM => RegisterVM.HoTen)).ReverseMap();
        }
    }
}
