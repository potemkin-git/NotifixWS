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

                res = dbReturn == 1 ? "200"+hashedString : "404";
            }
            return res;
        }

        [Route("savenotification")]
        [HttpPost]
        public string SaveNotification([FromBody]String jsonRequest)
        {
            dynamic tmp = JsonConvert.DeserializeObject(jsonRequest);
            string loginReq = (string)tmp.login;
            string hashReq = (string)tmp.hash;

            Notification notif = JsonConvert.DeserializeObject<Notification>(jsonRequest);
            string dbReturn = "0";

            using (UserContext ctxUser = new UserContext())
            {
                using (NotificationContext ctxNotification = new NotificationContext())
                {
                    User user = ctxUser.userList.FirstOrDefault(q => q.login == loginReq);
                    if (user != null)
                    {
                        string userHash = Sha256encrypt(user.login + user.password);
                        if (userHash == hashReq)
                        {
                            notif.user = user;
                            ctxNotification.notificationList.Add(notif);
                            ctxNotification.Entry(user).State = user.id == 0 ? EntityState.Added : EntityState.Modified;
                            int res = ctxNotification.SaveChanges();
                            dbReturn = res > 0 ? userHash : "0";
                        }
                    }
                }
            }

            return dbReturn;
        }

        [Route("getNotifications")]
        [HttpGet]
        public JArray GetAllNotifications()
        {
            var notifList = new JArray();

            using (NotificationContext ctxNotification = new NotificationContext())
            {
                List<Notification> notifs = ctxNotification.notificationList.Include("User").ToList();

                foreach (Notification notification in notifs)
                {
                    User user = notification.user;
                    String token = Sha256encrypt(user.login + user.password);

                    dynamic notifCustom = new JObject();
                    notifCustom.userToken = token;
                    notifCustom.desc = notification.description;
                    notifCustom.type = notification.type;
                    notifCustom.date = notification.expirationDate.ToString("dd-MM-yyyy");
                    notifCustom.time = notification.expirationDate.ToString("h:mm");
                    notifCustom.lat = notification.latitude;
                    notifCustom.lng = notification.longitude;
                    notifCustom.nbConf = notification.nbConf;
                    notifCustom.nbDeny = notification.nbDeny;
                    notifList.Add(notifCustom);
                }
            }                      

            return notifList;
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
