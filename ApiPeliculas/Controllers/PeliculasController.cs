using ApiPeliculas.Modelos;
using ApiPeliculas.Modelos.Dtos;
using ApiPeliculas.Repositorio.IRepositorio;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiPeliculas.Controllers
{
    [Route("api/peliculas")]
    [ApiController]
    public class PeliculasController : ControllerBase
    {

        private readonly IPeliculaRepositorio _pelRepo;
        private readonly IMapper _mapper;

        public PeliculasController(IPeliculaRepositorio pelRepo, IMapper mapper)
        {
            _pelRepo = pelRepo;
            _mapper = mapper;
        }

        [HttpGet]
        // Sirven para que swagger muestre los posibles códigos de estado que esta solicitud pueda retornar
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetPeliculas()
        {
            var listaPeliculas = _pelRepo.GetPeliculas();
            var listaPeliculasDto = new List<PeliculasDTO>();
            foreach (var pelicula in listaPeliculas)
            {
                listaPeliculasDto.Add(_mapper.Map<PeliculasDTO>(pelicula));
            }
            return Ok(listaPeliculasDto);
        }

        [HttpGet("{peliculaId:int}", Name = "GetPelicula")] // Name se usa para que se llame a este método desde otras partes del código (solo para "CreatedAtRoute" por que para "CreatedAtAction" da igual)
        // Sirven para que swagger muestre los posibles códigos de estado que esta solicitud pueda retornar
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetPelicula(int peliculaId)
        {
            var itemPelicula = _pelRepo.GetPelicula(peliculaId);

            if (itemPelicula == null) return NotFound();

            var itemPeliculaDto = _mapper.Map<PeliculasDTO>(itemPelicula);

            return Ok(itemPeliculaDto);
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(PeliculasDTO))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public IActionResult CrearPelicula([FromBody] PeliculasDTO peliculaDto)
        {

            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (peliculaDto == null) return BadRequest(ModelState);

            if (_pelRepo.ExistePelicula(peliculaDto.Nombre))
            {
                ModelState.AddModelError("", "La pelicula ya existe");
                return StatusCode(StatusCodes.Status409Conflict, ModelState);
                //return Conflict(ModelState); // otra manera de hacerlo
            }

            var pelicula = _mapper.Map<Pelicula>(peliculaDto);

            if (!_pelRepo.CrearPelicula(pelicula))
            {
                ModelState.AddModelError("", "No se pudo guardar la pelicula");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }

            // Explicación importante: En CreatedAtRoute, los primeros 2 argumentos son para que SE CREE UNA URL EN EL ENCABEZADO "LOCATION" PARA QUE EL RECURSO CREADO PUEDA SER OBTENIDO A TRAVEZ DE UNA UNA SOLICITUD HTTP (EJEMPLO: https://localhost:7026/api/categorias/1)
            // El segundo argumento es para que en el cuerpo de la respuesta se encuentre el recurso creado en formato JSON
            return CreatedAtRoute("GetPelicula", new { peliculaId = pelicula.Id }, pelicula); // NECESITA "Name = GetPelicula" en GetPelicula
            // Otras maneras de hacerlo
            //return CreatedAtAction("GetPelicula", new { categoriaId = categoria.Id }, categoria); // No cecesita "Name =GetPelicula" en GetPelicula
            //return CreatedAtAction(nameof(GetPelicula), new { categoriaId = categoria.Id }, categoria);
        }

        [HttpPatch("{peliculaId:int}", Name = "ActualizarPatchPelicula")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ActualizarPatchPelicula(int peliculaId, [FromBody] PeliculasDTO peliculaDto)
        {

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var pelicula = _mapper.Map<Pelicula>(peliculaDto);

            if (!_pelRepo.ActualizarPelicula(pelicula))
            {
                ModelState.AddModelError("", "No se pudo actualizar la pelicula");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{peliculaId:int}", Name = "BorrarPelicula")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult BorrarPelicula(int peliculaId)
        {

            if (!_pelRepo.ExistePelicula(peliculaId)) return NotFound();

            var pelicula = _pelRepo.GetPelicula(peliculaId);

            if (!_pelRepo.BorrarPelicula(pelicula))
            {
                ModelState.AddModelError("", "No se pudo borrar la pelicuala");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }

            return NoContent();
        }

        [HttpGet("GetPeliculasEnCategoria/{categoriaId:int}")]
        public IActionResult GetPeliculasEnCategoria(int categoriaId)
        {
            var listaPeliculas = _pelRepo.GetPeliculasEnCategoria(categoriaId);

            if (listaPeliculas == null) return NotFound();

            List<PeliculasDTO> listaPeliculasDto = new List<PeliculasDTO>();

            foreach (var pelicula in listaPeliculas)
            {
                listaPeliculasDto.Add(_mapper.Map<PeliculasDTO>(pelicula));
            }
                
            return Ok(listaPeliculasDto);
        }

        [HttpGet("Buscar")]
        public IActionResult Buscar(string nombre)
        {

            try
            {

                var resultado = _pelRepo.BuscarPelicula(nombre.Trim());

                if (resultado.Any())
                {
                    return Ok(resultado);
                }

                return NotFound();

            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Error recuperando datos");

            }
          
        }

    }
}
