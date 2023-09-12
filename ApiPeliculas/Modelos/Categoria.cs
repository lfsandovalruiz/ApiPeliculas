using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Modelos
{
    public class Categoria
    {
        [Key] // Define la propiedad como id y la hace incremental
        public int Id { get; set; } // Autoincrementable
        [Required]
        public string Nombre { get; set; }
        public DateTime FechaCreacion { get; set; } // Se asigna por defecto en la clase repositorio

    }
}
