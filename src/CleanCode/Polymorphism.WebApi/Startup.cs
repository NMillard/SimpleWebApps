using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Polymorphism.WebApi.DataLayer;
using Polymorphism.WebApi.DataLayer.Repositories;
using Polymorphism.WebApi.Users.Commands;
using Polymorphism.WebApi.Users.Queries;

namespace Polymorphism.WebApi {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) {
            services.AddControllers();
            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Polymorphism.WebApi", Version = "v1" });
            });

            services.AddDbContext<AppDbContext>(options => {
                options.UseSqlServer(Configuration.GetConnectionString("sql"), builder => {
                    builder.MigrationsHistoryTable("_EFMigrationHistory", "Polymorphism");
                });
            });

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICreateUserCommand, CreateUserCommand>();
            services.AddScoped<IGetAllUsersQuery, GetAllUsersQuery>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Polymorphism.WebApi v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}