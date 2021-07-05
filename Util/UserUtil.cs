using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DMPackageManager.Website.Database;
using DMPackageManager.Website.Models;
using DMPackageManager.Website.Models.Database;
using Microsoft.AspNetCore.Http;

namespace DMPackageManager.Website.Util {
    public static class UserUtil {
        public static UserModel UserFromContext(HttpContext ctx) {
            UserModel u = new UserModel();
            if(ctx.User.Claims.Count() == 0) {
                u.valid = false;
                return u;
            }
            u.valid = true;
            u.userId = Int64.Parse(ctx.User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).First().Value);
            u.username = ctx.User.Claims.Where(x => x.Type == ClaimTypes.Name).First().Value;
            return u;
        }

        public static bool IsLoggedIn(HttpContext ctx) {
            if (ctx.User.Claims.Count() == 0) {
                return false;
            } else {
                return true;
            }
        }

        public static void Save2DB(UserModel user, DatabaseContext dbc) {
            // First see if our user exists
            if(dbc.users.Where(u => u.userId == user.userId).Any()) {
                // We exist, update name
                DatabaseUser dbu = dbc.users.Where(u => u.userId == user.userId).First();
                dbu.username = user.username;
                dbc.SaveChanges();
            } else {
                // We dont exist. Insert us.
                DatabaseUser dbu = new DatabaseUser();
                dbu.userId = user.userId;
                dbu.username = user.username;
                dbc.users.Add(dbu);
                dbc.SaveChanges();
            }
        }
    }
}
