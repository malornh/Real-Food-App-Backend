﻿using AutoMapper;
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
        private readonly IFreeImageHostService _photoService;
        private readonly IUserAccessorService _userAccessorService;

        public ProductsService(DataContext context, IMapper mapper, IFreeImageHostService photoService, IUserAccessorService userAccessorService)
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

        public async Task<IEnumerable<string>> GetAllProductTypes()
        {
            var productTypes = await _context.Products.Select(p => p.Type).Distinct().ToListAsync();

            return productTypes;
        }

        public async Task<ProductDto> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> CreateProduct(ProductDto productDto)
        {
            if (productDto.FarmId == null) throw new ArgumentException("FarmId field is required.");

            var product = _mapper.Map<Product>(productDto);
            var farm = await GetProductsFarmAsync(productDto);

            var userId = _userAccessorService.GetUserId();
            if (userId != farm.UserId) throw new ArgumentException("User cannot add products to another user's farm.");

            product.PhotoUrl = await _photoService.StorePhotoAsync(productDto.PhotoFile);

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

            var farm = await _context.Farms.FirstOrDefaultAsync(f => f.Id == productInDb.FarmId);
            var userId = _userAccessorService.GetUserId();
            if (userId != farm.UserId) throw new ArgumentException("User cannot add products to another user's farm");

            productInDb.Name = productDto.Name;
            productInDb.Description = productDto.Description;
            productInDb.Type = productDto.Type;
            productInDb.MinUnitOrder = productDto.MinUnitOrder;
            productInDb.UnitOfMeasurement = productDto.UnitOfMeasurement;
            productInDb.Quantity = productDto.Quantity;
            productInDb.PricePerUnit = productDto.PricePerUnit;
            productInDb.DateUpdated = DateOnly.FromDateTime(DateTime.Now);
            if(productDto.DeliveryRadius != null) productInDb.DeliveryRadius = productDto.DeliveryRadius;

            if (productDto.PhotoFile != null)
            {
                if (!string.IsNullOrEmpty(productInDb.PhotoUrl))
                {
                    productInDb.PhotoUrl = await _photoService.UpdatePhotoAsync(productDto.PhotoFile);
                }
                else
                {
                    productInDb.PhotoUrl = await _photoService.StorePhotoAsync(productDto.PhotoFile);
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

            var farm = await _context.Farms.FirstOrDefaultAsync(f => f.Id == product.FarmId);

            var userId = _userAccessorService.GetUserId();
            if (userId != farm.UserId) throw new ArgumentException("User cannot add products to another user's farm");

            if (product.PhotoUrl != null)
            {
                await _photoService.DeletePhotoAsync(product.PhotoUrl);
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        private async Task<Farm> GetProductsFarmAsync(ProductDto productDto)
        {
            var farm = await _context.Farms.FirstOrDefaultAsync(f => f.Id == productDto.FarmId);

            if (farm == null) throw new ArgumentNullException("Farm not found.");

            return farm;
        }
    }
}
