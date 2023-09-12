using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Modelos.Dtos
{
    public class CategoriaDto // Para obtener categoria
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo requerido.")]
        [MaxLength(60, ErrorMessage = "El máximo de caracteres es de 60.")]
        public string Nombre { get; set; }
    }
}
