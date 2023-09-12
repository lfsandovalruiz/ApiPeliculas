using ApiPeliculas.Modelos;
using ApiPeliculas.Modelos.Dtos;
using AutoMapper;

namespace ApiPeliculas.PeliculasMappers
{
    public class PeliculasMapper : Profile // Paquete NuGet: automapper
    {
        public PeliculasMapper()
        {

            CreateMap<Categoria, CategoriaDto>().ReverseMap();

            CreateMap<Categoria, CrearCategoriaDto>().ReverseMap();

            CreateMap<Pelicula, PeliculasDTO>().ReverseMap();

        }
    }
}
