using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Modelos.Dtos
{
    public class CrearCategoriaDto
    {
        [Required(ErrorMessage = "Campo requerido.")]
        [MaxLength(60, ErrorMessage = "El máximo de caracteres es de 60.")]
        public string Nombre { get; set; }
    }
}
