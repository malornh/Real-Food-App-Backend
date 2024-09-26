using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RF1.Data;
using RF1.Dtos;
using RF1.Models;
using RF1.Services.PhotoClients;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RF1.Services
{
    public class ProductsService : IProductsService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public ProductsService(DataContext context, IMapper mapper, IPhotoService photoService)
        {
            _context = context;
            _mapper = mapper;
            _photoService = photoService;
        }

        public async Task<IEnumerable<ProductDto>> GetProducts()
        {
            var products = await _context.Products.ToListAsync();
            return _mapper.Map<List<ProductDto>>(products);
        }

        public async Task<ProductDto> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> CreateProduct(ProductDto productDto)
        {
            var product = _mapper.Map<Product>(productDto);
            var farm = GetProductsFarm(productDto);

            product.PhotoId = await _photoService.StorePhotoAsync(productDto.PhotoFile, farm.UserId);

            _context.Products.Add(product);

            await _context.SaveChangesAsync();

            productDto.Id = product.Id;
            productDto.PhotoId = product.PhotoId;

            return productDto;
        }

        public async Task<ProductDto> UpdateProduct(int id, ProductDto productDto)
        {
            var productInDb = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (productInDb == null) throw new ArgumentNullException();

            var farm = await _context.Farms.FirstOrDefaultAsync(f => f.Id == productDto.FarmId);

            _mapper.Map(productDto, productInDb);

            if (productDto.PhotoFile != null)
            {
                if (!string.IsNullOrEmpty(productInDb.PhotoId))
                {
                    productInDb.PhotoId = await _photoService.UpdatePhotoAsync(productDto.PhotoFile, productInDb.PhotoId, farm.UserId);
                }
                else
                {
                    productInDb.PhotoId = await _photoService.StorePhotoAsync(productDto.PhotoFile, farm.UserId);
                }
            }

            await _context.SaveChangesAsync();

            _mapper.Map(productInDb, productDto);

            return productDto;
        }

        public async Task DeleteProduct(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) throw new ArgumentNullException();

            if (product.PhotoId != null)
            {
                await _photoService.DeletePhotoAsync(product.PhotoId);
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        private Farm GetProductsFarm(ProductDto productDto)
        {
            var farm = _context.Farms.FirstOrDefault(f => f.Id == productDto.FarmId);

            if (farm == null) throw new ArgumentNullException();

            return farm;
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
