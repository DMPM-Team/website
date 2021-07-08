using DMPackageManager.Website.Database;
using DMPackageManager.Website.Models.Database;
using DMPackageManager.Website.Models.Page;
using DMPackageManager.Website.Util;
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
            EditPackageInfo cpi = new EditPackageInfo();
            // We got a post! Begin the validation.
            if (HttpContext.Request.Method == "POST") {
                // Set the model to our answers to preserve them
                cpi.pname = npi.pname;
                cpi.pdesc = npi.pdesc;
                cpi.surl = npi.surl;
                cpi.durl = npi.durl;
                bool success = true;

                cpi.errors = CheckInputValidity(npi, true); // We want to check the name here

                if (cpi.errors.Count > 0) {
                    success = false;
                }

                // Success, now we begin extra validation
                if (success) {
                    if(_dbc.packages.Where(p => p.package_name == npi.pname).Any()) {
                        success = false;
                        cpi.errors.Add("Package name already in use");
                    }

                    // We can now make the package
                    if(success) {
                        Package P = new Package();
                        P.package_name = npi.pname;
                        P.description = npi.pdesc;
                        P.creation_date = DateTime.Now;
                        // We need our user. Do not just assign the context user here. It wont work
                        DatabaseUser temp_user = UserUtil.UserFromContext(HttpContext);
                        DatabaseUser stored_user = _dbc.users.Where(u => u.userId == temp_user.userId).First();
                        P.owner = stored_user;
                        P.documentation_url = npi.durl;
                        P.source_url = npi.surl;
                        _dbc.packages.Add(P);
                        _dbc.SaveChanges(); // Save it
                        // Send them to the page they just made
                        return RedirectToAction(P.package_name, "package");
                    }
                }
                return View("CreatePackage", cpi);
            } else {
                // This wasnt a post, lets just load this page
                return View("CreatePackage", cpi);
            }
            
        }

        [Route("core/edit/{package_name}")]
        [HttpGet]
        [HttpPost]
        public IActionResult EditPackage(string package_name, [FromForm] NewPackageInput npi) {
            // First we need to see if they supplied a package at all
            if(package_name == null) {
                return BadRequest("No package name supplied!");
            }
            // They supplied a package name, see if it exists
            if (!_dbc.packages.Where(p => p.package_name == package_name).Any()) {
                return NotFound(String.Format("The package '{0}' could not be found.", package_name));
            }
            // Lets see if they own it
            if (!_dbc.packages.Where(p => p.package_name == package_name).Where(p => p.owner.userId == UserUtil.UserFromContext(HttpContext).userId).Any()) {
                return Unauthorized(String.Format("You do not own '{0}'!", package_name));
            }
            EditPackageInfo epi = new EditPackageInfo();
            // We got a post! Begin the validation.
            if (HttpContext.Request.Method == "POST") {
                // Set the model to our answers to preserve them
                epi.pname = npi.pname;
                epi.pdesc = npi.pdesc;
                epi.surl = npi.surl;
                epi.durl = npi.durl;
                bool success = true;
                // Re-validate incase they edited it maliciously

                // First we need to see if they supplied a package at all
                if (npi.pname == null) {
                    return BadRequest("No package name supplied!");
                }
                // They supplied a package name, see if it exists
                if (!_dbc.packages.Where(p => p.package_name == npi.pname).Any()) {
                    return NotFound(String.Format("The package '{0}' could not be found.", npi.pname));
                }
                // Lets see if they own it
                if (!_dbc.packages.Where(p => p.package_name == npi.pname).Where(p => p.owner.userId == UserUtil.UserFromContext(HttpContext).userId).Any()) {
                    return Unauthorized(String.Format("You do not own '{0}'!", npi.pname));
                }

                epi.errors = CheckInputValidity(npi, true); // We want to check the name here

                if (epi.errors.Count > 0) {
                    success = false;
                }

                // Success, now we save the edit
                if (success) {
                    Package P = _dbc.packages.Where(p => p.package_name == npi.pname).First();
                    P.description = npi.pdesc;
                    P.source_url = npi.surl;
                    P.documentation_url = npi.durl;
                    _dbc.SaveChanges();
                    // Tell them the edit saved
                    epi.success = true;
                }
                return View("EditPackage", epi);
            } else {
                // This wasnt a post, lets just load this page
                // Assign our existing values to it
                Package P1 = _dbc.packages.Where(p => p.package_name == package_name).First();
                epi.pname = P1.package_name;
                epi.pdesc = P1.description;
                epi.surl = P1.source_url;
                epi.durl = P1.documentation_url;
                return View("EditPackage", epi);
            }

        }

        [Route("core/createrelease/{package_name}")]
        [HttpGet]
        [HttpPost]
        public IActionResult CreateNewRelease(string package_name) {
            // First we need to see if they supplied a package at all
            if (package_name == null) {
                return BadRequest("No package name supplied!");
            }
            // They supplied a package name, see if it exists
            if (!_dbc.packages.Where(p => p.package_name == package_name).Any()) {
                return NotFound(String.Format("The package '{0}' could not be found.", package_name));
            }
            // Lets see if they own it
            if (!_dbc.packages.Where(p => p.package_name == package_name).Where(p => p.owner.userId == UserUtil.UserFromContext(HttpContext).userId).Any()) {
                return Unauthorized(String.Format("You do not own '{0}'!", package_name));
            }
            // Standardise the name
            Package P = _dbc.packages.Where(p => p.package_name == package_name).First();
            NewRelease nr = new NewRelease();
            nr.package_name = P.package_name;
            return View("CreateRelease", nr);
        }

        /// <summary>
        /// Checks a <see cref="NewPackageInput">NewPackageInput</see> to see if it matches criteria.
        /// </summary>
        /// <param name="npi">The <see cref="NewPackageInput">NewPackageInput</see> to check</param>
        /// <param name="check_name">Whether we want to check the name or not</param>
        /// <returns>A <see cref="List{string}">string list</see> of errors, if any</returns>
        List<string> CheckInputValidity(NewPackageInput npi, bool check_name) {
            List<string> errors = new List<string>();
            // All the checks for the name
            if (check_name && String.IsNullOrWhiteSpace(npi.pname)) {
                errors.Add("Package name cannot be empty");
            } else {
                if (npi.pname.Length < 4) {
                    errors.Add("Package name is too short (Must be atleast 4 characters)");
                }
                if (npi.pname.Length > 32) {
                    errors.Add("Package name is too long (Must be 32 characters or less)");
                }
                // Verify characters
                string converted_name = Regex.Replace(npi.pname, @"[^a-z0-9\-_]", "");
                if (converted_name != npi.pname) {
                    // Its an example of why its bad, not a bad example per say
                    string bad_example = Regex.Replace(npi.pname, @"[^a-z0-9\-_]", "#");
                    errors.Add("Package name contains invalid characters. Said characters have been replaced with hashtags in the following line");
                    errors.Add(bad_example);
                }

            }

            // All the checks for the description 
            if (String.IsNullOrWhiteSpace(npi.pdesc)) {
                errors.Add("Package description cannot be empty");
            } else {
                if (npi.pdesc.Length < 8) {
                    errors.Add("Package description is too short (Must be atleast 8 characters)");
                }
                if (npi.pdesc.Length > 256) {
                    errors.Add("Package description is too long (Must be 256 characters or less)");
                }
                // Verify characters
                string converted_desc = Regex.Replace(npi.pdesc, @"[^\w  \.]", "");
                if (converted_desc != npi.pdesc) {
                    // Its an example of why its bad, not a bad example per say
                    string bad_example = Regex.Replace(npi.pdesc, @"[^\w \.]", "#");
                    errors.Add("Package description contains invalid characters. Said characters have been replaced with hashtags in the following line");
                    errors.Add(bad_example);
                }
            }

            // Source URL
            if (!String.IsNullOrWhiteSpace(npi.surl)) {
                Uri result;
                if (!(Uri.TryCreate(npi.surl, UriKind.Absolute, out result) && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps))) {
                    errors.Add("Source code URL is not a valid URL");
                }
            }

            // Documenation URL
            if (!String.IsNullOrWhiteSpace(npi.durl)) {
                Uri result;
                if (!(Uri.TryCreate(npi.durl, UriKind.Absolute, out result) && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps))) {
                    errors.Add("Documentation URL is not a valid URL");
                }
            }

            return errors;
        }
    }
}
