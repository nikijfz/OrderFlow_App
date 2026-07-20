using OrderFlow_App.Frameworks.ResponseFrameworks.Contracts;
using OrderFlow_App.Models.DomainModels.CustomerAggregates;
using OrderFlow_App.ApplicationServices.Services.Contracts;
using OrderFlow_App.Models.DomainModels.ProductAggregates;
using OrderFlow_App.Models.DomainModels.OrderAggregates;
using OrderFlow_App.ApplicationServices.Dtos.OrderDtos;
using OrderFlow_App.Frameworks.ResponseFrameworks;
using OrderFlow_App.Models.Services.Contracts;
using OrderFlow_App.Frameworks;
using System.Text.Json;
using System.Net;

namespace OrderFlow_App.ApplicationServices.Services
{
    public class OrderApplicationService : IOrderApplicationService
    {
        #region [- Private Fields -]
        private readonly IOrderRepository _orderRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IProductRepository _productRepository;
        #endregion

        #region [- Ctor() -]
        public OrderApplicationService(IOrderRepository orderRepository,ICustomerRepository customerRepository, IProductRepository productRepository) 
        {
            _orderRepository = orderRepository;
            _customerRepository = customerRepository;
            _productRepository = productRepository;
        }
        #endregion

        #region [- PostAsync() -]
        public async Task<IResponse<PostOrderDto>> PostAsync(PostOrderDto postOrderDto)
        {
            if (postOrderDto == null || postOrderDto.Details == null || !postOrderDto.Details.Any())
            {
                return new Response<PostOrderDto>(false, HttpStatusCode.BadRequest, ResponseMessages.Error, null);
            }
            else
            {
                var orderJson = JsonSerializer.Serialize(postOrderDto);
                var result = await _orderRepository.InsertOrderJsonAsync(orderJson);

                if (!result.IsSuccessful)
                {
                    return new Response<PostOrderDto>(false, HttpStatusCode.InternalServerError, ResponseMessages.Error, null);
                }
                else
                {
                    return new Response<PostOrderDto>(true, HttpStatusCode.Created, ResponseMessages.SuccessfullOperation, postOrderDto);
                }
            }
        }
        #endregion

        #region [- PutAsync() -]
        public async Task<IResponse<PutOrderDto>> PutAsync(PutOrderDto putOrderDto)
        {
            if (putOrderDto == null)
            {
                return new Response<PutOrderDto>(false, HttpStatusCode.BadRequest, ResponseMessages.Error, null);
            }
            else
            {
                var orderHeader = new OrderHeader()
                {
                    GuidKey = putOrderDto.GuidKey,
                    SellerId = putOrderDto.SellerId,
                    BuyerId = putOrderDto.BuyerId,
                    OrderDate = putOrderDto.OrderDate,
                    TotalAmount = putOrderDto.TotalAmount
                };

                var result = await _orderRepository.UpdateAsync(orderHeader);

                if (!result.IsSuccessful)
                {
                    return new Response<PutOrderDto>(false, HttpStatusCode.InternalServerError, ResponseMessages.Error, null);
                }
                else
                {
                    return new Response<PutOrderDto>(true, HttpStatusCode.OK, ResponseMessages.SuccessfullOperation, putOrderDto);
                }
            }
        }
        #endregion

        #region [- DeleteAsync() -]
        public async Task<IResponse<DeleteOrderDto>> DeleteAsync(DeleteOrderDto deleteOrderDto)
        {
            if (deleteOrderDto == null)
            {
                return new Response<DeleteOrderDto>(false, HttpStatusCode.BadRequest, ResponseMessages.Error, null);
            }
            else
            {
                var orderHeader = new OrderHeader()
                {
                    GuidKey = deleteOrderDto.GuidKey
                };
                var result = await _orderRepository.DeleteAsync(orderHeader);

                if (!result.IsSuccessful)
                {
                    return new Response<DeleteOrderDto>(false, HttpStatusCode.NotFound, ResponseMessages.NullInput, null);
                }
                else
                {
                    return new Response<DeleteOrderDto>(true, HttpStatusCode.OK, ResponseMessages.SuccessfullOperation, deleteOrderDto);
                }
            }
        }
        #endregion

        #region [- GetAllAsync() -]
        public async Task<IResponse<List<GetAllOrderDto>>> GetAllAsync()
        {
            var orders = await _orderRepository.SelectAllAsync();
            var customers = await _customerRepository.SelectAllAsync();

            if (!orders.IsSuccessful || orders.Value == null)
            {
                return new Response<List<GetAllOrderDto>>(false, HttpStatusCode.NotFound, ResponseMessages.NullInput, null);
            }
            else
            {
                var result = (from order in orders.Value
                              join customer in customers.Value on order.BuyerId equals customer.Id into customerGroup
                              from c in customerGroup.DefaultIfEmpty()
                              select new GetAllOrderDto
                              {
                                  GuidKey = order.GuidKey,
                                  OrderDate = order.OrderDate,
                                  TotalAmount = order.TotalAmount,
                                  BuyerName = c != null ? c.FirstName + " " + c.LastName : "N/A"
                              }).ToList();

                return new Response<List<GetAllOrderDto>>(true, HttpStatusCode.OK, ResponseMessages.SuccessfullOperation, result);
            }
        }
        #endregion

        #region [- GetByIdAsync() -]
        public async Task<IResponse<GetOrderByIdDto>> GetByIdAsync(GetOrderByIdDto getOrderByIdDto)
        {
            if (getOrderByIdDto == null || getOrderByIdDto.GuidKey == Guid.Empty)
            {
                return new Response<GetOrderByIdDto>(false, HttpStatusCode.BadRequest, ResponseMessages.Error, null);
            }

            var orderHeaderEntity = new OrderHeader { GuidKey = getOrderByIdDto.GuidKey };
            var repositoryResult = await _orderRepository.SelectByIdAsync(orderHeaderEntity);

            if (!repositoryResult.IsSuccessful || repositoryResult.Value == null)
            {
                return new Response<GetOrderByIdDto>(false, HttpStatusCode.NotFound, ResponseMessages.DataNotFound, null);
            }

            var order = repositoryResult.Value;

            // Load Customer
            var customersResult = await _customerRepository.SelectAllAsync();
            Customer customer = null;
            if (customersResult.IsSuccessful && customersResult.Value != null)
            {
                customer = customersResult.Value.FirstOrDefault(c => c.Id == order.BuyerId);
            }

            // Load Details
            var detailsResult = await _orderRepository.SelectDetailsByHeaderIdAsync(order.Id);
            var orderDetails = Enumerable.Empty<OrderDetail>();
            if (detailsResult.IsSuccessful && detailsResult.Value != null)
            {
                orderDetails = detailsResult.Value;
            }

            // Load Products
            var productsResult = await _productRepository.SelectAllAsync();
            var products = Enumerable.Empty<Product>();
            if (productsResult.IsSuccessful && productsResult.Value != null)
            {
                products = productsResult.Value;
            }

            // Prepare Details List
            var detailsList = orderDetails.Select(d => {
                var product = products.FirstOrDefault(p => p.Id == d.ProductId);
                return new OrderDetailDto
                {
                    GuidKey = d.GuidKey,
                    ProductId = d.ProductId,
                    ProductTitle = product != null ? product.Title : "N/A",
                    UnitPrice = d.UnitPrice,
                    Amount = d.Amount
                };
            }).ToList();

            var orderDto = new GetOrderByIdDto()
            {
                GuidKey = order.GuidKey,
                SellerId = order.SellerId,
                BuyerId = order.BuyerId,
                BuyerName = customer != null ? customer.FirstName + " " + customer.LastName : "N/A",
                BuyerPhone = customer != null ? customer.Phone : "N/A",
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Details = detailsList
            };

            return new Response<GetOrderByIdDto>(true, HttpStatusCode.OK, ResponseMessages.SuccessfullOperation, orderDto);
        }
        #endregion
    }
}