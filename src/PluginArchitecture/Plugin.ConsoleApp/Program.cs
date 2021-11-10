using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

IServiceCollection serviceCollection = new ServiceCollection();

void Dynamic(IServiceCollection services) {
    ModuleLoader.Load(services);

    IEnumerable<IAction> actions = services.BuildServiceProvider().GetServices<IAction>().ToList();
    IEnumerable<string> availableCommands = actions.GetActionNames();

    if (args.Length == 0) {
        Console.WriteLine();
        Console.WriteLine("Plugin.ConsoleApp");
        Console.WriteLine("-----------------");
        Console.WriteLine("Usage: Plugin.ConsoleApp [command]");
        Console.WriteLine();
        Console.WriteLine("Commands:");
        foreach (string availableCommand in availableCommands) Console.WriteLine($"\t{availableCommand}");
        return;
    }

    IAction? action = actions.PickAction(args[0]);
    if (action is null) {
        Console.WriteLine($"{args[0]} is not a valid command...");
        return;
    }

    action.Action();
}

void ManualSetup(IServiceCollection services) {
    // Initial step to create modules
    new WeatherModuleInjector().ConfigureServices(serviceCollection);
    new CountriesModuleInjector().ConfigureServices(serviceCollection);
    var weatherQuery = serviceCollection.BuildServiceProvider().GetService<IWeatherQuery>();
    weatherQuery?.Execute();

    var countriesQuery = serviceCollection.BuildServiceProvider().GetService<ICountriesQuery>();
    countriesQuery?.Execute();
}

void ManualParsing(string[] args) {
    if (args.Length == 0) {
        Console.WriteLine();
        Console.WriteLine("Plugin.ConsoleApp");
        Console.WriteLine("-----------------");
        Console.WriteLine("Usage: Plugin.ConsoleApp [command]");
        Console.WriteLine();
        Console.WriteLine("Commands:");
        Console.WriteLine("\tdisplay-weather");
        Console.WriteLine("\tdisplay-countries");
        Console.WriteLine("\tdisplay-google");
        return;
    }

    if (args[0] == "display-weather") {
        Console.WriteLine("visibility is good");
    } else if (args[0] == "display-countries") {
        
    }
}