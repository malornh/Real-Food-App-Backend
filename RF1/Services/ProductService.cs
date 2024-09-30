using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RF1.Data;
using RF1.Dtos;
using RF1.Models;
using RF1.Services.PhotoClients;
using RF1.Services.UserAccessorService;
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
        private readonly IUserAccessorService _userAccessorService;

        public ProductsService(DataContext context, IMapper mapper, IPhotoService photoService, IUserAccessorService userAccessorService)
        {
            _context = context;
            _mapper = mapper;
            _photoService = photoService;
            _userAccessorService = userAccessorService;
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
            var farm = await GetProductsFarmAsync(productDto);
            var userId = _userAccessorService.GetUserId();
            if (userId != farm.UserId) throw new ArgumentException("User cannot add products to another user's farm");

            product.PhotoId = await _photoService.StorePhotoAsync(productDto.PhotoFile);

            _context.Products.Add(product);

            await _context.SaveChangesAsync();

            productDto.Id = product.Id;

            return productDto;
        }

        [HttpPut("{id}")]
        public async Task<ProductDto> UpdateProduct(int id, ProductDto productDto)
        {
            var productInDb = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (productInDb == null) throw new ArgumentNullException();

            var farm = await GetProductsFarmAsync(productDto);
            var userId = _userAccessorService.GetUserId();
            if (userId != farm.UserId) throw new ArgumentException("User cannot add products to another user's farm");

            _mapper.Map(productDto, productInDb);

            if (productDto.PhotoFile != null)
            {
                if (!string.IsNullOrEmpty(productInDb.PhotoId))
                {
                    productInDb.PhotoId = await _photoService.UpdatePhotoAsync(productDto.PhotoFile, productInDb.PhotoId);
                }
                else
                {
                    productInDb.PhotoId = await _photoService.StorePhotoAsync(productDto.PhotoFile);
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

            var farm = await _context.Farms.FirstOrDefaultAsync(f => f.Id == product.Id);
            var userId = _userAccessorService.GetUserId();
            if (userId != farm.UserId) throw new ArgumentException("User cannot add products to another user's farm");

            if (product.PhotoId != null)
            {
                await _photoService.DeletePhotoAsync(product.PhotoId);
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        private async Task<Farm> GetProductsFarmAsync(ProductDto productDto)
        {
            var farm = await _context.Farms.FirstOrDefaultAsync(f => f.Id == productDto.FarmId);

            if (farm == null) throw new ArgumentNullException();

            return farm;
        }
    }
}
