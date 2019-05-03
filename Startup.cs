using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IPTGram.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Logging;

namespace IPTGram
{
    public class Startup
    {
        private readonly IConfiguration config;

        public Startup(IConfiguration config)
        {
            this.config = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Configurar a base de dados. Escolher apenas um entre `UseSqlServer` e `UseMySQL`.
            // As connection strings estão no ficheiro `appsettings.json`.
            services.AddDbContext<IPTGramDb>(dbContextOptions =>
            {
                // Para usar SQL Server
                dbContextOptions.UseSqlServer(config.GetConnectionString("DefaultConnection"));

                // Para usar MySQL
                // dbContextOptions.UseMySQL(config.GetConnectionString("DefaultConnection"));
            });

            // Adicionar o ASP.NET Core Identity com Entity Framework.
            services.AddIdentity<User, IdentityRole>(identityOptions =>
            {
                identityOptions.Password.RequireUppercase = false;
                identityOptions.Password.RequireLowercase = false;
                identityOptions.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<IPTGramDb>();

            // Adicionar autenticação por cookies (notar que o AccountController precisa de ser criado,
            // assim como configurar o [Authorize]).
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(cookieOptions =>
                {
                    cookieOptions.LoginPath = new PathString("/api/account/login");
                    cookieOptions.LogoutPath = new PathString("/api/accont/logout");
                });

            // Adicionar o MVC.
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // Configurar a classe AppOptions com os dados do ficheiro de configurações
            // (Usar com IOptions<AppOptions> nos parâmetros dos Controllers).
            services.Configure<AppOptions>(config.GetSection("IPTGram"));

            // Adicionar uma classe que faz o Seed da base de dados.
            services.AddTransient(provider =>
            {
                return new DbInitializer(
                    provider.GetRequiredService<IPTGramDb>(),
                    provider.GetRequiredService<UserManager<User>>(),
                    provider.GetRequiredService<ILogger<DbInitializer>>()
                );
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Inicializar a base de dados (Seed).
            using (var scope = app.ApplicationServices.CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<DbInitializer>().Seed().Wait();
            }

            // Permitir que outros servidores acedam aos dados.
            app.UseCors(corsOptions =>
            {
                corsOptions.AllowAnyHeader();
                corsOptions.AllowAnyMethod();
                corsOptions.AllowCredentials();
                corsOptions.AllowAnyOrigin();
            });

            // Adicionar autenticação.
            app.UseAuthentication();

            // Adicionar o MVC.
            app.UseMvc();
        }
    }
}
