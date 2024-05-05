using CustomerAccounting.Messages;

namespace CustomerAccounting.Handlers;

public class BlockAccountBalanceCommandHandler : IHandleMessages<BlockAccountBalanceCommand>
{
    public async Task Handle(BlockAccountBalanceCommand message, IMessageHandlerContext context)
    {
        if (message.Amount > 100000)
            await context.Publish(new AccountBalanceBlockingFailed
            {
                AccountId = message.AccountId,
                RequestId = message.OrderId,
                Reason = "Balance is not enough.",
                BlockingAmount = message.Amount
            });

        await context.Publish(new AccountBalanceBlocked
        {
            Amount = message.Amount,
            AccountId = message.AccountId,
            RequestId = message.OrderId
        });
    }
}