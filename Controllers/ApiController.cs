using DMPackageManager.Website.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DMPackageManager.Api.Models;
using DMPackageManager.Website.Models.Database;

namespace DMPackageManager.Website.Controllers {
    [Route("api")]
    public class ApiController : Controller {
        private readonly ILogger<ApiController> _logger;
        private readonly DatabaseContext _dbc;

        public ApiController(ILogger<ApiController> logger, DatabaseContext dbc) {
            _logger = logger;
            _dbc = dbc;
        }

        [Route("package/{package_name}")]
        public IActionResult GetPackageMeta(string package_name) {
            // Make sure they supplied a name
            if(String.IsNullOrWhiteSpace(package_name)) {
                return BadRequest("No package name supplied");
            }
            // Make sure the package exists
            if(!_dbc.packages.Where(p => p.package_name == package_name).Any()) {
                return NotFound(String.Format("The package {0} could not be found", package_name));
            }
            Package P = _dbc.packages.Where(p => p.package_name == package_name).First();
            // Make sure some releases exist
            if (!_dbc.package_releases.Where(r => r.package == P).Any()) {
                return NotFound(String.Format("The package {0} has no releases", package_name));
            }
            // Ok we have some releases, and package info. Lets go!
            List<PackageVersion> versions = _dbc.package_releases.Where(r => r.package == P).OrderBy(r => r.id).ToList();

            // And now our model
            PackageSpec PS = new PackageSpec();
            PS.package_name = P.package_name;
            PS.package_desc = P.description;
            PS.release_date = P.creation_date;
            PS.update_date = _dbc.package_releases.Where(r => r.package == P).OrderByDescending(r => r.release_date).First().release_date;
            PS.latest_version = _dbc.package_releases.Where(r => r.package == P).OrderByDescending(r => r.release_date).First().version;
            PS.all_versions = new List<string>();
            foreach(PackageVersion PV in versions) {
                PS.all_versions.Add(PV.version);
            }
            return Ok(PS);
        }
    }
}
