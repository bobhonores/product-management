namespace PMSvc.Api.Models
{
    public static class Products
    {
        public class CreateRequest
        {
            public string Name { get; set; }
            public string Brand { get; set; }
            public string Manufacter { get; set; }
            public string Model { get; set; }
            public string Image { get; set; }
        }

        public class UpdateRequest
        {
            public string Name { get; set; }
            public string Brand { get; set; }
            public string Manufacter { get; set; }
            public string Model { get; set; }
            public string Image { get; set; }
        }
    }
}