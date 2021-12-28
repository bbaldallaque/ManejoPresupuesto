using System.ComponentModel.DataAnnotations;

namespace Presupuesto.Web.Models
{
    public class Transaccion
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; }

        [Display(Name = "Fecha Transaccion")]
        [DataType(DataType.Date)]
        public DateTime FechaTransaccion { get; set; } = DateTime.Today;
        //DateTime.Parse(DateTime.Now.ToString("yyy-MM-dd hh:MM tt"));
        //DateTime.Parse(DateTime.Now.ToString("g"));

        public decimal Monto { get; set; }

        [Display(Name = "Categoria")]
        [Range(1, maximum: int.MaxValue, ErrorMessage ="Debe seleccionar una categoria")]
        public int CategoriaId { get; set; }
        [StringLength(maximumLength: 1000, ErrorMessage ="La nota no puede pasar de {1} carateres")]
        public string Nota { get; set; }

        [Display(Name = "Cuenta")]
        [Range(1, maximum: int.MaxValue, ErrorMessage = "Debe seleccionar una cuenta")]
        public int CuentaId { get; set; }

        [Display(Name = "Tipo operacion")]
        public TipoOperacion TipoOperacionId { get; set; } = TipoOperacion.Ingreso;
    }
}
