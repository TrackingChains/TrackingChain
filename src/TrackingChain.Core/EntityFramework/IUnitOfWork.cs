using System.Threading.Tasks;

namespace TrackingChain.TrackingChainCore.EntityFramework
{
    public interface IUnitOfWork
    {
        Task SaveChangesAsync();
    }
}
