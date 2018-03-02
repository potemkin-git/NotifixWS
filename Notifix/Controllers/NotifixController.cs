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

        [Route("checklogin")]
        [HttpPost]
        public String CheckLogin([FromBody]String loginData)
        {
            User user = JsonConvert.DeserializeObject<User>(loginData);
            String response;
            string toHash = user.login + user.password;
            var hashedString = Sha256encrypt(toHash);


            using (UserContext ctx = new UserContext())
            {
                if (ctx.userList.Any(q => q.login == user.login && q.password == user.password))
                {
                    response = "200"+hashedString;
                } else
                {
                    response = "403"+hashedString;
                }
            }

            return response;
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
                } else {
                    response = "200";
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
            String res;
            using (UserContext ctx = new UserContext())
            {
                ctx.userList.Add(user);
                int dbReturn = ctx.SaveChanges();

                res = dbReturn == 1 ? "200" + user.password : "404";
            }
            return res;
        }

        [Route("savenotification")]
        [HttpPost]
        public String SaveNotification([FromBody]String newNotification)
        {
            Notification notif = JsonConvert.DeserializeObject<Notification>(newNotification);
            String res;
            using (NotificationContext ctx = new NotificationContext())
            {
                ctx.notificationList.Add(notif);
                int dbReturn = ctx.SaveChanges();

                res = dbReturn == 1 ? "200" : "404";
            }
            return res;
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
