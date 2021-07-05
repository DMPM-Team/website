using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMPackageManager.Website.Controllers {
    [Route("api")]
    public class ApiController : Controller {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ApiController> _logger;

        public ApiController(ILogger<ApiController> logger, IConfiguration configuration) {
            _logger = logger;
            _configuration = configuration;
        }

        public IActionResult Index() {
            return Ok();
        }
    }
}
