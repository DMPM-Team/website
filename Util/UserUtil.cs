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
        /// <summary>
        /// Gets a <see cref="UserModel">UserModel</see> from the current request.
        /// </summary>
        /// <param name="ctx">The <see cref="HttpContext">HttpContext</see> of the request.</param> 
        /// <returns></returns>
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

        /// <summary>
        /// Simple check to see if a user is logged in or not
        /// </summary>
        /// <param name="ctx">The <see cref="HttpContext">HttpContext</see> of the request.</param> 
        /// <returns>True if user is logged in, false if not</returns>
        public static bool IsLoggedIn(HttpContext ctx) {
            if (ctx.User.Claims.Count() == 0) {
                return false;
            } else {
                return true;
            }
        }

        /// <summary>
        /// Saves a user to the database, or updates their name
        /// </summary>
        /// <param name="user">The <see cref="UserModel">UserModel</see> to save or update.</param>
        /// <param name="dbc">The <see cref="DatabaseContext">DatabaseContext</see> to use.</param>
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

        /// <summary>
        /// Formats a user name based on the DB entry
        /// </summary>
        /// <param name="username">The username to format</param>
        /// <param name="dbc">The <see cref="DatabaseContext">DatabaseContext</see> to use.</param>
        /// <returns></returns>
        public static string FormatUser(string username, DatabaseContext dbc) {
            if(dbc.users.Where(u => u.username == username).Any()) {
                return dbc.users.Where(u => u.username == username).First().username;
            } else {
                // Return an empty if the user doesnt exist, though this should never be called if the user doesnt exist.
                return "";
            }
            
        }
    }
}
