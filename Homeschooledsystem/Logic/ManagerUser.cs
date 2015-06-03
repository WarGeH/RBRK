using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using Homeschooledsystem.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Homeschooledsystem.Logic
{
    public class ManagerUser
    {
        public static string GetUser(IPrincipal user, ApplicationDbContext db)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            return userManager.FindById(user.Identity.GetUserId()).Id;
        }
    }
}