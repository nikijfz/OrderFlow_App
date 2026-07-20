namespace OrderFlow_App.ApplicationServices.Dtos.CustomerDtos
{
    public class GetCustomerByIdDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
    }
}
