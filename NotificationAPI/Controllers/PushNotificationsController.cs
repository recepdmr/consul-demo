using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NotificationAPI.Options;

namespace NotificationAPI.Controllers
{
    [Route("api/[controller]")]
    public class PushNotificationsController : ControllerBase
    {

        public PushNotificationsController(IConfiguration configuration, IOptionsSnapshot<PushNotificationOptions> options)
        {
            Configuration = configuration;
            PushNotificationOptions = options.Value;
        }

        public IConfiguration Configuration { get; }
        public PushNotificationOptions PushNotificationOptions { get; }

        [HttpGet]
        public IActionResult Get()
        {
            var uri = Configuration["Message"];

            return Ok(new
            {
                uri,
                PushNotificationOptions
            });
        }
    }
}