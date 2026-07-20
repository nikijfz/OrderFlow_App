using OrderFlow_App.Models.Frameworks;

namespace OrderFlow_App.Models.DomainModels.OrderAggregates
{
    public class OrderDetail : IDbSetEntity
    {
        public Guid Id { get; set; }
        public Guid GuidKey { get; set; }
        public Guid OrderHeaderId { get; set; }
        public Guid ProductId { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Amount { get; set; }
        public bool IsDeleted { get; set; }
    }
}
