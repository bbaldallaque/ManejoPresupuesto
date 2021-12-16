using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Presupuesto.Web.Models;
using Presupuesto.Web.Servicio;

namespace Presupuesto.Web.Controllers
{
    public class CuentasController : Controller
    {
        private readonly IRepositorioTiposCuentas repositorioTiposCuentas;
        private readonly IServicioUsuarios servicioUsuarios;
        private readonly IRepositorioCuenta repositorioCuenta;

        public CuentasController(IRepositorioTiposCuentas repositorioTiposCuentas, 
            IServicioUsuarios servicioUsuarios,
            IRepositorioCuenta repositorioCuenta)
        {
            this.repositorioTiposCuentas = repositorioTiposCuentas;
            this.servicioUsuarios = servicioUsuarios;
            this.repositorioCuenta = repositorioCuenta;
        }

        public async Task<IActionResult> Index()
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var cuentasConTipoCuentas = await repositorioCuenta.Buscar(usuarioId);

            var modelo = cuentasConTipoCuentas.
                GroupBy(x => x.TipoCuenta)
                .Select(grupo => new IndiceCuentaViewModel
                {
                    TipoCuenta = grupo.Key,
                    Cuentas = grupo.AsEnumerable(),
                }).ToList();

            return View(modelo);
        }

        public async Task<IActionResult> Crear()
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();         
            var modelo = new CuentaCreacionViewModel();
            modelo.TiposCuentas = await ObttenerTiposCuentas(usuarioId);
            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(CuentaCreacionViewModel cuenta)
        {
           
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(cuenta.TipoCuentaId, usuarioId);

            if (tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            if (!ModelState.IsValid)
            {
                cuenta.TiposCuentas = await ObttenerTiposCuentas(usuarioId);
                return View(cuenta);
            }
            await repositorioCuenta.Crear(cuenta);
            return RedirectToAction("Index");
        }

        private async Task<IEnumerable<SelectListItem>> ObttenerTiposCuentas(int usuarioId)
        {
            var tipoCuentas = await repositorioTiposCuentas.Obtener(usuarioId);
            return tipoCuentas.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
        }
    }
}
