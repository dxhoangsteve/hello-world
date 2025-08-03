using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BaseSource.BackendApi.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {
        private string _userId;
        public string UserId
        {
            get { return _userId ?? User.FindFirstValue(ClaimTypes.NameIdentifier); }
            set { _userId = value; }
        }

        private string _language;
        public string Language
        {
            get { return _language ?? CultureInfo.CurrentCulture.Name; }
            set { _language = value; }
        }
    }
}
