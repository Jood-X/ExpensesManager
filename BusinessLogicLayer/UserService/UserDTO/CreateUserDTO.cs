using System.ComponentModel.DataAnnotations;

namespace ExpenseManager.BusinessLayer.UserService.UserDTO
{
    public class CreateUserDTO
    {
        [Required] public string Name { get; set; } = string.Empty;

        [Required] public string Email { get; set; } = string.Empty;

        [Required] public string Password { get; set; } = string.Empty;

        public string? ClientUri { get; set; }

    }
}
