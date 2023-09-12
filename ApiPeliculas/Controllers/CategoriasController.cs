using ApiPeliculas.Modelos;
using ApiPeliculas.Modelos.Dtos;
using ApiPeliculas.Repositorio.IRepositorio;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace ApiPeliculas.Controllers
{
    [ApiController]
    [Route("api/categorias")]
    //[Route("api/[controller]")]
    public class CategoriasController : ControllerBase
    {
        private readonly ICategoriaRepositorio _ctRepo;
        private readonly IMapper _mapper;

        public CategoriasController(ICategoriaRepositorio ctRepo, IMapper mapper)
        {
            _ctRepo = ctRepo;
            _mapper = mapper;
        }

        [HttpGet]
        // Sirven para que swagger muestre los posibles códigos de estado que esta solicitud pueda retornar
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetCategorias() {
            var listaCategorias = _ctRepo.GetCategorias();
            var listaCategoriasDto = new List<CategoriaDto>();
            foreach (var categoria in listaCategorias) {
                listaCategoriasDto.Add(_mapper.Map<CategoriaDto>(categoria));
            }
            return Ok(listaCategoriasDto);
        }

        [HttpGet("{categoriaId:int}", Name = "GetCategoria")] // Name se usa para que se llame a este método desde otras partes del código (solo para "CreatedAtRoute" por que para "CreatedAtAction" da igual)
        // Sirven para que swagger muestre los posibles códigos de estado que esta solicitud pueda retornar
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetCategoria(int categoriaId)
        {
            var itemCategoria = _ctRepo.GetCategoria(categoriaId);

            if (itemCategoria == null) return NotFound();

            var itemCategoriaDto = _mapper.Map<CategoriaDto>(itemCategoria);

            return Ok(itemCategoriaDto);
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(CategoriaDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public IActionResult CrearCategoria([FromBody] CrearCategoriaDto crearCategoriaDto)
        {

            if(!ModelState.IsValid) return BadRequest(ModelState);

            if(crearCategoriaDto == null) return BadRequest(ModelState);

            if(_ctRepo.ExisteCategoria(crearCategoriaDto.Nombre))
            {
                ModelState.AddModelError("", "La categoría ya existe");
                return StatusCode(StatusCodes.Status409Conflict, ModelState);
                //return Conflict(ModelState); // otra manera de hacerlo
            }

            var categoria = _mapper.Map<Categoria>(crearCategoriaDto);

            if(!_ctRepo.CrearCategoria(categoria)) {
                ModelState.AddModelError("", "No se pudo guardar la categoría");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }

            // Explicación importante: En CreatedAtRoute, los primeros 2 argumentos son para que SE CREE UNA URL EN EL ENCABEZADO "LOCATION" PARA QUE EL RECURSO CREADO PUEDA SER OBTENIDO A TRAVEZ DE UNA UNA SOLICITUD HTTP (EJEMPLO: https://localhost:7026/api/categorias/1)
            // El segundo argumento es para que en el cuerpo de la respuesta se encuentre el recurso creado en formato JSON
            return CreatedAtRoute("GetCategoria", new { categoriaId = categoria.Id }, categoria); // NECESITA "Name =GetCategoria" en GetCategoria
            // Otras maneras de hacerlo
            //return CreatedAtAction("GetCategoria", new { categoriaId = categoria.Id }, categoria); // No cecesita "Name =GetCategoria" en GetCategoria
            //return CreatedAtAction(nameof(GetCategoria), new { categoriaId = categoria.Id }, categoria);
        }

        [HttpPatch("{categoriaId:int}", Name = "ActualizarPatchCategoria")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ActualizarPatchCategoria(int categoriaId, [FromBody] CategoriaDto categoriaDto)
        {

            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (categoriaDto == null || categoriaId != categoriaDto.Id) return BadRequest(ModelState);

            var categoria = _mapper.Map<Categoria>(categoriaDto);

            if (!_ctRepo.ActualizarCategoria(categoria))
            {
                ModelState.AddModelError("", "No se pudo actualizar la categoría");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{categoriaId:int}", Name = "BorrarCategoria")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult BorrarCategoria(int categoriaId)
        {

            if(!_ctRepo.ExisteCategoria(categoriaId)) return NotFound();

            var categoria = _ctRepo.GetCategoria(categoriaId);

            if (!_ctRepo.BorrarCategoria(categoria))
            {
                ModelState.AddModelError("", "No se pudo borrar la categoría");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }

            return NoContent();
        }

    }
}
