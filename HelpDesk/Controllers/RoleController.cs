using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.Controllers
{
    public class RoleController : Controller
    {
        [Authorize(Policy = "TechnicianOnly")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
