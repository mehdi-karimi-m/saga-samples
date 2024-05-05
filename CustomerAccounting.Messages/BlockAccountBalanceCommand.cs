namespace CustomerAccounting.Messages;

public class BlockAccountBalanceCommand : ICommand
{
    public int AccountId { get; set; }
    public long OrderId { get; set; }
    public decimal Amount { get; set; }
}