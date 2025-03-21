﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ferreteria_Frontend.Models
{
    public class MarcaViewModel
    {
        [Display(Name = "Id")]
        public int Marc_Id { get; set; }

        [Display(Name = "Marca")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Marc_Descripcion { get; set; }

        public int Usua_Creacion { get; set; }

        public DateTime Feca_Creacion { get; set; }

        public int? Usua_Modificacion { get; set; }

        public DateTime? Feca_Modificacion { get; set; }

        public bool? Marc_Estado { get; set; }

        [NotMapped]
        public string? UsuaC_Nombre { get; set; }

        [NotMapped]
        public string? UsuaM_Nombre { get; set; }
    }
}
