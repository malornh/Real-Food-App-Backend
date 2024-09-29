using RF1.Dtos;
using RF1.Models;
using System.Collections.Generic;

namespace RF1.Services
{
    public interface IShopsService
    {
        IEnumerable<ShopDto> GetShops();
        ShopDto GetShop(int id);
        IEnumerable<ShopDto> GetShopsByUserId();
        ShopFullInfoDto GetShopOrdersWithFarms(int shopId);
        ShopDto CreateShop(ShopDto shopDto);
        ShopDto UpdateShop(ShopDto shopDto);
        void DeleteShop(int id);
    }
}
