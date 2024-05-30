using System.Threading.Tasks;

namespace MiniErp.Core.UnitOfWorks
{
    public interface IUnitOfWork
    {
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
