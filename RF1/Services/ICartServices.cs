using RF1.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RF1.Services
{
    public interface ICartsService
    {
        Task<IEnumerable<CartDto>> GetCarts();
        Task<CartDto> GetCart(int id);
        Task<IEnumerable<CartDto>> GetCartsByUserId(string userId);
        Task<CartDto> CreateCart(CartDto cartDto);
        Task<bool> UpdateCart(int id, CartDto cartDto);
        Task<bool> DeleteCart(int id);
    }
}
