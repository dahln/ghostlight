using System.Threading.Tasks;
using ghostlight.Server.Models;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ghostlight.Server.Controllers
{
    public class OidcConfigurationController : Controller
    {
        private readonly ILogger<OidcConfigurationController> _logger;
        private SignInManager<ApplicationUser> _signInManager {get;set;}


        public OidcConfigurationController(IClientRequestParametersProvider clientRequestParametersProvider, ILogger<OidcConfigurationController> logger, SignInManager<ApplicationUser> signInManager)
        {
            ClientRequestParametersProvider = clientRequestParametersProvider;
            _logger = logger;
            _signInManager = signInManager;
        }

        public IClientRequestParametersProvider ClientRequestParametersProvider { get; }

        [HttpGet("_configuration/{clientId}")]
        public IActionResult GetClientRequestParameters([FromRoute]string clientId)
        {
            var parameters = ClientRequestParametersProvider.GetClientParameters(HttpContext, clientId);
            return Ok(parameters);
        }

        //Called from index.html, when there is a blazor UI error
        [HttpGet]
        [Route("api/v1/signout-error")]
        async public Task<IActionResult> SignoutError()
        {
            await _signInManager.SignOutAsync();
            return LocalRedirect("/authentication/login");
        }
    }
}
