using AutoMapper;
using ExpenseManager.BusinessLayer.UserService.UserDTO;
using ExpenseManager.DataAccessLayer.Entities;
using ExpenseManager.DataAccessLayer.Interfaces.UserRepository;
using Microsoft.Extensions.Configuration;

namespace ExpenseManager.BusinessLayer.UserService
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private DateTime date = DateTime.UtcNow;

        public UserService(IUserRepository userRepository, IMapper mapper, IConfiguration config)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper;
            _config = config;
        }

        public async Task<UsersPagingDTO> GetAllUsersAsync(string? searchTerm, int page = 1)
        {
            var pageSizeSetting = _config["AppSettings:PageSize"];
            if (string.IsNullOrEmpty(pageSizeSetting))
            {
                throw new InvalidOperationException("Page size setting is not configured.");
            }

            if (!int.TryParse(pageSizeSetting, out var pageResult))
            {
                throw new InvalidOperationException("Page size setting is not a valid integer.");
            }

            var totalUsersCount = await _userRepository.GetAllUsersCount();
            var pageCount = (int)Math.Ceiling(totalUsersCount / (double)pageResult);

            var users = await _userRepository.GetAllUsersAsync(pageResult, page);

            var response = new UsersPagingDTO
            {
                Users = users.Select(user => _mapper.Map<UsersDTO>(user)).ToList(),
                Pages = pageCount,
                CurrentPage = page
            };
            return response;
        }

        public async Task<UsersDTO> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                throw new InvalidOperationException($"User with ID {id} not found.");
            }
            return _mapper.Map<UsersDTO>(user);
        }

        public async Task<UsersDTO> CreateUserAsync(CreateUserDTO newUser)
        {
            var user = _mapper.Map<User>(newUser);
            user.CreateDate = date;
            await _userRepository.Add(user);
            var createdUser= _mapper.Map<UsersDTO>(user);
            return createdUser;
        }
        public async Task<bool> UpdateUserAsync(UpdateUserDTO updatedUser)
        {
            var user = new User();
            _mapper.Map(updatedUser, user);

            var existingUser = await _userRepository.GetUserByIdAsync(user.Id);

            if (existingUser == null)
            {
                throw new Exception($"User with ID {user.Id} not found or could not be updated.");
            }
            _mapper.Map(updatedUser, existingUser);
            existingUser.UpdateDate = date;

            await _userRepository.Update(existingUser);
            return true;
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException("User not found or you do not have permission to delete this user.");
            }
            await _userRepository.Delete(user);
            return true;
        }
    }
}
