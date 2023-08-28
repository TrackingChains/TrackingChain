using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using TrackingChain.Substrate.Generic.Client;
using Xunit;

namespace TrackingChain.UnitTest
{
    public class GetTrackTest
    {
        [Fact]
        public async Task TestAsync()
        {
            SubstrateGenericClient substrateGenericClient = new SubstrateGenericClient(Mock.Of<ILogger<SubstrateGenericClient>>(), NullLoggerFactory.Instance);

            await substrateGenericClient.GetTrasactionDataAsync(
                "T-SHIRT-524313", 
                "XqLt7FSZrn8nffSGkRYWZKn4JCWZBvHNH3sTRnQL4qDr2Dp", 
                "wss://shibuya.blastapi.io/c8707e18-84fe-4b15-9665-a2897c0687df",
                1,
                new Common.ExtraInfos.ContractExtraInfo { GetTrackSelectorValue = "0x43f80da1", ByteWeight = 1000000000, InsertTrackProofSize = 1000000000, InsertTrackRefTime = 1000000000, InsertTrackBasicWeight = 1000000000,  },
                CancellationToken.None);
        }
    }
}
