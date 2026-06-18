using Microsoft.EntityFrameworkCore;
using Evote360.Infrastructure.Context;
using Evote360.Core.Interfaces;
using Evote360.Infrastructure.Repositories.Implementations;
using Evote360.Application.Interfaces;
using Evote360.Application.Services;
using Evote360.Application.Services.Interfaces;
using Evote360.Application.Services.Implementations;
using Evote360.Infrastructure.Services.Implementations;
using Evote360.Core.Entities;

namespace WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            
            builder.Services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Auth/Login";
                    options.AccessDeniedPath = "/Auth/AccessDenied";
                });

            // Register DbContext
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Register Generic Repository
            builder.Services.AddScoped(typeof(IRepositoryAsync<>), typeof(RepositoryAsync<>));

            // Register Specific Repositories
            builder.Services.AddScoped<IPuestoElectivoRepository, PuestoElectivoRepository>();
            builder.Services.AddScoped<ICiudadanoRepository, CiudadanoRepository>();
            builder.Services.AddScoped<IPartidoPoliticoRepository, PartidoPoliticoRepository>();
            builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            builder.Services.AddScoped<IAsignacionDirigenteRepository, AsignacionDirigenteRepository>();
            builder.Services.AddScoped<ICandidatoRepository, CandidatoRepository>();
            builder.Services.AddScoped<IEleccionRepository, EleccionRepository>();
            builder.Services.AddScoped<IAlianzaPoliticaRepository, AlianzaPoliticaRepository>();
            builder.Services.AddScoped<IAsignarCandidatoPuestoRepository, AsignarCandidatoPuestoRepository>();
            builder.Services.AddScoped<IVotoRepository, VotoRepository>();
            builder.Services.AddScoped<ICodigoVerificacionRepository, CodigoVerificacionRepository>();

            builder.Services.AddScoped<IRepositoryAsync<AsignacionDirigente>, RepositoryAsync<AsignacionDirigente>>();

            builder.Services.AddScoped<IRepositoryAsync<AsignarCandidatoPuesto>, RepositoryAsync<AsignarCandidatoPuesto>>();
            

            // registrar service
            builder.Services.AddScoped<IPartidoPoliticoService, PartidoPoliticoService>();
            builder.Services.AddScoped<ICiudadanoService, CiudadanoService>();
            builder.Services.AddScoped<IPuestoElectivoService, PuestoElectivoService>();

             builder.Services.AddScoped<IUsuarioService, UsuarioService>();
             builder.Services.AddScoped<IAlianzaPoliticaService, AlianzaPoliticaService>();

            builder.Services.AddScoped<IUsuarioService, UsuarioService>();
            builder.Services.AddScoped<IEleccionService, EleccionService>();
            builder.Services.AddScoped<IAsignarCandidatoPuestoService, AsignarCandidatoPuestoService>();



            builder.Services.AddScoped<IDirigenteService, DirigenteService>();


            builder.Services.AddScoped<IAdministradorService, AdministradorService>();




            builder.Services.AddScoped<IAdministradorService, AdministradorService>();



            builder.Services.AddScoped<IFileStorageService, FileStorageService>();
            builder.Services.AddScoped<IOcrService, OcrService>();
            builder.Services.AddScoped<IEmailService, Evote360.Infrastructure.Shared.Services.EmailService>();
            builder.Services.AddScoped<ICandidatoService, CandidatoService>();
            builder.Services.AddScoped<IAsignacionDirigentePoliticoService, AsignacionDirigentePoliticoService>();
            builder.Services.AddScoped<IVotacionService, VotacionService>();



            var app = builder.Build();

            // --- SEEDER DE USUARIO ADMIN POR DEFECTO ---
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<Evote360.Infrastructure.Context.ApplicationDbContext>();
                    
                    try 
                    {
                        context.Database.Migrate();
                    } 
                    catch 
                    {
                        // Se ignora si las tablas ya existen pero no el historial de EF
                    }

                    // Asegurar que siempre exista el usuario "admin" independientemente de otros usuarios
                    if (!context.Usuarios.Any(u => u.NombreUsuario == "admin"))
                    {
                        context.Usuarios.Add(new Evote360.Core.Entities.Usuario
                        {
                            Nombre = "Administrador",
                            Apellido = "Sistema",
                            Correo = "admin@evote360.com",
                            NombreUsuario = "admin",
                            ClaveHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                            Rol = Evote360.Core.Enums.RolUsuarioEnum.Administrador,
                            Estado = true
                        });
                        context.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Ocurrió un error inicializando la base de datos.");
                }
            }
            // -------------------------------------------

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles(); // permite guardar y leer fotos 
            app.UseRouting();

            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
