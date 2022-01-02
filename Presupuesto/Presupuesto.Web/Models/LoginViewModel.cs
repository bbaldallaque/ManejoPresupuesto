using System.ComponentModel.DataAnnotations;

namespace Presupuesto.Web.Models
{
	public class LoginViewModel
	{
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [EmailAddress(ErrorMessage = "El campo {0} debe de ser un correo electronico valido")]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

		public bool Recuerdame { get; set; }
	}
}
