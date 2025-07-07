
namespace ExpenseManager.BusinessLayer.UserService.UserDTO
{
    public class UsersDTO
    {
        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public DateTime CreateDate { get; set; }

        public int? UpdateBy { get; set; }

        public DateTime? UpdateDate { get; set; }


    }
}
