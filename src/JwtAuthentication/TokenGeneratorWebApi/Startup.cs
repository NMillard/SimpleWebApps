using System.IO;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

/*
 * This web api will generate tokens
 * 
 */

namespace TokenGeneratorWebApi {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) {
            services.AddControllers();
            services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo { Title = "TokenGeneratorWebApi", Version = "v1" }));
            
            string rsaPrivatePath = Configuration["SecurityOptions:RsaPrivateKeyPath"];
            string rsaPublicPath = Configuration["SecurityOptions:RsaPublicKeyPath"];
            services.AddRSAKey(options => {
                options.RsaPrivateKeyPath = rsaPrivatePath;
                options.RsaPublicKeyPath = rsaPublicPath;
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(async options => {
                    var rsa = RSA.Create();
                    string keyContent = await File.ReadAllTextAsync(rsaPublicPath);
                    rsa.FromXmlString(keyContent);
                    
                    options.IncludeErrorDetails = true;
                    options.TokenValidationParameters = new TokenValidationParameters {
                        IssuerSigningKey = new RsaSecurityKey(rsa),
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        ValidateLifetime = true,
                        ValidAudience = "webapi",
                        ValidIssuer = "webapi",
                        RequireAudience = true,
                        RequireExpirationTime = true,
                        AuthenticationType = JwtBearerDefaults.AuthenticationScheme,
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

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}