using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiPeliculas.Modelos
{
    public class Usuario
    {
        [Key] // Define la propiedad como id y la hace incremental
        public int Id { get; set; } // Autoincrementable
        public string NombreUsuario { get; set; }
        public string Nombre { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

    }
}
