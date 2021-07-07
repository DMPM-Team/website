using DMPackageManager.Website.Database;
using DMPackageManager.Website.Models.Page;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DMPackageManager.Website.Controllers {
    [Authorize]
    public class PackageManagementController : Controller {
        private readonly ILogger<PackageManagementController> _logger;
        private readonly DatabaseContext _dbc;

        public PackageManagementController(ILogger<PackageManagementController> logger, DatabaseContext dbc) {
            _logger = logger;
            _dbc = dbc;
        }

        [Route("core/createnew")]
        [HttpGet]
        [HttpPost]
        public IActionResult CreatePackage([FromForm] NewPackageInput npi) {
            CreatePackageInfo cpi = new CreatePackageInfo();
            // We got a post! Begin the validation.
            if (HttpContext.Request.Method == "POST") {
                // Set the model to our answers to preserve them
                cpi.pname = npi.pname;
                cpi.pdesc = npi.pdesc;
                cpi.surl = npi.surl;
                cpi.durl = npi.durl;
                bool success = true;
                
                // All the checks for the name
                if(String.IsNullOrWhiteSpace(npi.pname)) {
                    success = false;
                    cpi.errors.Add("Package name cannot be empty");
                } else {
                    if(npi.pname.Length < 4) {
                        success = false;
                        cpi.errors.Add("Package name is too short (Must be atleast 4 characters)");
                    }
                    if (npi.pname.Length > 32) {
                        success = false;
                        cpi.errors.Add("Package name is too long (Must be 32 characters or less)");
                    }
                    // Verify characters
                    string converted_name = Regex.Replace(npi.pname, @"[^a-z0-9\-_]", "");
                    if(converted_name != npi.pname) {
                        success = false;
                        // Its an example of why its bad, not a bad example per say
                        string bad_example = Regex.Replace(npi.pname, @"[^a-z0-9\-_]", "#");
                        cpi.errors.Add("Package name contains invalid characters. Said characters have been replaced with hashtags in the following line");
                        cpi.errors.Add(bad_example);
                    }

                }

                // All the checks for the description 
                if (String.IsNullOrWhiteSpace(npi.pdesc)) {
                    success = false;
                    cpi.errors.Add("Package description cannot be empty");
                } else {
                    if (npi.pdesc.Length < 8) {
                        success = false;
                        cpi.errors.Add("Package description is too short (Must be atleast 4 characters)");
                    }
                    if (npi.pdesc.Length > 256) {
                        success = false;
                        cpi.errors.Add("Package description is too long (Must be 256 characters or less)");
                    }
                    // Verify characters
                    string converted_desc = Regex.Replace(npi.pdesc, @"[^\w  \.]", "");
                    if (converted_desc != npi.pdesc) {
                        success = false;
                        // Its an example of why its bad, not a bad example per say
                        string bad_example = Regex.Replace(npi.pdesc, @"[^\w \.]", "#");
                        cpi.errors.Add("Package description contains invalid characters. Said characters have been replaced with hashtags in the following line");
                        cpi.errors.Add(bad_example);
                    }
                }

                // Source URL
                if(!String.IsNullOrWhiteSpace(npi.surl)) {
                    Uri result;
                    if(!(Uri.TryCreate(npi.surl, UriKind.Absolute, out result) && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps))) {
                        success = false;
                        cpi.errors.Add("Source code URL is not a valid URL");
                    }
                }

                // Documenation URL
                if (!String.IsNullOrWhiteSpace(npi.durl)) {
                    Uri result;
                    if (!(Uri.TryCreate(npi.durl, UriKind.Absolute, out result) && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps))) {
                        success = false;
                        cpi.errors.Add("Documentation URL is not a valid URL");
                    }
                }
                
                // Success, now we begin extra validation
                if(success) {
                    if(_dbc.packages.Where(p => p.package_name == npi.pname).Any()) {
                        success = false;
                        cpi.errors.Add("Package name already in use");
                    }
                }
                return View("CreatePackage", cpi);
            } else {
                // This wasnt a post, lets just load this page
                return View("CreatePackage", cpi);
            }
            
        }
    }
}
