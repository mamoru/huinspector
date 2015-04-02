using huinspector.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace huinspector.Catalogs
{
    public static class UserCatalog
    {
        public static IList<User> GetUsers()
        {
            using (var db = new HUInspectorEntities())
            {
                return (from e in db.User 
                        orderby e.LastName, e.FirstName
                        select e).ToList();
            }
        }

        public static IList<User> GetUsers(int userTypeId)
        {
            using (var db = new HUInspectorEntities())
            {
                return (from e in db.User
                        where e.UserTypeId == userTypeId
                        orderby e.LastName, e.FirstName
                        select e).ToList();
            }
        }

        public static User GetUser(int userId)
        {
            using (var db = new HUInspectorEntities())
            {
                return (from e in db.User
                        where e.Id == userId
                        orderby e.LastName, e.FirstName
                        select e).FirstOrDefault();
            }
        }

        public static User GetUser(string mailAdress)
        {
            using (var db = new HUInspectorEntities())
            {
                return (from e in db.User
                        where e.Mail == mailAdress
                        orderby e.LastName, e.FirstName
                        select e).FirstOrDefault();
            }
        }

    }
}