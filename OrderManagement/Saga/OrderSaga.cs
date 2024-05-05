using CustomerAccounting.Messages;
using OrderManagement.Messages;

namespace OrderManagement.Saga;

public class OrderSaga : Saga<OrderSagaData>,
                         IAmStartedByMessages<PlaceOrderCommand>,
                         IHandleMessages<AccountBalanceBlocked>, 
                         IHandleMessages<AccountBalanceBlockingFailed>,
                         IHandleTimeouts<OrderExpired>
{
    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<OrderSagaData> mapper)
    {
        mapper.MapSaga(saga => saga.OrderId)
            .ToMessage<PlaceOrderCommand>(placeOrderCommand => placeOrderCommand.OrderId)
            .ToMessage<AccountBalanceBlocked>(accountBalanceBlocked => accountBalanceBlocked.RequestId)
            .ToMessage<AccountBalanceBlockingFailed>(accountBalanceBlockingFailed => 
                accountBalanceBlockingFailed.RequestId);
    }

    public async Task Handle(PlaceOrderCommand message, IMessageHandlerContext context)
    {
        Console.WriteLine("Place order command received.");

        await context.Send("CustomersAccounting",new BlockAccountBalanceCommand
        {
            AccountId = GetTraderAccountId(message.TraderId),
            OrderId = message.OrderId,
            Amount = message.Price * message.Quantity
        });
        
        // await RequestTimeout(context, TimeSpan.FromMinutes(5), new OrderExpired
        // {
        //     OrderId = message.OrderId
        // });
    }

    private int GetTraderAccountId(string traderId)
    {
        return traderId.Length;
    }

    public Task Handle(AccountBalanceBlocked message, IMessageHandlerContext context)
    {
        Console.WriteLine($"Customer account id: {message.AccountId} blocked for order id: {message.RequestId}");
        MarkAsComplete();
        return Task.CompletedTask;
    }

    public Task Handle(AccountBalanceBlockingFailed message, IMessageHandlerContext context)
    {
        Console.WriteLine($"Blocking customer account id: {message.AccountId} for order id: {message.RequestId} failed, because of {message.Reason}");
        MarkAsComplete();
        return Task.CompletedTask;
    }

    public Task Timeout(OrderExpired state, IMessageHandlerContext context)
    {
        //Remove order from sending list.
        //Don't assume that other messages haven't arrived in the meantime.
        //If required, a Saga can store boolean flags in the SagaData and then check these flags to confirm that
        //an incoming timeout message should be processed based on the current state.
        Console.WriteLine($"Order id: {state.OrderId} expired.");
        MarkAsComplete();
        return Task.CompletedTask;
    }
}