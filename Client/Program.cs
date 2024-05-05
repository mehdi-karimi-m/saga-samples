// See https://aka.ms/new-console-template for more information

using Microsoft.Data.SqlClient;
using OrderManagement.Messages;

const string endpointName = "Client";
Console.WriteLine($"{endpointName} starting.");
Console.Title = endpointName;

const string connectionString = "Server=192.168.1.1;Initial Catalog=YourDatabaseName;User Id=YourUserName;Password=YourPassword;TrustServerCertificate=True";
var endpointConfiguration = new EndpointConfiguration(endpointName);
var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
transport.ConnectionString("host=192.168.1.1;UserName=your-user-name;password=your-password");
transport.UseConventionalRoutingTopology(QueueType.Quorum);

endpointConfiguration.UseSerialization<SystemJsonSerializer>();

var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
persistence.SqlDialect<SqlDialect.MsSqlServer>();
persistence.ConnectionBuilder(connectionBuilder: () => new SqlConnection(connectionString));
var subscriptions = persistence.SubscriptionSettings();
subscriptions.CacheFor(TimeSpan.FromMinutes(1));

var endpointInstance = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

Console.WriteLine("Press enter to start sending order command.");

while (true)
{
    Console.ReadLine();
    var orderId = new Random(Guid.NewGuid().GetHashCode()).Next(1, 1000);
    await endpointInstance.Send("OrdersManagement", new PlaceOrderCommand
    {
        OrderId = orderId,
        Price = new Random(Guid.NewGuid().GetHashCode()).Next(1, 10000),
        Quantity = new Random(Guid.NewGuid().GetHashCode()).Next(1, 100000),
        TraderId = Guid.NewGuid().ToString()
    });

    Console.WriteLine($"Place order command with order id: {orderId} sent.");
}