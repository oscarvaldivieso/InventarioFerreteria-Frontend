using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ferreteria_Frontend.Models
{
    public class UsuarioViewModel
    {
        public int Usua_Id { get; set; }

        public string Usua_Nombre { get; set; }

        public string Usua_Clave { get; set; }

        public int Empl_Id { get; set; }

        [NotMapped]
        public string Empl_NombreCompleto { get; set; }

        public int Role_Id { get; set; }
        [NotMapped]
        public string Role_Descripcion { get; set; }

        public bool Usua_EsAdmin { get; set; }

        public int Usua_Creacion { get; set; }

        public DateTime Feca_Creacion { get; set; }

        public int? Usua_Modificacion { get; set; }

        public DateTime? Feca_Modificacion { get; set; }

        public bool? Usua_Estado { get; set; }
    }
}
