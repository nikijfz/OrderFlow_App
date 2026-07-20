using OrderFlow_App.ApplicationServices.Dtos.ProductDtos;

namespace OrderFlow_App.ApplicationServices.Services.Contracts
{
    public interface IProductApplicationService
         : IApplicationService<PostProductDto, PutProductDto, DeleteProductDto, GetProductByIdDto, GetAllProductDto>
    {
    }
}
