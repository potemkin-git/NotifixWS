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
using System.Net.Mail;
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

        [Route("resetpassword")]
        [HttpPost]
        public String ResetPassword([FromBody]String mail)
        {
            User user = JsonConvert.DeserializeObject<User>(mail);
            string maill = user.email;
            using (UserContext ctx = new UserContext())
            {
                User updatedUser = new User();
                updatedUser = ctx.userList.FirstOrDefault(q => q.email == maill);
                if (updatedUser == null)
                {
                    return "401";
                }
                    string password = RandomString(15);
                updatedUser.password = Sha256encrypt(password);
                MailPassword(updatedUser.login, updatedUser.email, password);

                ctx.userList.Attach(updatedUser);
                ctx.Entry(updatedUser).State = EntityState.Modified;
                int dbReturn = ctx.SaveChanges();
            }
                return "400";
        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789,?;.:/!§*";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public void MailPassword(string login, string email, string password)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress("notifixmail@gmail.com");
                mail.To.Add(email);
                mail.Subject = "NOTIFIX: Reset Password";
                mail.Body = "Dear " + login + ",\nhere is your new password, you can change it on Notifix:\n" + password + "\n See you soon on Notifix ! ";

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential("notifixmail@gmail.com", "Notifix666");
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);
            }
            catch (Exception ex)
            {
            }
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

                res = dbReturn == 1 ? "200" + hashedString : "404";
            }
            return res;
        }

        [Route("updateuser")]
        [HttpPost]
        public String UpdateUser([FromBody]String newUser)
        {
            User user = JsonConvert.DeserializeObject<User>(newUser);
            String res;

            using (UserContext ctx = new UserContext())
            {
                User updatedUser = new User();
                updatedUser = ctx.userList.FirstOrDefault(q => q.login == user.login);
                updatedUser.address = user.address;
                updatedUser.avatarSrc = user.avatarSrc;
                updatedUser.city = user.city;
                updatedUser.email = user.email;
                updatedUser.firstName = user.firstName;
                updatedUser.lastName = user.lastName;
                if (user.password.Length > 0)
                {
                    updatedUser.password = Sha256encrypt(user.password);
                }

                ctx.userList.Attach(updatedUser);
                ctx.Entry(updatedUser).State = EntityState.Modified;
                int dbReturn = ctx.SaveChanges();

                res = dbReturn == 1 ? "200" : "404";
            }
            return res;
        }

        [Route("userinfos")]
        [HttpPost]
        public JObject UserInfos([FromBody]String loginData)
        {
            dynamic tmp = JsonConvert.DeserializeObject(loginData);
            string loginReq = (string)tmp.login;
            string hashReq = (string)tmp.hash;
            dynamic userInfo = new JObject();

            using (UserContext ctxUser = new UserContext())
            {
                User user = new User();
                user = ctxUser.userList.FirstOrDefault(q => q.login == loginReq);
                if (user == null || Sha256encrypt(user.login + user.password) != hashReq)
                {
                    userInfo = null;
                }
                else
                {
                    userInfo.address = user.address;
                    userInfo.firstName = user.firstName;
                    userInfo.lastName = user.lastName;
                    userInfo.login = user.login;
                    userInfo.city = user.city;
                    userInfo.avatar = user.avatarSrc;
                    userInfo.mail = user.email;
                }
            }

            return userInfo;
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

        [Route("votenotification")]
        [HttpPost]
        public int VoteNotification([FromBody]String jsonRequest)
        {
            dynamic tmp = JsonConvert.DeserializeObject(jsonRequest);
            int notifId = (int)tmp.notifId;
            int vote = (int)tmp.vote;
            string login = (string)tmp.userLogin;
            int dbReturn;

            using (UserContext ctxUser = new UserContext())
            {
                using (NotificationContext ctxNotification = new NotificationContext())
                {
                Notification notification = ctxNotification.notificationList.Include(q => q.user).FirstOrDefault(q => q.id == notifId);
                if (vote == 1)
                {
                    notification.nbConf++;
                }
                else if (vote == -1)
                {
                    notification.nbDeny++;
                }

                ctxNotification.notificationList.Attach(notification);
                ctxNotification.Entry(notification).State = EntityState.Modified;
                ctxNotification.Entry(notification.user).State = EntityState.Unchanged;
                dbReturn = ctxNotification.SaveChanges();
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
                    notifCustom.id = notification.id;
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
