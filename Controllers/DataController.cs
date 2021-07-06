using DMPackageManager.Website.Database;
using DMPackageManager.Website.Models.Page;
using DMPackageManager.Website.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMPackageManager.Website.Controllers {
    public class DataController : Controller {
        private readonly ILogger<DataController> _logger;
        private readonly DatabaseContext _dbc;

        public DataController(ILogger<DataController> logger, DatabaseContext dbc) {
            _logger = logger;
            _dbc = dbc;
        }
        /// <summary>
        /// Route to get the packages that belong to a user
        /// </summary>
        /// <param name="userName">The username to search for</param>
        /// <returns></returns>
        [Route("user/{username}")]
        public IActionResult GetUserPackages(string userName) {
            // First check if they have a user
            bool userfound = _dbc.users.Where(x => x.username == userName).Any();
            if(userfound) {
                UserPackageList upl = new UserPackageList();
                // Format their username
                upl.package_owner = UserUtil.FormatUser(userName, _dbc);
                return View("UserPackagesList", upl);
            } else {
                return NotFound(String.Format("The user '{0}' could not be found.", userName));
            }
            
        }
    }
}
