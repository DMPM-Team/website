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

        /// <summary>
        /// Raw username. Used for modifying URL parameters.
        /// </summary>
        public string raw_username { get; set; }

        /// <summary>
        /// Are we looking at our own package list?
        /// </summary>
        public bool own_packages { get; set; } = false;
    }
}
