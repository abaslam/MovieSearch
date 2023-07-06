
using MovieSearch.API.Core.Data.Entities;
using MovieSearch.API.Core.Infrstructure.Entities;
using Nest;
using System.Diagnostics;

namespace MovieSearch.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var appSettings = new AppSettings();
            builder.Configuration.GetSection(nameof(AppSettings)).Bind(appSettings);

            builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddMediatR(x => x.RegisterServicesFromAssembly(typeof(Program).Assembly));
            builder.Services.AddSingleton<IElasticClient>(x =>
            {

                var connectionSettings = new ConnectionSettings(new Uri(appSettings.ElasticSearchUrl))
                                            .DisableDirectStreaming(Debugger.IsAttached)
                                            .DefaultMappingFor<Movie>(x => x.IdProperty(y => y.Id));
                return new ElasticClient(connectionSettings);
            });


            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}