using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMPackageManager.Website.Models.Page {
    public class CreatePackageInfo : NewPackageInput {
        /// <summary>
        /// List of errors when trying to create a package
        /// </summary>
        public List<string> errors { get; set; } = new List<string>();
    }
}
