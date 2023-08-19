using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackingChain.TransactionMonitorCore.UseCases
{
    public interface IAlertUseCase
    {
        Task<bool> RunAsync(int max);
    }
}
