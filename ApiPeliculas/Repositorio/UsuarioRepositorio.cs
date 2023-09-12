using ApiPeliculas.Data;
using ApiPeliculas.Modelos;
using ApiPeliculas.Modelos.Dtos;
using ApiPeliculas.Repositorio.IRepositorio;
using XSystem.Security.Cryptography;

namespace ApiPeliculas.Repositorio
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {

        private readonly ApplicationDbContext _db;

        public UsuarioRepositorio(ApplicationDbContext db) {
            _db = db;
        }


        public Usuario GetUsuario(int usuarioId)
        {
            return _db.Usuario.FirstOrDefault(usuario => usuario.Id == usuarioId);
        }
        public ICollection<Usuario> GetUsuario()
        {
            return _db.Usuario.OrderBy(usuario => usuario.NombreUsuario).ToList();
        }

        public bool isUniqueUser(string nombreUsuario)
        {
            var usuarioBd = _db.Usuario.FirstOrDefault(usuario => usuario.NombreUsuario == nombreUsuario);
            return usuarioBd == null;
        }

        public async Task<Usuario> Registro(UsuarioRegistroDto usuarioRegistroDto)
        {
            var passwordCifrado = obtenerMd5(usuarioRegistroDto.Password);

            Usuario usuario = new ()
            {
                NombreUsuario = usuarioRegistroDto.NombreUsuario,
                Password = passwordCifrado,
                Nombre = usuarioRegistroDto.Nombre,
                Role = usuarioRegistroDto.Role
            };

            _db.Add(usuario);

            await _db.SaveChangesAsync();

            return usuario;
        }

        public Task<UsuarioLoginRespuestaDto> Login(UsuarioLoginDto usuarioLoginDto)
        {
            throw new NotImplementedException();
        }

        public static string obtenerMd5(string valor)
        {
            // Paquete XAct.Core.PCL
            MD5CryptoServiceProvider x = new MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.UTF8.GetBytes(valor);
            data = x.ComputeHash(data);
            string resp = string.Empty;
            for (int i = 0; i < data.Length; i++) resp += data[i].ToString("x2").ToLower();
            return resp;
        }
    }
}
