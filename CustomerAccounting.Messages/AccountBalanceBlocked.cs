namespace CustomerAccounting.Messages;

public class AccountBalanceBlocked : IEvent
{
    public long RequestId { get; set; }
    public int AccountId { get; set; }
    public decimal Amount { get; set; }
}