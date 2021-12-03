using AsisPas.Helpers;
using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace AsisPas.Entitys
{
    /// <summary>
    /// almacena los datos de usuario
    /// </summary>
    public class Usuario : MD5
    {
        #region propiedades
        /// <summary>
        /// id en base de datos
        /// </summary>
        [Key]
        public int id { get; set; }
        /// <summary>
        /// prop nav
        /// </summary>
        public IdentityUser user { get; set; }
        /// <summary>
        /// Usuario al que pertenece
        /// </summary>
        [Required]
        public string userid { get; set; }
        /// <summary>
        /// nombre del usuario
        /// </summary>
        [Required(ErrorMessage ="El Nombre es obligatorio")]
        [StringLength(100, ErrorMessage ="no mas de 100 caracteres")]
        public string Nombres { get; set; }
        /// <summary>
        /// apellidos del usuario
        /// </summary>
        [StringLength(100, ErrorMessage ="no mas de 100 caracteres")]
        [Required(ErrorMessage ="El Apellido es obligatorio")]
        public string Apellidos { get; set; }
        /// <summary>
        /// identificacion del usuario
        /// </summary>
        [StringLength(12, ErrorMessage ="no mas de 12 caracteres")]
        [Required(ErrorMessage = "El Rut es obligatorio")]
        public string Rut { get; set; }
        /// <summary>
        /// correo electronico del usuario
        /// </summary>
        [Required(ErrorMessage ="el Correo es necesario")]
        [EmailAddress]
        [StringLength(100, ErrorMessage = "no mas de 100 caracteres")]
        public string Email { get; set; }
        /// <summary>
        /// numero telefonico
        /// </summary>
        [StringLength(15, ErrorMessage ="no mas de 15 caracteres")]
        public string telefono { get; set; }
        /// <summary>
        /// Hash generado del usuario
        /// </summary>
        public string Hash { get; set; }
        /// <summary>
        /// Salt para la desencriptacion de datos
        /// </summary>
        public string Salt { get; set; }
        #endregion

        #region funciones para complementar la data de seguridad
        /// <summary>
        /// para el llenado de hash y salt
        /// </summary>
        public void Complete()
        {
            this.Salt = GenerarKey();
            this.Hash = Encriptar($"{this.Nombres}||{this.Apellidos}||{this.Email}||{this.Rut}",this.Salt);
        }
        /// <summary>
        /// me retorna un objeto con los datos del hash
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public Usuario verData(string hash, string salt)
        {
            try
            {
                Usuario usuario = new Usuario();
                usuario.Hash = hash;
                usuario.Salt = salt;

                var resp = Desencriptar(hash, salt);
                var all = resp.Split("||");
                if (all.Length < 5)
                    return null;

                usuario.Nombres = all[0];
                usuario.Apellidos = all[1];
                usuario.Email = all[2];
                usuario.Email = all[3];
                usuario.Rut = all[4];

                return usuario;

            }catch(Exception ex)
            {
                Console.WriteLine("Error en la desencriptacion de datos");
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        #endregion

    }
}
