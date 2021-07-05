using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMPackageManager.Website.Models.Page {
    public class PageBaseModel {
        /// <summary>
        /// Are we logged in or not
        /// </summary>
        public bool logged_in { get; set; }

        /// <summary>
        /// Username if we are logged in
        /// </summary>
        public string username { get; set; }
    }
}
