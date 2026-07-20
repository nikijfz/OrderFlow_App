namespace OrderFlow_App.ApplicationServices.Dtos.OrderDtos
{
    public class PostOrderDto
    {
        public Guid GuidKey { get; set; }
        public Guid SellerId { get; set; }
        public Guid BuyerId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderDetailDto> Details { get; set; }
    }
}
