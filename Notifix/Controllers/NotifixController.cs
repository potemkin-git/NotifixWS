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

namespace Notifix.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("notifix/api")]
    public class NotifixController : ApiController
    {
        [Route("start")]
        [HttpGet]
        public String checkService()
        {
          return "Service Notifix Working";
        }

        [Route("checklogin")]
        [HttpPost]
        public int CheckLogin([FromBody]String login)
        {
            User user = JsonConvert.DeserializeObject<User>(login);
            int response;

            using (UserContext ctx = new UserContext())
            {
                if (ctx.userList.Any(q => q.login == user.login && q.password == user.password))
                {
                    response = 200;
                } else
                {
                    response = 403;
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
    }
}
