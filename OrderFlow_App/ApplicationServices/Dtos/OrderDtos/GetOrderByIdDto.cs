namespace OrderFlow_App.ApplicationServices.Dtos.OrderDtos
{
    public class GetOrderByIdDto
    {
        public Guid GuidKey { get; set; }
        public Guid SellerId { get; set; }
        public Guid BuyerId { get; set; }
        public string BuyerName { get; set; } 
        public string BuyerPhone { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderDetailDto> Details { get; set; }
    }
}
