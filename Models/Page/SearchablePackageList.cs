using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMPackageManager.Website.Models.Page {
    public class SearchablePackageList : PackageList {
        /// <summary>
        /// Current search query
        /// </summary>
        public string search_query { get; set; }
    }
}
