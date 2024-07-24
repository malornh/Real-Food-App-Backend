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

        public CartsService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CartDto>> GetCarts()
        {
            var carts = await _context.Carts.ToListAsync();
            return _mapper.Map<List<CartDto>>(carts);
        }

        public async Task<CartDto> GetCart(int id)
        {
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.Id == id);
            return _mapper.Map<CartDto>(cart);
        }

        public async Task<IEnumerable<CartDto>> GetCartsByUserId(string userId)
        {
            var carts = await _context.Carts
                                      .Include(c => c.Product)
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

        public async Task<bool> UpdateCart(int id, CartDto cartDto)
        {
            if (id != cartDto.Id)
            {
                return false;
            }

            var cartInDb = await _context.Carts.FirstOrDefaultAsync(c => c.Id == id);
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
            var cartInDb = await _context.Carts.FirstOrDefaultAsync(c => c.Id == id);
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
