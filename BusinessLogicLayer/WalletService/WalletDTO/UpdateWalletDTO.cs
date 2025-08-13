namespace ExpenseManager.BusinessLayer.WalletService.WalletDTO
{
    public class UpdateWalletDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; } = null!;

        public decimal? Balance { get; set; }

    }
}
