namespace ExpenseManager.BusinessLayer.AuthorizationService.AuthDTO
{
    public class RefreshTokenRequestDTO
    {
        public int id { get; set; }

        public required string RefreshToken { get; set; }
    }
}
