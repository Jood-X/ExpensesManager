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
    public class CategoriesController : ControllerBase
    {
        private readonly WalletManagerDbContext _context;
        private readonly IMapper _mapper;
        public CategoriesController(WalletManagerDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<List<Category>>> GetAll()
        {
            var categories = await _context.Categories
                .Include(c => c.CreateByNavigation)
                .Include(c => c.UpdateByNavigation)
                .ToListAsync();
            var categoriesDTOs = categories.Select(t => _mapper.Map<CategoryDTO>(t)).ToList();
            return Ok(categoriesDTOs);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<List<Category>>> GetByID(int id)
        {
            var category = await _context.Categories
                .Include(c => c.CreateByNavigation)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
                return NotFound();
            return Ok(_mapper.Map<CategoryDTO>(category));
        }

        [HttpPost]
        public async Task<ActionResult<string>> Create(CreateCategoryDTO newCategory)
        {
            if (newCategory == null)
            {
                return BadRequest("Category cannot be null");
            }
            var category = _mapper.Map<Category>(newCategory);
            category.CreateDate = DateTime.UtcNow;
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            string message = $"Category {category.Name} Added Successfully";
            return message;
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<string>> Update(int id, UpdateCategoryDTO updatedCategory)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            category.UpdateDate = DateTime.UtcNow;
            _mapper.Map(updatedCategory, category);
            await _context.SaveChangesAsync();
            string message = $"Category {category.Name} Updated Successfully";
            return message;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<string>> Delete(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            string message = $"Category {category.Name} Deleted Successfully";
            return message;
        }
    }
}
