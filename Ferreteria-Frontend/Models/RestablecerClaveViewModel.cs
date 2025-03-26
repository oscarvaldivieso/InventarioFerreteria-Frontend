using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ferreteria_Frontend.Models
{
    public class RestablecerClaveViewModel
    {
        public int Usua_Id { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Ingrese la nueva contraseña.")]
        [DataType(DataType.Password)]
        public string NuevaClave { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Confirme la nueva contraseña.")]
        [DataType(DataType.Password)]
        [Compare("NuevaClave", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmarClave { get; set; }
    }
}
