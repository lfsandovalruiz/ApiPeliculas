using ApiPeliculas.Data;
using ApiPeliculas.Modelos;
using ApiPeliculas.Repositorio.IRepositorio;

namespace ApiPeliculas.Repositorio
{
    public class CategoriaRepositorio : ICategoriaRepositorio
    {
        private readonly ApplicationDbContext _context;
        public CategoriaRepositorio(ApplicationDbContext context)
        {
            _context = context;
        }
        public bool ActualizarCategoria(Categoria categoria)
        {
            categoria.FechaCreacion = DateTime.Now; // nos permite no enviar este parámetro jeje
            _context.Categoria.Update(categoria);
            return Guardar();
        }

        public bool BorrarCategoria(Categoria categoria)
        {
            _context.Categoria.Remove(categoria);
            return Guardar();
        }

        public bool CrearCategoria(Categoria categoria)
        {
            categoria.FechaCreacion = DateTime.Now; // nos permite no enviar este parámetro jeje
            _context.Categoria.Add(categoria);
            return Guardar();
        }

        public bool ExisteCategoria(string nombre)
        {
            return _context.Categoria.Any(categoria => categoria.Nombre.ToLower() == nombre.Trim().ToLower());
        }

        public bool ExisteCategoria(int categoriaId)
        {
            return _context.Categoria.Any(categoria => categoria.Id == categoriaId);
        }

        public Categoria GetCategoria(int categoriaId)
        {
            return _context.Categoria.FirstOrDefault(categoria => categoria.Id == categoriaId);
        }

        public ICollection<Categoria> GetCategorias()
        {
            return _context.Categoria.OrderBy(categoria => categoria.Nombre).ToList();
        }

        public bool Guardar()
        {
            return _context.SaveChanges() > 0;
        }
    }
}
