namespace OrderManagement.API.Controllers
{
    /// <summary>
    /// Home controller for OrderManagement API.
    /// </summary>    
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            Console.WriteLine($"HomeController được khởi tạo");
        }

        /// <summary>
        /// Gets the welcome message for the API.
        /// </summary>
        /// <returns>A welcome message.</returns>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("OrderManagement API");
        }
    }
}
