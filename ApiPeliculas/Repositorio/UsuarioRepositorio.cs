using ApiPeliculas.Data;
using ApiPeliculas.Modelos;
using ApiPeliculas.Modelos.Dtos;
using ApiPeliculas.Repositorio.IRepositorio;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using XSystem.Security.Cryptography;
using System.Security.Claims;

namespace ApiPeliculas.Repositorio
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {

        private readonly ApplicationDbContext _db;
        private readonly string claveSecreta;

        public UsuarioRepositorio(ApplicationDbContext db, IConfiguration configuration) {
            _db = db;
            claveSecreta = configuration.GetValue<string>("ApiSettings:Secreta");
        }


        public Usuario GetUsuario(int usuarioId)
        {
            return _db.Usuario.FirstOrDefault(usuario => usuario.Id == usuarioId);
        }
        public ICollection<Usuario> GetUsuarios()
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

        public UsuarioLoginRespuestaDto Login(UsuarioLoginDto usuarioLoginDto)
        {

            var passwordCifrado = obtenerMd5(usuarioLoginDto.Password);

            var usuario = _db.Usuario.FirstOrDefault(u =>
                u.NombreUsuario.ToLower() == usuarioLoginDto.NombreUsuario.ToLower()
                &&
                u.Password == passwordCifrado
            );

            if (usuario == null) return new UsuarioLoginRespuestaDto
            {
                Token = string.Empty,
                Usuario = null
            };

            // Creación del token
            var manejadorToken = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(claveSecreta);

            var tokenDescryptor = new SecurityTokenDescriptor() {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(type: ClaimTypes.Name, usuario.NombreUsuario),
                    new Claim(type: ClaimTypes.Role, usuario.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = manejadorToken.CreateToken(tokenDescryptor);

            UsuarioLoginRespuestaDto usuarioLoginRespuestaDto = new UsuarioLoginRespuestaDto() {
                Token = manejadorToken.WriteToken(token),
                Usuario = usuario
            };

            return usuarioLoginRespuestaDto;

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
