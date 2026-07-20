using OrderFlow_App.Frameworks.ResponseFrameworks.Contracts;
using OrderFlow_App.Models.DomainModels.CustomerAggregates;
using OrderFlow_App.ApplicationServices.Services.Contracts;
using OrderFlow_App.ApplicationServices.Dtos.CustomerDtos;
using OrderFlow_App.Frameworks.ResponseFrameworks;
using OrderFlow_App.Models.Services.Contracts;
using OrderFlow_App.Frameworks;
using System.Net;

namespace OrderFlow_App.ApplicationServices.Services
{
    public class CustomerApplicationService : ICustomerApplicationService
    {
        #region [- Private Fields -]
        private readonly ICustomerRepository _customerRepository;
        #endregion

        #region [- Ctor() -]
        public CustomerApplicationService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }
        #endregion

        #region [- PostAsync() -]
        public async Task<IResponse<PostCustomerDto>> PostAsync(PostCustomerDto postCustomerDto)
        {
            if (postCustomerDto == null)
            {
                return new Response<PostCustomerDto>(false, HttpStatusCode.BadRequest, ResponseMessages.Error, null);
            }
            else
            {
                var customer = new Customer()
                {
                    GuidKey = postCustomerDto.GuidKey,
                    FirstName = postCustomerDto.FirstName,
                    LastName = postCustomerDto.LastName,
                    Phone = postCustomerDto.Phone
                };
                var result = await _customerRepository.InsertAsync(customer);

                if (!result.IsSuccessful)
                {
                    return new Response<PostCustomerDto>(false, HttpStatusCode.InternalServerError, ResponseMessages.Error, null);
                }
                else
                {
                    return new Response<PostCustomerDto>(true, HttpStatusCode.Created, ResponseMessages.SuccessfullOperation, postCustomerDto);
                }
            }
        }
        #endregion

        #region [- PutAsync() -]
        public async Task<IResponse<PutCustomerDto>> PutAsync(PutCustomerDto putCustomerDto)
        {
            if (putCustomerDto == null)
            {
                return new Response<PutCustomerDto>(false, HttpStatusCode.BadRequest, ResponseMessages.Error, null);
            }
            else
            {
                var customer = new Customer()
                {
                    Id = putCustomerDto.Id,
                    FirstName = putCustomerDto.FirstName,
                    LastName = putCustomerDto.LastName,
                    Phone = putCustomerDto.Phone
                };
                var result = await _customerRepository.UpdateAsync(customer);

                if (!result.IsSuccessful)
                {
                    return new Response<PutCustomerDto>(false, HttpStatusCode.InternalServerError, ResponseMessages.Error, null);
                }
                else
                {
                    return new Response<PutCustomerDto>(true, HttpStatusCode.OK, ResponseMessages.SuccessfullOperation, putCustomerDto);
                }
            }
        }
        #endregion

        #region [- DeleteAsync() -]
        public async Task<IResponse<DeleteCustomerDto>> DeleteAsync(DeleteCustomerDto deleteCustomerDto)
        {
            if (deleteCustomerDto == null)
            {
                return new Response<DeleteCustomerDto>(false, HttpStatusCode.BadRequest, ResponseMessages.Error, null);
            }
            else
            {
                var customer = new Customer()
                {
                    Id = deleteCustomerDto.Id
                };
                var result = await _customerRepository.DeleteAsync(customer);

                if (!result.IsSuccessful)
                {
                    return new Response<DeleteCustomerDto>(false, HttpStatusCode.NotFound, ResponseMessages.NullInput, null);
                }
                else
                {
                    return new Response<DeleteCustomerDto>(true, HttpStatusCode.OK, ResponseMessages.SuccessfullOperation, deleteCustomerDto);
                }
            }
        }
        #endregion

        #region [- GetAllAsync() -]
        public async Task<IResponse<List<GetAllCustomerDto>>> GetAllAsync()
        {
            var customers = await _customerRepository.SelectAllAsync();

            if (!customers.IsSuccessful || customers.Value == null)
            {
                return new Response<List<GetAllCustomerDto>>(false, HttpStatusCode.NotFound, ResponseMessages.NullInput, null);
            }
            else
            {
                var result = customers.Value.Select(customer => new GetAllCustomerDto()
                {
                    Id = customer.Id,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Phone = customer.Phone
                }).ToList();

                return new Response<List<GetAllCustomerDto>>(true, HttpStatusCode.OK, ResponseMessages.SuccessfullOperation, result);
            }
        }
        #endregion

        #region [- GetById() -]
        public async Task<IResponse<GetCustomerByIdDto>> GetByIdAsync(GetCustomerByIdDto getCustomerByIdDto)
        {
            if (getCustomerByIdDto == null)
            {
                return new Response<GetCustomerByIdDto>(false, HttpStatusCode.BadRequest, ResponseMessages.Error, null);
            }
            else
            {
                var customer = new Customer()
                {
                    Id = getCustomerByIdDto.Id,
                    FirstName = getCustomerByIdDto.FirstName,
                    LastName = getCustomerByIdDto.LastName,
                    Phone = getCustomerByIdDto.Phone
                };
                var repositoryResult = await _customerRepository.SelectByIdAsync(customer);

                if (!repositoryResult.IsSuccessful || repositoryResult == null)
                {
                    return new Response<GetCustomerByIdDto>(false, HttpStatusCode.NotFound, ResponseMessages.NullInput, null);
                }
                else
                {
                    var cusomerDto = new GetCustomerByIdDto()
                    {
                        Id = getCustomerByIdDto.Id,
                        FirstName = getCustomerByIdDto.FirstName,
                        LastName = getCustomerByIdDto.LastName,
                        Phone = getCustomerByIdDto.Phone
                    };

                    return new Response<GetCustomerByIdDto>(true, HttpStatusCode.OK, ResponseMessages.SuccessfullOperation, cusomerDto);
                }
            }
        }
        #endregion
    }
}
