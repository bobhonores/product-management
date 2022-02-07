using System.Threading;
using System.Threading.Tasks;

namespace PMSvc.Core
{
    public interface IExternalDataModelProvider
    {
        Task<DataModel> Get(Product product, CancellationToken cancellationToken);
    }
}
