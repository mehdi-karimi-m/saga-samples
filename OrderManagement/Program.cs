// See https://aka.ms/new-console-template for more information

using Microsoft.Data.SqlClient;
using NServiceBus.Persistence.Sql;

const string endpointName = "OrdersManagement";
Console.Title = endpointName;
Console.WriteLine($"{endpointName} starting.");

const string connectionString = "Server=192.168.1.1;Initial Catalog=YourDatabaseName;User Id=YourUserName;Password=YourPassword;TrustServerCertificate=True";

//ToDo create tables on sql server
//await RunScripts(connectionString);

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

static async Task RunScripts(string connectionString)
{
    await ScriptRunner.Install(
        sqlDialect: new SqlDialect.MsSqlServer(),
        tablePrefix: "OrdersManagement",
        connectionBuilder: () => new SqlConnection(connectionString),
        scriptDirectory: AppDomain.CurrentDomain.BaseDirectory + "NServiceBus.Persistence.Sql/MsSqlServer",
        shouldInstallOutbox: true,
        shouldInstallSagas: true,
        shouldInstallSubscriptions: true
    );
}