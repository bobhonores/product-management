using System;
using System.Collections.Generic;

namespace PMSvc.Core
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public string Manufacter { get; set; }
        public string Model { get; set; }
        public string Image { get; set; }
        public List<Review> Reviews { get; set; }
    }
}