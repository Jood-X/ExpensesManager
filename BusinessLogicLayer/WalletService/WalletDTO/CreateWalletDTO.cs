namespace ExpenseManager.BusinessLayer.WalletService.WalletDTO
{
    public class CreateWalletDTO
    {
        public string Name { get; set; } = null!;

        public decimal Balance { get; set; }

    }
}
