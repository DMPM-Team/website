using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMPackageManager.Website.Models.Page {
    public class UserPackageList : PackageList {
        /// <summary>
        /// Usename that this package list belongs to
        /// </summary>
        public string package_owner { get; set; }
    }
}
