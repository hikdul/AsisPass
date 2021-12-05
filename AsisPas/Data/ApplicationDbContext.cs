using AsisPas.Entitys;
using Microsoft.AspNetCore.Identity;
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
        // == seccion principal

        /// <summary>
        /// empresas
        /// </summary>
        public DbSet<Empresa> Empresas { get; set; }
        /// <summary>
        /// sedes que contiene una empresa
        /// </summary>
        public DbSet<Sedes> Sedes { get; set; }
        
        // == manejo de horarios
        
        /// <summary>
        /// Horarios
        /// </summary>
        public DbSet<Horario> Horarios { get; set; }
        /// <summary>
        /// admo de horarios
        /// </summary>
        public DbSet<AdmoHorario> AdmoHorarios { get; set; }

        // == manejo de usuarios

        /// <summary>
        /// para almacenar los datos de un usuario en especifico
        /// </summary>
        public DbSet<Usuario> Usuarios { get; set; }
        /// <summary>
        /// para almacenar los usuarios con el rol de administradores de empresas
        /// </summary>
        public DbSet<AdmoEmpresas> AdmoEmpresas { get; set; }
        /// <summary>
        /// fiscales
        /// </summary>
        public DbSet<Fiscal> Fiscales { get; set; }
        /// <summary>
        /// empleados
        /// </summary>
        public DbSet<Empleado> Empleados { get; set; }
        /// <summary>
        /// almacena el admo sistema
        /// </summary>
        public DbSet<AdmoSistema> AdmoSistema { get; set; }
        /// <summary>
        /// para almacenar las marcas una a una
        /// </summary>
        public DbSet<Marca> Marcaciones { get; set; }
        /// <summary>
        /// para almacenar los puntos de ingresos por medio de la api
        /// </summary>
        public DbSet<Gate> Gates { get; set; }
        #endregion

        #region on model creating
        /// <summary>
        /// para sobreescibir algunos valores en la base de datos
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // RolesYSU(builder);
            AsignacionLlaves(builder);
           
            base.OnModelCreating(builder);
        }

        #endregion

        #region creacion de llaves

        #endregion

        #region Asignacion de llaves
        private void AsignacionLlaves(ModelBuilder builder)
        {
           // builder.Entity<AdmoEmpresas>().HasKey(x => new { x.Empresaid, x.userid });

        }

        #endregion

        #region Roles de usuario y SA
        /// <summary>
        /// funcion que crea mis roles y el SA
        /// </summary>
        /// <param name="builder"></param>
        private void RolesYSU(ModelBuilder builder)
        {
            var hasher = new PasswordHasher<IdentityUser>();
            var SuperAdmin = new IdentityUser()
            {
                Id = Guid.NewGuid().ToString(),
                Email = "desarrollo@automatismoslau.cl",
                NormalizedEmail = "desarrollo@automatismoslau.cl".ToUpper(),
                UserName = "desarrollo@automatismoslau.cl",
                NormalizedUserName = "desarrollo@automatismoslau.cl".ToUpper(),
                EmailConfirmed = true,
                PhoneNumber = "+56 9 3315 8879",
                PhoneNumberConfirmed = true,
                PasswordHash = hasher.HashPassword(null, "Alau123#")

            };

            var SuperAdminJefe = new IdentityUser()
            {
                Id = Guid.NewGuid().ToString(),
                Email = "pcortes@automatismoslau.cl",
                NormalizedEmail = "pcortes@automatismoslau.cl".ToUpper(),
                UserName = "pcortes@automatismoslau.cl",
                NormalizedUserName = "pcortes@automatismoslau.cl".ToUpper(),
                EmailConfirmed = true,
                PhoneNumber = "+56 9 9499 8131",
                PhoneNumberConfirmed = true,
                PasswordHash = hasher.HashPassword(null, "Alau123#")

            };

            var RoleSuperAdmin = new IdentityRole()
            {

                Id = "5e2a8afe-fe62-4598-a773-1f89ce15af3a",
                Name = "SuperAdmin",
                NormalizedName = "SuperAdmin".ToUpperInvariant()

            };

            var RoleAdmin = new IdentityRole()
            {

                Id = "16e33ec0-7fa5-4c84-b073-15ce21f4e60a",
                Name = "Admin",
                NormalizedName = "ADMIN".ToUpperInvariant()

            };

            var RoleSuperv = new IdentityRole()
            {

                Id = "dddb7443-d5c8-4b38-ba6e-abd0ef20d9f3",
                Name = "Fiscal",
                NormalizedName = "FISCAL".ToUpperInvariant()

            };

            var RoleEmpresa = new IdentityRole()
            {

                Id = "073c51e1-fdab-4349-83d5-5b34cc82e541",
                Name = "Empresa",
                NormalizedName = "Empresa".ToUpperInvariant()

            };

            var RoleEmpleado = new IdentityRole()
            {

                Id = "d95a2f3d-531f-4466-b6fe-2a69a6e49e5a",
                Name = "Empleado",
                NormalizedName = "Empleado".ToUpperInvariant()

            };

            var DesarroloAlau = new IdentityUserRole<string>()
            {
                RoleId = RoleSuperAdmin.Id,
                UserId = SuperAdmin.Id
            };


            var pcortes = new IdentityUserRole<string>()
            {
                RoleId = RoleSuperAdmin.Id,
                UserId = SuperAdminJefe.Id
            };

            builder.Entity<IdentityUser>().HasData(SuperAdmin);
            builder.Entity<IdentityUser>().HasData(SuperAdminJefe);

            builder.Entity<IdentityRole>().HasData(RoleSuperAdmin);
            builder.Entity<IdentityRole>().HasData(RoleAdmin);
            builder.Entity<IdentityRole>().HasData(RoleEmpresa);
            builder.Entity<IdentityRole>().HasData(RoleSuperv);
            builder.Entity<IdentityRole>().HasData(RoleEmpleado);

            builder.Entity<IdentityUserRole<string>>().HasData(DesarroloAlau);
            builder.Entity<IdentityUserRole<string>>().HasData(pcortes);

        }


        #endregion


    }
}
