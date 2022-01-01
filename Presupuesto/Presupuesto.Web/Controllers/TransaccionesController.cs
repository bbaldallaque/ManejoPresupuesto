using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Presupuesto.Web.Models;
using Presupuesto.Web.Servicio;
using System.Threading.Tasks;

namespace Presupuesto.Web.Controllers
{
    public class TransaccionesController : Controller
    {
        private readonly IServicioUsuarios servicioUsuarios;
        private readonly IRepositorioTransacciones repositorioTransacciones;
        private readonly IRepositorioCuenta repositorioCuenta;
        private readonly IRepositorioCategoria repositorioCategoria;
        private readonly IMapper mapper;
		private readonly IServicioReporte servicioReporte;

		public TransaccionesController(IServicioUsuarios servicioUsuarios, IRepositorioTransacciones repositorioTransacciones,
            IRepositorioCuenta repositorioCuenta,
            IRepositorioCategoria repositorioCategoria,
            IMapper mapper,
            IServicioReporte servicioReporte)
        {
            this.servicioUsuarios = servicioUsuarios;
            this.repositorioTransacciones = repositorioTransacciones;
            this.repositorioCuenta = repositorioCuenta;
            this.repositorioCategoria = repositorioCategoria;
            this.mapper = mapper;
			this.servicioReporte = servicioReporte;
		}

        public async Task<IActionResult> Index(int mes, int año )
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var modelo = await servicioReporte.ObtenerReporteTransaccionesDetallas(usuarioId, mes, año, ViewBag);
            return View(modelo);       
        }

        public async Task<IActionResult> Semanal(int mes, int año)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            IEnumerable<ResultadoObtenerPorSemana> transaccionesPorSemana = await servicioReporte.ObtenerReporteSemanal(usuarioId, mes, año, ViewBag);

            var agrupado = transaccionesPorSemana.GroupBy(x => x.Semana).Select(x => new
             ResultadoObtenerPorSemana()
            {
                Semana = x.Key,
                Ingresos = x.Where(x => x.TipoOperacionId == TipoOperacion.Ingreso).Select(x => x.Monto).FirstOrDefault(),
                Gastos = x.Where(x => x.TipoOperacionId == TipoOperacion.Gasto).Select(x => x.Monto).FirstOrDefault(),
            }).ToList();

            if (año == 0 || mes == 0)
            {
                var hoy = DateTime.Today;
                año = hoy.Year;
                mes = hoy.Month;    
            }
            var fechaReferencia = new DateTime(año, mes, 1);
            var diasDelMes = Enumerable.Range(1, fechaReferencia.AddMonths(1).AddDays(-1).Day);

            var diaSegmentados = diasDelMes.Chunk(7).ToList();

            for (int i = 0; i < diaSegmentados.Count(); i++)
            {
                var semana = i + 1;
                var fechaInicio = new DateTime(año, mes, diaSegmentados[i].First());
                var fechaFin = new DateTime(año,mes, diaSegmentados[i].Last());
                var gruposSemana = agrupado.FirstOrDefault(x => x.Semana == semana);

                if (gruposSemana is null)
                {
                    agrupado.Add(new ResultadoObtenerPorSemana()
                    {
                        Semana = semana,
                        FechaInicio = fechaInicio,
                        FechaFin = fechaFin,
                    });
                }
                else
                {
                    gruposSemana.FechaInicio = fechaInicio;
                    gruposSemana.FechaFin = fechaFin;
                }
            }
            agrupado = agrupado.OrderByDescending(x => x.Semana).ToList();

            var modelo = new ReporteSemanaViewModel();
            modelo.TransaccionesPorSemana = agrupado;
            modelo.FechaReferencia = fechaReferencia;

            return View(modelo);  
        }

        public IActionResult ExcelReporte()
        {
            return View();
        }

        public async Task<IActionResult> Mensual(int año)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

			if (año == 0)
			{
                año = DateTime.Today.Year;
			}

            var transaccionesPorMes = await repositorioTransacciones.ObtenerPorMes(usuarioId, año);

            var TransaccionesAgrupadas = transaccionesPorMes.GroupBy(x => x.Mes)
                .Select(x => new ResultadoObtnerPorMes()
                {
                    Mes = x.Key,
                    Ingreso = x.Where(x => x.TipoOperacionId == TipoOperacion.Ingreso).Select(x => x.Monto).FirstOrDefault(),
                    Gastos = x.Where(x => x.TipoOperacionId == TipoOperacion.Gasto).Select(x => x.Monto).FirstOrDefault()
                }).ToList();

			for (int mes = 1; mes <= 12; mes++)
			{
                var transaccion = TransaccionesAgrupadas.FirstOrDefault(x => x.Mes == mes);
                var fechaReferencia = new DateTime(año, mes, 1);
				if (transaccion is null)
				{
                    TransaccionesAgrupadas.Add(new ResultadoObtnerPorMes()
                    {
                        Mes =mes,
                        FechaReferencia = fechaReferencia
                    });
				}
				else
				{
                    transaccion.FechaReferencia = fechaReferencia;
				}
			}

            TransaccionesAgrupadas = TransaccionesAgrupadas.OrderByDescending(x => x.Mes).ToList();

            var modelo = new ReporteMensualViewModel();
            modelo.Año = año;
            modelo.TransaccionesPorMes = TransaccionesAgrupadas;
            
            return View(modelo);
        }

        public IActionResult Calendario()
        {
            return View();
        }


        public async Task<ActionResult> Crear()
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var modelo = new TransaccionCreacionViewModel();
            modelo.Cuentas = await ObtenerCuentas(usuarioId);
            modelo.Categorias = await ObtenerCategorias(usuarioId, modelo.TipoOperacionId);
            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(TransaccionCreacionViewModel modelo)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            if (!ModelState.IsValid)
            {
                modelo.Cuentas = await ObtenerCuentas(usuarioId);
                modelo.Categorias = await ObtenerCategorias(usuarioId, modelo.TipoOperacionId);
                return View(modelo);
            }

            var cuenta = await repositorioCuenta.ObtenerPorId(modelo.CuentaId, usuarioId);
            
            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var categoria = await repositorioCategoria.ObtenerPorId(modelo.CategoriaId, usuarioId);

            if (categoria is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            modelo.UsuarioId = usuarioId;

            if (modelo.TipoOperacionId == TipoOperacion.Gasto)
            {
                modelo.Monto *= -1;
            }

            await repositorioTransacciones.Crear(modelo);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Editar(int id, string urlRetorno = null)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var transaccion = await repositorioTransacciones.ObtenerPorId(id, usuarioId);

            if (transaccion is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var modelo = mapper.Map<TransaccionActualizacionViewModel>(transaccion);

            modelo.MontoAnterior = modelo.Monto;

            if (modelo.TipoOperacionId == TipoOperacion.Gasto)
            {
                modelo.MontoAnterior = modelo.Monto * -1;
            }

            modelo.CuentaAnteriorId = transaccion.CuentaId;
            modelo.Categorias = await ObtenerCategorias(usuarioId, transaccion.TipoOperacionId);
            modelo.Cuentas = await ObtenerCuentas(usuarioId);
            modelo.UrlRetorno = urlRetorno;

            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(TransaccionActualizacionViewModel modelo)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            
            if (!ModelState.IsValid)
            {
                modelo.Categorias = await ObtenerCategorias(usuarioId, modelo.TipoOperacionId);
                modelo.Cuentas = await ObtenerCuentas(usuarioId);
                return View(modelo);
            }

            var cuenta = await repositorioCuenta.ObtenerPorId(modelo.CuentaId, usuarioId);

            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var categoria = await repositorioCategoria.ObtenerPorId(modelo.CategoriaId, usuarioId);

            if (categoria is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var transaccion = mapper.Map<Transaccion>(modelo);

            if (modelo.TipoOperacionId == TipoOperacion.Gasto)
            {
                transaccion.Monto *= -1;
            }

            await repositorioTransacciones.Actualizar(transaccion, modelo.MontoAnterior, modelo.CuentaAnteriorId);

            if (string.IsNullOrEmpty(modelo.UrlRetorno))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return LocalRedirect(modelo.UrlRetorno);
            }
           
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerCuentas(int usuarioId)
        {
            var cuentas = await repositorioCuenta.Buscar(usuarioId);
            return cuentas.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerCategorias(int usuarioId, TipoOperacion tipoOperacion)
        {
            var categorias = await repositorioCategoria.Obtener(usuarioId, tipoOperacion);
            return categorias.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
        }

        [HttpPost]
        public async Task<IActionResult> ObtenerCategorias([FromBody] TipoOperacion tipoOperacion)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var categorias = await ObtenerCategorias(usuarioId, tipoOperacion);
            return Ok(categorias);
        }

        [HttpPost]
        public async Task<IActionResult> Borrar(int id, string urlRetorno = null)
		{
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            var transaccion = await repositorioTransacciones.ObtenerPorId(id, usuarioId);

			if (transaccion is null)
			{
                return RedirectToAction("NoEncontrado", "Home");
            }

            await repositorioTransacciones.Borrar(id);
            if (string.IsNullOrEmpty(urlRetorno))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return LocalRedirect(urlRetorno);
            }

        }
    }
}
