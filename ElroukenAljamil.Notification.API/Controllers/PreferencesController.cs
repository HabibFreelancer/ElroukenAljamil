using Microsoft.AspNetCore.Mvc;

namespace ElroukenAljamil.Notification.API.Controllers
{
    public class PreferencesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
