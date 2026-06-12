using Microsoft.EntityFrameworkCore;
using Evote360.Infrastructure.Context;
using Evote360.Core.Interfaces;
using Evote360.Infrastructure.Repositories.Implementations;
using Evote360.Application.Interfaces;
using Evote360.Application.Services;
using Evote360.Application.Services.Interfaces;
using Evote360.Application.Services.Implementations;
using Evote360.Infrastructure.Services.Implementations;

namespace WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

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

            // registrar service
            builder.Services.AddScoped<IPartidoPoliticoService, PartidoPoliticoService>();
            builder.Services.AddScoped<ICiudadanoService, CiudadanoService>();
            builder.Services.AddScoped<IPuestoElectivoService, PuestoElectivoService>();

            builder.Services.AddScoped<IFileStorageService, FileStorageService>();
            builder.Services.AddScoped<ICandidatoService, CandidatoService>();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

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
