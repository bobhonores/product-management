using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMSvc.Core
{
    public interface IProductQueryRepository
    {
        Task<Product> GetById(Guid id);
    }
}
