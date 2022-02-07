using LazyCache;
using Microsoft.Extensions.Caching.Memory;
using PMSvc.Core;
using PMSvc.Core.Exceptions;
using SqlKata.Execution;
using System;
using System.Threading.Tasks;

namespace PMSvc.Infrastructure
{
    public class ProductQueryRepository : IProductQueryRepository
    {
        private readonly QueryFactory _queryFactory;

        private readonly IAppCache _cache;

        public ProductQueryRepository(QueryFactory queryFactory, IAppCache cache)
        {
            _queryFactory = queryFactory;

            _cache = cache;
        }

        public async Task<Product> GetById(Guid id)
        {
            Func<Task<dynamic>> productGetter = () => _queryFactory.Query("Products")
                                                        .WhereRaw("lower(Id) = lower(?)", id.ToString())
                                                        .Select("Id", "Name", "Brand", "Manufacter", "Model", "Image")
                                                        .FirstOrDefaultAsync();

            var product = await _cache.GetOrAddAsync(id.ToString(), productGetter, new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(5) });

            if (product == null)
            {
                throw new NotFoundException("product doesn't exist");
            }

            return new Product 
            { 
                Id = Guid.Parse(product.Id), 
                Name = product.Name,
                Brand = product.Brand,
                Manufacter = product.Manufacter,
                Model = product.Model,
                Image = product.Image
            };
        }
    }
}
