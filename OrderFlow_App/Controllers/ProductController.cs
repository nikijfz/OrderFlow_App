using Microsoft.AspNetCore.Mvc;
using OrderFlow_App.ApplicationServices.Dtos.ProductDtos;
using OrderFlow_App.ApplicationServices.Services.Contracts;

namespace OrderFlow_App.Controllers
{
    public class ProductController : Controller
    {
        #region [- Private Fields -]
        private readonly IProductApplicationService _productApplicationService;
        #endregion

        #region [- Ctor() -]
        public ProductController(IProductApplicationService productApplicationService)
        {
            _productApplicationService = productApplicationService;
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
        public async Task<IActionResult> Post([FromBody] PostProductDto postProductDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _productApplicationService.PostAsync(postProductDto);

            if (!result.IsSuccessful)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Value);
        }
        #endregion

        #region [- Put() -]
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] PutProductDto putProductDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _productApplicationService.PutAsync(putProductDto);

            if (!result.IsSuccessful)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Value);
        }
        #endregion

        #region [- Delete() -]
        [HttpDelete]
        public async Task<IActionResult> Delete(DeleteProductDto deleteProductDto)
        {
            var result = await _productApplicationService.DeleteAsync(deleteProductDto);

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
            var response = await _productApplicationService.GetAllAsync();

            return Ok(response.Value);
        }
        #endregion

        #region [- GetById() -]
        [HttpGet]
        public async Task<IActionResult> GetById([FromQuery] GetProductByIdDto getProductByIdDto)
        {
            var response = await _productApplicationService.GetByIdAsync(getProductByIdDto);

            if (!response.IsSuccessful)
            {
                return BadRequest(response.Message);
            }

            return Ok(response.Value);
        }
        #endregion
    }
}
