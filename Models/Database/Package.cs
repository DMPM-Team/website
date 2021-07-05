using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DMPackageManager.Website.Models.Database {
    public class Package {
        /// <summary>
        /// Internal package ID
        /// </summary>
        [Key]
        public int id { get; set; }

        /// <summary>
        /// Owner ID (GitHub account)
        /// </summary>
        public DatabaseUser owner { get; set; }

        /// <summary>
        /// Package name
        /// </summary>
        public string package_name { get; set; }

        /// <summary>
        /// Package description
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// Latest version of the package
        /// </summary>
        public string latest_version { get; set; } = null;

        /// <summary>
        /// Date the package was created
        /// </summary>
        public DateTime creation_date { get; set; }
    }
}
