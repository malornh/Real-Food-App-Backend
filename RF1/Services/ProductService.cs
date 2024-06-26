﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RF1.Data;
using RF1.Dtos;
using RF1.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RF1.Services
{
    public class ProductsService : IProductsService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public ProductsService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            productDto.Id = product.Id;

            return productDto;
        }

        public async Task<bool> UpdateProduct(int id, ProductDto productDto)
        {
            if (id != productDto.Id)
            {
                return false;
            }

            var productInDb = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (productInDb == null)
            {
                return false;
            }

            _mapper.Map(productDto, productInDb);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }

            return true;
        }

        public async Task<bool> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return false;
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ValidateFarmId(int farmId)
        {
            return await _context.Farms.AnyAsync(f => f.Id == farmId);
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
