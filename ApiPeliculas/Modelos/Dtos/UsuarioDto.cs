namespace ApiPeliculas.Modelos.Dtos
{
    public class UsuarioDto
    {
        public int Id { get; set; } // Autoincrementable
        public string NombreUsuario { get; set; }
        public string Nombre { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
