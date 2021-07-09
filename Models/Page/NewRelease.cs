using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMPackageManager.Website.Models.Page {
    public class NewRelease : ReleaseInput {
        /// <summary>
        /// Name of the package we are making a release for
        /// </summary>
        public string package_name { get; set; }

        /// <summary>
        /// List of all errors while creating the release
        /// </summary>
        public List<string> errors { get; set; } = new List<string>();
    }
}
