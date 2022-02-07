using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMSvc.Api.Models
{
    public static class Reviews
    {
        public class CreateRequest
        {
            public int Rating { get; set; }
            public string Comment { get; set; }
        }

        public class UpdateRequest
        {
            public int Rating { get; set; }
            public string Comment { get; set; }
        }
    }
}
