using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsisPas
{
    /// <summary>
    /// program class
    /// </summary>
    public class Program
    {
        /// <summary>
        /// main function
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }
        /// <summary>
        /// host builder
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
           Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((env, conf) => {
                // env =>  es el ambiente de configuracion
                // conf => son las configuraciones
                // //aqui obtenenos en nombre del ambiente
                var ambiente = env.HostingEnvironment.EnvironmentName;

                // // usamos nuestros jason
                conf.AddJsonFile("appsettiongs.json", optional: true, reloadOnChange: true);
                conf.AddJsonFile($"appsettiongs{ambiente}.json", optional: true, reloadOnChange: true);
                // // permitimos el uso de variables de ambiente
                conf.AddEnvironmentVariables();
                // // permitimos la entrada de valores por medio de la linea de comandos por medios de dotnetcli
                if (args != null)
                    conf.AddCommandLine(args);
                // => de este modo la configuracion por defecto del netcore quedo deshabilitada
            }).ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
                // esta linea se deja solo para exportar en el servidor de pruebas
                //webBuilder.UseUrls("http://*:2945");
            });
    }
}
