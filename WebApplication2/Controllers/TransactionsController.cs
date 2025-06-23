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
    public class TransactionsController: ControllerBase
    {
        private readonly WalletManagerDbContext _context;
        private readonly IMapper _mapper;
        public TransactionsController(WalletManagerDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<List<Transaction>>> GetAll()
        {
            var trans = await _context.Transactions
            .Include(t => t.Wallet)
            .Include(t => t.Category)
            .Include(t => t.CreateByNavigation)
            .Include(t => t.UpdateByNavigation)
            .ToListAsync();
            var transDTOs = trans.Select(t => _mapper.Map<TransactionDTO>(t)).ToList();
            return Ok(transDTOs);
        }        

        [HttpGet("{id}")]
        public async Task<ActionResult<List<Transaction>>> GetByID(int id)
        {
            var transaction = await _context.Transactions
            .Include(t => t.Wallet)
            .Include(t => t.Category)
            .Include(t => t.CreateByNavigation)
            .Include(t => t.UpdateByNavigation)
            .FirstOrDefaultAsync(t => t.Id == id);

            if (transaction == null) return NotFound();
            return Ok(_mapper.Map<TransactionDTO>(transaction));
        }

        [HttpPost]
        public async Task<ActionResult<string>> Create(CreateTransactionDTO newTrans)
        {
            if (newTrans == null)
            {
                return BadRequest("Transaction cannot be null");
            }
            var trans = _mapper.Map<Transaction>(newTrans);
            trans.CreateDate = DateTime.UtcNow;
            _context.Transactions.Add(trans);
            await _context.SaveChangesAsync();
            string message = $"New Transaction Created Successfully";
            return message;
        }
        
        [HttpPut("{id}")]
        public async Task<ActionResult<string>> Update (int id, UpdateTransactionDTO updatedTrans)
        {
            var trans = await _context.Transactions.FindAsync(id);
            if (trans == null)
            {
                return NotFound();
            }
            _mapper.Map(updatedTrans, trans);
            trans.UpdateDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            string message = $"Transacrion Updated Successfully";
            return message;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<string>> Delete(int id)
        {
            var trans = await _context.Transactions.FindAsync(id);
            if (trans == null)
            {
                return NotFound();
            }
            _context.Transactions.Remove(trans);
            await _context.SaveChangesAsync();
            string message = $"Transacrion Deleted Successfully";
            return message;
        }
    }
}
