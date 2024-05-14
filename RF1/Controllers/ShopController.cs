using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RF1.Data;
using RF1.Models;
using RF1.Dtos; // Assuming Dtos folder exists
using AutoMapper;
using System.Threading.Tasks;

namespace RF1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public ShopController(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Shops
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShopDto>>> GetShops()
        {
            var shops = await _context.Shops.ToListAsync();
            return Ok(_mapper.Map<List<ShopDto>>(shops));
        }

        // GET: api/Shops/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ShopDto>> GetShop(int id)
        {
            var shop = await _context.Shops.FindAsync(id);

            if (shop == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<ShopDto>(shop));
        }

        // GET: api/Shops/ByUser/5
        [HttpGet("ByUser/{userId}")]
        public async Task<ActionResult<IEnumerable<ShopDto>>> GetShopsByUserId(string userId)
        {
            var shops = await _context.Shops
                .Where(s => s.UserId == userId)
                .ToListAsync();

            if (shops == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<List<ShopDto>>(shops));
        }

        [HttpGet("{shopId}/OrdersWithFarms")]
        public async Task<ActionResult<Dtos.ShopFullInfoDto>> GetShopOrdersWithFarms(int shopId)
        {
            // Fetch shop orders with farms using LINQ query
            var shopOrdersWithFarms = await _context.Orders
                .Where(o => o.ShopId == shopId)
                .Select(o => new OrderFarmDto
                {
                    Id = o.Id,
                    Quantity = o.Quantity,
                    ShopPrice = o.ShopPrice, // Assuming this is the shop's price for the product
                    Product = new ShortProductDto
                    {
                        Id = o.Product.Id,
                        Name = o.Product.Name,
                        Description = o.Product.Description,
                        Type = o.Product.Type,
                        PricePerUnit = o.Product.PricePerUnit,
                        UnitOfMeasurement = o.Product.UnitOfMeasurement,
                        Image = o.Product.Image
                    },
                    ShortFarm = new ShortFarmDto
                    {
                        Id = o.Product.FarmId,
                        Name = o.Product.Farm.Name,
                        Image = o.Product.Farm.Image
                    }
                })
                .ToListAsync();

            // Fetch shop details
            var shopDetails = await _context.Shops
                .Where(s => s.Id == shopId)
                .Select(s => new ShopFullInfoDto
                {
                    Id = s.Id,
                    Image = s.Image,
                    UserId = s.UserId,
                    Name = s.Name,
                    Latitude = s.Latitude,
                    Longitude = s.Longitude,
                    Rating = s.Rating,
                    Orders = shopOrdersWithFarms
                })
                .FirstOrDefaultAsync();

            // Check if any data is returned
            if (shopDetails == null)
            {
                return NotFound("No data found for the shop");
            }

            return Ok(shopDetails);
        }












        // PUT: api/Shops/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutShop(int id, ShopDto shopDto)
        {
            if (id != shopDto.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var shopInDb = await _context.Shops.FirstOrDefaultAsync(s => s.Id == id);
            if (shopInDb == null)
            {
                return NotFound();
            }

            _mapper.Map(shopDto, shopInDb);

            _context.Entry(shopInDb).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShopExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Shops
        [HttpPost]
        public async Task<ActionResult<ShopDto>> PostShop(ShopDto shopDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var shop = _mapper.Map<Shop>(shopDto);

            _context.Shops.Add(shop);
            await _context.SaveChangesAsync();

            shopDto.Id = shop.Id;

            return CreatedAtAction("GetShop", new { id = shopDto.Id }, shopDto);
        }

        // DELETE: api/Shops/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShop(int id)
        {
            var shop = await _context.Shops.FindAsync(id);
            if (shop == null)
            {
                return NotFound();
            }

            _context.Shops.Remove(shop);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ShopExists(int id)
        {
            return _context.Shops.Any(e => e.Id == id);
        }
    }
}
