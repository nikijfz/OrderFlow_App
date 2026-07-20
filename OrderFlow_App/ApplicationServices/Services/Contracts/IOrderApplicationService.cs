using OrderFlow_App.ApplicationServices.Dtos.OrderDtos;
using OrderFlow_App.Frameworks.ResponseFrameworks.Contracts;
using OrderFlow_App.Models.DomainModels.OrderAggregates;

namespace OrderFlow_App.ApplicationServices.Services.Contracts
{
    public interface IOrderApplicationService
      : IApplicationService<PostOrderDto, PutOrderDto, DeleteOrderDto, GetOrderByIdDto, GetAllOrderDto>
    {
       // Task<IResponse<OrderHeader>> SelectWithDetailsByGuidKeyAsync(Guid guidKey);
    }
}
