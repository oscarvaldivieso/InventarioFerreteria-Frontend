namespace Ferreteria_Frontend.Models
{
    public class LoginViewModel
    {
        public int Usua_Id { get; set; }
        public string? Usua_Nombre { get; set; }
        public string? Usua_Clave { get; set; }

        public int Empl_Id { get; set; }
        public int Role_Id { get; set; }
        public string? Empl_NombreCompleto { get; set; }
        public bool Usua_EsAdmin { get; set; }
    }
}
