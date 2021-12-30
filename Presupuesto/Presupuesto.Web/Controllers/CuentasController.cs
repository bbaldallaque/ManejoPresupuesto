using AutoMapper;
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
        private readonly IMapper mapper;
		private readonly IRepositorioTransacciones repositorioTransacciones;
		private readonly IServicioReporte servicioReporte;

		public CuentasController(IRepositorioTiposCuentas repositorioTiposCuentas, 
            IServicioUsuarios servicioUsuarios,
            IRepositorioCuenta repositorioCuenta,
            IMapper mapper,
            IRepositorioTransacciones repositorioTransacciones,
            IServicioReporte servicioReporte)
        {
            this.repositorioTiposCuentas = repositorioTiposCuentas;
            this.servicioUsuarios = servicioUsuarios;
            this.repositorioCuenta = repositorioCuenta;
            this.mapper = mapper;
			this.repositorioTransacciones = repositorioTransacciones;
			this.servicioReporte = servicioReporte;
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

        public async Task<IActionResult> Detalle(int id, int mes, int año)
		{
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var cuenta = await repositorioCuenta.ObtenerPorId(id, usuarioId);
            
			if (cuenta is null)
			{
                return RedirectToAction("NoEncontrado", "Home");
            }

   //         DateTime fechaInicio;
   //         DateTime fechaFin;

			//if (mes <= 0 || mes > 12 || año <= 1900)
			//{
   //             var hoy = DateTime.Today;
   //             fechaInicio = new DateTime(hoy.Year, hoy.Month, 1);
			//}
   //         else
			//{
   //             fechaInicio = new DateTime(año, mes, 1);
			//}

   //         fechaFin = fechaInicio.AddMonths(1).AddDays(-1);

            ViewBag.Cuenta = cuenta.Nombre;
            var modelo = await servicioReporte.ObtenerReporteTransaccionesDetallasPorCuenta(usuarioId, id, mes, año, ViewBag);
            return View(modelo);
            //var obtenerTransaccionesPorCuenta = new ObtenerTransaccionesPorCuenta()
            //{
            //    CuentaId = id,
            //    UsuarioId = usuarioId,
            //    FechaInicio = fechaInicio,
            //    FechaFin = fechaFin
            //};

            //var transacciones = await repositorioTransacciones.ObtenerPorCuentaId(obtenerTransaccionesPorCuenta);

            //var modelo = new ReporteTransaccionesDetalladas();
            //ViewBag.Cuenta = cuenta.Nombre;

            //var transaccionesPorFecha = transacciones.OrderByDescending(x => x.FechaTransaccion)
            //      .GroupBy(x => x.FechaTransaccion)
            //      .Select(grupo => new ReporteTransaccionesDetalladas.TransaccionesPorFecha()
            //      {
            //          FechaTransaccion = grupo.Key,
            //          Transacciones = grupo.AsEnumerable()
            //      });

            //modelo.TransaccionesAgrupadas = transaccionesPorFecha;
            //modelo.FechaInicio = fechaInicio;
            //modelo.FechaFin = fechaFin;

            //ViewBag.mesAnterior = fechaInicio.AddMonths(-1).Month;
            //ViewBag.añoAnterior = fechaInicio.AddMonths(-1).Year;
            //ViewBag.mesPosterior = fechaInicio.AddMonths(1).Month;
            //ViewBag.añoPosterior = fechaInicio.AddMonths(1).Year;
            //ViewBag.UrlRetorno = HttpContext.Request.Path + HttpContext.Request.QueryString;


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

        public async Task<IActionResult> Editar(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var cuentas = await repositorioCuenta.ObtenerPorId(id, usuarioId);

            if (cuentas is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var modelo = mapper.Map<CuentaCreacionViewModel>(cuentas);

            modelo.TiposCuentas = await ObttenerTiposCuentas(usuarioId);
            return View(modelo); 
        }

        [HttpPost]
        public async Task<IActionResult> Editar(CuentaCreacionViewModel cuentaEditar)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var cuenta = await repositorioCuenta.ObtenerPorId(cuentaEditar.Id, usuarioId);

            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(cuentaEditar.TipoCuentaId, usuarioId);

            if (tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await repositorioCuenta.Actualizar(cuentaEditar);
            return RedirectToAction("Index");
        }      

        public async Task<IActionResult> Borrar(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var cuenta = await repositorioCuenta.ObtenerPorId(id, usuarioId);

            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            return View(cuenta);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarCuenta(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var cuenta = await repositorioCuenta.ObtenerPorId(id, usuarioId);

            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await repositorioCuenta.Borrar(id); 
            return RedirectToAction("Index");
        }
    }
}
