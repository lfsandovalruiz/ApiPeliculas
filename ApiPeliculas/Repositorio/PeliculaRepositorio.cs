using ApiPeliculas.Data;
using ApiPeliculas.Modelos;
using ApiPeliculas.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace ApiPeliculas.Repositorio
{
    public class PeliculaRepositorio : IPeliculaRepositorio
    {
        private readonly ApplicationDbContext _context;
        public PeliculaRepositorio(ApplicationDbContext context)
        {
            _context = context;
        }
        public bool ActualizarPelicula(Pelicula pelicula)
        {
            pelicula.FechaCreacion = DateTime.Now; // nos permite no enviar este parámetro jeje
            _context.Pelicula.Update(pelicula);
            return Guardar();
        }

        public bool BorrarPelicula(Pelicula pelicula)
        {
            _context.Pelicula.Remove(pelicula);
            return Guardar();
        }

        public ICollection<Pelicula> BuscarPelicula(string nombre)
        {
            //return _context.Pelicula.Where(pelicula => pelicula.Nombre == nombre).ToList();
            IQueryable<Pelicula> query = _context.Pelicula;

            if (!string.IsNullOrEmpty(nombre)) {
                query = query.Where(pelicula => pelicula.Nombre.Contains(nombre) || pelicula.Descripcion.Contains(nombre));
            }

            return query.ToList();
        }

        public bool CrearPelicula(Pelicula pelicula)
        {
            pelicula.FechaCreacion = DateTime.Now; // nos permite no enviar este parámetro jeje
            _context.Pelicula.Add(pelicula);
            return Guardar();
        }

        public bool ExistePelicula(string nombre)
        {
            return _context.Pelicula.Any(pelicula => pelicula.Nombre.ToLower() == nombre.Trim().ToLower());
        }

        public bool ExistePelicula(int peliculaId)
        {
            return _context.Pelicula.Any(pelicula => pelicula.Id == peliculaId);
        }

        public Pelicula GetPelicula(int peliculaId)
        {
            return _context.Pelicula.FirstOrDefault(pelicula => pelicula.Id == peliculaId);
        }

        public ICollection<Pelicula> GetPeliculas()
        {
            return _context.Pelicula.OrderBy(pelicula => pelicula.Nombre).ToList();
        }

        public ICollection<Pelicula> GetPeliculasEnCategoria(int categoriaId)
        {
            return _context.Pelicula.Include(pelicula => pelicula.Categoria).Where(pelicula => pelicula.categoriaId == categoriaId).ToList();
        }

        public bool Guardar()
        {
            return _context.SaveChanges() > 0;
        }
    }
}
