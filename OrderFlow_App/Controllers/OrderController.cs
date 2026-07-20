using Microsoft.AspNetCore.Mvc;
using OrderFlow_App.ApplicationServices.Dtos.OrderDtos;
using OrderFlow_App.ApplicationServices.Services.Contracts;

namespace OrderFlow_App.Controllers
{
    public class OrderController : Controller
    {
        #region [- Private Fields -]
        private readonly IOrderApplicationService _orderApplicationService;
        #endregion

        #region [- Ctor() -]
        public OrderController(IOrderApplicationService orderApplicationService)
        {
            _orderApplicationService = orderApplicationService;
        }
        #endregion

        #region [- Index() -]
        public IActionResult Index()
        {
            return View();
        }
        #endregion

        #region [- Post() -]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PostOrderDto postOrderDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _orderApplicationService.PostAsync(postOrderDto);

            if (!result.IsSuccessful)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Value);
        }
        #endregion

        #region [- Put() -]
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] PutOrderDto putOrderDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _orderApplicationService.PutAsync(putOrderDto);

            if (!result.IsSuccessful)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Value);
        }
        #endregion

        #region [- Delete() -]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] DeleteOrderDto deleteOrderDto)
        {
            var result = await _orderApplicationService.DeleteAsync(deleteOrderDto);

            if (!result.IsSuccessful)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Value);
        }
        #endregion

        #region [- GetAll() -]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _orderApplicationService.GetAllAsync();

            if (!response.IsSuccessful)
            {
                return BadRequest(response.Message);
            }

            return Ok(response.Value);
        }
        #endregion

        #region [- GetById() -]
        [HttpGet]
        public async Task<IActionResult> GetById([FromQuery] GetOrderByIdDto getOrderByIdDto)
        {
            var response = await _orderApplicationService.GetByIdAsync(getOrderByIdDto);

            if (!response.IsSuccessful)
            {
                return BadRequest(response.Message);
            }

            return Ok(response.Value);
        }
        #endregion
    }
}
