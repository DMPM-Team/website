using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DMPackageManager.Website.Models.Database {
    public class DatabaseUser {
        /// <summary>
        /// GitHub ID of our user
        /// </summary>
        [Key]
        public Int64 userId { get; set; }

        /// <summary>
        /// Name of our user for caching purposes
        /// </summary>
        public string username { get; set; }

    }
}
