using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DMPackageManager.Website.Models.Database;

namespace DMPackageManager.Website.Models {
    public class UserModel : DatabaseUser {
        /// <summary>
        /// Is the user valid or not
        /// </summary>
        public bool valid { get; set; }
    }
}
