using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseManager.DataAccessLayer.Interfaces.GenericRepository
{
    public interface IGenericRepo<T> where T : class
    {
        Task<IEnumerable<T>> GetAll();
        Task<T?> GetById(int id);
        Task Add(T entity);
        Task Update(T entity);
        Task Delete(T entity);
        Task<int> Count();
        Task Save();
    }
}
