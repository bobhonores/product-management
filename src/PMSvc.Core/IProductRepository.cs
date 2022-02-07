using System;
using System.Threading;
using System.Threading.Tasks;

namespace PMSvc.Core
{
    public interface IProductRepository
    {
        Task Add(Product product, CancellationToken cancellationToken);
        Task Save(CancellationToken cancellationToken);
        Task<Product> GetById(Guid id, CancellationToken cancellationToken);
    }
}