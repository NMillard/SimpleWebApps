using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TokenGeneratorWebApi.Configurations;

namespace TokenGeneratorWebApi {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) {
            services.AddControllers();
            services.AddSwaggerGen(c =>
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TokenGeneratorWebApi", Version = "v1" }));

            var securityOptions = new SecurityOptions();
            Configuration.Bind(nameof(SecurityOptions), securityOptions);
            services.AddSingleton(securityOptions);

            services.AddRsaKeys(securityOptions);
            
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    var rsa = RSA.Create();
                    string key = File.ReadAllText(securityOptions.PublicKeyFilePath);
                    rsa.FromXmlString(key);
                    
                    options.TokenValidationParameters = new TokenValidationParameters {
                        ValidateAudience = true,
                        ValidateIssuer = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "webapi",
                        ValidAudience = "webapi",
                        IssuerSigningKey = new RsaSecurityKey(rsa)
                    };
                })
                .AddJwtBearer("symm", options => {
                    options.TokenValidationParameters = new TokenValidationParameters {
                        ValidateAudience = true,
                        ValidateIssuer = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "webapi",
                        ValidAudience = "webapi",
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityOptions.SymmetricKey))
                    };
                });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TokenGeneratorWebApi v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}