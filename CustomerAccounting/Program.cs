// See https://aka.ms/new-console-template for more information

using Microsoft.Data.SqlClient;

const string endpointName = "CustomersAccounting";
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

Console.WriteLine("Press enter to exit.");
Console.ReadLine();

await endpointInstance.Stop();