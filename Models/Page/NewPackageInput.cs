using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DMPackageManager.Website.Models.Page {
    public class NewPackageInput {
        /// <summary>
        /// Name of the package
        /// </summary>
        [Required]
        public string pname { get; set; }

        /// <summary>
        /// Description of the package
        /// </summary>
        [Required]
        public string pdesc { get; set; }

        /// <summary>
        /// Source code URL. Optional.
        /// </summary>
        public string surl { get; set; }

        /// <summary>
        /// Documentation URL. Optional.
        /// </summary>
        public string durl { get; set; }
    }
}
