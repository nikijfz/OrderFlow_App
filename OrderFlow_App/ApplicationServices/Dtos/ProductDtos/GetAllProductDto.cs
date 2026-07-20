namespace OrderFlow_App.ApplicationServices.Dtos.ProductDtos
{
    public class GetAllProductDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string RecordDescription { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
