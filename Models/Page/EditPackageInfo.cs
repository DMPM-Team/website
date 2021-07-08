using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMPackageManager.Website.Models.Page {
    public class EditPackageInfo : NewPackageInfo {
        /// <summary>
        /// Did the edit go through successfully
        /// </summary>
        public bool success { get; set; }
    }
}
