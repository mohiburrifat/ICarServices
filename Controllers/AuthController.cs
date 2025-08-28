using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace I_Car_Services.Controllers
{
    public class AuthController : Controller
    {
        // GET: /Auth/Logout
        public IActionResult Logout()
        {
            // Clear all session data
            HttpContext.Session.Clear();

            // Optionally add a message (can be displayed in the view)
            TempData["Message"] = "You have been logged out.";

            // Redirect to home or login page
            return RedirectToAction("Index", "Home");
        }
    }
}
