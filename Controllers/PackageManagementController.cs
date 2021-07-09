using DMPackageManager.Website.Database;
using DMPackageManager.Website.Models.Database;
using DMPackageManager.Website.Models.Page;
using DMPackageManager.Website.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using nClam;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.IO.Compression;
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
        public IActionResult CreateNewRelease(string package_name, [FromForm] ReleaseInput ri) {
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

            // We got a post! Begin the validation.
            if (HttpContext.Request.Method == "POST") {
                // Set the model to our answers to preserve them
                nr.vtag = ri.vtag;
                nr.rnotes = ri.rnotes;

                bool success = true;
                if (String.IsNullOrWhiteSpace(ri.rnotes)) {
                    nr.errors.Add("Release notes must not be blank");
                    return View("CreateRelease", nr);
                } else {
                    if(ri.rnotes.Length < 16) {
                        nr.errors.Add("Release notes must be more than 16 characters");
                        return View("CreateRelease", nr);
                    }
                    if (ri.rnotes.Length > 8192) {
                        nr.errors.Add("Release notes must be less than 8192 characters");
                        return View("CreateRelease", nr);
                    }
                }

                if (String.IsNullOrWhiteSpace(ri.vtag)) {
                    nr.errors.Add("Version tag must not be blank");
                    return View("CreateRelease", nr);
                }

                // Trim whitespace
                ri.vtag = ri.vtag.Trim();

                // We parse the version to a Version operator to enforce semantic versioning. If this fails to parse, we can assume the user didnt use semver
                Version v;
                string semver_string = "";
                try {
                    v = Version.Parse(ri.vtag);
                    semver_string = v.ToString();
                } catch(Exception) {
                    nr.errors.Add("Version tag must be in semver format.");
                    return View("CreateRelease", nr);
                }


                // Make sure they didnt already use that revision
                if (_dbc.package_releases.Where(r => r.package == P).Where(r => r.version == semver_string).Any()) {
                    nr.errors.Add("Version tag already in use");
                    return View("CreateRelease", nr);
                }


                if (ri.packagezip == null) {
                    nr.errors.Add("Package file was not uploaded");
                    success = false;
                } else {
                    // Begin validating this fucking mess
                    // First make sure its a "zip" (More in depth validation done later)
                    string ext = System.IO.Path.GetExtension(ri.packagezip.FileName);
                    if (ext != ".zip") {
                        nr.errors.Add("Package is not a zip. Please read the package spec.");
                        success = false;
                    }

                    // If we arent successful here, bail early
                    if (!success) {
                        return View("CreateRelease", nr);
                    }

                    if (ri.packagezip.Length > 5242880) { // 5 megabytes
                        nr.errors.Add("Package file is too large (>5mb)");
                        success = false;
                    }

                    // Then make sure its valid
                    if (!(ri.packagezip.Length > 0)) {
                        nr.errors.Add("Package file is invalid");
                        success = false;
                    }

                    // If we arent successful here, bail early
                    if (!success) {
                        return View("CreateRelease", nr);
                    }

                    // Hold it in RAM. This thing aint touching the disk.
                    MemoryStream ms = new MemoryStream();
                    ri.packagezip.OpenReadStream().CopyTo(ms);
                    byte[] fileBytes = ms.ToArray();

                    // Begin a virus scan
                    // TODO: Make this host configurable
                    ClamClient clam = new ClamClient("127.0.0.1", 3310);
                    Task<ClamScanResult> scanThread = clam.SendAndScanFileAsync(fileBytes);
                    scanThread.Wait();
                    ClamScanResults result = scanThread.Result.Result; // Yes double result here. One for the thread, one for the scanner.
                    if (result != ClamScanResults.Clean) {
                        nr.errors.Add("Package file did not pass security clearance");
                        success = false;
                        DatabaseUser U = UserUtil.UserFromContext(HttpContext);
                        _logger.LogWarning(String.Format("{0}/{1} attempted to upload malware.", U.username, U.userId));
                    }

                    // If we arent successfull here, bail early
                    if (!success) {
                        return View("CreateRelease", nr);
                    }

                    // It didnt have a virus, we can now see 
                    string temp_path = Path.GetTempFileName();
                        using (FileStream stream = System.IO.File.Create(temp_path)) {
                            ms.Position = 0; // Gotta reset this
                            ms.CopyTo(stream);
                        }

                    // See if the zip is valid
                    if (!IsZipValid(temp_path)) {
                        nr.errors.Add("Package file was not valid");
                        // Clean it up otherwise
                        System.IO.File.Delete(temp_path);
                        success = false;
                    }

                    // Bail
                    if (!success) {
                        return View("CreateRelease", nr);
                    }

                    // At this point, we have a confirmed zip file thats <5mb, and isnt malware. More validation can be done in the future, but this will do for now
                    // Make the release
                    PackageVersion PV = new PackageVersion();
                    PV.package = P;
                    PV.release_date = DateTime.Now;
                    PV.release_notes = ri.rnotes;
                    PV.version = semver_string;
                    _dbc.package_releases.Add(PV);
                    _dbc.SaveChanges();

                    // Move it to the right place. Make sure we have our package folder
                    string new_path = String.Format("App_Data/Package_Data/{0}", P.id);
                    if (!Directory.Exists(new_path)) {
                        Directory.CreateDirectory(new_path);
                    }
                    System.IO.File.Move(temp_path, String.Format("{0}/{1}.zip", new_path, PV.id));

                    // If we got here, take them back to their package
                    return RedirectToAction(P.package_name, "package");
                }
            }
            
            return View("CreateRelease", nr);
        }

        /// <summary>
        /// Check if a file is a valid zip or not
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsZipValid(string path) {
            try {
                using (var zipFile = ZipFile.OpenRead(path)) {
                    var entries = zipFile.Entries;
                    zipFile.Dispose();
                    return true;
                }
            } catch (InvalidDataException) {
                return false;
            }
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
