using DMPackageManager.Website.Database;
using DMPackageManager.Website.Models.Database;
using DMPackageManager.Website.Models.Page;
using DMPackageManager.Website.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        [HttpGet]
        public IActionResult GetUserPackages(string userName, string searchQuery = "", [FromQuery(Name = "p")] int page = 1) {
            // First check if they have a user
            bool userfound = _dbc.users.Where(x => x.username == userName).Any();
            if(userfound) {
                UserPackageList upl = new UserPackageList();
                upl.raw_username = userName;
                // Format their username
                upl.package_owner = UserUtil.FormatUser(userName, _dbc);
                upl.packages = FilterPackages(userName, searchQuery, page);
                if (UserUtil.IsLoggedIn(HttpContext) && (UserUtil.UserFromContext(HttpContext).username == upl.package_owner)) {
                    upl.own_packages = true;
                }
                return View("UserPackagesList", upl);
            } else {
                return NotFound(String.Format("The user '{0}' could not be found.", userName));
            }
            
        }

        /// <summary>
        /// Helper to get packages and paginate them
        /// </summary>
        /// <param name="userName">The username to filter by, if any</param>
        /// <param name="searchQuery">The package name to filter by, if any</param>
        /// <param name="page">Which page to retrieve</param>
        /// <returns></returns>
        public PaginatedList<Package> FilterPackages(string userName = null, string searchQuery = null, int page = 1) {
            int pageSize = 20; // Tweak this as needed
            int offset = (pageSize * page) - pageSize;
            DbSet<Package> query = _dbc.packages;
            if(!String.IsNullOrEmpty(userName)) {
                query.Where(p => p.owner.userId == UserUtil.Name2ID(userName, _dbc));
            }

            if(!String.IsNullOrEmpty(searchQuery)) {
                query.Where(p => p.package_name.Contains(searchQuery));
            }

            return PaginatedList<Package>.Create(query.AsNoTracking(), page, pageSize);
        }
    }
}
