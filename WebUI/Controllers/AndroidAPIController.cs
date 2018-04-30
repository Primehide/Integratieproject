using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebUI.Controllers
{
    public class AndroidAPIController : ApiController
    {
        private static int platformId { get; set; }

        //test method, pay to attention to this.
        [HttpGet]
        public void Get()
        {
           
        }

        //Change on which platform you are
        //return HttpStatusCode.OK if success
        [HttpPost]
        public HttpStatusCode PostPlatform([FromBody] int platId)
        {
            platformId = platId;
            return HttpStatusCode.OK;
        }

        //Post username and password to login
        //return HttpStatusCode.OK if success
        //return HttpStatusCode.Unauthorized if not
        [HttpPost]
        public HttpStatusCode PostAccount([FromBody] AndroidAccount account) 
        {
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    
        }

    }
}
