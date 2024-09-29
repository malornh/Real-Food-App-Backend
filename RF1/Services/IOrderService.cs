using RF1.Dtos;
using RF1.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RF1.Services
{
    public interface IOrdersService
    {
        Task<IEnumerable<OrderDto>> GetOrders();
        Task<OrderDto> GetOrder(int id);
        Task<List<AllFarmOrdersDto>> GetAllFarmOrdersByFarmId(int farmId);
        Task<List<AllShopOrdersDto>> GetAllShopOrdersByShopId(int shopId);
        Task<OrderDto> CreateOrder(OrderDto orderDto);
        Task<OrderDto> UpdateOrder(OrderDto orderDto);
        Task DeleteOrder(int id);
    }
}
