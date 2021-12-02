using AsisPas.Entitys;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsisPas.Data
{
    /// <summary>
    /// contexto de datos
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext
    {
    #region constructor
        /// <summary>
        /// contexto de datos
        /// </summary>
        /// <param name="options"></param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        #endregion

        #region tablan de negocio
        /// <summary>
        /// empresas
        /// </summary>
        public DbSet<Empresa> Empresas { get; set; }
        /// <summary>
        /// sedes que contiene una empresa
        /// </summary>
        public DbSet<Sedes> Sedes { get; set; }
        /// <summary>
        /// Horarios
        /// </summary>
        public DbSet<Horario> Horarios { get; set; }
        #endregion

        #region on model creating
        /// <summary>
        /// para sobreescibir algunos valores en la base de datos
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        #endregion

        #region creacion de llaves

        #endregion

        #region seed data


        #endregion


    }
}
