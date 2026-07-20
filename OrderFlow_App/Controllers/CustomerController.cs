using Microsoft.AspNetCore.Mvc;
using OrderFlow_App.ApplicationServices.Dtos.CustomerDtos;
using OrderFlow_App.ApplicationServices.Services.Contracts;

namespace OrderFlow_App.Controllers
{
    public class CustomerController : Controller
    {
        #region [- Private Fields -]
        private readonly ICustomerApplicationService _customerApplicationService;
        #endregion

        #region [- Ctor() -]
        public CustomerController(ICustomerApplicationService customerApplicationService)
        {
            _customerApplicationService = customerApplicationService;
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
        public async Task<IActionResult> Post([FromBody] PostCustomerDto postCustomerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _customerApplicationService.PostAsync(postCustomerDto);

            if (!result.IsSuccessful)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Value);
        }
        #endregion

        #region [- Put() -]
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] PutCustomerDto putCustomerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _customerApplicationService.PutAsync(putCustomerDto);

            if (!result.IsSuccessful)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Value);
        }
        #endregion

        #region [- Delete() -]
        [HttpDelete]
        public async Task<IActionResult> Delete(DeleteCustomerDto deleteCustomerDto)
        {
            var result = await _customerApplicationService.DeleteAsync(deleteCustomerDto);

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
            var response = await _customerApplicationService.GetAllAsync();

            return Ok(response.Value);
        }
        #endregion

        #region [- GetById() -]
        [HttpGet]
        public async Task<IActionResult> GetById([FromQuery] GetCustomerByIdDto getCustomerByIdDto)
        {
            var response = await _customerApplicationService.GetByIdAsync(getCustomerByIdDto);

            if (!response.IsSuccessful)
            {
                return BadRequest(response.Message);
            }

            return Ok(response.Value);
        }
        #endregion
    }
}
