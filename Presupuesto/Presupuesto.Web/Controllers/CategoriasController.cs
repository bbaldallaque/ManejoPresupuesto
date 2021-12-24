using Microsoft.AspNetCore.Mvc;
using Presupuesto.Web.Models;
using Presupuesto.Web.Servicio;

namespace Presupuesto.Web.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly IRepositorioCategoria repositorioCategoria;
        private readonly IServicioUsuarios servicioUsuarios;

        public CategoriasController(IRepositorioCategoria repositorioCategoria, IServicioUsuarios servicioUsuarios)
        {
            this.repositorioCategoria = repositorioCategoria;
            this.servicioUsuarios = servicioUsuarios;
        }
        public async Task<IActionResult> Index()
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var categorias = await repositorioCategoria.Obtner(usuarioId);
            return View(categorias);
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Categoria categoria)
        {
            if (!ModelState.IsValid)
            {
                return View(categoria);
            }

            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            categoria.UsuarioId = usuarioId;
            await repositorioCategoria.Crear(categoria);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Editar(int id)
		{
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var categoria = await repositorioCategoria.ObtnerPorId(id, usuarioId);

			if (categoria is null)
			{
                return RedirectToAction("NoEncontrado", "Home");
            }

            return View(categoria);
		}

        [HttpPost]
        public async Task<IActionResult> Editar(Categoria categoriaEditar)
		{
			if (!ModelState.IsValid)
			{
                return View(categoriaEditar);
			}

            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var categoria = await repositorioCategoria.ObtnerPorId(categoriaEditar.Id, usuarioId);

			if (categoria is null)
			{
                return RedirectToAction("NoEncontrado", "Home");
            }

            categoriaEditar.UsuarioId = usuarioId;  
            await repositorioCategoria.Actualizar(categoriaEditar);
            return RedirectToAction("Index");
		}
    }
}
