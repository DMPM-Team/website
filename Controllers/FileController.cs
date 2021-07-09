using DMPackageManager.Website.Database;
using DMPackageManager.Website.Models.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DMPackageManager.Website.Controllers {
    public class FileController : Controller {
        private readonly ILogger<FileController> _logger;
        private readonly DatabaseContext _dbc;

        public FileController(ILogger<FileController> logger, DatabaseContext dbc) {
            _logger = logger;
            _dbc = dbc;
        }

        [Route("download/{package_name}/{package_version}")]
        public IActionResult GetPackageZIP(string package_name, string package_version) {
            if (String.IsNullOrWhiteSpace(package_name)) {
                return BadRequest("Package name not supplied");
            }
            if (String.IsNullOrWhiteSpace(package_version)) {
                return BadRequest("Package version not supplied");
            }

            // Check for package
            if (!_dbc.packages.Where(p => p.package_name == package_name).Any()) {
                return NotFound(String.Format("The pacakge {0} could not be found.", package_name));
            }

            // Now check for releases
            Package P = _dbc.packages.Where(p => p.package_name == package_name).First();
            if(!_dbc.package_releases.Where(r => r.package == P).Where(r => r.version == package_version).Any()) {
                return NotFound(String.Format("Version {0} of pacakge {1} could not be found.", package_version, package_name));
            }

            PackageVersion PV = _dbc.package_releases.Where(r => r.package == P).Where(r => r.version == package_version).First();
            byte[] file_bytes = System.IO.File.ReadAllBytes(String.Format("App_Data/Package_Data/{0}/{1}.zip", P.id, PV.id));

            return File(file_bytes, "application/force-download", String.Format("{0}-{1}.zip", P.package_name, PV.version));
        }
    }
}
