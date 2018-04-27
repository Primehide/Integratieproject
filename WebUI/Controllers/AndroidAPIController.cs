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
        private string eersteString { get; set; }
        private string tweedeString { get; set; }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { eersteString, tweedeString };
        }

        [HttpPost]
        public void Post([FromBody] string een)
        {
            eersteString = een;
        }

    }
}
