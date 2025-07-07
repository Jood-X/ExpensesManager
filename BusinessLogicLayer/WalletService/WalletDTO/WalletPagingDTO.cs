namespace ExpenseManager.BusinessLayer.WalletService.WalletDTO
{
    public class WalletPagingDTO
    {
        public List<WalletsDTO> Wallets { get; set; } = new List<WalletsDTO>();
        public int Pages { get; set; }
        public int CurrentPage { get; set; }
    }
}
