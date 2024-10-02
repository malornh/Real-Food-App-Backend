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

            if (cart == null) throw new ArgumentNullException($"Cart with id {id} not found.");

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

        public async Task<CartDto> CreateCart(int orderId)
        {
            var userId = _userAccessorService.GetUserId();

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null) throw new ArgumentNullException("Order not found.");

            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == order.ProductId);
            if (product == null) throw new ArgumentNullException($"Order's product with id {order.ProductId} not found.");

            var shop = await _context.Shops.FirstOrDefaultAsync(s => s.Id == order.ShopId);
            if (shop == null) throw new ArgumentNullException($"Order's shop with id {order.ShopId} not found.");


            var cart = new Cart
            {
                UserId = userId,
                Product = product,
                Shop = shop
            };

            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();

            return _mapper.Map<CartDto>(cart);
        }

        [HttpPut("{id}")]
        public async Task<CartDto> UpdateCart(int id, int quantity)
        {
            var userId = _userAccessorService.GetUserId();

            var cartInDb = await _context.Carts
                                         .Include(c => c.Product)
                                         .Include(c => c.Shop)
                                         .FirstOrDefaultAsync(c => c.Id == id);

            if (cartInDb == null) throw new ArgumentNullException($"Cart item with id {id} not found.");
            if (cartInDb.UserId != userId) throw new UnauthorizedAccessException("User cannot edit another user's cart.");

            cartInDb.Quantity = quantity;

            await _context.SaveChangesAsync();

            return _mapper.Map<CartDto>(cartInDb);
        }

        public async Task DeleteCart(int id)
        {
            var userId = _userAccessorService.GetUserId();

            var cartInDb = await _context.Carts
                                         .Include(c => c.Product)
                                         .Include(c => c.Shop)
                                         .FirstOrDefaultAsync(c => c.Id == id);

            if (cartInDb == null) throw new ArgumentNullException($"Cart with id {id} not found.");
            if (cartInDb.UserId != userId) throw new UnauthorizedAccessException("User cannot delete another user's cart.");

            _context.Carts.Remove(cartInDb);
            await _context.SaveChangesAsync();
        }
    }
}
