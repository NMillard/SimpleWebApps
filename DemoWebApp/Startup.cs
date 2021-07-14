using CompositionOverInheritance.Composition;
using FactoryPattern.Factories;
using FactoryPattern.Factories.FullyOpenClosed;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace DemoWebApp {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) {
            services.AddControllers();
            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "DemoWebApp", Version = "v1" });
            });

            services.AddSingleton<FactoryPattern.Factories.SwitchBased.ScheduleFactory>();
            services.AddSingleton<FactoryPattern.Factories.DictionaryBased.ScheduleFactory>(_ => {
                var scheduleFactory = new FactoryPattern.Factories.DictionaryBased.ScheduleFactory();
                
                scheduleFactory.RegisterSchedule("Daily", _ => new DailyTrigger());
                scheduleFactory.RegisterSchedule("EndOfWeek", _ => new EndOfWeekTrigger());
                scheduleFactory.RegisterSchedule("Weekly", parameters => {
                    var p = parameters as WeeklyParameters;
                    return new WeeklyTrigger(p.DayOfWeek);
                });
                
                return scheduleFactory;
            });
            
            services.AddSingleton<ScheduleFactoryProvider>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "DemoWebApp v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}