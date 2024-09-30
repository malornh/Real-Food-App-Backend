using RF1.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RF1.Services
{
    public interface ICartsService
    {
        Task<IEnumerable<CartDto>> GetCarts();
        Task<CartDto> GetCart(int id);
        Task<IEnumerable<CartDto>> GetCartsByUserId();
        Task<CartDto> CreateCart(int productId, int shopId);
        Task<CartDto> UpdateCart(int id, CartDto cartDto);
        Task<bool> DeleteCart(int id);
    }
}
