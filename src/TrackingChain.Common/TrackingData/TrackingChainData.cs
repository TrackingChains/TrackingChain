using System.Collections.Generic;

namespace TrackingChain.Common.Dto
{
    public class TrackingChainData
    {
        public string Code { get; set; } = default!;
        public IEnumerable<DataDetail> DataValues { get; set; } = default!;
    }
}
