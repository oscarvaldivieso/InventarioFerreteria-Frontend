namespace Ferreteria_Frontend.Models
{
    public class LoginViewModel
    {
        public int Usua_Id { get; set; }
        public string Usuario { get; set; }
        public string Contrasena { get; set; }

        public int Empleado_Id { get; set; }
        public int Rol { get; set; }
        public string Nombre_Empleado { get; set; }
        public bool Admin { get; set; }
    }
}
