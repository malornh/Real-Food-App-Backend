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
        Task<CartDto> CreateCart(int orderId);
        Task<CartDto> UpdateCart(int id, int quantity);
        Task DeleteCart(int id);
    }
}
