using OrderFlow_App.Frameworks.ResponseFrameworks.Contracts;
using OrderFlow_App.Models.DomainModels.OrderAggregates;

namespace OrderFlow_App.Models.Services.Contracts
{
    public interface IOrderRepository 
    {
        Task<IResponse<bool>> InsertOrderJsonAsync(string orderJson);
        Task<IResponse<OrderHeader>> UpdateAsync(OrderHeader orderHeader);
        Task<IResponse<OrderHeader>> DeleteAsync(OrderHeader orderHeader);
        Task<IResponse<IEnumerable<OrderHeader>>> SelectAllAsync();
        Task<IResponse<OrderHeader>> SelectByIdAsync(OrderHeader orderHeader);
        Task<IResponse<IEnumerable<OrderDetail>>> SelectDetailsByHeaderIdAsync(Guid headerId);
    }
}

