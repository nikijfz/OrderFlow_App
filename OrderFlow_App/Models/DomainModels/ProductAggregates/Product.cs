using OrderFlow_App.Models.Frameworks;

namespace OrderFlow_App.Models.DomainModels.ProductAggregates
{
    public class Product : IDbSetEntity
    {
        public Guid Id { get; set; }
        public Guid GuidKey { get; set; }
        public string Title { get; set; }
        public string RecordDescription { get; set; }
        public decimal UnitPrice { get; set; }
        public bool IsDeleted { get; set; }
    }
}
