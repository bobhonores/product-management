using System;

namespace PMSvc.Core
{
    public class DataModel
    {
        public string EventId { get; set; }
        public string DeploymentId { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public decimal? Value { get; set; }
    }
}
