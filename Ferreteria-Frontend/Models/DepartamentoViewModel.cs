using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ferreteria_Frontend.Models
{
    public class DepartamentoViewModel
    {
        [Display(Name = "Código")]
        public string Depa_Codigo { get; set; }

        [Display(Name = "Departamento")]
        public string Depa_Descripcion { get; set; }

        public int Usua_Creacion { get; set; }

        public DateTime Feca_Creacion { get; set; }

        public int? Usua_Modificacion { get; set; }

        public DateTime? Feca_Modificacion { get; set; }

        [NotMapped]
        public string? UsuaC_Nombre { get; set; }
        [NotMapped]
        public string? UsuaM_Nombre { get; set; }

        //public virtual tbUsuarios Usua_CreacionNavigation { get; set; }

        //public virtual tbUsuarios Usua_ModificacionNavigation { get; set; }

        //public virtual ICollection<tbMunicipios> tbMunicipios { get; set; } = new List<tbMunicipios>();
        
    }
}
