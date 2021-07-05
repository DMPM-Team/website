using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DMPackageManager.Website.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace DMPackageManager.Website.Database {
    public class DatabaseContext : DbContext {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) {
            
        }
        
        public DbSet<Package> packages { get; set; }

        public DbSet<DatabaseUser> users { get; set; }
    }
}
