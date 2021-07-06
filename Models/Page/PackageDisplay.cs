using DMPackageManager.Website.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMPackageManager.Website.Models.Page {
    public class PackageDisplay {
        /// <summary>
        /// The date that the most recent release came out
        /// </summary>
        public DateTime last_update { get; set; }

        /// <summary>
        /// The latest version of the package
        /// </summary>
        public string latest_version { get; set; }

        /// <summary>
        /// Total downloads of the package
        /// </summary>
        public int total_downloads { get; set; }
    }
}
