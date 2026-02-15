using LearningAzureScalableAPI.Contracts;
using LearningAzureScalableAPI.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace LearningAzureScalableAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<OrdersController> _logger;
        private readonly HttpClient _httpClient;
        public OrdersController(IOrderRepository orderRepository, ILogger<OrdersController> logger, HttpClient httpClient)
        {
            _orderRepository = orderRepository;
            _logger = logger;
            _httpClient = httpClient;
        }

        [HttpGet("GetOrderById/{id}")]
        public async Task<ActionResult<OrderDTO>> MyOrderById(int id)
        {
            try
            {
                _logger.LogInformation("Order API called with id {Id}", id);
                return await _orderRepository.GetOrderById(id);
            }
            catch (Exception ex)
            {
                _logger.LogError("Getting error in MyOrderId in " + ex);
                return null;
            }
        }

        [HttpGet]
        public async Task<string> GetAppName()
        {
            try
            {
                _logger.LogInformation("Get Application Name method is start to running.");
                string appname = await _orderRepository.GetApplicationName();
                _logger.LogInformation("Getting the AppName" + appname);
                return appname;
            }
            catch (Exception ex)
            {
                _logger.LogError("Getting error in GetApp Name" + ex);
                return "";
            }
        }

        [HttpGet("age")]
        public async Task<IActionResult> GetAge()
        {
            var response = _httpClient.GetStringAsync("https://api.agify.io/?name=arpit");
            return Ok(response);
        }

        [HttpGet("Product")]
        public async Task<IActionResult> GetProduct()
        {
            List<string> st = new List<string>();
            st.Add("Apple");
            st.Add("Mango");
            st.Add("Orange");
            return Ok(st);

        }
    }
}
