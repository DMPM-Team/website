using DMPackageManager.Website.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMPackageManager.Website.Models.Page {
    public class PackageInfo {
        /// <summary>
        /// The package we are looking at
        /// </summary>
        public Package package { get; set; }
        /// <summary>
        /// List of all its releases
        /// </summary>
        public List<PackageVersion> releases { get; set; } = new List<PackageVersion>();

        /// <summary>
        /// Meta for the package
        /// </summary>
        public PackageDisplay package_meta { get; set; } = new PackageDisplay();

        /// <summary>
        /// Holder for current version
        /// </summary
        public PackageVersion current_version { get; set; }
    }
}
