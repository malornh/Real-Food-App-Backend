using RF1.Dtos;
using RF1.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RF1.Services
{
    public interface IShopsService
    {
        Task<IEnumerable<ShopDto>> GetShops();
        Task<ShopDto> GetShop(int id);
        Task<IEnumerable<ShopDto>> GetShopsByUserId(string userId);
        Task<ShopFullInfoDto> GetShopOrdersWithFarms(int shopId);
        Task<ShopDto> CreateShop(ShopDto shopDto);
        Task<bool> UpdateShop(int id, ShopDto shopDto);
        Task<bool> DeleteShop(int id);
    }
}
