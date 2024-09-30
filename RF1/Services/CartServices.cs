using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RF1.Data;
using RF1.Dtos;
using RF1.Models;
using RF1.Services.UserAccessorService;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RF1.Services
{
    public class CartsService : ICartsService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IProductsService _productService;
        private readonly IShopsService _shopService;
        private readonly IUserAccessorService _userAccessorService;

        public CartsService(DataContext context, IMapper mapper, IProductsService productService, IShopsService shopService, IUserAccessorService userAccessorService)
        {
            _context = context;
            _mapper = mapper;
            _productService = productService;
            _shopService = shopService;
            _userAccessorService = userAccessorService;
        }

        public async Task<IEnumerable<CartDto>> GetCarts()
        {
            var carts = await _context.Carts
                                      .Include(c => c.Product)
                                      .Include(c => c.Shop)
                                      .ToListAsync();
            return _mapper.Map<List<CartDto>>(carts);
        }

        public async Task<CartDto> GetCart(int id)
        {
            var cart = await _context.Carts
                                     .Include(c => c.Product)
                                     .Include(c => c.Shop)
                                     .FirstOrDefaultAsync(c => c.Id == id);
            return _mapper.Map<CartDto>(cart);
        }

        public async Task<IEnumerable<CartDto>> GetCartsByUserId()
        {
            var userId = _userAccessorService.GetUserId();

            var carts = await _context.Carts
                                      .Include(c => c.Product)
                                      .Include(c => c.Shop)
                                      .Where(c => c.UserId == userId)
                                      .ToListAsync();
            return _mapper.Map<List<CartDto>>(carts);
        }

        public async Task<CartDto> CreateCart(CartDto cartDto)
        {
            var cart = _mapper.Map<Cart>(cartDto);
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
            cartDto.Id = cart.Id;
            return cartDto;
        }

        public async Task<CartDto> CreateCart(int productId, int shopId)
        {
            var userId = _userAccessorService.GetUserId();

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return null;
            }

            var shop = await _context.Shops.FindAsync(shopId);
            if (shop == null)
            {
                return null; 
            }

            var cart = new Cart
            {
                UserId = userId,
                Product = product,
                Shop = shop
            };

            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();

            var cartDto = _mapper.Map<CartDto>(cart);
            return cartDto;
        }

        [HttpPut("{id}")]
        public async Task<CartDto> UpdateCart(int id, CartDto cartDto)
        {
            var cartInDb = await _context.Carts
                                         .Include(c => c.Product)
                                         .Include(c => c.Shop)
                                         .FirstOrDefaultAsync(c => c.Id == id);
            if (cartInDb == null)
            {
                throw new ArgumentNullException();
            }

            _mapper.Map(cartDto, cartInDb);
            await _context.SaveChangesAsync();

            return cartDto;
        }

        public async Task<bool> DeleteCart(int id)
        {
            var cartInDb = await _context.Carts
                                         .Include(c => c.Product)
                                         .Include(c => c.Shop)
                                         .FirstOrDefaultAsync(c => c.Id == id);
            if (cartInDb == null)
            {
                return false;
            }

            _context.Carts.Remove(cartInDb);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
