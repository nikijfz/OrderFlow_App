using OrderFlow_App.Models.Frameworks;

namespace OrderFlow_App.Models.DomainModels.CustomerAggregates
{
    public class Customer : IDbSetEntity
    {
        public Guid Id { get; set; }
        public Guid GuidKey { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public bool IsDeleted { get; set; }
    }
}
