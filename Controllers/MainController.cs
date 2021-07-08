using DMPackageManager.Website.Database;
using DMPackageManager.Website.Models;
using DMPackageManager.Website.Models.Page;
using DMPackageManager.Website.Util;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DMPackageManager.Website.Controllers {
    [Route("")]
    public class MainController : Controller {
        private readonly ILogger<MainController> _logger;
        private readonly IConfiguration _configuration;
        private readonly DatabaseContext _dbc;

        public MainController(ILogger<MainController> logger, IConfiguration configuration, DatabaseContext dbc) {
            _logger = logger;
            _configuration = configuration;
            _dbc = dbc;
        }

        // Simple sign out
        [Route("logout")]
        public IActionResult Logout() {
            HttpContext.SignOutAsync();
            return Redirect("/");
        }

        [Route("")]
        public IActionResult Index() {
            return View();
        }

        [Route("packagespec")]
        public IActionResult PackageSpec() {
            return View("PackageSpec");
        }

        // High quality login handler
        [Authorize]
        [Route("login")]
        public IActionResult Login() {
            UserUtil.Save2DB(UserUtil.UserFromContext(HttpContext), _dbc);
            return Redirect("/");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [Route("error")]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
