namespace OrderFlow_App.ApplicationServices.Dtos.OrderDtos
{
    public class GetAllOrderDto
    {
        public Guid GuidKey { get; set; }
        public string SellerName { get; set; }
        public string BuyerName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
