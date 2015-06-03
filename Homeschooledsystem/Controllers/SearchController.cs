using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Homeschooledsystem.Models;

namespace Homeschooledsystem.Controllers
{
    public class SearchController : Controller
    {
        // GET: Search
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index(string search)
        {
            if (search != null && search != string.Empty)
            {
                var searchArr = search.Split(' ');
                List<ApplicationUser> foundedUser = new List<ApplicationUser>();

                if (searchArr.Count<String>() <= 2)
                {
                    List<ApplicationUser> users = db.Users.ToList<ApplicationUser>();
                    if(searchArr.Count<String>() == 1)
                    {
                        foreach (var user in users)
                        {
                            if (user.FirstName.ToLower() == searchArr[0].ToLower() || user.LastName.ToLower() == searchArr[0].ToLower())
                            {
                                foundedUser.Add(user);
                            }
                        }
                    }
                    else
                    {
                        foreach (var user in users)
                        {
                            if (user.FirstName.ToLower() == searchArr[0].ToLower() && user.LastName.ToLower() == searchArr[1].ToLower())
                            {
                                foundedUser.Add(user);
                            }
                            if (user.LastName.ToLower() == searchArr[0].ToLower() && user.FirstName.ToLower() == searchArr[1].ToLower())
                            {
                                foundedUser.Add(user);
                            }
                        }
                    }

                        return View(foundedUser);           
                }
            }
            return View();
        }
       
    }
}