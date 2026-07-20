namespace OrderFlow_App.ApplicationServices.Dtos.OrderDtos
{
    public class OrderDetailDto
    {
        public Guid GuidKey { get; set; }
        public Guid ProductId { get; set; }
        public string ProductTitle { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Amount { get; set; }
    }
}
