using OrderFlow_App.ApplicationServices.Dtos.CustomerDtos;

namespace OrderFlow_App.ApplicationServices.Services.Contracts
{
    public interface ICustomerApplicationService
        : IApplicationService<PostCustomerDto, PutCustomerDto, DeleteCustomerDto, GetCustomerByIdDto, GetAllCustomerDto>
    {

    }
}
