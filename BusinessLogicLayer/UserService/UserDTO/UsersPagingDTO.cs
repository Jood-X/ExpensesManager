namespace ExpenseManager.BusinessLayer.UserService.UserDTO
{
    public class UsersPagingDTO
    {
        public List<UsersDTO> Users { get; set; } = new List<UsersDTO>();
        public int Pages { get; set; }
        public int CurrentPage { get; set; }
        
    }
}
