using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Yaml;
using Microsoft.VisualBasic;
using Schnorrkel.Keys;
using Serilog;
using Shibuya.Integration;
using Shibuya.Integration.Client;
using Shibuya.Integration.Helper;
using Substrate.NetApi;
using Substrate.NetApi.Model.Types;
using Substrate.NetApi.Model.Types.Metadata.V14;
using System.Numerics;
using System.Text;

namespace Shibuya.Client
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            // configure serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo
                .Console()
                .CreateLogger();

            var config = new ConfigurationBuilder()
                // this will be used more later on
                .SetBasePath(AppContext.BaseDirectory)
                // I chose using YML files for my config data as I am familiar with them
                .AddYamlFile("config.yml")
                .Build();

            // Add this to your C# console app's Main method to give yourself
            // a CancellationToken that is canceled when the user hits Ctrl+C.
            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (s, e) =>
            {
                Console.WriteLine("Canceling...");
                cts.Cancel();
                e.Cancel = true;
            };

            try
            {
                Console.WriteLine("Press Ctrl+C to end.");

                await RunClientAsync(config, cts.Token);
            }
            catch (OperationCanceledException)
            {
                // This is the normal way we close.
            }

            // Finally, once just before the application exits...
            await Log.CloseAndFlushAsync();
        }

        private static async Task RunClientAsync(IConfigurationRoot config, CancellationToken token)
        {

            var miniSecret = new MiniSecret(Utils.HexToByteArray("0xe5be9a5092b81bca64be81d212e7f2f9eba183bb7a90954f7b76361f6edb5c0a"), ExpandMode.Ed25519);
            var account = Account.Build(KeyType.Sr25519, miniSecret.ExpandToSecret().ToBytes(), miniSecret.GetPair().Public.Key);

            Log.Information("Your address: {address}", Utils.GetAddressFrom(account.Bytes, 5));
            
            var url = config["node:live"];
            var client = new ShibuyaNetwork(account, url);

            if (!await client.ConnectAsync(true, true, token))
            {
                Log.Error("Failed to connect to node");
                return;
            }

            Log.Information("Connected to {url}: {flag}", url, client.IsConnected);

            var blockNumber = await client.GetBlocknumberAsync(token);
            Log.Information("Current block is {number}", blockNumber != null ? blockNumber.Value : null);
            
            // do action in here ...
            Thread.Sleep(3000);

            var smartContracAddress = "aKpb5m5WBvTA164EdZhkYHU1SHBixY4QPnxbekMDUSfUYGd";
            var dest = Utils.GetPublicKeyFrom(smartContracAddress).ToAccountId32();
            var value = new BigInteger(0);
            var refTime = (ulong)3951114240;
            var proofSize = (ulong)125952;
            var storageDepositLimit = new BigInteger(54000000000);
            var data = Utils.HexToByteArray("0x1ba63d86363617270650000000000000000000000000000000000000000000000000000014616161616100");
            var subscriptionId = await client.ContractsCallAsync(dest, value, refTime, proofSize, storageDepositLimit, data, 1, token);
            if (subscriptionId != null)
            {
                Log.Information("SubscriptionId: {subscriptionId}", subscriptionId);
                var queueInfo = client.ExtrinsicManger.Get(subscriptionId);
                while (queueInfo != null && !queueInfo.IsCompleted)
                {
                    Log.Information("QueueInfo {subscription} [{state}]", subscriptionId, queueInfo != null ? queueInfo.State.ToString() : queueInfo);
                    Thread.Sleep(1000);
                    queueInfo = client.ExtrinsicManger.Get(subscriptionId);
                }
            }
            else
            {
                Log.Error("Failed to call contract");
            }


            await client.DisconnectAsync();
            Log.Information("Disconnected from {url}", url);
        }
    }
}