using Microsoft.AspNetCore.Mvc;
using Presupuesto.Web.Models;

namespace Presupuesto.Web.Controllers
{
    public class TiposCuentasController : Controller
    {
        public IActionResult Crear()
        {
          return View();
        }

        [HttpPost]
        public IActionResult Crear(TipoCuenta tipoCuenta)
        {
            return View();
        }
    }
}
