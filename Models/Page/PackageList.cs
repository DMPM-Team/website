using DMPackageManager.Website.Models.Database;
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
        public List<Package> packages { get; set; } = new List<Package>();
    }
}
