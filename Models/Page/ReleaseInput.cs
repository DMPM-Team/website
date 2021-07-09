using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMPackageManager.Website.Models.Page {
    public class ReleaseInput {
        /// <summary>
        /// The version tag of the package
        /// </summary>
        public string vtag { get; set; }

        /// <summary>
        /// Release notes of the package
        /// </summary>
        public string rnotes { get; set; }

        /// <summary>
        /// The zip file with the package itself
        /// </summary>
        public IFormFile packagezip { get; set; }
    }
}
