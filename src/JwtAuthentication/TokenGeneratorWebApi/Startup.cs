using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
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
            services.AddSwaggerGen(c =>
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TokenGeneratorWebApi", Version = "v1" }));

            string rsaPrivatePath = Configuration["SecurityOptions:RsaPrivateKeyPath"];
            string rsaPublicPath = Configuration["SecurityOptions:RsaPublicKeyPath"];
            services.AddRSAKey(options => {
                options.RsaPrivateKeyPath = rsaPrivatePath;
                options.RsaPublicKeyPath = rsaPublicPath;
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    var key = services.BuildServiceProvider().GetRequiredService<RsaSecurityKey>();
                    
                    options.IncludeErrorDetails = true;
                    options.TokenValidationParameters = new TokenValidationParameters {
                        IssuerSigningKey = key,
                        RequireAudience = true,
                        RequireExpirationTime = true,
                        ValidIssuer = "webapi",
                        ValidAudience = "webapi",
                        ValidateLifetime = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        RequireSignedTokens = true,
                        ValidateIssuer = true,
                        AuthenticationType = JwtBearerDefaults.AuthenticationScheme
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