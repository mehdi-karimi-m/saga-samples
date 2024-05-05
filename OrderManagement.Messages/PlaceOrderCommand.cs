namespace OrderManagement.Messages;

public class PlaceOrderCommand : ICommand
{
    public long OrderId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public string TraderId { get; set; }
}