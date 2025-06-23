using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.DTO;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletsController : ControllerBase
    {
        private readonly WalletManagerDbContext _context;
        private readonly IMapper _mapper;
        public WalletsController(WalletManagerDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<List<Wallet>>> GetAll()
        {
            var wallets = await _context.Wallets
                .Include(w => w.CreateByNavigation)
                .Include(w => w.UpdateByNavigation)
                .ToListAsync();
            var walletDTOs = wallets.Select(wallet => _mapper.Map<WalletDTO>(wallet)).ToList();
            return Ok(walletDTOs);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<List<Wallet>>> GetByID(int id)
        {
            var wallet = await _context.Wallets
                .Include(w => w.CreateByNavigation)
                .Include(w => w.UpdateByNavigation)
                .FirstOrDefaultAsync(w => w.Id == id);
            
            if (wallet == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<WalletDTO>(wallet));
        }

        [HttpPost]
        public async Task<ActionResult<string>> Create(CreateWalletDTO newWallet)
        {
            if (newWallet == null)
            {
                return BadRequest("Wallet cannot be null");
            }
            var wallet = _mapper.Map<Wallet>(newWallet);
            wallet.CreateDate = DateTime.UtcNow;

            _context.Wallets.Add(wallet);
            await _context.SaveChangesAsync();
            string message = $"New {wallet.Name} Wallet Added Successfully";
            return message;
        }


        [HttpPut("{id}")]
        public async Task<ActionResult<string>> Update(int id, UpdateWalletDTO updatedWallet)
        {
            var wallet = await _context.Wallets.FindAsync(id);
            if (wallet == null)
            {
                return NotFound();
            }
            _mapper.Map(updatedWallet, wallet);
            wallet.UpdateDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            string message = $"Wallet {wallet.Name} Updated Successfully";
            return message;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<string>> Delete(int id)
        {
            var wallet = await _context.Wallets.FindAsync(id);
            if (wallet == null)
            {
                return NotFound();
            }
            _context.Wallets.Remove(wallet);
            await _context.SaveChangesAsync();
            string message = $"Wallet {wallet.Name} Deleted Successfully";
            return message;
        }
    }
}
