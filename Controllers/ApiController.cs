using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMPackageManager.Website.Controllers {
    [Route("api")]
    public class ApiController : Controller {
        public IActionResult Index() {
            return Ok("TODO: API Routes");
        }
    }
}
