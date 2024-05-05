namespace CustomerAccounting.Messages;

public class AccountBalanceBlockingFailed : IEvent
{
    public int AccountId { get; set; }
    public decimal BlockingAmount { get; set; }
    public string Reason { get; set; }
    public long RequestId { get; set; }
}