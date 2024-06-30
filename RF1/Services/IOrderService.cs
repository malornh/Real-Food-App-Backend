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
        Task<OrderDto> CreateOrder(OrderDto orderDto);
        Task<bool> UpdateOrder(int id, OrderDto orderDto);
        Task<bool> DeleteOrder(int id);
    }
}
