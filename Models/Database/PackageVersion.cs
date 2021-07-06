using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DMPackageManager.Website.Models.Database {
    public class PackageVersion {
        /// <summary>
        /// Internal version ID
        /// </summary>
        [Key]
        public int id { get; set; }

        /// <summary>
        /// Foreign key to the package this release corresponds to
        /// </summary>
        public Package package { get; set; }

        /// <summary>
        /// Release notes for this version of the package (markdown)
        /// </summary>
        public string release_notes { get; set; }

        /// <summary>
        /// Version code. This is not a version type itself, so people can specify things such as 1.0A
        /// </summary>
        public string version { get; set; }

        /// <summary>
        /// Download count. Mostly just for stat logging of downloads of each release;
        /// </summary>
        public int download_count { get; set; }

        /// <summary>
        /// DateTime that this package release was created
        /// </summary>
        public DateTime release_date { get; set; }
    }
}
