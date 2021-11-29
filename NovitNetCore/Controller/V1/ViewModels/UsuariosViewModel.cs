
namespace NovitNetCore.Controller.V1.ViewModels
{
    public class UsuariosViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool Estado { get; set; }

    }
}
