using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WebApi.Exceptions;
using WebApi.OutputFormatters;
using WebApi.Repositories;

namespace WebApi {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) {
            services.AddControllers()
                .AddMvcOptions(options => {
                    options.Filters.Add(new ApplicationExceptionsFilter());
                    options.OutputFormatters.Add(new CsvOutputFormatter());
                });
            
            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApi", Version = "v1" });
                c.AddSecurityDefinition("jwt", new OpenApiSecurityScheme {
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    Description = "Use your JWT",
                    In = ParameterLocation.Header,
                    BearerFormat = "JWT"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = "jwt",
                            }
                        },
                        new List<string>()
                    }
                });
            });
            
            services.AddDbContext<AppDbContext>(config => {
                config.UseSqlServer(Configuration.GetConnectionString("default"));
            });

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

            // Run migrations on startup - this is NOT a best practice, but fine for development purposes
            var db = services.BuildServiceProvider().GetRequiredService<AppDbContext>();
            db.Database.Migrate();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                // bad practice to have dev and prod environments behave differently
                app.UseDeveloperExceptionPage();
                app.UseSwagger()
                    .UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApi v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}