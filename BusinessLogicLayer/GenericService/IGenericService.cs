using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseManager.BusinessLayer.GenericService
{
    public interface IGenericService<T> where T : class
    {
        Task<IEnumerable<T>> GetAll();
        Task<T?> GetById(int id);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<int> Count();
        void Save();
    }
}
