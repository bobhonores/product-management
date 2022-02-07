using System;

namespace PMSvc.Core
{
    public class Review
    {
        public Guid Id { get; set; }
        public decimal Rating { get; set; }
        public string Comment { get; set; }
        public DateTimeOffset CreationDate { get; set; }
        public DateTimeOffset? ModificationDate { get; set; }
    }
}