using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.Cors;
using Newtonsoft.Json.Linq;
using Notifix.Models;
using System.Security.Cryptography;
using System.Text;
using System.Data.Entity;

namespace Notifix.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("notifix/api")]
    public class NotifixController : ApiController
    {
        [Route("start")]
        [HttpGet]
        public String CheckService()
        {
          return "Service Notifix Working";
        }

        [Route("checkUniqueLoginEmail")]
        [HttpPost]
        public String CheckUniqueLoginEmail([FromBody]String credentials)
        {
            User user = JsonConvert.DeserializeObject<User>(credentials);
            String response;
            using (UserContext ctx = new UserContext())
            {
                if (ctx.userList.Any(q => q.login == user.login))
                {
                    response = "403login";
                }
                else if (ctx.userList.Any(q => q.email == user.email))
                {
                    response = "403pwd";
                }
                else
                {
                    response = "200";
                }
            }

            return response;
        }

        [Route("checklogin")]
        [HttpPost]
        public String CheckLogin([FromBody]String loginData)
        {
            User user = JsonConvert.DeserializeObject<User>(loginData);
            user.password = Sha256encrypt(user.password);
            string toHash = user.login + user.password;
            var hashedString = Sha256encrypt(toHash);
            String response;

            using (UserContext ctx = new UserContext())
            {
                if (ctx.userList.Any(q => q.login == user.login && q.password == user.password))
                {
                    response = "200"+hashedString;
                } else if (!ctx.userList.Any(q => q.login == user.login))
                {
                    response = "403login";
                }
                else
                {
                    response = "403password";
                }
            }

            return response;
        }

        [Route("registeruser")]
        [HttpPost]
        public String RegisterUser([FromBody]String newUser)
        {
            User user = JsonConvert.DeserializeObject<User>(newUser);
            user.password = Sha256encrypt(user.password);
            string toHash = user.login + user.password;
            var hashedString = Sha256encrypt(toHash);
            String res;

            using (UserContext ctx = new UserContext())
            {
                ctx.userList.Add(user);
                int dbReturn = ctx.SaveChanges();

                res = dbReturn == 1 ? "200"+hashedString : "404"+hashedString;
            }
            return res;
        }

        [Route("savenotification")]
        [HttpPost]
        public int SaveNotification([FromBody]String newNotification)
        {
            Notification notif = JsonConvert.DeserializeObject<Notification>(newNotification);
            int dbReturn;

            using (UserContext ctxUser = new UserContext())
            {
                using (NotificationContext ctxNotification = new NotificationContext())
                {
                    var user = ctxUser.userList.FirstOrDefault(q => q.login == "Potemkin");
                    notif.user = user;
                    ctxNotification.notificationList.Add(notif);
                    ctxNotification.Entry(user).State = user.id == 0 ? EntityState.Added : EntityState.Modified;
                    dbReturn = ctxNotification.SaveChanges();
                }
            }

            return dbReturn;
        }

        private string Sha256encrypt(string phrase)
        {
            UTF8Encoding encoder = new UTF8Encoding();
            SHA256Managed sha256hasher = new SHA256Managed();
            byte[] hashedDataBytes = sha256hasher.ComputeHash(encoder.GetBytes(phrase));
            return Convert.ToBase64String(hashedDataBytes);
        }
    }




}
