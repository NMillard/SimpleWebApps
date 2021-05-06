using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using WebApi.OutputFormatters;
using WebApi.Repositories;

namespace WebApi {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) {
            services.AddControllers().AddMvcOptions(options => {
                options.RespectBrowserAcceptHeader = true; // allows us to use OutputFormatters
                options.OutputFormatters.Add(new CsvOutputFormatter());
            });
            services.AddDbContext<AppDbContext>();
            services.AddScoped<UserRepository>();
            services.AddScoped<ArticleRepository>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.TokenValidationParameters = new TokenValidationParameters {
                        IssuerSigningKey =
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Auth:PrivateKey"])),
                        AuthenticationType = JwtBearerDefaults.AuthenticationScheme,
                        // Obvious bad practice to not check these properties below
                        ValidateLifetime = false,
                        ValidateIssuer = false,
                        ValidateAudience = false,
                    };
                });

            // Run migrations on startup - obviously also a very bad practice
            var db = services.BuildServiceProvider().GetRequiredService<AppDbContext>();
            db.Database.Migrate();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) { // Not a good idea to have dev and prod behaving differently
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}