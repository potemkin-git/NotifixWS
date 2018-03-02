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
        public String CheckLogin([FromBody]String login)
        {
            User user = JsonConvert.DeserializeObject<User>(login);
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

        [Route("registeruser")]
        [HttpPost]
        public HttpResponseMessage RegisterUser([FromBody]DynamicJsonObject newUser)
        {
            //@todo INSERT BDD
            using (UserContext ctx = new UserContext())
            {
                User user = new User();
                ctx.userList.Add(user);
                ctx.SaveChanges();
            }
         

            return Request.CreateResponse(HttpStatusCode.OK, "user created:");
        }

        // GET: api/Notifix/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST: api/Notifix
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT: api/Notifix/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        // DELETE: api/Notifix/5
        //public void Delete(int id)
        //{
        //}

        private string Sha256encrypt(string phrase)
        {
            UTF8Encoding encoder = new UTF8Encoding();
            SHA256Managed sha256hasher = new SHA256Managed();
            byte[] hashedDataBytes = sha256hasher.ComputeHash(encoder.GetBytes(phrase));
            return Convert.ToBase64String(hashedDataBytes);
        }
    }




}
