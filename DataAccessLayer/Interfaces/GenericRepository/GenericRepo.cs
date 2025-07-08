using ExpenseManager.DataAccessLayer.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseManager.DataAccessLayer.Interfaces.GenericRepository
{
    public class GenericRepo<T> : IGenericRepo<T> where T : class
    {
        private WalletManagerDbContext _context;
        private DbSet<T> table;

        public GenericRepo(WalletManagerDbContext _context)
        {
            this._context = _context;
            table = _context.Set<T>();

        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await table.ToListAsync();
        }
        public async Task<T?> GetById(int id)
        {
            return await table.FindAsync(id)!;
        }
        public async Task Add(T entity)
        {
            await table.AddAsync(entity);
            await Save();
        }
        public async Task Update(T entity)
        {
            table.Update(entity);
            await Save();
        }
        public async Task Delete(T entity)
        {
            table.Remove(entity);
            await Save();
        }
        public async Task<int> Count()
        {
            return await table.CountAsync();
        }
        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}
