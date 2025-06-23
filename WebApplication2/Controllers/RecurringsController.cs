using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebApplication2.Data;
using WebApplication2.DTO;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecurringsController : ControllerBase
    {
        private readonly WalletManagerDbContext _context;
        private readonly IMapper _mapper;
        public RecurringsController(WalletManagerDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<List<RecurringExpense>>> GetAll()
        {
            var recurrings = await _context.RecurringExpenses
            .Include(r => r.Wallet)
            .Include(r => r.Category)
            .Include(r => r.CreateByNavigation)
            .Include(r => r.UpdateByNavigation)
            .ToListAsync();
            var recurringsDTOs = recurrings.Select(t => _mapper.Map<RecurringExpenseDTO>(t)).ToList();
            return Ok(recurringsDTOs);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<List<RecurringExpense>>> GetByID(int id)
        {
            var recurring = await _context.RecurringExpenses
            .Include(r => r.Wallet)
            .Include(r => r.Category)
            .Include(r => r.CreateByNavigation)
            .Include(r => r.UpdateByNavigation)
            .FirstOrDefaultAsync(r => r.Id == id);

            if (recurring == null) return NotFound();
            return Ok(_mapper.Map<RecurringExpenseDTO>(recurring));
        }

        [HttpPost]
        public async Task<ActionResult<string>> Create(CreateRecurringExpenseDTO newRecurring)
        {
            if (newRecurring == null)
            {
                return BadRequest("Transaction cannot be null");
            }
            var recurring = _mapper.Map<RecurringExpense>(newRecurring);
            recurring.CreateDate = DateTime.UtcNow;
            _context.RecurringExpenses.Add(recurring);
            await _context.SaveChangesAsync();
            string message = $"New Recurring Added Successfully";
            return message;
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<string>> Update(int id, UpdateRecurringExpenseDTO updatedRecurring)
        {
            var recurring = await _context.RecurringExpenses.FindAsync(id);
            if (recurring == null)
            {
                return NotFound();
            }
            _mapper.Map(updatedRecurring, recurring);
            recurring.UpdateDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            string message = $"Recurring Updated Successfully";
            return message;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<string>> Delete(int id)
        {
            var recurring = await _context.RecurringExpenses.FindAsync(id);
            if (recurring == null)
            {
                return NotFound();
            }
            _context.RecurringExpenses.Remove(recurring);
            await _context.SaveChangesAsync();
            string message = $"Recurring Deleted Successfully";
            return message;
        }
    }
}
