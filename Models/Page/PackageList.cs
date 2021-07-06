using DMPackageManager.Website.Models.Database;
using DMPackageManager.Website.Util;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMPackageManager.Website.Models.Page {
    public class PackageList : PageBaseModel {
        /// <summary>
        /// List of all packages to display here
        /// </summary>
        public PaginatedList<Package> packages { get; set; }

        /// <summary>
        /// Package meta info that isnt the same as the DB model (latest release date, downloads, etc)
        /// </summary>
        public Dictionary<int, PackageDisplay> package_meta { get; set; }
    }
}
