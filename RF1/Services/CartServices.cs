using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RF1.Data;
using RF1.Dtos;
using RF1.Models;
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

        public CartsService(DataContext context, IMapper mapper, IProductsService productService, IShopsService shopService)
        {
            _context = context;
            _mapper = mapper;
            _productService = productService;
            _shopService = shopService;
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

        public async Task<IEnumerable<CartDto>> GetCartsByUserId(string userId)
        {
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

        public async Task<CartDto> CreateCart(int productId, int shopId, string userId)
        {
            // Fetch the product and shop to avoid multiple instances
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return null; // or handle not found
            }

            var shop = await _context.Shops.FindAsync(shopId);
            if (shop == null)
            {
                return null; // or handle not found
            }

            // Create a new Cart instance and attach existing entities
            var cart = new Cart
            {
                UserId = userId,
                Product = product,
                Shop = shop
            };

            // Add the new cart instance to the context
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();

            // Map the created cart back to DTO
            var cartDto = _mapper.Map<CartDto>(cart);
            return cartDto;
        }

        public async Task<bool> UpdateCart(int id, CartDto cartDto)
        {
            if (id != cartDto.Id)
            {
                return false;
            }

            var cartInDb = await _context.Carts
                                         .Include(c => c.Product)
                                         .Include(c => c.Shop)
                                         .FirstOrDefaultAsync(c => c.Id == id);
            if (cartInDb == null)
            {
                return false;
            }

            _mapper.Map(cartDto, cartInDb);
            await _context.SaveChangesAsync();

            return true;
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
