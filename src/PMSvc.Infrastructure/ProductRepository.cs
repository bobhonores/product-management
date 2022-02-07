using Microsoft.EntityFrameworkCore;
using PMSvc.Core;
using PMSvc.Core.Exceptions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PMSvc.Infrastructure
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductDbContext _dbContext;

        public ProductRepository(ProductDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(Product product, CancellationToken cancellationToken)
        {
            await _dbContext.Products.AddAsync(product, cancellationToken);
        }

        public async Task<Product> GetById(Guid id, CancellationToken cancellationToken)
        {
            var product = await _dbContext.Products.Include(p => p.Reviews)
                                    .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

            if (product == null)
            {
                throw new NotFoundException("product doesn't exist");
            }

            return product;
        }

        public Task Save(CancellationToken cancellationToken)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
