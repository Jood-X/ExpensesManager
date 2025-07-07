using ExpenseManager.DataAccessLayer.Interfaces.GenericRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseManager.BusinessLayer.GenericService
{
    public class GenericService<T> : IGenericService<T> where T : class
    {
        private readonly IGenericRepo<T> _repo;
        public GenericService(IGenericRepo<T> repo)
        {
            _repo = repo;
        }
        public void Add(T entity)
        {
            _repo.Add(entity);
        }

        public Task<int> Count()
        {
            return _repo.Count();
        }

        public void Delete(T entity)
        {
            _repo.Delete(entity);
        }

        public Task<IEnumerable<T>> GetAll()
        {
            return _repo.GetAll();
        }

        public Task<T?> GetById(int id)
        {
            return _repo.GetById(id);
        }

        public void Save()
        {
            _repo.Save();
        }

        public void Update(T entity)
        {
            _repo.Update(entity);
        }
    }
}
