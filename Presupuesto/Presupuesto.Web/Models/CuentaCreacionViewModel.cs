using Microsoft.AspNetCore.Mvc.Rendering;

namespace Presupuesto.Web.Models
{
    public class CuentaCreacionViewModel : Cuenta
    {
        public IEnumerable<SelectListItem> TiposCuentas { get; set; }
    }
}
