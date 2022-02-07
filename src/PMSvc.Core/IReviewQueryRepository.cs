using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMSvc.Core
{
    public interface IReviewQueryRepository
    {
        Task<IEnumerable<Review>> GetAllByProductId(Guid productId);
    }
}
