using Microsoft.AspNetCore.Mvc;


namespace wepapp.Controllers
{
    public class ProductsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
