using OrderFlow_App.Models.Frameworks;

namespace OrderFlow_App.Models.DomainModels.OrderAggregates
{
    public class OrderHeader : IDbSetEntity
    {
        public Guid Id { get; set; }
        public Guid GuidKey { get; set; }
        public Guid SellerId { get; set; }
        public Guid BuyerId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsDeleted { get; set; }
    }
}
