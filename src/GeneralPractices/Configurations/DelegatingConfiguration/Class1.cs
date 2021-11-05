using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DelegatingConfiguration {
    /*
     * 1. Create the SomeSettings class
     * 2. Install Microsoft.Extensions.Configuration.Abstractions and
     *            Microsoft.Extensions.Configuration.Binder
     */


    /*
     * The class name is typically used as the object name in the configuration 
     */
    public class SomeSettings : ISettings {
        [Required]
        [MinLength(10)]
        public string ServiceBusTopic { get; set; }

        [Range(1, 10)]
        public int Max { get; set; }

        [MinLength(10)] public string Something { get; set; } = "hello";

        public void Verify() {
            if (string.IsNullOrEmpty(ServiceBusTopic)) throw new InvalidOperationException($"{nameof(ServiceBusTopic)} cannot be empty. This setting is used as the service bus topic we publish messages to.");
        }
    }

    public interface ISettings {
        void Verify();
    }

    public static class ConfigurationExtensions {
        
        /// <summary>
        /// Simply automatically bind and return the settings to the caller. 
        /// </summary>
        public static IConfiguration BindSimpleSettings<T>(this IConfiguration configuration, out T settings)
            where T : ISettings, new() {
            settings = new T();
            configuration.Bind(settings);

            var context = new ValidationContext(settings);
            var validationResults = new List<ValidationResult>();
            bool result = Validator.TryValidateObject(settings, context, validationResults, true);

            if (!result) throw new InvalidOperationException();
            
            // settings.Verify();

            return configuration;
        }
    }
    
    public static class ServiceInjector {
        public static IServiceCollection AddSomeService(this IServiceCollection services, Action<OtherSettings> configure) {
            var settings = new OtherSettings();
            configure(settings);
            
            settings.Verify();

            services
                .AddSingleton(settings)
                .AddScoped<SomeService>();
            
            return services;
        }

        public static IServiceCollection AddConfiguredSettings<TSettings>(this IServiceCollection services, IConfiguration configuration) where TSettings : class, ISettings, new() {
            configuration.BindSimpleSettings(out TSettings settings);
            services.AddSingleton(settings);
            
            return services;
        }

        public static IServiceCollection AddConfiguration<T>(this IServiceCollection services, Func<T> configure) where T : class {
            T settings = configure();
            services.AddSingleton(settings);
            
            return services;
        }
    }

    public class OtherSettings : ISettings {
        public string DefaultFilePath { get; set; }
        
        public void Verify() {
            if (string.IsNullOrEmpty(DefaultFilePath)) throw new InvalidOperationException();
        }
    }
    
    public class SomeService {
        private readonly OtherSettings settings;

        public SomeService(OtherSettings settings) {
            this.settings = settings;
        }

        public void PrintDefaultPath() => Console.WriteLine(settings.DefaultFilePath);
    }
}