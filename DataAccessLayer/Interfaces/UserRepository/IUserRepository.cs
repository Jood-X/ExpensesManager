using ExpenseManager.DataAccessLayer.Entities;
using ExpenseManager.DataAccessLayer.Interfaces.GenericRepository;

namespace ExpenseManager.DataAccessLayer.Interfaces.UserRepository
{
    public interface IUserRepository: IGenericRepo<User>
    {
        Task<int> GetAllUsersCount();
        Task<IEnumerable<User>> GetAllUsersAsync(int pageResult, int page = 1);
        Task<User?> GetUserByIdAsync(int userId);
    }
}
