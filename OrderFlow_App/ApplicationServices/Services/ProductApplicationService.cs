using OrderFlow_App.Frameworks.ResponseFrameworks.Contracts;
using OrderFlow_App.ApplicationServices.Services.Contracts;
using OrderFlow_App.Models.DomainModels.ProductAggregates;
using OrderFlow_App.ApplicationServices.Dtos.ProductDtos;
using OrderFlow_App.Frameworks.ResponseFrameworks;
using OrderFlow_App.Models.Services.Contracts;
using OrderFlow_App.Frameworks;
using System.Net;

namespace OrderFlow_App.ApplicationServices.Services
{
    public class ProductApplicationService : IProductApplicationService
    {
        #region [- Private Fields -]
        private readonly IProductRepository _productRepository;
        #endregion

        #region [- Ctor() -]
        public ProductApplicationService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        #endregion

        #region [- PostAsync() -]
        public async Task<IResponse<PostProductDto>> PostAsync(PostProductDto postProductDto)
        {
            if (postProductDto == null)
            {
                return new Response<PostProductDto>(false, HttpStatusCode.BadRequest, ResponseMessages.Error, null);
            }
            else
            {
                var product = new Product()
                {
                    GuidKey = postProductDto.GuidKey,
                    Title = postProductDto.Title,
                    RecordDescription = postProductDto.RecordDescription,
                    UnitPrice = postProductDto.UnitPrice
                };
                var result = await _productRepository.InsertAsync(product);

                if (!result.IsSuccessful)
                {
                    return new Response<PostProductDto>(false, HttpStatusCode.InternalServerError, ResponseMessages.Error, null);
                }
                else
                {
                    return new Response<PostProductDto>(true, HttpStatusCode.Created, ResponseMessages.SuccessfullOperation, postProductDto);
                }
            }
        }
        #endregion

        #region [- PutAsync() -]
        public async Task<IResponse<PutProductDto>> PutAsync(PutProductDto putProductDto)
        {
            if (putProductDto == null)
            {
                return new Response<PutProductDto>(false, HttpStatusCode.BadRequest, ResponseMessages.Error, null);
            }
            else
            {
                var product = new Product()
                {
                    Id = putProductDto.Id,
                    Title = putProductDto.Title,
                    RecordDescription = putProductDto.RecordDescription,
                    UnitPrice = putProductDto.UnitPrice
                };
                var result = await _productRepository.UpdateAsync(product);

                if (!result.IsSuccessful)
                {
                    return new Response<PutProductDto>(false, HttpStatusCode.InternalServerError, ResponseMessages.Error, null);
                }
                else
                {
                    return new Response<PutProductDto>(true, HttpStatusCode.OK, ResponseMessages.SuccessfullOperation, putProductDto);
                }
            }
        }
        #endregion

        #region [- DeleteAsync() -]
        public async Task<IResponse<DeleteProductDto>> DeleteAsync(DeleteProductDto deleteProductDto)
        {
            if (deleteProductDto == null)
            {
                return new Response<DeleteProductDto>(false, HttpStatusCode.BadRequest, ResponseMessages.Error, null);
            }
            else
            {
                var product = new Product()
                {
                    Id = deleteProductDto.Id
                };
                var result = await _productRepository.DeleteAsync(product);

                if (!result.IsSuccessful)
                {
                    return new Response<DeleteProductDto>(false, HttpStatusCode.InternalServerError, ResponseMessages.Error, null);
                }
                else
                {
                    return new Response<DeleteProductDto>(true, HttpStatusCode.OK, ResponseMessages.SuccessfullOperation, deleteProductDto);
                }
            }
        }
        #endregion

        #region [- GetAllAsync() -]
        public async Task<IResponse<List<GetAllProductDto>>> GetAllAsync()
        {
            var products = await _productRepository.SelectAllAsync();

            if (!products.IsSuccessful || products == null)
            {
                return new Response<List<GetAllProductDto>>(false, HttpStatusCode.NotFound, ResponseMessages.NullInput, null);
            }
            else
            {
                var result = products.Value.Select(p => new GetAllProductDto()
                {
                    Id = p.Id,
                    Title = p.Title,
                    RecordDescription = p.RecordDescription,
                    UnitPrice = p.UnitPrice
                }).ToList();

                return new Response<List<GetAllProductDto>>(true, HttpStatusCode.OK, ResponseMessages.SuccessfullOperation, result);
            }
        }
        #endregion

        #region [- GetByIdAsync() -]
        public async Task<IResponse<GetProductByIdDto>> GetByIdAsync(GetProductByIdDto getProductByIdDto)
        {
            if (getProductByIdDto == null)
            {
                return new Response<GetProductByIdDto>(false, HttpStatusCode.BadRequest, ResponseMessages.Error, null);
            }
            else
            {
                var product = new Product()
                {
                    Id = getProductByIdDto.Id,
                    Title = getProductByIdDto.Title,
                    RecordDescription= getProductByIdDto.RecordDescription,
                    UnitPrice = getProductByIdDto.UnitPrice
                };
                var repositoryDto = await _productRepository.SelectByIdAsync(product);

                if (!repositoryDto.IsSuccessful || repositoryDto == null)
                {
                    return new Response<GetProductByIdDto>(false, HttpStatusCode.NotFound, ResponseMessages.NullInput, null);
                }
                else
                {
                    var productDto = new GetProductByIdDto()
                    {
                        Id = repositoryDto.Value.Id,
                        Title = repositoryDto.Value.Title,
                        RecordDescription = repositoryDto.Value.RecordDescription,
                        UnitPrice = repositoryDto.Value.UnitPrice
                    };

                    return new Response<GetProductByIdDto>(true, HttpStatusCode.OK, ResponseMessages.SuccessfullOperation, productDto);
                }
            }
        }
        #endregion
    }
}
