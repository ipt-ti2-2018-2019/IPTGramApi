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

                // Para usar SQLite
                // dbContextOptions.UseSqlite(config.GetConnectionString("DefaultConnection"))
            });

            // Adicionar o ASP.NET Core Identity com Entity Framework.
            services.AddIdentity<User, IdentityRole>(identityOptions =>
            {
                identityOptions.Password.RequireUppercase = false;
                identityOptions.Password.RequireLowercase = false;
                identityOptions.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<IPTGramDb>();

            // Configurar autenticação por cookies (notar que o AccountController precisa de ser criado,
            // assim como configurar o [Authorize] nos Controllers e Actions).
            // Ver: https://github.com/ipt-ti2-2018-2019/ac-aula-12/blob/resolvido/exercicios/2-api-multas/ApiMultas/Controllers/AccountController.cs
            // para um exemplo de um AccountController.
            services.ConfigureApplicationCookie(cookieOptions =>
            {
                cookieOptions.Cookie.HttpOnly = true;

                cookieOptions.LoginPath = new PathString("/api/account/login");
                cookieOptions.LogoutPath = new PathString("/api/accont/logout");
                cookieOptions.Cookie.SecurePolicy = CookieSecurePolicy.None;
                cookieOptions.Cookie.SameSite = SameSiteMode.None;

                cookieOptions.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin = context =>
                    {
                        if (context.Request.Path.StartsWithSegments("/api"))
                        {
                            context.Response.Clear();
                            context.Response.StatusCode = 401;
                            return Task.CompletedTask;
                        }

                        context.Response.Redirect(context.RedirectUri);

                        return Task.CompletedTask;
                    }
                };
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

            // Página de erros em modo de desenvolvimento.
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
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

            // Adicionar suporte para ficheiros estáticos (pasta wwwroot)
            app.UseStaticFiles();

            // Adicionar o MVC.
            app.UseMvc(routes =>
            {
                // Permitir conventions-based routing.
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
