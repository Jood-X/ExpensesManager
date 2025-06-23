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
    public class UsersController : ControllerBase
    {
        private readonly WalletManagerDbContext _context;
        private readonly IMapper _mapper;
        public UsersController(WalletManagerDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        
        [HttpGet]
        public async Task<ActionResult<List<User>>> GetAll()
        {
            var users = await _context.Users
                .Include(u => u.UpdateByNavigation)
                .ToListAsync();
            return Ok(users.Select(user => _mapper.Map<UserDTO>(user)));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<List<User>>> GetByID(int id)
        {
            var user = await _context.Users
                .Include(u => u.UpdateByNavigation)
                .FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<UserDTO>(user));
        }

        [HttpPost]
        public async Task<ActionResult<string>> Create(CreateUserDTO newUser) 
        {
            var user = _mapper.Map<User>(newUser);
            user.CreateDate = DateTime.UtcNow;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            string message = $"User {user.Name} Added Successfully";
            return message;
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<string>> Update(int id, UpdateUserDTO updatedUser) 
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            _mapper.Map(updatedUser, user);
            user.UpdateDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            string message = $"User {user.Name} Updated Successfully";
            return message;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<string>> Delete(int id) 
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            string message = $"User {user.Name} Deleted Successfully";
            return message;
        }
    }
}
