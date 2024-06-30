using RF1.Dtos;
using RF1.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RF1.Services
{
    public interface IProductsService
    {
        Task<IEnumerable<ProductDto>> GetProducts();
        Task<ProductDto> GetProduct(int id);
        Task<ProductDto> CreateProduct(ProductDto productDto);
        Task<bool> UpdateProduct(int id, ProductDto productDto);
        Task<bool> DeleteProduct(int id);
    }
}
