using PMSvc.Core;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMSvc.Infrastructure
{
    public class ReviewQueryRepository : IReviewQueryRepository
    {
        private readonly QueryFactory _queryFactory;

        public ReviewQueryRepository(QueryFactory queryFactory)
        {
            _queryFactory = queryFactory;
        }

        public async Task<IEnumerable<Review>> GetAllByProductId(Guid productId)
        {
            var reviews = await _queryFactory.Query("Reviews")
                                        .WhereRaw("lower(ProductId) = lower(?)", productId.ToString())
                                        .Select("Id", "Rating", "Comment", "CreationDate", "ModificationDate")
                                        .GetAsync();

            return reviews?.Select(r => new Review 
            { 
                Id = Guid.Parse(r.Id),
                Rating = r.Rating,
                Comment = r.Comment,
                CreationDate = DateTimeOffset.Parse(r.CreationDate),
                ModificationDate = !string.IsNullOrWhiteSpace(r.ModificationDate) ? DateTimeOffset.Parse(r.ModificationDate) : null
            })?.ToList();
        }
    }
}
