using Microsoft.AspNetCore.Mvc;

namespace ElroukenAljamil.Notification.API.Controllers
{
    public class NotificationsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
